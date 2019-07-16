using CssUI.DOM.Events;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an area within a document that can be focused
    /// </summary>
    internal class FocusableArea
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#focusable-area */
        #region Properties
        public readonly EventTarget FocusTarget;
        public readonly Element DOMAnchor;

        #endregion

        #region Constructors

        public FocusableArea(EventTarget FocusTarget, Element DOMAnchor)
        {
            this.FocusTarget = FocusTarget;
            this.DOMAnchor = DOMAnchor;
        }
        #endregion
    }
}
