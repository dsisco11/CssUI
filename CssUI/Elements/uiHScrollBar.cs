using CssUI.DOM;

namespace CssUI
{
    public class uiHScrollBar : cssScrollBarElement
    {
        public static readonly new string CssTagName = "HorzScrollBar";

        #region Constructors
        public uiHScrollBar(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, ESliderDirection.Horizontal, className, ID)
        {
        }
        #endregion
    }
}
