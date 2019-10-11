using System;
using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines all the possible border styles
    /// </summary>
    [Flags, MetaEnum]
    public enum EBorderStyle : int
    {
        /// <summary>
        /// No border, Color and width are ignored.
        /// </summary>
        [MetaKeyword("none")]
        None = (1 << 0),
        /// <summary>
        /// Same as 'None' but with different conflict resolution for border-collapsed tables
        /// </summary>
        [MetaKeyword("hidden")]
        Hidden = (1 << 1),
        [MetaKeyword("dotted")]
        Dotted = (1 << 2),
        [MetaKeyword("dashed")]
        Dashed = (1 << 3),
        [MetaKeyword("solid")]
        Solid = (1 << 4),
        [MetaKeyword("double")]
        Double = (1 << 5),
        [MetaKeyword("groove")]
        Groove = (1 << 6),
        [MetaKeyword("ridge")]
        Ridge = (1 << 7),
        [MetaKeyword("inset")]
        Inset = (1 << 8),
        [MetaKeyword("outset")]
        Outset = (1 << 9),
    };
}
