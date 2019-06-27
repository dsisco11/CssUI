
namespace CssUI
{
    public class uiVScrollBar : cssScrollBarElement
    {
        public override string TypeName { get { return "VertScrollBar"; } }

        #region Constructors
        public uiVScrollBar(IParentElement Parent, string className = null, string ID = null) : base(Parent, ESliderDirection.Vertical, className, ID)
        {
        }
        #endregion
    }
}
