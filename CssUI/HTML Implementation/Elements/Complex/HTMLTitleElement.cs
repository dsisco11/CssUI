namespace CssUI.DOM
{
    public sealed class HTMLTitleElement : HTMLElement
    {
        public string Text { get; set; }

        public HTMLTitleElement(Document document, string localName, string prefix, string Namespace) : base(document, localName)
        {
        }
    }
}
