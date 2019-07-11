using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node that isnt slotable
    /// </summary>
    public class FilterTagName : NodeFilter
    {
        public readonly string TagName;

        public FilterTagName(string tagName)
        {
            TagName = tagName;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element))
                return ENodeFilterResult.FILTER_SKIP;

            if (0==string.Compare((node as Element).tagName, TagName))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
