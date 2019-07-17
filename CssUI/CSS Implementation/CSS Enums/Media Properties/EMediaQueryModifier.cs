using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaQueryModifier : int
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#media-query-modifier */

        None = 0x0,
        /// <summary>
        /// Negates the media query result
        /// </summary>
        [CssKeyword("not")]
        Not,
        /// <summary>
        /// The only keyword has no effect on the media query’s result, but will cause the media query to be parsed by legacy user agents as specifying the unknown media type “only”, and thus be ignored.
        /// </summary>
        [CssKeyword("only")]
        Only,
    }
}
