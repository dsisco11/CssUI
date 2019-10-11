using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;

namespace CssUI.DOM
{
    public class NodeIterator
    {/* Docs:  */
        public static LinkedList<WeakReference<NodeIterator>> ALL = new LinkedList<WeakReference<NodeIterator>>();

        #region Properties
        public readonly Node root = null;
        public readonly ENodeFilterMask whatToShow = 0x0;
        public readonly NodeFilter Filter = null;

        // XXX: Still dont know where this collection comes from
        private IList<Node> iterCollection;// = new ICollection<Node>();
        private Node referenceNode = null;
        private bool pointerBeforeReferenceNode = false;
        private bool isActive = false;
        #endregion

        #region Constructor
        public NodeIterator(Node root, ENodeFilterMask whatToShow)
        {
            this.root = root;
            this.referenceNode = root;
            this.whatToShow = whatToShow;
            this.iterCollection = Array.Empty<Node>();
        }

        public NodeIterator(Node root, IList<Node> Collection, ENodeFilterMask whatToShow, NodeFilter Filter = null)
        {
            this.root = root;
            this.referenceNode = root;
            this.whatToShow = whatToShow;
            this.iterCollection = Collection;
            this.Filter = Filter;
        }

        ~NodeIterator()
        {
            /* Remove us from the list */
            foreach (WeakReference<NodeIterator> weakRef in ALL)
            {
                if (weakRef.TryGetTarget(out NodeIterator target))
                {
                    if (ReferenceEquals(this, target))
                    {
                        ALL.Remove(weakRef);
                        break;
                    }
                }
            }
        }
        #endregion

        #region Internal Utility
        internal static void pre_removing_steps(NodeIterator nodeIterator, Node toBeRemovedNode)
        {/* Docs: https://dom.spec.whatwg.org/#nodeiterator-pre-removing-steps */
            /* 1) If toBeRemovedNode is not an inclusive ancestor of nodeIterator’s reference, or toBeRemovedNode is nodeIterator’s root, then return. */
            if (!DOMCommon.Is_Inclusive_Ancestor(toBeRemovedNode, nodeIterator.referenceNode) || ReferenceEquals(toBeRemovedNode, nodeIterator.root))
                return;
            /* 2) If nodeIterator’s pointer before reference is true, then: */
            if (nodeIterator.pointerBeforeReferenceNode)
            {
                /* 1) Let next be toBeRemovedNode’s first following node that is an inclusive descendant of nodeIterator’s root and is not an inclusive descendant of toBeRemovedNode, and null if there is no such node. */
                Node next = null;
                var tree = new TreeWalker(toBeRemovedNode, ENodeFilterMask.SHOW_ALL);
                Node n = tree.nextSibling();
                while(!ReferenceEquals(n, null))
                {
                    if (DOMCommon.Is_Inclusive_Descendant(n, nodeIterator.root) && !DOMCommon.Is_Inclusive_Descendant(n, toBeRemovedNode))
                    {
                        next = n;
                        break;
                    }
                    n = tree.nextSibling();
                }
                /* 2) If next is non-null, then set nodeIterator’s reference to next and return. */
                if (!ReferenceEquals(next, null))
                {
                    nodeIterator.referenceNode = next;
                    return;
                }
                /* 3) Otherwise, set nodeIterator’s pointer before reference to false. */
                nodeIterator.pointerBeforeReferenceNode = false;
            }
            /* 3) Set nodeIterator’s reference to toBeRemovedNode’s parent, if toBeRemovedNode’s previous sibling is null, and to the inclusive descendant of toBeRemovedNode’s previous sibling that appears last in tree order otherwise. */
            if (toBeRemovedNode.previousSibling == null)
            {
                nodeIterator.referenceNode = toBeRemovedNode.parentNode;
            }
            else
            {
                /* 
                 * "the inclusive descendant of toBeRemovedNode’s previous sibling that appears last in tree order"
                 *  That would just be the previous sibling itsself yea? or is it the most-descended last child? wtf
                 */
                var tree = new TreeWalker(toBeRemovedNode.previousSibling, ENodeFilterMask.SHOW_ALL);
                Node n = tree.lastChild();
                Node newNode = toBeRemovedNode.previousSibling;
                /* Find the most-descended last child */
                while (n != null)
                {
                    /* We keep going deeper until the traversal stops going deeper */
                    if (!DOMCommon.Is_Descendant(n, newNode))
                        break;

                    newNode = n;
                    n = tree.lastChild();
                }

                nodeIterator.referenceNode = newNode;
            }
        }
        #endregion

