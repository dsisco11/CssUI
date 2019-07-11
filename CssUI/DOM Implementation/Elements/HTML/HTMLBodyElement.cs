using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class HTMLBodyElement : HTMLElement, IWindowEventHandlers
    {

        #region Constructor
        public HTMLBodyElement(Document document, string localName) : base(document, localName)
        {
            tagName = "body";
        }
        #endregion


    }
}
