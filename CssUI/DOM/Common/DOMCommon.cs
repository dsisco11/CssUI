using System;
using System.Linq;
using System.Collections.Generic;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;
using CssUI.DOM.Traversal;
using CssUI.CSS;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Geometry;
using System.Reflection;

namespace CssUI.DOM
{
    public static class DOMCommon
    {
        #region Metadata
        /// <summary>
        /// Official HTML namespace string
        /// </summary>
        public const string HTMLNamespace = ("http://www.w3.org/1999/xhtml");
        /// <summary>
        /// Official MathML namespace string
        /// </summary>
        public const string MathMLNamespace = ("http://www.w3.org/1998/Math/MathML");
        /// <summary>
        /// Official SVG namespace string
        /// </summary>
        public const string SVGNamespace = ("http://www.w3.org/2000/svg");
        /// <summary>
        /// Official XLink namespace string
        /// </summary>
        public const string XLinkNamespace = ("http://www.w3.org/1999/xlink");
        /// <summary>
        /// Official XML namespace string
        /// </summary>
        public const string XMLNamespace = ("http://www.w3.org/XML/1998/namespace");
        /// <summary>
        /// Official XMLNS namespace string
        /// </summary>
        public const string XMLNSNamespace = ("http://www.w3.org/2000/xmlns/");
        #endregion

        #region Lookups
        /// <summary>
        /// Returns the HTML Content-Type string associated with the given namespace
        /// </summary>
        /// <param name="Namespace"></param>
        /// <returns></returns>
        internal static string Lookup_Content_Type_String(string Namespace)
        {
            switch (Namespace)
            {
                case HTMLNamespace:
                    return "application/xhtml+xml";
                case SVGNamespace:
                    return "image/svg+xml";
                default:
                    return "application/xml";
            }
        }

        internal static ConstructorInfo Lookup_Element_Interface(AtomicString localName, AtomicString Namespace)
        {
#if ENABLE_HTML
            if (Namespace.Equals(HTMLNamespace))
            {
                //ElementMetadata outMetadata = HTML.HTMLElementTable.TABLE[(int)localName.EnumValue.Value];
                if (!HTML.HTMLElementTable.KEYWORD.TryGetValue(localName, out ElementMetadata outMetadata))
                {
                    return ElementMetadata.UnknownMeta.ctor;
                    // throw new Exception($"unable to find tag type for HTML tag matching \"{localName}\"");
                }

                return outMetadata.ctor;
            }
#endif

            return ElementMetadata.ElementMeta.ctor;
        }
        #endregion

        #region Classifications
        /// <summary>
        /// Returns True if the specified document is the active one
        /// </summary>
        /// <param name="document"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Active_Document(Document document)
        {
            return true;
        }

        /// <summary>
        /// Returns true if the node is one of the 3 common text node types: <see cref="Text"/>, <see cref="ProcessingInstruction"/>, or <see cref="Comment"/>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_CommonTextNode(Node node)
        {
            return node is Text || node is ProcessingInstruction || node is Comment;
        }

        /// <summary>
        /// Returns true if the event was triggered by a user action
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Triggered_By_UserActivation(Event @event)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#triggered-by-user-activation */
            if (!@event.isTrusted)
                return false;

