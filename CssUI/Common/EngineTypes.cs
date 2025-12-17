using System;

namespace CssUI
{
    /// <summary>
    /// Opaque handle to a font instance managed by an <see cref="IFontEngine"/>.
    /// </summary>
    public readonly record struct FontHandle(int Id)
    {
        /// <summary>
        /// Returns true if this handle is null/invalid.
        /// </summary>
        public bool IsNull => Id == 0;

        /// <summary>
        /// A null font handle.
        /// </summary>
        public static FontHandle Null => default;

        public override string ToString() => $"FontHandle({Id})";
    }

    /// <summary>
    /// Opaque handle to a texture managed by an <see cref="ITextureEngine"/>.
    /// </summary>
    public readonly record struct TextureHandle(int Id)
    {
        /// <summary>
        /// Returns true if this handle is null/invalid.
        /// </summary>
        public bool IsNull => Id == 0;

        /// <summary>
        /// A null texture handle.
        /// </summary>
        public static TextureHandle Null => default;

        public override string ToString() => $"TextureHandle({Id})";
    }

    /// <summary>
    /// A rectangle with position and size for rendering operations.
    /// </summary>
    public readonly record struct RenderRect(float X, float Y, float Width, float Height)
    {
        public float Left => X;
        public float Top => Y;
        public float Right => X + Width;
        public float Bottom => Y + Height;

        public static RenderRect FromLTRB(float left, float top, float right, float bottom)
            => new(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// A 2D point for rendering operations.
    /// </summary>
    public readonly record struct RenderPoint(float X, float Y);

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

    /// <summary>
    /// Result of measuring text dimensions.
    /// </summary>
    public readonly record struct TextMeasurement(float Width, float Height, float Baseline);

    /// <summary>
    /// Decoded image data from an <see cref="ITextureEngine"/>.
    /// </summary>
    public readonly record struct ImageData
    {
        /// <summary>
        /// Image width in pixels.
        /// </summary>
        public required int Width { get; init; }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        public required int Height { get; init; }

        /// <summary>
        /// Raw pixel data in RGBA format (4 bytes per pixel).
        /// </summary>
        public required byte[] Pixels { get; init; }

        /// <summary>
        /// Number of frames (1 for static images, >1 for animations).
        /// </summary>
        public int FrameCount { get; init; }

        /// <summary>
        /// Per-frame delay in milliseconds for animated images.
        /// Null or empty for static images.
        /// </summary>
        public int[]? FrameDelaysMs { get; init; }

        /// <summary>
        /// Returns true if this is an animated image.
        /// </summary>
        public bool IsAnimated => FrameCount > 1;
    }
}
