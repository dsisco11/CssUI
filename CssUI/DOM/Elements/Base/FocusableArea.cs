using CssUI.DOM.Events;
using CssUI.DOM.Internal;
using CssUI.DOM.Nodes;

#if ENABLE_HTML
using CssUI.HTML;
#endif

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

        /// <summary>
        /// Returns <c>True</c> if the given <paramref name="target"/> is a valid focusable area
        /// </summary>
        public static bool Is_Focusable(EventTarget target)
        {/* Docs: https://html.spec.whatwg.org/multipage/interaction.html#focusable-area */

            /* This list may be incomplete, there may be other valid focusable areas not accounted for here */

#if ENABLE_HTML
            /* Elements that have their tabindex focus flag set, that are not actually disabled, that are not expressly inert, and that are either being rendered or being used as relevant canvas fallback content. */
            if (target is HTMLElement htmlElement)
            {
                if (htmlElement.tabindex_focus_flag && !htmlElement.is_actually_disabled && !htmlElement.isExpresslyInert)
                {
                    if (htmlElement.is_being_rendered || DOMCommon.Is_Being_Used_As_Canvas_Fallback_Content(htmlElement))
                    {
                        return true;
                    }
                }
            }

            /* The shapes of area elements in an image map associated with an img element that is being rendered and is not expressly inert. */
            if (target is HTMLAreaElement areaElement)
            {
                /* XXX: figure this out */
            }

            if (target is Element element)
            {
                /* The contents of an iframe */
                if (element.parentElement is HTMLIFrameElement)
                {
                    return true;
                }
            }
#endif
            /* The scrollable regions of elements that are being rendered and are not expressly inert. */
            if (target is ScrollBox scrollbox && scrollbox.Owner.is_being_rendered && !scrollbox.Owner.isExpresslyInert)
            {
                return true;
            }

            /* The viewport of a Document that has a non-null browsing context and is not inert. */
            if (target is IViewport viewport && viewport.document.BrowsingContext != null)
            {
                return true;
            }


            if (target is Document document) return true;
            if (target is BrowsingContext context) return true;



            /* The user-agent provided subwidgets of elements that are being rendered and are not actually disabled or expressly inert. */
            /* XXX: dont forget these */


            /* Any other element or part of an element, especially to aid with accessibility or to better match platform conventions. */
            /* XXX: This seems to contradict the fact that elements must have the tabindex_focus_flag set */


            return false;
        }

        #region Implicit
        public static implicit operator FocusableArea(EventTarget target)
        {
            if (target is Element element) return new FocusableArea(element);
            else if (target is Viewport viewport) return new FocusableArea(viewport, viewport.document);
            else if (target is ScrollBox scrollbox) return new FocusableArea(scrollbox, scrollbox.Owner);
            else if (target is Document document) return new FocusableArea(document, document.documentElement);
            else if (target is BrowsingContext context) return new FocusableArea(context, context.activeDocument);

            return new FocusableArea(target);
        }
        #endregion
    }
}
