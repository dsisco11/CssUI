using CssUI.CSS.Enums;

namespace CssUI.CSS;

/// <summary>
/// Helper structure for identifying and categorizing table-internal display types.
/// Per CSS Display 3 §2.4: Table-internal display types have specific parent requirements.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-display-3/#layout-specific-display
/// Docs: https://www.w3.org/TR/CSS22/tables.html#anonymous-boxes
/// </remarks>
public static class TableInternalDisplayType
{
    #region Display Type Classification

    /// <summary>
    /// Checks if the display type is a table-internal type that requires specific parent boxes.
    /// Per CSS Display 3 §2.4: table-row-group, table-header-group, table-footer-group, 
    /// table-row, table-cell, table-column-group, table-column
    /// </summary>
    public static bool IsTableInternal(EDisplayMode displayMode)
    {
        return displayMode switch
        {
            EDisplayMode.TABLE_ROW_GROUP => true,
            EDisplayMode.TABLE_HEADER_GROUP => true,
            EDisplayMode.TABLE_FOOTER_GROUP => true,
            EDisplayMode.TABLE_ROW => true,
            EDisplayMode.TABLE_CELL => true,
            EDisplayMode.TABLE_COLUMN_GROUP => true,
            EDisplayMode.TABLE_COLUMN => true,
            EDisplayMode.TABLE_CAPTION => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if the display type is a row group box.
    /// Per CSS 2.2 §17.2.1: A row group box is a table-row-group, table-header-group, or table-footer-group.
    /// </summary>
    public static bool IsRowGroupBox(EDisplayMode displayMode)
    {
        return displayMode switch
        {
            EDisplayMode.TABLE_ROW_GROUP => true,
            EDisplayMode.TABLE_HEADER_GROUP => true,
            EDisplayMode.TABLE_FOOTER_GROUP => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if the display type is a proper table child.
    /// Per CSS 2.2 §17.2.1: A proper table child is a table-row box, row group box, 
    /// table-column box, table-column-group box, or table-caption box.
    /// </summary>
    public static bool IsProperTableChild(EDisplayMode displayMode)
    {
        return displayMode switch
        {
            EDisplayMode.TABLE_ROW => true,
            EDisplayMode.TABLE_ROW_GROUP => true,
            EDisplayMode.TABLE_HEADER_GROUP => true,
            EDisplayMode.TABLE_FOOTER_GROUP => true,
            EDisplayMode.TABLE_COLUMN => true,
            EDisplayMode.TABLE_COLUMN_GROUP => true,
            EDisplayMode.TABLE_CAPTION => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if the display type is a proper table row parent.
    /// Per CSS 2.2 §17.2.1: A proper table row parent is a table or inline-table box or row group box.
    /// </summary>
    public static bool IsProperTableRowParent(EDisplayMode displayMode)
    {
        return displayMode switch
        {
            EDisplayMode.TABLE => true,
            EDisplayMode.INLINE_TABLE => true,
            EDisplayMode.TABLE_ROW_GROUP => true,
            EDisplayMode.TABLE_HEADER_GROUP => true,
            EDisplayMode.TABLE_FOOTER_GROUP => true,
            _ => false
        };
    }

    /// <summary>
    /// Checks if the display type is a tabular container (can directly contain table-cells).
    /// Per CSS 2.2 §17.2.1: A tabular container is a table-row box or proper table row parent.
    /// </summary>
    public static bool IsTabularContainer(EDisplayMode displayMode)
    {
        return displayMode == EDisplayMode.TABLE_ROW || IsProperTableRowParent(displayMode);
    }

    /// <summary>
    /// Checks if the display type is a table box (table or inline-table).
    /// </summary>
    public static bool IsTableBox(EDisplayMode displayMode)
    {
        return displayMode == EDisplayMode.TABLE || displayMode == EDisplayMode.INLINE_TABLE;
    }

    #endregion

    #region Parent Validation

    /// <summary>
    /// Returns the required parent display type(s) for a table-internal box.
    /// Per CSS Display 3 §2.4 and CSS 2.2 §17.2.1.
    /// </summary>
    public static EDisplayMode GetRequiredParent(EDisplayMode displayMode)
    {
        return displayMode switch
        {
            // table-cell requires table-row parent
            EDisplayMode.TABLE_CELL => EDisplayMode.TABLE_ROW,
            // table-row requires row group parent (prefer TABLE_ROW_GROUP for anonymous boxes)
            EDisplayMode.TABLE_ROW => EDisplayMode.TABLE_ROW_GROUP,
            // row groups and columns require table parent
            EDisplayMode.TABLE_ROW_GROUP => EDisplayMode.TABLE,
            EDisplayMode.TABLE_HEADER_GROUP => EDisplayMode.TABLE,
            EDisplayMode.TABLE_FOOTER_GROUP => EDisplayMode.TABLE,
            EDisplayMode.TABLE_COLUMN => EDisplayMode.TABLE_COLUMN_GROUP,
            EDisplayMode.TABLE_COLUMN_GROUP => EDisplayMode.TABLE,
            EDisplayMode.TABLE_CAPTION => EDisplayMode.TABLE,
            _ => EDisplayMode.NONE
        };
    }

    /// <summary>
    /// Checks if a parent display type is valid for a given table-internal child display type.
    /// </summary>
    public static bool IsValidParentFor(EDisplayMode parentDisplay, EDisplayMode childDisplay)
    {
        return childDisplay switch
        {
            // table-cell requires table-row parent
            EDisplayMode.TABLE_CELL => parentDisplay == EDisplayMode.TABLE_ROW,

            // table-row requires proper table row parent (row group or table)
            EDisplayMode.TABLE_ROW => IsProperTableRowParent(parentDisplay),

            // row groups require table/inline-table parent
            EDisplayMode.TABLE_ROW_GROUP or
            EDisplayMode.TABLE_HEADER_GROUP or
            EDisplayMode.TABLE_FOOTER_GROUP => IsTableBox(parentDisplay),

            // table-column requires table-column-group or table
            EDisplayMode.TABLE_COLUMN => parentDisplay == EDisplayMode.TABLE_COLUMN_GROUP || IsTableBox(parentDisplay),

            // table-column-group requires table
            EDisplayMode.TABLE_COLUMN_GROUP => IsTableBox(parentDisplay),

            // table-caption requires table
            EDisplayMode.TABLE_CAPTION => IsTableBox(parentDisplay),

            _ => true // Non-table-internal types don't have special requirements
        };
    }

    /// <summary>
    /// Checks if a table-internal child is misparented (requires anonymous wrapper generation).
    /// Per CSS 2.2 §17.2.1: A box is misparented if its parent is not the required type.
    /// </summary>
    public static bool IsMisparented(EDisplayMode parentDisplay, EDisplayMode childDisplay)
    {
        // Only table-internal types can be misparented
        if (!IsTableInternal(childDisplay))
            return false;

        return !IsValidParentFor(parentDisplay, childDisplay);
    }

    #endregion
}
