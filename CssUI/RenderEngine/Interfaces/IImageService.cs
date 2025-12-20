using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;

namespace CssUI;

/// <summary>
/// Service interface for decoding image data into raw pixels.
/// </summary>
public interface IImageService
{
    #region Synchronous Decoding

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

    #endregion

    #region Asynchronous Decoding

    /// <summary>
    /// Decode image data asynchronously.
    /// </summary>
    /// <param name="data">Encoded image data.</param>
    /// <returns>Decoded image data, or default if decoding fails.</returns>
    ValueTask<ImageData> DecodeImageAsync(ReadOnlyMemory<byte> data);

    /// <summary>
    /// Decode image data from a stream asynchronously.
    /// </summary>
    /// <param name="stream">Stream containing encoded image data.</param>
    /// <returns>Decoded image data, or default if decoding fails.</returns>
    ValueTask<ImageData> DecodeImageAsync(Stream stream);

    #endregion

    #region Format Support

    /// <summary>
    /// Get supported image format extensions (e.g., ".png", ".jpg", ".gif").
    /// </summary>
    ImmutableArray<string> SupportedFormats { get; }

    #endregion
}
