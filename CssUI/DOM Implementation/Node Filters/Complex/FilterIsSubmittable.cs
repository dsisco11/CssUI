using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt submittable
    /// </summary>
    public class FilterIsSubmittable : NodeFilter
    {
        public static NodeFilter Instance = new FilterIsSubmittable();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.nodeType != ENodeType.ELEMENT_NODE || !(node is HTMLElement nodeElement))
                return ENodeFilterResult.FILTER_REJECT;

            if (DOMCommon.Is_Submittable_Element(nodeElement))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
