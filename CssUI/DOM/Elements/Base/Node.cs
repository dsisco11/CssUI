using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.DOM.CustomElements;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Mutation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using xLog;

namespace CssUI.DOM.Nodes
{

    /// <summary>
    /// </summary>
    public abstract class Node : EventTarget, INode
    {/* Docs: https://dom.spec.whatwg.org/#interface-node */

        internal ILogger Log { get => nodeDocument.Log; }
        #region Properties
        private ENodeFlags nodeFlags = ENodeFlags.Clear;
        internal List<RegisteredObserver> RegisteredObservers = new List<RegisteredObserver>();
        public Document nodeDocument { get; internal set; }

        #region Abstracts
        public abstract ENodeType nodeType { get; }

        /* Docs: https://dom.spec.whatwg.org/#dom-node-nodename */
        public abstract string nodeName { get; }

        /* Docs: https://dom.spec.whatwg.org/#dom-node-nodevalue */
        [CEReactions] public abstract string nodeValue { get; set; }

        /* Docs: https://dom.spec.whatwg.org/#dom-node-textcontent */
        [CEReactions] public abstract string textContent { get; set; }

        /* Docs: https://dom.spec.whatwg.org/#concept-node-length */
        public abstract int nodeLength { get; }
        #endregion

        #region Slottable
        /// <summary>
        /// Returns whether this node has been assigned a slot
        /// </summary>
#if ENABLE_HTML
        public bool isAssigned => (this is ISlottable slotable && slotable.assignedSlot != null);
#else
        public bool isAssigned => false;
#endif
        #endregion

        #region DOM
        public Document ownerDocument
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-ownerdocument */
            get
            {
                return (this is Document) ? null : nodeDocument;
            }
        }
        public Node parentNode { get; private set; }
        public Element parentElement { get; private set; }
        public ChildNodeList childNodes { get; private set; }

        public Node firstChild { get => childNodes.Count > 0 ? childNodes[0] : null; }
        public Node lastChild { get => childNodes.Count > 0 ? childNodes[childNodes.Count - 1] : null; }

#if USE_FUNCTIONS_FOR_NODE_RELATIONSHIP_LINKS
        public Node previousSibling
        {
            get
            {
                if (parentNode is null) return null;
                if (index <= 0) return null;
                return parentNode.childNodes[index - 1];
            }
            internal set { }
        }
        public Node nextSibling
        {
            get
            {
                if (parentNode is null) return null;
                if (index >= parentNode.childNodes.Count-1) return null;
                return parentNode.childNodes[index + 1];
            }
            internal set { }
        }
#else
        public Node previousSibling { get; internal set; }
        public Node nextSibling { get; internal set; }
#endif

        /// <summary>
        /// Returns true if node is connected to a document and false otherwise.
        /// </summary>
        /// https://dom.spec.whatwg.org/#connected
        public bool isConnected { get => DOMCommon.Get_Shadow_Including_Root(this) is Document; }

        /// <summary>
        /// The index of this node within it's parent nodes child list
        /// </summary>
        /// Note: The index for nodes is now automatically assigned and updated by the ChildNodeList class
        public int index { get; internal set; }

        #endregion

        #region CSS
        /// <summary>
        /// The layout box for this element
        /// </summary>
        public CssBoxTreeNode Box { get; internal set; } = null;
        public StyleProperties Style { get; internal set; } = null;
        #endregion

        #endregion

        #region Constructors
        public Node()
        {
            childNodes = new ChildNodeList(this);
        }
        #endregion

