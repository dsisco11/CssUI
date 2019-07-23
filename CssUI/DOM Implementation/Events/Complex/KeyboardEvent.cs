using System;

namespace CssUI.DOM.Events
{
    public class KeyboardEvent : UIEvent
    {/* Docs: https://w3c.github.io/uievents/#idl-keyboardevent */
        // KeyLocationCode
        public static Type initType = typeof(KeyboardEventInit);

        #region Properties
        /// <summary>
        /// key holds the value corresponding to the key pressed.
        /// </summary>
        public string key { get; private set; } = string.Empty;

        /// <summary>
        /// Identifies the physical key being pressed. 
        /// The value is not affected by the current keyboard layout or modifier state, so a particular key will always return the same value.
        /// </summary>
        public EKeyboardCode code { get; private set; } = 0x0;

        /// <summary>
        /// The location attribute contains an indication of the logical location of the key on the device.
        /// </summary>
        public EKeyLocation location { get; private set; } = 0x0;

        /// <summary>
        /// true if the Control (control) key modifier was active.
        /// </summary>
        public bool ctrlKey { get; private set; } = false;
        /// <summary>
        /// true if the shift (Shift) key modifier was active.
        /// </summary>
        public bool shiftKey { get; private set; } = false;
        /// <summary>
        /// true if the Alt (alternative) (or "Option") key modifier was active.
        /// </summary>
        public bool altKey { get; private set; } = false;
        /// <summary>
        /// true if the meta (Meta) key modifier was active.
        /// </summary>
        public bool metaKey { get; private set; } = false;

        /// <summary>
        /// true if the key has been pressed in a sustained manner.
        /// </summary>
        public bool repeat { get; private set; } = false;
        /// <summary>
        /// true if the key event occurs as part of a composition session, i.e., after a compositionstart event and before the corresponding compositionend event.
        /// </summary>
        public bool isComposing { get; private set; } = false;
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
        public KeyboardEvent(EEventName type, KeyboardEventInit eventInit) : base(type, eventInit)
        {
            key = eventInit.key;
            code = eventInit.code;
            location = eventInit.location;
            repeat = eventInit.repeat;
            isComposing = eventInit.isComposing;

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
