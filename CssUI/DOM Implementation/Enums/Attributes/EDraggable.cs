using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    [DomEnum]
    public enum EDraggable : int
    {/* Docs: https://html.spec.whatwg.org/multipage/dnd.html#the-draggable-attribute */

        /// <summary>
        /// The auto state uses the default behavior of the user agent to determine if an element is draggable.
        /// </summary>
        Auto,

        /// <summary>
        /// The true state means the element is draggable
        /// </summary>
        [DomKeyword("true")]
        True,

        /// <summary>
        /// The false state means the element is not draggable
        /// </summary>
        [DomKeyword("false")]
        False,
    }
}
