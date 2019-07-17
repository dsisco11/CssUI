using CssUI.CSS.Internal;

namespace CssUI.CSS.Enums
{
    [CssEnum]
    public enum EPseudoElement : int
    {
        [CssKeyword("::before")]
        Before,

        [CssKeyword("::after")]
        After,

        [CssKeyword("::first-letter")]
        First_Letter,

        [CssKeyword("::first-line")]
        First_Line,

        [CssKeyword("::marker")]
        Marker,

        [CssKeyword("::placeholder")]
        Placeholder,
    }
}
