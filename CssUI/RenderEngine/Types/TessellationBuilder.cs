using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using CssUI.Common;
using CssUI.Rendering;

namespace CssUI;

/// <summary>
/// Default implementation of <see cref="ITessellationBuilder"/> that accumulates
/// vertices and indices into byte arrays.
/// </summary>
public sealed class TessellationBuilder : ITessellationBuilder
{
    private readonly TessellationRequirements _requirements;
    private readonly VertexLayout _layout;
    private readonly int _vertexSize;

    private readonly List<byte> _vertices = new();
    private readonly List<int> _indices = new();

    // Default values for missing attributes
    private static readonly Color DefaultColor = new(255, 255, 255, 255);
    private const float DefaultU = 0f;
    private const float DefaultV = 0f;
    private const float DefaultNX = 0f;
    private const float DefaultNY = 0f;
    private const float DefaultNZ = 1f;

    /// <summary>
    /// Creates a new tessellation builder with the specified vertex requirements.
    /// </summary>
    /// <param name="requirements">The vertex attribute requirements.</param>
    public TessellationBuilder(TessellationRequirements requirements)
    {
        _requirements = requirements;
        _layout = DetermineLayout(requirements);
        _vertexSize = _layout.Stride;
    }

    /// <inheritdoc/>
    public int VertexCount => _vertices.Count / _vertexSize;

    /// <inheritdoc/>
    public int IndexCount => _indices.Count;

