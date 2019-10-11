using CssUI.Enums;
using System.Runtime.CompilerServices;

namespace CssUI.NodeTree
{
    public class TreeNode : ITreeNode
    {
        #region Properties

        /// <summary> Index of this node within its parent's list of children </summary>
        public int index { get; set; }
        /// <summary> An integer representing an arbitrary 'type' for this node, used for filtering nodes when traversing the tree. </summary>
        public int nodeType { get; set; } = 0x0;


        /// <summary></summary>
        public bool hasChildNodes => (childNodes.Count > 0);
        /// <summary> List of all nodes which are direct descendents of this one </summary>
        public TreeNodeList childNodes { get; private set; }


        /// <summary> The node immediately containing this one </summary>
        public ITreeNode parentNode { get; set; }
        /// <summary> The first child in this nodes list of children </summary>
        public ITreeNode firstChild => (childNodes.Count <= 0 ? null : childNodes[0]);
        /// <summary> The last child in this nodes list of children </summary>
        public ITreeNode lastChild => (childNodes.Count <= 0 ? null : childNodes[childNodes.Count - 1]);
        /// <summary> The node which is directly adjacent to this one within the parent </summary>
        public ITreeNode nextSibling => Tree.Get_Next_Sibling(this);
        /// <summary> The node which is directly adjacent to this one within the parent </summary>
        public ITreeNode previousSibling => Tree.Get_Previous_Sibling(this);
        #endregion

        #region Constructors
        public TreeNode(ITreeNode parentNode)
        {
            this.parentNode = parentNode;
            childNodes = new TreeNodeList(parentNode);
        }
        #endregion


        /// <summary>
        /// Returns the relative position of this node compared to another
        /// </summary>
        /// <param name="other">Node to compare against</param>
        /// <returns>Relative position compared to other node</returns>
        public ETreePosition ComparePosition(ITreeNode other) => Tree.ComparePosition(this, other);

        /// <summary> Returns true if <paramref name="other"/> is an inclusive descendant of this node, and false otherwise. </summary>
        public bool Contains(ITreeNode other) => Tree.Is_Inclusive_Descendant(this, other);

        /// <summary> Returns the nodes root </summary>
        public ITreeNode GetRootNode() => Tree.Get_Root(this);


        #region List Implementation
        public TreeNode this[int index] { get => (TreeNode)childNodes[index]; }

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ITreeNode item) => childNodes.Add(item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, ITreeNode item) => childNodes.Insert(index, item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => childNodes.Clear();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(ITreeNode item) => childNodes.IndexOf(item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(ITreeNode item) => childNodes.Remove(item);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index) => childNodes.RemoveAt(index);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Replace(ITreeNode node, ITreeNode child) => childNodes.Replace(node, child);
        #endregion
    }
}
