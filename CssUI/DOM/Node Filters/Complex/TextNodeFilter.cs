﻿using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt an instance of <see cref="Text"/>
    /// </summary>
    public class TextNodeFilter : NodeFilter
    {
        public static NodeFilter Instance = new TextNodeFilter();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType == ENodeType.TEXT_NODE || node is Text)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
