using System;

namespace CssUI
{
    /// <summary>
    /// Defines the parts of an element that a property can affect
    /// </summary>
    [Flags]
    public enum EPropertyAffects : uint
    {
        /// <summary>Property primarily affects the visual appearence of the element</summary>
        Visual = (1 << 0),
        /// <summary>Property primarily changes the size of an elements blocks</summary>
        Block = (1 << 1),
        /// <summary>Property will change the flow(layout) positions of child elements</summary>
        Flow = (1 << 2),
        /// <summary>Property will change how the elements text is rendered</summary>
        Text = (1 << 3),
    }
}
