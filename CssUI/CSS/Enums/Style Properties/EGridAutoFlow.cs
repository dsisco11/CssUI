using CssUI.Internal;
using System;

namespace CssUI.CSS
{
    /// <summary>
    /// Values for the grid-auto-flow property.
    /// Spec: https://www.w3.org/TR/css-grid-1/#propdef-grid-auto-flow
    /// </summary>
    [Flags, MetaEnum]
    public enum EGridAutoFlow
    {
        /// <summary>
        /// Items are placed by filling each row, adding new rows as necessary.
        /// </summary>
        [MetaKeyword("row")]
        Row = 0,

        /// <summary>
        /// Items are placed by filling each column, adding new columns as necessary.
        /// </summary>
        [MetaKeyword("column")]
        Column = 1,

        /// <summary>
        /// Use a "dense" packing algorithm that attempts to fill in holes earlier in the grid.
        /// </summary>
        [MetaKeyword("dense")]
        Dense = 2
    }
}
