using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines all of the CSS length unit types
    /// </summary>
    [CssEnum]
    public enum EUnit : byte
    {
        /// <summary>
        /// Specified no unit length
        /// </summary>
        [CssKeyword("")]
        None = 0,
        /* Physical Units */
        /// <summary>
        /// 1/96th of 1in
        /// </summary>
        [CssKeyword("px")]
        PX,
        /// <summary>
        /// Points (1pt = 1/72th of 1in)
        /// </summary>
        [CssKeyword("pt")]
        PT,
        /// <summary>
        /// Picas (1pc = 1/6th of 1in)
        /// </summary>
        [CssKeyword("pc")]
        PC,
        /// <summary>
        /// Inches (1in = 2.54cm = 96px)
        /// </summary>
        [CssKeyword("in")]
        IN,
        /// <summary>
        /// Quarter-Millimeters (1Q = 12/40th of 1cm)
        /// </summary>
        [CssKeyword("q")]
        Q,
        /// <summary>
        /// Millimeters (1mm = 1/10th of 1cm)
        /// </summary>
        [CssKeyword("mm")]
        MM,
        /// <summary>
        /// Centimeters (1cm = 96px / 2.54)
        /// </summary>
        [CssKeyword("cm")]
        CM,

        /* <Resolution> Units */
        /// <summary>
        /// Dots per inch
        /// </summary>
        [CssKeyword("dpi")]
        DPI,
        /// <summary>
        /// Dots per centimeter
        /// </summary>
        [CssKeyword("dpcm")]
        DPCM,
        /// <summary>
        /// Dots per 'px' unit
        /// </summary>
        [CssKeyword("dppx")]
        DPPX,

        /* Time Units*/

        /// <summary>
        /// Seconds
        /// </summary>
        [CssKeyword("s")]
        S,
        /// <summary>
        /// Milliseconds
        /// </summary>
        [CssKeyword("ms")]
        MS,

        /* Frequency Units*/

        /// <summary>
        /// Hertz
        /// </summary>
        [CssKeyword("hz")]
        HZ,
        /// <summary>
        /// KiloHertz
        /// </summary>
        [CssKeyword("khz")]
        KHZ,

        /* Font Units */
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
        TURN,


    }
}
