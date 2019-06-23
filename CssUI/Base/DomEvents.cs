using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Describes all of the events for CSS elements
    /// </summary>
    public interface DomEvents
    {
        #region Mouse Event Delegates
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomMouseButtonEventArgs> MouseUp;
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomMouseButtonEventArgs> MouseDown;
        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomMouseWheelEventArgs> MouseWheel;
        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomRoutedEventArgs> Clicked;
        /// <summary>
        /// Called whenever the element is 'double-clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomRoutedEventArgs> DoubleClicked;
        /// <summary>
        /// Called whenever the mouse clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomMouseButtonEventArgs> MouseClick;
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomMouseButtonEventArgs> MouseDoubleClick;
        /// <summary>
        /// Called whenever the mouse moves whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement, DomMouseMoveEventArgs> MouseMove;
        /// <summary>
        /// Called whenever the mouse pauses for longer than <see cref="UI_CONSTANTS.HOVER_TIME"/> over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        event Action<cssElement> MouseHover;
        /// <summary>
        /// Called whenever the mouse first moves overtop the element
        /// </summary>
        event Action<cssElement> MouseEnter;
        /// <summary>
        /// Called whenever the mouse moves off the element
        /// </summary>
        event Action<cssElement> MouseLeave;
        #endregion

        #region Keyboard Event Delegates
        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        event Action<cssElement, DomCancellableEvent<DomKeyboardKeyEventArgs>> KeyPress;
        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        event Action<cssElement, DomKeyboardKeyEventArgs> KeyUp;
        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        event Action<cssElement, DomKeyboardKeyEventArgs> KeyDown;
        #endregion


        #region Mouse Event Handlers
        /// <summary>
        /// Called whenever the mouse releases a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseUp(cssElement Sender, DomMouseButtonEventArgs Args);
        /// <summary>
        /// Called whenever the mouse presses a button whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseDown(cssElement Sender, DomMouseButtonEventArgs Args);

        /// <summary>
        /// Called whenever the element is 'clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_Click(cssElement Sender, DomRoutedEventArgs Args);

        /// <summary>
        /// Called whenever the element is 'double-clicked' by the mouse or by keyboard input (pressing ENTER)
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_DoubleClick(cssElement Sender, DomRoutedEventArgs Args);
        /// <summary>
        /// Called whenever the mouse clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseClick(cssElement Sender, DomMouseButtonEventArgs Args);
        /// <summary>
        /// Called whenever the mouse double clicks the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseDoubleClick(cssElement Sender, DomMouseButtonEventArgs Args);

        /// <summary>
        /// Called whenever the mouse wheel scrolls whilst over the element
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseWheel(cssElement Sender, DomMouseWheelEventArgs Args);
        /// <summary>
        /// Called whenever the mouse moves whilst over the element.
        /// <para>Fires after MouseEnter</para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseMove(cssElement Sender, DomMouseMoveEventArgs Args);
        /// <summary>
        /// Called whenever the mouse rests on the element
        /// <para>Rest delay dictated by <see cref="UI_CONSTANTS.HOVER_TIME"/></para>
        /// </summary>
        /// <param name="Sender">The UI element sending us this event, or NULL if we are the first element to receive it.</param>
        void Handle_MouseHover(cssElement Sender);
        /// <summary>
        /// Called whenever the mouse first moves overtop the element.
        /// <para>Fires before MouseMove</para>
        /// </summary>
        void Handle_MouseEnter(cssElement Sender);
        /// <summary>
        /// Called whenever the mouse moves off the element.
        /// </summary>
        void Handle_MouseLeave(cssElement Sender);
        #endregion

        #region Keyboard Event Handlers
        /// <summary>
        /// Called whenever the user presses a character key while the element has input-focus
        /// </summary>
        bool Handle_KeyPress(cssElement Sender, DomKeyboardKeyEventArgs Args);
        /// <summary>
        /// Called whenever a keyboard key is depressed while the element has input-focus
        /// </summary>
        bool Handle_KeyUp(cssElement Sender, DomKeyboardKeyEventArgs Args);
        /// <summary>
        /// Called whenever a keyboard key is pressed while the element has input-focus
        /// </summary>
        bool Handle_KeyDown(cssElement Sender, DomKeyboardKeyEventArgs Args);
        #endregion
    }

    /// <summary>
    /// Allows event receivers to prevent elements from responding to certain events.
    /// </summary>
    /// <typeparam name="Ty"></typeparam>
    public class DomCancellableEvent<Ty>
    {
        public readonly Ty Args;
        /// <summary>
        /// If True then the element will not act on this event.
        /// </summary>
        public bool Cancel = false;

        public DomCancellableEvent(Ty args)
        {
            Args = args;
        }
    }
}
