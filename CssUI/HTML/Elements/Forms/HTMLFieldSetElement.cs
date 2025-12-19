using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Collections.Generic;

namespace CssUI.HTML
{
    /// <summary>
    /// The fieldset element represents a set of form controls optionally grouped under a common name.
    /// </summary>
    /// <remarks>
    /// The fieldset element is commonly used to group related form controls together.
    /// A legend element can be used to provide a caption for the fieldset.
    /// </remarks>
    [MetaElement("fieldset")]
    public class HTMLFieldSetElement : FormAssociatedElement, IListedElement, IAutoCapitalizeInheritingElement
    {/* Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-fieldset-element */

        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.SectioningRoot | EContentCategories.Listed | EContentCategories.FormAssociated | EContentCategories.Palpable;
        #endregion

        #region Properties
        /// <summary>
        /// Returns an HTMLCollection of the form controls in the element.
        /// </summary>
        public HTMLCollection<HTMLElement> elements
        {
            get
            {
                // Return a collection of listed elements that are descendants of this fieldset
                return new HTMLCollection<HTMLElement>(this, new FilterIsListed(), DOM.Enums.ENodeFilterMask.SHOW_ELEMENT);
            }
        }
        #endregion

        #region Constructors
        public HTMLFieldSetElement(Document document) : this(document, "fieldset")
        {
        }

        public HTMLFieldSetElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Whether the descendant form controls, except any inside legend, are disabled
        /// </summary>
        [CEReactions]
        public new bool disabled
        {
            get => hasAttribute(EAttributeName.Disabled);
            set
            {
                if (value)
                    CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Disabled, AttributeValue.From(string.Empty)));
                else
                    CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => removeAttribute(EAttributeName.Disabled));
            }
        }

        /// <summary>
        /// The name of the fieldset
        /// </summary>
        [CEReactions]
        public string? name
        {
            get => getAttribute(EAttributeName.Name)?.AsString();
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Returns "fieldset".
        /// </summary>
        public new string type => "fieldset";
        #endregion

        #region Validation
        /// <summary>
        /// Returns true if the element's value has no validity problems; false otherwise.
        /// Fires an invalid event at the element in the latter case.
        /// </summary>
        public override bool checkValidity()
        {
            // Fieldsets are always valid
            return true;
        }

        /// <summary>
        /// Returns true if the element's value has no validity problems; otherwise, returns false, 
        /// fires an invalid event at the element, and (if the event isn't canceled) reports the problem to the user.
        /// </summary>
        public override bool reportValidity()
        {
            // Fieldsets are always valid
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Filter that accepts listed elements
    /// </summary>
    internal class FilterIsListed : NodeFilter
    {
        public override DOM.Enums.ENodeFilterResult acceptNode(Node node)
        {
            if (node is IListedElement)
                return DOM.Enums.ENodeFilterResult.FILTER_ACCEPT;
            return DOM.Enums.ENodeFilterResult.FILTER_SKIP;
        }
    }
}
