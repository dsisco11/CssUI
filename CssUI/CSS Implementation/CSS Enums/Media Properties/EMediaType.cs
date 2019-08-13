using CssUI.Internal;

namespace CssUI.CSS.Media
{
    [MetaEnum]
    public enum EMediaType : int
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#media-types */

        NONE = 0x0,
        /// <summary>
        /// Matches all devices
        /// </summary>
        [MetaKeyword("all")]
        All,
        /// <summary>
        /// Matches printers, and devices intended to reproduce a printed display, such as a web browser showing a document in “Print Preview”.
        /// </summary>
        [MetaKeyword("print")]
        Print,
        /// <summary>
        /// Matches all devices that aren’t matched by print or speech.
        /// </summary>
        [MetaKeyword("screen")]
        Screen,
        /// <summary>
        /// Matches devices that similar devices that “read out” a page.
        /// </summary>
        [MetaKeyword("speech")]
        Speech,

        /* The following are DEPRECIATED values. they must be recognized, but ignored. */

        [MetaKeyword("tty")]
        tty,
        [MetaKeyword("tv")]
        tv,
        [MetaKeyword("projection")]
        projection,
        [MetaKeyword("handheld")]
        handheld,
        [MetaKeyword("braille")]
        braille,
        [MetaKeyword("embossed")]
        embossed,
        [MetaKeyword("aural")]
        aural,
    }
}
