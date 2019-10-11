using CssUI.Internal;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// Describes the comparison logic for media a feature
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
