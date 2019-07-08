using CssUI.DOM.Mutation;

namespace CssUI.DOM
{
    public class RegisteredObserver
    {
        public MutationObserver observer = null;
        public MutationObserverInit options = null;

        public RegisteredObserver(MutationObserver observer, MutationObserverInit options)
        {
            this.observer = observer;
            this.options = options;
        }
    }
}
