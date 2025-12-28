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
    /// <returns>
    /// The content dimensions (width, height) of the laid out content.
    /// Used for resolving 'auto' height on the container during bottom-up height resolution.
    /// </returns>
    Rect2f Flow(CssBoxTreeNode node);

    /// <summary>
    /// Performs layout with fragmentation support.
    /// </summary>
    /// <param name="node">The box tree node to layout.</param>
    /// <param name="fragmentationContext">The fragmentation context for break handling.</param>
    /// <returns>
    /// The content dimensions (width, height) of the laid out content.
    /// Used for resolving 'auto' height on the container during bottom-up height resolution.
    /// </returns>
    Rect2f Flow(CssBoxTreeNode node, FragmentationContext fragmentationContext) => Flow(node);
}