    #region Add Vertex Overloads

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z)
    {
        AddVertexInternal(x, y, z, DefaultU, DefaultV, DefaultColor, DefaultNX, DefaultNY, DefaultNZ);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, float u, float v)
    {
        AddVertexInternal(x, y, z, u, v, DefaultColor, DefaultNX, DefaultNY, DefaultNZ);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, Color color)
    {
        AddVertexInternal(x, y, z, DefaultU, DefaultV, color, DefaultNX, DefaultNY, DefaultNZ);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, float u, float v, Color color)
    {
        AddVertexInternal(x, y, z, u, v, color, DefaultNX, DefaultNY, DefaultNZ);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, float nx, float ny, float nz)
    {
        AddVertexInternal(x, y, z, DefaultU, DefaultV, DefaultColor, nx, ny, nz);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, float u, float v, float nx, float ny, float nz)
    {
        AddVertexInternal(x, y, z, u, v, DefaultColor, nx, ny, nz);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, Color color, float nx, float ny, float nz)
    {
        AddVertexInternal(x, y, z, DefaultU, DefaultV, color, nx, ny, nz);
    }

    /// <inheritdoc/>
    public void AddVertex(float x, float y, float z, float u, float v, Color color, float nx, float ny, float nz)
    {
        AddVertexInternal(x, y, z, u, v, color, nx, ny, nz);
    }

    private void AddVertexInternal(float x, float y, float z, float u, float v, Color color, float nx, float ny, float nz)
    {
        Span<byte> buffer = stackalloc byte[_vertexSize];
        int offset = 0;

        // Position (always present)
        WriteFloat(buffer, ref offset, x);
        WriteFloat(buffer, ref offset, y);
        WriteFloat(buffer, ref offset, z);

        // TexCoord (if required)
        if ((_requirements & TessellationRequirements.TexCoord) != 0)
        {
            WriteFloat(buffer, ref offset, u);
            WriteFloat(buffer, ref offset, v);
        }

        // Color (if required)
        if ((_requirements & TessellationRequirements.Color) != 0)
        {
            buffer[offset++] = color.R;
            buffer[offset++] = color.G;
            buffer[offset++] = color.B;
            buffer[offset++] = color.A;
        }

        // Normal (if required)
        if ((_requirements & TessellationRequirements.Normal) != 0)
        {
            WriteFloat(buffer, ref offset, nx);
            WriteFloat(buffer, ref offset, ny);
            WriteFloat(buffer, ref offset, nz);
        }

        // Add to vertex list
        for (int i = 0; i < _vertexSize; i++)
        {
            _vertices.Add(buffer[i]);
        }
    }

    private static void WriteFloat(Span<byte> buffer, ref int offset, float value)
    {
        BinaryPrimitives.WriteSingleLittleEndian(buffer.Slice(offset, 4), value);
        offset += 4;
    }

    #endregion

    #region Add Index

    /// <inheritdoc/>
    public void AddIndex(int index)
    {
        _indices.Add(index);
    }

    /// <inheritdoc/>
    public void AddTriangle(int i0, int i1, int i2)
    {
        _indices.Add(i0);
        _indices.Add(i1);
        _indices.Add(i2);
    }

    /// <inheritdoc/>
    public void AddQuad(int i0, int i1, int i2, int i3)
    {
        // First triangle: i0, i1, i2
        _indices.Add(i0);
        _indices.Add(i1);
        _indices.Add(i2);

        // Second triangle: i0, i2, i3
        _indices.Add(i0);
        _indices.Add(i2);
        _indices.Add(i3);
    }

    #endregion

    /// <inheritdoc/>
    public TessellationResult Build()
    {
        var vertexCount = VertexCount;
        var indexCount = IndexCount;

        if (vertexCount == 0)
        {
            return TessellationResult.Empty;
        }

        // Determine index format based on vertex count
        var indexFormat = vertexCount <= ushort.MaxValue ? EIndexFormat.UInt16 : EIndexFormat.UInt32;
        var indexData = BuildIndexData(indexFormat);

        return new TessellationResult
        {
            Vertices = _vertices.ToArray(),
            Indices = indexData,
            Layout = _layout,
            IndexFormat = indexFormat,
            VertexCount = vertexCount,
            IndexCount = indexCount
        };
    }

    private byte[] BuildIndexData(EIndexFormat format)
    {
        if (_indices.Count == 0)
        {
            return Array.Empty<byte>();
        }

        if (format == EIndexFormat.UInt16)
        {
            var data = new byte[_indices.Count * 2];
            for (int i = 0; i < _indices.Count; i++)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(data.AsSpan(i * 2, 2), (ushort)_indices[i]);
            }
            return data;
        }
        else
        {
            var data = new byte[_indices.Count * 4];
            for (int i = 0; i < _indices.Count; i++)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(i * 4, 4), (uint)_indices[i]);
            }
            return data;
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _vertices.Clear();
        _indices.Clear();
    }

    private static VertexLayout DetermineLayout(TessellationRequirements requirements)
    {
        bool hasTexCoord = (requirements & TessellationRequirements.TexCoord) != 0;
        bool hasColor = (requirements & TessellationRequirements.Color) != 0;
        bool hasNormal = (requirements & TessellationRequirements.Normal) != 0;

        // Use predefined layouts where possible
        if (!hasTexCoord && !hasColor && !hasNormal)
        {
            return VertexLayout.PositionOnly;
        }

        if (hasTexCoord && !hasColor && !hasNormal)
        {
            return VertexLayout.PositionTexCoord;
        }

        if (!hasTexCoord && hasColor && !hasNormal)
        {
            return VertexLayout.PositionColor;
        }

        if (hasTexCoord && hasColor && !hasNormal)
        {
            return VertexLayout.PositionTexCoordColor;
        }

        // Build custom layout for combinations with normals
        int offset = 12; // Start after position (3 floats)
        var position = VertexAttribute.Position3f(0);
        var texCoord = default(VertexAttribute);
        var color = default(VertexAttribute);
        var normal = default(VertexAttribute);

        if (hasTexCoord)
        {
            texCoord = VertexAttribute.TexCoord2f(offset);
            offset += 8; // 2 floats
        }

        if (hasColor)
        {
            color = VertexAttribute.Color4b(offset);
            offset += 4; // 4 bytes
        }

        if (hasNormal)
        {
            normal = VertexAttribute.Normal3f(offset);
            offset += 12; // 3 floats
        }

        return new VertexLayout(offset, position, texCoord, color, normal);
    }
}
