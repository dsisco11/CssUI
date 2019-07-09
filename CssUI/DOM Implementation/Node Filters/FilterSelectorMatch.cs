﻿using CssUI.CSS;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM.Traversal
{
    /// <summary>
    /// Rejects any nodes not matching the given CSS Selector
    /// </summary>
    public class FilterSelectorMatch : NodeFilter
    {
        private CssSelector Selector;

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element))
                return ENodeFilterResult.FILTER_SKIP;

            if (Selector.Match_Against_Element((Element)node))
                return ENodeFilterResult.FILTER_ACCEPT;


            return ENodeFilterResult.FILTER_REJECT;
        }

        public FilterSelectorMatch(CssSelector selector)
        {
            this.Selector = selector;
        }
    }
}
