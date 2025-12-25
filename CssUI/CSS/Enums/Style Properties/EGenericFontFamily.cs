using EnumRecords;

namespace CssUI.CSS;

/// <summary>
/// Provides values to reference the CSS defined generic font familys
/// </summary>
[EnumRecord<KeywordProperties>]
public enum EGenericFontFamily
{
    [EnumData("serif")]
    Serif = 0,
    [EnumData("sans-serif")]
    SansSerif,
    [EnumData("cursive")]
    Cursive,
    [EnumData("fantasy")]
    Fantasy,
    [EnumData("monospace")]
    Monospace
}

