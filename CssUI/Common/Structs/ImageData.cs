namespace CssUI;

/// <summary>
/// Decoded image data from an <see cref="IImageService"/>.
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

