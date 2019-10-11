using System;

namespace CssUI.CSS.Enums
{
    /// <summary>
    /// Defines the parts of an element that a property can affect
    /// </summary>
    [Flags]
    public enum EPropertyDirtFlags : uint
    {
        /// <summary>
        /// Property affects the visual appearence (rendering) of the element
        /// </summary>
        Visual = 1 << 0,
        /// <summary>
        /// Property will change the flow(layout) positions of child elements
        /// </summary>
        Flow = 1 << 1,
        /// <summary>
        /// Property will change how the elements text is rendered
        /// </summary>
        Text = 1 << 2,

        /* Box Flags */
        /// <summary>
        /// Property changes the size of an elements box
        /// (MASKS ALL box area flags)
        /// </summary>
        Box = 0x1F << 10,
        /// <summary>
        /// Property changes the size of an element boxes Content area
        /// </summary>
        Content_Area = 1 << 10,
        /// <summary>
        /// Property changes the size of an element boxes Padding area
        /// </summary>
        Padding_Area = 1 << 11,
        /// <summary>
        /// Property changes the size of an element boxes Border area
        /// </summary>
        Border_Area = 1 << 12,
        /// <summary>
        /// Property changes the size of an element boxes Margin area
        /// </summary>
        Margin_Area = 1 << 13,
        /// <summary>
        /// Property changes the size of an element boxes Replaced-Content area
        /// </summary>
        Replaced_Area = 1 << 14,
    }
}
