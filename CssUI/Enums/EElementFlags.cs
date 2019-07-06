using System;

namespace CssUI
{
    /// <summary>
    /// Flags which toggle different attributes and abilitys for a UI element
    /// </summary>
    [Flags]
    public enum EElementFlags : byte
    {
        /// <summary></summary>
        None = 0x0,
        /// <summary>
        /// The element is done initializing
        /// </summary>
        Ready = (1 << 1),
        /// <summary>
        /// The element can contain other elements
        /// </summary>
        Container = (1 << 2),
        /// <summary>
        /// The element can receive input focus, only one element at a time can have input focus.
        /// </summary>
        Focusable = (1 << 3),
        /// <summary>
        /// The element can receive dragging events(as in the user dragging it's position, not drag-drop events)
        /// </summary>
        Draggable = (1 << 4),
        /// <summary>
        /// The element processes click events
        /// </summary>
        Clickable = (1 << 5),
        /// <summary>
        /// The element processes double click events
        /// </summary>
        DoubleClickable = (1 << 6),

    }
}
