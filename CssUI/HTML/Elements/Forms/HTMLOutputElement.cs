using CssUI.DOM;

namespace CssUI.HTML
{
    /// <summary>
    /// The output element represents the result of a calculation or user action.
    /// </summary>
    /// <remarks>
    /// Docs: https://html.spec.whatwg.org/multipage/form-elements.html#the-output-element
    /// </remarks>
    [MetaElement("output")]
    public class HTMLOutputElement : FormAssociatedElement, IListedElement, IResettableElement
    {
        #region Definition
        public override EContentCategories Categories => EContentCategories.Flow | EContentCategories.Phrasing | EContentCategories.Listed | EContentCategories.FormAssociated;
        #endregion

        #region Internal State
        private string _defaultValue = string.Empty;
        #endregion

        #region Constructors
        public HTMLOutputElement(Document document) : base(document, "output")
        {
        }

        public HTMLOutputElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion

        #region Content Attributes
        /// <summary>
        /// Specifies controls from which the output was calculated
        /// </summary>
        [CEReactions]
        public DOMTokenList htmlFor => new DOMTokenList(this, EAttributeName.For);

        /// <summary>
        /// Name of the form control, used in form submission
        /// </summary>
        [CEReactions]
        public string name
        {
            get => getAttribute(EAttributeName.Name)?.AsString() ?? string.Empty;
            set => CEReactions.Wrap_CEReaction(nodeDocument.defaultView, () => setAttribute(EAttributeName.Name, AttributeValue.From(value)));
        }
        #endregion

        #region IDL Attributes
        /// <summary>
        /// Returns "output"
        /// </summary>
        public string type => "output";

        /// <summary>
        /// Gets or sets the default value
        /// </summary>
        public new string defaultValue
        {
            get => this._defaultValue;
            set
            {
                this._defaultValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the current value
        /// </summary>
        public override string value
        {
            get => textContent ?? string.Empty;
            set => textContent = value;
        }
        #endregion

        #region IResettableElement
        public void Reset()
        {
            textContent = _defaultValue;
        }
        #endregion
    }
}
