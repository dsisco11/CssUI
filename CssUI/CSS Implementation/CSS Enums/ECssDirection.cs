using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    [CssEnum]
    public enum ECssDirection : int
    {
        [CssKeyword("ltr")]
        LTR = 1,
        [CssKeyword("rtl")]
        RTL
    }
}
