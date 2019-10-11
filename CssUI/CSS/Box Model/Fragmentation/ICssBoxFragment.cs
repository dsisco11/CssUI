using CssUI.CSS.BoxTree;

namespace CssUI.CSS
{
    public interface ICssBoxFragment
    {
        CssBoxArea Border { get; }
        CssBoxArea ClickArea { get; }
        CssBoxArea Content { get; }
        CssBoxArea Margin { get; }
        CssBoxArea Padding { get; }
        CssBoxArea Replaced { get; }
        /// <summary>
        /// The box that this one resides within
        /// </summary>
        ICssBoxFragment Parent { get; }
        CssBox Owner { get; }
    }
}