using CssUI.Internal;

namespace CssUI.Enums
{
    /// <summary>
    /// Provides values to reference the CSS defined generic font familys
    /// </summary>
    [CssEnum]
    public enum ECssGenericFontFamily
    {
        [CssKeyword("serif")]
        Serif = 0,
        [CssKeyword("sans-serif")]
        SansSerif,
        [CssKeyword("cursive")]
        Cursive,
        [CssKeyword("fantasy")]
        Fantasy,
        [CssKeyword("monospace")]
        Monospace
    }
}
