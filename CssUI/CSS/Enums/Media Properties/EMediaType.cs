using EnumRecords;

namespace CssUI.CSS.Media;

[EnumRecord<KeywordProperties>]
public enum EMediaType : int
{/* Docs: https://drafts.csswg.org/mediaqueries-4/#media-types */

    NONE = 0x0,
    /// <summary>
    /// Matches all devices
    /// </summary>
    [EnumRecordProperties("all")]
    All,
    /// <summary>
    /// Matches printers, and devices intended to reproduce a printed display, such as a web browser showing a document in “Print Preview”.
    /// </summary>
    [EnumRecordProperties("print")]
    Print,
    /// <summary>
    /// Matches all devices that aren’t matched by print or speech.
    /// </summary>
    [EnumRecordProperties("screen")]
    Screen,
    /// <summary>
    /// Matches devices that similar devices that “read out” a page.
    /// </summary>
    [EnumRecordProperties("speech")]
    Speech,

    /* The following are DEPRECIATED values. they must be recognized, but ignored. */

    [EnumRecordProperties("tty")]
    tty,
    [EnumRecordProperties("tv")]
    tv,
    [EnumRecordProperties("projection")]
    projection,
    [EnumRecordProperties("handheld")]
    handheld,
    [EnumRecordProperties("braille")]
    braille,
    [EnumRecordProperties("embossed")]
    embossed,
    [EnumRecordProperties("aural")]
    aural,
}

