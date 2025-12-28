using System;
using System.Diagnostics.Contracts;
using CssUI.CSS.BoxTree;

namespace CssUI.CSS.Formatting;

public class BlockFormattingContext : IFormattingContext
{
    /// <summary>
    /// Performs block layout on the container and its children.
    /// </summary>
    /// <returns>The content dimensions (width, height) of the laid out content.</returns>
    public Rect2f Flow(CssBoxTreeNode Node)
    {
        System.ArgumentNullException.ThrowIfNull(Node);
        Contract.EndContractBlock();

        double maxWidth = 0;
        double totalHeight = 0;

        CssBoxTreeNode Current = Node.firstChild;
        while (Current is not null)
        {
            if (Current.previousSibling is null)
            {
                Current.Position = Point2f.Zero;
            }
            else
            {
                var prev = Current.previousSibling;
                var prev_pos = prev.Position;
                var prev_size = prev.Size;
                Current.Position = new Point2f(prev_pos.X + prev_size.Width, prev_pos.Y + prev_size.Height);
            }

            // Track content dimensions
            var currentSize = Current.Size;
            var currentPos = Current.Position;
            maxWidth = Math.Max(maxWidth, currentPos.X + currentSize.Width);
            totalHeight = Math.Max(totalHeight, currentPos.Y + currentSize.Height);

            Current = Current.nextSibling;
        }

        return new Rect2f(maxWidth, totalHeight);
    }
}

