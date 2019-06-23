
namespace CssUI
{
    /// <summary>
    /// Implements a focus scope containing logical focus and active focus for a UI Element
    /// </summary>
    public class FocusScope
    {
        /// <summary>
        /// Whether this focus scope currently has active input focus
        /// </summary>
        private bool Active = false;
        /// <summary>
        /// The logical element which should recieve active input focus when it is available for this scope
        /// </summary>
        public cssElement FocusedElement { get; private set; } = null;
        /// <summary>
        /// Attempts to set logical focus to a specified element
        /// </summary>
        /// <param name="E">The element to set logical focus to</param>
        /// <returns>Focused element</returns>
        public cssElement Set(cssElement E)
        {
            if (E.HasFlags(EElementFlags.Focusable))
            {
                if (Active && FocusedElement != null) FocusedElement.Handle_InputFocusLose();
                FocusedElement = E;
                if (Active) E.Handle_InputFocusGain();
            }

            return FocusedElement;
        }

        /// <summary>
        /// Causes the focus scope to gain active input focus
        /// </summary>
        public void GainFocus()
        {
            Active = true;
            FocusedElement?.Handle_InputFocusGain();// Notify this element that it has gained active input focus
        }

        /// <summary>
        /// Causes the focus scope to lose active input focus
        /// </summary>
        public void LoseFocus()
        {
            Active = false;
            FocusedElement?.Handle_InputFocusLose();// Notify this element that it nolonger has active focus
        }
    }
}
