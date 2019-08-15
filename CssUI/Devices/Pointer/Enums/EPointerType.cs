using CssUI.Internal;

namespace CssUI.Devices
{
    [MetaEnum]
    public enum EPointerType : short
    {/* Docs: https://w3c.github.io/pointerevents/#pointerevent-interface */
        [MetaKeyword("mouse")]
        Mouse,
        [MetaKeyword("pen")]
        PenStylus,
        [MetaKeyword("touch")]
        TouchContact,
    }
}
