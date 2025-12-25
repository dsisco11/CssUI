using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum EDir : int
{
    /// <summary>
    /// Indicates that the contents of the element are explicitly directionally isolated left-to-right text.
    /// </summary>
    [EnumRecordProperties("ltr")]
    Ltr = 1,

    /// <summary>
    /// Indicates that the contents of the element are explicitly directionally isolated right-to-left text.
    /// </summary>
    [EnumRecordProperties("rtl")]
    Rtl,

    /// <summary>
    /// Indicates that the contents of the element are explicitly directionally isolated text, but that the direction is to be determined programmatically using the contents of the element
    /// </summary>
    [EnumRecordProperties("auto")]
    Auto,

}

