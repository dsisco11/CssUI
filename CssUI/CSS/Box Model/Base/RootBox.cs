using CssUI.CSS.Enums;
using CssUI.CSS.Formatting;
using CssUI.DOM;

namespace CssUI.CSS.BoxTree
{
    /// <summary>
    /// A specialized <see cref="CssPrincipalBox"/> for use specifically by the root element.
    /// <para>Provides a block formatting context and other key special values and behaviors</para>
    /// </summary>
    public class RootBox : CssPrincipalBox
    {
        #region Properties

        #region Backing
        private readonly DisplayType _displayType = new DisplayType(EOuterDisplayType.Block, EInnerDisplayType.Flow_Root);
        private readonly IFormattingContext _formattingContext;
        #endregion

        #region Overrides
        public override IFormattingContext FormattingContext
        {
            get
            {
                return _formattingContext;
            }
        }

        public override EDisplayMode Display => EDisplayMode.BLOCK;
        public override DisplayType DisplayType => _displayType;
        #endregion

        #endregion


        #region Constructors
        public RootBox(in Element owner) : base(owner, null)
        {
            _formattingContext = new BlockFormattingContext();
        }

        #endregion

        #region Geometry
        public override Rect4d getBoundingClientRect()
        {
            var viewport = this.ownerElement.nodeDocument.Viewport;
            return new Rect4d(
                Top: (int)viewport.Top,
                Right: (int)(viewport.Left + viewport.Width),
                Bottom: (int)(viewport.Top + viewport.Height),
                Left: (int)viewport.Left
                );
        }
        #endregion
    }
}
