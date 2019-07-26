using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any element which isnt a label for the specified one
    /// </summary>
    public class FilterLabelFor : NodeFilter
    {
        public readonly Element Subject;

        public FilterLabelFor(Element Subject)
        {
            this.Subject = Subject;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is HTMLLabelElement label))
                return ENodeFilterResult.FILTER_SKIP;

            if (ReferenceEquals(Subject, label.control))
            {
                return ENodeFilterResult.FILTER_ACCEPT;
            }

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
