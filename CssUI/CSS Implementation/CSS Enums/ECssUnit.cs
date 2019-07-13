using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines all of the CSS length unit types
    /// </summary>
    [CssEnum]
    public enum ECssUnit : byte
    {
        /// <summary>
        /// Specified no unit length
        /// </summary>
        [CssKeyword("")]
        None = 0,
        /// <summary>
        /// Relative to pixel size
        /// </summary>
        [CssKeyword("px")]
        PX,
        /// <summary>
        /// Relative to font size
        /// </summary>
        [CssKeyword("em")]
        EM,
        /// <summary>
        /// Relative to x-height of the elements font
        /// </summary>
        [CssKeyword("ex")]
        EX,
        /// <summary>
        /// Relative to the width of the "0" glyph in the elements font
        /// </summary>
        [CssKeyword("ch")]
        CH,
        /// <summary>
        /// Relative to font size of the root element
        /// </summary>
        [CssKeyword("rem")]
        REM,
        /// <summary>
        /// Relative to viewports width
        /// </summary>
        [CssKeyword("vw")]
        VW,
        /// <summary>
        /// Relative to viewports height
        /// </summary>
        [CssKeyword("vh")]
        VH,
        /// <summary>
        /// Relative to the minimum of the viewports height and width
        /// </summary>
        [CssKeyword("vmin")]
        VMIN,
        /// <summary>
        /// Relative to the maximum of the viewports height and width
        /// </summary>
        [CssKeyword("vmax")]
        VMAX,

        /// <summary>
        /// Degrees. There are 360 degrees in a full circle.
        /// </summary>
        [CssKeyword("deg")]
        DEG,
        /// <summary>
        /// Gradians, also known as 'gons' or 'grades'. There are 400 gradians in a full circle
        /// </summary>
        [CssKeyword("grad")]
        GRAD,
        /// <summary>
        /// Radians. There are 2PI radians in a full circle
        /// </summary>
        [CssKeyword("rad")]
        RAD,
        /// <summary>
        /// Turns. There is 1 turn in a full circle
        /// </summary>
        [CssKeyword("turn")]
        TURN
    }
}
