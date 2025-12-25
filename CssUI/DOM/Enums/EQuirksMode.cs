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
    [EnumData("quirks")]
    Quirks,

    /// <summary>
    ///
    /// </summary>
    [EnumData("no-quirks")]
    NoQuirks,

    /// <summary>
    ///
    /// </summary>
    [EnumData("limited-quirks")]
    LimitedQuirks
}

