using CssUI.CSS;

namespace CssUI.DOM
{
    public class ScrollToOptions : ScrollOptions
    {
        #region Propreties
        public double? left;
        public double? top;
        #endregion

        #region Constructors
        public ScrollToOptions(EScrollBehavior behavior, double? left = null, double? top = null) : base(behavior)
        {
            this.left = left;
            this.top = top;
        }
        #endregion
    }
}
