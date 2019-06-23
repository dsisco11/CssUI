
namespace CssUI
{
    public class uiVScrollBar : cssScrollBarElement
    {
        public override string Default_CSS_TypeName { get { return "VertScrollBar"; } }

        #region Constructors
        public uiVScrollBar(string Name = "Y_Scrollbar") : base(Name, ESliderDirection.Vertical)
        {
        }
        #endregion
    }
}
