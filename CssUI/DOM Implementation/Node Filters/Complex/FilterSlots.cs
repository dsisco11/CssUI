using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only nodes that are <see cref="HTMLSlotElement"/>
    /// </summary>
    public class FilterSlots : NodeFilter
    {
        public static NodeFilter Instance = new FilterSlots();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is HTMLSlotElement)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
