using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM.Traversal
{
    /// <summary>
    /// Rejects any nodes which do not descend from the specified Node
    /// </summary>
    public class FilterDescendantOf : NodeFilter
    {
        private Node targetNode;

        public override ENodeFilterResult acceptNode(Node node)
        {
            /* omitting any node without a parent */
            if (ReferenceEquals(node.parentNode, null))
                return ENodeFilterResult.FILTER_REJECT;

            if (DOMCommon.Is_Descendant(node, targetNode))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_REJECT;
        }

        public FilterDescendantOf(Node targetNode)
        {
            this.targetNode = targetNode;
        }
    }
}
