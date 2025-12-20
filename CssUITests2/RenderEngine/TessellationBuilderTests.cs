using CssUI;
using CssUI.Common;
using CssUI.Rendering;
using Xunit;

namespace CssUITests;

public class TessellationBuilderTests
{
    #region Constructor and Initial State

    [Fact]
    public void Constructor_WithNoRequirements_CreatesPositionOnlyLayout()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        Assert.Equal(0, builder.VertexCount);
        Assert.Equal(0, builder.IndexCount);
    }

    [Fact]
    public void Constructor_WithColorRequirement_CreatesPositionColorLayout()
    {
        var builder = new TessellationBuilder(TessellationRequirements.Color);

        builder.AddVertex(0, 0, 0, new Color(255, 0, 0, 255));
        var result = builder.Build();

        Assert.Equal(16, result.Layout.Stride); // 12 (pos) + 4 (color)
    }

    [Fact]
    public void Constructor_WithTexCoordRequirement_CreatesPositionTexCoordLayout()
    {
        var builder = new TessellationBuilder(TessellationRequirements.TexCoord);

        builder.AddVertex(0, 0, 0, 0.5f, 0.5f);
        var result = builder.Build();

        Assert.Equal(20, result.Layout.Stride); // 12 (pos) + 8 (uv)
    }

    [Fact]
    public void Constructor_WithAllRequirements_CreatesFullLayout()
    {
        var requirements = TessellationRequirements.TexCoord 
                         | TessellationRequirements.Color 
                         | TessellationRequirements.Normal;
        var builder = new TessellationBuilder(requirements);

        builder.AddVertex(0, 0, 0, 0, 0, new Color(255, 255, 255, 255), 0, 1, 0);
        var result = builder.Build();

        // 12 (pos) + 8 (uv) + 4 (color) + 12 (normal) = 36
        Assert.Equal(36, result.Layout.Stride);
    }

    #endregion

    #region AddVertex Tests

    [Fact]
    public void AddVertex_PositionOnly_IncrementsVertexCount()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(1.0f, 2.0f, 3.0f);

        Assert.Equal(1, builder.VertexCount);
    }

    [Fact]
    public void AddVertex_MultipleVertices_TracksCountCorrectly()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.AddVertex(0, 1, 0);

        Assert.Equal(3, builder.VertexCount);
    }

    [Fact]
    public void AddVertex_WithColor_StoresColorData()
    {
        var builder = new TessellationBuilder(TessellationRequirements.Color);
        var color = new Color(128, 64, 32, 255);

        builder.AddVertex(0, 0, 0, color);
        var result = builder.Build();

        Assert.Equal(1, result.VertexCount);
        Assert.False(result.IsEmpty);
    }

    [Fact]
    public void AddVertex_WithTexCoord_StoresUVData()
    {
        var builder = new TessellationBuilder(TessellationRequirements.TexCoord);

        builder.AddVertex(0, 0, 0, 0.25f, 0.75f);
        var result = builder.Build();

        Assert.Equal(1, result.VertexCount);
        Assert.Equal(20, result.Layout.Stride);
    }

    [Fact]
    public void AddVertex_WithNormal_StoresNormalData()
    {
        var builder = new TessellationBuilder(TessellationRequirements.Normal);

        builder.AddVertex(0, 0, 0, 0, 1, 0);
        var result = builder.Build();

        Assert.Equal(1, result.VertexCount);
        // 12 (pos) + 12 (normal) = 24
        Assert.Equal(24, result.Layout.Stride);
    }

    [Fact]
    public void AddVertex_FullOverload_StoresAllAttributes()
    {
        var requirements = TessellationRequirements.TexCoord 
                         | TessellationRequirements.Color 
                         | TessellationRequirements.Normal;
        var builder = new TessellationBuilder(requirements);

        builder.AddVertex(1, 2, 3, 0.5f, 0.5f, new Color(255, 0, 0, 255), 0, 1, 0);
        var result = builder.Build();

        Assert.Equal(1, result.VertexCount);
        Assert.Equal(36, result.Layout.Stride);
    }

    #endregion

    #region AddIndex Tests

    [Fact]
    public void AddIndex_SingleIndex_IncrementsIndexCount()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddIndex(0);

        Assert.Equal(1, builder.IndexCount);
    }

    [Fact]
    public void AddTriangle_AddsThreeIndices()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddTriangle(0, 1, 2);

        Assert.Equal(3, builder.IndexCount);
    }

    [Fact]
    public void AddQuad_AddsSixIndices()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddQuad(0, 1, 2, 3);

        Assert.Equal(6, builder.IndexCount);
    }

    [Fact]
    public void AddQuad_CreatesCorrectTriangles()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        // Add 4 vertices for a quad
        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.AddVertex(1, 1, 0);
        builder.AddVertex(0, 1, 0);

        builder.AddQuad(0, 1, 2, 3);

        var result = builder.Build();

        Assert.Equal(4, result.VertexCount);
        Assert.Equal(6, result.IndexCount);
        Assert.True(result.IsIndexed);
    }

    #endregion

    #region Build Tests

    [Fact]
    public void Build_WithNoVertices_ReturnsEmptyResult()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        var result = builder.Build();

        Assert.True(result.IsEmpty);
        Assert.Equal(0, result.VertexCount);
        Assert.Equal(0, result.IndexCount);
    }

    [Fact]
    public void Build_WithVerticesNoIndices_ReturnsNonIndexedResult()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.AddVertex(0, 1, 0);

        var result = builder.Build();

        Assert.False(result.IsIndexed);
        Assert.Equal(3, result.VertexCount);
        Assert.Equal(0, result.IndexCount);
    }

    [Fact]
    public void Build_WithVerticesAndIndices_ReturnsIndexedResult()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.AddVertex(0, 1, 0);
        builder.AddTriangle(0, 1, 2);

        var result = builder.Build();

        Assert.True(result.IsIndexed);
        Assert.Equal(3, result.VertexCount);
        Assert.Equal(3, result.IndexCount);
    }

    [Fact]
    public void Build_WithFewVertices_UsesUInt16Indices()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.AddVertex(0, 1, 0);
        builder.AddTriangle(0, 1, 2);

        var result = builder.Build();

        Assert.Equal(EIndexFormat.UInt16, result.IndexFormat);
    }

    [Fact]
    public void Build_ProducesCorrectVertexDataSize()
    {
        var builder = new TessellationBuilder(TessellationRequirements.Color);

        builder.AddVertex(0, 0, 0, new Color(255, 0, 0, 255));
        builder.AddVertex(1, 0, 0, new Color(0, 255, 0, 255));

        var result = builder.Build();

        // 2 vertices * 16 bytes each (12 pos + 4 color)
        Assert.Equal(32, result.Vertices.Length);
    }

    [Fact]
    public void Build_ProducesCorrectIndexDataSize()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.AddVertex(0, 1, 0);
        builder.AddTriangle(0, 1, 2);

        var result = builder.Build();

        // 3 indices * 2 bytes each (UInt16)
        Assert.Equal(6, result.Indices.Length);
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_ResetsVertexCount()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddVertex(1, 0, 0);
        builder.Clear();

        Assert.Equal(0, builder.VertexCount);
    }

    [Fact]
    public void Clear_ResetsIndexCount()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddTriangle(0, 1, 2);
        builder.Clear();

        Assert.Equal(0, builder.IndexCount);
    }

    [Fact]
    public void Clear_AllowsReuse()
    {
        var builder = new TessellationBuilder(TessellationRequirements.None);

        builder.AddVertex(0, 0, 0);
        builder.AddTriangle(0, 0, 0);
        builder.Clear();

        builder.AddVertex(1, 1, 1);
        builder.AddVertex(2, 2, 2);

        Assert.Equal(2, builder.VertexCount);
        Assert.Equal(0, builder.IndexCount);
    }

    #endregion

    #region Layout Determination Tests

    [Theory]
    [InlineData(TessellationRequirements.None, 12)]
    [InlineData(TessellationRequirements.TexCoord, 20)]
    [InlineData(TessellationRequirements.Color, 16)]
    [InlineData(TessellationRequirements.TexCoord | TessellationRequirements.Color, 24)]
    [InlineData(TessellationRequirements.Normal, 24)]
    [InlineData(TessellationRequirements.TexCoord | TessellationRequirements.Normal, 32)]
    [InlineData(TessellationRequirements.Color | TessellationRequirements.Normal, 28)]
    [InlineData(TessellationRequirements.TexCoord | TessellationRequirements.Color | TessellationRequirements.Normal, 36)]
    public void DetermineLayout_ProducesCorrectStride(TessellationRequirements requirements, int expectedStride)
    {
        var builder = new TessellationBuilder(requirements);

        // Add a vertex to trigger layout creation
        builder.AddVertex(0, 0, 0);
        var result = builder.Build();

        Assert.Equal(expectedStride, result.Layout.Stride);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void BuildTriangle_ProducesValidMeshData()
    {
        var builder = new TessellationBuilder(TessellationRequirements.Color);

        // Build a colored triangle
        var red = new Color(255, 0, 0, 255);
        var green = new Color(0, 255, 0, 255);
        var blue = new Color(0, 0, 255, 255);

        builder.AddVertex(0, 0, 0, red);
        builder.AddVertex(1, 0, 0, green);
        builder.AddVertex(0.5f, 1, 0, blue);
        builder.AddTriangle(0, 1, 2);

        var result = builder.Build();

        Assert.False(result.IsEmpty);
        Assert.True(result.IsIndexed);
        Assert.Equal(3, result.VertexCount);
        Assert.Equal(3, result.IndexCount);
        Assert.Equal(16, result.Layout.Stride);
        Assert.Equal(48, result.Vertices.Length); // 3 * 16
        Assert.Equal(6, result.Indices.Length);   // 3 * 2 (UInt16)
    }

    [Fact]
    public void BuildQuad_ProducesValidMeshData()
    {
        var builder = new TessellationBuilder(TessellationRequirements.TexCoord);

        // Build a textured quad
        builder.AddVertex(0, 0, 0, 0, 0);
        builder.AddVertex(1, 0, 0, 1, 0);
        builder.AddVertex(1, 1, 0, 1, 1);
        builder.AddVertex(0, 1, 0, 0, 1);
        builder.AddQuad(0, 1, 2, 3);

        var result = builder.Build();

        Assert.False(result.IsEmpty);
        Assert.True(result.IsIndexed);
        Assert.Equal(4, result.VertexCount);
        Assert.Equal(6, result.IndexCount);
        Assert.Equal(20, result.Layout.Stride);
    }

    #endregion
}
