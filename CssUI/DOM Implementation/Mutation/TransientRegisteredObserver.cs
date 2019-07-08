
namespace CssUI.DOM.Mutation
{
    public class TransientRegisteredObserver : RegisteredObserver
    {
        public RegisteredObserver source = null;

        public TransientRegisteredObserver(RegisteredObserver source, MutationObserver observer, MutationObserverInit options) : base(observer, options)
        {
            this.source = source;
        }
    }
}
