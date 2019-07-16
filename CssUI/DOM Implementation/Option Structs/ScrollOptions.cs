using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class ScrollOptions
    {
        #region Properties
        EScrollBehavior behavior;
        #endregion

        #region Constructors
        public ScrollOptions(EScrollBehavior behavior)
        {
            this.behavior = behavior;
        }
        #endregion
    }
}
