namespace CssUI;

/// <summary>
/// Information about a mesh.
/// </summary>
public readonly struct MeshInfo
{
    /// <summary>Number of vertices in the mesh.</summary>
    public readonly int VertexCount;

    /// <summary>Number of indices in the mesh (0 if non-indexed).</summary>
    public readonly int IndexCount;

    /// <summary>The vertex layout of the mesh.</summary>
    public readonly VertexLayout Layout;

    /// <summary>The index format (only valid if IndexCount > 0).</summary>
    public readonly EIndexFormat IndexFormat;

    /// <summary>Whether this mesh uses indexed drawing.</summary>
    public bool IsIndexed => IndexCount > 0;

    public MeshInfo(int vertexCount, VertexLayout layout, int indexCount = 0, EIndexFormat indexFormat = EIndexFormat.UInt16)
    {
        VertexCount = vertexCount;
        Layout = layout;
        IndexCount = indexCount;
        IndexFormat = indexFormat;
    }
}
