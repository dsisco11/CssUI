using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt slotable
    /// </summary>
    public class FilterIsActive : NodeFilter
    {
        public static NodeFilter Instance = new FilterIsActive();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType != ENodeType.ELEMENT_NODE || !(node is HTMLElement))
                return ENodeFilterResult.FILTER_REJECT;

            if ((node as HTMLElement).is_being_activated)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
