using CssUI.Internal;
using System;

namespace CssUI
{
    /// <summary>
    /// Defines all the possible border styles
    /// </summary>
    [Flags]
    public enum EBorderStyle : int
    {
        /// <summary>No border, Color and width are ignored.</summary>
        [CssKeyword("none")]
        None = (1 << 0),
        /// <summary>Same as 'None' but with different conflict resolution for border-collapsed tables</summary>
        [CssKeyword("hidden")]
        Hidden = (1 << 1),
        [CssKeyword("dotted")]
        Dotted = (1 << 2),
        [CssKeyword("dashed")]
        Dashed = (1 << 3),
        [CssKeyword("solid")]
        Solid = (1 << 4),
        [CssKeyword("double")]
        Double = (1 << 5),
        [CssKeyword("groove")]
        Groove = (1 << 6),
        [CssKeyword("ridge")]
        Ridge = (1 << 7),
        [CssKeyword("inset")]
        Inset = (1 << 8),
        [CssKeyword("outset")]
        Outset = (1 << 9),
    };
}
