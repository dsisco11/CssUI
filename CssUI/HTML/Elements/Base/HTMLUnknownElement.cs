using CssUI.DOM;

namespace CssUI.HTML
{
    public sealed class HTMLUnknownElement : HTMLElement
    {
        public override EContentCategories Categories => EContentCategories.None;

        #region Constructors
        public HTMLUnknownElement(Document document, string localName, string prefix, string Namespace) : base(document, localName)
        {
        }
        #endregion
    }
}
