using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EBorderSize : int
{
    /// <summary>
    ///
    /// </summary>
    [EnumData("thin")]
    Thin,

    /// <summary>
    ///
    /// </summary>
    [EnumData("medium")]
    Medium,

    /// <summary>
    ///
    /// </summary>
    [EnumData("thick")]
    Thick,
}