        #region Flags
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetFlag(ENodeFlags Flags) { return (nodeFlags & Flags) != 0; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(ENodeFlags Flags) { nodeFlags |= Flags; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(ENodeFlags Flags, bool State) { nodeFlags = (nodeFlags & ~Flags) | ((ENodeFlags)(-(State ? 1 : 0)) & Flags); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(ENodeFlags Flags) { nodeFlags &= ~Flags; }

        /// <summary>
        /// Sets the given flags on all ancestors ending at the first ancestor which already has all said flags or the root of the tree
        /// </summary>
        /// <param name="Flags">Flags to set on each node</param>
        /// <param name="exclude_self"></param>
        public void Propagate_Flag(ENodeFlags Flags, bool exclude_self = true)
        {
            if (!exclude_self)
            {
                SetFlag(Flags);
            }

            Node current = parentNode;
            while (current is object)
            {
                if (current.GetFlag(Flags))
                    break;

                current.SetFlag(Flags);
                current = current.parentNode;
            }
        }

        /// <summary>
        /// Clears the given flags on all ancestors ending at the first ancestor which has any of the <paramref name="StopFlags"/> flags or at the root of the tree
        /// </summary>
        /// <param name="Flags">Flags to clear from each node</param>
        /// <param name="StopFlags">Flags which will cause propagation to end</param>
        /// <param name="exclude_self"></param>
        public void Unpropagate_Flag(ENodeFlags Flags, ENodeFlags StopFlags, bool exclude_self = true)
        {
            if (!exclude_self)
            {
                ClearFlag(Flags);
            }

            Node current = parentNode;
            while (current is object)
            {
                if (current.GetFlag(StopFlags))
                    break;

                current.ClearFlag(Flags);
                current = current.parentNode;
            }
        }
        #endregion

        /// <summary>
        /// Returns node’s root.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Node getRootNode(GetRootNodeOptions options = null)
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-getrootnode */
            /* The getRootNode(options) method, when invoked, must return context object’s shadow-including root if options’s composed is true, and context object’s root otherwise. */
            if (options != null && options.composed)
            {
                DOMCommon.Get_Shadow_Including_Root(this);
            }

            return DOMCommon.Get_Root(this);
        }

        /// <summary>
        /// Returns whether node has children.
        /// </summary>
        /// <returns></returns>
        public bool hasChildNodes() => childNodes.Count > 0;

        [CEReactions]
        public void normalize()
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-normalize */
            CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                /* The normalize() method, when invoked, must run these steps for each descendant exclusive Text node node of context object: */
                foreach (Text node in DOMCommon.Get_Descendents(this, new TextNodeFilter()))
                {
                    var length = node.Length;
                    /* 2) If length is zero, then remove node and continue with the next exclusive Text node, if any. */
                    if (length <= 0)
                    {
                        Dom_remove_node_from_parent(node, this);
                        continue;
                    }
                    /* 3) Let data be the concatenation of the data of node’s contiguous exclusive Text nodes (excluding itself), in tree order. */
                    var textNodes = Text.get_contiguous_text_nodes(node, true);
                    StringBuilder sb = new StringBuilder();
                    foreach (Text txt in textNodes)
                    {
                        sb.Append(txt.textContent);
                    }
                    string data = sb.ToString();
                    /* 4) Replace data with node node, offset length, count 0, and data data. */
                    node.replaceData(length, 0, data);
                    /* 5) Let currentNode be node’s next sibling. */
                    var currentNode = node.nextSibling;
                    /* 6) While currentNode is an exclusive Text node: */
                    while (currentNode is Text)
                    {

                        foreach (WeakReference<Range> weakRef in nodeDocument.LIVE_RANGES)
                        {
                            if (weakRef.TryGetTarget(out Range liveRange))
                            {
                                /* 1) For each live range whose start node is currentNode, add length to its start offset and set its start node to node. */
                                if (ReferenceEquals(currentNode, liveRange.startContainer))
                                {
                                    liveRange.startContainer = node;
                                    liveRange.startOffset += length;
                                }
                                /* 2) For each live range whose end node is currentNode, add length to its end offset and set its end node to node. */
                                if (ReferenceEquals(currentNode, liveRange.endContainer))
                                {
                                    liveRange.endContainer = node;
                                    liveRange.endOffset += length;
                                }
                                /* 3) For each live range whose start node is currentNode’s parent and start offset is currentNode’s index, set its start node to node and its start offset to length. */
                                if (ReferenceEquals(currentNode.parentNode, liveRange.startContainer) && liveRange.startOffset == currentNode.index)
                                {
                                    liveRange.startContainer = node;
                                    liveRange.startOffset += length;
                                }
                                /* 4) For each live range whose end node is currentNode’s parent and end offset is currentNode’s index, set its end node to node and its end offset to length. */
                                if (ReferenceEquals(currentNode.parentNode, liveRange.endContainer) && liveRange.endOffset == currentNode.index)
                                {
                                    liveRange.endContainer = node;
                                    liveRange.endOffset += length;
                                }
                            }
                        }
                        /* 5) Add currentNode’s length to length. */
                        length += currentNode.nodeLength;
                        /* 6) Set currentNode to its next sibling. */
                        currentNode = currentNode.nextSibling;
                    }
                    /* 7) Remove node’s contiguous exclusive Text nodes (excluding itself), in tree order. */
                    foreach (Node cnode in textNodes)
                    {
                        Dom_remove_node_from_parent(cnode, this);
                    }
                }
            });
        }

        [CEReactions]
        public Node cloneNode(bool deep = false)
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-clonenode */
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                /* 1) If context object is a shadow root, then throw a "NotSupportedError" DOMException. */
                if (this is ShadowRoot)
                {
                    throw new NotSupportedError("Cannot clone a shadow-root");
                }

                /* 2) Return a clone of the context object, with the clone children flag set if deep is true. */
                /* Docs: https://dom.spec.whatwg.org/#concept-node-clone */
                return Dom_clone_node(this, null, deep);
            });
        }

        public EDocumentPosition compareDocumentPosition(Node other)
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-comparedocumentposition */
            if (ReferenceEquals(this, other))
                return 0x0;

            Node node1 = other;
            Node node2 = this;
            Attr attr1 = null, attr2 = null;
            if (node1 is Attr)
            {
                attr1 = (Attr)node1;
                node1 = attr1.ownerElement;
            }
            /* 5) If node2 is an attribute, then: */
            if (node2 is Attr)
            {
                attr2 = (Attr)node2;
                node2 = attr2.ownerElement;
                /* 2) If attr1 and node1 are non-null, and node2 is node1, then: */
                if (node1 != null && attr1 != null && ReferenceEquals(node2, node1))
                {
                    foreach (var attr in (node2 as Element).AttributeList)
                    {
                        if (attr.Equals(attr1))
                            return EDocumentPosition.PRECEDING | EDocumentPosition.IMPLEMENTATION_SPECIFIC;
                        if (attr.Equals(attr2))
                            return EDocumentPosition.FOLLOWING | EDocumentPosition.IMPLEMENTATION_SPECIFIC;
                    }
                }
            }

            /* 6) If node1 or node2 is null, or node1’s root is not node2’s root, then return the result of adding DOCUMENT_POSITION_DISCONNECTED, DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC, and either DOCUMENT_POSITION_PRECEDING or DOCUMENT_POSITION_FOLLOWING, with the constraint that this is to be consistent, together. */
            if (node1 == null || node2 == null || !ReferenceEquals(node1.getRootNode(), node2.getRootNode()))
            {
                return EDocumentPosition.DISCONNECTED | EDocumentPosition.IMPLEMENTATION_SPECIFIC | EDocumentPosition.PRECEDING;
            }

            /* 7) If node1 is an ancestor of node2 and attr1 is null, or node1 is node2 and attr2 is non-null, then return the result of adding DOCUMENT_POSITION_CONTAINS to DOCUMENT_POSITION_PRECEDING. */
            if ((DOMCommon.Is_Ancestor(node1, node2) && attr1 == null) || (ReferenceEquals(node1, node2) && attr2 != null))
            {
                return EDocumentPosition.CONTAINS | EDocumentPosition.PRECEDING;
            }

            /* 8) If node1 is a descendant of node2 and attr2 is null, or node1 is node2 and attr1 is non-null, then return the result of adding DOCUMENT_POSITION_CONTAINED_BY to DOCUMENT_POSITION_FOLLOWING. */
            if ((DOMCommon.Is_Descendant(node1, node2) && attr2 == null) || (ReferenceEquals(node1, node2) && attr1 != null))
            {
                return EDocumentPosition.CONTAINED_BY | EDocumentPosition.FOLLOWING;
            }

            /* 9) If node1 is preceding node2, then return DOCUMENT_POSITION_PRECEDING. */
            if (DOMCommon.Is_Preceeding(node1, node2))
                return EDocumentPosition.PRECEDING;
            /* 10) Return DOCUMENT_POSITION_FOLLOWING. */
            return EDocumentPosition.FOLLOWING;
        }

        /// <summary>
        /// Returns true if <paramref name="other"/> is an inclusive descendant of this node, and false otherwise.
        /// </summary>
        public bool contains(Node other)
        {
            return DOMCommon.Is_Inclusive_Descendant(other, this);
        }

        /// <summary>
        /// Inserts the given node at the front of this nodes children
        /// </summary>
        /// <param name="newNode"></param>
        /// <returns></returns>
        [CEReactions]
        internal Node insertFirst(Node newNode)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                return Dom_pre_insert_node(newNode, this, null);
            });
        }

        /// <summary>
        /// Inserts the given node into this nodes children infront of <paramref name="before"/>
        /// </summary>
        /// <param name="newNode"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        [CEReactions]
        public Node insertBefore(Node newNode, Node before)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                return Dom_pre_insert_node(newNode, this, before);
            });
        }

        [CEReactions]
        public Node appendChild(Node node)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                return Dom_pre_insert_node(node, this, null);
            });
        }

        [CEReactions]
        public Node replaceChild(Node node, Node child)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                return Dom_replace_node_within_parent(node, this, child);
            });
        }

        [CEReactions]
        public Node removeChild(Node child)
        {
            return CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                return Dom_pre_remove_node(child, this);
            });
        }


        #region Equality
        public bool isEqualNode(Node otherNode)
        {
            if (otherNode == null)
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {/* https://dom.spec.whatwg.org/#concept-node-equals */
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (!(obj is Node otherNode))
                return false;
            /* A and B’s nodeType attribute value is identical. */
            if (nodeType != otherNode.nodeType)
                return false;

            /* A and B have the same number of children. */
            if (childNodes.Count != otherNode.childNodes.Count)
                return false;
            /* Each child of A equals the child of B at the identical index. */
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (!childNodes[i].isEqualNode(otherNode.childNodes[i]))
                    return false;
            }

            return isEqualNode(otherNode);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (int)nodeType;
            return hash;
        }
        #endregion

        #region Internal
        /// <summary>
        /// Returns <c>True</c> is the node is the root of its node-tree.
        /// </summary>
        public bool isRoot => (parentNode is null);
        /// <summary>
        /// A node (in particular elements and text nodes) can be marked as inert. 
        /// When a node is inert, then the user agent must act as if the node was absent for the purposes of targeting user interaction events, 
        /// may ignore the node for the purposes of text search user interfaces (commonly known as "find in page"), 
        /// and may prevent the user from selecting text in that node. User agents should allow the user to override the restrictions on search and text selection, however.
        /// </summary>
        internal bool isInert = false;
        internal bool isExpresslyInert
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#expressly-inert */
            /* An element is expressly inert if it is inert and its node document is not inert. */
            get => (isInert && !nodeDocument.isInert);
        }
        #endregion

        #region Event Stuff
        public override EventTarget get_the_parent(Event @event)
        {
#if ENABLE_HTML
            /* A node’s get the parent algorithm, given an event, returns the node’s assigned slot, if node is assigned, and node’s parent otherwise. */
            if (this is ISlottable slottable && slottable.isAssigned)
                return (EventTarget)slottable.assignedSlot;
#endif
            return parentNode;
        }
        #endregion

        #region Customizable Overload Steps
        /// <summary>
        /// Runs the W3C specification defined steps on a given parent node for when a child <see cref="Text"/> node changes
        /// </summary>
        /// <param name="parent"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void Run_child_text_node_change_steps(Node node)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void Run_node_insertion_steps(Node node)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void Run_node_removing_steps(Node node)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void Run_cloning_steps(ref Node copy, Document document, bool clone_children = false)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        #endregion

        #region Internal Utilitys
        internal virtual bool Is_ShadowHost
        {/* Docs: https://dom.spec.whatwg.org/#element-shadow-host */
#if ENABLE_HTML
            get => this is Element asElement && asElement.shadowRoot is object;
#else
            get => false;
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Node Dom_clone_node(Node node, Document document = null, bool clone_children = false, Node targetNode = null)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-clone */
            /* 1) If document is not given, let document be node’s node document. */
            if (document is null)
            {
                document = node.ownerDocument;
            }

            /* 2) If node is an element, then: */
            Node copy = targetNode;
            if (node is Element element)
            {
                if (copy is null)
                {
                    copy = DOMCommon.Create_Element(document, element.localName, element.NamespaceURI, element.prefix, element.is_value, false);
                }
                else
                {
                    (copy as Element).localName = element.localName;
                }

                foreach (Attr attr in element.AttributeList)
                {
                    Attr copyAttribute = (Attr)attr.cloneNode();
                    (copy as Element).append_attribute(copyAttribute);
                }
            }
            /* 3) Otherwise, let copy be a node that implements the same interfaces as node, and fulfills these additional requirements, switching on node: */
            else
            {
                /* Ensure copy is an instance */
                if (copy is null)
                {
                    switch (node.nodeType)
                    {
                        case ENodeType.DOCUMENT_NODE:
                            {
                                if (node is HTMLDocument)
                                {
                                    copy = new HTMLDocument((node as Document).contentType);
                                }
                                else
                                {
                                    copy = new XMLDocument((node as Document).contentType);
                                }
                            }
                            break;
                        case ENodeType.DOCUMENT_TYPE_NODE:
                            {
                                var doc = ((DocumentType)node);
                                copy = new DocumentType(doc.name, doc.publicId, doc.systemId);
                            }
                            break;
                        case ENodeType.ATTRIBUTE_NODE:
                            {
                                var natr = (Attr)node;
                                copy = new Attr(natr.Name, document, natr.namespaceURI) { Value = natr.Value };
                            }
                            break;
                        default:
                            {
                                if (node is Text)
                                {
                                    copy = new Text(document, ((Text)node).data);
                                }
                                else if (node is Comment comment)
                                {
                                    copy = new Comment(document, comment.data);
                                }
                                else if (node is ProcessingInstruction processingInstruction)
                                {
                                    copy = new ProcessingInstruction(document, processingInstruction.target, processingInstruction.data);
                                }
                                else
                                {
                                    if (copy is null)
                                    {
                                        throw new InvalidNodeTypeError(ExceptionMessages.ERROR_Node_No_Cloning_Logic);
                                    }
                                    else
                                    {
                                        node.CopyTo(ref copy);
                                    }
                                }
                            }
                            break;
                    }
                }

                /* Copy any required data */
                switch (node.nodeType)
                {
                    case ENodeType.DOCUMENT_TYPE_NODE:
                        {
                            var doc = ((DocumentType)node);
                            (copy as DocumentType).name = doc.name;
                            (copy as DocumentType).publicId = doc.publicId;
                            (copy as DocumentType).systemId = doc.systemId;
                        }
                        break;
                    case ENodeType.ATTRIBUTE_NODE:
                        {
                            var natr = (Attr)node;
                            (copy as Attr).Value = natr.Value;
                        }
                        break;
                    default:
                        {
                            if (node is ProcessingInstruction processingInstruction)
                            {
                                (copy as ProcessingInstruction).target = processingInstruction.target;
                                (copy as ProcessingInstruction).data = processingInstruction.data;
                            }
                            else if (node is CharacterData charNode)
                            {
                                (copy as CharacterData).data = charNode.data;
                            }
                        }
                        break;
                }
            }
            /* 4) Set copy’s node document and document to copy, if copy is a document, and set copy’s node document to document otherwise. */
            if (copy is Document)
            {
                copy.nodeDocument = (Document)copy;
            }
            else
            {
                copy.nodeDocument = document;
            }

            /* 5) Run any cloning steps defined for node in other applicable specifications and pass copy, node, document and the clone children flag if set, as parameters. */
            node.Run_cloning_steps(ref copy, document, clone_children);
            /* 5.1) Also lets run CssUIs CopyTo function */
            node.CopyTo(ref copy, true);

            /* 6) If the clone children flag is set, clone all the children of node and append them to copy, with document as specified and the clone children flag being set */
            if (clone_children)
            {
                foreach (Node child in node.childNodes)
                {
                    var childCopy = child.cloneNode(clone_children);
                    copy.appendChild(childCopy);
                }
            }

            return copy;
        }

        /* Do NOT inline this function, its too large and used too often */
        private static void Dom_remove_node_from_parent(Node node, Node parent, bool suppress_observers = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-remove */
            /* 1) Let index be node’s index. */
            int index = node.index;
            foreach (WeakReference<Range> weakRef in node.nodeDocument.LIVE_RANGES)
            {
                if (weakRef.TryGetTarget(out Range liveRange))
                {
                    /* 2) For each live range whose start node is an inclusive descendant of node, set its start to (parent, index). */
                    if (DOMCommon.Is_Inclusive_Descendant(liveRange.startContainer, node))
                    {
                        liveRange.startContainer = parent;
                        liveRange.startOffset = index;
                    }
                    /* 3) For each live range whose end node is an inclusive descendant of node, set its end to (parent, index). */
                    if (DOMCommon.Is_Inclusive_Descendant(liveRange.endContainer, node))
                    {
                        liveRange.endContainer = parent;
                        liveRange.endOffset = index;
                    }
                    /* 4) For each live range whose start node is parent and start offset is greater than index, decrease its start offset by 1. */
                    if (ReferenceEquals(liveRange.startContainer, parent) && liveRange.startOffset > index)
                    {
                        liveRange.startOffset--;
                    }
                    /* 5) For each live range whose end node is parent and end offset is greater than index, decrease its end offset by 1. */
                    if (ReferenceEquals(liveRange.endContainer, parent) && liveRange.endOffset > index)
                    {
                        liveRange.endOffset--;
                    }
                }
            }
            /* 6) For each NodeIterator object iterator whose root’s node document is node’s node document, run the NodeIterator pre-removing steps given node and iterator. */
            foreach (var weakRef in NodeIterator.ALL)
            {
                if (weakRef.TryGetTarget(out NodeIterator iter))
                {
                    if (ReferenceEquals(node.ownerDocument, iter.root.ownerDocument))
                    {
                        NodeIterator.pre_removing_steps(iter, node);
                    }
                }
            }
            /* 7) Let oldPreviousSibling be node’s previous sibling. */
            var previousSibling = node.previousSibling;
            /* 8) Let oldNextSibling be node’s next sibling. */
            var nextSibling = node.nextSibling;
            /* 9) Remove node from its parent’s children. */
            parent.childNodes.Remove(node);
            /* 10) If node is assigned, then run assign slotables for node’s assigned slot. */

#if ENABLE_HTML
            if (node.isAssigned)
            {
                DOMCommon.Assign_Slottables((node as ISlottable).assignedSlot);
            }
#endif
            /* 11) If parent’s root is a shadow root, and parent is a slot whose assigned nodes is the empty list, then run signal a slot change for parent. */
            Node parentRoot = parent.getRootNode();
            if (parentRoot is ShadowRoot && parent is ISlot parentSlot && !parentSlot.assignedNodes().Any())
            {
                parentSlot.Signal_Slot_Change();
            }

            /* 12) If node has an inclusive descendant that is a slot, then: */
            if (DOMCommon.Get_Inclusive_Descendents(node).Where(child => child is ISlot).Any())
            {
                /* 1) Run assign slotables for a tree with parent’s root. */
                DOMCommon.Assign_Slottables_For_Tree(parentRoot);
                /* 2) Run assign slotables for a tree with node. */
                DOMCommon.Assign_Slottables_For_Tree(node);
            }

            /* 13) Run the removing steps with node and parent. */
            parent.Run_node_removing_steps(node);

            /* 14) If node is custom, then enqueue a custom element callback reaction with node, callback name "disconnectedCallback", and an empty argument list. */
            if (node is Element nodeElement)
            {
                CEReactions.Enqueue_Reaction(nodeElement, EReactionName.Disconnected, Array.Empty<object>());
            }

            /* 15) For each shadow-including descendant descendant of node, in shadow-including tree order, then: */
            foreach (Node descendant in DOMCommon.Get_Shadow_Including_Descendents(node))
            {
                /* 1) Run the removing steps with descendant. */
                Dom_remove_node_from_parent(descendant, node);
                /* 2) If descendant is custom, then enqueue a custom element callback reaction with descendant, callback name "disconnectedCallback", and an empty argument list. */
                if (descendant is Element childElement)
                {
                    CEReactions.Enqueue_Reaction(childElement, EReactionName.Disconnected, Array.Empty<object>());
                }
            }

            /* 16) For each inclusive ancestor inclusiveAncestor of parent, and then for each registered of inclusiveAncestor’s registered observer list, 
             * if registered’s options’s subtree is true, then append a new transient registered observer whose observer is registered’s observer, 
             * options is registered’s options, and source is registered to node’s registered observer list. */
            var ancestors = DOMCommon.Get_Inclusive_Ancestors(parent);
            foreach (Node inclusiveAncestor in ancestors)
            {
                foreach (RegisteredObserver registered in inclusiveAncestor.RegisteredObservers)
                {
                    if (registered.options.subtree)
                    {
                        var tro = new TransientRegisteredObserver(registered, registered.observer, registered.options);
                        node.RegisteredObservers.Add(tro);
                    }
                }
            }
            /* 17) If suppress observers flag is unset, then queue a tree mutation record for parent with « », « node », oldPreviousSibling, and oldNextSibling. */
            if (!suppress_observers)
            {
                MutationRecord.Queue_Tree_Mutation_Record(parent, Array.Empty<Node>(), new Node[] { node }, previousSibling, nextSibling);
            }
            /* 18) If node is a Text node, then run the child text content change steps for parent. */
            if (node is Text nodeAsText)
            {
                parent.Run_child_text_node_change_steps(nodeAsText);
            }

        }

        /// <summary>
        /// Inserts the given <paramref name="node"/> into <paramref name="parent"/> before the given <paramref name="child"/>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="suppress_observers"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Dom_insert_node_into_parent_before(Node node, Node parent, Node child, bool suppress_observers = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-insert */
            /* 1) Let count be the number of children of node if it is a DocumentFragment node, and one otherwise. */
            int count = (node is DocumentFragment doc) ? doc.childNodes.Count : 1;
            /* 2) If child is non-null, then: */
            if (child is object)
            {
                foreach (WeakReference<Range> weakRef in parent.nodeDocument.LIVE_RANGES)
                {
                    if (weakRef.TryGetTarget(out Range liveRange))
                    {
                        /* 1) For each live range whose start node is parent and start offset is greater than child’s index, increase its start offset by count. */
                        if (ReferenceEquals(parent, liveRange.startContainer) && liveRange.startOffset > child.index)
                            liveRange.startOffset += count;
                        /* 2) For each live range whose end node is parent and end offset is greater than child’s index, increase its end offset by count. */
                        if (ReferenceEquals(parent, liveRange.endContainer) && liveRange.endOffset > child.index)
                            liveRange.endOffset += count;
                    }
                }
            }
            /* 3) Let nodes be node’s children, if node is a DocumentFragment node; otherwise « node ». */
            IEnumerable<Node> nodes = (node is DocumentFragment doc1) ? doc1.childNodes : (IEnumerable<Node>)Array.Empty<Node>();
            /* 4) If node is a DocumentFragment node, remove its children with the suppress observers flag set. */
            if (node is DocumentFragment doc2)
            {
                foreach (Node cn in doc2.childNodes)
                {
                    Dom_remove_node_from_parent(cn, doc2, true);
                }
                /* 5) If node is a DocumentFragment node, then queue a tree mutation record for node with « », nodes, null, and null. */
                MutationRecord.Queue_Tree_Mutation_Record(node, Array.Empty<Node>(), nodes, null, null);
            }
            /* 6) Let previousSibling be child’s previous sibling or parent’s last child if child is null. */
            var previousSibling = child is null ? parent.lastChild : child.previousSibling;
            int childIndex = child is null ? 0 : child.index;
            /* 7) For each node in nodes, in tree order: */
            foreach (Node newNode in nodes)
            {
                /* 1) If child is null, then append node to parent’s children. */
                if (child is null)
                {
                    parent.childNodes.Add(newNode);
                }
                /* 2) Otherwise, insert node into parent’s children before child’s index. */
                else
                {
                    parent.childNodes.Insert(childIndex, newNode);
                    // Increment childIndex so its position is correct
                    childIndex++;
                }
                /* 3) If parent is a shadow host and node is a slotable, then assign a slot for node. */
                if (parent.Is_ShadowHost && node is ISlottable)
                {
                    DOMCommon.Assign_A_Slot(node as ISlottable);
                }

                /* 4) If node is a Text node, run the child text content change steps for parent. */
                if (node is Text nodeAsText)
                {
                    parent.Run_child_text_node_change_steps(nodeAsText);
                }
                /* 5) If parent’s root is a shadow root, and parent is a slot whose assigned nodes is the empty list, then run signal a slot change for parent. */
                if (parent.getRootNode().Is_ShadowHost && parent is ISlot parentSlot && !parentSlot.assignedNodes().Any())
                {
                    parentSlot.Signal_Slot_Change();
                }

                /* 6) Run assign slotables for a tree with node’s root. */
                DOMCommon.Assign_Slottables_For_Tree(node.getRootNode());

                /* 7) For each shadow-including inclusive descendant inclusiveDescendant of node, in shadow-including tree order: */
                var newNodeDescendents = DOMCommon.Get_Shadow_Including_Inclusive_Descendents(newNode);
                foreach (Node inclusiveDescendant in newNodeDescendents)
                {
                    /* 1) Run the insertion steps with inclusiveDescendant. */
                    parent.Run_node_insertion_steps(inclusiveDescendant);
                    /* 2) If inclusiveDescendant is connected, then: */
                    if (inclusiveDescendant.isConnected)
                    {
#if ENABLE_HTML
                        /* 1) If inclusiveDescendant is custom, then enqueue a custom element callback reaction with inclusiveDescendant, callback name "connectedCallback", and an empty argument list. */
                        CEReactions.Enqueue_Reaction(inclusiveDescendant as Element, EReactionName.Connected, Array.Empty<object>());
                        /* 2) Otherwise, try to upgrade inclusiveDescendant. */
                        CEReactions.Try_Upgrade_Element(inclusiveDescendant as Element);
#endif
                    }
                }
            }

            /* 8) If suppress observers flag is unset, then queue a tree mutation record for parent with nodes, « », previousSibling, and child. */
            if (!suppress_observers)
            {
                MutationRecord.Queue_Tree_Mutation_Record(parent, nodes, Array.Empty<Node>(), previousSibling, child);
            }
        }

        /// <summary>
        /// Replaces a specified node within a parent with a new one
        /// </summary>
        /// <param name="node">The new node being inserted</param>
        /// <param name="parent">Parent node which the new node will be inserted into</param>
        /// <param name="child">The child node to replace</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Node Dom_replace_node_within_parent(Node node, Node parent, Node child)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-replace */
            if (!(parent is Document) && !(parent is DocumentFragment) && !(parent is Element))
            {
                throw new HierarchyRequestError(ExceptionMessages.ERROR_Node_Parent_Type_Invalid);
            }

            if (DOMCommon.Is_Host_Including_Inclusive_Ancestor(node, parent))
            {
                throw new HierarchyRequestError(ExceptionMessages.ERROR_Cannot_Insert_Node_Due_To_Recursion);
            }

            if (!ReferenceEquals(child.parentNode, parent))
            {
                throw new NotFoundError();
            }

            if (!(node is DocumentFragment) && !(node is DocumentType) && !(node is Element) && !(node is Text) && !(node is ProcessingInstruction) && !(node is Comment))
            {
                throw new HierarchyRequestError("Node is not a valid type to insert into parent.");
            }

            if ((node is Text && parent is Document) || (node is DocumentType && !(parent is Document)))
            {
                throw new HierarchyRequestError();
            }

            if (parent is Document)
            {
                if (node is DocumentFragment)
                {
                    int eCount = node.childNodes.Count(c => c is Element);
                    if (eCount > 1 || node.childNodes.Any(c => c is Text))
                    {
                        throw new HierarchyRequestError();
                    }
                    else
                    {
                        /* Otherwise, if node has one element child and either parent has an element child that is not child or a doctype is following child. */
                        var eChildren = parent.childNodes.Where(c => c is Element);
                        if (eCount == 1 && (eChildren.Any(c => !ReferenceEquals(c, child))))
                        {
                            throw new HierarchyRequestError();
                        }
                        else if (eCount == 1 && DOMCommon.Get_Following(child).Any(c => c is DocumentType))
                        {
                            throw new HierarchyRequestError();
                        }
                    }
                }
            }
            else if (node is Element)
            {
                /* parent has an element child that is not child or a doctype is following child. */
                var eChildren = parent.childNodes.Where(c => c is Element);
                if (eChildren.Any(c => !ReferenceEquals(c, child)))// parent has element that is not child
                {
                    throw new HierarchyRequestError();
                }
                else if (DOMCommon.Get_Following(child).Any(c => c is DocumentType))// doctype is following child
                {
                    throw new HierarchyRequestError();
                }
            }
            else if (node is DocumentType)
            {
                /* parent has a doctype child that is not child, or an element is preceding child. */
                var eChildren = parent.childNodes.Where(c => c is DocumentType);
                if (eChildren.Any(c => !ReferenceEquals(c, child)))// parent has a doctype that is not child
                {
                    throw new HierarchyRequestError();
                }
                else if (DOMCommon.Get_Preceeding(child, FilterElements.Instance).Count > 0)// element is preceeding child
                {
                    throw new HierarchyRequestError();
                }
            }

            /* 7) Let reference child be child’s next sibling. */
            var referenceChild = child.nextSibling;
            /* 8) If reference child is node, set it to node’s next sibling. */
            if (ReferenceEquals(node, referenceChild))
                referenceChild = node.nextSibling;

            var previousSibling = child.previousSibling;
            /* 10) Adopt node into parent’s node document. */
            parent.ownerDocument.adoptNode(node);
            /* 11) Let removedNodes be the empty list. */
            var removedNodes = Array.Empty<Node>();
            /* 12) If child’s parent is not null, then: */
            if (child.parentNode is object)
            {
                /* 1) Set removedNodes to « child ». */
                removedNodes = new Node[] { child };
                Dom_remove_node_from_parent(child, child.parentNode, true);
            }
            /* 13) Let nodes be node’s children if node is a DocumentFragment node; otherwise « node ». */
            IEnumerable<Node> nodes = (node is DocumentFragment) ? node.childNodes : (IEnumerable<Node>)new Node[] { node };
            /* 14) Insert node into parent before reference child with the suppress observers flag set. */
            Dom_insert_node_into_parent_before(node, parent, referenceChild, true);
            /* 15) Queue a tree mutation record for parent with nodes, removedNodes, previousSibling, and reference child. */
            MutationRecord.Queue_Tree_Mutation_Record(parent, nodes, removedNodes, previousSibling, referenceChild);
            return child;
        }

        /// <summary>
        /// Removes all children within a parent element replacing them with the specified node.
        /// </summary>
        /// <param name="node">The node which will replace all others</param>
        /// <param name="parent">The parent node whose children are being replaced</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Dom_replace_all_within_node(Node node, Node parent)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-replace-all */
            /* 1) If node is not null, adopt node into parent’s node document. */
            if (node is object)
            {
                parent.ownerDocument.adoptNode(node);
            }

            IEnumerable<Node> removedNodes = parent.childNodes;
            IEnumerable<Node> addedNodes = Array.Empty<Node>();

            if (node is DocumentFragment)
                addedNodes = node.childNodes;
            else if (node is object)
                addedNodes = new Node[] { node };
            /* 6) Remove all parent’s children, in tree order, with the suppress observers flag set. */
            foreach (Node child in parent.childNodes)
            {
                Dom_remove_node_from_parent(child, parent, true);
            }
            /* 7) If node is not null, then insert node into parent before null with the suppress observers flag set. */
            if (node is object)
            {
                Dom_insert_node_into_parent_before(node, parent, null, true);
            }
            /* 8) Queue a tree mutation record for parent with addedNodes, removedNodes, null, and null. */
            MutationRecord.Queue_Tree_Mutation_Record(parent, addedNodes, removedNodes, null, null);
        }

        /// <summary>
        /// Ensures the given <paramref name="node"/>, <paramref name="parent"/>, and <paramref name="child"/> are valid types for insertion
        /// </summary>
        /// <param name="node">Node being inserted</param>
        /// <param name="parent">Parent node which is being inserted into</param>
        /// <param name="child">Child node being used as a reference point for insertion</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Dom_ensure_pre_insertion_validity(Node node, Node parent, Node child)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-ensure-pre-insertion-validity */
            if (!(parent is Document) && !(parent is DocumentFragment) && !(parent is Element))
                throw new HierarchyRequestError("Parent node is not a valid type!");
            /* 2) If node is a host-including inclusive ancestor of parent, throw a "HierarchyRequestError" DOMException. */
            if (DOMCommon.Is_Host_Including_Inclusive_Ancestor(node, parent))
                throw new HierarchyRequestError();

            /* 3) If child is not null and its parent is not parent, then throw a "NotFoundError" DOMException. */
            if (child is object && !ReferenceEquals(parent, child.parentNode))
                throw new NotFoundError();
            /* 4) If node is not a DocumentFragment, DocumentType, Element, Text, ProcessingInstruction, or Comment node, throw a "HierarchyRequestError" DOMException. */
            if (!(node is DocumentFragment) && !(node is DocumentType) && !(node is Element) && !(node is Text) && !(node is ProcessingInstruction) && !(node is Comment))
                throw new HierarchyRequestError("Node does not inherit from any element base types.");

            /* 5) If either node is a Text node and parent is a document, or node is a doctype and parent is not a document, throw a "HierarchyRequestError" DOMException. */
            if ((node is Text && parent is Document) || (node is DocumentType && !(parent is Document)))
                throw new HierarchyRequestError();
            /* 6) If parent is a document, and any of the statements below, switched on node, are true, throw a "HierarchyRequestError" DOMException. */
            if (parent is Document)
            {
                if (node is DocumentFragment docFrag)
                {
                    /* 
                     * If node has more than one element child or has a Text node child.
                     * Otherwise, if node has one element child and either parent has an element child, child is a doctype, or child is not null and a doctype is following child. 
                     */
                    var eCount = docFrag.childNodes.Count(c => c is Element);
                    if (eCount > 1 || docFrag.childNodes.Any(c => c is Text))
                    {
                        throw new HierarchyRequestError();
                    }
                    else if (eCount == 1)
                    {
                        if (parent.childNodes.Any(c => c is Element) || child is DocumentType)
                        {
                            throw new HierarchyRequestError();
                        }
                        if (child is object && DOMCommon.Get_Following(child).Any(c => c is DocumentType))
                        {
                            throw new HierarchyRequestError();
                        }
                    }
                }
                else if (node is Element)
                {
                    if (parent.childNodes.Any(c => c is Element) || child is DocumentType || (child is object && DOMCommon.Get_Following(child).Any(c => c is DocumentType)))
                    {
                        throw new HierarchyRequestError();
                    }
                }
                else if (node is DocumentType docType)
                {
                    if (parent.childNodes.Any(c => c is DocumentType) || (child is object && DOMCommon.Get_Preceeding(child).Any(c => c is Element)) || (child is null && parent.childNodes.Any(c => c is Element)))
                    {
                        throw new HierarchyRequestError();
                    }
                }
            }
        }

        /// <summary>
        /// Inserts a given <paramref name="node"/> into the given <paramref name="parent"/> before the given <paramref name="child"/>
        /// Validating the types of all parameters before executing the operation.
        /// </summary>
        /// <param name="node">Node being inserted</param>
        /// <param name="parent">Parent node which is being inserted into</param>
        /// <param name="child">Child node being used as a reference point for insertion</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node Dom_pre_insert_node(Node node, Node parent, Node child)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-pre-insert */
            Dom_ensure_pre_insertion_validity(node, parent, child);
            var referenceChild = child;
            if (ReferenceEquals(referenceChild, node))
                referenceChild = node.nextSibling;

            parent.ownerDocument.adoptNode(node);
            Dom_insert_node_into_parent_before(node, parent, referenceChild);
            return node;
        }

        /// <summary>
        /// Validates and then removes the given child from the given parent.
        /// </summary>
        /// <param name="child">Node to remove</param>
        /// <param name="parent">Parent to remove node from</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node Dom_pre_remove_node(Node child, Node parent)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-pre-remove */
            if (!ReferenceEquals(child.parentNode, parent))
                throw new NotFoundError("Node is not a child of the specified parent");

            Contract.EndContractBlock();

            Dom_remove_node_from_parent(child, parent);
            return child;
        }

        /// <summary>
        /// Takes multiple nodes and merges them into one, returning the resulting node
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node Dom_convert_nodes_into_node(Document document, params object[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#converting-nodes-into-a-node */
            Node node;
            /* 2) Replace each string in nodes with a new Text node whose data is the string and node document is document. */
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] is string)
                {
                    nodes[i] = new Text(document, (string)nodes[i]);
                }
            }
            /* 3) If nodes contains one node, set node to that node. */
            if (nodes.Length == 1)
            {
                node = (Node)nodes[0];
            }
            /* 4) Otherwise, set node to a new DocumentFragment whose node document is document, and then append each node in nodes, if any, to it. */
            else
            {
                node = new DocumentFragment(null, document);
                foreach (Node child in (Node[])nodes)
                {
                    node.appendChild(child);
                }
            }

            return node;
        }
        #endregion

        #region CssUI

        /// <summary>
        /// Whenever any node or element is cloned, this function is called internally such that derived classes may populate the newly cloned instance with any required data such that it would be a perfect copy
        /// </summary>
        /// <param name="newNode">The node to copy data too</param>
        /// <param name="deep">Whether to also copy all child nodes</param>
        protected virtual void CopyTo(ref Node newNode, bool deep = false) { }
        #endregion

    }
}
