namespace CssUI.DOM
{
    public sealed class HTMLDocument : Document
    {
        public HTMLDocument(string contentType = null) : base(DocumentType.HTML, contentType)
        {
        }
    }
}
