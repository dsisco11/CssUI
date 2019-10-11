using CssUI.Internal;

namespace CssUI.Devices
{
    [MetaEnum]
    public enum EPointerType : short
    {/* Docs: https://w3c.github.io/pointerevents/#pointerevent-interface */
        [MetaKeyword("mouse")]
        Mouse = 0,
        [MetaKeyword("pen")]
        PenStylus = 1,
        [MetaKeyword("touch")]
        TouchContact = 2,


        MAX// Tracks the end of the enum
    }
}
