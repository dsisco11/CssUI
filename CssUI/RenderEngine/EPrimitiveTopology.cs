namespace CssUI;

/// <summary>
/// Primitive topology for mesh rendering.
/// </summary>
public enum EPrimitiveTopology
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
