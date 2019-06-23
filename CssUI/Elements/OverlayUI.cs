using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// An CSS element container intended for use in displaying css elements in an isolated container which can be used as an overlay rendered overtop non CSSUI media.
    /// </summary>
    public class OverlayUI : cssRootElement
    {
        public override string Default_CSS_TypeName { get { return "OverlayUI"; } }

        #region Constructors
        public OverlayUI(IRenderEngine Engine) : base(Engine)
        {
            Style.User.Overflow_X.Value = EOverflowMode.Clip;
            Style.User.Overflow_Y.Value = EOverflowMode.Clip;
            Style.User.Width.Set(CSSValue.Pct_OneHundred);// Always match the viewport size
            Style.User.Height.Set(CSSValue.Pct_OneHundred);// Always match the viewport size
        }
        #endregion
    }
}
