using CssUI.DOM;
using CssUI.DOM.Events;
using System;
using System.Threading.Tasks;

namespace CssUI.HTML
{
    /// <summary>
    /// The details element represents a disclosure widget from which the user can obtain additional information or controls.
    /// </summary>
    [MetaElement("details")]
    public class HTMLDetailsElement : HTMLElement
    {/* Docs: https://html.spec.whatwg.org/multipage/interactive-elements.html#the-details-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.SectioningRoot | EContentCategories.Phrasing | EContentCategories.Palpable;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public HTMLDetailsElement(Document document) : base(document, "details")
        {
        }

        public HTMLDetailsElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        [CEReactions]
        public bool open
        {
            get => hasAttribute(EAttributeName.Open);
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => toggleAttribute(EAttributeName.Open, value));
        }
        #endregion

        #region Overrides
        internal override void run_attribute_change_steps(Element element, AtomicName<EAttributeName> localName, AttributeValue oldValue, AttributeValue newValue, ReadOnlyMemory<char> Namespace)
        {
            base.run_attribute_change_steps(element, localName, oldValue, newValue, Namespace);

            /* Whenever the open attribute is added to or removed from a details element, the user agent must queue a task that runs the following steps, 
             * which are known as the details notification task steps, for this details element: */
            if (localName == EAttributeName.Open)
            {
                Task.Factory.StartNew(() => dispatchEvent(new Event(EEventName.Toggle)));
            }
        }
        #endregion

    }
}
