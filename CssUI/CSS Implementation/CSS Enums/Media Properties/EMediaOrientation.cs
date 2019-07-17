using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaOrientation
    {
        /// <summary>
        /// The orientation media feature is portrait when the value of the height media feature is greater than or equal to the value of the width media feature.
        /// </summary>
        [CssKeyword("portrait")]
        Portrait,
        /// <summary>
        /// The orientation media feature is portrait when the value of the width media feature is greater than or equal to the value of the height media feature.
        /// </summary>
        [CssKeyword("landscape")]
        Landscape,
    }
}
