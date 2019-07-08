using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM.Traversal
{
    /// <summary>
    /// Rejects any nodes whose parent is contained by the given <see cref="Range"/> & Accepts any contained by is
    /// </summary>
    public class RangeDeleteFilter : NodeFilter
    {
        private Range targetRange;

        public override ENodeFilterResult acceptNode(Node node)
        {
            /* ...omitting any node whose parent is also contained in the context object. */
            if (ReferenceEquals(node.parentNode, null))
                return ENodeFilterResult.FILTER_REJECT;

            if (targetRange.Contains(node.parentNode))
                return ENodeFilterResult.FILTER_SKIP;

            if (targetRange.Contains(node))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_REJECT;
        }

        public RangeDeleteFilter(Range range)
        {
            this.targetRange = range;
        }
    }
}
