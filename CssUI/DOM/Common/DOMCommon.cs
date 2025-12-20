using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CssUI.CSS;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Geometry;
using CssUI.DOM.Nodes;
using CssUI.DOM.Traversal;
#if ENABLE_HTML
using CssUI.HTML;
#endif

namespace CssUI.DOM;

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

    internal static ConstructorInfo? Lookup_Element_Interface(AtomicString localName, AtomicString Namespace)
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
    public static bool Is_CommonTextNode(Node? node)
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
            top = !top.HasValue ? rect.Top : Math.Min(top.Value, rect.Top);
            right = !right.HasValue ? rect.Right : Math.Max(right.Value, rect.Right);
            bottom = !bottom.HasValue ? rect.Bottom : Math.Max(bottom.Value, rect.Bottom);
            left = !left.HasValue ? rect.Left : Math.Min(left.Value, rect.Left);
        }

        return new DOMRect(left.GetValueOrDefault(), top.GetValueOrDefault(), (right.GetValueOrDefault() - left.GetValueOrDefault()), (bottom.GetValueOrDefault() - top.GetValueOrDefault()));
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
    internal static ISlot? Find_Slot(ISlottable slottable, bool open_flag = false)
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
                ISlottable slotable = (node as ISlottable)!;
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
                    slotables.Add((child as ISlottable)!);
                }
            }
        }

        /* 5) For each node in slotables: */
        foreach (ISlottable node in slotables)
        {
            if (node is ISlot && node.getRootNode() is ShadowRoot)
            {
                var temporaryResult = DOMCommon.Find_Flattened_Slotables((node as ISlot)!);
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
        while (node is not null)
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
        if (A.getRootNode() is ShadowRoot aRootShadow && Is_Shadow_Including_Inclusive_Descendant(aRootShadow.Host!, B))
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
            if (doc.Host is not null)
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
        if (A.parentNode is null || B.parentNode is null)
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
        Node? node = A.nextSibling;
        while (node is not null)
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
        Node? node = A.previousSibling;
        while (node is not null)
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
            commonAncestor = commonAncestor.parentNode!;
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
    public static LinkedList<Node> Get_Ancestors(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        var list = new LinkedList<Node>();
        TreeWalker tree = new TreeWalker(node!, FilterMask, Filter);
        Node? current = tree.parentNode();
        while (current is not null)
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
    public static LinkedList<NodeType> Get_Ancestors<NodeType>(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
    {
        LinkedList<NodeType> list = new LinkedList<NodeType>();
        TreeWalker tree = new TreeWalker(node!, FilterMask, Filter);
        Node? current = tree.parentNode();
        while (current is not null)
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
    public static Node? Get_Nth_Ancestor(Node node, uint Nth, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.parentNode();
        while (current is not null)
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
    public static NodeType? Get_Nth_Ancestor<NodeType>(Node node, uint Nth, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.parentNode();
        while (current is not null)
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
    public static LinkedList<Node> Get_Inclusive_Ancestors(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        var list = new LinkedList<Node>();
        list.AddLast(node);
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.parentNode();
        while (current is not null)
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
    public static LinkedList<Node> Get_Descendents(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        var list = new LinkedList<Node>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextNode();
        while (current is not null)
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
    public static LinkedList<NodeType> Get_Descendents<NodeType>(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
    {
        LinkedList<NodeType> list = new LinkedList<NodeType>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextNode();
        while (current is not null)
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
    public static Node? Get_Nth_Descendant(Node node, uint Nth, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextNode();
        while (current is not null)
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
    public static NodeType? Get_Nth_Descendant<NodeType>(Node node, uint Nth, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextNode();
        while (current is not null)
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
    public static LinkedList<Node> Get_Shadow_Including_Descendents(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
#if ENABLE_HTML
        var list = new LinkedList<Node>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextNode();
        while (current is object)
        {
            if (Is_Shadow_Including_Descendant(current, node))
            {
                list.AddLast(current);
            }

            if (current.Is_ShadowHost)
            {
                var element = current as Element;
                Get_Shadow_Including_Descendents(element!.shadowRoot!);
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
    public static LinkedList<Node> Get_Inclusive_Descendents(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        var list = new LinkedList<Node>();
        list.AddLast(node);
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextNode();
        while (current is not null)
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
    public static LinkedList<Node> Get_Shadow_Including_Inclusive_Descendents(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
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
    public static LinkedList<Node> Get_Previous(Node node, NodeFilter? Filter = null)
    {
        var list = new LinkedList<Node>();

        Node? current = node.previousSibling;
        while (current is not null)
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
    public static LinkedList<NodeType> Get_Previous<NodeType>(Node node, NodeFilter? Filter = null) where NodeType : INode
    {
        var list = new LinkedList<NodeType>();

        Node? current = node.previousSibling;
        while (current is not null)
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
    public static Node? Get_Nth_Previous(Node node, uint Nth, NodeFilter? Filter = null)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        Node? current = node.previousSibling;
        while (current is not null)
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
    public static LinkedList<Node> Get_Preceeding(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        var list = new LinkedList<Node>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

        Node? current = tree.previousSibling();
        while (current is not null)
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
    public static LinkedList<NodeType> Get_Preceeding<NodeType>(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        LinkedList<NodeType> list = new LinkedList<NodeType>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

        Node? current = tree.previousSibling();
        while (current is not null)
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
    public static Node? Get_Nth_Preceeding(Node node, uint Nth, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.previousSibling();
        while (current is not null)
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
    public static LinkedList<Node> Get_Following(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        var list = new LinkedList<Node>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

        Node? current = tree.nextSibling();
        while (current is not null)
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
    public static LinkedList<NodeType> Get_Following<NodeType>(Node node, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL) where NodeType : INode
    {
        var list = new LinkedList<NodeType>();
        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);

        Node? current = tree.nextSibling();
        while (current is not null)
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
    public static Node? Get_Nth_Following(Node node, uint Nth, NodeFilter? Filter = null, ENodeFilterMask FilterMask = ENodeFilterMask.SHOW_ALL)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        TreeWalker tree = new TreeWalker(node, FilterMask, Filter);
        Node? current = tree.nextSibling();
        while (current is not null)
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
    public static LinkedList<Node> Get_Next(Node node, NodeFilter? Filter = null)
    {
        var list = new LinkedList<Node>();
        Node? current = node.nextSibling;
        while (current is not null)
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
    public static LinkedList<NodeType> Get_Next<NodeType>(Node node, NodeFilter? Filter = null) where NodeType : INode
    {
        var list = new LinkedList<NodeType>();
        Node? current = node.nextSibling;
        while (current is not null)
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
    public static Node? Get_Nth_Next(Node node, uint Nth, NodeFilter? Filter = null)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        Node? current = node.nextSibling;
        while (current is not null)
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
            Node? current = tree.nextNode();
            while (current is not null)
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
            Node? current = tree.nextNode();
            while (current is not null)
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
            while (node is not null)
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
    public static LinkedList<Element> Get_Elements_By_Namespace_And_Local_Name(Node root, string? Namespace, string localName)
    {/* Docs: https://dom.spec.whatwg.org/#concept-getelementsbytagnamens */
        if (Namespace?.Length <= 0)
            Namespace = null;

        /* 2) If both namespace and localName are "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements. */
        if (string.Equals(Namespace, "\u002A") && localName.Equals("\u002A"))
            return Get_Descendents<Element>(root, null, ENodeFilterMask.SHOW_ELEMENT);

        /* 3) Otherwise, if namespace is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements whose local name is localName. */
        var localNameFilter = new FilterLocalName(localName!);
        if (string.Equals(Namespace, "\u002A"))
            return Get_Descendents<Element>(root, localNameFilter, ENodeFilterMask.SHOW_ELEMENT);

        /* 4) Otherwise, if localName is "*" (U+002A), return a HTMLCollection rooted at root, whose filter matches descendant elements whose namespace is namespace. */
        var NamespaceFilter = new FilterNamespace(Namespace!);
        if (string.Equals(localName, "\u002A"))
            return Get_Descendents<Element>(root, NamespaceFilter, ENodeFilterMask.SHOW_ELEMENT);

        /* 5) Otherwise, return a HTMLCollection rooted at root, whose filter matches descendant elements whose namespace is namespace and local name is localName. */
        var localName_Namespace_Filter = new FilterLocalName_Namespace(localName!, Namespace!);
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
        Node? current = tree.nextNode();
        while (current is not null)
        {
            Element? E = current as Element;

            if (E.classList.ContainsAll(classes))
                descendents.AddLast(E);

            if (E is not null)
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
            Node next = Get_Root(((ShadowRoot)rootNode).Host!);
            if (next is not ShadowRoot)
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
    public static LinkedList<Node> Get_Children(Node node, NodeFilter? Filter = null)
    {
        var list = new LinkedList<Node>();
        Node? current = node.firstChild;
        if (Filter is not null)
        {
            while (current is not null)
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
            while (current is not null)
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
    public static Node? Get_Nth_Child(Node node, uint Nth, NodeFilter? Filter = null)
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        Node? current = node.firstChild;
        if (Filter is not null)
        {
            while (current is not null)
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
            while (current is not null)
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
    public static LinkedList<NodeType> Get_Children<NodeType>(Node node, NodeFilter? Filter = null) where NodeType : INode
    {
        LinkedList<NodeType> list = new LinkedList<NodeType>();
        Node? current = node.firstChild;
        if (Filter is not null)
        {
            while (current is not null)
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
            while (current is not null)
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
    public static NodeType? Get_Nth_Child<NodeType>(Node node, uint Nth, NodeFilter? Filter = null) where NodeType : INode
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        Node? current = node.firstChild;
        if (Filter is not null)
        {
            while (current is not null)
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
            while (current is not null)
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
    public static NodeType? Get_First_Child<NodeType>(Node node, NodeFilter? Filter = null) where NodeType : INode
    {
        return Get_Nth_Child<NodeType>(node, 1, Filter);
    }

    /// <summary>
    /// Returns the first descendent of <paramref name="node"/> whose parent node is <paramref name="node"/> and which matches the given <paramref name="Filter"/> and <typeparamref name="NodeType"/>
    /// <param name="node">The node to start searching from</param>
    /// <param name="Filter">Filter used for determining which nodes to allow</param>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NodeType? Get_Last_Child<NodeType>(Node node, NodeFilter? Filter = null) where NodeType : INode
    {
        Node? current = node.lastChild;
        while (current is not null)
        {
            if (current is NodeType childAsType)
            {
                if (Filter is not null)
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
    public static LinkedList<ElementType> Get_Element_Children<ElementType>(Element element, NodeFilter? Filter = null) where ElementType : Element
    {
        LinkedList<ElementType> list = new LinkedList<ElementType>();
        Element? child = element.firstElementChild;
        if (Filter is not null)
        {
            while (child is not null)
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
            while (child is not null)
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
    public static ElementType? Get_Nth_Element_Child<ElementType>(Element element, uint Nth, NodeFilter? Filter = null) where ElementType : Element
    {
        if (Nth == 0)
        {
            throw new IndexOutOfRangeException("N must be greater than 0");
        }

        Element? child = element.firstElementChild;
        if (Filter is not null)
        {
            while (child is not null)
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
            while (child is not null)
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
    public static ElementType? Get_First_Element_Child<ElementType>(Element element, NodeFilter? Filter = null) where ElementType : Element
    {
        return Get_Nth_Element_Child<ElementType>(element, 1, Filter);
    }

    /// <summary>
    /// Returns the last (<see cref="Element"/>) descendent of <paramref name="element"/> whose parent node is <paramref name="element"/> and whom matches the given <paramref name="Filter"/> and <typeparamref name="ElementType"/>
    /// <param name="element">The node to start searching from</param>
    /// <param name="Filter">Filter used for determining which nodes to allow</param>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ElementType? Get_Last_Element_Child<ElementType>(Element element, NodeFilter? Filter = null) where ElementType : Node
    {
        Element? child = element.lastElementChild;
        if (Filter is not null)
        {
            while (child is not null)
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
            while (child is not null)
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
    public static Node? Get_Junction(Node node, Node ancestor)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(ancestor);

        if (ReferenceEquals(node, ancestor) || ReferenceEquals(node.parentNode, ancestor))
        {
            return node;
        }

        Node? current = node;
        while (current is not null && !ReferenceEquals(current.parentNode, ancestor))
        {
            current = current.parentNode;
        }

        return current;
    }
    #endregion
    #endregion



    #region Element Creation
    internal static Element createElementNS(Document document, string qualifiedName, string Namespace, ElementCreationOptions? options = null)
    {
        XMLCommon.Validate_And_Extract(Namespace, qualifiedName, out string? Prefix, out string LocalName);
        return Create_Element(document, LocalName, Prefix!);
    }

    internal static Element Create_Element(Document document, AtomicString localName, string? Namespace, string? prefix = null, string? customClassName = null, bool synchronousCustomElementsFlag = false)
    {/* Docs: https://dom.spec.whatwg.org/#concept-create-element */
        /* 3) Let result be null. */
        Element? result = null;

        /* 4) Let definition be the result of looking up a custom element definition given document, namespace, localName, and is. */
        /* XXX: Implement custom elements */
        object? definition = null;
        /* 5) If definition is non-null, and definition’s name is not equal to its local name (i.e., definition represents a customized built-in element), then: */
        /* 6) Otherwise, if definition is non-null, then: */

        /* 7) Otherwise: */
        /* 1) Let interface be the element interface for localName and namespace. */
        /* 2) Set result to a new element that implements interface, with no attributes, namespace set to namespace, namespace prefix set to prefix, local name set to localName, custom element state set to "uncustomized", custom element definition set to null, is value set to is, and node document set to document. */
        /* 3) If namespace is the HTML namespace, and either localName is a valid custom element name or is is non-null, then set result’s custom element state to "undefined". */

        var ctor = Lookup_Element_Interface(localName, Namespace ?? string.Empty);
        if (ctor is null)
            throw new Exception($"Cannot find interface constructor for element type: \"{localName}\"");
        /* XXX: Just need to make sure that every tag type has an interface type correctly specified for it */
        result = (Element)ctor.Invoke(new object?[] { document, localName.ToString(), prefix, Namespace });

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
        while (currentObject is not null)
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

    #region Form-Associated Elements
#if ENABLE_HTML
    /// <summary>
    /// Checks if an element is a form-associated custom element.
    /// A form-associated custom element is an autonomous custom element whose definition has form-associated set to true.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is a form-associated custom element; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#form-associated-custom-element
    /// Form-associated custom elements have:
    /// - Static formAssociated property set to true
    /// - Are listed, labelable, submittable, and resettable form-associated elements
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Form_Associated_Custom_Element(Element? element)
    {
        // Form-associated custom elements are autonomous custom elements with formAssociated = true
        // Currently not implementing custom element registry, so return false
        // TODO: Implement custom element definition lookup when custom elements are supported
        return false;
    }

    /// <summary>
    /// Checks if an element is a listed element.
    /// Listed elements are form-associated elements that are listed in form.elements and fieldset.elements APIs.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is a listed element; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/forms.html#category-listed
    /// Listed elements: button, fieldset, input, object, output, select, textarea, form-associated custom elements
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Listed_Element(Element? element)
    {
        if (element == null)
            return false;

        // Check for standard listed HTML elements
        if (element is HTML.HTMLButtonElement ||
            element is HTML.HTMLFieldSetElement ||
            element is HTML.HTMLInputElement ||
            element is HTML.HTMLObjectElement ||
            element is HTML.HTMLOutputElement ||
            element is HTML.HTMLSelectElement ||
            element is HTML.HTMLTextAreaElement)
        {
            return true;
        }

        // Also check for form-associated custom elements
        return Is_Form_Associated_Custom_Element(element);
    }

    /// <summary>
    /// Checks if an element is a submittable element.
    /// Submittable elements can be used for constructing the entry list when a form is submitted.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is a submittable element; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/forms.html#category-submit
    /// Submittable elements: button, input, select, textarea, form-associated custom elements
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Submittable_Element(Element? element)
    {
        if (element == null)
            return false;

        // Check for standard submittable HTML elements
        if (element is HTML.HTMLButtonElement ||
            element is HTML.HTMLInputElement ||
            element is HTML.HTMLSelectElement ||
            element is HTML.HTMLTextAreaElement)
        {
            return true;
        }

        // Also check for form-associated custom elements
        return Is_Form_Associated_Custom_Element(element);
    }

    /// <summary>
    /// Checks if an element is a labelable element.
    /// Labelable elements can be associated with a label element.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is a labelable element; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/forms.html#category-label
    /// Labelable elements: button, input (not type=hidden), meter, output, progress, select, textarea, form-associated custom elements
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Labelable_Element(Element? element)
    {
        if (element == null)
            return false;

        // Check for standard labelable HTML elements
        if (element is HTML.HTMLButtonElement ||
            element is HTML.HTMLMeterElement ||
            element is HTML.HTMLOutputElement ||
            element is HTML.HTMLProgressElement ||
            element is HTML.HTMLSelectElement ||
            element is HTML.HTMLTextAreaElement)
        {
            return true;
        }

        // Input elements are labelable unless type=hidden
        if (element is HTML.HTMLInputElement inputElement)
        {
            // Input is labelable unless its type is "hidden"
            return inputElement.type != HTML.EInputType.Hidden;
        }

        // Also check for form-associated custom elements
        return Is_Form_Associated_Custom_Element(element);
    }

    /// <summary>
    /// Checks if an element is a submit button.
    /// A submit button submits the form when activated.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is a submit button; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/forms.html#concept-submit-button
    /// Submit buttons:
    /// - button element where type=submit (or type is in Auto state with no command/commandfor attributes)
    /// - input element where type=submit or type=image
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Submit_Button(Element? element)
    {
        if (element == null)
            return false;

        // Button element is a submit button if type="submit"
        if (element is HTML.HTMLButtonElement buttonElement)
        {
            return buttonElement.type == HTML.EButtonType.Submit;
        }

        // Input element is a submit button if type=submit or type=image
        if (element is HTML.HTMLInputElement inputElement)
        {
            return inputElement.type == HTML.EInputType.Submit || inputElement.type == HTML.EInputType.Image;
        }

        return false;
    }

    /// <summary>
    /// Checks if an element is an autocapitalize-inheriting element.
    /// These elements inherit the autocapitalize attribute from their form owner.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element inherits autocapitalize; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/forms.html#category-autocapitalize
    /// Autocapitalize-and-autocorrect-inheriting elements: button, fieldset, input, output, select, textarea
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Autocapitalize_Inheriting_Element(Element? element)
    {
        if (element == null)
            return false;

        // Check for autocapitalize-and-autocorrect inheriting elements
        return element is HTML.HTMLButtonElement ||
               element is HTML.HTMLFieldSetElement ||
               element is HTML.HTMLInputElement ||
               element is HTML.HTMLOutputElement ||
               element is HTML.HTMLSelectElement ||
               element is HTML.HTMLTextAreaElement;
    }
#endif
    #endregion

    #region Editing
#if ENABLE_HTML
    /// <summary>
    /// Checks if an element is an editing host.
    /// An editing host is an HTML element with contenteditable="true" or "plaintext-only", 
    /// or a child HTML element of a Document with designMode enabled.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is an editing host; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/interaction.html#editing-host
    /// An editing host is either:
    /// - An HTML element with its contenteditable attribute in the true state or plaintext-only state
    /// - A child HTML element of a Document whose design mode enabled is true
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Editing_Host(Element? element)
    {
        if (element == null)
            return false;

        // Check if element has contenteditable set to true or plaintext-only
        var contentEditableAttr = element.getAttribute("contenteditable");
        if (contentEditableAttr != null)
        {
            var value = contentEditableAttr.AsString()?.ToLowerInvariant() ?? "";
            if (value == "true" || value == "" || value == "plaintext-only")
            {
                return true;
            }
        }

        // Check if the document's designMode is enabled
        // When designMode is "on", all HTML elements that are children of the document become editing hosts
        var ownerDocument = element.ownerDocument;
        if (ownerDocument != null)
        {
            // Check document's designMode
            // TODO: Implement designMode property on Document
            // if (ownerDocument.designMode == "on") return true;
        }

        return false;
    }
#endif
    #endregion

    #region Canvas
#if ENABLE_HTML
    /// <summary>
    /// Checks if an element is being used as relevant canvas fallback content.
    /// An element is being used as canvas fallback content if its nearest canvas element ancestor 
    /// is being rendered and represents embedded content.
    /// </summary>
    /// <param name="element">The element to check</param>
    /// <returns>True if the element is being used as canvas fallback content; otherwise, false</returns>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/canvas.html#being-used-as-relevant-canvas-fallback-content
    /// An element whose nearest canvas element ancestor is being rendered and represents 
    /// embedded content is an element that is being used as relevant canvas fallback content.
    /// This affects focusability - such elements can still be focused even though the canvas shows graphics.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is_Being_Used_As_Canvas_Fallback_Content(Element? element)
    {
        if (element == null)
            return false;

        // Find the nearest canvas element ancestor
        Node? current = element.parentNode;
        while (current != null)
        {
            if (current is HTML.HTMLCanvasElement canvasElement)
            {
                // Check if the canvas is being rendered and represents embedded content
                // A canvas represents embedded content when it has a rendering context bound to it
                // TODO: Implement proper check for canvas rendering context
                // For now, return true if we found an ancestor canvas element
                return true;
            }
            current = current.parentNode;
        }

        return false;
    }
#endif
    #endregion

    #region Dialog
#if ENABLE_HTML
    /// <summary>
    /// Runs the dialog focusing steps for a dialog element.
    /// This focuses the appropriate element within the dialog when it is shown.
    /// </summary>
    /// <param name="subject">The dialog element</param>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#dialog-focusing-steps
    /// The dialog focusing steps are:
    /// 1. If allow focus steps return false, return
    /// 2. If subject has autofocus, set control to subject
    /// 3. Otherwise, set control to the focus delegate of subject
    /// 4. If control is null, set control to subject
    /// 5. Run the focusing steps for control
    /// 6. Clear autofocus candidates and set autofocus processed flag
    /// </remarks>
    internal static void Run_Dialog_Focusing_Steps(Element subject)
    {
        if (subject == null)
            return;

        // 1. If allow focus steps return false, return
        // TODO: Implement allow focus steps check

        Element? control = null;

        // 2. If subject has autofocus attribute, set control to subject
        if (subject.hasAttribute("autofocus"))
        {
            control = subject;
        }

        // 3. If control is null, set control to the focus delegate of subject
        if (control == null)
        {
            // TODO: Implement focus delegate lookup
            // The focus delegate is the first focusable area in tree order
            // that has the autofocus attribute set, or is otherwise focusable
            control = Find_Focus_Delegate(subject);
        }

        // 4. If control is null, set control to subject
        if (control == null)
        {
            control = subject;
        }

        // 5. Run the focusing steps for control
        // TODO: Call the proper focusing steps
        // Run_Focusing_Steps(control);

        // 6-10. Clear autofocus candidates and set processed flag
        // TODO: Implement autofocus candidates clearing
    }

    /// <summary>
    /// Finds the focus delegate for an element.
    /// </summary>
    /// <param name="element">The element to find the focus delegate for</param>
    /// <returns>The focus delegate element, or null if none found</returns>
    private static Element? Find_Focus_Delegate(Element element)
    {
        // The focus delegate is the first element in tree order that:
        // 1. Has the autofocus attribute, OR
        // 2. Is a focusable area (like a form control)
        
        // First, look for an element with autofocus
        var tree = new TreeWalker(element, ENodeFilterMask.SHOW_ELEMENT);
        var current = tree.nextNode();
        while (current != null)
        {
            if (current is Element el)
            {
                if (el.hasAttribute("autofocus"))
                {
                    return el;
                }
            }
            current = tree.nextNode();
        }

        // Then, look for the first focusable element
        tree = new TreeWalker(element, ENodeFilterMask.SHOW_ELEMENT);
        current = tree.nextNode();
        while (current != null)
        {
            if (current is Element el)
            {
                // Check if element is focusable (simplified check)
                if (Is_Focusable_Element(el))
                {
                    return el;
                }
            }
            current = tree.nextNode();
        }

        return null;
    }

    /// <summary>
    /// Simplified check if an element is focusable.
    /// </summary>
    private static bool Is_Focusable_Element(Element element)
    {
        // Simplified focusability check
        // A proper implementation would check many more conditions
        
        if (element is HTML.HTMLInputElement inputEl)
        {
            return inputEl.type != HTML.EInputType.Hidden && !inputEl.disabled;
        }
        if (element is HTML.HTMLButtonElement buttonEl)
        {
            return !buttonEl.disabled;
        }
        if (element is HTML.HTMLSelectElement selectEl)
        {
            return !selectEl.disabled;
        }
        if (element is HTML.HTMLTextAreaElement textareaEl)
        {
            return !textareaEl.disabled;
        }
        if (element is HTML.HTMLAElement anchorEl)
        {
            return anchorEl.hasAttribute("href");
        }
        if (element.hasAttribute("tabindex"))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the document to be blocked by the modal dialog.
    /// When blocked, the document's focused area becomes inert except for shadow-including descendants of the dialog.
    /// </summary>
    /// <param name="document">The document to block</param>
    /// <param name="dialog">The modal dialog blocking the document</param>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/interaction.html#blocked-by-a-modal-dialog
    /// A Document is blocked by a modal dialog subject when the subject element is in the 
    /// document's top layer. This causes elements outside the dialog to become inert.
    /// </remarks>
    internal static void Modal_Dialog_Block_Document(Document document, Element dialog)
    {
        if (document == null || dialog == null)
            return;

        // Mark the document as blocked by the modal dialog
        // This makes all elements outside the dialog inert
        // TODO: Implement proper blocking logic
        // - Add dialog to document's top layer
        // - Mark non-dialog elements as inert
        // - Update accessibility tree
    }

    /// <summary>
    /// Removes the modal dialog blocking from the document.
    /// </summary>
    /// <param name="document">The document to unblock</param>
    /// <param name="dialog">The modal dialog that was blocking the document</param>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#the-dialog-element
    /// When a modal dialog is closed, the document is unblocked and elements are no longer inert.
    /// </remarks>
    internal static void Modal_Dialog_Unblock_Document(Document document, Element dialog)
    {
        if (document == null || dialog == null)
            return;

        // Remove the blocking state from the document
        // TODO: Implement proper unblocking logic
        // - Remove dialog from document's top layer
        // - Restore interactivity to previously inert elements
        // - Update accessibility tree
    }
#endif
    #endregion
}

