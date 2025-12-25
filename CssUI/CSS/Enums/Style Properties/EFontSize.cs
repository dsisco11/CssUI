using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EFontSize : int
{/* Docs: https://www.w3.org/TR/css-fonts-3/#font-size-prop */

    /* ABSOLUTE SIZES */
    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("xx-small")]
    XXSmall = 0,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("x-small")]
    XSmall,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("small")]
    Small,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("medium")]
    Medium,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("large")]
    Large,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("x-large")]
    XLarge,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("xx-large")]
    XXLarge,


    /* RELATIVE SIZES */


    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("smaller")]
    Smaller,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("larger")]
    Larger,

}

