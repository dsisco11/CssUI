using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EDirection : int
{
    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("ltr")]
    LTR = 1,

    /// <summary>
    ///
    /// </summary>
    [EnumRecordProperties("rtl")]
    RTL,
}

