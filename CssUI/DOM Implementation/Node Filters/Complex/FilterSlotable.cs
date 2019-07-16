using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt slotable
    /// </summary>
    public class FilterSlotable : NodeFilter
    {
        public static FilterSlotable Instance = new FilterSlotable();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.isSlottable)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
