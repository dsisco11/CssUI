using CssUI.CSS;
using CssUI.DOM.Geometry;

namespace CssUI.DOM
{
    public class ScrollBox
    {
        #region Property
        public EWritingMode Block_Flow_Direction;
        public EDirection Inline_Base_Direction;
        public DOMRect ScrollArea;

        public ScrollBox(EWritingMode block_Flow_Direction, EDirection inline_Base_Direction, DOMRect scrollArea)
        {
            Block_Flow_Direction = block_Flow_Direction;
            Inline_Base_Direction = inline_Base_Direction;
            ScrollArea = scrollArea;
        }
        #endregion
    }
}
