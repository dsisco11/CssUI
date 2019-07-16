using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Specifies an area-type for a <see cref="CssLayoutBox"/>
    /// </summary>
    [Flags]
    public enum ECssBoxArea : int
    {
        Replaced = 1 << 1,
        Content = 1 << 2,
        Padding = 1 << 3,
        Border = 1 << 4,
        Margin = 1 << 5,
    }
}
