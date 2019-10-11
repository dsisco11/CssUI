using CssUI.Enums;

namespace CssUI.NodeTree
{
    public interface ITreeNode
    {
        #region Basic
        /// <summary> An integer representing an arbitrary 'type' for this node, used for filtering nodes when traversing the tree. </summary>
        int nodeType { get; set; }

        /// <summary></summary>
        bool hasChildNodes { get; }

        /// <summary> The node immediately containing this one </summary>
        ITreeNode parentNode { get; set; }
        /// <summary> The first child in this nodes list of children </summary>
        ITreeNode firstChild { get; }
        /// <summary> The last child in this nodes list of children </summary>
        ITreeNode lastChild { get; }
        /// <summary> The node which is directly adjacent to this one within the parent </summary>
        ITreeNode nextSibling { get; }
        /// <summary> The node which is directly adjacent to this one within the parent </summary>
        ITreeNode previousSibling { get; }
        #endregion


        /// <summary> Index of this node within its parent's list of children </summary>
        int index { get; set; }
        /// <summary> List of all nodes which are direct descendents of this one </summary>
        TreeNodeList childNodes { get; }

        /// <summary>
        /// Returns the relative position of this node compared to another
        /// </summary>
        /// <param name="other">Node to compare against</param>
        /// <returns>Relative position compared to other node</returns>
        ETreePosition ComparePosition(ITreeNode other);
        /// <summary> Returns true if <paramref name="other"/> is an inclusive descendant of this node, and false otherwise. </summary>
        bool Contains(ITreeNode other);
        /// <summary> Returns the nodes root </summary>
        ITreeNode GetRootNode();
    }
}
