using System;

namespace CssUI.DOM.Events
{
    public class MouseEvent : UIEvent
    {
        public static Type initType = typeof(MouseEventInit);

        #region Properties
        /// <summary>
        /// The horizontal coordinate at which the event occurred relative to the origin of the screen coordinate system.
        /// </summary>
        public long screenX { get; private set; } = 0;
        /// <summary>
        /// The vertical coordinate at which the event occurred relative to the origin of the screen coordinate system.
        /// </summary>
        public long screenY { get; private set; } = 0;
        /// <summary>
        /// The horizontal coordinate at which the event occurred relative to the viewport associated with the event.
        /// </summary>
        public long clientX { get; private set; } = 0;
        /// <summary>
        /// The vertical coordinate at which the event occurred relative to the viewport associated with the event.
        /// </summary>
        public long clientY { get; private set; } = 0;

        /// <summary>
        /// Refer to the KeyboardEvent's ctrlKey attribute.
        /// </summary>
        public bool ctrlKey { get; private set; } = false;
        /// <summary>
        /// Refer to the KeyboardEvent's shiftKey attribute.
        /// </summary>
        public bool shiftKey { get; private set; } = false;
        /// <summary>
        /// Refer to the KeyboardEvent's altKey attribute.
        /// </summary>
        public bool altKey { get; private set; } = false;
        /// <summary>
        /// Refer to the KeyboardEvent's metaKey attribute.
        /// </summary>
        public bool metaKey { get; private set; } = false;

        /// <summary>
        /// During mouse events caused by the depression or release of a mouse button, button MUST be used to indicate which pointer device button changed state.
        /// </summary>
        public EMouseButton button { get; private set; } = 0x0;
        public EMouseButtonFlags buttons { get; private set; } = 0x0;

        /// <summary>
        /// Used to identify a secondary EventTarget related to a UI event, depending on the type of event.
        /// </summary>
        public EventTarget relatedTarget { get; private set; } = null;
        #endregion

        #region ModifierState
        public readonly bool modifierAltGraph = false;
        public readonly bool modifierCapsLock = false;
        public readonly bool modifierFn = false;
        public readonly bool modifierFnLock = false;
        public readonly bool modifierHyper = false;
        public readonly bool modifierNumLock = false;
        public readonly bool modifierScrollLock = false;
        public readonly bool modifierSuper = false;
        public readonly bool modifierSymbol = false;
        public readonly bool modifierSymbolLock = false;
        #endregion

        #region Constructors
        public MouseEvent(EEventName type, MouseEventInit eventInit)  : base(type, eventInit)
        {
            screenX = eventInit.screenX;
            screenY = eventInit.screenY;
            clientX = eventInit.clientX;
            clientY = eventInit.clientY;
            button = eventInit.button;
            buttons = eventInit.buttons;
            relatedTarget = eventInit.relatedTarget;

            ctrlKey = eventInit.ctrlKey;
            shiftKey = eventInit.shiftKey;
            altKey = eventInit.altKey;
            metaKey = eventInit.metaKey;

            modifierAltGraph = eventInit.modifierAltGraph;
            modifierCapsLock = eventInit.modifierCapsLock;
            modifierFn = eventInit.modifierFn;
            modifierFnLock = eventInit.modifierFnLock;
            modifierHyper = eventInit.modifierHyper;
            modifierNumLock = eventInit.modifierNumLock;
            modifierScrollLock = eventInit.modifierScrollLock;
            modifierSuper = eventInit.modifierSuper;
            modifierSymbol = eventInit.modifierSymbol;
            modifierSymbolLock = eventInit.modifierSymbolLock;
        }
        #endregion

        [Obsolete("This method will always return false, access modifier states from the properties eg; event.modifierCapsLock", true)]
        public bool getModifierState(string keyArg) => false;
    }
}
