using System.Numerics;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Primitive topology for mesh rendering.
/// </summary>
public enum PrimitiveTopology
{
    /// <summary>Each vertex is a separate point.</summary>
    PointList,
    /// <summary>Each pair of vertices forms a line.</summary>
    LineList,
    /// <summary>Vertices form a connected line strip.</summary>
    LineStrip,
    /// <summary>Each set of 3 vertices forms a triangle.</summary>
    TriangleList,
    /// <summary>Vertices form a connected triangle strip.</summary>
    TriangleStrip,
    /// <summary>Vertices form a triangle fan from the first vertex.</summary>
    TriangleFan
}

/// <summary>
/// Blend mode for rendering operations.
/// </summary>
public enum BlendMode
{
    /// <summary>No blending, source overwrites destination.</summary>
    None,
    /// <summary>Standard alpha blending: src * srcAlpha + dst * (1 - srcAlpha).</summary>
    Alpha,
    /// <summary>Additive blending: src + dst.</summary>
    Additive,
    /// <summary>Multiplicative blending: src * dst.</summary>
    Multiply,
    /// <summary>Pre-multiplied alpha blending.</summary>
    PremultipliedAlpha
}

/// <summary>
/// Service interface for rendering meshes, textures, and text.
/// Implementations handle the actual drawing to a render target.
/// </summary>
public interface IRenderService
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

    /// <summary>
    /// Clear the render target with a color.
    /// </summary>
    void Clear(Color color);

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
    /// Set the blend mode for subsequent draw calls.
    /// </summary>
    void SetBlendMode(BlendMode mode);

    /// <summary>
    /// Set the color for subsequent draw calls.
    /// </summary>
    void SetColor(Color color);

    /// <summary>
    /// Set the texture for subsequent draw calls.
    /// Pass <see cref="TextureHandle.Null"/> to disable texturing.
    /// </summary>
    void SetTexture(TextureHandle texture);

    #endregion

    #region Mesh Rendering

    /// <summary>
    /// Draw a mesh with the current color and texture state.
    /// </summary>
    /// <param name="mesh">The mesh to render.</param>
    /// <param name="topology">How to interpret the vertices.</param>
    void DrawMesh(MeshHandle mesh, PrimitiveTopology topology);

    #endregion
}
