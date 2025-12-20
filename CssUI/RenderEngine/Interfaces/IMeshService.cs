using System;
using System.Threading.Tasks;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// Service interface for mesh/geometry buffer management.
/// Implementations handle GPU resource allocation internally.
/// </summary>
public interface IMeshService
{
    #region Mesh Creation

    /// <summary>
    /// Create a mesh from vertex data without indices (for non-indexed drawing).
    /// </summary>
    /// <param name="vertices">Raw vertex data.</param>
    /// <param name="layout">Describes the vertex data layout.</param>
    /// <returns>A handle to the created mesh, or <see cref="MeshHandle.Null"/> on failure.</returns>
    /// <remarks>
    /// The vertex data is consumed immediately. The caller may reuse or release
    /// the buffer after this method completes.
    /// </remarks>
    ValueTask<MeshHandle> CreateMesh(ReadOnlyMemory<byte> vertices, VertexLayout layout);

    /// <summary>
    /// Create a mesh from vertex and index data.
    /// </summary>
    /// <param name="vertices">Raw vertex data.</param>
    /// <param name="layout">Describes the vertex data layout.</param>
    /// <param name="indices">Index data for indexed drawing.</param>
    /// <param name="indexFormat">Size of each index element.</param>
    /// <returns>A handle to the created mesh, or <see cref="MeshHandle.Null"/> on failure.</returns>
    /// <remarks>
    /// The vertex and index data is consumed immediately. The caller may reuse or release
    /// the buffers after this method completes.
    /// </remarks>
    ValueTask<MeshHandle> CreateMesh(ReadOnlyMemory<byte> vertices, VertexLayout layout, ReadOnlyMemory<byte> indices, EIndexFormat indexFormat);

    #endregion

    #region Mesh Updates

    /// <summary>
    /// Update all vertex data for an existing mesh.
    /// </summary>
    /// <param name="handle">The mesh to update.</param>
    /// <param name="vertices">New vertex data (must match original layout).</param>
    /// <remarks>
    /// The vertex data is consumed immediately. The caller may reuse or release
    /// the buffer after this method completes.
    /// </remarks>
    ValueTask UpdateVertices(MeshHandle handle, ReadOnlyMemory<byte> vertices);

    /// <summary>
    /// Update a region of vertex data for an existing mesh.
    /// </summary>
    /// <param name="handle">The mesh to update.</param>
    /// <param name="byteOffset">Byte offset into the vertex buffer.</param>
    /// <param name="vertices">New vertex data for the region.</param>
    /// <remarks>
    /// The vertex data is consumed immediately. The caller may reuse or release
    /// the buffer after this method completes.
    /// </remarks>
    ValueTask UpdateVertices(MeshHandle handle, int byteOffset, ReadOnlyMemory<byte> vertices);

    /// <summary>
    /// Update all index data for an existing mesh.
    /// </summary>
    /// <param name="handle">The mesh to update.</param>
    /// <param name="indices">New index data.</param>
    /// <remarks>
    /// The index data is consumed immediately. The caller may reuse or release
    /// the buffer after this method completes.
    /// </remarks>
    ValueTask UpdateIndices(MeshHandle handle, ReadOnlyMemory<byte> indices);

    #endregion

    #region Mesh Info

    /// <summary>
    /// Get mesh information.
    /// </summary>
    /// <param name="handle">The mesh handle.</param>
    /// <returns>Mesh info, or default values if handle is invalid.</returns>
    MeshInfo GetMeshInfo(MeshHandle handle);

    /// <summary>
    /// Check if a mesh handle is still valid.
    /// </summary>
    bool IsValid(MeshHandle handle);

    /// <summary>
    /// Destroy a mesh and free associated resources.
    /// </summary>
    void Release(MeshHandle handle);

    #endregion
}
