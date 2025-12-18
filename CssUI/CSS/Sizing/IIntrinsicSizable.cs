using CssUI.CSS.BoxTree;

namespace CssUI.CSS;

/// <summary>
/// Interface for CSS boxes that can calculate their intrinsic sizes.
/// </summary>
/// <remarks>
/// Intrinsic sizes are content-based sizes used for:
/// - Shrink-to-fit width calculations
/// - Flex item sizing (flex-basis: content)
/// - Grid track sizing (min-content, max-content, fit-content)
/// - Table cell sizing
/// 
/// See: https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes
/// </remarks>
public interface IIntrinsicSizable
{
    /// <summary>
    /// Calculate the intrinsic sizes for this box.
    /// </summary>
    /// <param name="context">Context providing constraints and formatting information.</param>
    /// <returns>The calculated intrinsic sizes for both axes.</returns>
    IntrinsicSize GetIntrinsicSize(IntrinsicSizeContext context);

    /// <summary>
    /// Returns true if this element has a definite intrinsic width (e.g., images with natural dimensions).
    /// </summary>
    bool HasIntrinsicWidth { get; }

    /// <summary>
    /// Returns true if this element has a definite intrinsic height (e.g., images with natural dimensions).
    /// </summary>
    bool HasIntrinsicHeight { get; }

    /// <summary>
    /// Returns true if this element has an intrinsic aspect ratio.
    /// </summary>
    bool HasIntrinsicRatio { get; }

    /// <summary>
    /// Gets the intrinsic aspect ratio (width/height) if available.
    /// </summary>
    double? IntrinsicRatio { get; }
}

/// <summary>
/// Context passed during intrinsic size calculation.
/// </summary>
public readonly struct IntrinsicSizeContext
{
    /// <summary>
    /// Available inline size for percentage resolution.
    /// May be null if not yet determined (indefinite).
    /// </summary>
    public readonly double? AvailableInline;

    /// <summary>
    /// Available block size for percentage resolution.
    /// May be null if not yet determined (indefinite).
    /// </summary>
    public readonly double? AvailableBlock;

    /// <summary>
    /// The writing mode affecting inline/block direction mapping.
    /// </summary>
    public readonly EWritingMode WritingMode;

    /// <summary>
    /// The text direction (LTR/RTL).
    /// </summary>
    public readonly EDirection Direction;

    /// <summary>
    /// Whether we're calculating for min-content or max-content.
    /// This affects text wrapping behavior.
    /// </summary>
    public readonly EIntrinsicSizeType SizeType;

    public IntrinsicSizeContext(
        double? availableInline = null,
        double? availableBlock = null,
        EWritingMode writingMode = EWritingMode.Horizontal_TB,
        EDirection direction = EDirection.LTR,
        EIntrinsicSizeType sizeType = EIntrinsicSizeType.MaxContent)
    {
        AvailableInline = availableInline;
        AvailableBlock = availableBlock;
        WritingMode = writingMode;
        Direction = direction;
        SizeType = sizeType;
    }

    /// <summary>
    /// Create a context for min-content calculation.
    /// </summary>
    public IntrinsicSizeContext ForMinContent()
        => new(AvailableInline, AvailableBlock, WritingMode, Direction, EIntrinsicSizeType.MinContent);

    /// <summary>
    /// Create a context for max-content calculation.
    /// </summary>
    public IntrinsicSizeContext ForMaxContent()
        => new(AvailableInline, AvailableBlock, WritingMode, Direction, EIntrinsicSizeType.MaxContent);

    /// <summary>
    /// Create a context with updated available space.
    /// </summary>
    public IntrinsicSizeContext WithAvailableSpace(double? inline, double? block)
        => new(inline, block, WritingMode, Direction, SizeType);

    /// <summary>
    /// Returns true if calculating min-content size.
    /// </summary>
    public bool IsMinContent => SizeType == EIntrinsicSizeType.MinContent;

    /// <summary>
    /// Returns true if calculating max-content size.
    /// </summary>
    public bool IsMaxContent => SizeType == EIntrinsicSizeType.MaxContent;
}

