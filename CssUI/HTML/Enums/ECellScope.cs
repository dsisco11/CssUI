using EnumRecords;

namespace CssUI.HTML
{
    [EnumRecord<KeywordProperties>]
    public enum ECellScope : int
    {
        /// <summary>
        /// The auto state makes the header cell apply to a set of cells selected based on context.
        /// </summary>
        [EnumData("auto")]
        Auto,

        /// <summary>
        /// The row state means the header cell applies to some of the subsequent cells in the same row(s).
        /// </summary>
        [EnumData("row")]
        Row,

        /// <summary>
        /// The column state means the header cell applies to some of the subsequent cells in the same column(s).
        /// </summary>
        [EnumData("col")]
        Column,

        /// <summary>
        /// The row group state means the header cell applies to all the remaining cells in the row group. A th element's scope attribute must not be in the row group state if the element is not anchored in a row group.
        /// </summary>
        [EnumData("rowgroup")]
        RowGroup,

        /// <summary>
        /// The column group state means the header cell applies to all the remaining cells in the column group. A th element's scope attribute must not be in the column group state if the element is not anchored in a column group.
        /// </summary>
        [EnumData("colgroup")]
        ColumnGroup,
    }
}

