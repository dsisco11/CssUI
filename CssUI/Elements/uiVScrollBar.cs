
namespace CssUI
{
    public class uiVScrollBar : cssScrollBarElement
    {
        public override string Default_CSS_TypeName { get { return "VertScrollBar"; } }

        #region Constructors
        public uiVScrollBar(IParentElement Parent, string Name = "Y_Scrollbar") : base(Parent, Name, ESliderDirection.Vertical)
        {
        }
        #endregion
    }
}
