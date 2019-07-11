namespace CssUI.DOM.Events
{
    public class KeyboardEventInit : EventModifierInit
    {
        public string key = "";
        public string code = "";
        public EKeyLocation location = 0x0;
        public bool repeat = false;
        public bool isComposing = false;
    }
}
