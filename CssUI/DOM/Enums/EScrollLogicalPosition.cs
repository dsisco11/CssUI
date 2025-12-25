using EnumRecords;

namespace CssUI.DOM.Enums;

[EnumRecord<KeywordProperties>]
public enum EScrollLogicalPosition : int
{
    /// <summary>
    ///
    /// </summary>
    [EnumData("start")]
    Start,

    /// <summary>
    ///
    /// </summary>
    [EnumData("center")]
    Center,

    /// <summary>
    ///
    /// </summary>
    [EnumData("end")]
    End,

    /// <summary>
    ///
    /// </summary>
    [EnumData("nearest")]
    Nearest,
}

