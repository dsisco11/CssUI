using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only nodes that are of the given type
    /// </summary>
    public class FilterElementType : NodeFilter
    {
        public readonly Type type;

        public FilterElementType(Type type)
        {
            this.type = type;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node.GetType().IsAssignableFrom(type))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
