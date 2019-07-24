using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node whose NamespaceURI doesnt match
    /// </summary>
    public class FilterNamespace : NodeFilter
    {
        public readonly string Name;

        public FilterNamespace(string tagName)
        {
            Name = tagName;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element))
                return ENodeFilterResult.FILTER_SKIP;

            // We test reference of string just incase the one being used is like a const or static decleration, or maybe an interned string. it's faster to match an address ptr than to check all chars in an array.
            if (ReferenceEquals((node as Element).NamespaceURI, Name) || StringExt.streq((node as Element).NamespaceURI.AsSpan(), Name.AsSpan()))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
