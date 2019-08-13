using CssUI.Internal;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// Describes the comparison logic for media a feature, these are flags and a feature can have more than one comparator
    /// </summary>
    [MetaEnum]
    public enum EMediaOperator : int
    {
        [MetaKeyword("<")]
        LessThan,
        [MetaKeyword("=")]
        EqualTo,
        [MetaKeyword(">")]
        GreaterThan,

        [MetaKeyword("<=")]
        LessThanEq,
        [MetaKeyword(">=")]
        GreaterThanEq,
    }
}
