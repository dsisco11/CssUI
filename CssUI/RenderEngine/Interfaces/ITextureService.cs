using System;
using System.IO;
using System.Threading.Tasks;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Service interface for image decoding and texture management.
/// Implementations handle GPU resource allocation internally.
/// </summary>
public interface ITextureService
{
    #region Image Decoding

    /// <summary>
    /// Decode image data (PNG, JPEG, GIF, WebP, etc.) into raw RGBA pixels.
    /// </summary>
    /// <param name="data">Encoded image data.</param>
    /// <returns>Decoded image data, or default if decoding fails.</returns>
    ImageData DecodeImage(ReadOnlySpan<byte> data);

    /// <summary>
    /// Decode image data from a stream.
    /// </summary>
    /// <param name="stream">Stream containing encoded image data.</param>
    /// <returns>Decoded image data, or default if decoding fails.</returns>
    ImageData DecodeImage(Stream stream);

    /// <summary>
    /// Get supported image format extensions (e.g., ".png", ".jpg", ".gif").
    /// </summary>
    ReadOnlySpan<string> SupportedFormats { get; }

    #endregion

    #region Texture Management

    /// <summary>
    /// Create a texture from raw RGBA pixel data.
    /// </summary>
    /// <param name="width">Texture width in pixels.</param>
    /// <param name="height">Texture height in pixels.</param>
    /// <param name="rgbaPixels">Pixel data in RGBA format (4 bytes per pixel).</param>
    /// <returns>A handle to the created texture, or <see cref="TextureHandle.Null"/> on failure.</returns>
    TextureHandle CreateTexture(int width, int height, ReadOnlySpan<byte> rgbaPixels);

    /// <summary>
    /// Create a texture directly from encoded image data.
    /// Equivalent to DecodeImage + CreateTexture but may be more efficient.
    /// </summary>
    /// <param name="imageData">Encoded image data.</param>
    /// <returns>A handle to the created texture, or <see cref="TextureHandle.Null"/> on failure.</returns>
    TextureHandle CreateTextureFromImage(ReadOnlySpan<byte> imageData);

    /// <summary>
    /// Create a texture from a stream containing encoded image data.
    /// </summary>
    /// <param name="stream">Stream containing encoded image data.</param>
    /// <returns>A handle to the created texture, or <see cref="TextureHandle.Null"/> on failure.</returns>
    TextureHandle CreateTextureFromImage(Stream stream);

    /// <summary>
    /// Update a region of an existing texture.
    /// </summary>
    /// <param name="handle">The texture to update.</param>
    /// <param name="x">X offset of the region.</param>
    /// <param name="y">Y offset of the region.</param>
    /// <param name="width">Width of the region.</param>
    /// <param name="height">Height of the region.</param>
    /// <param name="rgbaPixels">New pixel data for the region.</param>
    void UpdateTexture(TextureHandle handle, int x, int y, int width, int height, ReadOnlySpan<byte> rgbaPixels);

    /// <summary>
    /// Get texture dimensions.
    /// </summary>
    /// <param name="handle">The texture handle.</param>
    /// <returns>Width and height in pixels, or (0, 0) if handle is invalid.</returns>
    (int Width, int Height) GetTextureSize(TextureHandle handle);

    /// <summary>
    /// Check if a texture handle is still valid.
    /// </summary>
    bool IsValid(TextureHandle handle);

    /// <summary>
    /// Destroy a texture and free associated resources.
    /// </summary>
    void DestroyTexture(TextureHandle handle);

    #endregion

    #region Async GpuTexture Loading

    /// <summary>
    /// Load image data into a GpuTexture asynchronously.
    /// </summary>
    /// <param name="imageData">Encoded image data.</param>
    /// <returns>A GpuTexture, or an empty texture on failure.</returns>
    Task<GpuTexture> LoadTextureAsync(ReadOnlyMemory<byte> imageData);

    /// <summary>
    /// Load an image file into a GpuTexture asynchronously.
    /// </summary>
    /// <param name="path">Path to the image file.</param>
    /// <returns>A GpuTexture, or an empty texture on failure.</returns>
    Task<GpuTexture> LoadTextureFromFileAsync(string path);

    #endregion
}
