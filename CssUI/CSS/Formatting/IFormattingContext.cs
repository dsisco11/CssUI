using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting;

/// <summary>
/// Represents a CSS formatting context that performs layout on box tree nodes.
/// </summary>
public interface IFormattingContext
{
    /// <summary>
    /// Performs layout on all elements within the target.
    /// </summary>
    /// <param name="node">The box tree node to layout.</param>
    void Flow(CssBoxTreeNode node);

    /// <summary>
    /// Performs layout with fragmentation support.
    /// </summary>
    /// <param name="node">The box tree node to layout.</param>
    /// <param name="fragmentationContext">The fragmentation context for break handling.</param>
    void Flow(CssBoxTreeNode node, FragmentationContext fragmentationContext) => Flow(node);
}

