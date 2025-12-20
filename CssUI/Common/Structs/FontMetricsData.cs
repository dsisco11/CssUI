namespace CssUI.Common;

/// <summary>
/// Font metrics data for CSS unit resolution and text layout.
/// Used by <see cref="IFontEngine"/>.
/// </summary>
public readonly record struct FontMetricsData
{
    /// <summary>
    /// The font size in pixels (for 'em' unit).
    /// </summary>
    public required float EmSize { get; init; }

    /// <summary>
    /// The height of lowercase 'x' (for 'ex' unit).
    /// </summary>
    public required float XHeight { get; init; }

    /// <summary>
    /// The height of capital letters.
    /// </summary>
    public float CapHeight { get; init; }

    /// <summary>
    /// The distance from baseline to top of tallest glyph.
    /// </summary>
    public required float Ascender { get; init; }

    /// <summary>
    /// The distance from baseline to bottom of lowest glyph (typically negative).
    /// </summary>
    public required float Descender { get; init; }

    /// <summary>
    /// Additional spacing between lines.
    /// </summary>
    public float LineGap { get; init; }

    /// <summary>
    /// The recommended line height (Ascender - Descender + LineGap).
    /// </summary>
    public float LineHeight => Ascender - Descender + LineGap;
}

