using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node whose localName doesnt match
    /// </summary>
    public class FilterLocalName_Namespace : NodeFilter
    {
        public readonly string Name;
        public readonly string NamespaceURI;

        public FilterLocalName_Namespace(string localName, string NamespaceURI)
        {
            Name = localName;
            this.NamespaceURI = NamespaceURI;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element element))
                return ENodeFilterResult.FILTER_SKIP;

            // We test references of strings just incase the one being used is like a const or static decleration, or maybe an interned string. it's faster to match an address ptr than to check all chars in an array.

            if (ReferenceEquals(element.NamespaceURI, NamespaceURI) || StringCommon.streq(element.NamespaceURI.AsSpan(), NamespaceURI.AsSpan()))
            {
                if (ReferenceEquals(element.localName, Name) || StringCommon.streq(element.localName.AsSpan(), Name.AsSpan()))
                {
                    return ENodeFilterResult.FILTER_ACCEPT;
                }
            }

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
