using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace CssUI.NodeTree
{
    /// <summary>
    /// Manages and allows indexing of a list of child-nodes, updating their previous/next siblings to the correct values
    /// </summary>
    public class TreeNodeList : ICollection<ITreeNode>, IEnumerable<ITreeNode>,  IList<ITreeNode>
    {
        #region Properties
        private List<ITreeNode> Items = new List<ITreeNode>();
        private readonly ITreeNode Owner;
        #endregion

        #region Events
        /// <summary>
        /// The list changed in size
        /// </summary>
        /// <param name="Index">Index where change occured</param>
        /// <param name="Source">Item responsible</param>
        public delegate void TreeNode_Item_Handler(int Index, ITreeNode Source);
        /// <summary>
        /// An item changed value
        /// </summary>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        public delegate void TreeNode_Value_Change_Handler(ITreeNode OldValue, ITreeNode NewValue);

        /// <summary> An item was added to the list </summary>
        public event TreeNode_Item_Handler onAdded;
        /// <summary> An item was removed from the list </summary>
        public event TreeNode_Item_Handler onRemoved;
        /// <summary> a node changed value </summary>
        public event TreeNode_Value_Change_Handler onChanged;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new list of tree nodes
        /// </summary>
        public TreeNodeList(ITreeNode owner)
        {
            Owner = owner;
        }
        #endregion


        public ITreeNode this[int index]
        {
            get => Items[index];
            set
            {
                var old = Items[index];
                Items[index] = value;
                value.index = index;
                old.parentNode = null;
                value.parentNode = Owner;
                onChanged?.Invoke(old, value);
            }
        }

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

            var node = Items[index];

            // Update the target nodes index
            node.index = index;
            // Update the index of all nodes following the target one
            for (int i=index; i<Count-1; i++)
            {
                Items[i].index = i;
            }
        }

        #region List Manipulation
        public void Add(ITreeNode item)
        {
            if (!(item.parentNode is null))
            {
                throw new ArgumentException($"Cannot adopt a node which is still parented.");
            }
            Contract.EndContractBlock();

            item.parentNode = Owner;
            Items.Add(item);
            Update_Node_Links(Items.Count - 1);
            onAdded?.Invoke(Items.Count - 1, item);
        }
        public void Insert(int index, ITreeNode item)
        {
            if (!(item.parentNode is null))
            {
                throw new ArgumentException($"Cannot adopt a node which is still parented.");
            }
            Contract.EndContractBlock();

            item.parentNode = Owner;
            Items.Insert(index, item);
            Update_Node_Links(index);
            onAdded?.Invoke(index, item);
        }

        public bool Remove(ITreeNode item)
        {
            int index = Items.IndexOf(item);
            var result = Items.Remove(item);
            item.parentNode = null;
            Update_Node_Links(index);
            onRemoved?.Invoke(index, item);
            return result;
        }

        public void RemoveAt(int index)
        {
            var item = Items[index];
            item.parentNode = null;
            Items.RemoveAt(index);
            Update_Node_Links(index);
            onRemoved?.Invoke(index, item);
        }

        public void Clear()
        {
            /* Clear the index values for all nodes */
            foreach (ITreeNode node in Items)
            {
                onRemoved?.Invoke(node.index, node);
                node.parentNode = null;
                node.index = 0;
            }
            /* Now erase the list */
            Items.Clear();
        }

        public void Replace(ITreeNode node, ITreeNode child)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (child is null)
            {
                throw new ArgumentNullException(nameof(child));
            }
            Contract.EndContractBlock();

            int index = Items.IndexOf(node);
            node.parentNode = null;
            child.parentNode = Owner;
            Items[index] = child;
            Update_Node_Links(index);
        }
        #endregion


        public bool Contains(ITreeNode item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(ITreeNode[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ITreeNode> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public int IndexOf(ITreeNode item)
        {
            return Items.IndexOf(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
