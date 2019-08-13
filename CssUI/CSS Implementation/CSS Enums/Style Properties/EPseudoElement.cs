using CssUI.Internal;

namespace CssUI.CSS.Enums
{
    [MetaEnum]
    public enum EPseudoElement : int
    {
        [MetaKeyword("::before")]
        Before,

        [MetaKeyword("::after")]
        After,

        [MetaKeyword("::first-letter")]
        First_Letter,

        [MetaKeyword("::first-line")]
        First_Line,

        [MetaKeyword("::marker")]
        Marker,

        [MetaKeyword("::placeholder")]
        Placeholder,
    }
}
