using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using CssUI.HTML;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only nodes whose form-owner is the given one
    /// </summary>
    public class FilterFormOwner : NodeFilter
    {
        public readonly HTMLFormElement Subject = null;

        public FilterFormOwner(HTMLFormElement subject)
        {
            Subject = subject;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is FormAssociatedElement formElement && ReferenceEquals(Subject, formElement.form))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
