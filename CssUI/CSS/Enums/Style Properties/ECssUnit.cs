using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Defines all of the CSS length unit types
    /// </summary>
    [MetaEnum]
    public enum ECssUnit : int
    {
        /// <summary>
        /// Specified no unit length
        /// </summary>
        [MetaKeyword("")]
        None = 0,
        /* Physical Units */
        /// <summary>
        /// 1/96th of 1in
        /// </summary>
        [MetaKeyword("px")]
        PX,
        /// <summary>
        /// Points (1pt = 1/72th of 1in)
        /// </summary>
        [MetaKeyword("pt")]
        PT,
        /// <summary>
        /// Picas (1pc = 1/6th of 1in)
        /// </summary>
        [MetaKeyword("pc")]
        PC,
        /// <summary>
        /// Inches (1in = 2.54cm = 96px)
        /// </summary>
        [MetaKeyword("in")]
        IN,
        /// <summary>
        /// Quarter-Millimeters (1Q = 12/40th of 1cm)
        /// </summary>
        [MetaKeyword("q")]
        Q,
        /// <summary>
        /// Millimeters (1mm = 1/10th of 1cm)
        /// </summary>
        [MetaKeyword("mm")]
        MM,
        /// <summary>
        /// Centimeters (1cm = 96px / 2.54)
        /// </summary>
        [MetaKeyword("cm")]
        CM,

        /* <Resolution> Units */
        /// <summary>
        /// Dots per inch
        /// </summary>
        [MetaKeyword("dpi")]
        DPI,
        /// <summary>
        /// Dots per centimeter
        /// </summary>
        [MetaKeyword("dpcm")]
        DPCM,
        /// <summary>
        /// Dots per 'px' unit
        /// </summary>
        [MetaKeyword("dppx")]
        DPPX,

        /* Time Units*/

        /// <summary>
        /// Seconds
        /// </summary>
        [MetaKeyword("s")]
        S,
        /// <summary>
        /// Milliseconds
        /// </summary>
        [MetaKeyword("ms")]
        MS,

        /* Frequency Units*/

        /// <summary>
        /// Hertz
        /// </summary>
        [MetaKeyword("hz")]
        HZ,
        /// <summary>
        /// KiloHertz
        /// </summary>
        [MetaKeyword("khz")]
        KHZ,

        /* Font Units */
        /// <summary>
        /// Relative to font size
        /// </summary>
        [MetaKeyword("em")]
        EM,
        /// <summary>
        /// Relative to x-height of the elements font
        /// </summary>
        [MetaKeyword("ex")]
        EX,
        /// <summary>
        /// Relative to the width of the "0" glyph in the elements font
        /// </summary>
        [MetaKeyword("ch")]
        CH,
        /// <summary>
        /// Relative to font size of the root element
        /// </summary>
        [MetaKeyword("rem")]
        REM,
        /// <summary>
        /// Relative to viewports width
        /// </summary>
        [MetaKeyword("vw")]
        VW,
        /// <summary>
        /// Relative to viewports height
        /// </summary>
        [MetaKeyword("vh")]
        VH,
        /// <summary>
        /// Relative to the minimum of the viewports height and width
        /// </summary>
        [MetaKeyword("vmin")]
        VMIN,
        /// <summary>
        /// Relative to the maximum of the viewports height and width
        /// </summary>
        [MetaKeyword("vmax")]
        VMAX,

        /// <summary>
        /// Degrees. There are 360 degrees in a full circle.
        /// </summary>
        [MetaKeyword("deg")]
        DEG,
        /// <summary>
        /// Gradians, also known as 'gons' or 'grades'. There are 400 gradians in a full circle
        /// </summary>
        [MetaKeyword("grad")]
        GRAD,
        /// <summary>
        /// Radians. There are 2PI radians in a full circle
        /// </summary>
        [MetaKeyword("rad")]
        RAD,
        /// <summary>
        /// Turns. There is 1 turn in a full circle
        /// </summary>
        [MetaKeyword("turn")]
        TURN,


    }
}
