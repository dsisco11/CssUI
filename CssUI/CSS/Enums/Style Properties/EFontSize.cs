using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EFontSize : int
{/* Docs: https://www.w3.org/TR/css-fonts-3/#font-size-prop */

    /* ABSOLUTE SIZES */
    /// <summary>
    ///
    /// </summary>
    [EnumData("xx-small")]
    XXSmall = 0,

    /// <summary>
    ///
    /// </summary>
    [EnumData("x-small")]
    XSmall,

    /// <summary>
    ///
    /// </summary>
    [EnumData("small")]
    Small,

    /// <summary>
    ///
    /// </summary>
    [EnumData("medium")]
    Medium,

    /// <summary>
    ///
    /// </summary>
    [EnumData("large")]
    Large,

    /// <summary>
    ///
    /// </summary>
    [EnumData("x-large")]
    XLarge,

    /// <summary>
    ///
    /// </summary>
    [EnumData("xx-large")]
    XXLarge,


    /* RELATIVE SIZES */


    /// <summary>
    ///
    /// </summary>
    [EnumData("smaller")]
    Smaller,

    /// <summary>
    ///
    /// </summary>
    [EnumData("larger")]
    Larger,

}

