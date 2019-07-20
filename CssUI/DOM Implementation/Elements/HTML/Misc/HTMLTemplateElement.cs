namespace CssUI.DOM
{
    public sealed class HTMLTemplateElement : HTMLElement
    {
        #region Properties
        public DocumentFragment content { get; internal set; }
        #endregion

        #region Constructor
        public HTMLTemplateElement(Document document, string localName, string prefix, string Namespace, DocumentFragment content) : base(document, localName)
        {
            this.content = content;
        }
        #endregion

    }
}
