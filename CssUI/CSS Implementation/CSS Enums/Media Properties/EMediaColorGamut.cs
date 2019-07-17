using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaColorGamut : int
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#color-gamut */

        /// <summary>
        /// The output device can support approximately the sRGB gamut or more.
        /// </summary>
        [CssKeyword("srgb")]
        SRGB,

        /// <summary>
        /// The output device can support approximately the gamut specified by the DCI P3 Color Space or more.
        /// </summary>
        [CssKeyword("p3")]
        P3,

        /// <summary>
        /// The output device can support approximately the gamut specified by the ITU-R Recommendation BT.2020 Color Space or more.
        /// </summary>
        [CssKeyword("rec2020")]
        REC2020,

    }
}