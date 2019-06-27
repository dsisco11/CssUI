
namespace CssUI
{
    public class uiHScrollBar : cssScrollBarElement
    {
        public override string TypeName { get { return "HorzScrollBar"; } }

        #region Constructors
        public uiHScrollBar(IParentElement Parent, string className = null, string ID = null) : base(Parent, ESliderDirection.Horizontal, className, ID)
        {
        }
        #endregion
    }
}
