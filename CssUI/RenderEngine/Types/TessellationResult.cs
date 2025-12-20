using System;
using CssUI.Common;

namespace CssUI;

/// <summary>
/// The result of tessellating one or more objects into mesh data.
/// </summary>
public readonly record struct TessellationResult
{
    /// <summary>
    /// Packed vertex data according to <see cref="Layout"/>.
    /// </summary>
    public required ReadOnlyMemory<byte> Vertices { get; init; }

    /// <summary>
    /// Packed index data. Empty if non-indexed.
    /// </summary>
    public ReadOnlyMemory<byte> Indices { get; init; }

    /// <summary>
    /// Describes the vertex format.
    /// </summary>
    public required VertexLayout Layout { get; init; }

    /// <summary>
    /// The index format (UInt16 or UInt32). Only relevant if <see cref="IsIndexed"/> is true.
    /// </summary>
    public EIndexFormat IndexFormat { get; init; }

    /// <summary>
    /// Number of vertices in the result.
    /// </summary>
    public required int VertexCount { get; init; }

    /// <summary>
    /// Number of indices in the result. Zero if non-indexed.
    /// </summary>
    public int IndexCount { get; init; }

    /// <summary>
    /// Returns true if this result uses indexed drawing.
    /// </summary>
    public bool IsIndexed => IndexCount > 0;

    /// <summary>
    /// Returns true if this result contains no geometry.
    /// </summary>
    public bool IsEmpty => VertexCount == 0;

    /// <summary>
    /// An empty tessellation result.
    /// </summary>
    public static TessellationResult Empty => new()
    {
        Vertices = ReadOnlyMemory<byte>.Empty,
        Indices = ReadOnlyMemory<byte>.Empty,
        Layout = VertexLayout.PositionOnly,
        IndexFormat = EIndexFormat.UInt16,
        VertexCount = 0,
        IndexCount = 0
    };
}
