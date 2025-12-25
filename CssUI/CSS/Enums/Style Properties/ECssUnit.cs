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
    [EnumRecordProperties("")]
    None = 0,
    /* Physical Units */
    /// <summary>
    /// 1/96th of 1in
    /// </summary>
    [EnumRecordProperties("px")]
    PX,
    /// <summary>
    /// Points (1pt = 1/72th of 1in)
    /// </summary>
    [EnumRecordProperties("pt")]
    PT,
    /// <summary>
    /// Picas (1pc = 1/6th of 1in)
    /// </summary>
    [EnumRecordProperties("pc")]
    PC,
    /// <summary>
    /// Inches (1in = 2.54cm = 96px)
    /// </summary>
    [EnumRecordProperties("in")]
    IN,
    /// <summary>
    /// Quarter-Millimeters (1Q = 12/40th of 1cm)
    /// </summary>
    [EnumRecordProperties("q")]
    Q,
    /// <summary>
    /// Millimeters (1mm = 1/10th of 1cm)
    /// </summary>
    [EnumRecordProperties("mm")]
    MM,
    /// <summary>
    /// Centimeters (1cm = 96px / 2.54)
    /// </summary>
    [EnumRecordProperties("cm")]
    CM,

    /* <Resolution> Units */
    /// <summary>
    /// Dots per inch
    /// </summary>
    [EnumRecordProperties("dpi")]
    DPI,
    /// <summary>
    /// Dots per centimeter
    /// </summary>
    [EnumRecordProperties("dpcm")]
    DPCM,
    /// <summary>
    /// Dots per 'px' unit
    /// </summary>
    [EnumRecordProperties("dppx")]
    DPPX,

    /* Time Units*/

    /// <summary>
    /// Seconds
    /// </summary>
    [EnumRecordProperties("s")]
    S,
    /// <summary>
    /// Milliseconds
    /// </summary>
    [EnumRecordProperties("ms")]
    MS,

    /* Frequency Units*/

    /// <summary>
    /// Hertz
    /// </summary>
    [EnumRecordProperties("hz")]
    HZ,
    /// <summary>
    /// KiloHertz
    /// </summary>
    [EnumRecordProperties("khz")]
    KHZ,

    /* Font Units */
    /// <summary>
    /// Relative to font size
    /// </summary>
    [EnumRecordProperties("em")]
    EM,
    /// <summary>
    /// Relative to x-height of the elements font
    /// </summary>
    [EnumRecordProperties("ex")]
    EX,
    /// <summary>
    /// Relative to the width of the "0" glyph in the elements font
    /// </summary>
    [EnumRecordProperties("ch")]
    CH,
    /// <summary>
    /// Relative to font size of the root element
    /// </summary>
    [EnumRecordProperties("rem")]
    REM,
    /// <summary>
    /// Relative to viewports width
    /// </summary>
    [EnumRecordProperties("vw")]
    VW,
    /// <summary>
    /// Relative to viewports height
    /// </summary>
    [EnumRecordProperties("vh")]
    VH,
    /// <summary>
    /// Relative to the minimum of the viewports height and width
    /// </summary>
    [EnumRecordProperties("vmin")]
    VMIN,
    /// <summary>
    /// Relative to the maximum of the viewports height and width
    /// </summary>
    [EnumRecordProperties("vmax")]
    VMAX,

    /// <summary>
    /// Degrees. There are 360 degrees in a full circle.
    /// </summary>
    [EnumRecordProperties("deg")]
    DEG,
    /// <summary>
    /// Gradians, also known as 'gons' or 'grades'. There are 400 gradians in a full circle
    /// </summary>
    [EnumRecordProperties("grad")]
    GRAD,
    /// <summary>
    /// Radians. There are 2PI radians in a full circle
    /// </summary>
    [EnumRecordProperties("rad")]
    RAD,
    /// <summary>
    /// Turns. There is 1 turn in a full circle
    /// </summary>
    [EnumRecordProperties("turn")]
    TURN,

    /* Grid Units */
    /// <summary>
    /// Flexible length unit for CSS Grid (fraction of remaining space)
    /// </summary>
    [EnumRecordProperties("fr")]
    FR,

}

