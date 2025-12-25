using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EBorderSize : int
{
    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("thin")]
    Thin,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("medium")]
    Medium,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("thick")]
    Thick,
}

