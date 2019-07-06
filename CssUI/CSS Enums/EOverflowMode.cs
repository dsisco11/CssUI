using CssUI.Internal;

namespace CssUI.Enums
{
    [CssEnum]
    public enum EOverflowMode : int
    {
        /// <summary>
        /// There is no special handling of overflow, that is, it may be rendered outside the box. The box is not a scroll container.
        /// </summary>
        [CssKeyword("visible")]
        Visible,
        /// <summary>
        /// This value indicates that the box’s content is clipped to its padding box and that no scrolling user interface should be provided by the UA to view the content outside the clipping region. However, the content may still be scrolled programatically, for example using the mechanisms defined in [CSSOM-VIEW], and the box is therefore still a scroll container.
        /// </summary>
        [CssKeyword("hidden")]
        Hidden,
        /// <summary>
        /// Like hidden, this value indicates that the box’s content is clipped to its padding box and that no scrolling user interface should be provided by the UA to view the content outside the clipping region. In addition, unlike overflow: hidden which still allows programmatic scrolling, overflow: clip forbids scrolling entirely, through any mechanism, and therefore the box is not a scroll container.
        /// </summary>
        [CssKeyword("clip")]
        Clip,
        /// <summary>
        /// This value indicates that the content is clipped to the padding box, but can be scrolled into view (and therefore the box is a scroll container). Furthermore, if the user agent uses a scrolling mechanism that is visible on the screen (such as a scroll bar or a panner), that mechanism should be displayed whether or not any of its content is clipped. This avoids any problem with scrollbars appearing and disappearing in a dynamic environment. When this value is specified and the target medium is print, overflowing content may be printed.
        /// </summary>
        [CssKeyword("scroll")]
        Scroll,
        /// <summary>
        /// This value indicates that the box’s content is clipped to the padding box, but can be scrolled into view (and therefore the box is a scroll container). However, if the user agent uses a scrolling mechanism that is visible on the screen (such as a scroll bar or a panner), that mechanism should only be displayed if there is overflow.
        /// </summary>
        [CssKeyword("auto")]
        Auto,
    }
}
