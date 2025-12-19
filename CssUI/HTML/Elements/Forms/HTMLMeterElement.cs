using CssUI.DOM;
using System;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// The meter element represents a scalar measurement within a known range, or a fractional value.
    /// </summary>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-meter-element
    /// Examples: disk usage, relevance of a query result, or percentage of voting population
    /// </remarks>
    [MetaElement("meter")]
    public class HTMLMeterElement : HTMLElement, ILableableElement
    {
        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing;
        #endregion

        #region Constructors
        public HTMLMeterElement(Document document) : base(document, "meter")
        {
        }

        public HTMLMeterElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Current numeric value
        /// </summary>
        [CEReactions]
        public double value
        {
            get
            {
                var attr = getAttribute(EAttributeName.Value);
                if (attr == null) return 0.0;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return 0.0;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Value, AttributeValue.From(value)));
        }

        /// <summary>
        /// Lower bound of the range
        /// </summary>
        [CEReactions]
        public double min
        {
            get
            {
                var attr = getAttribute(EAttributeName.Min);
                if (attr == null) return 0.0;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return 0.0;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Min, AttributeValue.From(value)));
        }

        /// <summary>
        /// Upper bound of the range
        /// </summary>
        [CEReactions]
        public double max
        {
            get
            {
                var attr = getAttribute(EAttributeName.Max);
                if (attr == null) return 1.0;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return 1.0;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Max, AttributeValue.From(value)));
        }

        /// <summary>
        /// Low boundary of the range
        /// </summary>
        [CEReactions]
        public double low
        {
            get
            {
                var attr = getAttribute("low");
                if (attr == null) return min;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return min;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute("low", AttributeValue.From(value)));
        }

        /// <summary>
        /// High boundary of the range
        /// </summary>
        [CEReactions]
        public double high
        {
            get
            {
                var attr = getAttribute("high");
                if (attr == null) return max;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return max;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute("high", AttributeValue.From(value)));
        }

        /// <summary>
        /// Optimum value in the range
        /// </summary>
        [CEReactions]
        public double optimum
        {
            get
            {
                var attr = getAttribute("optimum");
                if (attr == null) return (min + max) / 2.0;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return (min + max) / 2.0;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute("optimum", AttributeValue.From(value)));
        }
        #endregion

        #region ILableableElement
        public HTMLFormElement? form
        {
            get => null;
            set { /* Meter elements don't have a form owner */ }
        }

        public IReadOnlyCollection<HTMLLabelElement>? labels
        {
            get
            {
                // TODO: Implement label lookup
                return null;
            }
        }
        #endregion
    }
}
