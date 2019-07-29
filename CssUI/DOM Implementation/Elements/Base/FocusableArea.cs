using CssUI.DOM.Events;
using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Represents an area within a document that can be focused
    /// </summary>
    internal class FocusableArea
    {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#focusable-area */
        #region Properties
        public readonly EventTarget FocusTarget = null;
        /// <summary>
        /// Each focusable area has a DOM anchor, which is a Node object that represents the position of the focusable area in the DOM. 
        /// (When the focusable area is itself a Node, it is its own DOM anchor.) 
        /// The DOM anchor is used in some APIs as a substitute for the focusable area when there is no other DOM object to represent the focusable area.
        /// </summary>
        public readonly Node DOMAnchor = null;

        #endregion

        #region Constructors

        private FocusableArea(EventTarget focusTarget)
        {
            FocusTarget = focusTarget;
            if (focusTarget is Node targetNode)
            {
                DOMAnchor = targetNode;
            }
        }

        private FocusableArea(EventTarget focusTarget, Node DOMAnchor)
        {
            FocusTarget = focusTarget;
            this.DOMAnchor = DOMAnchor;
        }
        #endregion

        #region Implicit
        public static implicit operator FocusableArea(Element element) => new FocusableArea(element);
        public static implicit operator FocusableArea(Viewport viewport) => new FocusableArea(viewport, viewport.document);
        public static implicit operator FocusableArea(ScrollBox scrollbox) => new FocusableArea(scrollbox, scrollbox.Owner);
        public static implicit operator FocusableArea(Document document) => new FocusableArea(document, document.documentElement);
        public static implicit operator FocusableArea(BrowsingContext context) => new FocusableArea(context, context.activeDocument);
        #endregion
    }
}
