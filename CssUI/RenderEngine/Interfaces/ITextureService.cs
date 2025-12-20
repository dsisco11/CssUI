using System;
using System.Threading.Tasks;

namespace CssUI;

/// <summary>
/// Service interface for GPU texture management.
/// Implementations handle GPU resource allocation internally.
/// </summary>
public interface ITextureService
{
    #region Synchronous Texture Management

    /// <summary>
    /// Create a texture from raw pixel data.
    /// </summary>
    /// <param name="width">Texture width in pixels.</param>
    /// <param name="height">Texture height in pixels.</param>
    /// <param name="pixels">The pixel data.</param>
    /// <param name="format">The pixel format.</param>
    /// <returns>A descriptor for the created texture, or default on failure.</returns>
    TextureDescriptor CreateTexture(int width, int height, ReadOnlySpan<byte> pixels, EPixelFormat format);

    /// <summary>
    /// Update a region of an existing texture.
    /// </summary>
    /// <param name="handle">The texture to update.</param>
    /// <param name="x">X offset of the region.</param>
    /// <param name="y">Y offset of the region.</param>
    /// <param name="width">Width of the region.</param>
    /// <param name="height">Height of the region.</param>
    /// <param name="pixels">New pixel data for the region.</param>
    /// <param name="format">The pixel format.</param>
    void UpdateTexture(TextureHandle handle, int x, int y, int width, int height, ReadOnlySpan<byte> pixels, EPixelFormat format);

    /// <summary>
    /// Check if a texture handle is still valid.
    /// </summary>
    bool IsValid(TextureHandle handle);

    /// <summary>
    /// Destroy a texture and free associated resources.
    /// </summary>
    void Release(TextureHandle handle);

    #endregion

    #region Asynchronous Texture Management

    /// <summary>
    /// Create a texture from raw pixel data asynchronously.
    /// </summary>
    /// <param name="width">Texture width in pixels.</param>
    /// <param name="height">Texture height in pixels.</param>
    /// <param name="pixels">The pixel data.</param>
    /// <param name="format">The pixel format.</param>
    /// <returns>A descriptor for the created texture, or default on failure.</returns>
    ValueTask<TextureDescriptor> CreateTextureAsync(int width, int height, ReadOnlyMemory<byte> pixels, EPixelFormat format);

    /// <summary>
    /// Update a region of an existing texture asynchronously.
    /// </summary>
    /// <param name="handle">The texture to update.</param>
    /// <param name="x">X offset of the region.</param>
    /// <param name="y">Y offset of the region.</param>
    /// <param name="width">Width of the region.</param>
    /// <param name="height">Height of the region.</param>
    /// <param name="pixels">New pixel data for the region.</param>
    /// <param name="format">The pixel format.</param>
    ValueTask UpdateTextureAsync(TextureHandle handle, int x, int y, int width, int height, ReadOnlyMemory<byte> pixels, EPixelFormat format);

    /// <summary>
    /// Destroy a texture and free associated resources asynchronously.
    /// </summary>
    ValueTask ReleaseAsync(TextureHandle handle);

    #endregion
}
