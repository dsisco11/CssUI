namespace CssUI.DOM
{
    public class HTMLTableCellHeadElement : HTMLTableCellElement
    {
        #region Properties
        [CEReactions] public string scope
        {
            get => getAttribute(EAttributeName.Scope)?.Get_String();
            set => setAttribute(EAttributeName.Scope, AttributeValue.From_String(value));
        }
        [CEReactions] public string abbr
        {
            get => getAttribute(EAttributeName.Abbr).Get_String();
            set => setAttribute(EAttributeName.Abbr, AttributeValue.From_String(value));
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
