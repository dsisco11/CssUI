using EnumRecords;

namespace CssUI.CSS.Media;

/// <summary>
/// Combinators specify how a media query determines if it's set of features cause it to match a given document
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EMediaCombinator
{
    [EnumRecordProperties("")]
    None = 0x0,
    /// <summary>
    /// </summary>
    [EnumRecordProperties("and")]
    AND,

    /// <summary>
    /// Instantly returns true for a match
    /// </summary>
    [EnumRecordProperties("or")]
    OR,

    /// <summary>
    /// Negates the comparison result for a match
    /// </summary>
    [EnumRecordProperties("not")]
    NOT,
}

