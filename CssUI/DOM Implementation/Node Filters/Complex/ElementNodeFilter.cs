﻿using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt an instance of <see cref="Text"/>
    /// </summary>
    public class ElementNodeFilter : NodeFilter
    {
        public static ElementNodeFilter Instance = new ElementNodeFilter();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType == ENodeType.ELEMENT_NODE)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}