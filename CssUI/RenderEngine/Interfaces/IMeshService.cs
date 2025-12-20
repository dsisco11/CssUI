using System;
using System.Threading.Tasks;

namespace CssUI;

/// <summary>
/// Numeric data types for vertex attributes.
/// </summary>
public enum VertexAttributeType
{
    /// <summary>8-bit signed integer.</summary>
    SByte,
    /// <summary>8-bit unsigned integer.</summary>
    Byte,
    /// <summary>16-bit signed integer.</summary>
    Int16,
    /// <summary>16-bit unsigned integer.</summary>
    UInt16,
    /// <summary>32-bit signed integer.</summary>
    Int32,
    /// <summary>32-bit unsigned integer.</summary>
    UInt32,
    /// <summary>16-bit floating point (half).</summary>
    Half,
    /// <summary>32-bit floating point (single).</summary>
    Single,
    /// <summary>64-bit floating point (double).</summary>
    Double
}

/// <summary>
/// Describes a single vertex attribute (position, color, etc.).
/// </summary>
public readonly struct VertexAttribute
{
    /// <summary>
    /// The numeric data type of each component.
    /// </summary>
    public readonly VertexAttributeType Type;

    /// <summary>
    /// Number of components (1-4). E.g., 3 for xyz position, 2 for uv coords.
    /// Zero indicates the attribute is not present.
    /// </summary>
    public readonly int ComponentCount;

    /// <summary>
    /// Byte offset of this attribute within a vertex.
    /// </summary>
    public readonly int Offset;

    /// <summary>
    /// If true, integer types are normalized to [0,1] or [-1,1] range when read as floats.
    /// </summary>
    public readonly bool Normalized;

    /// <summary>
    /// Returns true if this attribute is present (has components).
    /// </summary>
    public bool IsPresent => ComponentCount > 0;

    public VertexAttribute(VertexAttributeType type, int componentCount, int offset, bool normalized = false)
    {
        Type = type;
        ComponentCount = componentCount;
        Offset = offset;
        Normalized = normalized;
    }

    /// <summary>
    /// Gets the size in bytes of one component of this attribute.
    /// </summary>
    public int ComponentSize => Type switch
    {
        VertexAttributeType.SByte => 1,
        VertexAttributeType.Byte => 1,
        VertexAttributeType.Int16 => 2,
        VertexAttributeType.UInt16 => 2,
        VertexAttributeType.Int32 => 4,
        VertexAttributeType.UInt32 => 4,
        VertexAttributeType.Half => 2,
        VertexAttributeType.Single => 4,
        VertexAttributeType.Double => 8,
        _ => 0
    };

    /// <summary>
    /// Gets the total size in bytes of this attribute.
    /// </summary>
    public int TotalSize => ComponentSize * ComponentCount;

    /// <summary>
    /// Creates a 3-component float position attribute.
    /// </summary>
    public static VertexAttribute Position3f(int offset) => new(VertexAttributeType.Single, 3, offset);

    /// <summary>
    /// Creates a 2-component float texture coordinate attribute.
    /// </summary>
    public static VertexAttribute TexCoord2f(int offset) => new(VertexAttributeType.Single, 2, offset);

    /// <summary>
    /// Creates a 4-component byte color attribute (normalized to 0-1).
    /// </summary>
    public static VertexAttribute Color4b(int offset) => new(VertexAttributeType.Byte, 4, offset, normalized: true);

    /// <summary>
    /// Creates a 4-component float color attribute.
    /// </summary>
    public static VertexAttribute Color4f(int offset) => new(VertexAttributeType.Single, 4, offset);

    /// <summary>
    /// Creates a 3-component float normal attribute.
    /// </summary>
    public static VertexAttribute Normal3f(int offset) => new(VertexAttributeType.Single, 3, offset);
}

