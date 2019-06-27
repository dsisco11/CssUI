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
        None = (1 << 0),
        /// <summary>Same as 'None' but with different conflict resolution for border-collapsed tables</summary>
        Hidden = (1 << 1),
        Solid = (1 << 2),
        Dotted = (1 << 3),
    };
}
