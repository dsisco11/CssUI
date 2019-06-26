
namespace CssUI
{
    public class uiHScrollBar : cssScrollBarElement
    {
        public override string Default_CSS_TypeName { get { return "HorzScrollBar"; } }

        #region Constructors
        public uiHScrollBar(IParentElement Parent, string Name = "X_Scrollbar") : base(Parent, Name, ESliderDirection.Horizontal)
        {
        }
        #endregion
    }
}
