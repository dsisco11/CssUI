using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Provides values to reference the CSS defined generic font familys
    /// </summary>
    [CssEnum]
    public enum EGenericFontFamily
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
