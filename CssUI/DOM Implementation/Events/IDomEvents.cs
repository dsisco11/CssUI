using System;

namespace CssUI.DOM
{
    // DOCS: https://www.w3.org/TR/uievents/

    /// <summary>
    /// Describes all of the events for Dom elements
    /// </summary>
    public interface IDomEvents
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
    }
}
