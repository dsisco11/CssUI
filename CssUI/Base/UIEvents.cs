using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Describes all of the events for UI element's
    /// </summary>
    public interface UIEvents
    {
        #region Mouse Event Delegates
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomMouseButtonEventArgs> MouseUp;
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomMouseButtonEventArgs> MouseDown;
        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomMouseWheelEventArgs> MouseWheel;
        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomRoutedEventArgs> Clicked;
        /// <summary>
        /// Called whenever the element is 'double-clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomRoutedEventArgs> DoubleClicked;
        /// <summary>
        /// Called whenever the mouse clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomMouseButtonEventArgs> MouseClick;
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomMouseButtonEventArgs> MouseDoubleClick;
        /// <summary>
        /// Called whenever the mouse moves whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement, DomMouseMoveEventArgs> MouseMove;
        /// <summary>
        /// Called whenever the mouse pauses for longer than <see cref="UI_CONSTANTS.HOVER_TIME"/> over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<uiElement> MouseHover;
        /// <summary>
        /// Called whenever the mouse first moves overtop the element
        /// </summary>
        event Action<uiElement> MouseEnter;
        /// <summary>
        /// Called whenever the mouse moves off the element
        /// </summary>
        event Action<uiElement> MouseLeave;
        #endregion

        #region Keyboard Event Delegates
        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        event Action<uiElement, uiCancellableEvent<DomKeyboardKeyEventArgs>> KeyPress;
        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        event Action<uiElement, DomKeyboardKeyEventArgs> KeyUp;
        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        event Action<uiElement, DomKeyboardKeyEventArgs> KeyDown;
        #endregion


        #region Mouse Event Handlers
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseUp(uiElement Sender, DomMouseButtonEventArgs Args);
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseDown(uiElement Sender, DomMouseButtonEventArgs Args);

        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_Click(uiElement Sender, DomRoutedEventArgs Args);

        /// <summary>
        /// Called whenever the element is 'double-clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_DoubleClick(uiElement Sender, DomRoutedEventArgs Args);
        /// <summary>
        /// Called whenever the mouse clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseClick(uiElement Sender, DomMouseButtonEventArgs Args);
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseDoubleClick(uiElement Sender, DomMouseButtonEventArgs Args);

        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseWheel(uiElement Sender, DomMouseWheelEventArgs Args);
        /// <summary>
        /// Called whenever the mouse moves whilst over the element.
        /// <para>Fires after MouseEnter</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseMove(uiElement Sender, DomMouseMoveEventArgs Args);
        /// <summary>
        /// Called whenever the mouse rests on the element
        /// <para>Rest delay dictated by <see cref="UI_CONSTANTS.HOVER_TIME"/></para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseHover(uiElement Sender);
        /// <summary>
        /// Called whenever the mouse first moves overtop the element.
        /// <para>Fires before MouseMove</para>
        /// </summary>
        void Handle_MouseEnter(uiElement Sender);
        /// <summary>
        /// Called whenever the mouse moves off the element.
        /// </summary>
        void Handle_MouseLeave(uiElement Sender);
        #endregion

        #region Keyboard Event Handlers
        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        bool Handle_KeyPress(uiElement Sender, DomKeyboardKeyEventArgs Args);
        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        bool Handle_KeyUp(uiElement Sender, DomKeyboardKeyEventArgs Args);
        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        bool Handle_KeyDown(uiElement Sender, DomKeyboardKeyEventArgs Args);
        #endregion
    }

    /// <summary>
    /// Allows event receivers to prevent elements from responding to certain events.
    /// </summary>
    /// <typeparam name="Ty"></typeparam>
    public class uiCancellableEvent<Ty>
    {
        public readonly Ty Args;
        /// <summary>
        /// If True then the element will not act on this event.
        /// </summary>
        public bool Cancel = false;

        public uiCancellableEvent(Ty args)
        {
            Args = args;
        }
    }
}
