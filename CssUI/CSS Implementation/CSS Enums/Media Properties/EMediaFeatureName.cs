using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaFeatureName : int
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mf-dimensions */
        
        Unknown = 0x0,

        /// <summary>
        /// The ‘width’ media feature describes the width of the targeted display area of the output device. 
        /// For continuous media, this is the width of the viewport (as described by CSS2, section 9.1.1 [CSS21]) including the size of a rendered scroll bar (if any). 
        /// For paged media, this is the width of the page box (as described by CSS2, section 13.2 [CSS21]).
        /// </summary>
        [CssKeyword("width")]
        Width = 1,
        [CssKeyword("min-width")]
        Min_Width,
        [CssKeyword("max-width")]
        Max_Width,

                /// <summary>
        /// The ‘height’ media feature describes the height of the targeted display area of the output device. 
        /// For continuous media, this is the height of the viewport including the size of a rendered scroll bar (if any). 
        /// For paged media, this is the height of the page box.
        /// </summary>
        [CssKeyword("height")]
        Height,
        [CssKeyword("min-height")]
        Min_Height,
        [CssKeyword("max-height")]
        Max_Height,
        
        /// <summary>
        /// The aspect-ratio media feature is defined as the ratio of the value of the width media feature to the value of the height media feature.
        /// </summary>
        [CssKeyword("aspect-ratio")]
        AspectRatio,
        [CssKeyword("min-aspect-ratio")]
        Min_AspectRatio,
        [CssKeyword("max-aspect-ratio")]
        Max_AspectRatio,


        /// <summary>
        /// portrait:
        ///     The orientation media feature is portrait when the value of the height media feature is greater than or equal to the value of the width media feature.
        /// landscape:
        /// Otherwise orientation is landscape.
        /// </summary>
        [CssKeyword("orientation")]
        Orientation,

        /// <summary>
        /// The resolution media feature describes the resolution of the output device, i.e. the density of the pixels, taking into account the page zoom but assuming a pinch zoom of 1.0.
        /// </summary>
        [CssKeyword("resolution")]
        Resolution,
        [CssKeyword("min-resolution")]
        Min_Resolution,
        [CssKeyword("max-resolution")]
        Max_Resolution,

        /// <summary>
        /// The scan media feature describes the scanning process of some output devices.
        /// </summary>
        [CssKeyword("scan")]
        Scan,

        /// <summary>
        /// The grid media feature is used to query whether the output device is grid or bitmap. 
        /// If the output device is grid-based (e.g., a “tty” terminal, or a phone display with only one fixed font), the value will be 1. Otherwise, the value will be 0.
        /// </summary>
        [CssKeyword("grid")]
        Grid,

        /// <summary>
        /// The update media feature is used to query the ability of the output device to modify the apearance of content once it has been rendered.
        /// </summary>
        [CssKeyword("update")]
        Update,

        /// <summary>
        /// The overflow-block media feature describes the behavior of the device when content overflows the initial containing block in the block axis.
        /// </summary>
        [CssKeyword("overflow-block")]
        OverflowBlock,

        /// <summary>
        /// The overflow-inline media feature describes the behavior of the device when content overflows the initial containing block in the inline axis.
        /// </summary>
        [CssKeyword("overflow-inline")]
        OverflowInline,

        /// <summary>
        /// The color media feature describes the number of bits per color component of the output device. If the device is not a color device, the value is zero.
        /// </summary>
        [CssKeyword("color")]
        Color,
        [CssKeyword("min-color")]
        Min_Color,
        [CssKeyword("max-color")]
        Max_Color,

        /// <summary>
        /// The color-index media feature describes the number of entries in the color lookup table of the output device. 
        /// If the device does not use a color lookup table, the value is zero.
        /// </summary>
        [CssKeyword("color-index")]
        ColorIndex,
        [CssKeyword("min-color-index")]
        Min_ColorIndex,
        [CssKeyword("max-color-index")]
        Max_ColorIndex,

        /// <summary>
        /// The monochrome media feature describes the number of bits per pixel in a monochrome frame buffer. If the device is not a monochrome device, the output device value will be 0.
        /// </summary>
        [CssKeyword("monochrome")]
        Monochrome,
        [CssKeyword("min-monochrome")]
        Min_Monochrome,
        [CssKeyword("max-monochrome")]
        Max_Monochrome,

        /// <summary>
        /// The color-gamut media feature describes the approximate range of colors that are supported by the UA and output device. 
        /// That is, if the UA receives content with colors in the specified space it can cause the output device to render the appropriate color, or something appropriately close enough.
        /// </summary>
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
    }
}
