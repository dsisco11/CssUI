using CssUI.Internal;

namespace CssUI.CSS.Media
{
    [MetaEnum]
    public enum EMediaUpdate
    {
        /// <summary>
        /// Once it has been rendered, the layout can no longer be updated. Example: documents printed on paper.
        /// </summary>
        [MetaKeyword("none")]
        None,

        /// <summary>
        /// The layout may change dynamically according to the usual rules of CSS, but the output device is not able to render or display changes quickly enough for them to be perceived as a smooth animation. 
        /// Example: E-ink screens or severely under-powered devices.
        /// </summary>
        [MetaKeyword("slow")]
        Slow,

        /// <summary>
        /// The layout may change dynamically according to the usual rules of CSS, and the output device is not unusually constrained in speed, so regularly-updating things like CSS Animations can be used. Example: computer screens.
        /// </summary>
        [MetaKeyword("fast")]
        Fast,

    }
}
