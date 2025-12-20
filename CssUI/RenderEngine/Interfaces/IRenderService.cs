using System.Numerics;
using CssUI.Common;
using CssUI.Rendering;

namespace CssUI;

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
    void SetClipRect(in RenderRect rect);

    /// <summary>
    /// Clear the clip rectangle (disable clipping).
    /// </summary>
    void ClearClipRect();

    /// <summary>
    /// Set the current transform matrix.
    /// </summary>
    void SetTransform(in Matrix3x2 transform);

    /// <summary>
    /// Clear the transform (reset to identity).
    /// </summary>
    void ClearTransform();

    /// <summary>
    /// Set the blend mode for subsequent draw calls.
    /// </summary>
    void SetBlendMode(EBlendMode mode);

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
    void DrawMesh(MeshHandle mesh, EPrimitiveTopology topology);

    #endregion
}
