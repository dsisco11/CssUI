using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;

namespace CssUI.DOM
{
    /// <summary>
    /// Rejects any node which doesnt have an the specified attribute that matches the given value
    /// </summary>
    public class FilterAttribute : NodeFilter
    {
        #region Properties
        /// <summary>
        /// Name of the attribute to check
        /// </summary>
        public readonly AtomicName<EAttributeName> Name;
        /// <summary>
        /// Value to check the attribute for
        /// </summary>
        public readonly AttributeValue Value;
        /// <summary>
        /// Whether the attribute should be checked for a boolean status
        /// </summary>
        public readonly bool IsBoolean = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new attribute filter that matches a BOOLEAN attribute
        /// </summary>
        /// <param name="Name">Athe attribute to check</param>
        public FilterAttribute(AtomicName<EAttributeName> Name)
        {
            this.Name = Name;
            IsBoolean = true;
        }

        /// <summary>
        /// Creates a new attribute filter that matches a BOOLEAN attribute
        /// </summary>
        /// <param name="Name">Athe attribute to check</param>
        public FilterAttribute(AtomicName<EAttributeName> Name, AttributeValue Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        #endregion

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (!(node is Element element))
                return ENodeFilterResult.FILTER_SKIP;

            //Attr attr = element.getAttributeNode(Name);

            /* Skip elements which do not have this attribute */
            if (!element.hasAttribute(Name, out Attr attr))
            {
                return ENodeFilterResult.FILTER_SKIP;
            }

            if (IsBoolean)
            {
                return ENodeFilterResult.FILTER_ACCEPT;
            }

            AttributeValue attrValue = attr.Value;

            if (ReferenceEquals(Value, attrValue))
            {
                return ENodeFilterResult.FILTER_ACCEPT;
            }

            if (Value.Equals(attrValue))
            {
                return ENodeFilterResult.FILTER_ACCEPT;
            }

            return ENodeFilterResult.FILTER_SKIP;
        }


    }
}
