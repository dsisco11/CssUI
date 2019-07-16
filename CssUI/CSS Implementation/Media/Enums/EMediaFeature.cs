using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaFeature : int
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mf-dimensions */
        [CssKeyword("unknown")]
        Unknown = 0x0,
        [CssKeyword("width")]
        Width = 1,
        [CssKeyword("height")]
        Height,
        [CssKeyword("aspect-ratio")]
        AspectRatio,
        [CssKeyword("orientation")]
        Orientation,
        [CssKeyword("resolution")]
        Resolution,
        [CssKeyword("scan")]
        Scan,
        [CssKeyword("grid")]
        Grid,
        [CssKeyword("update")]
        Update,
        [CssKeyword("overflow-block")]
        OverflowBlock,
        [CssKeyword("overflow-inline")]
        OverflowInline,
        [CssKeyword("color")]
        Color,
        [CssKeyword("color-index")]
        ColorIndex,
        [CssKeyword("monochrome")]
        Monochrome,
        [CssKeyword("color-gamut")]
        ColorGamut,
/*
        [CssKeyword("pointer")]
        Pointer,
        [CssKeyword("hover")]
        Hover,
        [CssKeyword("any-pointer")]
        AnyPointer,
        [CssKeyword("any-hover")]
        AnyHover,
*/
        [CssKeyword("")]
        ,
    }
}
