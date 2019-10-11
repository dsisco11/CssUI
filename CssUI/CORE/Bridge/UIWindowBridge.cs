using System;
using System.Diagnostics.Contracts;
using CssUI.DOM;
using CssUI.Devices;
using CssUI.DOM.Events;

namespace CssUI
{
    /// <summary>
    /// Bridges the gap between the abstracted DOM Window object and the platform specific windowing/rendering systems
    /// </summary>
    public abstract class UIWindowBridge : Window
    {
        #region Properties
        private bool[] KeyState = new bool[(int)EKeyboardCode.MAX];
        #endregion

        #region Events
        /*public delegate void Keyboard_Input_Handler(KeyboardDevice Device, EKeyboardInputType EventType, EKeyboardCode KeyCode);
        public delegate void Pointer_Input_Handler(PointerDevice Device, EPointerInputType EventType);
        /// <summary>
        /// Passes keyboard input to the DOM window
        /// </summary>
        public event Keyboard_Input_Handler onKeyboardInput;
        /// <summary>
        /// Passes pointer (Mouse/Touch/Tablet) inputs to the DOM window
        /// </summary>
        public event Pointer_Input_Handler onPointerInput;*/
        #endregion

        #region Constructors
        public UIWindowBridge(Screen screen) : base(screen, "CssUI")
        {
            KeyState.Initialize();
        }
        #endregion

        /// <summary>
        /// Performs main-loop processing for the DOM window
        /// </summary>
        protected void Run_Event_Loop()
        {
        }

        #region Input Handling
        protected enum EKeyboardInputType { KeyDown, KeyUp, KeyPress }
        protected enum EPointerInputType { Moved, ButtonPress, ButtonRelease }

        KeyboardEventInit initKeyEvent(KeyboardDevice Device, EKeyboardCode KeyCode, char Key, bool isRepeat)
        {
            bool altKey = Device.IsDown(EKeyboardCode.AltLeft) | Device.IsDown(EKeyboardCode.AltRight);
            bool shiftKey = Device.IsDown(EKeyboardCode.ShiftLeft) | Device.IsDown(EKeyboardCode.ShiftRight);
            bool ctrlKey = Device.IsDown(EKeyboardCode.ControlLeft) | Device.IsDown(EKeyboardCode.ControlRight);
            bool superKey = Device.IsDown(EKeyboardCode.MetaLeft) | Device.IsDown(EKeyboardCode.MetaRight);
            string key = Key == 0 ? ((char)Lookup.Data(KeyCode).Data[1]).ToString() : Key.ToString();

            return new KeyboardEventInit()
            {
                view = this,
                key = key,
                code = KeyCode,
                repeat = isRepeat,

                altKey = altKey,
                shiftKey = shiftKey,
                ctrlKey = ctrlKey,
                metaKey = superKey,

                modifierCapsLock = Device.IsDown(EKeyboardCode.CapsLock),
                modifierFn = Device.IsDown(EKeyboardCode.Fn),
                modifierFnLock = Device.IsDown(EKeyboardCode.FnLock),
                modifierNumLock = Device.IsDown(EKeyboardCode.NumLock),
                modifierScrollLock = Device.IsDown(EKeyboardCode.ScrollLock),
                modifierSuper = superKey,

                modifierSymbol = false,
                modifierSymbolLock = false,
                modifierHyper = false,
                modifierAltGraph = false,
            };
        }

        /// <summary>
        /// Passes keyboard input to the DOM window
        /// </summary>
        protected void Handle_Keyboard_Input(KeyboardDevice Device, EKeyboardInputType InputType, EKeyboardCode KeyCode, char Key)
        {
            Event RetEvent = null;
            switch (InputType)
            {
                case EKeyboardInputType.KeyDown:
                    {
                        bool Old = KeyState[(int)KeyCode];
                        KeyState[(int)KeyCode] = true;
                        RetEvent = new KeyboardEvent(EEventName.KeyDown, initKeyEvent(Device, KeyCode, Key, Old == true));
                    }
                    break;
                case EKeyboardInputType.KeyUp:
                    {
                        KeyState[(int)KeyCode] = false;
                        RetEvent = new KeyboardEvent(EEventName.KeyUp, initKeyEvent(Device, KeyCode, Key, false));
                    }
                    break;
                case EKeyboardInputType.KeyPress:
                    {
                        KeyState[(int)KeyCode] = false;
                        RetEvent = new KeyboardEvent(EEventName.KeyPress, initKeyEvent(Device, KeyCode, Key, false));
                    }
                    break;
                default:
                    {
                        throw new NotImplementedException($"No logic implemented for {nameof(EKeyboardInputType)} value \"{InputType}\"");
                    }
            }

            dispatchEvent(RetEvent);
        }

        /// <summary>
        /// Passes pointer (Mouse/Touch/Tablet) inputs to the DOM window
        /// </summary>
        protected void Handle_Pointer_Input(PointerDevice Device, EPointerInputType InputType)
        {
            Event RetEvent = null;
            switch (InputType)
            {
                case EPointerInputType.Moved:
                    {
                    }
                    break;
                case EPointerInputType.ButtonPress:
                    {
                    }
                    break;
                case EPointerInputType.ButtonRelease:
                    {
                    }
                    break;
                default:
                    {
                        throw new NotImplementedException($"No logic implemented for {nameof(EPointerInputType)} value \"{InputType}\"");
                    }
            }

            dispatchEvent(RetEvent);
        }
        #endregion

    }
}
