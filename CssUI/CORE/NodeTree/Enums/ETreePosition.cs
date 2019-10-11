using System;

namespace CssUI.Enums
{
    /// <summary>
    /// Indicates the relative position between two TreeNodes
    /// </summary>
    [Flags]
    public enum ETreePosition : ushort
    {
        /// <summary>
        /// Set when node and other are not in the same tree.
        /// </summary>
        DISCONNECTED = (0x01 << 0),
        /// <summary>
        /// Set when other is preceding node.
        /// </summary>
        PRECEDING = (0x01 << 1),
        /// <summary>
        /// Set when other is following node.
        /// </summary>
        FOLLOWING = (0x01 << 2),
        /// <summary>
        /// Set when other is an ancestor of node.
        /// </summary>
        CONTAINS = (0x01 << 3),
        /// <summary>
        /// Set when other is a descendant of node.
        /// </summary>
        CONTAINED_BY = (0x01 << 4),
    }
}
