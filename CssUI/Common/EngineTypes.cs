using System;

namespace CssUI
{
    /// <summary>
    /// Opaque handle to a font instance managed by an <see cref="IFontEngine"/>.
    /// </summary>
    public readonly struct FontHandle : IEquatable<FontHandle>
    {
        internal readonly int Id;

        internal FontHandle(int id) => Id = id;

        /// <summary>
        /// Returns true if this handle is null/invalid.
        /// </summary>
        public bool IsNull => Id == 0;

        /// <summary>
        /// A null font handle.
        /// </summary>
        public static FontHandle Null => default;

        public bool Equals(FontHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is FontHandle other && Equals(other);
        public override int GetHashCode() => Id;
        public static bool operator ==(FontHandle left, FontHandle right) => left.Equals(right);
        public static bool operator !=(FontHandle left, FontHandle right) => !left.Equals(right);
        public override string ToString() => $"FontHandle({Id})";
    }

    /// <summary>
    /// Opaque handle to a texture managed by an <see cref="ITextureEngine"/>.
    /// </summary>
    public readonly struct TextureHandle : IEquatable<TextureHandle>
    {
        internal readonly int Id;

        internal TextureHandle(int id) => Id = id;

        /// <summary>
        /// Returns true if this handle is null/invalid.
        /// </summary>
        public bool IsNull => Id == 0;

        /// <summary>
        /// A null texture handle.
        /// </summary>
        public static TextureHandle Null => default;

        public bool Equals(TextureHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is TextureHandle other && Equals(other);
        public override int GetHashCode() => Id;
        public static bool operator ==(TextureHandle left, TextureHandle right) => left.Equals(right);
        public static bool operator !=(TextureHandle left, TextureHandle right) => !left.Equals(right);
        public override string ToString() => $"TextureHandle({Id})";
    }

    /// <summary>
    /// A rectangle with position and size for rendering operations.
    /// </summary>
    public readonly struct RenderRect
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float Width { get; init; }
        public float Height { get; init; }

        public RenderRect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float Left => X;
        public float Top => Y;
        public float Right => X + Width;
        public float Bottom => Y + Height;

        public static RenderRect FromLTRB(float left, float top, float right, float bottom)
            => new RenderRect(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// A 2D point for rendering operations.
    /// </summary>
    public readonly struct RenderPoint
    {
        public float X { get; init; }
        public float Y { get; init; }

        public RenderPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Font metrics for CSS unit resolution and text layout.
    /// </summary>
    public readonly struct FontMetrics
    {
        /// <summary>
        /// The font size in pixels (for 'em' unit).
        /// </summary>
        public float EmSize { get; init; }

        /// <summary>
        /// The height of lowercase 'x' (for 'ex' unit).
        /// </summary>
        public float XHeight { get; init; }

        /// <summary>
        /// The height of capital letters.
        /// </summary>
        public float CapHeight { get; init; }

        /// <summary>
        /// The distance from baseline to top of tallest glyph.
        /// </summary>
        public float Ascender { get; init; }

        /// <summary>
        /// The distance from baseline to bottom of lowest glyph (typically negative).
        /// </summary>
        public float Descender { get; init; }

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
    public readonly struct TextMeasurement
    {
        /// <summary>
        /// The total width of the measured text.
        /// </summary>
        public float Width { get; init; }

        /// <summary>
        /// The total height of the measured text.
        /// </summary>
        public float Height { get; init; }

        /// <summary>
        /// The Y offset from top to the text baseline.
        /// </summary>
        public float Baseline { get; init; }
    }

    /// <summary>
    /// Decoded image data from an <see cref="ITextureEngine"/>.
    /// </summary>
    public readonly struct ImageData
    {
        /// <summary>
        /// Image width in pixels.
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// Raw pixel data in RGBA format (4 bytes per pixel).
        /// </summary>
        public byte[] Pixels { get; init; }

        /// <summary>
        /// Number of frames (1 for static images, >1 for animations).
        /// </summary>
        public int FrameCount { get; init; }

        /// <summary>
        /// Per-frame delay in milliseconds for animated images.
        /// Null or empty for static images.
        /// </summary>
        public int[] FrameDelaysMs { get; init; }

        /// <summary>
        /// Returns true if this is an animated image.
        /// </summary>
        public bool IsAnimated => FrameCount > 1;
    }
}
