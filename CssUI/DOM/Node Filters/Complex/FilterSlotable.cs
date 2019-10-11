using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only nodes that implement <see cref="ISlottable"/>
    /// </summary>
    public class FilterSlotable : NodeFilter
    {
        public static NodeFilter Instance = new FilterSlotable();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is ISlottable)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
