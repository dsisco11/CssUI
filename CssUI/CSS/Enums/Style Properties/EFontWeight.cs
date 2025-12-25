using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EFontWeight : int
{/* DOcs: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/#font-weight-prop */
    /// <summary>
    /// Specifies a lighter weight than the inherited value.
    /// </summary>
    [EnumRecordProperties("lighter")]
    Lighter,

    /// <summary>
    /// Same as '400'
    /// </summary>
    [EnumRecordProperties("normal")]
    Normal,

    /// <summary>
    /// Same as '700'
    /// </summary>
    [EnumRecordProperties("bold")]
    Bold,

    /// <summary>
    /// Specifies a bolder weight than the inherited value.
    /// </summary>
    [EnumRecordProperties("bolder")]
    Bolder,
}

