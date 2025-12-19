using CssUI.DOM;
using System;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// The progress element represents the completion progress of a task.
    /// </summary>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-progress-element
    /// The progress element can be determinate (with value) or indeterminate (without value)
    /// </remarks>
    [MetaElement("progress")]
    public class HTMLProgressElement : HTMLElement, ILableableElement
    {
        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing;
        #endregion

        #region Constructors
        public HTMLProgressElement(Document document) : base(document, "progress")
        {
        }

        public HTMLProgressElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Current progress value (0 to max). If not specified, the progress is indeterminate.
        /// </summary>
        [CEReactions]
        public double? value
        {
            get
            {
                var attr = getAttribute(EAttributeName.Value);
                if (attr == null)
                    return null;
                if (double.TryParse(attr.AsString(), out double result))
                    return result;
                return null;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () =>
            {
                if (value.HasValue)
                    setAttribute(EAttributeName.Value, AttributeValue.From(value.Value));
                else
                    removeAttribute(EAttributeName.Value);
            });
        }

        /// <summary>
        /// Maximum progress value. Defaults to 1.0 if not specified.
        /// </summary>
        [CEReactions]
        public double max
        {
            get
            {
                var attr = getAttribute(EAttributeName.Max);
                if (attr == null) return 1.0;
                if (double.TryParse(attr.AsString(), out double result))
                    return Math.Max(0, result);
                return 1.0;
            }
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Max, AttributeValue.From(Math.Max(0, value))));
        }
        #endregion

        #region IDL Attributes
        /// <summary>
        /// Returns the current progress position, between 0 and 1.
        /// Returns -1 if the progress bar is indeterminate.
        /// </summary>
        public double position
        {
            get
            {
                var currentValue = value;
                if (!currentValue.HasValue)
                    return -1.0; // Indeterminate
                
                return currentValue.Value / max;
            }
        }
        #endregion

        #region ILableableElement
        public HTMLFormElement? form
        {
            get => null;
            set { /* Progress elements don't have a form owner */ }
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
