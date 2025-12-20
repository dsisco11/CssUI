using System;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// A reference to a single frame of an image, with duration for animation support.
/// </summary>
/// <param name="Texture">The texture reference for this frame.</param>
/// <param name="DurationSeconds">The duration this frame should be displayed, in seconds.</param>
/// <param name="Bounds">The bounds/dimensions of this frame.</param>
public sealed record class ImageFrameRef(
    TextureRef Texture,
    float DurationSeconds,
    Rect2i Bounds) : IDisposable, IRenderableResource
{
    private bool _disposed;

    /// <summary>
    /// Gets the texture handle for this frame.
    /// </summary>
    public TextureHandle Handle => Texture.Handle;

    /// <summary>
    /// Gets the width of this frame in pixels.
    /// </summary>
    public int Width => Bounds.Width;

    /// <summary>
    /// Gets the height of this frame in pixels.
    /// </summary>
    public int Height => Bounds.Height;

    /// <summary>
    /// Returns true if this frame reference is null or invalid.
    /// </summary>
    public bool IsNull => Texture.IsNull;

    /// <inheritdoc/>
    public void Render(IRenderService renderService)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        Texture.Render(renderService);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        Texture.Dispose();
    }
}
