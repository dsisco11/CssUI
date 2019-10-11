using CssUI.DOM.Enums;
using CssUI.DOM.Events;

namespace CssUI.DOM.Nodes
{
    public class ShadowRoot : DocumentFragment
    {/* Docs: https://dom.spec.whatwg.org/#interface-shadowroot */
        #region Properties
        public EShadowRootMode Mode { get; private set; }
        /* Host property provided by DocumentFragment */
        //public Element Host;
        #endregion

        #region Constructor
        public ShadowRoot(Element Host, Document document, EShadowRootMode mode) : base(Host, document)
        {
            SetFlag(ENodeFlags.IsShadowRoot);
            Mode = mode;
        }
        #endregion

        public override EventTarget get_the_parent(Event @event)
        {
            /* A shadow root’s get the parent algorithm, given an event, returns null if event’s composed flag is unset and shadow root is the root of event’s path’s first struct’s invocation target, and shadow root’s host otherwise. */
            if (!@event.composed && @event.Path.Count > 0 && ReferenceEquals(this, @event.Path[0].invocationTarget))
            {
                return null;
            }

            return Host;
        }
    }
}
