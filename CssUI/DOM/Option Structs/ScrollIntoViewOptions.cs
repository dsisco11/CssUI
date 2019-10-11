using CssUI.DOM.Enums;

namespace CssUI.DOM
{
    public class ScrollIntoViewOptions
    {
        #region Properties
        public readonly EScrollLogicalPosition Block;
        public readonly EScrollLogicalPosition Inline;
        #endregion

        #region Constructor
        public ScrollIntoViewOptions(EScrollLogicalPosition block = EScrollLogicalPosition.Center, EScrollLogicalPosition inline = EScrollLogicalPosition.Center)
        {
            Block = block;
            Inline = inline;
        }
        #endregion
    }
}
