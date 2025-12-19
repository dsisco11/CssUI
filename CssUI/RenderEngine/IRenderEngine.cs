using System;
using System.Numerics;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Engine interface for rendering primitives, textures, and text.
/// Implementations handle the actual drawing to a render target.
/// </summary>
public interface IRenderEngine
{
    #region Frame Management

    /// <summary>
    /// Begin a new frame/render pass.
    /// Must be called before any drawing operations.
    /// </summary>
    void BeginFrame();

    /// <summary>
    /// End the current frame/render pass.
    /// Submits all queued drawing operations.
    /// </summary>
    void EndFrame();

    /// <summary>
    /// Set the render target size (typically the window size).
    /// </summary>
    void SetViewportSize(int width, int height);

    #endregion

    #region State Management

    /// <summary>
    /// Push the current render state onto a stack.
    /// </summary>
    void PushState();

    /// <summary>
    /// Pop and restore the previous render state.
    /// </summary>
    void PopState();

    /// <summary>
    /// Set the current clip rectangle. Drawing outside this area is clipped.
    /// </summary>
    void SetClipRect(RenderRect rect);

    /// <summary>
    /// Clear the clip rectangle (disable clipping).
    /// </summary>
    void ClearClipRect();

    /// <summary>
    /// Set the current transform matrix.
    /// </summary>
    void SetTransform(Matrix3x2 transform);

    /// <summary>
    /// Clear the transform (reset to identity).
    /// </summary>
    void ClearTransform();

    /// <summary>
    /// Set the global opacity for subsequent drawing operations.
    /// </summary>
    void SetOpacity(float opacity);

    #endregion

    #region Primitive Drawing

    /// <summary>
    /// Fill a rectangle with a solid color.
    /// </summary>
    void FillRect(RenderRect rect, Color color);

    /// <summary>
    /// Draw a rectangle outline.
    /// </summary>
    void StrokeRect(RenderRect rect, Color color, float strokeWidth);

    /// <summary>
    /// Fill a rounded rectangle with a solid color.
    /// </summary>
    void FillRoundedRect(RenderRect rect, float radiusX, float radiusY, Color color);

    /// <summary>
    /// Draw a rounded rectangle outline.
    /// </summary>
    void StrokeRoundedRect(RenderRect rect, float radiusX, float radiusY, Color color, float strokeWidth);

    /// <summary>
    /// Draw a line between two points.
    /// </summary>
    void DrawLine(RenderPoint start, RenderPoint end, Color color, float strokeWidth);

    #endregion

    #region Texture Drawing

    /// <summary>
    /// Draw a texture filling the destination rectangle.
    /// </summary>
    void DrawTexture(TextureHandle texture, RenderRect destRect);

    /// <summary>
    /// Draw a portion of a texture to a destination rectangle.
    /// </summary>
    void DrawTexture(TextureHandle texture, RenderRect srcRect, RenderRect destRect);

    /// <summary>
    /// Draw a texture with a color tint.
    /// </summary>
    void DrawTexture(TextureHandle texture, RenderRect destRect, Color tint);

    /// <summary>
    /// Draw a portion of a texture with a color tint.
    /// </summary>
    void DrawTexture(TextureHandle texture, RenderRect srcRect, RenderRect destRect, Color tint);

    #endregion

    #region Text Drawing

    /// <summary>
    /// Draw text at a position.
    /// </summary>
    /// <param name="font">Font handle from <see cref="IFontEngine"/>.</param>
    /// <param name="text">Text to draw.</param>
    /// <param name="position">Top-left position for the text.</param>
    /// <param name="color">Text color.</param>
    void DrawText(FontHandle font, ReadOnlySpan<char> text, RenderPoint position, Color color);

    /// <summary>
    /// Draw text within a bounding box with alignment.
    /// </summary>
    /// <param name="font">Font handle from <see cref="IFontEngine"/>.</param>
    /// <param name="text">Text to draw.</param>
    /// <param name="bounds">Bounding rectangle for the text.</param>
    /// <param name="color">Text color.</param>
    /// <param name="horizontalAlign">Horizontal alignment (0=left, 0.5=center, 1=right).</param>
    /// <param name="verticalAlign">Vertical alignment (0=top, 0.5=middle, 1=bottom).</param>
    void DrawText(FontHandle font, ReadOnlySpan<char> text, RenderRect bounds, Color color,
                  float horizontalAlign = 0f, float verticalAlign = 0f);

    #endregion

    #region Border Drawing

    /// <summary>
    /// Draw CSS-style borders with potentially different styles/colors per side.
    /// </summary>
    void DrawBorders(RenderRect rect,
                     float topWidth, float rightWidth, float bottomWidth, float leftWidth,
                     Color topColor, Color rightColor, Color bottomColor, Color leftColor,
                     EBorderStyle topStyle, EBorderStyle rightStyle, EBorderStyle bottomStyle, EBorderStyle leftStyle);

    #endregion
}

/// <summary>
/// Border style values matching CSS border-style.
/// </summary>
public enum EBorderStyle
{
    None,
    Hidden,
    Dotted,
    Dashed,
    Solid,
    Double,
    Groove,
    Ridge,
    Inset,
    Outset
}

