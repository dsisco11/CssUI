using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts any node that is non-inert
    /// </summary>
    public class FilterNonInert : NodeFilter
    {
        public static NodeFilter Instance = new FilterNonInert();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType != ENodeType.ELEMENT_NODE)
                return ENodeFilterResult.FILTER_REJECT;

            if (!(node as Element).inert)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
