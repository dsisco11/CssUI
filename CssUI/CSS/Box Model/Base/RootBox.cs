using CssUI.CSS.Enums;
using CssUI.CSS.Formatting;
using CssUI.DOM;
using CssUI.DOM.Geometry;

namespace CssUI.CSS.BoxTree;

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

    #region Accessors
    /// <summary>
    /// Gets the formatting context for this root box.
    /// </summary>
    public IFormattingContext RootFormattingContext => _formattingContext;

    /// <summary>
    /// Gets the display mode for the root box.
    /// </summary>
    public EDisplayMode RootDisplay => EDisplayMode.BLOCK;

    /// <summary>
    /// Gets the display type for this root box.
    /// </summary>
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
    /// <summary>
    /// Gets the bounding client rectangle for this root box.
    /// </summary>
    public DOMRect GetBoundingClientRect()
    {
        var viewport = this.Owner.nodeDocument.Viewport;
        return new DOMRect(
            x: viewport.Left,
            y: viewport.Top,
            width: viewport.Width,
            height: viewport.Height
        );
    }
    #endregion
}

