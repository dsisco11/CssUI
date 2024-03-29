﻿using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUI.HTML;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that is disabled
    /// </summary>
    public class FilterDisabled : NodeFilter
    {
        public static NodeFilter Instance = new FilterDisabled();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType != ENodeType.ELEMENT_NODE || !(node is HTMLElement element))
                return ENodeFilterResult.FILTER_REJECT;

            if (element.disabled)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
