using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any nodes which do not descend from the specified Node
    /// </summary>
    public class FilterNotOneOf : NodeFilter
    {
        private readonly ReadOnlyMemory<Node> nodeList;
        public FilterNotOneOf(ReadOnlyMemory<Node> nodeList)
        {
            this.nodeList = nodeList;
        }


        public override ENodeFilterResult acceptNode(Node node)
        {
            for (int i=0; i<nodeList.Length; i++)
            {
                if (ReferenceEquals(node, nodeList.Span[i]))
                    return ENodeFilterResult.FILTER_REJECT;
            }

            return ENodeFilterResult.FILTER_ACCEPT;
        }

    }
}
