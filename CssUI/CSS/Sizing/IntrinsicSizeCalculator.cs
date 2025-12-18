using System;
using CssUI.CSS.BoxTree;
using CssUI.DOM;

namespace CssUI.CSS;

/// <summary>
/// Calculates intrinsic sizes for CSS boxes.
/// </summary>
/// <remarks>
/// Implements algorithms from CSS Sizing Level 3:
/// https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes
/// </remarks>
public static class IntrinsicSizeCalculator
{
    /// <summary>
    /// Calculate the intrinsic size for a box, dispatching to the appropriate algorithm
    /// based on the box's display type and content.
    /// </summary>
    public static IntrinsicSize Calculate(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        if (box == null)
            return IntrinsicSize.Zero;

        // Check if the box implements IIntrinsicSizable for custom sizing
        if (box is IIntrinsicSizable sizable)
        {
            return sizable.GetIntrinsicSize(context);
        }

        // Dispatch based on display type
        var display = box.Style?.Display ?? EDisplayMode.BLOCK;

        return display switch
        {
            EDisplayMode.BLOCK => CalculateBlockContainerIntrinsicSize(box, context),
            EDisplayMode.INLINE => CalculateInlineBoxIntrinsicSize(box, context),
            EDisplayMode.INLINE_BLOCK => CalculateInlineBlockIntrinsicSize(box, context),
            EDisplayMode.FLEX => CalculateFlexContainerIntrinsicSize(box, context),
            EDisplayMode.GRID => CalculateGridContainerIntrinsicSize(box, context),
            EDisplayMode.NONE => IntrinsicSize.Zero,
            _ => CalculateBlockContainerIntrinsicSize(box, context) // Default to block
        };
    }

    /// <summary>
    /// Calculate intrinsic size for a block container.
    /// </summary>
    /// <remarks>
    /// For block containers:
    /// - Inline size: max of children's inline intrinsic sizes
    /// - Block size: sum of children's block intrinsic sizes (for block flow)
    /// 
    /// See: https://www.w3.org/TR/css-sizing-3/#block-intrinsic
    /// </remarks>
    public static IntrinsicSize CalculateBlockContainerIntrinsicSize(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        var inlineSize = IntrinsicAxisSize.Zero;
        var blockSize = IntrinsicAxisSize.Zero;

        // Iterate through child boxes using tree node traversal
        var child = box.firstChild as CssPrincipalBox;
        if (child == null)
        {
            // Empty container - return zero or any explicit sizing
            return GetExplicitOrZeroSize(box);
        }

        while (child != null)
        {
            // Skip out-of-flow children (absolute, fixed positioned)
            if (!IsOutOfFlow(child))
            {
                var childSize = Calculate(child, context);

                // Add margins to child's intrinsic size
                var childInlineWithMargin = AddInlineMargins(child, childSize.Inline);
                var childBlockWithMargin = AddBlockMargins(child, childSize.Block);

                // For block flow: inline sizes take max, block sizes sum
                inlineSize = IntrinsicAxisSize.Max(inlineSize, childInlineWithMargin);
                blockSize = new IntrinsicAxisSize(
                    blockSize.MinContent + childBlockWithMargin.MinContent,
                    blockSize.MaxContent + childBlockWithMargin.MaxContent);
            }

            child = child.nextSibling as CssPrincipalBox;
        }

        // Add container's own padding and border
        inlineSize = AddInlinePaddingBorder(box, inlineSize);
        blockSize = AddBlockPaddingBorder(box, blockSize);

        return new IntrinsicSize(inlineSize, blockSize);
    }