        private ENodeFilterResult filterNode(Node node)
        {
            /* To filter a node node within a NodeIterator or TreeWalker object traverser, run these steps: */
            /* 1) If traverser’s active flag is set, then throw an "InvalidStateError" DOMException. */
            if (isActive) throw new InvalidStateError();
            /* 2) Let n be node’s nodeType attribute value − 1. */
            int n = (int)node.nodeType;
            /* 3) If the nth bit (where 0 is the least significant bit) of traverser’s whatToShow is not set, then return FILTER_SKIP. */
            ulong mask = (1UL << n);
            if (0 == ((ulong)this.whatToShow & mask))
                return ENodeFilterResult.FILTER_SKIP;
            /* If traverser’s filter is null, then return FILTER_ACCEPT. */
            if (this.Filter == null)
                return ENodeFilterResult.FILTER_ACCEPT;

            /* Set traverser’s active flag. */
            isActive = true;
            /* Let result be the return value of call a user object’s operation with traverser’s filter, "acceptNode", and « node ». If this throws an exception, then unset traverser’s active flag and rethrow the exception. */
            ENodeFilterResult result;
            try
            {
                result = this.Filter.acceptNode(node);
            }
            catch(DOMException)
            {
                /* Unset traverser’s active flag. */
                isActive = false;
                throw;
            }
            /* Unset traverser’s active flag. */
            isActive = false;
            return result;
        }

        public Node nextNode()
        {/* Docs: https://dom.spec.whatwg.org/#concept-traversal-active */
            Node node = referenceNode;
            bool beforeNode = pointerBeforeReferenceNode;

            while (true)
            {
                /* 1) If beforeNode is false, then set node to the first node following node in iterator’s iterator collection. If there is no such node, then return null. */
                if (!beforeNode)
                {
                    int i = iterCollection.IndexOf(node);
                    if (i < (iterCollection.Count - 1))
                        return null;

                    node = iterCollection[i + 1];
                }
                else
                    beforeNode = false;

                /* 2) Let result be the result of filtering node within iterator. */
                ENodeFilterResult result = filterNode(node);
                /* 3) If result is FILTER_ACCEPT, then break. */
                if (result == ENodeFilterResult.FILTER_ACCEPT) break;
            }

            /* 4) Set iterator’s reference to node. */
            referenceNode = node;
            /* 5) Set iterator’s pointer before reference to beforeNode. */
            pointerBeforeReferenceNode = beforeNode;
            /* 6) Return node. */
            return node;

        }
        public Node previousNode()
        {/* Docs: https://dom.spec.whatwg.org/#concept-traversal-active */

            Node node = referenceNode;
            bool beforeNode = pointerBeforeReferenceNode;

            while (true)
            {
                /* 1) If beforeNode is true, then set node to the first node preceding node in iterator’s iterator collection. If there is no such node, then return null. */
                if (beforeNode)
                {
                    int i = iterCollection.IndexOf(node);
                    if (i <= 0)
                        return null;

                    node = iterCollection[i - 1];
                }
                else
                    beforeNode = true;

                /* 2) Let result be the result of filtering node within iterator. */
                ENodeFilterResult result = filterNode(node);
                /* 3) If result is FILTER_ACCEPT, then break. */
                if (result == ENodeFilterResult.FILTER_ACCEPT) break;
            }

            /* 4) Set iterator’s reference to node. */
            referenceNode = node;
            /* 5) Set iterator’s pointer before reference to beforeNode. */
            pointerBeforeReferenceNode = beforeNode;
            /* 6) Return node. */
            return node;

        }

    }
}
