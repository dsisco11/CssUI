using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Defines all of the CSS length unit types
/// </summary>
[EnumRecord<KeywordProperties>]
public enum ECssUnit : int
{
    /// <summary>
    /// Specified no unit length
    /// </summary>
    [EnumData("")]
    None = 0,
    /* Physical Units */
    /// <summary>
    /// 1/96th of 1in
    /// </summary>
    [EnumData("px")]
    PX,
    /// <summary>
    /// Points (1pt = 1/72th of 1in)
    /// </summary>
    [EnumData("pt")]
    PT,
    /// <summary>
    /// Picas (1pc = 1/6th of 1in)
    /// </summary>
    [EnumData("pc")]
    PC,
    /// <summary>
    /// Inches (1in = 2.54cm = 96px)
    /// </summary>
    [EnumData("in")]
    IN,
    /// <summary>
    /// Quarter-Millimeters (1Q = 12/40th of 1cm)
    /// </summary>
    [EnumData("q")]
    Q,
    /// <summary>
    /// Millimeters (1mm = 1/10th of 1cm)
    /// </summary>
    [EnumData("mm")]
    MM,
    /// <summary>
    /// Centimeters (1cm = 96px / 2.54)
    /// </summary>
    [EnumData("cm")]
    CM,

    /* <Resolution> Units */
    /// <summary>
    /// Dots per inch
    /// </summary>
    [EnumData("dpi")]
    DPI,
    /// <summary>
    /// Dots per centimeter
    /// </summary>
    [EnumData("dpcm")]
    DPCM,
    /// <summary>
    /// Dots per 'px' unit
    /// </summary>
    [EnumData("dppx")]
    DPPX,

    /* Time Units*/

    /// <summary>
    /// Seconds
    /// </summary>
    [EnumData("s")]
    S,
    /// <summary>
    /// Milliseconds
    /// </summary>
    [EnumData("ms")]
    MS,

    /* Frequency Units*/

    /// <summary>
    /// Hertz
    /// </summary>
    [EnumData("hz")]
    HZ,
    /// <summary>
    /// KiloHertz
    /// </summary>
    [EnumData("khz")]
    KHZ,

    /* Font Units */
    /// <summary>
    /// Relative to font size
    /// </summary>
    [EnumData("em")]
    EM,
    /// <summary>
    /// Relative to x-height of the elements font
    /// </summary>
    [EnumData("ex")]
    EX,
    /// <summary>
    /// Relative to the width of the "0" glyph in the elements font
    /// </summary>
    [EnumData("ch")]
    CH,
    /// <summary>
    /// Relative to font size of the root element
    /// </summary>
    [EnumData("rem")]
    REM,
    /// <summary>
    /// Relative to viewports width
    /// </summary>
    [EnumData("vw")]
    VW,
    /// <summary>
    /// Relative to viewports height
    /// </summary>
    [EnumData("vh")]
    VH,
    /// <summary>
    /// Relative to the minimum of the viewports height and width
    /// </summary>
    [EnumData("vmin")]
    VMIN,
    /// <summary>
    /// Relative to the maximum of the viewports height and width
    /// </summary>
    [EnumData("vmax")]
    VMAX,

    /// <summary>
    /// Degrees. There are 360 degrees in a full circle.
    /// </summary>
    [EnumData("deg")]
    DEG,
    /// <summary>
    /// Gradians, also known as 'gons' or 'grades'. There are 400 gradians in a full circle
    /// </summary>
    [EnumData("grad")]
    GRAD,
    /// <summary>
    /// Radians. There are 2PI radians in a full circle
    /// </summary>
    [EnumData("rad")]
    RAD,
    /// <summary>
    /// Turns. There is 1 turn in a full circle
    /// </summary>
    [EnumData("turn")]
    TURN,

    /* Grid Units */
    /// <summary>
    /// Flexible length unit for CSS Grid (fraction of remaining space)
    /// </summary>
    [EnumData("fr")]
    FR,

}

