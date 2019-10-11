using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any nodes which do not descend from the specified Node
    /// </summary>
    public class FilterLayoutBoxHitTest : NodeFilter
    {
        private readonly double x, y;
        public FilterLayoutBoxHitTest(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType == ENodeType.ELEMENT_NODE && node is Element element)
            {
                if (element.Box == null) return ENodeFilterResult.FILTER_REJECT;
                if (!element.Box.HitTest(x, y)) return ENodeFilterResult.FILTER_REJECT;

                return ENodeFilterResult.FILTER_ACCEPT;
            }

            return ENodeFilterResult.FILTER_REJECT;
        }

    }
}
