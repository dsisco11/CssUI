using System.Collections.Generic;

namespace CssUI.DOM.Mutation
{
    public class MutationObserverInit {
        public bool childList = false;
        public bool attributes;
        public bool characterData;
        public bool subtree = false;
        public bool attributeOldValue;
        public bool characterDataOldValue;
        public IList<string> attributeFilter;

        public MutationObserverInit()
        {
        }
    }
}
