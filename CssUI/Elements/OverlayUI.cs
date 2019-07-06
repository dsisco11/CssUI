using CssUI.CSS;
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// An CSS element container intended for use in displaying css elements in an isolated container which can be used as an overlay rendered overtop non CSSUI media.
    /// </summary>
    public class OverlayUI : cssRootElement
    {
        public override string TypeName { get { return "OverlayUI"; } }

        #region Constructors
        public OverlayUI(IRenderEngine Engine) : base(Engine)
        {
            Style.ImplicitRules.Overflow_X.Set(EOverflowMode.Clip);
            Style.ImplicitRules.Overflow_Y.Set(EOverflowMode.Clip);
            Style.ImplicitRules.Width.Set(CssValue.Pct_OneHundred);// Always match the viewport size
            Style.ImplicitRules.Height.Set(CssValue.Pct_OneHundred);// Always match the viewport size
        }
        #endregion
    }
}
