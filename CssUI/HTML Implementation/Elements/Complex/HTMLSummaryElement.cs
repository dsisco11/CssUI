using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Events;

namespace CssUI.HTML
{
    /// <summary>
    /// The summary element represents a summary, caption, or legend for the rest of the contents of the summary element's parent details element, if any.
    /// </summary>
    [MetaElement("summary")]
    public class HTMLSummaryElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#the-summary-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.None;
        #endregion

        #region Properties
        internal override bool has_activation_behaviour => true;
        #endregion

        #region Constructors
        public HTMLSummaryElement(Document document) : base(document, "summary")
        {
        }

        public HTMLSummaryElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        internal bool is_summary_for_parent_details
        {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#summary-for-its-parent-details */
            get
            {
                var parent = parentElement;
                if (parent == null || !(parent is HTMLDetailsElement parentDetails))
                    return false;

                var firstSummary = DOMCommon.Get_Nth_Descendant_OfType<HTMLSummaryElement>(parentDetails, 1, null, ENodeFilterMask.SHOW_ELEMENT);
                if (!ReferenceEquals(this, firstSummary))
                    return false;

                return true;
            }
        }

        #region Overrides
        internal override void activation_behaviour(Event @event)
        {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#the-summary-element:activation-behaviour */
            base.activation_behaviour(@event);

            if (!is_summary_for_parent_details)
                return;

            var parent = parentElement;
            parent.toggleAttribute(EAttributeName.Open);
        }
        #endregion
    }
}
