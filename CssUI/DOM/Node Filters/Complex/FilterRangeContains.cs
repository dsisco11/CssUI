using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM.Traversal
{
    /// <summary>
    /// Skips any nodes which are not partially contained in the given range
    /// </summary>
    public class FilterRangeContains : NodeFilter
    {
        private Range targetRange;

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!targetRange.Contains(node))
                return ENodeFilterResult.FILTER_SKIP;

            return ENodeFilterResult.FILTER_ACCEPT;
        }

        public FilterRangeContains(Range range)
        {
            this.targetRange = range;
        }
    }
}
