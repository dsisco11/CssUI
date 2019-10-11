using CssUI.Internal;

namespace CssUI.HTML
{
    [MetaEnum]
    public enum EDraggable : int
    {/* Docs: https://html.spec.whatwg.org/multipage/dnd.html#the-draggable-attribute */

        /// <summary>
        /// The auto state uses the default behavior of the user agent to determine if an element is draggable.
        /// </summary>
        Auto,

        /// <summary>
        /// The true state means the element is draggable
        /// </summary>
        [MetaKeyword("true")]
        True,

        /// <summary>
        /// The false state means the element is not draggable
        /// </summary>
        [MetaKeyword("false")]
        False,
    }
}
