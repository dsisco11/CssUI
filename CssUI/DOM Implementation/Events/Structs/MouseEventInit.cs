namespace CssUI.DOM.Events
{
    public class MouseEventInit : EventModifierInit
    {
        public long screenX = 0;
        public long screenY = 0;
        public long clientX = 0;
        public long clientY = 0;

        public EMouseButton button = 0;
        public EMouseButtonFlags buttons = 0;
        public EventTarget relatedTarget = null;
    }
}
