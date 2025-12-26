using EnumRecords;

namespace CssUI.CSS.Media;

[EnumRecord<KeywordProperties>]
public enum EMediaType : int
{/* Docs: https://drafts.csswg.org/mediaqueries-4/#media-types */

    [Ignore]
    NONE = 0x0,
    /// <summary>
    /// Matches all devices
    /// </summary>
    [EnumData("all")]
    All,
    /// <summary>
    /// Matches printers, and devices intended to reproduce a printed display, such as a web browser showing a document in “Print Preview”.
    /// </summary>
    [EnumData("print")]
    Print,
    /// <summary>
    /// Matches all devices that aren’t matched by print or speech.
    /// </summary>
    [EnumData("screen")]
    Screen,
    /// <summary>
    /// Matches devices that similar devices that “read out” a page.
    /// </summary>
    [EnumData("speech")]
    Speech,

    /* The following are DEPRECIATED values. they must be recognized, but ignored. */

    [EnumData("tty")]
    tty,
    [EnumData("tv")]
    tv,
    [EnumData("projection")]
    projection,
    [EnumData("handheld")]
    handheld,
    [EnumData("braille")]
    braille,
    [EnumData("embossed")]
    embossed,
    [EnumData("aural")]
    aural,
}

