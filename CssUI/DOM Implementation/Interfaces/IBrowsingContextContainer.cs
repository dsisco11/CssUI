using CssUI.DOM.Internal;

namespace CssUI.DOM
{
    public interface IBrowsingContextContainer
    {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#browsing-context-container */
        BrowsingContext Nested_Browsing_Context { get; }
    }
}
