using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node whose localName doesnt match
    /// </summary>
    public class FilterLocalName : NodeFilter
    {
        public readonly string Name;

        public FilterLocalName(string localName)
        {
            Name = localName;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element))
                return ENodeFilterResult.FILTER_SKIP;

            // We test reference of string just incase the one being used is like a const or static decleration, or maybe an interned string. it's faster to match an address ptr than to check all chars in an array.
            if (ReferenceEquals((node as Element).localName, Name) || 0 == string.Compare((node as Element).localName, Name))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
