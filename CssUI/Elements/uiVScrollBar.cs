
using CssUI.DOM;

namespace CssUI
{
    public class uiVScrollBar : cssScrollBarElement
    {
        public static readonly new string CssTagName = "VertScrollBar";

        #region Constructors
        public uiVScrollBar(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, ESliderDirection.Vertical, className, ID)
        {
        }
        #endregion
    }
}
