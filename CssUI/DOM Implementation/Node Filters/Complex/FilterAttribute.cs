using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node which doesnt have an the specified attribute that matches the given value
    /// </summary>
    public class FilterAttribute : NodeFilter
    {
        public readonly AtomicName<EAttributeName> Name;
        public readonly AttributeValue Value;

        public FilterAttribute(AtomicName<EAttributeName> Name, AttributeValue Value)
        {
            this.Name = Name;
            this.Value = Value;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element element))
                return ENodeFilterResult.FILTER_SKIP;

            Attr attr = element.getAttributeNode(Name);

            /* Skip elements which do not have this attribute */
            if (attr == null)
                return ENodeFilterResult.FILTER_SKIP;

            AttributeValue attrValue = attr.Value;

            if (ReferenceEquals(Value, attrValue))
                return ENodeFilterResult.FILTER_ACCEPT;

            if (Value.Equals(attrValue))
                return ENodeFilterResult.FILTER_ACCEPT;

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
