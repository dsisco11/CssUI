
namespace CssUI.Internal
{
    /// <summary>
    /// Provides listing and translation of the CSS specification defined valid keywords for various properties
    /// </summary>
    public static class Keywords
    {
        public static string[] DisplayMode = new string[] { "block", "inline", "run-in", "flow", "flow-root", "table", "flex", "grid", "ruby", "list-item", "table-row-group", "table-header-group", "table-footer-group", "table-row", "table-cell", "table-column-group", "table-column", "table-caption", "ruby-base", "ruby-text", "ruby-base-container", "ruby-text-container", "contents", "none", "inline-block", "inline-table", "inline-flex",  "inline-grid" };
        public static string[] Box_Sizing = new string[] { "content-box", "border-box" };
        public static string[] Border_Style = new string[] { "none","hidden","dotted","dashed","solid","double","groove","ridge","inset","outset" };
        public static string[] MinMaxSize = new string[] { "min-content", "max-content", "fit-content" };
        public static string[] Border_Width = new string[] { "thin", "medium", "thick" };
        public static string[] Direction = new string[] { "ltr", "rtl" };
        public static string[] Writing_Mode = new string[] { "horizontal-tb", "vertical-rl", "vertical-lr", "sideways-rl", "sideways-lr" };
        public static string[] Font_Familys = new string[] { "serif", "sans-serif", "cursive", "fantasy", "monospace" };
        public static string[] Font_Weight = new string[] { "normal", "bold", "bolder", "lighter" };
        public static string[] Text_Align = new string[] { "start", "end", "left", "right", "center", "justify", "match-parent", "justify-all" };
        public static string[] Text_Align_Start = new string[] { "start", "end", "left", "right", "center", "justify", "match-parent" };
        public static string[] Text_Align_Last = new string[] { "start", "end", "left", "right", "center", "justify", "match-parent" };
        public static string[] Text_Justify = new string[] { "auto", "none", "inter-word", "inter-character" };
        public static string[] Overflow = new string[] { "visible", "hidden", "clip", "scroll", "auto" };
    }
}
