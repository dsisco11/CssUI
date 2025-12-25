using System;
using EnumRecords;

namespace CssUI.CSS;

[Flags, EnumRecord<KeywordProperties>]
public enum EDisplayMode : int
{
    /// <summary>This value causes an element to generate no boxes (i.e., the element has no effect on layout). Descendant elements do not generate any boxes either; this behavior cannot be overridden by setting the ‘display’ property on the descendants.</summary>
    [EnumData("none")]
    NONE = (1 << 0),
    [EnumData("content")]
    CONTENT = (1 << 1),

    /// <summary>Element takes up only as much space as it needs, inline elements cannot specify an explicit width or height, their dimensions are completely determined by their content.</summary>
    [EnumData("inline")]
    INLINE = (1 << 2),
    /// <summary></summary>
    [EnumData("run-in")]
    RUN_IN = (1 << 3),
    /// <summary>Element takes up an entire line by itself</summary>
    [EnumData("block")]
    BLOCK = (1 << 4),


    [EnumData("list-item")]
    LIST_ITEM = (1 << 5),


    /// <summary></summary>
    [EnumData("flex")]
    FLEX = (1 << 6),
    /// <summary></summary>
    [EnumData("flow")]
    FLOW = (1 << 7),
    /// <summary></summary>
    [EnumData("flow-root")]
    FLOW_ROOT = (1 << 8),
    /// <summary></summary>
    [EnumData("grid")]
    GRID = (1 << 9),
    /*/// <summary></summary>
    [MetaKeyword("ruby")]
    RUBY,*/


    /// <summary></summary>
    [EnumData("table")]
    TABLE = (1 << 10),
    /// <summary></summary>
    [EnumData("table-row-group")]
    TABLE_ROW_GROUP = (1 << 11),
    /// <summary></summary>
    [EnumData("table-header-group")]
    TABLE_HEADER_GROUP = (1 << 12),
    /// <summary></summary>
    [EnumData("table-footer-group")]
    TABLE_FOOTER_GROUP = (1 << 13),
    /// <summary></summary>
    [EnumData("table-row")]
    TABLE_ROW = (1 << 14),
    /// <summary></summary>
    [EnumData("table-cell")]
    TABLE_CELL = (1 << 15),
    /// <summary></summary>
    [EnumData("table-column-group")]
    TABLE_COLUMN_GROUP = (1 << 16),
    /// <summary></summary>
    [EnumData("table-column")]
    TABLE_COLUMN = (1 << 17),
    /// <summary></summary>
    [EnumData("table-caption")]
    TABLE_CAPTION = (1 << 18),
    /*/// <summary></summary>
    [MetaKeyword("ruby-base")]
    RUBY_BASE,
    /// <summary></summary>
    [MetaKeyword("ruby-text")]
    RUBY_TEXT,
    /// <summary></summary>
    [MetaKeyword("ruby-base-container")]
    RUBY_BASE_CONTAINER,
    /// <summary></summary>
    [MetaKeyword("ruby-text-container")]
    RUBY_TEXT_CONTAINER,
*/


    /// <summary></summary>
    [EnumData("inline-block")]
    INLINE_BLOCK = (1 << 19),
    /// <summary></summary>
    [EnumData("inline-table")]
    INLINE_TABLE = (1 << 20),
    /// <summary></summary>
    [EnumData("inline-flex")]
    INLINE_FLEX = (1 << 21),
    /// <summary></summary>
    [EnumData("inline-grid")]
    INLINE_GRID = (1 << 22),

}
