using CssUI.Devices;

namespace CssUI.DOM.Events
{
    public class KeyboardEventInit : EventModifierInit
    {
        public string key = "";
        public EKeyboardCode code = 0x0;
        public EKeyLocation location = 0x0;
        public bool repeat = false;
        public bool isComposing = false;
    }
}
