using EnumRecords;

namespace CssUI.DOM;

[EnumRecord<KeywordProperties>]
public enum EDirName : int
{
    /// <summary>
    /// Indicates that the contents of the element are explicitly directionally isolated left-to-right text.
    /// </summary>
    [EnumData("ltr")]
    Ltr = 1,

    /// <summary>
    /// Indicates that the contents of the element are explicitly directionally isolated right-to-left text.
    /// </summary>
    [EnumData("rtl")]
    Rtl,

}

