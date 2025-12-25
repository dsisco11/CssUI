using EnumRecords;

namespace CssUI.DOM.Enums;

[EnumRecord<KeywordProperties>]
public enum EScrollLogicalPosition : int
{
    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("start")]
    Start,

    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("center")]
    Center,

    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("end")]
    End,

    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("nearest")]
    Nearest,
}

