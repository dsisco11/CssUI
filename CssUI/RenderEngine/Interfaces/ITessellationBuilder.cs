using CssUI.Common;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Builder that accumulates vertices and indices during tessellation.
/// </summary>
internal interface ITessellationBuilder
{
    /// <summary>
    /// Gets the current vertex count. Use this to compute indices.
    /// </summary>
    int VertexCount { get; }

    /// <summary>
    /// Gets the current index count.
    /// </summary>
    int IndexCount { get; }

    /// <summary>
    /// Gets the primitive topology for the resulting mesh.
    /// </summary>
    EPrimitiveTopology Topology { get; }

    #region Add Vertex Overloads

    /// <summary>
    /// Adds a vertex with position only.
    /// </summary>
    void AddVertex(float x, float y, float z);

    /// <summary>
    /// Adds a vertex with position and texture coordinates.
    /// </summary>
    void AddVertex(float x, float y, float z, float u, float v);

    /// <summary>
    /// Adds a vertex with position and color.
    /// </summary>
    void AddVertex(float x, float y, float z, Color color);

    /// <summary>
    /// Adds a vertex with position, texture coordinates, and color.
    /// </summary>
    void AddVertex(float x, float y, float z, float u, float v, Color color);

    /// <summary>
    /// Adds a vertex with position and normal.
    /// </summary>
    void AddVertex(float x, float y, float z, float nx, float ny, float nz);

    /// <summary>
    /// Adds a vertex with position, texture coordinates, and normal.
    /// </summary>
    void AddVertex(float x, float y, float z, float u, float v, float nx, float ny, float nz);

    /// <summary>
    /// Adds a vertex with position, color, and normal.
    /// </summary>
    void AddVertex(float x, float y, float z, Color color, float nx, float ny, float nz);

    /// <summary>
    /// Adds a vertex with all attributes: position, texture coordinates, color, and normal.
    /// </summary>
    void AddVertex(float x, float y, float z, float u, float v, Color color, float nx, float ny, float nz);

    #endregion

    #region Add Index

    /// <summary>
    /// Adds a single index.
    /// </summary>
    /// <param name="index">The vertex index.</param>
    void AddIndex(int index);

    /// <summary>
    /// Adds three indices forming a triangle.
    /// </summary>
    void AddTriangle(int i0, int i1, int i2);

    /// <summary>
    /// Adds four indices forming a quad (as two triangles).
    /// </summary>
    /// <remarks>
    /// Indices are added as: (i0, i1, i2), (i0, i2, i3).
    /// </remarks>
    void AddQuad(int i0, int i1, int i2, int i3);

    #endregion

    /// <summary>
    /// Builds the final tessellation result from accumulated data.
    /// </summary>
    /// <returns>The tessellation result containing all vertices and indices.</returns>
    TessellationResult Build();

    /// <summary>
    /// Resets the builder for reuse, clearing all accumulated data.
    /// </summary>
    void Clear();
}
