using CssUI.DOM.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace CssUI.DOM
{
    /// <summary>
    /// Manages and allows indexing of a list of child-nodes, updating their previous/next siblings to the correct values
    /// </summary>
    public class ChildNodeList : IList<Node>
    {
        #region Properties
        private readonly WeakReference<Node> Owner;
        private readonly bool IsParentNode = false;
        private List<Node> Items = new List<Node>();

        // Element type children link management
        public LinkedList<Element> ChildElements;
        private int firstElementIndex = -1;
        private int lastElementIndex = -1;
        #endregion

        #region Constructor
        public ChildNodeList(in Node owner)
        {
            Owner = new WeakReference<Node>(owner);
            IsParentNode = (owner is ParentNode);
            if (IsParentNode)
            {
                ChildElements = new LinkedList<Element>();
            }
        }
        #endregion

        #region Child Element Handling

        private void _element_child_added(int index, in Element item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            Contract.EndContractBlock();

            // Figure out where to add this item in our list of child elements
            if (index <= firstElementIndex)
            {
                item.ptrSelfRef = ChildElements.AddFirst(item);
            }
            else if (index >= lastElementIndex)
            {
                item.ptrSelfRef = ChildElements.AddLast(item);
            }
            else
            {// Scan through the list and 
                var current = ChildElements.First;
                while (current is object)
                {
                    if (current.Value.index <= index)
                    {
                        break;
                    }
                    current = current.Next;
                }

                item.ptrSelfRef = ChildElements.AddBefore(current, item);
            }
        }

        private void _element_child_removed(int index, in Element item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            Contract.EndContractBlock();

            if (item.ptrSelfRef is null)
            {
                bool exists = ChildElements.Contains(item);
                Debug.Assert(exists, "The removal handler for an already removed child element was called!");
                if (exists)
                {
                    // Remove the item from the list
                    ChildElements.Remove(item.ptrSelfRef);
                }
                // Update our first/last indices
                firstElementIndex = ChildElements.First?.Value.index ?? -1;
                lastElementIndex = ChildElements.Last?.Value.index ?? firstElementIndex;
            }
            else
            {
                // Remove the item from the list
                ChildElements.Remove(item.ptrSelfRef);
                // Update our first/last indices
                firstElementIndex = ChildElements.First?.Value.index ?? -1;
                lastElementIndex = ChildElements.Last?.Value.index ?? firstElementIndex;
                // Remove the elements list node pointer
                item.ptrSelfRef = null;
            }
        }
        #endregion

        #region List Implementation
        public Node this[int index] { get => Items[index]; set => Items[index] = value; }

        public int Count => Items.Count;

        public bool IsReadOnly => false;
        /// <summary>
        /// Updates the previous and next siblings of the node at <paramref name="index"/> as well as its neighbors
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Update_Node_Links(int index)
        {
            if (index > Count) throw new IndexOutOfRangeException();
            Contract.EndContractBlock();
            if (Count <= 0) return;

            var previousNode = (Count > (index - 1)) ? Items[index - 1] : null;
            var node = Items[index];
            var nextNode = (Count > (index+1)) ? Items[index + 1] : null;

            node.index = index;

            if (previousNode is null)
            {
                node.previousSibling = null;
            }
            else
            {
                node.previousSibling = previousNode;
                previousNode.nextSibling = node;
                previousNode.index = index - 1;
            }

            if (nextNode is null)
            {
                node.nextSibling = null;
            }
            else
            {
                node.nextSibling = nextNode;
                nextNode.previousSibling = node;
                nextNode.index = index + 1;
            }

        }

        #region List Manipulation
        public void Add(Node item)
        {
            Items.Add(item);
            item.parentNode = this.Owner.TryGetTarget(out Node outParent) ? outParent : null;

            int index = Items.Count - 1;
            Update_Node_Links(index);
            // Update our linked elements list (if needed)
            if (item is Element element)
            {
                _element_child_added(index, element);
            }
        }
        public void Insert(int index, Node item)
        {
            Items.Insert(index, item);
            item.parentNode = this.Owner.TryGetTarget(out Node outParent) ? outParent : null;

            Update_Node_Links(index);
            // Update our linked elements list (if needed)
            if (item is Element element)
            {
                _element_child_added(index, element);
            }
        }

        public bool Remove(Node item)
        {
            int index = Items.IndexOf(item);
            var result = Items.Remove(item);
            if (result)
            {
                item.parentNode = null;
                item.nextSibling = item.previousSibling = null;

                Update_Node_Links(index);
                if (item is Element element)
                {
                    _element_child_removed(index, element);
                }
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            Node item = Items[index];
            Items.RemoveAt(index);
            item.parentNode = null;
            item.nextSibling = item.previousSibling = null;

            Update_Node_Links(index);
            if (item is Element element)
            {
                _element_child_removed(index, element);
            }
        }

        public void Clear()
        {
            /* Clear the sibling/index values for all nodes */
            foreach (Node item in Items)
            {
                item.parentNode = null;
                item.previousSibling = item.nextSibling = null;
                item.index = 0;
                if (item is Element element)
                {
                    element.ptrSelfRef = null;
                }
            }

            firstElementIndex = lastElementIndex = -1;
            /* Now erase the list */
            Items.Clear();
            ChildElements.Clear();
        }
        #endregion


        public bool Contains(Node item)
        {
            // A minor search improvement here is to, if the item is an Element-type, search our (likely smaller) list of elements
            if (item is Element element)
            {
                return ChildElements.Contains(element);
            }

            return Items.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int IndexOf(Node item)
        {
            return Items.IndexOf(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        #endregion
    }
}
