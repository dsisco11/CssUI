using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Provides values to reference the CSS defined generic font familys
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EGenericFontFamily
{
    [EnumRecordProperties("serif")]
    Serif = 0,
    [EnumRecordProperties("sans-serif")]
    SansSerif,
    [EnumRecordProperties("cursive")]
    Cursive,
    [EnumRecordProperties("fantasy")]
    Fantasy,
    [EnumRecordProperties("monospace")]
    Monospace
}

