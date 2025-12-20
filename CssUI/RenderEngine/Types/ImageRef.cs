using System;
using System.Collections.Immutable;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// A reference to an image resource, potentially animated with multiple frames.
/// </summary>
public sealed record class ImageRef : IDisposable, IRenderableResource, ITickable
{
    private bool _disposed;
    private int _currentFrameIndex;
    private double _elapsedTime;

    /// <summary>
    /// The frames comprising this image.
    /// </summary>
    public ImmutableArray<ImageFrameRef> Frames { get; }

    /// <summary>
    /// Total duration of the animation in seconds.
    /// </summary>
    public double TotalDuration { get; }

    /// <summary>
    /// Creates a new image reference with the specified frames.
    /// </summary>
    /// <param name="frames">The frames comprising this image.</param>
    public ImageRef(ImmutableArray<ImageFrameRef> frames)
    {
        Frames = frames;
        TotalDuration = 0;

        foreach (var frame in frames)
        {
            TotalDuration += frame.DurationSeconds;
        }
    }

    /// <summary>
    /// Gets the number of frames in this image.
    /// </summary>
    public int FrameCount => Frames.Length;

    /// <summary>
    /// Returns true if this is an animated image (more than one frame).
    /// </summary>
    public bool IsAnimated => Frames.Length > 1;

    /// <summary>
    /// Returns true if this image reference is empty or invalid.
    /// </summary>
    public bool IsEmpty => Frames.IsDefaultOrEmpty;

    /// <summary>
    /// Gets the current frame index.
    /// </summary>
    public int CurrentFrameIndex => _currentFrameIndex;

    /// <summary>
    /// Gets the current frame to display.
    /// </summary>
    public ImageFrameRef CurrentFrame => Frames.IsDefaultOrEmpty ? null! : Frames[_currentFrameIndex];

    /// <summary>
    /// Gets the width of the first frame, or 0 if empty.
    /// </summary>
    public int Width => Frames.IsDefaultOrEmpty ? 0 : Frames[0].Width;

    /// <summary>
    /// Gets the height of the first frame, or 0 if empty.
    /// </summary>
    public int Height => Frames.IsDefaultOrEmpty ? 0 : Frames[0].Height;

    /// <inheritdoc/>
    public void OnTicked(double deltaTime)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!IsAnimated || TotalDuration <= 0)
            return;

        _elapsedTime += deltaTime;

        // Loop the animation
        while (_elapsedTime >= TotalDuration)
        {
            _elapsedTime -= TotalDuration;
        }

        while (_elapsedTime < 0)
        {
            _elapsedTime += TotalDuration;
        }

        // Find the correct frame for the current time
        double accumulatedTime = 0;
        for (int i = 0; i < Frames.Length; i++)
        {
            accumulatedTime += Frames[i].DurationSeconds;
            if (_elapsedTime < accumulatedTime)
            {
                _currentFrameIndex = i;
                return;
            }
        }

        // Fallback to last frame
        _currentFrameIndex = Frames.Length - 1;
    }

    /// <summary>
    /// Resets the animation to the first frame.
    /// </summary>
    public void Reset()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _currentFrameIndex = 0;
        _elapsedTime = 0;
    }

    /// <inheritdoc/>
    public void Render(IRenderService renderService)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!Frames.IsDefaultOrEmpty)
        {
            CurrentFrame.Render(renderService);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var frame in Frames)
        {
            frame.Dispose();
        }
    }

    #region Factory Methods

    /// <summary>
    /// Creates an ImageRef from decoded image data.
    /// </summary>
    /// <param name="imageData">The decoded image data.</param>
    /// <param name="textureService">The texture service to create GPU textures.</param>
    /// <returns>A new ImageRef, or an empty ImageRef on failure.</returns>
    public static ImageRef FromImageData(ImageData imageData, ITextureService textureService)
    {
        ArgumentNullException.ThrowIfNull(textureService);

        if (imageData.IsEmpty)
        {
            return Empty;
        }

        var builder = ImmutableArray.CreateBuilder<ImageFrameRef>(imageData.FrameCount);

        foreach (var frame in imageData.Frames)
        {
            var descriptor = textureService.CreateTexture(
                frame.Width,
                frame.Height,
                frame.PixelSpan,
                frame.Format);

            if (descriptor.IsNull)
            {
                // Clean up any already-created textures
                foreach (var createdFrame in builder)
                {
                    createdFrame.Dispose();
                }
                return Empty;
            }

            var textureRef = new TextureRef(descriptor, textureService);
            var frameRef = new ImageFrameRef(
                textureRef,
                frame.DelaySeconds,
                new Rect2i(frame.Width, frame.Height));

            builder.Add(frameRef);
        }

        return new ImageRef(builder.MoveToImmutable());
    }

    /// <summary>
    /// Creates an ImageRef from decoded image data asynchronously.
    /// </summary>
    /// <param name="imageData">The decoded image data.</param>
    /// <param name="textureService">The texture service to create GPU textures.</param>
    /// <returns>A new ImageRef, or an empty ImageRef on failure.</returns>
    public static async System.Threading.Tasks.ValueTask<ImageRef> FromImageDataAsync(
        ImageData imageData,
        ITextureService textureService)
    {
        ArgumentNullException.ThrowIfNull(textureService);

        if (imageData.IsEmpty)
        {
            return Empty;
        }

        var builder = ImmutableArray.CreateBuilder<ImageFrameRef>(imageData.FrameCount);

        foreach (var frame in imageData.Frames)
        {
            var descriptor = await textureService.CreateTextureAsync(
                frame.Width,
                frame.Height,
                frame.Pixels,
                frame.Format);

            if (descriptor.IsNull)
            {
                // Clean up any already-created textures
                foreach (var createdFrame in builder)
                {
                    createdFrame.Dispose();
                }
                return Empty;
            }

            var textureRef = new TextureRef(descriptor, textureService);
            var frameRef = new ImageFrameRef(
                textureRef,
                frame.DelaySeconds,
                new Rect2i(frame.Width, frame.Height));

            builder.Add(frameRef);
        }

        return new ImageRef(builder.MoveToImmutable());
    }

    /// <summary>
    /// An empty image reference.
    /// </summary>
    public static ImageRef Empty { get; } = new ImageRef(ImmutableArray<ImageFrameRef>.Empty);

    #endregion
}
