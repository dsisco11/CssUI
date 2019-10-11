using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt flagged for updates
    /// </summary>
    public class FilterNodeUpdate : NodeFilter
    {
        public static NodeFilter Instance = new FilterNodeUpdate();
        const ENodeFlags UPDATE_MASK = (ENodeFlags.NeedsBoxUpdate | ENodeFlags.NeedsStyleUpdate | ENodeFlags.NeedsReflow) | (ENodeFlags.ChildNeedsBoxUpdate | ENodeFlags.ChildNeedsStyleUpdate | ENodeFlags.DirectChildNeedsStyleUpdate | ENodeFlags.ChildNeedsReflow);

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.GetFlag(UPDATE_MASK))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_REJECT;
        }
    }
}
