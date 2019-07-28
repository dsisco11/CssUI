using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Skips any nodes that are not of the given type
    /// </summary>
    public class FilterElementType<Ty> : NodeFilter
    {
        //public readonly Type type;

        public FilterElementType()
        {
            //this.type = typeof(Ty);
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            //if (type.IsInstanceOfType(node))
            if (node is Ty)
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }
    }
}
