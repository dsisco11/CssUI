using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// A custom element reaction event
    /// </summary>
    public interface IElementReaction
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#custom-element-reaction-queue */

        /// <summary>
        /// Executes the logic for this reaction
        /// </summary>
        void Handle(Element element);
    }
}
