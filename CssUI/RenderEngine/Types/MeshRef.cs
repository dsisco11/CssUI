using System;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// A reference to a GPU mesh resource that manages its lifecycle.
/// </summary>
/// <param name="Descriptor">The mesh descriptor containing handle and metadata.</param>
/// <param name="MeshService">The mesh service used to release the mesh.</param>
public sealed record class MeshRef(
    MeshDescriptor Descriptor,
    IMeshService MeshService) : IDisposable, IRenderableResource
{
    private bool _disposed;

    /// <summary>
    /// Gets the mesh handle.
    /// </summary>
    public MeshHandle Handle => Descriptor.Handle;

    /// <summary>
    /// Gets the number of vertices in the mesh.
    /// </summary>
    public int VertexCount => Descriptor.VertexCount;

    /// <summary>
    /// Gets the number of indices in the mesh (0 if non-indexed).
    /// </summary>
    public int IndexCount => Descriptor.IndexCount;

    /// <summary>
    /// Gets the vertex layout of the mesh.
    /// </summary>
    public VertexLayout Layout => Descriptor.Layout;

    /// <summary>
    /// Gets the index format.
    /// </summary>
    public EIndexFormat IndexFormat => Descriptor.IndexFormat;

    /// <summary>
    /// Gets the primitive topology of the mesh.
    /// </summary>
    public EPrimitiveTopology Topology => Descriptor.Topology;

    /// <summary>
    /// Whether this mesh uses indexed drawing.
    /// </summary>
    public bool IsIndexed => Descriptor.IsIndexed;

    /// <summary>
    /// Returns true if this mesh reference is null or invalid.
    /// </summary>
    public bool IsNull => Descriptor.IsNull;

    /// <inheritdoc/>
    public void Render(IRenderService renderService)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        renderService.DrawMesh(Handle, Topology);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (!Handle.IsNull)
        {
            MeshService.Release(Handle);
        }
    }
}
