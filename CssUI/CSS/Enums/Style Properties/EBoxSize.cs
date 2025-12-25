using EnumRecords;

namespace CssUI.CSS;

[EnumRecord<KeywordProperties>]
public enum EBoxSize
{
    //Auto,// This is already just defined as a special, reserved CssValue TYPE
    //None,// This is already just defined as a special, reserved CssValue TYPE
    /// <summary>
    ///
    /// </summary>
    [EnumData("min-content")]
    Min_Content,

    /// <summary>
    ///
    /// </summary>
    [EnumData("max-content")]
    Max_Content,

    /// <summary>
    ///
    /// </summary>
    [EnumData("fit-content")]
    Fit_Content,
}

