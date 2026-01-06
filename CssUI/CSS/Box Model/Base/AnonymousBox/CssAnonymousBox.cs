namespace CssUI.CSS.BoxTree;

/* Docs: https://www.w3.org/TR/css-display-3/#anonymous */
/// <summary>
/// An anonymous box is a box that is not associated with any element. Anonymous boxes are generated in certain circumstances to fix up the box tree when
/// it requires a particular nested structure that is not provided by the boxes generated from the element tree.
/// <para>(Holds children and its style data is linked to its parent nodes but can have style properties individually overriden)</para>
/// </summary>
public class CssAnonymousBox : CssBox
{
    #region Properties
    /// <summary>
    /// The display mode of this anonymous box, used for table fixup detection.
    /// </summary>
    public EDisplayMode SourceDisplayMode { get; }
    #endregion

    #region Constructors
    public CssAnonymousBox(in CssBoxTreeNode parent, DisplayType displayType) : base(parent)
    {
        this.DisplayType = displayType;
        this.SourceDisplayMode = EDisplayMode.NONE;
    }

    public CssAnonymousBox(in CssBoxTreeNode parent, DisplayType displayType, EDisplayMode sourceDisplayMode) : base(parent)
    {
        this.DisplayType = displayType;
        this.SourceDisplayMode = sourceDisplayMode;
    }
    #endregion

    #region Block/Inline Anonymous Box Factories

    /// <summary>
    /// Creates an anonymous block box.
    /// Per CSS 2.2 §9.2.1.1: Anonymous block boxes wrap inline-level children when
    /// a block container has mixed block and inline-level children.
    /// Uses Flow_Root inner to make it a block container.
    /// </summary>
    public static CssAnonymousBox Create_Block(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(Enums.EOuterDisplayType.Block, Enums.EInnerDisplayType.Flow_Root), EDisplayMode.BLOCK);

    /// <summary>
    /// Creates an anonymous inline box.
    /// Per CSS 2.2 §9.2.1.1: Anonymous inline boxes wrap text that is a direct child
    /// of a block container, before inline formatting context processing.
    /// </summary>
    public static CssAnonymousBox Create_Inline(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(Enums.EOuterDisplayType.Inline, Enums.EInnerDisplayType.Flow), EDisplayMode.INLINE);

    #endregion

    #region Table Anonymous Box Factories
    /*
     * Docs: https://www.w3.org/TR/css-display-3/#layout-specific-display
     * Docs: https://www.w3.org/TR/CSS22/tables.html#anonymous-boxes
     *
     * Per CSS Display 3 §2.4 and CSS 2.2 §17.2.1:
     * Table-internal display types require specific parent boxes.
     * If a table-internal box is misparented, anonymous wrapper boxes are generated.
     */

    /// <summary>
    /// Creates an anonymous table box (display: table).
    /// Per CSS 2.2 §17.2.1: Generated to wrap misparented row groups, captions, column groups.
    /// </summary>
    public static CssAnonymousBox Create_Table(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(Enums.EOuterDisplayType.Block, Enums.EInnerDisplayType.Table), EDisplayMode.TABLE);

    /// <summary>
    /// Creates an anonymous inline-table box (display: inline-table).
    /// Per CSS 2.2 §17.2.1: When parent is inline, anonymous table should be inline-table.
    /// </summary>
    public static CssAnonymousBox Create_InlineTable(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(Enums.EOuterDisplayType.Inline, Enums.EInnerDisplayType.Table), EDisplayMode.INLINE_TABLE);

    /// <summary>
    /// Creates an anonymous table-row-group box.
    /// Per CSS 2.2 §17.2.1: Generated to wrap misparented table-row boxes.
    /// </summary>
    public static CssAnonymousBox Create_TableRowGroup(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(EDisplayMode.TABLE_ROW_GROUP), EDisplayMode.TABLE_ROW_GROUP);

    /// <summary>
    /// Creates an anonymous table-row box.
    /// Per CSS 2.2 §17.2.1: Generated to wrap misparented table-cell boxes.
    /// </summary>
    public static CssAnonymousBox Create_TableRow(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(EDisplayMode.TABLE_ROW), EDisplayMode.TABLE_ROW);

    /// <summary>
    /// Creates an anonymous table-cell box.
    /// Per CSS 2.2 §17.2.1: Generated to wrap non-table content in a table-row.
    /// </summary>
    public static CssAnonymousBox Create_TableCell(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(EDisplayMode.TABLE_CELL), EDisplayMode.TABLE_CELL);

    /// <summary>
    /// Creates an anonymous table-column-group box.
    /// Per CSS 2.2 §17.2.1: Generated to wrap misparented table-column boxes.
    /// </summary>
    public static CssAnonymousBox Create_TableColumnGroup(in CssBoxTreeNode parent)
        => new CssAnonymousBox(parent, new DisplayType(EDisplayMode.TABLE_COLUMN_GROUP), EDisplayMode.TABLE_COLUMN_GROUP);

    #endregion
}

