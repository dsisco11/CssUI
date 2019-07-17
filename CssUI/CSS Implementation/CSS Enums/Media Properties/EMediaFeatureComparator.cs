using CssUI.CSS.Internal;
using System;

namespace CssUI.CSS.Media
{
    /// <summary>
    /// Describes the comparison logic for media a feature, these are flags and a feature can have more than one comparator
    /// </summary>
    [Flags, CssEnum]
    public enum EMediaFeatureComparator : int
    {
        [CssKeyword("<")]
        LessThan = (1 << 1),
        [CssKeyword("=")]
        EqualTo = (1 << 2),
        [CssKeyword(">")]
        GreaterThan = (1 << 3)
    }
}
