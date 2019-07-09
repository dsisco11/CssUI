using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Mutation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using xLog;

namespace CssUI.DOM.Nodes
{

    /// <summary>
    /// </summary>
    public abstract class Node : EventTarget
    {/* Docs: https://dom.spec.whatwg.org/#interface-node */

        internal ILogger Log { get => this.ownerDocument.Log; }
        #region Properties
        internal List<RegisteredObserver> RegisteredObservers = new List<RegisteredObserver>();
        #endregion

        #region Abstracts
        public abstract ENodeType nodeType { get; }
        public abstract string nodeName { get; }

        /* Docs: https://dom.spec.whatwg.org/#dom-node-nodevalue */
        public abstract string nodeValue { get; set; }

        /* Docs: https://dom.spec.whatwg.org/#dom-node-textcontent */
        public abstract string textContent { get; set; }

        /* Docs: https://dom.spec.whatwg.org/#concept-node-length */
        public abstract int nodeLength { get; }
        #endregion

        #region Slottable
        /// <summary>
        /// Element and Text nodes are slotables.
        /// </summary>
        public virtual bool isSlottable => false;

        public virtual Node assignedSlot { get => null; protected set { } }
        public bool isAssigned => (isSlottable && !ReferenceEquals(null, assignedSlot));
        #endregion

        #region DOM
        public virtual Document ownerDocument { get; internal set; }
        public Node parentNode { get; private set; }
        public Element parentElement { get; private set; }
        public readonly ChildNodeList childNodes = new ChildNodeList();

        public Node firstChild { get => childNodes.Count > 0 ? childNodes[0] : null; }
        public Node lastChild { get=> childNodes.Count > 0 ? childNodes[childNodes.Count-1] : null; }
        public Node previousSibling { get; internal set; }
        public Node nextSibling { get; internal set; }


        /// <summary>
        /// Returns true if node is connected and false otherwise.
        /// </summary>
        /// https://dom.spec.whatwg.org/#connected
        /// NOTE: ShadowDOM stuff here
        public bool isConnected { get => false; }

        /// <summary>
        /// The index of this node within it's parent nodes child list
        /// </summary>
        public int index
        {/* The index for nodes is now automatically assigned and updated by the ChildNodeList class */
            get;
            /*{
                *//* The index of an object is its number of preceding siblings, or 0 if it has none. *//*
                int retVal = 0;
                var node = previousSibling;
                while (!ReferenceEquals(node, null))
                {
                    retVal++;
                    node = node.previousSibling;
                }
                return retVal;
            }*/
            internal set;
        }

        /// <summary>
        /// Returns node’s root.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Node getRootNode(GetRootNodeOptions options = null)
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-getrootnode */
            /* The getRootNode(options) method, when invoked, must return context object’s shadow-including root if options’s composed is true, and context object’s root otherwise. */
            if (!ReferenceEquals(options, null) && options.composed)
            {
                throw new NotImplementedException("shadow-nodes are not yet implemented");
            }
            /* The root of an object is itself, if its parent is null, or else it is the root of its parent. The root of a tree is any object participating in that tree whose parent is null. */
            if (ReferenceEquals(parentNode, null))
                return this;

            return parentNode.getRootNode();
        }

        /// <summary>
        /// Returns whether node has children.
        /// </summary>
        /// <returns></returns>
        public bool hasChildNodes() => (this.childNodes.Count > 0);

