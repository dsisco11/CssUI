using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaType : int
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#media-types */

        NONE = 0x0,
        /// <summary>
        /// Matches all devices
        /// </summary>
        [CssKeyword("all")]
        All,
        /// <summary>
        /// Matches printers, and devices intended to reproduce a printed display, such as a web browser showing a document in “Print Preview”.
        /// </summary>
        [CssKeyword("print")]
        Print,
        /// <summary>
        /// Matches all devices that aren’t matched by print or speech.
        /// </summary>
        [CssKeyword("screen")]
        Screen,
        /// <summary>
        /// Matches devices that similar devices that “read out” a page.
        /// </summary>
        [CssKeyword("speech")]
        Speech,

        /* The following are DEPRECIATED values. they must be recognized, but ignored. */

        [CssKeyword("tty")]
        tty,
        [CssKeyword("tv")]
        tv,
        [CssKeyword("projection")]
        projection,
        [CssKeyword("handheld")]
        handheld,
        [CssKeyword("braille")]
        braille,
        [CssKeyword("embossed")]
        embossed,
        [CssKeyword("aural")]
        aural,
    }
}
