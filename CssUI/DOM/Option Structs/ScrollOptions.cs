using CssUI.CSS;

namespace CssUI.DOM
{
    public class ScrollOptions
    {
        #region Properties
        public EScrollBehavior behavior;
        #endregion

        #region Constructors
        public ScrollOptions(EScrollBehavior behavior)
        {
            this.behavior = behavior;
        }
        #endregion
    }
}
