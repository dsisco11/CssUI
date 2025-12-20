using System;
using System.Collections.Immutable;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using CssUI.Common;
using CssUI.CSS;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Null implementation of IFontService for headless/testing scenarios.
/// Returns placeholder values without actual font rendering.
/// </summary>
public sealed class NullFontService : IFontService
{
    private const float DEFAULT_EM_SIZE = 16f;
    private const float DEFAULT_CHAR_WIDTH = 8f;

    public FontHandle ResolveFont(ReadOnlySpan<string> familyNames, float size, EFontWeight weight, EFontStyle style)
        => new FontHandle(1); // Return a valid-looking handle

    public FontHandle ResolveFont(EGenericFontFamily genericFamily, float size, EFontWeight weight, EFontStyle style)
        => new FontHandle(1);

    public FontHandle GetDefaultFont(float size)
        => new FontHandle(1);

    public FontMetricsData GetMetrics(FontHandle font)
        => new FontMetricsData
        {
            EmSize = DEFAULT_EM_SIZE,
            XHeight = DEFAULT_EM_SIZE * 0.5f,
            CapHeight = DEFAULT_EM_SIZE * 0.7f,
            Ascender = DEFAULT_EM_SIZE * 0.8f,
            Descender = -DEFAULT_EM_SIZE * 0.2f,
            LineGap = DEFAULT_EM_SIZE * 0.1f
        };

    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text)
        => new TextMeasurement
        {
            Width = text.Length * DEFAULT_CHAR_WIDTH,
            Height = DEFAULT_EM_SIZE,
            Baseline = DEFAULT_EM_SIZE * 0.8f
        };

    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text, float maxWidth)
    {
        var singleLine = MeasureText(font, text);
        if (singleLine.Width <= maxWidth)
            return singleLine;

        // Simple wrapping estimate
        int charsPerLine = Math.Max(1, (int)(maxWidth / DEFAULT_CHAR_WIDTH));
        int lines = (text.Length + charsPerLine - 1) / charsPerLine;

        return new TextMeasurement
        {
            Width = Math.Min(singleLine.Width, maxWidth),
            Height = lines * DEFAULT_EM_SIZE,
            Baseline = DEFAULT_EM_SIZE * 0.8f
        };
    }

    public float GetCharAdvance(FontHandle font, char c)
        => DEFAULT_CHAR_WIDTH;

    public bool IsValid(FontHandle font)
        => !font.IsNull;

    public void Release(FontHandle font) { }
}

/// <summary>
/// Null implementation of IImageService for headless/testing scenarios.
/// </summary>
public sealed class NullImageService : IImageService
{
    private static readonly ImmutableArray<string> _formats = [".png", ".jpg", ".jpeg", ".gif", ".webp"];

    public ImmutableArray<string> SupportedFormats => _formats;

    public ImageData DecodeImage(ReadOnlySpan<byte> data)
        => new ImageData
        {
            Frames = [new ImageFrame
            {
                Width = 1,
                Height = 1,
                Pixels = new byte[4],
                Format = EPixelFormat.RGBA8,
                DelaySeconds = 0
            }]
        };

    public ImageData DecodeImage(Stream stream)
        => DecodeImage(ReadOnlySpan<byte>.Empty);

    public ValueTask<ImageData> DecodeImageAsync(ReadOnlyMemory<byte> data)
        => ValueTask.FromResult(DecodeImage(data.Span));

    public ValueTask<ImageData> DecodeImageAsync(Stream stream)
        => ValueTask.FromResult(DecodeImage(stream));
}

/// <summary>
/// Null implementation of ITextureService for headless/testing scenarios.
/// </summary>
public sealed class NullTextureService : ITextureService
{
    private int _nextId = 1;

    public TextureDescriptor CreateTexture(int width, int height, ReadOnlySpan<byte> pixels, EPixelFormat format)
        => new TextureDescriptor
        {
            Handle = new TextureHandle(_nextId++),
            Width = width,
            Height = height,
            Format = format
        };

    public void UpdateTexture(TextureHandle handle, int x, int y, int width, int height, ReadOnlySpan<byte> pixels, EPixelFormat format) { }

    public bool IsValid(TextureHandle handle)
        => !handle.IsNull;

    public void Release(TextureHandle handle) { }

    public ValueTask<TextureDescriptor> CreateTextureAsync(int width, int height, ReadOnlyMemory<byte> pixels, EPixelFormat format)
        => ValueTask.FromResult(CreateTexture(width, height, pixels.Span, format));

    public ValueTask UpdateTextureAsync(TextureHandle handle, int x, int y, int width, int height, ReadOnlyMemory<byte> pixels, EPixelFormat format)
    {
        UpdateTexture(handle, x, y, width, height, pixels.Span, format);
        return ValueTask.CompletedTask;
    }

    public ValueTask ReleaseAsync(TextureHandle handle)
    {
        Release(handle);
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// Null implementation of IRenderService for headless/testing scenarios.
/// All drawing operations are no-ops.
/// </summary>
public sealed class NullRenderService : IRenderService
{
    public void BeginFrame() { }
    public void EndFrame() { }
    public void SetViewportSize(int width, int height) { }
    public void Clear(Color color) { }

    public void PushState() { }
    public void PopState() { }
    public void SetClipRect(in RenderRect rect) { }
    public void ClearClipRect() { }
    public void SetTransform(in Matrix3x2 transform) { }
    public void ClearTransform() { }
    public void SetBlendMode(EBlendMode mode) { }
    public void SetColor(Color color) { }
    public void SetTexture(TextureHandle texture) { }

    public void DrawMesh(MeshHandle mesh, EPrimitiveTopology topology) { }
}

/// <summary>
/// Null implementation of IMeshService for headless/testing scenarios.
/// </summary>
public sealed class NullMeshService : IMeshService
{
    private int _nextId = 1;

    public ValueTask<MeshHandle> CreateMesh(ReadOnlyMemory<byte> vertices, VertexLayout layout)
        => ValueTask.FromResult(new MeshHandle(_nextId++));

    public ValueTask<MeshHandle> CreateMesh(ReadOnlyMemory<byte> vertices, VertexLayout layout, ReadOnlyMemory<byte> indices, EIndexFormat indexFormat)
        => ValueTask.FromResult(new MeshHandle(_nextId++));

    public ValueTask UpdateVertices(MeshHandle handle, ReadOnlyMemory<byte> vertices)
        => ValueTask.CompletedTask;

    public ValueTask UpdateVertices(MeshHandle handle, int byteOffset, ReadOnlyMemory<byte> vertices)
        => ValueTask.CompletedTask;

    public ValueTask UpdateIndices(MeshHandle handle, ReadOnlyMemory<byte> indices)
        => ValueTask.CompletedTask;

    public MeshInfo GetMeshInfo(MeshHandle handle)
        => default;

    public bool IsValid(MeshHandle handle)
        => !handle.IsNull;

    public void Release(MeshHandle handle) { }
}
