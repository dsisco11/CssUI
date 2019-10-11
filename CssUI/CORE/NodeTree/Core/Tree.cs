using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CssUI.Enums;

namespace CssUI.NodeTree
{
    /// <summary>
    /// Provides functionality for retreiving information from within a node tree
    /// </summary>
    public static class Tree
    {
        const ulong FILTER_SHOW_ALL = 0xFFFFFFFF;
        #region Hierarchy Checks

        #region Descendants
        /// <summary>
        /// An object A is called a descendant of an object B, if either A is a child of B or A is a child of an object C that is a descendant of B.
        /// </summary>
        /// <returns>If <paramref name="A"/> is a descendant of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Descendant(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-descendant */
            var tree = new NodeTreeWalker(A, FILTER_SHOW_ALL);
            /* Proove it true */
            ITreeNode node = tree.ParentNode();
            while (node is object)
            {
                if (ReferenceEquals(node, B))
                    return true;
                node = tree.ParentNode();
            }

            return false;
        }

        /// <summary>
        /// An inclusive descendant is an object or one of its descendants.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (inclusive) descendant of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Inclusive_Descendant(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-inclusive-descendant */
            return ReferenceEquals(A, B) || Is_Descendant(A, B);
        }
        #endregion

        #region Ancestors
        /// <summary>
        /// An object A is called an ancestor of an object B if and only if B is a descendant of A.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ancestor(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-ancestor */
            /* Fastest method to determine this is to look at it in reverse and ask if B is a descendant of A */
            return Is_Descendant(B, A);
        }

        /// <summary>
        /// An inclusive ancestor is an object or one of its ancestors.
        /// </summary>
        /// <returns>If <paramref name="A"/> is an (inclusive) ancestor of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Inclusive_Ancestor(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-inclusive-ancestor */
            /* Fastest method to determine this is to look at it in reverse and ask if B is a descendant of A */
            return ReferenceEquals(A, B) || Is_Descendant(B, A);
        }

        #endregion

        #region Siblings
        /// <summary>
        /// An object A is called a sibling of an object B, if and only if B and A share the same non-null parent.
        /// </summary>
        /// <returns>If <paramref name="A"/> is a sibling of <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Sibling(ITreeNode A, ITreeNode B)
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
        public static bool Is_Inclusive_Sibling(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-inclusive-sibling */
            return ReferenceEquals(A, B) || Is_Sibling(A, B);
        }

        /// <summary>
        /// An object A is preceding an object B if A and B are in the same tree and A comes before B in tree order.
        /// </summary>
        /// <returns>If <paramref name="A"/> is preeceding <paramref name="B"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Preceeding(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-preceding */
            if (!ReferenceEquals(Get_Root(A), Get_Root(B)))// Different trees
                return false;

            /* Proove it true */
            ITreeNode node = A.nextSibling;
            while (node is object)
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
        public static bool Is_Following(ITreeNode A, ITreeNode B)
        {/* Docs: https://dom.spec.whatwg.org/#concept-tree-following */
            if (!ReferenceEquals(Get_Root(A), Get_Root(B)))// Different trees
                return false;

            /* Proove it true */
            ITreeNode node = A.previousSibling;
            while (node is object)
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

        #region Vertical Traversals
        /// <summary>
        /// Returns a list of all ancestors for the given node, that is; the chain of parent elements all the way up to the root element.
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Ancestors(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.ParentNode();
            while (current != null)
            {
                list.AddLast(current);
                current = tree.ParentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all ancestors for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Ancestors_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL) where NodeType : ITreeNode
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.ParentNode();
            while (current != null)
            {
                if (current is NodeType currentAsType)
                {
                    list.AddLast(currentAsType);
                }
                current = tree.ParentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth ancestor for the given node, that is; the Nth parent element along the chain of elements going all the way up to the root element.
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Nth_Ancestor(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.ParentNode();
            while (current != null)
            {
                if (--N <= 0) { return current; }
                current = tree.ParentNode();
            }

            return null;
        }

        /// <summary>
        /// Returns Nth ancestor for the given node whom matches the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Nth_Ancestor_OfType<NodeType>(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL) where NodeType : ITreeNode
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.ParentNode();
            while (current != null)
            {
                if (current is NodeType nodeAsType)
                {
                    if (--N <= 0) { return nodeAsType; }
                }

                current = tree.ParentNode();
            }

            return default;
        }

        /// <summary>
        /// Returns a list of all (inclusive) ancestors for the given node, that is; the chain of parent elements all the way up to the root element.
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Inclusive_Ancestors(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            list.AddLast(node);
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.ParentNode();
            while (current != null)
            {
                list.AddLast(current);
                current = tree.ParentNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all descendents for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Descendents(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.NextNode();
            while (current != null)
            {
                list.AddLast(current);
                current = tree.NextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all descendents for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Descendents_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL) where NodeType : ITreeNode
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.NextNode();
            while (current != null)
            {
                if (current is NodeType currentAsType)
                {
                    list.AddLast(currentAsType);
                }
                current = tree.NextNode();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth descendant for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Nth_Descendant(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.NextNode();
            while (current != null)
            {
                if (--N <= 0) { return current; }
                current = tree.NextNode();
            }

            return null;
        }

        /// <summary>
        /// Returns Nth descendant for the given node whom matches the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Nth_Descendant_OfType<NodeType>(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL) where NodeType : ITreeNode
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.NextNode();
            while (current != null)
            {
                if (current is NodeType nodeAsType)
                {
                    if (--N <= 0) { return nodeAsType; }
                }

                current = tree.NextNode();
            }

            return default;
        }

        /// <summary>
        /// Returns a list of all (inclusive) descendents for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Inclusive_Descendents(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            list.AddLast(node);
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode descendant = tree.NextNode();
            while (descendant != null)
            {
                list.AddLast(descendant);
                descendant = tree.NextNode();
            }

            return list;
        }
        #endregion

        #region Horizontal Traversals
        /// <summary>
        /// Returns a list of all previous and adjacent sibling nodes for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Previous(ITreeNode node, in NodeTreeFilter Filter = null)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();

            ITreeNode sibling = node.previousSibling;
            while (sibling != null)
            {
                var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                if (fres == EFilterResult.FILTER_REJECT)
                    break;

                if (fres == EFilterResult.FILTER_ACCEPT)
                    list.AddLast(sibling);

                sibling = sibling.previousSibling;
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
        public static ITreeNode Get_Nth_Previous(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            ITreeNode sibling = node.previousSibling;
            while (sibling != null)
            {
                var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                if (fres == EFilterResult.FILTER_REJECT)
                    break;

                if (fres == EFilterResult.FILTER_ACCEPT)
                {
                    if (--N <= 0)
                    {
                        return sibling;
                    }
                }

                sibling = sibling.previousSibling;
            }

            return null;
        }


        /// <summary>
        /// Returns a list of all tree-order preceeding (sibling) nodes for the given node 
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Preceeding(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);

            ITreeNode sibling = tree.PreviousSibling();
            while (sibling != null)
            {
                var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                if (fres == EFilterResult.FILTER_REJECT)
                    break;

                if (fres == EFilterResult.FILTER_ACCEPT)
                    list.AddLast(sibling);

                sibling = tree.PreviousSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all tree-order preceeding (sibling) nodes for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Preceeding_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);

            ITreeNode sibling = tree.PreviousSibling();
            while (sibling != null)
            {
                if (sibling is NodeType nodeAsType)
                {
                    var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                    if (fres == EFilterResult.FILTER_REJECT)
                        break;

                    if (fres == EFilterResult.FILTER_ACCEPT)
                        list.AddLast(nodeAsType);
                }

                sibling = tree.PreviousSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth tree-order preceeding sibling for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Nth_Preceeding(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.PreviousSibling();
            while (current != null)
            {
                if (--N <= 0) { return current; }
                current = tree.PreviousSibling();
            }

            return null;
        }


        /// <summary>
        /// Returns a list of all tree-order following (sibling) nodes for the given node 
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Following(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);

            ITreeNode sibling = tree.NextSibling();
            while (sibling != null)
            {
                var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                if (fres == EFilterResult.FILTER_REJECT)
                    break;

                if (fres == EFilterResult.FILTER_ACCEPT)
                    list.AddLast(sibling);

                sibling = tree.NextSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns a list of all tree-order following (sibling) nodes for the given node whom match the given <typeparamref name="NodeType"/>
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<NodeType> Get_Following_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);

            ITreeNode sibling = tree.NextSibling();
            while (sibling != null)
            {
                if (sibling is NodeType nodeAsType)
                {
                    var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                    if (fres == EFilterResult.FILTER_REJECT)
                        break;

                    if (fres == EFilterResult.FILTER_ACCEPT)
                        list.AddLast(nodeAsType);
                }

                sibling = tree.NextSibling();
            }

            return list;
        }

        /// <summary>
        /// Returns Nth tree-order following sibling for the given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Nth">The number of elements to traverse</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// <param name="FilterMask">Mask for which IWalkableTreeNode types to allow</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Nth_Following(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null, ulong FilterMask = FILTER_SHOW_ALL)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            NodeTreeWalker tree = new NodeTreeWalker(node, FilterMask, Filter);
            ITreeNode current = tree.NextSibling();
            while (current != null)
            {
                if (--N <= 0) { return current; }
                current = tree.NextSibling();
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
        public static LinkedList<ITreeNode> Get_Next(ITreeNode node, in NodeTreeFilter Filter = null)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            ITreeNode sibling = node.nextSibling;
            while (sibling != null)
            {
                var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                if (fres == EFilterResult.FILTER_REJECT)
                    break;

                if (fres == EFilterResult.FILTER_ACCEPT)
                    list.AddLast(sibling);

                sibling = sibling.nextSibling;
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
        public static ITreeNode Get_Nth_Next(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            var N = Nth;
            ITreeNode sibling = node.nextSibling;
            while (sibling != null)
            {
                var fres = Filter?.acceptNode(sibling) ?? EFilterResult.FILTER_ACCEPT;
                if (fres == EFilterResult.FILTER_REJECT)
                    break;

                if (fres == EFilterResult.FILTER_ACCEPT)
                {
                    if (--N <= 0)
                    {
                        return sibling;
                    }
                }

                sibling = sibling.nextSibling;
            }

            return null;
        }
        #endregion

        #region Relationship Traversals
        /// <summary>
        /// Returns the node where a given <paramref name="node"/> node meets the given <paramref name="ancestor"/>, if any.
        /// </summary>
        /// <param name="node">Node to begin searching from</param>
        /// <param name="ancestor">Target stopping point for the search</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Junction(ITreeNode node, ITreeNode ancestor)
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

        #region CHILD RELATED

        /// <summary>
        /// Returns a list of all descendents of <paramref name="node"/> whose parent node is <paramref name="node"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LinkedList<ITreeNode> Get_Children(ITreeNode node, in NodeTreeFilter Filter = null)
        {
            LinkedList<ITreeNode> list = new LinkedList<ITreeNode>();
            ITreeNode child = node.firstChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    var fr = Filter.acceptNode(child);
                    if (fr == EFilterResult.FILTER_REJECT) break;// abort and return
                    else if (fr == EFilterResult.FILTER_ACCEPT)
                    {
                        list.AddLast(child);
                    }
                    child = child.nextSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    list.AddLast(child);
                    child = child.nextSibling;
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
        public static ITreeNode Get_Nth_Child(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null)
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            ITreeNode child = node.firstChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    var fr = Filter.acceptNode(child);
                    if (fr == EFilterResult.FILTER_REJECT) break;// abort and return
                    else if (fr == EFilterResult.FILTER_ACCEPT)
                    {
                        if (--N <= 0)
                        {
                            return child;
                        }
                    }
                    child = child.nextSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    if (--N <= 0)
                    {
                        return child;
                    }
                    child = child.nextSibling;
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
        public static LinkedList<NodeType> Get_Children_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null) where NodeType : ITreeNode
        {
            LinkedList<NodeType> list = new LinkedList<NodeType>();
            ITreeNode child = node.firstChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    if (child is NodeType childAsType)
                    {
                        var fr = Filter.acceptNode(child);
                        if (fr == EFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == EFilterResult.FILTER_ACCEPT)
                        {
                            list.AddLast(childAsType);
                        }
                    }
                    child = child.nextSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    if (child is NodeType childAsType)
                    {
                        list.AddLast(childAsType);
                        child = child.nextSibling;
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
        public static NodeType Get_Nth_Child_OfType<NodeType>(ITreeNode node, uint Nth, in NodeTreeFilter Filter = null) where NodeType : ITreeNode
        {
            if (Nth == 0)
            {
                throw new IndexOutOfRangeException("N must be greater than 0");
            }

            uint N = Nth;
            ITreeNode child = node.firstChild;
            if (Filter is object)
            {
                while (child is object)
                {
                    if (child is NodeType childAsType)
                    {
                        var fr = Filter.acceptNode(child);
                        if (fr == EFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == EFilterResult.FILTER_ACCEPT)
                        {
                            if (--N <= 0)
                            {
                                return childAsType;
                            }
                        }
                    }
                    child = child.nextSibling;
                }
            }
            else
            {
                while (child is object)
                {
                    if (child is NodeType childAsType)
                    {
                        if (--N <= 0)
                        {
                            return childAsType;
                        }
                    }
                    child = child.nextSibling;
                }
            }

            return default;
        }


        /// <summary>
        /// Returns the first immediate descendent which matches the given <paramref name="Filter"/> and Type <typeparamref name="NodeType"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_First_Child_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null) where NodeType : ITreeNode
        {
            return Get_Nth_Child_OfType<NodeType>(node, 1, Filter);
        }

        /// <summary>
        /// Returns the first descendent of <paramref name="node"/> whose parent node is <paramref name="node"/> and which matches the given <paramref name="Filter"/> and <typeparamref name="NodeType"/>
        /// <param name="node">The node to start searching from</param>
        /// <param name="Filter">Filter used for determining which nodes to allow</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NodeType Get_Last_Child_OfType<NodeType>(ITreeNode node, in NodeTreeFilter Filter = null) where NodeType : ITreeNode
        {
            ITreeNode child = node.lastChild;
            while (child is object)
            {
                if (child is NodeType childAsType)
                {
                    if (Filter is object)
                    {
                        var fr = Filter.acceptNode(child);
                        if (fr == EFilterResult.FILTER_REJECT) break;// abort and return
                        else if (fr == EFilterResult.FILTER_ACCEPT)
                        {
                            return childAsType;
                        }
                    }
                    else
                    {
                        return childAsType;
                    }
                }

                child = child.previousSibling;
            }

            return default;
        }
        #endregion
        #endregion

        #region PROPERTY IMPLEMENTATION

        /// <summary>
        /// Returns the root of a given node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Root(ITreeNode node)
        {
            /* The root of an object is itself, if its parent is null, or else it is the root of its parent. The root of a tree is any object participating in that tree whose parent is null. */
            ITreeNode current = node;
            while (current is object)
            {
                if (current.parentNode is null)
                    return current;

                current = current.parentNode;
            }

            return null;
        }

        /// <summary>
        /// Returns the sibling that comes before the given node within it's parent node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Previous_Sibling(ITreeNode node)
        {
            if (node is null || node.parentNode is null)
            {
                return null;
            }

            if (node.index > 0)
            {
                return ((ITreeNode)node.parentNode).childNodes[node.index - 1];
            }

            return null;
        }

        /// <summary>
        /// Returns the sibling that comes after the given node within it's parent node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITreeNode Get_Next_Sibling(ITreeNode node)
        {
            if (node is null || node.parentNode is null)
            {
                return null;
            }

            if (node.index < ((ITreeNode)node.parentNode).childNodes.Count)
            {
                return ((ITreeNode)node.parentNode).childNodes[node.index + 1];
            }

            return null;
        }

        /// <summary>
        /// Returns the index of the given node within it's parent node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Get_Node_Index(ITreeNode node)
        {
            if (node is null || node.parentNode is null)
            {
                return 0;
            }

            return ((ITreeNode)node.parentNode).childNodes.IndexOf(node);
        }

        /// <summary>
        /// Returns the index of the given node within it's parent node
        /// </summary>
        /// <param name="node">The node to start searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ETreePosition ComparePosition(ITreeNode node1, ITreeNode node2)
        {
            if (ReferenceEquals(node1, node2))
            {
                return 0x0;
            }

            /* 6) If node1 or node2 is null, or node1’s root is not node2’s root, then return DISCONNECTED and PRECEDING. */
            if (node1 is null || node2 is null || !ReferenceEquals(node1.GetRootNode(), node2.GetRootNode()))
            {
                return ETreePosition.DISCONNECTED | ETreePosition.PRECEDING;
            }

            /* 7) If node1 is an ancestor of node2, then return CONTAINS and PRECEDING. */
            if (Is_Ancestor(node1, node2))
            {
                return ETreePosition.CONTAINS | ETreePosition.PRECEDING;
            }

            /* 8) If node1 is a descendant of node2, then return CONTAINED_BY and FOLLOWING. */
            if (Is_Descendant(node1, node2))
            {
                return ETreePosition.CONTAINED_BY | ETreePosition.FOLLOWING;
            }

            /* 9) If node1 is preceding node2, then return PRECEDING. */
            if (Is_Preceeding(node1, node2))
                return ETreePosition.PRECEDING;

            /* 10) Return FOLLOWING. */
            return ETreePosition.FOLLOWING;
        }
        #endregion

    }
}
