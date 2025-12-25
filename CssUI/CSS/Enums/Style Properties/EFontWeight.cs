using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EFontWeight : int
{/* DOcs: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/#font-weight-prop */
    /// <summary>
    /// Specifies a lighter weight than the inherited value.
    /// </summary>
    [EnumData("lighter")]
    Lighter,

    /// <summary>
    /// Same as '400'
    /// </summary>
    [EnumData("normal")]
    Normal,

    /// <summary>
    /// Same as '700'
    /// </summary>
    [EnumData("bold")]
    Bold,

    /// <summary>
    /// Specifies a bolder weight than the inherited value.
    /// </summary>
    [EnumData("bolder")]
    Bolder,
}

