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

        /// <summary>
        ///
        /// </summary>
        /// <param name="screenX">x-coordinate of the position where the event occurred relative to the origin of the <see cref="VisualViewport"/>.</param>
        /// <param name="screenY">y-coordinate of the position where the event occurred relative to the origin of the <see cref="VisualViewport"/>.</param>
        /// <param name="clientX">x-coordinate of the position where the event occurred relative to the origin of the <see cref="Viewport"/>.</param>
        /// <param name="clientY">y-coordinate of the position where the event occurred relative to the origin of the <see cref="Viewport"/>.</param>
        /// <param name="button"></param>
        /// <param name="buttons"></param>
        /// <param name="relatedTarget"></param>
        public MouseEventInit(long screenX, long screenY, long clientX, long clientY, EMouseButton button, EMouseButtonFlags buttons, EventTarget relatedTarget)
        {
            this.screenX = screenX;
            this.screenY = screenY;
            this.clientX = clientX;
            this.clientY = clientY;
            this.button = button;
            this.buttons = buttons;
            this.relatedTarget = relatedTarget;
        }
    }
}
