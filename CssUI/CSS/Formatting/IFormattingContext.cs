using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting
{
    public interface IFormattingContext
    {
        /// <summary>
        /// Performs layout on all elements within the target
        /// </summary>
        /// <param name="node"></param>
        void Flow(CssBoxTreeNode node);
    }
}