/// <summary>
/// Describes the layout of vertex data in a mesh.
/// </summary>
public readonly struct VertexLayout
{
    /// <summary>
    /// Total size in bytes of a single vertex.
    /// </summary>
    public readonly int Stride;

    /// <summary>
    /// Position attribute (typically vec3). Check <see cref="VertexAttribute.IsPresent"/> to determine if set.
    /// </summary>
    public readonly VertexAttribute Position;

    /// <summary>
    /// Texture coordinate attribute (typically vec2). Check <see cref="VertexAttribute.IsPresent"/> to determine if set.
    /// </summary>
    public readonly VertexAttribute TexCoord;

    /// <summary>
    /// Color attribute (typically vec4 or 4 bytes). Check <see cref="VertexAttribute.IsPresent"/> to determine if set.
    /// </summary>
    public readonly VertexAttribute Color;

    /// <summary>
    /// Normal attribute (typically vec3). Check <see cref="VertexAttribute.IsPresent"/> to determine if set.
    /// </summary>
    public readonly VertexAttribute Normal;

    public VertexLayout(int stride, VertexAttribute position = default, VertexAttribute texCoord = default, VertexAttribute color = default, VertexAttribute normal = default)
    {
        Stride = stride;
        Position = position;
        TexCoord = texCoord;
        Color = color;
        Normal = normal;
    }

    /// <summary>
    /// Creates a layout for position-only vertices (3 floats = 12 bytes).
    /// </summary>
    public static VertexLayout PositionOnly => new(
        stride: 12,
        position: VertexAttribute.Position3f(0));

    /// <summary>
    /// Creates a layout for position + texture coordinate vertices (5 floats = 20 bytes).
    /// </summary>
    public static VertexLayout PositionTexCoord => new(
        stride: 20,
        position: VertexAttribute.Position3f(0),
        texCoord: VertexAttribute.TexCoord2f(12));

    /// <summary>
    /// Creates a layout for position + 4-byte color vertices (3 floats + 4 bytes = 16 bytes).
    /// </summary>
    public static VertexLayout PositionColor => new(
        stride: 16,
        position: VertexAttribute.Position3f(0),
        color: VertexAttribute.Color4b(12));

    /// <summary>
    /// Creates a layout for position + texture coordinate + 4-byte color vertices (5 floats + 4 bytes = 24 bytes).
    /// </summary>
    public static VertexLayout PositionTexCoordColor => new(
        stride: 24,
        position: VertexAttribute.Position3f(0),
        texCoord: VertexAttribute.TexCoord2f(12),
        color: VertexAttribute.Color4b(20));
}

/// <summary>
/// Index element size for indexed drawing.
/// </summary>
public enum IndexFormat
{
    /// <summary>16-bit unsigned indices (ushort).</summary>
    UInt16,
    /// <summary>32-bit unsigned indices (uint).</summary>
    UInt32
}

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
    ValueTask<MeshHandle> CreateMeshAsync(ReadOnlyMemory<byte> vertices, VertexLayout layout);

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
    ValueTask<MeshHandle> CreateMeshAsync(ReadOnlyMemory<byte> vertices, VertexLayout layout, ReadOnlyMemory<byte> indices, IndexFormat indexFormat);

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
    ValueTask UpdateVerticesAsync(MeshHandle handle, ReadOnlyMemory<byte> vertices);

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
    ValueTask UpdateVerticesAsync(MeshHandle handle, int byteOffset, ReadOnlyMemory<byte> vertices);

    /// <summary>
    /// Update all index data for an existing mesh.
    /// </summary>
    /// <param name="handle">The mesh to update.</param>
    /// <param name="indices">New index data.</param>
    /// <remarks>
    /// The index data is consumed immediately. The caller may reuse or release
    /// the buffer after this method completes.
    /// </remarks>
    ValueTask UpdateIndicesAsync(MeshHandle handle, ReadOnlyMemory<byte> indices);

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
    void DestroyMesh(MeshHandle handle);

    #endregion
}

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
    public readonly IndexFormat IndexFormat;

    /// <summary>Whether this mesh uses indexed drawing.</summary>
    public bool IsIndexed => IndexCount > 0;

    public MeshInfo(int vertexCount, VertexLayout layout, int indexCount = 0, IndexFormat indexFormat = IndexFormat.UInt16)
    {
        VertexCount = vertexCount;
        Layout = layout;
        IndexCount = indexCount;
        IndexFormat = indexFormat;
    }
}
