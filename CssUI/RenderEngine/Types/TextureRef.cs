using System;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// A reference to a GPU texture resource that manages its lifecycle.
/// </summary>
/// <param name="Descriptor">The texture descriptor containing handle and metadata.</param>
/// <param name="TextureService">The texture service used to release the texture.</param>
public sealed record class TextureRef(
    TextureDescriptor Descriptor,
    ITextureService TextureService) : IDisposable, IRenderableResource
{
    private bool _disposed;

    /// <summary>
    /// Gets the texture handle.
    /// </summary>
    public TextureHandle Handle => Descriptor.Handle;

    /// <summary>
    /// Gets the texture width in pixels.
    /// </summary>
    public int Width => Descriptor.Width;

    /// <summary>
    /// Gets the texture height in pixels.
    /// </summary>
    public int Height => Descriptor.Height;

    /// <summary>
    /// Gets the pixel format of the texture.
    /// </summary>
    public EPixelFormat Format => Descriptor.Format;

    /// <summary>
    /// Returns true if this texture reference is null or invalid.
    /// </summary>
    public bool IsNull => Descriptor.IsNull;

    /// <inheritdoc/>
    public void Render(IRenderService renderService)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        renderService.SetTexture(Handle);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (!Handle.IsNull)
        {
            TextureService.Release(Handle);
        }
    }
}
