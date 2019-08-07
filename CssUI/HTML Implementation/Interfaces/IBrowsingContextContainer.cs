using CssUI.DOM.Events;
using CssUI.DOM.Internal;

namespace CssUI.HTML
{
    public interface IBrowsingContextContainer : IEventTarget
    {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#browsing-context-container */
        BrowsingContext Nested_Browsing_Context { get; }
    }
}
