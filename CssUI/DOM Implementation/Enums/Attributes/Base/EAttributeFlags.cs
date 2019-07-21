using System;

namespace CssUI.DOM.Enums
{
    /// <summary>
    /// For future use
    /// </summary>
    [Flags]
    public enum EAttributeFlags : int
    {
        None = 0x0,

        /// <summary>
        /// This attribute can inherit its value from an ancestor element
        /// </summary>
        Inherited = (1 << 1),
    }
}
