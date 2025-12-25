using EnumRecords;

namespace CssUI.DOM.Enums;

/// <summary>
/// Describes the <see cref="Document"/>s quirks mode.
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EQuirksMode
{
    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("quirks")]
    Quirks,

    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("no-quirks")]
    NoQuirks,

    /// <summary>
    /// 
    /// </summary>
    [EnumRecordProperties("limited-quirks")]
    LimitedQuirks
}

