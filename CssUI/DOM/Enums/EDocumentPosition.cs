using System;

namespace CssUI.DOM.Enums
{
    [Flags]
    public enum EDocumentPosition : ushort
    {
        /// <summary>
        /// Set when node and other are not in the same tree.
        /// </summary>
        DISCONNECTED = 0x01,
        /// <summary>
        /// Set when other is preceding node.
        /// </summary>
        PRECEDING = 0x02,
        /// <summary>
        /// Set when other is following node.
        /// </summary>
        FOLLOWING = 0x04,
        /// <summary>
        /// Set when other is an ancestor of node.
        /// </summary>
        CONTAINS = 0x08,
        /// <summary>
        /// Set when other is a descendant of node.
        /// </summary>
        CONTAINED_BY = 0x10,
        /// <summary>
        /// 
        /// </summary>
        IMPLEMENTATION_SPECIFIC = 0x20,
    }
}
