using CssUI.Internal;
using System;

namespace CssUI.Enums
{
    [Flags]
    public enum EDisplayMode : int
    {
        /// <summary>This value causes an element to generate no boxes (i.e., the element has no effect on layout). Descendant elements do not generate any boxes either; this behavior cannot be overridden by setting the ‘display’ property on the descendants.</summary>
        [CssKeyword("none")]
        NONE = (1 << 0),
        [CssKeyword("content")]
        CONTENT = (1 << 1),

        /// <summary>Element takes up only as much space as it needs, inline elements cannot specify an explicit width or height, their dimensions are completely determined by their content.</summary>
        [CssKeyword("inline")]
        INLINE = (1 << 2),
        /// <summary></summary>
        [CssKeyword("run-in")]
        RUN_IN = (1 << 3),
        /// <summary>Element takes up an entire line by itself</summary>
        [CssKeyword("block")]
        BLOCK = (1 << 4),


        [CssKeyword("list-item")]
        LIST_ITEM = (1 << 5),


        /// <summary></summary>
        [CssKeyword("flex")]
        FLEX = (1 << 6),
        /// <summary></summary>
        [CssKeyword("flow")]
        FLOW = (1 << 7),
        /// <summary></summary>
        [CssKeyword("flow-root")]
        FLOW_ROOT = (1 << 8),
        /// <summary></summary>
        [CssKeyword("grid")]
        GRID = (1 << 9),
        /*/// <summary></summary>
        [CssKeyword("ruby")]
        RUBY,*/


        /// <summary></summary>
        [CssKeyword("table")]
        TABLE = (1 << 10),
        /// <summary></summary>
        [CssKeyword("table-row-group")]
        TABLE_ROW_GROUP = (1 << 11),
        /// <summary></summary>
        [CssKeyword("table-header-group")]
        TABLE_HEADER_GROUP = (1 << 12),
        /// <summary></summary>
        [CssKeyword("table-footer-group")]
        TABLE_FOOTER_GROUP = (1 << 13),
        /// <summary></summary>
        [CssKeyword("table-row")]
        TABLE_ROW = (1 << 14),
        /// <summary></summary>
        [CssKeyword("table-cell")]
        TABLE_CELL = (1 << 15),
        /// <summary></summary>
        [CssKeyword("table-column-group")]
        TABLE_COLUMN_GROUP = (1 << 16),
        /// <summary></summary>
        [CssKeyword("table-column")]
        TABLE_COLUMN = (1 << 17),
        /// <summary></summary>
        [CssKeyword("table-caption")]
        TABLE_CAPTION = (1 << 18),
        /*/// <summary></summary>
        [CssKeyword("ruby-base")]
        RUBY_BASE,
        /// <summary></summary>
        [CssKeyword("ruby-text")]
        RUBY_TEXT,
        /// <summary></summary>
        [CssKeyword("ruby-base-container")]
        RUBY_BASE_CONTAINER,
        /// <summary></summary>
        [CssKeyword("ruby-text-container")]
        RUBY_TEXT_CONTAINER,
*/


        /// <summary></summary>
        [CssKeyword("inline-block")]
        INLINE_BLOCK = (1 << 19),
        /// <summary></summary>
        [CssKeyword("inline-table")]
        INLINE_TABLE = (1 << 20),
        /// <summary></summary>
        [CssKeyword("inline-flex")]
        INLINE_FLEX = (1 << 21),
        /// <summary></summary>
        [CssKeyword("inline-grid")]
        INLINE_GRID = (1 << 22),

    }
}