using System;

namespace CssUI.CSS;

/// <summary>
/// Represents intrinsic size measurements for a single axis.
/// </summary>
/// <remarks>
/// See: https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes
/// </remarks>
public readonly struct IntrinsicAxisSize
{
    /// <summary>
    /// The min-content size: the smallest size the box can take without overflow.
    /// For text, this is typically the width of the longest word.
    /// </summary>
    public readonly double MinContent;

    /// <summary>
    /// The max-content size: the ideal size with no soft wrap opportunities taken.
    /// For text, this is typically the width of all content on a single line.
    /// </summary>
    public readonly double MaxContent;

    public IntrinsicAxisSize(double minContent, double maxContent)
    {
        MinContent = minContent;
        MaxContent = maxContent;
    }

    /// <summary>
    /// Returns a size with both min and max set to the same definite value.
    /// Used for replaced elements with known intrinsic dimensions.
    /// </summary>
    public static IntrinsicAxisSize Definite(double size) => new(size, size);

    /// <summary>
    /// Returns a zero-size intrinsic measurement.
    /// </summary>
    public static IntrinsicAxisSize Zero => new(0, 0);

    /// <summary>
    /// Combine two intrinsic sizes by taking the maximum of each component.
    /// Used when stacking items (e.g., block children's inline sizes).
    /// </summary>
    public static IntrinsicAxisSize Max(IntrinsicAxisSize a, IntrinsicAxisSize b)
        => new(Math.Max(a.MinContent, b.MinContent), Math.Max(a.MaxContent, b.MaxContent));

    /// <summary>
    /// Combine two intrinsic sizes by summing each component.
    /// Used when items flow in sequence (e.g., inline children).
    /// </summary>
    public static IntrinsicAxisSize Sum(IntrinsicAxisSize a, IntrinsicAxisSize b)
        => new(a.MinContent + b.MinContent, a.MaxContent + b.MaxContent);

    public override string ToString() => $"[min:{MinContent:F1}, max:{MaxContent:F1}]";
}

/// <summary>
/// Represents intrinsic size measurements for both axes.
/// </summary>
public readonly struct IntrinsicSize
{
    /// <summary>
    /// Intrinsic size in the inline (horizontal in LTR) direction.
    /// </summary>
    public readonly IntrinsicAxisSize Inline;

    /// <summary>
    /// Intrinsic size in the block (vertical in LTR) direction.
    /// </summary>
    public readonly IntrinsicAxisSize Block;

    public IntrinsicSize(IntrinsicAxisSize inline, IntrinsicAxisSize block)
    {
        Inline = inline;
        Block = block;
    }

    public IntrinsicSize(double minInline, double maxInline, double minBlock, double maxBlock)
    {
        Inline = new IntrinsicAxisSize(minInline, maxInline);
        Block = new IntrinsicAxisSize(minBlock, maxBlock);
    }

    /// <summary>
    /// Returns a size with definite dimensions on both axes.
    /// </summary>
    public static IntrinsicSize Definite(double width, double height)
        => new(IntrinsicAxisSize.Definite(width), IntrinsicAxisSize.Definite(height));

    /// <summary>
    /// Returns a zero-size intrinsic measurement.
    /// </summary>
    public static IntrinsicSize Zero => new(IntrinsicAxisSize.Zero, IntrinsicAxisSize.Zero);

    public override string ToString() => $"Inline:{Inline}, Block:{Block}";
}

/// <summary>
/// Specifies which intrinsic size to use when resolving auto sizes.
/// </summary>
public enum EIntrinsicSizeType
{
    /// <summary>
    /// Use min-content size (smallest without overflow).
    /// </summary>
    MinContent,

    /// <summary>
    /// Use max-content size (ideal size, no wrapping).
    /// </summary>
    MaxContent,

    /// <summary>
    /// Use fit-content size (shrink-to-fit behavior).
    /// Clamps max-content to available space, but not below min-content.
    /// </summary>
    FitContent
}

