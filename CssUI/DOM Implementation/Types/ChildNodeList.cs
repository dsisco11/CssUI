using CssUI.DOM.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.DOM
{
    /// <summary>
    /// Manages and allows indexing of a list of child-nodes, updating their previous/next siblings to the correct values
    /// </summary>
    public class ChildNodeList : IList<Node>
    {
        #region Properties
        private List<Node> Items = new List<Node>();
        #endregion

        #region Constructor
        public ChildNodeList()
        {
        }
        #endregion


        public Node this[int index] { get => ((IList<Node>)Items)[index]; set => ((IList<Node>)Items)[index] = value; }

        public int Count => ((IList<Node>)Items).Count;

        public bool IsReadOnly => ((IList<Node>)Items).IsReadOnly;

        /// <summary>
        /// Updates the previous and next siblings of the node at <paramref name="index"/> as well as its neighbors
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Update_Node_Links(int index)
        {
            if (index > Items.Count) throw new IndexOutOfRangeException();

            var previousNode = (Items.Count > (index-1)) ? Items[index - 1] : null;
            var node = Items[index];
            var nextNode = (Items.Count > (index+1)) ? Items[index + 1] : null;

            node.index = index;

            if (previousNode != null)
            {
                node.previousSibling = previousNode;
                previousNode.nextSibling = node;
                previousNode.index = index - 1;
            }
            else
            {
                node.previousSibling = null;
            }

            if (nextNode != null)
            {
                node.nextSibling = nextNode;
                nextNode.previousSibling = node;
                nextNode.index = index + 1;
            }
            else
            {
                node.nextSibling = null;
            }

        }

        #region List Manipulation
        public void Add(Node node)
        {
            ((IList<Node>)Items).Add(node);
            Update_Node_Links(Items.Count - 1);
        }
        public void Insert(int index, Node item)
        {
            ((IList<Node>)Items).Insert(index, item);
            Update_Node_Links(index);
        }

        public bool Remove(Node item)
        {
            int index = Items.IndexOf(item);
            var result = ((IList<Node>)Items).Remove(item);
            Update_Node_Links(index);
            return result;
        }

        public void RemoveAt(int index)
        {
            ((IList<Node>)Items).RemoveAt(index);
            Update_Node_Links(index);
        }

        public void Clear()
        {
            /* Clear the sibling/index values for all nodes */
            foreach (Node node in Items)
            {
                node.previousSibling = node.nextSibling = null;
                node.index = 0;
            }
            /* Now erase the list */
            ((IList<Node>)Items).Clear();
        }
        #endregion


        public bool Contains(Node item)
        {
            return ((IList<Node>)Items).Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            ((IList<Node>)Items).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return ((IList<Node>)Items).GetEnumerator();
        }

        public int IndexOf(Node item)
        {
            return ((IList<Node>)Items).IndexOf(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Node>)Items).GetEnumerator();
        }
    }
}