        public void normalize()
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-normalize */
            /* The normalize() method, when invoked, must run these steps for each descendant exclusive Text node node of context object: */
            foreach (Text node in DOMCommon.Get_Descendents(this, new TextNodeFilter()))
            {
                var length = node.Length;
                /* 2) If length is zero, then remove node and continue with the next exclusive Text node, if any. */
                if (length <= 0)
                {
                    Node._remove_node_from_parent(node, this);
                    continue;
                }
                /* 3) Let data be the concatenation of the data of node’s contiguous exclusive Text nodes (excluding itself), in tree order. */
                var textNodes = Text.get_contiguous_text_nodes(node, true);
                StringBuilder sb = new StringBuilder();
                foreach(Text txt in textNodes)
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

                    foreach (WeakReference<Range> weakRef in Range.LIVE_RANGES)
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
                    Node._remove_node_from_parent(cnode, this);
                }
            }
        }

        public Node cloneNode(bool deep = false)
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-clonenode */
            /* 1) If context object is a shadow root, then throw a "NotSupportedError" DOMException. */
            /* NOTE: ShadowDOM stuff here */
            /* 2) Return a clone of the context object, with the clone children flag set if deep is true. */
            /* Docs: https://dom.spec.whatwg.org/#concept-node-clone */
            return _clone_node(this, null, deep);
        }

        #region Equality
        public bool isEqualNode(Node otherNode) => this.Equals(otherNode);

        public override bool Equals(object obj)
        {/* https://dom.spec.whatwg.org/#concept-node-equals */
            if (ReferenceEquals(null, obj))
                return false;

            if (!(obj is Node B))
                return false;

            if (this.nodeType != B.nodeType)
                return false;

            if (this.childNodes.Count != B.childNodes.Count)
                return false;

            for (int i=0; i<this.childNodes.Count; i++)
            {
                if (!childNodes[i].Equals(B.childNodes[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + (int)nodeType;

            for (int i = 0; i < childNodes.Count; i++)
            {
                hash = hash * 31 + childNodes[i].GetHashCode();
            }

            return hash;
        }
        #endregion

        public EDocumentPosition compareDocumentPosition(Node other)
        {/* Docs: https://dom.spec.whatwg.org/#dom-node-comparedocumentposition */
            if (ReferenceEquals(this, other))
                return 0x0;

            Node node1 = other;
            Node node2 = this;
            Attr attr1=null, attr2=null;
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
                if (!ReferenceEquals(null, node1) && !ReferenceEquals(null, attr1) && ReferenceEquals(node2, node1))
                {
                    foreach(var attr in (node2 as Element).AttributeList)
                    {
                        if (attr.Equals(attr1))
                            return EDocumentPosition.PRECEDING | EDocumentPosition.IMPLEMENTATION_SPECIFIC;
                        if (attr.Equals(attr2))
                            return EDocumentPosition.FOLLOWING | EDocumentPosition.IMPLEMENTATION_SPECIFIC;
                    }
                }
            }

            /* 6) If node1 or node2 is null, or node1’s root is not node2’s root, then return the result of adding DOCUMENT_POSITION_DISCONNECTED, DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC, and either DOCUMENT_POSITION_PRECEDING or DOCUMENT_POSITION_FOLLOWING, with the constraint that this is to be consistent, together. */
            if (ReferenceEquals(null, node1) || ReferenceEquals(null, node2) || !ReferenceEquals(node1.getRootNode(), node2.getRootNode()) )
            {
                return EDocumentPosition.DISCONNECTED | EDocumentPosition.IMPLEMENTATION_SPECIFIC | EDocumentPosition.PRECEDING;
            }

            /* 7) If node1 is an ancestor of node2 and attr1 is null, or node1 is node2 and attr2 is non-null, then return the result of adding DOCUMENT_POSITION_CONTAINS to DOCUMENT_POSITION_PRECEDING. */
            if ((DOMCommon.Is_Ancestor(node1, node2) && ReferenceEquals(null, attr1)) || (ReferenceEquals(node1, node2) && !ReferenceEquals(null, attr2)))
            {
                return EDocumentPosition.CONTAINS | EDocumentPosition.PRECEDING;
            }

            /* 8) If node1 is a descendant of node2 and attr2 is null, or node1 is node2 and attr1 is non-null, then return the result of adding DOCUMENT_POSITION_CONTAINED_BY to DOCUMENT_POSITION_FOLLOWING. */
            if ((DOMCommon.Is_Descendant(node1, node2) && ReferenceEquals(null, attr2)) || (ReferenceEquals(node1, node2) && !ReferenceEquals(null, attr1)))
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

        public Node insertBefore(Node node, Node child)
        {
            return Node._pre_insert_node(node, this, child);
        }

        public Node appendChild(Node node)
        {
            return Node._pre_insert_node(node, this, null);
        }

        public Node replaceChild(Node node, Node child)
        {
            return Node._replace_node_within_parent(node, this, child);
        }

        public Node removeChild(Node child)
        {
            return Node._pre_remove_node(child, this);
        }

        #endregion

        #region Event Stuff
        public override IEventTarget get_the_parent(Event @event)
        {
            /* A node’s get the parent algorithm, given an event, returns the node’s assigned slot, if node is assigned, and node’s parent otherwise. */
            /* XXX: Assigned Slot stuff here */
            return this.parentNode;
        }
        #endregion

        #region Customizable Overload Steps
        /// <summary>
        /// Runs the W3C specification defined steps on a given parent node for when a child <see cref="Text"/> node changes
        /// </summary>
        /// <param name="parent"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void run_child_text_node_change_steps(Node node)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void run_node_insertion_steps(Node node)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void run_node_removing_steps(Node node)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void run_cloning_steps(ref Node copy, Document document, bool clone_children = false)
        {
            /* Currently none of our specifications define this, so it's a placeholder. */
        }
        #endregion

        #region Internal Utilitys

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node _clone_node(Node node, Document document = null, bool clone_children = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-clone */
            /* 1) If document is not given, let document be node’s node document. */
            if (ReferenceEquals(null, document))
                document = node.ownerDocument;
            /* 2) If node is an element, then: */
            Node copy = node.onClone();
            if (node is Element element)
            {
                if (ReferenceEquals(null, copy))
                    copy = new Element(document, element.localName);
                else
                {
                    (copy as Element).localName = element.localName;
                }

                foreach (Attr attr in element.AttributeList)
                {
                    Attr copyAttribute = (Attr)Node._clone_node(attr);
                    (copy as Element).append_attribute(copyAttribute);
                }
            }
            /* 3) Otherwise, let copy be a node that implements the same interfaces as node, and fulfills these additional requirements, switching on node: */
            else
            {
                /* Ensure copy is an instance */
                if (ReferenceEquals(null, copy))
                {
                    switch (node.nodeType)
                    {
                        case ENodeType.DOCUMENT_NODE:
                            {
                                copy = new Document();
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
                                copy = new Attr(natr.Name, document) { Value = natr.Value };
                            }
                            break;
                        default:
                            {
                                if (node is Text txt)
                                {
                                    copy = new Text(document, txt.data);
                                }
                                else if (node is Comment comment)
                                {
                                    copy = new Comment(comment.data);
                                }
                                else if (node is ProcessingInstruction processingInstruction)
                                {
                                    copy = new ProcessingInstruction(processingInstruction.target) { data = processingInstruction.data };
                                }
                                else
                                {
                                    if (ReferenceEquals(null, copy)) throw new InvalidNodeTypeError("Cloning logic for this node type has not been implemented!");
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
                            (copy as Attr).Name = natr.Name;
                            (copy as Attr).Value = natr.Value;
                        }
                        break;
                    default:
                        {
                            if (node is Text txt)
                            {
                                (copy as Text).data = txt.data;
                            }
                            else if (node is Comment comment)
                            {
                                (copy as Comment).data = comment.data;
                            }
                            else if (node is ProcessingInstruction processingInstruction)
                            {
                                (copy as ProcessingInstruction).target = processingInstruction.target;
                                (copy as ProcessingInstruction).data = processingInstruction.data;
                            }
                        }
                        break;
                }
            }
            /* 4) Set copy’s node document and document to copy, if copy is a document, and set copy’s node document to document otherwise. */
            if (copy is Document)
            {
                copy.ownerDocument = copy as Document;
            }
            else
            {
                copy.ownerDocument = document;
            }
            /* 5) Run any cloning steps defined for node in other applicable specifications and pass copy, node, document and the clone children flag if set, as parameters. */
            node.run_cloning_steps(ref copy, document, clone_children);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _remove_node_from_parent(Node node, Node parent, bool suppress_observers = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-remove */
            /* 1) Let index be node’s index. */
            int index = node.index;
            foreach (WeakReference<Range> weakRef in Range.LIVE_RANGES)
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
            /* NOTE: Slottable / Shadow DOM steps go here, if we ever support those things then we must update this code */
            /* 13) Run the removing steps with node and parent. */
            parent.run_node_removing_steps(node);

            /* NOTE: Custom element handling code goes here. */
            /* NOTE: Shadow dom removal code here */

            /* 16) For each inclusive ancestor inclusiveAncestor of parent, and then for each registered of inclusiveAncestor’s registered observer list, 
             * if registered’s options’s subtree is true, then append a new transient registered observer whose observer is registered’s observer, 
             * options is registered’s options, and source is registered to node’s registered observer list. */
            var ancestors = DOMCommon.Get_Inclusive_Ancestors(parent);
            foreach(Node inclusiveAncestor in ancestors)
            {
                foreach(RegisteredObserver registered in inclusiveAncestor.RegisteredObservers)
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
                MutationRecord.Queue_Tree_Mutation_Record(parent, new Node[] { }, new Node[] { node }, previousSibling, nextSibling);
            }
            /* 18) If node is a Text node, then run the child text content change steps for parent. */
            if (node is Text txt)
            {
                parent.run_child_text_node_change_steps(txt);
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
        internal static void _insert_node_into_parent_before(Node node, Node parent, Node child, bool suppress_observers = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-insert */
            /* 1) Let count be the number of children of node if it is a DocumentFragment node, and one otherwise. */
            int count = (node is DocumentFragment doc) ? doc.childNodes.Count : 1;
            /* 2) If child is non-null, then: */
            if (!ReferenceEquals(null, child))
            {
                int childIdx = child.index;// Cache this value here so it doesnt keep getting looked up
                foreach (WeakReference<Range> weakRef in Range.LIVE_RANGES)
                {
                    if (weakRef.TryGetTarget(out Range liveRange))
                    {
                        /* 1) For each live range whose start node is parent and start offset is greater than child’s index, increase its start offset by count. */
                        if (ReferenceEquals(parent, liveRange.startContainer) && liveRange.startOffset > childIdx)
                            liveRange.startOffset += count;
                        /* 2) For each live range whose end node is parent and end offset is greater than child’s index, increase its end offset by count. */
                        if (ReferenceEquals(parent, liveRange.endContainer) && liveRange.endOffset > childIdx)
                            liveRange.endOffset += count;
                    }
                }
            }
            /* 3) Let nodes be node’s children, if node is a DocumentFragment node; otherwise « node ». */
            IEnumerable<Node> nodes = (node is DocumentFragment doc1) ? doc1.childNodes : (IEnumerable<Node>)new Node[] { };
            /* 4) If node is a DocumentFragment node, remove its children with the suppress observers flag set. */
            if (node is DocumentFragment doc2)
            {
                foreach (Node cn in doc2.childNodes)
                {
                    Node._remove_node_from_parent(cn, doc2, true);
                }
                /* 5) If node is a DocumentFragment node, then queue a tree mutation record for node with « », nodes, null, and null. */
                MutationRecord.Queue_Tree_Mutation_Record(node, new Node[] { }, nodes, null, null);
            }
            /* 6) Let previousSibling be child’s previous sibling or parent’s last child if child is null. */
            var previousSibling = ReferenceEquals(null,child) ? parent.lastChild : child.previousSibling;
            int childIndex = ReferenceEquals(null, child) ? 0 : parent.childNodes.IndexOf(child);
            /* 7) For each node in nodes, in tree order: */
            foreach (Node newNode in nodes)
            {
                /* 1) If child is null, then append node to parent’s children. */
                if (ReferenceEquals(null, child))
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
                /* NOTE: ShadowDOM stuff here */
                /* 4) If node is a Text node, run the child text content change steps for parent. */
                if (node is Text txt)
                {
                    parent.run_child_text_node_change_steps(txt);
                }
                /* NOTE: ShadowDOM stuff here */
                /* 7) For each shadow-including inclusive descendant inclusiveDescendant of node, in shadow-including tree order: */
                var newNodeDescendents = DOMCommon.Get_Shadow_Including_Inclusive_Descendents(newNode);
                foreach(Node inclusiveDescendant in newNodeDescendents)
                {
                    /* 1) Run the insertion steps with inclusiveDescendant. */
                    parent.run_node_insertion_steps(inclusiveDescendant);
                    /* 2) If inclusiveDescendant is connected, then: */
                    if (inclusiveDescendant.isConnected)
                    {
                        /* 1) If inclusiveDescendant is custom, then enqueue a custom element callback reaction with inclusiveDescendant, callback name "connectedCallback", and an empty argument list. */
                        /* NOTE: Custom element stuff here */
                        /* 2) Otherwise, try to upgrade inclusiveDescendant. */
                        /* NOTE: Custom element stuff here */
                    }
                }
            }

            /* 8) If suppress observers flag is unset, then queue a tree mutation record for parent with nodes, « », previousSibling, and child. */
            if (!suppress_observers)
            {
                MutationRecord.Queue_Tree_Mutation_Record(parent, nodes, new Node[] { }, previousSibling, child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node _replace_node_within_parent(Node node, Node parent, Node child)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-replace */
            if (!(parent is Document) && !(parent is DocumentFragment) && !(parent is Element))
                throw new HierarchyRequestError("Parent is not a valid type");

            if (DOMCommon.Is_Host_Including_Inclusive_Ancestor(node, parent))
                throw new HierarchyRequestError("Cannot insert a node which is the ancestor of parent into parent.");

            if (!ReferenceEquals(child.parentNode, parent))
                throw new NotFoundError();

            if (!(node is DocumentFragment) && !(node is DocumentType) && !(node is Element) && !(node is Text) && !(node is ProcessingInstruction) && !(node is Comment))
                throw new HierarchyRequestError("Node is not a valid type to insert into parent.");

            if ((node is Text && parent is Document) || (node is DocumentType && !(parent is Document)))
                throw new HierarchyRequestError();

            if (parent is Document)
            {
                if (node is DocumentFragment)
                {
                    int eCount = node.childNodes.Count(c => c is Element);
                    if (eCount > 1 || node.childNodes.Count(c => c is Text) > 0)
                        throw new HierarchyRequestError();
                    else
                    {
                        /* Otherwise, if node has one element child and either parent has an element child that is not child or a doctype is following child. */
                        var eChildren = parent.childNodes.Where(c => c is Element);
                        if (eCount == 1 && (eChildren.Count(c => !ReferenceEquals(c, child)) > 0))
                            throw new HierarchyRequestError();
                        else if (eCount == 1 && DOMCommon.Get_Following(child).Count(c => c is DocumentType) > 0)
                            throw new HierarchyRequestError();
                    }
                }
            }
            else if (node is Element)
            {
                /* parent has an element child that is not child or a doctype is following child. */
                var eChildren = parent.childNodes.Where(c => c is Element);
                if (eChildren.Count(c => !ReferenceEquals(c, child)) > 0)// parent has element that is not child
                    throw new HierarchyRequestError();
                else if (DOMCommon.Get_Following(child).Count(c => c is DocumentType) > 0)// doctype is following child
                    throw new HierarchyRequestError();
            }
            else if (node is DocumentType)
            {
                /* parent has a doctype child that is not child, or an element is preceding child. */
                var eChildren = parent.childNodes.Where(c => c is DocumentType);
                if (eChildren.Count(c => !ReferenceEquals(c, child)) > 0)// parent has a doctype that is not child
                    throw new HierarchyRequestError();
                else if (DOMCommon.Get_Preceeding(child).Count(c => c is Element) > 0)// element is preceeding child
                    throw new HierarchyRequestError();
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
            var removedNodes = new Node[] { };
            /* 12) If child’s parent is not null, then: */
            if (!ReferenceEquals(null, child.parentNode))
            {
                /* 1) Set removedNodes to « child ». */
                removedNodes = new Node[] { child };
                Node._remove_node_from_parent(child, child.parentNode, true);
            }
            /* 13) Let nodes be node’s children if node is a DocumentFragment node; otherwise « node ». */
            IEnumerable<Node> nodes = (node is DocumentFragment) ? node.childNodes : (IEnumerable<Node>)new Node[] { node };
            /* 14) Insert node into parent before reference child with the suppress observers flag set. */
            Node._insert_node_into_parent_before(node, parent, referenceChild, true);
            /* 15) Queue a tree mutation record for parent with nodes, removedNodes, previousSibling, and reference child. */
            MutationRecord.Queue_Tree_Mutation_Record(parent, nodes, removedNodes, previousSibling, referenceChild);
            return child;
        }

        /// <summary>
        /// Removes all children within a parent element replacing them with the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _replace_all_within_node(Node node, Node parent)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-replace-all */
            /* 1) If node is not null, adopt node into parent’s node document. */
            if (!ReferenceEquals(null, node))
            {
                parent.ownerDocument.adoptNode(node);
            }

            IEnumerable<Node> removedNodes = parent.childNodes;
            IEnumerable<Node> addedNodes = new Node[] { };

            if (node is DocumentFragment)
                addedNodes = node.childNodes;
            else if (!ReferenceEquals(null, node))
                addedNodes = new Node[] { node };
            /* 6) Remove all parent’s children, in tree order, with the suppress observers flag set. */
            foreach (Node child in parent.childNodes)
            {
                Node._remove_node_from_parent(child, parent, true);
            }
            /* 7) If node is not null, then insert node into parent before null with the suppress observers flag set. */
            if (!ReferenceEquals(null, node))
            {
                Node._insert_node_into_parent_before(node, parent, null, true);
            }
            /* 8) Queue a tree mutation record for parent with addedNodes, removedNodes, null, and null. */
            MutationRecord.Queue_Tree_Mutation_Record(parent, addedNodes, removedNodes, null, null);
        }

        /// <summary>
        /// Ensures the given <paramref name="node"/>, <paramref name="parent"/>, and <paramref name="child"/> are valid types for insertion
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void _ensure_pre_insertion_validity(Node node, Node parent, Node child)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-ensure-pre-insertion-validity */
            if (!(parent is Document) && !(parent is DocumentFragment) && !(parent is Element))
                throw new HierarchyRequestError("parent is not a valid type!");
            /* 2) If node is a host-including inclusive ancestor of parent, throw a "HierarchyRequestError" DOMException. */
            if (DOMCommon.Is_Host_Including_Inclusive_Ancestor(node, parent))
                throw new HierarchyRequestError();

            /* 3) If child is not null and its parent is not parent, then throw a "NotFoundError" DOMException. */
            if (!ReferenceEquals(null, child) && !ReferenceEquals(parent, child.parentNode))
                throw new NotFoundError();
            /* 4) If node is not a DocumentFragment, DocumentType, Element, Text, ProcessingInstruction, or Comment node, throw a "HierarchyRequestError" DOMException. */
            if (!(node is DocumentFragment) && !(node is DocumentType) && !(node is Element) && !(node is Text) && !(node is ProcessingInstruction) && !(node is Comment))
                throw new HierarchyRequestError("node does not inherit from any element base types.");

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
                    if (eCount > 1 || docFrag.childNodes.Count(c => c is Text) > 0)
                        throw new HierarchyRequestError();
                    else if (eCount == 1)
                    {
                        if (parent.childNodes.Count(c => c is Element) > 0 || child is DocumentType)
                            throw new HierarchyRequestError();
                        if (!ReferenceEquals(null, child) && DOMCommon.Get_Following(child).Count(c => c is DocumentType) > 0)
                            throw new HierarchyRequestError();
                    }
                }
                else if (node is Element element)
                {
                    if (parent.childNodes.Count(c => c is Element) > 0 || child is DocumentType || (!ReferenceEquals(null, child) && DOMCommon.Get_Following(child).Count(c => c is DocumentType) > 0))
                        throw new HierarchyRequestError();
                }
                else if (node is DocumentType docType)
                {
                    if (parent.childNodes.Count(c => c is DocumentType) > 0 || (!ReferenceEquals(null, child) && DOMCommon.Get_Preceeding(child).Count(c => c is Element) > 0) || (ReferenceEquals(null, child) && parent.childNodes.Count(c => c is Element) > 0))
                        throw new HierarchyRequestError();
                }
            }

        }

        /// <summary>
        /// Inserts a given <paramref name="node"/> into the given <paramref name="parent"/> before the given <paramref name="child"/>
        /// Validating the types of all parameters before executing the operation.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node _pre_insert_node(Node node, Node parent, Node child)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-pre-insert */
            Node._ensure_pre_insertion_validity(node, parent, child);
            var referenceChild = child;
            if (ReferenceEquals(referenceChild, node))
                referenceChild = node.nextSibling;

            parent.ownerDocument.adoptNode(node);
            Node._insert_node_into_parent_before(node, parent, referenceChild);
            return node;
        }

        /// <summary>
        /// Validates and then removes the given child from the given parent.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node _pre_remove_node(Node child, Node parent)
        {/* Docs: https://dom.spec.whatwg.org/#concept-node-pre-remove */
            if (!ReferenceEquals(child.parentNode, parent))
                throw new NotFoundError("child node is not a child of the specified parent");

            Node._remove_node_from_parent(child, parent);
            return child;
        }

        /// <summary>
        /// Takes multiple nodes and merges them into one, returning the resulting node
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Node _convert_nodes_into_node(Document document, params object[] nodes)
        {/* Docs: https://dom.spec.whatwg.org/#converting-nodes-into-a-node */
            Node node = null;
            /* 2) Replace each string in nodes with a new Text node whose data is the string and node document is document. */
            for (int i=0; i<nodes.Count(); i++)
            {
                if (nodes[i] is string)
                {
                    nodes[i] = new Text(document, (string)nodes[i]);
                }
            }
            /* 3) If nodes contains one node, set node to that node. */
            if (nodes.Count() == 1)
            {
                node = (Node)nodes[0];
            }
            /* 4) Otherwise, set node to a new DocumentFragment whose node document is document, and then append each node in nodes, if any, to it. */
            else
            {
                node = new DocumentFragment(null, document);
                foreach(Node child in (Node[])nodes)
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
        /// <param name="newClone"></param>
        protected virtual Node onClone() { return null; }
        #endregion

    }
}
