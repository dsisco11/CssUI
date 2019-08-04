using CssUI.DOM.CustomElements;

namespace CssUI.DOM
{
    public class HTMLObjectElement : FormAssociatedElement, IListedElement, ISubmittableElement
    {
        #region Properties
        public readonly Document contentDocument;
        #endregion

        #region Constructors
        public HTMLObjectElement(Document document, Document contentDocument) : this(document, "object", contentDocument)
        {
        }

        public HTMLObjectElement(Document document, string localName, Document contentDocument) : base(document, localName)
        {
            this.contentDocument = contentDocument;
        }
        #endregion

        #region Accessors
        public Window contentWindow => contentDocument.defaultView;
        #endregion

        #region Content Attributes
        [CEReactions] public string data
        {
            get => getAttribute(EAttributeName.Data)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Data, AttributeValue.From_String(value)));
        }

        /// <summary>
        /// A valid MIME-type
        /// </summary>
        [CEReactions] public override string type
        {
            get => getAttribute(EAttributeName.Type)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Type, AttributeValue.From_String(value)));
        }

        [CEReactions] public string name
        {
            get => getAttribute(EAttributeName.Name)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.Name, AttributeValue.From_String(value)));
        }

        [CEReactions] public string useMap
        {
            get => getAttribute(EAttributeName.UseMap)?.Get_String();
            set => CEReactions.Wrap_CEReaction(this, () => setAttribute(EAttributeName.UseMap, AttributeValue.From_String(value)));
        }

        [CEReactions] public int width;
        [CEReactions] public int height;
        #endregion


        public Document getSVGDocument();

    }
}