            switch (@event.type.EnumValue)
            {
                case EEventName.Change:
                case EEventName.Click:
                case EEventName.ContextMenu:
                case EEventName.DoubleClick:
                case EEventName.MouseUp:
                case EEventName.PointerUp:
                case EEventName.Reset:
                case EEventName.Submit:
                case EEventName.TouchEnd:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region Ordered Sets
        public static string Serialize_Ordered_Set(IEnumerable<ReadOnlyMemory<char>> set)
        {
            return StringCommon.Concat(UnicodeCommon.CHAR_SPACE, set);
        }

        public static IReadOnlyList<ReadOnlyMemory<char>> Parse_Ordered_Set(StringPtr Input)
        {
            return StringCommon.Strtok(Input, UnicodeCommon.CHAR_SPACE);
        }


        public static string Serialize_Comma_Seperated_list(IEnumerable<ReadOnlyMemory<char>> list)
        {
            return StringCommon.Concat(UnicodeCommon.CHAR_SPACE, list);
        }

        public static IReadOnlyList<ReadOnlyMemory<char>> Parse_Comma_Seperated_List(StringPtr Input)
        {/* Docs: https://infra.spec.whatwg.org/#split-on-commas */
            var Tokens = StringCommon.Strtok(Input, UnicodeCommon.CHAR_COMMA);
            var newList = new ReadOnlyMemory<char>[Tokens.Length];

            for (int i = 0; i < Tokens.Length; i++)
            {
                newList[i] = StringCommon.Trim(Tokens[i], UnicodeCommon.CHAR_SPACE);
            }

            return newList;
        }
        #endregion

        #region Geometry
        /// <summary>
        /// Returns the encompasing bounds of a list of <see cref="DOMRect"/>s
        /// </summary>
        /// <param name="Rects"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DOMRect getBoundingClientRect(IEnumerable<DOMRect> Rects)
        {/* Docs: https://www.w3.org/TR/cssom-view-1/#dom-element-getboundingclientrect */
            if (!Rects.GetEnumerator().MoveNext())
                return new DOMRect(0, 0, 0, 0);

            /* 3) Otherwise, return a static DOMRect object describing the smallest rectangle that includes the first rectangle in list and all of the remaining rectangles of which the height or width is not zero. */
            double? top = null, right = null, bottom = null, left = null;
            Rects.GetEnumerator().Reset();
            foreach (var rect in Rects)
            {
                top = !top.HasValue ? rect.Top : MathExt.Min(top.Value, rect.Top);
                right = !right.HasValue ? rect.Right : MathExt.Max(right.Value, rect.Right);
                bottom = !bottom.HasValue ? rect.Bottom : MathExt.Max(bottom.Value, rect.Bottom);
                left = !left.HasValue ? rect.Left : MathExt.Min(left.Value, rect.Left);
            }

            return new DOMRect(left.Value, top.Value, (right.Value - left.Value), (bottom.Value - top.Value));
        }
        #endregion

        #region CSS Selectors
        public static IEnumerable<Element> Scope_Match_Selector_String(Node node, string selector)
        {
            /* The closest(selectors) method, when invoked, must run these steps: */
            /* 1) Let s be the result of parse a selector from selectors. [SELECTORS4] */
            var Selector = new CssSelector(selector);
            /* 2) If s is failure, throw a "SyntaxError" DOMException. */
            if (Selector == null)
            {
                throw new DomSyntaxError("Could not parse selector.");
            }
            /* 3) Return the result of match a selector against a tree with s and node’s root using scoping root node. [SELECTORS4]. */
            return Selector.Match_Against_Tree(new Node[] { node }, node.getRootNode());
        }
        #endregion

        #region Slottables
        internal static ISlot Find_Slot(ISlottable slottable, bool open_flag = false)
        {/* Docs: https://dom.spec.whatwg.org/#find-a-slot */
#if ENABLE_HTML
            if (slottable.parentNode == null)
                return null;

            var shadow = slottable.parentNode.getRootNode() as ShadowRoot;
            if (shadow == null)
                return null;

            if (open_flag && shadow.Mode != EShadowRootMode.Open)
                return null;

            /* 5) Return the first slot in tree order in shadow’s descendants whose name is slotable’s name, if any, and null otherwise. */
            var tree = new TreeWalker(shadow, ENodeFilterMask.SHOW_ALL);
            var node = tree.nextNode();
            while (node is object)
            {
                if (node is HTMLSlotElement slot)
                {
                    if (StringCommon.StrEq(slot.Name, (slottable as ISlottable).Slot_Name))
                        return slot;
                }
                node = tree.nextNode();
            }
#endif

            return null;
        }

        internal static List<ISlottable> Find_Slotables(ISlot slot)
        {/* Docs: https://dom.spec.whatwg.org/#find-slotables */
#if ENABLE_HTML
            var result = new List<ISlottable>();
            var root = (slot as Node).getRootNode();
            if (!(root is ShadowRoot))
                return result;

            var shadowRoot = root as ShadowRoot;
            var host = shadowRoot.Host;

            /* 4) For each slotable child of host, slotable, in tree order: */
            foreach (Node node in host.childNodes)
            {
                if (node is ISlottable)
                {
                    ISlottable slotable = node as ISlottable;
                    var foundSlot = Find_Slot(slotable);
                    if (ReferenceEquals(foundSlot, slot))
                        result.Add(slotable);
                }
            }

            return result;
#else
            return new List<ISlottable>();
#endif
        }

        internal static List<ISlottable> Find_Flattened_Slotables(ISlot slot)
        {/* Docs: https://dom.spec.whatwg.org/#find-flattened-slotables */
#if ENABLE_HTML
            var result = new List<ISlottable>();
            if (!(slot.getRootNode() is ShadowRoot))
                return result;

            var slotables = Find_Slotables(slot);
            /* 4) If slotables is the empty list, then append each slotable child of slot, in tree order, to slotables. */
            if (slotables.Count <= 0)
            {
                foreach (Node child in slot.childNodes)
                {
                    if (child is ISlottable)
                    {
                        slotables.Add(child as ISlottable);
                    }
                }
            }

            /* 5) For each node in slotables: */
            foreach (ISlottable node in slotables)
            {
                if (node is ISlot && node.getRootNode() is ShadowRoot)
                {
                    var temporaryResult = DOMCommon.Find_Flattened_Slotables(node as ISlot);
                    result.AddRange(temporaryResult);
                }
                else
                {
                    result.Add(node);
                }
            }

            /* 6) Return result. */
            return result;
#else 
            return new List<ISlottable>();
#endif
        }

        internal static void Assign_Slottables_For_Tree(Node root)
        {/* Docs: https://dom.spec.whatwg.org/#assign-slotables-for-a-tree */
#if ENABLE_HTML
            /* To assign slotables for a tree, given a node root, run assign slotables for each slot slot in root’s inclusive descendants, in tree order. */
            var inclusiveDescendants = Get_Inclusive_Descendents(root, FilterSlots.Instance);
            foreach (ISlot descendant in inclusiveDescendants)
            {
                Assign_Slottables(descendant);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        internal static void Assign_Slottables(ISlot slot)
        {/* Docs: https://dom.spec.whatwg.org/#assign-slotables */
#if ENABLE_HTML
            var slotables = DOMCommon.Find_Slotables(slot);

            bool match = true;
            if (slotables.Count != slot.Assigned.Count)
                match = false;
            else
            {
                for (int i = 0; i < slotables.Count; i++)
                {
                    if (!ReferenceEquals(slotables[i], slot.Assigned[i]))
                    {
                        match = false;
                        break;
                    }
                }
            }
            /* 2) If slotables and slot’s assigned nodes are not identical, then run signal a slot change for slot. */
            if (!match)
            {
                slot.Signal_Slot_Change();
            }
            /* 3) Set slot’s assigned nodes to slotables. */
            slot.Assigned = slotables;
            /* 4) For each slotable in slotables, set slotable’s assigned slot to slot. */
            foreach (ISlottable slotable in slotables)
            {
                slotable.assignedSlot = slot;
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slotable"></param>
        internal static void Assign_A_Slot(ISlottable slotable)
        {/* Docs: https://dom.spec.whatwg.org/#assign-a-slot */
#if ENABLE_HTML
            var slot = DOMCommon.Find_Slot(slotable);
            if (slot is object)
                DOMCommon.Assign_Slottables(slot);
#endif
        }
        #endregion

        #region Hierarchy Checks

        #region Descendants
        /// <summary>
        /// An object A is called a descendant of an object B, if either A is a child of B or A is a child of an object C that is a descendant of B.
        /// </summary>
        /// <returns>If <paramref name="A"/> is a descendant of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Descendant(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-descendant */
            var tree = new TreeWalker(A, Enums.ENodeFilterMask.SHOW_ALL);
            /* Proove it true */
            Node node = tree.parentNode();
            while (!ReferenceEquals(node, null))
            {
                if (ReferenceEquals(node, B))
                    return true;
                node = tree.parentNode();
            }

            return false;
        }

        /// <summary>
        /// An inclusive descendant is an object or one of its descendants.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (inclusive) descendant of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Inclusive_Descendant(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-inclusive-descendant */
            return ReferenceEquals(A, B) || Is_Descendant(A, B);
        }

        /// <summary>
        /// An object A is a shadow-including descendant of an object B, if A is a descendant of B, or A’s root is a shadow root and A’s root’s host is a shadow-including inclusive descendant of B.
        /// </summary>
        /// <returns>If <paramref name="A"/> is a (shadow including) descendant of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Shadow_Including_Descendant(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-shadow-including-descendant */
            if (Is_Descendant(A, B))
                return true;
#if ENABLE_HTML
            if (A.getRootNode() is ShadowRoot aRootShadow && Is_Shadow_Including_Inclusive_Descendant(aRootShadow.Host, B))
                return true;
#endif

            return false;
        }

        /// <summary>
        /// A shadow-including inclusive descendant is an object or one of its shadow-including descendants.
        /// </summary>
        /// <returns>If <paramref name="A"/> is a (shadow including) (inclusive) descendant of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Shadow_Including_Inclusive_Descendant(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-shadow-including-inclusive-descendant */
            return (ReferenceEquals(A, B) || Is_Shadow_Including_Descendant(A, B));
        }
        #endregion

        #region Ancestors
        /// <summary>
        /// An object A is called an ancestor of an object B if and only if B is a descendant of A.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ancestor(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-ancestor */
            /* Fastest method to determine this is to look at it in reverse and ask if B is a descendant of A */
            return Is_Descendant(B, A);
        }

