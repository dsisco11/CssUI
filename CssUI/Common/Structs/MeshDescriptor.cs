namespace CssUI.Common;

/// <summary>
/// Describes a GPU mesh created by an <see cref="IMeshService"/>.
/// </summary>
public readonly record struct MeshDescriptor
{
    /// <summary>
    /// The handle to the mesh.
    /// </summary>
    public required MeshHandle Handle { get; init; }

    /// <summary>
    /// Number of vertices in the mesh.
    /// </summary>
    public required int VertexCount { get; init; }

    /// <summary>
    /// Number of indices in the mesh (0 if non-indexed).
    /// </summary>
    public int IndexCount { get; init; }

    /// <summary>
    /// The vertex layout of the mesh.
    /// </summary>
    public required VertexLayout Layout { get; init; }

    /// <summary>
    /// The index format (only valid if IndexCount > 0).
    /// </summary>
    public EIndexFormat IndexFormat { get; init; }

    /// <summary>
    /// The primitive topology of the mesh.
    /// </summary>
    public EPrimitiveTopology Topology { get; init; }

    /// <summary>
    /// Whether this mesh uses indexed drawing.
    /// </summary>
    public bool IsIndexed => IndexCount > 0;

    /// <summary>
    /// Returns true if this descriptor is invalid.
    /// </summary>
    public bool IsNull => Handle.IsNull;
}
