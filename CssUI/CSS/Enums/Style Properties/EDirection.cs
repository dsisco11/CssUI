using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EDirection : int
{
    /// <summary>
    ///
    /// </summary>
    [EnumData("ltr")]
    LTR = 1,

    /// <summary>
    ///
    /// </summary>
    [EnumData("rtl")]
    RTL,
}