    /// <summary>
    /// Calculate intrinsic size for inline-level boxes.
    /// </summary>
    public static IntrinsicSize CalculateInlineBoxIntrinsicSize(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        // For inline boxes, we need to consider text content and inline children
        var inlineSize = IntrinsicAxisSize.Zero;
        var blockSize = IntrinsicAxisSize.Zero;

        // Check for text content
        var textSize = CalculateTextIntrinsicSize(box, context);
        if (textSize.Inline.MaxContent > 0)
        {
            inlineSize = textSize.Inline;
            blockSize = textSize.Block;
        }

        // Process inline children using tree node traversal
        var child = box.firstChild as CssPrincipalBox;
        while (child != null)
        {
            if (!IsOutOfFlow(child))
            {
                var childSize = Calculate(child, context);

                // For inline flow: both dimensions generally sum/max depending on wrapping
                // Min-content: can break between inline items
                // Max-content: all items on one line
                if (context.IsMinContent)
                {
                    // Min-content can break, so take max of individual items
                    inlineSize = IntrinsicAxisSize.Max(inlineSize, childSize.Inline);
                }
                else
                {
                    // Max-content sums inline sizes
                    inlineSize = IntrinsicAxisSize.Sum(inlineSize, childSize.Inline);
                }

                blockSize = IntrinsicAxisSize.Max(blockSize, childSize.Block);
            }

            child = child.nextSibling as CssPrincipalBox;
        }

        return new IntrinsicSize(inlineSize, blockSize);
    }

    /// <summary>
    /// Calculate intrinsic size for inline-block boxes.
    /// </summary>
    public static IntrinsicSize CalculateInlineBlockIntrinsicSize(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        // Inline-block is laid out as a block internally but participates in inline flow
        // Calculate as block container, then treat as atomic inline
        return CalculateBlockContainerIntrinsicSize(box, context);
    }

    /// <summary>
    /// Calculate intrinsic size for flex containers.
    /// </summary>
    /// <remarks>
    /// See: https://www.w3.org/TR/css-flexbox-1/#intrinsic-sizes
    /// </remarks>
    public static IntrinsicSize CalculateFlexContainerIntrinsicSize(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        // TODO: Full flex intrinsic sizing algorithm
        // For now, use block container algorithm as approximation
        return CalculateBlockContainerIntrinsicSize(box, context);
    }

    /// <summary>
    /// Calculate intrinsic size for grid containers.
    /// </summary>
    /// <remarks>
    /// See: https://www.w3.org/TR/css-grid-1/#intrinsic-sizes
    /// </remarks>
    public static IntrinsicSize CalculateGridContainerIntrinsicSize(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        // TODO: Full grid intrinsic sizing algorithm
        // For now, use block container algorithm as approximation
        return CalculateBlockContainerIntrinsicSize(box, context);
    }

    /// <summary>
    /// Calculate intrinsic size for text content within a box.
    /// </summary>
    public static IntrinsicSize CalculateTextIntrinsicSize(CssPrincipalBox box, IntrinsicSizeContext context)
    {
        return TextIntrinsicSizer.Calculate(box, context);
    }

    /// <summary>
    /// Calculate intrinsic size for a replaced element (img, video, canvas, etc.).
    /// </summary>
    public static IntrinsicSize CalculateReplacedIntrinsicSize(CssPrincipalBox box)
    {
        var width = box.Intrinsic_Width;
        var height = box.Intrinsic_Height;
        var ratio = box.Intrinsic_Ratio;

        // If we have both dimensions, use them
        if (width.HasValue && height.HasValue)
        {
            return IntrinsicSize.Definite(width.Value, height.Value);
        }

        // If we have one dimension and a ratio, calculate the other
        if (width.HasValue && ratio.HasValue)
        {
            return IntrinsicSize.Definite(width.Value, width.Value / ratio.Value);
        }

        if (height.HasValue && ratio.HasValue)
        {
            return IntrinsicSize.Definite(height.Value * ratio.Value, height.Value);
        }

        // Fallback to default object size (300x150 per spec)
        const double DefaultWidth = 300;
        const double DefaultHeight = 150;

        if (width.HasValue)
            return IntrinsicSize.Definite(width.Value, DefaultHeight);

        if (height.HasValue)
            return IntrinsicSize.Definite(DefaultWidth, height.Value);

        return IntrinsicSize.Definite(DefaultWidth, DefaultHeight);
    }

    #region Helper Methods

    private static bool IsOutOfFlow(CssPrincipalBox box)
    {
        if (box.Style == null) return false;

        var positioning = box.Style.Positioning;
        return positioning == EBoxPositioning.Absolute || positioning == EBoxPositioning.Fixed;
    }

