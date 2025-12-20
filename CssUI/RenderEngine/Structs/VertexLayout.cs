namespace CssUI;

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
