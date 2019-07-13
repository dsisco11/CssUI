using System.Linq;
using System.Collections.Generic;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;
using CssUI.DOM.Traversal;
using CssUI.CSS;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Internal;
using System;
using CssUI.Internal;

namespace CssUI.DOM
{
    public static class DOMCommon
    {
        #region Metadata
        /// <summary>
        /// Official HTML namespace string
        /// </summary>
        public const string HTMLNamespace = "http://www.w3.org/1999/xhtml";
        /// <summary>
        /// Official MathML namespace string
        /// </summary>
        public const string MathMLNamespace = "http://www.w3.org/1998/Math/MathML";
        /// <summary>
        /// Official SVG namespace string
        /// </summary>
        public const string SVGNamespace = "http://www.w3.org/2000/svg";
        /// <summary>
        /// Official XLink namespace string
        /// </summary>
        public const string XLinkNamespace = "http://www.w3.org/1999/xlink";
        /// <summary>
        /// Official XML namespace string
        /// </summary>
        public const string XMLNamespace = "http://www.w3.org/XML/1998/namespace";
        /// <summary>
        /// Official XMLNS namespace string
        /// </summary>
        public const string XMLNSNamespace = "http://www.w3.org/2000/xmlns/";
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
                case DOMCommon.HTMLNamespace:
                    return "application/xhtml+xml";
                case DOMCommon.SVGNamespace:
                    return "image/svg+xml";
                default:
                    return "application/xml";
            }
        }

        internal static Type Lookup_Element_Interface(string localName, string Namespace)
        {
            if (Namespace == DOMCommon.HTMLNamespace)
            {
                EElementTag? tag = DomLookup.Enum_From_Keyword<EElementTag>(localName);
                if (!tag.HasValue)
                    throw new Exception($"unable to find tag type for HTML tag matching \"{localName}\"");

                switch (tag.Value)
                {
                    case EElementTag.Div:
                        return typeof(HTMLElement);
                    case EElementTag.Body:
                        return typeof(HTMLBodyElement);
                    case EElementTag.Template:
                        return typeof(HTMLTemplateElement);
                    case EElementTag.Slot:
                        return typeof(HTMLSlotElement);
                }
            }

            return typeof(Element);
        }
        #endregion

        #region Ordered Sets
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Serialize_Ordered_Set(IEnumerable<string> set)
        {
            return string.Join("\u0020", set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<string> Parse_Ordered_Set(string input)
        {
            return input.Split('\u0020').ToArray();
        }
        #endregion
        
        #region CSS Selectors

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Element> Scope_Match_Selector_String(Node node, string selector)
        {
            /* The closest(selectors) method, when invoked, must run these steps: */
            /* 1) Let s be the result of parse a selector from selectors. [SELECTORS4] */
            var Selector = new CssSelector(selector);
            /* 2) If s is failure, throw a "SyntaxError" DOMException. */
            if (ReferenceEquals(null, Selector))
            {
                throw new SyntaxError("Could not parse selector.");
            }
            /* 3) Return the result of match a selector against a tree with s and node’s root using scoping root node. [SELECTORS4]. */
            return Selector.Match_Against_Tree(new Node[] { node }, node.getRootNode());
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
        /// Returns true if the node is a slot
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Slot(Node node)
        {
            return node is Element element && 0==string.Compare("slot", element.tagName);
        }
        #endregion

        #region Slottables
        internal static ISlot find_slot(ISlottable slottable, bool open_flag = false)
        {/* Docs: https://dom.spec.whatwg.org/#find-a-slot */
            if (ReferenceEquals(null, slottable.parentNode))
                return null;

            var shadow = slottable.parentNode.getRootNode() as ShadowRoot;
            if (ReferenceEquals(null, shadow))
                return null;

            if (open_flag && shadow.Mode != EShadowRootMode.Open)
                return null;

            /* 5) Return the first slot in tree order in shadow’s descendants whose name is slotable’s name, if any, and null otherwise. */
            var tree = new TreeWalker(shadow, ENodeFilterMask.SHOW_ALL);
            var node = tree.nextNode();
            while(!ReferenceEquals(null, node))
            {
                if (node is HTMLSlotElement slot)
                {
                    if (0 == string.Compare(slot.Name, (slottable as ISlottable).Name))
                        return slot;
                }
                node = tree.nextNode();
            }

            return null;
        }

        internal static List<ISlottable> find_slotables(ISlot slot)
        {/* Docs: https://dom.spec.whatwg.org/#find-slotables */
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
                    var foundSlot = DOMCommon.find_slot(slotable);
                    if (ReferenceEquals(foundSlot, slot))
                        result.Add(slotable);
                }
            }

            return result;
        }

        internal static List<ISlottable> find_flattened_slotables(ISlot slot)
        {/* Docs: https://dom.spec.whatwg.org/#find-flattened-slotables */
            var result = new List<ISlottable>();
            if (!(slot.getRootNode() is ShadowRoot))
                return result;

            var slotables = DOMCommon.find_slotables(slot);
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
                    var temporaryResult = DOMCommon.find_flattened_slotables(node as ISlot);
                    result.AddRange(temporaryResult);
                }
                else
                {
                    result.Add(node);
                }
            }

            /* 6) Return result. */
            return result;
        }

        internal static void assign_slottables(ISlot slot)
        {/* Docs: https://dom.spec.whatwg.org/#assign-slotables */
            var slotables = DOMCommon.find_slotables(slot);

            bool match = true;
            if (slotables.Count() != slot.Assigned.Count())
                match = false;
            else
            {
                for (int i=0; i<slotables.Count(); i++)
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
                slot.ownerDocument.window.SignalSlots.Add(slot);
                slot.ownerDocument.window.QueueObserverMicroTask();
            }
            /* 3) Set slot’s assigned nodes to slotables. */
            slot.Assigned = slotables;
            /* 4) For each slotable in slotables, set slotable’s assigned slot to slot. */
            foreach(ISlottable slotable in slotables)
            {
                slotable.assignedSlot = slot;
            }
        }

        internal static void assign_a_slot(ISlottable slotable)
        {/* Docs: https://dom.spec.whatwg.org/#assign-a-slot */
            var slot = DOMCommon.find_slot(slotable);
            if (!ReferenceEquals(null, slot))
                DOMCommon.assign_slottables(slot);
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
            while(!ReferenceEquals(node, null))
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

            if (A.getRootNode() is ShadowRoot aShadow && Is_Shadow_Including_Inclusive_Descendant(aShadow.Host, B))
                return true;

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
                if (!ReferenceEquals(null, doc.Host))
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
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Range_Nodes(Range range)
        {
            var commonAncestor = range.startContainer;
            while (!DOMCommon.Is_Inclusive_Ancestor(commonAncestor, range.endContainer))
            {
                commonAncestor = commonAncestor.parentNode;
            }

            var containedChildren = DOMCommon.Get_Descendents(commonAncestor, new FilterRangeContains(range));
            return containedChildren;
        }

        /// <summary>
        /// Returns a list of all ancestors for the given node, that is; the chain of parent elements all the way up to the root element.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Ancestors(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            LinkedList<Node> list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (!ReferenceEquals(null, current))
            {
                list.AddLast(current);
                current = tree.parentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all (inclusive) ancestors for the given node, that is; the chain of parent elements all the way up to the root element.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Inclusive_Ancestors(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            LinkedList<Node> list = new LinkedList<Node>();
            list.AddLast(node);
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.parentNode();
            while (!ReferenceEquals(null, current))
            {
                list.AddLast(current);
                current = tree.parentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all descendents for the given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            LinkedList<Node> list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (!ReferenceEquals(null, current))
            {
                list.AddLast(current);
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all descendents for the given node and, in addition, all of its ShadowDOM descendents
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Shadow_Including_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            /* XXX: Shadow dom stuff */
            LinkedList<Node> list = new LinkedList<Node>();
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (!ReferenceEquals(null, current))
            {
                list.AddLast(current);
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all (inclusive) descendents for the given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Inclusive_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            LinkedList<Node> list = new LinkedList<Node>();
            list.AddLast(node);
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (!ReferenceEquals(null, current))
            {
                list.AddLast(current);
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all (inclusive) descendents for the given node and, in addition, all of its ShadowDOM (inclusive) descendents
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Shadow_Including_Inclusive_Descendents(Node node, NodeFilter Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
        {
            /* XXX: Shadow dom stuff */
            LinkedList<Node> list = new LinkedList<Node>();
            list.AddLast(node);
            TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
            Node current = tree.nextNode();
            while (!ReferenceEquals(null, current))
            {
                list.AddLast(current);
                current = tree.nextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all nodes that preceed the given node (siblings)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Preceeding(Node node, NodeFilter Filter = null)
        {
            LinkedList<Node> list = new LinkedList<Node>();
            Node current = node.previousSibling;
            while (!ReferenceEquals(null, current))
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
        /// Returns a list of all nodes that follow the given node (siblings)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Node> Get_Following(Node node, NodeFilter Filter = null)
        {
            LinkedList<Node> list = new LinkedList<Node>();
            Node current = node.nextSibling;
            while (!ReferenceEquals(null, current))
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
        /// Returns a list of <see cref="Element"/>s matching <paramref name="qualifiedName"/>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="qualifiedName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Element> Get_Elements_By_Qualified_Name(Node root, string qualifiedName)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbytagname */
            /* 1) If qualifiedName is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches only descendant elements. */
            if (qualifiedName == "\u002A")
            {
                LinkedList<Element> descendents = new LinkedList<Element>();
                var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
                Node node = tree.nextNode();
                while (!ReferenceEquals(null, node))
                {
                    descendents.AddLast((Element)node);
                    node = tree.nextNode();
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
                Node node = tree.nextNode();
                while (!ReferenceEquals(null, node))
                {
                    var element = (Element)node;
                    if (element.NamespaceURI == DOMCommon.HTMLNamespace)
                    {
                        if (0 == string.Compare(qualifiedName, element.tagName.ToLowerInvariant()))
                        {
                            descendents.AddLast(element);
                        }
                    }
                    else if (0 == string.Compare(qualifiedName, element.tagName))
                    {
                        descendents.AddLast(element);
                    }

                    node = tree.nextNode();
                }

                return descendents;
            }
            else
            {
                /* 3) Otherwise, return a HTMLCollection rooted at root, whose filter matches descendant elements whose qualified name is qualifiedName. */
                LinkedList<Element> descendents = new LinkedList<Element>();
                var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
                Node node = tree.nextNode();
                while (!ReferenceEquals(null, node))
                {
                    var element = (Element)node;
                    if (element.NamespaceURI == DOMCommon.HTMLNamespace)
                    {
                        if (0 == string.Compare(qualifiedName, element.tagName.ToLowerInvariant()))
                        {
                            descendents.AddLast(element);
                        }
                    }
                    else if (0 == string.Compare(qualifiedName, element.tagName))
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
        /// <param name="root"></param>
        /// <param name="Namespace"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Element> Get_Elements_By_Namespace_And_Local_Name(Node root, string Namespace, string localName)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbytagnamens */
            if (Namespace.Length <= 0)
                Namespace = null;

            /* 2) If both namespace and localName are "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements. */
            if (Namespace == "\u002A" && localName == "\u002A")
                return DOMCommon.Get_Descendents(root, null, ENodeFilterMask.SHOW_ELEMENT).Cast<Element>();
            /* 3) Otherwise, if namespace is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements whose local name is localName. */
            var localNameFilter = new FilterLocalName(localName);
            if (Namespace == "\u002A")
                return DOMCommon.Get_Descendents(root, localNameFilter, ENodeFilterMask.SHOW_ELEMENT).Cast<Element>();
            /* 4) Otherwise, if localName is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements whose namespace is namespace. */
            var NamespaceFilter = new FilterNamespace(Namespace);
            if (localName == "\u002A")
                return DOMCommon.Get_Descendents(root, NamespaceFilter, ENodeFilterMask.SHOW_ELEMENT).Cast<Element>();

            /* 5) Otherwise, return a HTMLCollection rooted at root, whose filter matches descendant elements whose namespace is namespace and local name is localName. */
            return DOMCommon.Get_Descendents(root, localNameFilter, ENodeFilterMask.SHOW_ELEMENT)
                .Where(node => NamespaceFilter.acceptNode(node) == ENodeFilterResult.FILTER_ACCEPT)
                .Cast<Element>();
        }

        /// <summary>
        /// Returns a list of <see cref="Element"/>s which match <paramref name="localName"/> and <paramref name="Namespace"/>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="Namespace"></param>
        /// <param name="localName"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Element> Get_Elements_By_Class_Name(Node root, string classNames)
        {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbyclassname */
            var classes = DOMCommon.Parse_Ordered_Set(classNames.ToLowerInvariant()).Cast<AtomicString>();
            /* 2) If classes is the empty set, return an empty HTMLCollection. */
            if (classes.Count() <= 0)
                return new Element[] { };

            /* 3) Return a HTMLCollection rooted at root, whose filter matches descendant elements that have all their classes in classes. */
            var descendents = new LinkedList<Element>();
            var tree = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
            Node node = tree.nextNode();
            while(!ReferenceEquals(null, node))
            {
                Element E = node as Element;

                if (E.classList.ContainsAll(classes))
                    descendents.AddLast(E);

                if (!ReferenceEquals(null, E))
                    descendents.AddLast(E);

                node = tree.nextNode();
            }

            return descendents;
        }
        #endregion

        #region Internal Algorithms
        internal static Element createElementNS(Document document, string qualifiedName, string Namespace, ElementCreationOptions options = null)
        {
            XMLCommon.Validate_And_Extract(Namespace, qualifiedName, out string Prefix, out string LocalName);
            return Create_Element(document, LocalName, Prefix);
        }

        internal static Element Create_Element(Document document, string localName, string Namespace, string prefix = null, string customClassName = null, bool synchronousCustomElementsFlag = false)
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

            var Interface = Lookup_Element_Interface(localName, Namespace);
            var ctor = Interface.GetConstructor(new Type[]{ typeof(Document), typeof(string), typeof(string), typeof(string) } );
            if (ReferenceEquals(null, ctor))
                throw new Exception($"Cannot find interface constructor for element type: \"{localName}\"");
            /* XXX: Just need to make sure that every tag type has an interface type correctly specified for it */
            result = (Element)ctor.Invoke(new object[] { document, localName, prefix, Namespace });

            return result;
        }
        #endregion
    }
}
