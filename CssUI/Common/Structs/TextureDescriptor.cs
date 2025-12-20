namespace CssUI.Common;

/// <summary>
/// Describes a GPU texture created by an <see cref="ITextureService"/>.
/// </summary>
public readonly record struct TextureDescriptor
{
    /// <summary>
    /// The handle to the texture.
    /// </summary>
    public required TextureHandle Handle { get; init; }

    /// <summary>
    /// Texture width in pixels.
    /// </summary>
    public required int Width { get; init; }

    /// <summary>
    /// Texture height in pixels.
    /// </summary>
    public required int Height { get; init; }

    /// <summary>
    /// The pixel format of the texture.
    /// </summary>
    public required EPixelFormat Format { get; init; }

    /// <summary>
    /// Returns true if this descriptor is invalid.
    /// </summary>
    public bool IsNull => Handle.IsNull;
}

