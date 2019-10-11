using System;

namespace CssUI.DOM.Nodes
{
    [Flags]
    public enum ENodeFlags : int
    {
        Clear = 0x0,
        IsConnected = 1 << 0,
        IsShadowRoot = 1 << 1,
        IsCustomElement = 1 << 2,
        IsReplaced = 1 << 3,

        /// <summary>
        /// This node needs to have it's layout-box reflowed
        /// </summary>
        NeedsReflow = 1 << 10,
        /// <summary>
        /// Some descendent of this node needs to have its layout-box reflowed
        /// </summary>
        ChildNeedsReflow = 1 << 11,

        /// <summary>
        /// This node needs to have it's layout-box updated
        /// </summary>
        NeedsBoxUpdate = 1 << 12,
        /// <summary>
        /// Some descendent of this node needs to have its layout-box updated
        /// </summary>
        ChildNeedsBoxUpdate = 1 << 13,

        /// <summary>
        /// This node needs to have it's style updated
        /// </summary>
        NeedsStyleUpdate = 1 << 15,
        /// <summary>
        /// Some descendent of this node needs to have it's style updated
        /// </summary>
        ChildNeedsStyleUpdate = 1 << 16,
        /// <summary>
        /// An immediate descendent of this node needs to have it's style updated
        /// </summary>
        DirectChildNeedsStyleUpdate = 1 << 17,

        /// <summary>
        /// Some descendent of this node currently has input focus
        /// </summary>
        DescendentHasFocus = 1 << 18,
    }
}
