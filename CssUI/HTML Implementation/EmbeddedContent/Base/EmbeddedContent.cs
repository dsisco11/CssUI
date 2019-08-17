
namespace CssUI.HTML
{
    public abstract class EmbeddedContent
    {/* Docs: https://html.spec.whatwg.org/multipage/dom.html#embedded-content-category */

        #region Accessors
        internal abstract int? Intrinsic_Width { get; }
        internal abstract int? Intrinsic_Height { get; }
        #endregion
    }
}
