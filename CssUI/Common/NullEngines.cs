using System;
using System.IO;
using System.Numerics;
using CssUI.CSS;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Null implementation of IFontEngine for headless/testing scenarios.
/// Returns placeholder values without actual font rendering.
/// </summary>
public sealed class NullFontEngine : IFontEngine
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
/// Null implementation of ITextureEngine for headless/testing scenarios.
/// </summary>
public sealed class NullTextureEngine : ITextureEngine
{
    private int _nextId = 1;
    private static readonly string[] _formats = { ".png", ".jpg", ".gif" };
    
    public ReadOnlySpan<string> SupportedFormats => _formats;
    
    public ImageData DecodeImage(ReadOnlySpan<byte> data)
        => new ImageData { Width = 1, Height = 1, Pixels = new byte[4], FrameCount = 1, FrameDelaysMs = Array.Empty<int>() };
    
    public ImageData DecodeImage(Stream stream)
        => DecodeImage(ReadOnlySpan<byte>.Empty);
    
    public TextureHandle CreateTexture(int width, int height, ReadOnlySpan<byte> rgbaPixels)
        => new TextureHandle(_nextId++);
    
    public TextureHandle CreateTextureFromImage(ReadOnlySpan<byte> imageData)
        => new TextureHandle(_nextId++);
    
    public TextureHandle CreateTextureFromImage(Stream stream)
        => new TextureHandle(_nextId++);
    
    public void UpdateTexture(TextureHandle handle, int x, int y, int width, int height, ReadOnlySpan<byte> rgbaPixels) { }
    
    public (int Width, int Height) GetTextureSize(TextureHandle handle)
        => (1, 1);
    
    public bool IsValid(TextureHandle handle)
        => !handle.IsNull;
    
    public void DestroyTexture(TextureHandle handle) { }
}

/// <summary>
/// Null implementation of IRenderEngine for headless/testing scenarios.
/// All drawing operations are no-ops.
/// </summary>
public sealed class NullRenderEngine : IRenderEngine
{
    public void BeginFrame() { }
    public void EndFrame() { }
    public void SetViewportSize(int width, int height) { }
    
    public void PushState() { }
    public void PopState() { }
    public void SetClipRect(RenderRect rect) { }
    public void ClearClipRect() { }
    public void SetTransform(Matrix3x2 transform) { }
    public void ClearTransform() { }
    public void SetOpacity(float opacity) { }
    
    public void FillRect(RenderRect rect, Color color) { }
    public void StrokeRect(RenderRect rect, Color color, float strokeWidth) { }
    public void FillRoundedRect(RenderRect rect, float radiusX, float radiusY, Color color) { }
    public void StrokeRoundedRect(RenderRect rect, float radiusX, float radiusY, Color color, float strokeWidth) { }
    public void DrawLine(RenderPoint start, RenderPoint end, Color color, float strokeWidth) { }
    
    public void DrawTexture(TextureHandle texture, RenderRect destRect) { }
    public void DrawTexture(TextureHandle texture, RenderRect srcRect, RenderRect destRect) { }
    public void DrawTexture(TextureHandle texture, RenderRect destRect, Color tint) { }
    public void DrawTexture(TextureHandle texture, RenderRect srcRect, RenderRect destRect, Color tint) { }
    
    public void DrawText(FontHandle font, ReadOnlySpan<char> text, RenderPoint position, Color color) { }
    public void DrawText(FontHandle font, ReadOnlySpan<char> text, RenderRect bounds, Color color, float horizontalAlign = 0, float verticalAlign = 0) { }
    
    public void DrawBorders(RenderRect rect,
        float topWidth, float rightWidth, float bottomWidth, float leftWidth,
        Color topColor, Color rightColor, Color bottomColor, Color leftColor,
        EBorderStyle topStyle, EBorderStyle rightStyle, EBorderStyle bottomStyle, EBorderStyle leftStyle) { }
}
