using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// Describes the comparison logic for media a feature, these are flags and a feature can have more than one comparator
    /// </summary>
    [CssEnum]
    public enum EMediaOperator : int
    {
        [CssKeyword("<")]
        LessThan,
        [CssKeyword("=")]
        EqualTo,
        [CssKeyword(">")]
        GreaterThan,

        [CssKeyword("<=")]
        LessThanEq,
        [CssKeyword(">=")]
        GreaterThanEq,
    }
}
