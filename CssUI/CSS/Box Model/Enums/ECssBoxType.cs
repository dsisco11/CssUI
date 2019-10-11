using System;

namespace CssUI.CSS.Enums
{
    /// <summary>
    /// Specifies an area-type for a CSS box
    /// </summary>
    [Flags]
    public enum ECssBoxType : int
    {
        Replaced = 1 << 1,
        Content = 1 << 2,
        Padding = 1 << 3,
        Border = 1 << 4,
        Margin = 1 << 5,
    }
}
