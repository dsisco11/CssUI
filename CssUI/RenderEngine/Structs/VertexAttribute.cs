namespace CssUI;

/// <summary>
/// Describes a single vertex attribute (position, color, etc.).
/// </summary>
public readonly struct VertexAttribute
{
    /// <summary>
    /// The numeric data type of each component.
    /// </summary>
    public readonly EVertexAttributeType Type;

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

    public VertexAttribute(EVertexAttributeType type, int componentCount, int offset, bool normalized = false)
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
        EVertexAttributeType.SByte => 1,
        EVertexAttributeType.Byte => 1,
        EVertexAttributeType.Int16 => 2,
        EVertexAttributeType.UInt16 => 2,
        EVertexAttributeType.Int32 => 4,
        EVertexAttributeType.UInt32 => 4,
        EVertexAttributeType.Half => 2,
        EVertexAttributeType.Single => 4,
        EVertexAttributeType.Double => 8,
        _ => 0
    };

    /// <summary>
    /// Gets the total size in bytes of this attribute.
    /// </summary>
    public int TotalSize => ComponentSize * ComponentCount;

    /// <summary>
    /// Creates a 3-component float position attribute.
    /// </summary>
    public static VertexAttribute Position3f(int offset) => new(EVertexAttributeType.Single, 3, offset);

    /// <summary>
    /// Creates a 2-component float texture coordinate attribute.
    /// </summary>
    public static VertexAttribute TexCoord2f(int offset) => new(EVertexAttributeType.Single, 2, offset);

    /// <summary>
    /// Creates a 4-component byte color attribute (normalized to 0-1).
    /// </summary>
    public static VertexAttribute Color4b(int offset) => new(EVertexAttributeType.Byte, 4, offset, normalized: true);

    /// <summary>
    /// Creates a 4-component float color attribute.
    /// </summary>
    public static VertexAttribute Color4f(int offset) => new(EVertexAttributeType.Single, 4, offset);

    /// <summary>
    /// Creates a 3-component float normal attribute.
    /// </summary>
    public static VertexAttribute Normal3f(int offset) => new(EVertexAttributeType.Single, 3, offset);
}
