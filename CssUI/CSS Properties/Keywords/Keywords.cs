
namespace CssUI.Internal
{
    /// <summary>
    /// Provides listing and translation of the CSS specification defined keyword for each property
    /// </summary>
    public static class Keywords
    {
        public static string[] Box_Sizing = new string[] { "content-box", "border-box" };
        public static string[] Border_Style = new string[] { "none","hidden","dotted","dashed","solid","double","groove","ridge","inset","outset" };
        public static string[] MinMaxSize = new string[] { "min-content", "max-content", "fit-content" };
        public static string[] Border_Width = new string[] { "thin", "medium", "thick" };
        public static string[] Direction = new string[] { "ltr", "rtl" };
        public static string[] Writing_Mode = new string[] { "horizontal-tb", "vertical-rl", "vertical-lr", "sideways-rl", "sideways-lr" };
        public static string[] Font_Familys = new string[] { "serif", "sans-serif", "cursive", "fantasy", "monospace" };
        public static string[] Font_Weight = new string[] { "normal", "bold", "bolder", "lighter" };
    }
}
