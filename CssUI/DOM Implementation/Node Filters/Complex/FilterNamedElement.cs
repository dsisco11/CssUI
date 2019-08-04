using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Accepts only HTML elements whose ID attribute matches the given name or whom are in the HTML namespace and whose name attribute matches the given name
    /// </summary>
    public class FilterNamedElement : NodeFilter
    {
        public readonly string Name;

        public FilterNamedElement(string name)
        {
            Name = name;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element element))
                return ENodeFilterResult.FILTER_SKIP;

            // We test reference of string just incase the one being used is like a const or static decleration, or maybe an interned string. it's faster to match an address ptr than to check all chars in an array.
            if (ReferenceEquals(element.id, Name) || StringCommon.StrEq(element.id.AsSpan(), Name.AsSpan()))
                return ENodeFilterResult.FILTER_ACCEPT;

            if (ReferenceEquals(element.NamespaceURI, DOMCommon.HTMLNamespace) || StringCommon.StrEq(element.NamespaceURI.AsSpan(), DOMCommon.HTMLNamespace.AsSpan()))
            {
                var nameAttrValue = element.getAttribute(EAttributeName.Name)?.Get_String();
                if (ReferenceEquals(nameAttrValue, Name) || StringCommon.StrEq(nameAttrValue.AsSpan(), Name.AsSpan()))
                    return ENodeFilterResult.FILTER_ACCEPT;
            }

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
