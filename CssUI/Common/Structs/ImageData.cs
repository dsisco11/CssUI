using System;
using System.Collections.Immutable;

namespace CssUI.Common;

/// <summary>
/// Represents a single frame of image data.
/// </summary>
public readonly record struct ImageFrame
{
    /// <summary>
    /// Frame width in pixels.
    /// </summary>
    public required int Width { get; init; }

    /// <summary>
    /// Frame height in pixels.
    /// </summary>
    public required int Height { get; init; }

    /// <summary>
    /// Raw pixel data for this frame.
    /// </summary>
    public required byte[] Pixels { get; init; }

    /// <summary>
    /// The pixel format of the frame data.
    /// </summary>
    public required EPixelFormat Format { get; init; }

    /// <summary>
    /// Frame delay in seconds for animated images.
    /// </summary>
    public float DelaySeconds { get; init; }

    /// <summary>
    /// Gets the pixel data as a span.
    /// </summary>
    public ReadOnlySpan<byte> PixelSpan => Pixels;
}

/// <summary>
/// Decoded image data from an <see cref="IImageService"/>.
/// </summary>
public readonly record struct ImageData
{
    /// <summary>
    /// The frames comprising this image.
    /// </summary>
    public required ImmutableArray<ImageFrame> Frames { get; init; }

    /// <summary>
    /// Returns the number of frames in this image.
    /// </summary>
    public int FrameCount => Frames.Length;

    /// <summary>
    /// Returns true if this is an animated image.
    /// </summary>
    public bool IsAnimated => Frames.Length > 1;

    /// <summary>
    /// Returns true if this image data is empty or invalid.
    /// </summary>
    public bool IsEmpty => Frames.IsDefaultOrEmpty;

    /// <summary>
    /// Gets the width of the first frame, or 0 if empty.
    /// </summary>
    public int Width => Frames.IsDefaultOrEmpty ? 0 : Frames[0].Width;

    /// <summary>
    /// Gets the height of the first frame, or 0 if empty.
    /// </summary>
    public int Height => Frames.IsDefaultOrEmpty ? 0 : Frames[0].Height;
}

