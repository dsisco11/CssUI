using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUI.HTML;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt lableable
    /// </summary>
    public class FilterIsLableable : NodeFilter
    {
        public static NodeFilter Instance = new FilterIsLableable();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType != ENodeType.ELEMENT_NODE || !(node is HTMLElement nodeElement))
                return ENodeFilterResult.FILTER_REJECT;

            if (DOMCommon.Is_Lableable_Element(nodeElement))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
