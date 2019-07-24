using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only nodes that are <see cref="HTMLFormElement"/>
    /// </summary>
    public class FilterForms : NodeFilter
    {
        public static NodeFilter Instance = new FilterForms();

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is HTMLFormElement)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
