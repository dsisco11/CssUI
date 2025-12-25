using EnumRecords;

namespace CssUI.Devices;

[EnumRecord<KeywordProperties>]
public enum EPointerType : short
{/* Docs: https://w3c.github.io/pointerevents/#pointerevent-interface */
    [EnumRecordProperties("mouse")]
    Mouse = 0,
    [EnumRecordProperties("pen")]
    PenStylus = 1,
    [EnumRecordProperties("touch")]
    TouchContact = 2,


    MAX// Tracks the end of the enum
}

