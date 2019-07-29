using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that is not disabled
    /// </summary>
    public class FilterNotDisabled : NodeFilter
    {
        public static NodeFilter Instance = new FilterNotDisabled();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType != ENodeType.ELEMENT_NODE || !(node is HTMLElement element))
                return ENodeFilterResult.FILTER_REJECT;

            if (!element.disabled)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