    private static IntrinsicSize GetExplicitOrZeroSize(CssPrincipalBox box)
    {
        // Check for explicit intrinsic dimensions (replaced elements)
        if (box.Intrinsic_Width.HasValue || box.Intrinsic_Height.HasValue)
        {
            return CalculateReplacedIntrinsicSize(box);
        }

        return IntrinsicSize.Zero;
    }

    private static IntrinsicAxisSize AddInlineMargins(CssPrincipalBox box, IntrinsicAxisSize size)
    {
        if (box.Style == null) return size;

        double marginStart = box.Style.Cascaded.Margin_Left?.Actual ?? 0;
        double marginEnd = box.Style.Cascaded.Margin_Right?.Actual ?? 0;

        // Auto margins resolve to 0 for intrinsic sizing
        if (marginStart < 0) marginStart = 0;
        if (marginEnd < 0) marginEnd = 0;

        double totalMargin = marginStart + marginEnd;
        return new IntrinsicAxisSize(size.MinContent + totalMargin, size.MaxContent + totalMargin);
    }

    private static IntrinsicAxisSize AddBlockMargins(CssPrincipalBox box, IntrinsicAxisSize size)
    {
        if (box.Style == null) return size;

        double marginStart = box.Style.Cascaded.Margin_Top?.Actual ?? 0;
        double marginEnd = box.Style.Cascaded.Margin_Bottom?.Actual ?? 0;

        if (marginStart < 0) marginStart = 0;
        if (marginEnd < 0) marginEnd = 0;

        double totalMargin = marginStart + marginEnd;
        return new IntrinsicAxisSize(size.MinContent + totalMargin, size.MaxContent + totalMargin);
    }

    private static IntrinsicAxisSize AddInlinePaddingBorder(CssPrincipalBox box, IntrinsicAxisSize size)
    {
        if (box.Style == null) return size;

        double paddingStart = box.Style.Cascaded.Padding_Left?.Actual ?? 0;
        double paddingEnd = box.Style.Cascaded.Padding_Right?.Actual ?? 0;
        double borderStart = box.Style.Cascaded.Border_Left_Width?.Actual ?? 0;
        double borderEnd = box.Style.Cascaded.Border_Right_Width?.Actual ?? 0;

        double total = paddingStart + paddingEnd + borderStart + borderEnd;
        return new IntrinsicAxisSize(size.MinContent + total, size.MaxContent + total);
    }

    private static IntrinsicAxisSize AddBlockPaddingBorder(CssPrincipalBox box, IntrinsicAxisSize size)
    {
        if (box.Style == null) return size;

        double paddingStart = box.Style.Cascaded.Padding_Top?.Actual ?? 0;
        double paddingEnd = box.Style.Cascaded.Padding_Bottom?.Actual ?? 0;
        double borderStart = box.Style.Cascaded.Border_Top_Width?.Actual ?? 0;
        double borderEnd = box.Style.Cascaded.Border_Bottom_Width?.Actual ?? 0;

        double total = paddingStart + paddingEnd + borderStart + borderEnd;
        return new IntrinsicAxisSize(size.MinContent + total, size.MaxContent + total);
    }

    #endregion

    #region Fit-Content Algorithm

    /// <summary>
    /// Calculate fit-content size given available space.
    /// fit-content = clamp(min-content, max-content, available)
    /// </summary>
    public static double FitContent(IntrinsicAxisSize intrinsic, double available)
    {
        // fit-content formula: max(min-content, min(max-content, available))
        return Math.Max(intrinsic.MinContent, Math.Min(intrinsic.MaxContent, available));
    }

    /// <summary>
    /// Calculate shrink-to-fit width.
    /// </summary>
    /// <remarks>
    /// Shrink-to-fit is essentially fit-content behavior:
    /// min(max(min-content, available), max-content)
    /// 
    /// See: https://www.w3.org/TR/CSS2/visudet.html#shrink-to-fit-float
    /// </remarks>
    public static double ShrinkToFit(CssPrincipalBox box, double availableWidth, IntrinsicSizeContext context)
    {
        var intrinsic = Calculate(box, context);
        return FitContent(intrinsic.Inline, availableWidth);
    }

    #endregion
}