        /// <summary>
        /// An inclusive ancestor is an object or one of its ancestors.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (inclusive) ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Inclusive_Ancestor(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-inclusive-ancestor */
            /* Fastest method to determine this is to look at it in reverse and ask if B is a descendant of A */
            return ReferenceEquals(A, B) || Is_Descendant(B, A);
        }

        /// <summary>
        /// An object A is a host-including inclusive ancestor of an object B, if either A is an inclusive ancestor of B, or if B’s root has a non-null host and A is a host-including inclusive ancestor of B’s root’s host.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (Host including) (inclusive) ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Host_Including_Inclusive_Ancestor(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-host-including-inclusive-ancestor */
            /* Fastest method to determine this is to look at it in reverse and ask if B is a descendant of A */
            if (ReferenceEquals(A, B) || Is_Descendant(B, A))
                return true;

            var root = B.getRootNode();
            if (root is DocumentFragment doc)
            {
                if (doc.Host is object)
                {
                    return Is_Host_Including_Inclusive_Ancestor(A, doc.Host);
                }
            }

            return false;
        }


        /// <summary>
        /// An object A is a shadow-including ancestor of an object B, if and only if B is a shadow-including descendant of A.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (shadow including) ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Shadow_Including_Ancestor(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-shadow-including-ancestor */
            return Is_Shadow_Including_Descendant(B, A);
        }


        /// <summary>
        /// A shadow-including inclusive ancestor is an object or one of its shadow-including ancestors.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (shadow including) (inclusive) ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Shadow_Including_Inclusive_Ancestor(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-shadow-including-inclusive-ancestor */
            return (ReferenceEquals(A, B) || Is_Shadow_Including_Ancestor(A, B));
        }
        #endregion

        #region Siblings
        /// <summary>
        /// An object A is called a sibling of an object B, if and only if B and A share the same non-null parent.
        /// </summary>
        /// <returns>If <paramref name="A"/> is a sibling of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Sibling(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-sibling */
            if (ReferenceEquals(A.parentNode, null) || ReferenceEquals(B.parentNode, null))
                return false;
            return ReferenceEquals(A.parentNode, B.parentNode);
        }

        /// <summary>
        /// An inclusive sibling is an object or one of its siblings.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (inclusive) sibling of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Inclusive_Sibling(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-inclusive-sibling */
            return ReferenceEquals(A, B) || Is_Sibling(A, B);
        }

        /// <summary>
        /// An object A is preceding an object B if A and B are in the same tree and A comes before B in tree order.
        /// </summary>
        /// <returns>If <paramref name="A"/> is preeceding <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Preceeding(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-preceding */
            if (!ReferenceEquals(A.getRootNode(), B.getRootNode()))// Different trees
                return false;

            /* Proove it true */
            Node node = A.nextSibling;
            while (!ReferenceEquals(node, null))
            {
                if (ReferenceEquals(node, B))
                    return true;
                node = node.nextSibling;
            }

            return false;
        }

        /// <summary>
        /// An object A is following an object B if A and B are in the same tree and A comes after B in tree order.
        /// </summary>
        /// <returns>If <paramref name="A"/> is following <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Following(Node A, Node B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-following */
            if (!ReferenceEquals(A.getRootNode(), B.getRootNode()))// Different trees
                return false;

            /* Proove it true */
            Node node = A.previousSibling;
            while (!ReferenceEquals(node, null))
            {
                if (ReferenceEquals(node, B))
                    return true;
                node = node.previousSibling;
            }

            return false;
        }
        #endregion

        #endregion

        #region Hierarchy Getters
        /// <summary>
        /// Returns a list of all nodes within the given range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Range_Nodes(Range range)
        {
            var commonAncestor = range.startContainer;
            while (!Is_Inclusive_Ancestor(commonAncestor, range.endContainer))
            {
                commonAncestor = commonAncestor.parentNode;
            }

            var containedChildren = Get_Descendents(commonAncestor, new FilterRangeContains(range));
            return containedChildren;
        }

        /// <summary>
        /// Returns a list of all ancestors for the given node, that is; the chain of parent elements all the way up to the root element.
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Ancestors(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (current is object)
            {
                list.AddLast(current);
                current = tree.parentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all ancestors for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Ancestors<NodeType>(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (current is object)
            {
                if (current is NodeType currentAsType)
                {
                    list.AddLast(currentAsType);
                }
                current = tree.parentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth ancestor for the given node, that is; the Nth parent element along the chain of elements going all the way up to the root element.
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Ancestor(Node node, uint Nth, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (current is object)
            {
                if (--Nth <= 0) { return current; }
                current = tree.parentNode();
            }

            return null;
        }

        /// <summary>
        /// Returns Nth ancestor for the given node whom matches the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Nth_Ancestor<NodeType>(Node node, uint Nth, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (current is object)
            {
                if (current is NodeType nodeAsType)
                {
                    if (--Nth <= 0) { return nodeAsType; }
                }

                current = tree.parentNode();
            }

            return default(NodeType);
        }

        /// <summary>
        /// Returns a list of all (inclusive) ancestors for the given node, that is; the chain of parent elements all the way up to the root element.
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Inclusive_Ancestors(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = new LinkedList<Node>();
            list.AddLast(node);
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (current is object)
            {
                list.AddLast(current);
                current = tree.parentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all descendents for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (current is object)
            {
                list.AddLast(current);
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all descendents for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Descendents<NodeType>(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (current is object)
            {
                if (current is NodeType currentAsType)
                {
                    list.AddLast(currentAsType);
                }
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth descendant for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Descendant(Node node, uint Nth, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (current is object)
            {
                if (--Nth <= 0) { return current; }
                current = tree.nextNode();
            }

            return null;
        }

        /// <summary>
        /// Returns Nth descendant for the given node whom matches the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Nth_Descendant<NodeType>(Node node, uint Nth, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (current is object)
            {
                if (current is NodeType nodeAsType)
                {
                    if (--Nth <= 0) { return nodeAsType; }
                }

                current = tree.nextNode();
            }

            return default(NodeType);
        }

        /// <summary>
        /// Returns a list of all descendents for the given node and, in addition, all of its ShadowDOM descendents
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Shadow_Including_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
#if ENABLE_HTML
            var list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (current is object)
            {
                if (Is_Shadow_Including_Descendant(current, node))
                {
                    list.AddLast(current);
                }

                if (current.Is_ShadowHost)
                {
                    var element = current as Element;
                    Get_Shadow_Including_Descendents(element.shadowRoot);
                }
                current = tree.nextNode();
            }

            return list;
#else
            return Get_Descendents(node, Filter, FilterMask);
#endif
        }

        /// <summary>
        /// Returns a list of all (inclusive) descendents for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Inclusive_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = new LinkedList<Node>();
            list.AddLast(node);
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (current is object)
            {
                list.AddLast(current);
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all (inclusive) descendents for the given node and, in addition, all of its ShadowDOM (inclusive) descendents
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Shadow_Including_Inclusive_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = (LinkedList<Node>)Get_Shadow_Including_Descendents(node, Filter, FilterMask);
            list.AddFirst(node);// Add the node (inclusive)
            return list;
        }


        /// <summary>
        /// Returns a list of all previous and adjacent sibling nodes for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Previous(Node node, NodeFilter Filter = null)
        {
            var list = new LinkedList<Node>();

            Node current = node.previousSibling;
            while (current is object)
            {
                var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                    break;

                if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                    list.AddLast(current);

                current = current.previousSibling;
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all previous and adjacent sibling nodes for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Previous<NodeType>(Node node, NodeFilter Filter = null) where NodeType : INode
        {
            var list = new LinkedList<NodeType>();

            Node current = node.previousSibling;
            while (current is object)
            {
                if (current is NodeType nodeAsType)
                {
                    var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                    if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                        break;

                    if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                        list.AddLast(nodeAsType);
                }

                current = current.previousSibling;
            }

            return list;
        }

        /// <summary>
        /// Returns Nth previous and adjacent sibling for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Previous(Node node, uint Nth, NodeFilter Filter = null)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            Node current = node.previousSibling;
            while (current is object)
            {
                var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                    break;

                if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                {
                    if (--Nth <= 0)
                    {
                        return current;
                    }
                }

                current = current.previousSibling;
            }

            return null;
        }


        /// <summary>
        /// Returns a list of all tree-order preceeding (sibling) nodes for the given node 
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Preceeding(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

            Node current = tree.previousSibling();
            while (current is object)
            {
                var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                    break;

                if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                    list.AddLast(current);

                current = tree.previousSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all tree-order preceeding (sibling) nodes for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Preceeding<NodeType>(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

            Node current = tree.previousSibling();
            while (current is object)
            {
                if (current is NodeType nodeAsType)
                {
                    var fres = Filter?.acceptNode(current) ?? ENodeFilterResult.FILTER_ACCEPT;
                    if (fres == ENodeFilterResult.FILTER_REJECT)
                        break;

                    if (fres == ENodeFilterResult.FILTER_ACCEPT)
                        list.AddLast(nodeAsType);
                }

                current = tree.previousSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth tree-order preceeding sibling for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Preceeding(Node node, uint Nth, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.previousSibling();
            while (current is object)
            {
                if (--Nth <= 0) { return current; }
                current = tree.previousSibling();
            }

            return null;
        }


        /// <summary>
        /// Returns a list of all tree-order following (sibling) nodes for the given node 
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Following(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            var list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

            Node current = tree.nextSibling();
            while (current is object)
            {
                var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                    break;

                if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                    list.AddLast(current);

                current = tree.nextSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all tree-order following (sibling) nodes for the given node 
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Following<NodeType>(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
        {
            var list = new LinkedList<NodeType>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

            Node current = tree.nextSibling();
            while (current is object)
            {
                if (current is NodeType nodeAsType)
                {
                    var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                    if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                        break;

                    if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                        list.AddLast(nodeAsType);

                }
                current = tree.nextSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth tree-order following sibling for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which Node types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Following(Node node, uint Nth, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextSibling();
            while (current is object)
            {
                if (--Nth <= 0) { return current; }
                current = tree.nextSibling();
            }

            return null;
        }


        /// <summary>
        /// Returns a list of all nodes after and adjacent to the given node (siblings)
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Next(Node node, NodeFilter Filter = null)
        {
            var list = new LinkedList<Node>();
            Node current = node.nextSibling;
            while (current is object)
            {
                var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                    break;

                if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                    list.AddLast(current);

                current = current.nextSibling;
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all nodes after and adjacent to the given node (siblings)
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Next<NodeType>(Node node, NodeFilter Filter = null) where NodeType : INode
        {
            var list = new LinkedList<NodeType>();
            Node current = node.nextSibling;
            while (current is object)
            {
                if (current is NodeType nodeAsType)
                {
                    var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                    if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                        break;

                    if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                        list.AddLast(nodeAsType);
                }

                current = current.nextSibling;
            }

            return list;
        }

        /// <summary>
        /// Returns Nth next sibling for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Next(Node node, uint Nth, NodeFilter Filter = null)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            Node current = node.nextSibling;
            while (current is object)
            {
                var fres = Filter?.acceptNode(current) ?? Enums.ENodeFilterResult.FILTER_ACCEPT;
                if (fres == Enums.ENodeFilterResult.FILTER_REJECT)
                    break;

                if (fres == Enums.ENodeFilterResult.FILTER_ACCEPT)
                {
                    if (--Nth <= 0)
                    {
                        return current;
                    }
                }

                current = current.nextSibling;
            }

            return null;
        }


        /// <summary>
        /// Returns a list of <see cref="Element"/>s matching <paramref name="qualifiedName"/>
        /// </summary>
        /// <param name="root">The node to start searching from</param>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Element> Get_Elements_By_Qualified_Name(Node root, string qualifiedName)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbytagname */
            /* 1) If qualifiedName is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches only descendant elements. */
            if (StringCommon.StrEq(qualifiedName, "\u002A"))
            {
                LinkedList<Element> descendents = new LinkedList<Element>();
                var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
                Node current = tree.nextNode();
                while (current is object)
                {
                    descendents.AddLast((Element)current);
                    current = tree.nextNode();
                }

                return descendents;
            }
            /* 2) Otherwise, if root’s node document is an HTML document, return a HTMLCollection rooted at root, whose filter matches the following descendant elements: */
            if (root.ownerDocument is HTMLDocument)
            {
                /* 1) Whose namespace is the HTML namespace and whose qualified name is qualifiedName, in ASCII lowercase. */
                /* 2) Whose namespace is not the HTML namespace and whose qualified name is qualifiedName. */

                LinkedList<Element> descendents = new LinkedList<Element>();
                var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
                Node current = tree.nextNode();
                while (current is object)
                {
                    var element = (Element)current;
                    if (StringCommon.StrEq(element.NamespaceURI, HTMLNamespace))
                    {
                        if (StringCommon.StrEq(qualifiedName, element.tagName.ToLowerInvariant()))
                        {
                            descendents.AddLast(element);
                        }
                    }
                    else if (StringCommon.StrEq(qualifiedName, element.tagName))
                    {
                        descendents.AddLast(element);
                    }

                    current = tree.nextNode();
                }

                return descendents;
            }
            else
            {
                /* 3) Otherwise, return a HTMLCollection rooted at root, whose filter matches descendant elements whose qualified name is qualifiedName. */
                LinkedList<Element> descendents = new LinkedList<Element>();
                var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
                Node node = tree.nextNode();
                while (node is object)
                {
                    var element = (Element)node;
                    if (StringCommon.StrEq(element.NamespaceURI, HTMLNamespace))
                    {
                        if (StringCommon.StrEq(qualifiedName, element.tagName.ToLowerInvariant()))
                        {
                            descendents.AddLast(element);
                        }
                    }
                    else if (StringCommon.StrEq(qualifiedName, element.tagName))
                    {
                        descendents.AddLast(element);
                    }

                    node = tree.nextNode();
                }

                /* When invoked with the same argument, the same HTMLCollection object may be returned as returned by an earlier call. */
                return descendents;
            }
        }

        /// <summary>
        /// Returns a list of <see cref="Element"/>s which match <paramref name="localName"/> and <paramref name="Namespace"/>
        /// </summary>
        /// <param name="root">The node to start searching from</param>
        /// <param name="Namespace"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Element> Get_Elements_By_Namespace_And_Local_Name(Node root, string Namespace, string localName)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbytagnamens */
            if (Namespace.Length <= 0)
                Namespace = null;

            /* 2) If both namespace and localName are "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements. */
            if (Namespace.Equals("\u002A") && localName.Equals("\u002A"))
                return Get_Descendents<Element>(root, null, ENodeFilterMask.SHOW_ELEMENT);

            /* 3) Otherwise, if namespace is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements whose local name is localName. */
            var localNameFilter = new FilterLocalName(localName);
            if (Namespace.Equals("\u002A"))
                return Get_Descendents<Element>(root, localNameFilter, ENodeFilterMask.SHOW_ELEMENT);

            /* 4) Otherwise, if localName is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements whose namespace is namespace. */
            var NamespaceFilter = new FilterNamespace(Namespace);
            if (localName.Equals("\u002A"))
                return Get_Descendents<Element>(root, NamespaceFilter, ENodeFilterMask.SHOW_ELEMENT);

            /* 5) Otherwise, return a HTMLCollection rooted at root, whose filter matches descendant elements whose namespace is namespace and local name is localName. */
            var localName_Namespace_Filter = new FilterLocalName_Namespace(localName, Namespace);
            return Get_Descendents<Element>(root, localName_Namespace_Filter, ENodeFilterMask.SHOW_ELEMENT);
        }

        /// <summary>
        /// Returns a list of <see cref="Element"/>s which match <paramref name="localName"/> and <paramref name="Namespace"/>
        /// </summary>
        /// <param name="root">The node to start searching from</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Element> Get_Elements_By_Class_Name(Node root, string classNames)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbyclassname */

            classNames = StringCommon.Transform(classNames.AsMemory(), UnicodeCommon.To_ASCII_Lower_Alpha);
            var classes = Parse_Ordered_Set(classNames.AsMemory()).Select(o => (AtomicString)o.ToString());
            /* 2) If classes is the empty set, return an empty HTMLCollection. */
            if (!classes.Any())
            {
                return new LinkedList<Element>();
            }

            /* 3) Return a HTMLCollection rooted at root, whose filter matches descendant elements that have all their classes in classes. */
            var descendents = new LinkedList<Element>();
            var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
            Node current = tree.nextNode();
            while (current is object)
            {
                Element E = current as Element;

                if (E.classList.ContainsAll(classes))
                    descendents.AddLast(E);

                if (E is object)
                    descendents.AddLast(E);

                current = tree.nextNode();
            }

            return descendents;
        }

        /// <summary>
        /// Returns the root of a given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Root(Node node)
        {
            /* The root of an object is itself, if its parent is null, or else it is the root of its parent. The root of a tree is any object participating in that tree whose parent is null. */
            if (node.parentNode is null)
                return node;

            return node.parentNode.getRootNode();
        }

        /// <summary>
        /// Returns the (shadow-including) root of a given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Shadow_Including_Root(Node node)
        {/* Docs: https://dom.spec.whatwg.org/#concept-shadow-including-root */
            /* The shadow-including root of an object is its root’s host’s shadow-including root, if the object’s root is a shadow root, and its root otherwise. */
            Node rootNode = Get_Root(node);
            while (rootNode is ShadowRoot)
            {
                Node next = Get_Root(((ShadowRoot)rootNode).Host);
                if (!(next is ShadowRoot))
                {
                    return rootNode;
                }

                rootNode = next;
            }

            return rootNode;
        }

        /// <summary>
        /// Returns a list of all descendents of <paramref name="node"/> whose parent node is <paramref name="node"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<Node> Get_Children(Node node, NodeFilter Filter = null)
        {
            var list = new LinkedList<Node>();
            Node current = node.firstChild;
            if (Filter is object)
            {
                while (current is object)
                {
                    var fr = Filter.acceptNode(current);
                    if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                    else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                    {
                        list.AddLast(current);
                    }
                    current = current.nextSibling;
                }
            }
            else
            {
                while (current is object)
                {
                    list.AddLast(current);
                    current = current.nextSibling;
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the Nth descendent of <paramref name="node"/> whose parent node is <paramref name="node"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Nth_Child(Node node, uint Nth, NodeFilter Filter = null)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            Node current = node.firstChild;
            if (Filter is object)
            {
                while (current is object)
                {
                    var fr = Filter.acceptNode(current);
                    if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                    else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                    {
                        if (--Nth <= 0)
                        {
                            return current;
                        }
                    }
                    current = current.nextSibling;
                }
            }
            else
            {
                while (current is object)
                {
                    if (--Nth <= 0)
                    {
                        return current;
                    }
                    current = current.nextSibling;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a list of all descendents of <paramref name="node"/> whose parent node is <paramref name="node"/> and whom match the given <typeparamref name="NodeType"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Children<NodeType>(Node node, NodeFilter Filter = null) where NodeType : INode
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            Node current = node.firstChild;
            if (Filter is object)
            {
                while (current is object)
                {
                    if (current is NodeType childAsType)
                    {
                        var fr = Filter.acceptNode(current);
                        if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                        {
                            list.AddLast(childAsType);
                        }
                    }
                    current = current.nextSibling;
                }
            }
            else
            {
                while (current is object)
                {
                    if (current is NodeType childAsType)
                    {
                        list.AddLast(childAsType);
                        current = current.nextSibling;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the Nth descendent of <paramref name="node"/> whose parent node is <paramref name="node"/> and whom matches the given <typeparamref name="NodeType"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Nth_Child<NodeType>(Node node, uint Nth, NodeFilter Filter = null) where NodeType : INode
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            Node current = node.firstChild;
            if (Filter is object)
            {
                while (current is object)
                {
                    if (current is NodeType childAsType)
                    {
                        var fr = Filter.acceptNode(current);
                        if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                        {
                            if (--Nth <= 0)
                            {
                                return childAsType;
                            }
                        }
                    }
                    current = current.nextSibling;
                }
            }
            else
            {
                while (current is object)
                {
                    if (current is NodeType childAsType)
                    {
                        if (--Nth <= 0)
                        {
                            return childAsType;
                        }
                    }
                    current = current.nextSibling;
                }
            }

            return default(NodeType);
        }


        /// <summary>
        /// Returns the first immediate descendent which matches the given <paramref name="Filter"/> and Type <typeparamref name="NodeType"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_First_Child<NodeType>(Node node, NodeFilter Filter = null) where NodeType : INode
        {
            return Get_Nth_Child<NodeType>(node, 1, Filter);
        }

        /// <summary>
        /// Returns the first descendent of <paramref name="node"/> whose parent node is <paramref name="node"/> and which matches the given <paramref name="Filter"/> and <typeparamref name="NodeType"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Last_Child<NodeType>(Node node, NodeFilter Filter = null) where NodeType : INode
        {
            Node current = node.lastChild;
            while (current is object)
            {
                if (current is NodeType childAsType)
                {
                    if (Filter is object)
                    {
                        var fr = Filter.acceptNode(current);
                        if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                        {
                            return childAsType;
                        }
                    }
                    else
                    {
                        return childAsType;
                    }
                }

                current = current.previousSibling;
            }

            return default(NodeType);
        }


        /// <summary>
        /// Returns the descendents of <paramref name="element"/> whose parent node is <paramref name="element"/> and whom matches the given <typeparamref name="ElementType"/>
        /// <param name="element">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ElementType> Get_Element_Children<ElementType>(Element element, NodeFilter Filter = null) where ElementType : Element
        {
            LinkedList<ElementType> list = new LinkedList<ElementType>();
            Element child = element.firstElementChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    if (child is ElementType childAsType)
                    {
                        var fr = Filter.acceptNode(child);
                        if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                        {
                            list.AddLast(childAsType);
                        }
                    }
                    child = child.nextElementSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    if (child is ElementType childAsType)
                    {
                        list.AddLast(childAsType);
                    }
                    child = child.nextElementSibling;
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the Nth descendent of <paramref name="element"/> whose parent node is <paramref name="element"/> and whom matches the given <typeparamref name="ElementType"/>
        /// <param name="element">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementType Get_Nth_Element_Child<ElementType>(Element element, uint Nth, NodeFilter Filter = null) where ElementType : Element
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            Element child = element.firstElementChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    if (child is ElementType childAsType)
                    {
                        var fr = Filter.acceptNode(child);
                        if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                        {
                            if (--Nth <= 0)
                            {
                                return childAsType;
                            }
                        }
                    }
                    child = child.nextElementSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    if (child is ElementType childAsType)
                    {
                        if (--Nth <= 0)
                        {
                            return childAsType;
                        }
                    }
                    child = child.nextElementSibling;
                }
            }

            return default(ElementType);
        }

        /// <summary>
        /// Returns the first immediate (<see cref="Element"/>) descendent which matches the given <paramref name="Filter"/> and Type <typeparamref name="ElementType"/>
        /// <param name="element">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementType Get_First_Element_Child<ElementType>(Element element, NodeFilter Filter = null) where ElementType : Element
        {
            return Get_Nth_Element_Child<ElementType>(element, 1, Filter);
        }

        /// <summary>
        /// Returns the last (<see cref="Element"/>) descendent of <paramref name="element"/> whose parent node is <paramref name="element"/> and whom matches the given <paramref name="Filter"/> and <typeparamref name="ElementType"/>
        /// <param name="element">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ElementType Get_Last_Element_Child<ElementType>(Element element, NodeFilter Filter = null) where ElementType : Node
        {
            Element child = element.lastElementChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    if (child is ElementType childAsType)
                    {
                        var fr = Filter.acceptNode(child);
                        if (fr == ENodeFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == ENodeFilterResult.FILTER_ACCEPT)
                        {
                            return childAsType;
                        }
                    }
                    child = child.previousElementSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    if (child is ElementType childAsType)
                    {
                        return childAsType;
                    }
                    child = child.previousElementSibling;
                }
            }

            return null;
        }


        #region Relationship Traversals
        /// <summary>
        /// Returns the node where a given <paramref name="node"/> node meets the given <paramref name="ancestor"/>, if any.
        /// </summary>
        /// <param name="node">Node to begin searching from</param>
        /// <param name="ancestor">Target stopping point for the search</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Node Get_Junction(Node node, Node ancestor)
        {
            if (node is null) throw new ArgumentNullException(nameof(node));
            if (ancestor is null) throw new ArgumentNullException(nameof(ancestor));

            if (ReferenceEquals(node, ancestor) || ReferenceEquals(node.parentNode, ancestor))
            {
                return node;
            }

            var current = node;
            while (current is object && !ReferenceEquals(current.parentNode, ancestor))
            {
                current = current.parentNode;
            }

            return current;
        }
        #endregion
        #endregion



        #region Element Creation
        internal static Element createElementNS(Document document, string qualifiedName, string Namespace, ElementCreationOptions options = null)
        {
            XMLCommon.Validate_And_Extract(Namespace, qualifiedName, out string Prefix, out string LocalName);
            return Create_Element(document, LocalName, Prefix);
        }

        internal static Element Create_Element(Document document, AtomicString localName, string Namespace, string prefix = null, string customClassName = null, bool synchronousCustomElementsFlag = false)
        {/* Docs: https://dom.spec.whatwg.org/#concept-create-element */
            /* 3) Let result be null. */
            Element result = null;

            /* 4) Let definition be the result of looking up a custom element definition given document, namespace, localName, and is. */
            /* XXX: Implement custom elements */
            object definition = null;
            /* 5) If definition is non-null, and definition’s name is not equal to its local name (i.e., definition represents a customized built-in element), then: */
            /* 6) Otherwise, if definition is non-null, then: */

            /* 7) Otherwise: */
            /* 1) Let interface be the element interface for localName and namespace. */
            /* 2) Set result to a new element that implements interface, with no attributes, namespace set to namespace, namespace prefix set to prefix, local name set to localName, custom element state set to "uncustomized", custom element definition set to null, is value set to is, and node document set to document. */
            /* 3) If namespace is the HTML namespace, and either localName is a valid custom element name or is is non-null, then set result’s custom element state to "undefined". */

            var ctor = Lookup_Element_Interface(localName, Namespace);
            if (ctor is null)
                throw new Exception($"Cannot find interface constructor for element type: \"{localName}\"");
            /* XXX: Just need to make sure that every tag type has an interface type correctly specified for it */
            result = (Element)ctor.Invoke(new object[] { document, localName, prefix, Namespace });

            return result;
        }
        #endregion

        #region Focus

        /// <summary>
        /// Returns the chain of focus up through the hierarchy from the given node to it's owning document
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        internal static LinkedList<FocusableArea> Get_Focus_Chain(FocusableArea subject)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#focus-chain */
            if (subject is null)
            {
                return new LinkedList<FocusableArea>();
            }

            /* 1) Let current object be subject. */
            FocusableArea currentObject = subject;
            /* 2) Let output be an empty list. */
            var output = new LinkedList<FocusableArea>();
            /* 3) Loop: Append current object to output. */
            while (currentObject is object)
            {
                output.AddLast(currentObject);

#if ENABLE_HTML
                /* 4) If current object is an area element's shape, append that area element to output. */
                if (currentObject.FocusTarget is HTMLAreaElement area)
                {
                    output.AddLast(area);
                }
                /* Otherwise) if current object is a focusable area whose DOM anchor is an element that is not current object itself, append that DOM anchor element to output. */
                else
                {
#endif
                if (currentObject.DOMAnchor is Element element && !ReferenceEquals(currentObject.DOMAnchor, currentObject.FocusTarget))
                {
                    output.AddLast(element);
                }
#if ENABLE_HTML
                }
#endif

                /* 5) If current object is a Document in a nested browsing context, let current object be its browsing context container, and return to the step labeled loop. */
                if (currentObject.FocusTarget is Document document && document.BrowsingContext is IBrowsingContextContainer)
                {
                    currentObject = document.BrowsingContext;
                }
                else
                {
                    break;
                }
            }

            return output;
        }

        internal static void Run_Focusing_Steps(FocusableArea new_focus_target)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#focusing-steps */
            //XXX:
            throw new NotImplementedException();
        }

        internal static void Run_Unfocusing_Steps(FocusableArea new_focus_target)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#unfocusing-steps */
            //XXX:
            throw new NotImplementedException();
        }

        internal static void Run_Focus_Update_Steps(IReadOnlyCollection<FocusableArea> oldChain, IReadOnlyCollection<FocusableArea> newChain, FocusableArea newFocusTarget)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#focus-update-steps */
            //XXX:
            throw new NotImplementedException();
        }
        #endregion
    }
}
