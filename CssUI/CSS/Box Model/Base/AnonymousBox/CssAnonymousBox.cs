namespace CssUI.CSS.BoxTree;

/* Docs: https://www.w3.org/TR/css-display-3/#anonymous */
/// <summary>
/// An anonymous box is a box that is not associated with any element. Anonymous boxes are generated in certain circumstances to fix up the box tree when 
/// it requires a particular nested structure that is not provided by the boxes generated from the element tree.
/// <para>(Holds children and its style data is linked to its parent nodes but can have style properties individually overriden)</para>
/// </summary>
public class CssAnonymousBox : CssBox
{
    #region Constructor
    public CssAnonymousBox(in CssBoxTreeNode parent, DisplayType displayType) : base(parent)
    {
        this.DisplayType = displayType;
    }
    #endregion

    /// <summary>
    /// Creates an anonymous block box.
    /// Per CSS 2.2 §9.2.1.1: Anonymous block boxes wrap inline-level children when
    /// a block container has mixed block and inline-level children.
    /// Uses Flow_Root inner to make it a block container.
    /// </summary>
    public static CssAnonymousBox Create_Block(in CssBoxTreeNode parent) => new CssAnonymousBox(parent, new DisplayType(Enums.EOuterDisplayType.Block, Enums.EInnerDisplayType.Flow_Root));
    
    /// <summary>
    /// Creates an anonymous inline box.
    /// Per CSS 2.2 §9.2.1.1: Anonymous inline boxes wrap text that is a direct child
    /// of a block container, before inline formatting context processing.
    /// </summary>
    public static CssAnonymousBox Create_Inline(in CssBoxTreeNode parent) => new CssAnonymousBox(parent, new DisplayType(Enums.EOuterDisplayType.Inline, Enums.EInnerDisplayType.Flow));
}

