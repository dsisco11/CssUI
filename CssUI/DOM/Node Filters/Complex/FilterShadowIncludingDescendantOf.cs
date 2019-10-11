using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM.Traversal
{
    /// <summary>
    /// Rejects any nodes which do not descend from the specified Node
    /// </summary>
    public class FilterShadowIncludingDescendantOf : NodeFilter
    {
        #region Properties
        public readonly Node targetNode;
        public readonly ENodeFilterResult Affirmative = ENodeFilterResult.FILTER_ACCEPT;
        public readonly ENodeFilterResult Negative = ENodeFilterResult.FILTER_REJECT;
        #endregion

        #region Constructors
        public FilterShadowIncludingDescendantOf(Node targetNode)
        {
            this.targetNode = targetNode;
        }

        public FilterShadowIncludingDescendantOf(Node targetNode, ENodeFilterResult affirmative, ENodeFilterResult negative) : this(targetNode)
        {
            Affirmative = affirmative;
            Negative = negative;
        }
        #endregion

        public override ENodeFilterResult acceptNode(Node node)
        {
            /* omitting any node without a parent */
            if (ReferenceEquals(node.parentNode, null))
                return Negative;

            if (DOMCommon.Is_Shadow_Including_Descendant(node, targetNode))
                return Affirmative;

            return Negative;
        }
    }
}
