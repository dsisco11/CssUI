namespace CssUI.DOM
{
    public class HTMLTableCellHeadElement : HTMLTableCellElement
    {
        #region Properties
        [CEReactions] public string scope
        {
            get => getAttribute(EAttributeName.Scope);
            set => setAttribute(EAttributeName.Scope, value);
        }
        [CEReactions] public string abbr
        {
            get => getAttribute(EAttributeName.Abbr);
            set => setAttribute(EAttributeName.Abbr, value);
        }
        #endregion

        #region Constructors
        public HTMLTableCellHeadElement(Document document) : base(document, "th")
        {
        }

        public HTMLTableCellHeadElement(Document document, string localName) : base(document, localName)
        {
        }
        #endregion
    }
}
