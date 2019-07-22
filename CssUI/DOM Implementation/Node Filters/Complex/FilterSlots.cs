using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only nodes that implements <see cref="ISlot"/>
    /// </summary>
    public class FilterSlots : NodeFilter
    {
        public static FilterSlots Instance = new FilterSlots();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is ISlot)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
