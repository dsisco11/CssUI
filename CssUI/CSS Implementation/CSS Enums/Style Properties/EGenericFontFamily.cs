using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Provides values to reference the CSS defined generic font familys
    /// </summary>
    [MetaEnum]
    public enum EGenericFontFamily
    {
        [MetaKeyword("serif")]
        Serif = 0,
        [MetaKeyword("sans-serif")]
        SansSerif,
        [MetaKeyword("cursive")]
        Cursive,
        [MetaKeyword("fantasy")]
        Fantasy,
        [MetaKeyword("monospace")]
        Monospace
    }
}
