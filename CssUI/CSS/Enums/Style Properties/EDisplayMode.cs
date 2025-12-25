using System;
using EnumRecords;

namespace CssUI.CSS;

[Flags, EnumRecord<KeywordProperties>]
public enum EDisplayMode : int
{
    /// <summary>This value causes an element to generate no boxes (i.e., the element has no effect on layout). Descendant elements do not generate any boxes either; this behavior cannot be overridden by setting the ‘display’ property on the descendants.</summary>
    [EnumRecordProperties("none")]
    NONE = (1 << 0),
    [EnumRecordProperties("content")]
    CONTENT = (1 << 1),

    /// <summary>Element takes up only as much space as it needs, inline elements cannot specify an explicit width or height, their dimensions are completely determined by their content.</summary>
    [EnumRecordProperties("inline")]
    INLINE = (1 << 2),
    /// <summary></summary>
    [EnumRecordProperties("run-in")]
    RUN_IN = (1 << 3),
    /// <summary>Element takes up an entire line by itself</summary>
    [EnumRecordProperties("block")]
    BLOCK = (1 << 4),


    [EnumRecordProperties("list-item")]
    LIST_ITEM = (1 << 5),


    /// <summary></summary>
    [EnumRecordProperties("flex")]
    FLEX = (1 << 6),
    /// <summary></summary>
    [EnumRecordProperties("flow")]
    FLOW = (1 << 7),
    /// <summary></summary>
    [EnumRecordProperties("flow-root")]
    FLOW_ROOT = (1 << 8),
    /// <summary></summary>
    [EnumRecordProperties("grid")]
    GRID = (1 << 9),
    /*/// <summary></summary>
    [MetaKeyword("ruby")]
    RUBY,*/


    /// <summary></summary>
    [EnumRecordProperties("table")]
    TABLE = (1 << 10),
    /// <summary></summary>
    [EnumRecordProperties("table-row-group")]
    TABLE_ROW_GROUP = (1 << 11),
    /// <summary></summary>
    [EnumRecordProperties("table-header-group")]
    TABLE_HEADER_GROUP = (1 << 12),
    /// <summary></summary>
    [EnumRecordProperties("table-footer-group")]
    TABLE_FOOTER_GROUP = (1 << 13),
    /// <summary></summary>
    [EnumRecordProperties("table-row")]
    TABLE_ROW = (1 << 14),
    /// <summary></summary>
    [EnumRecordProperties("table-cell")]
    TABLE_CELL = (1 << 15),
    /// <summary></summary>
    [EnumRecordProperties("table-column-group")]
    TABLE_COLUMN_GROUP = (1 << 16),
    /// <summary></summary>
    [EnumRecordProperties("table-column")]
    TABLE_COLUMN = (1 << 17),
    /// <summary></summary>
    [EnumRecordProperties("table-caption")]
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
    [EnumRecordProperties("inline-block")]
    INLINE_BLOCK = (1 << 19),
    /// <summary></summary>
    [EnumRecordProperties("inline-table")]
    INLINE_TABLE = (1 << 20),
    /// <summary></summary>
    [EnumRecordProperties("inline-flex")]
    INLINE_FLEX = (1 << 21),
    /// <summary></summary>
    [EnumRecordProperties("inline-grid")]
    INLINE_GRID = (1 << 22),

}
