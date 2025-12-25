using EnumRecords;

namespace CssUI.Devices;

[EnumRecord<KeywordProperties>]
public enum EPointerType : short
{/* Docs: https://w3c.github.io/pointerevents/#pointerevent-interface */
    [EnumData("mouse")]
    Mouse = 0,
    [EnumData("pen")]
    PenStylus = 1,
    [EnumData("touch")]
    TouchContact = 2,


    MAX// Tracks the end of the enum
}

