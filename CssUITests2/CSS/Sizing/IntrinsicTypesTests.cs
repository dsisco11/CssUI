using Xunit;

namespace CssUI.CSS.Tests;

/// <summary>
/// Tests for IntrinsicAxisSize struct operations.
/// </summary>
public class IntrinsicAxisSizeTests
{
    [Fact]
    public void Constructor_SetsMinAndMaxContent()
    {
        var size = new IntrinsicAxisSize(50, 100);

        Assert.Equal(50, size.MinContent);
        Assert.Equal(100, size.MaxContent);
    }

    [Fact]
    public void Definite_SetsBothToSameValue()
    {
        var size = IntrinsicAxisSize.Definite(75);

        Assert.Equal(75, size.MinContent);
        Assert.Equal(75, size.MaxContent);
    }

    [Fact]
    public void Zero_ReturnsBothAsZero()
    {
        var size = IntrinsicAxisSize.Zero;

        Assert.Equal(0, size.MinContent);
        Assert.Equal(0, size.MaxContent);
    }

    [Fact]
    public void Max_ReturnsMaxOfBothComponents()
    {
        var a = new IntrinsicAxisSize(30, 100);
        var b = new IntrinsicAxisSize(50, 80);

        var result = IntrinsicAxisSize.Max(a, b);

        Assert.Equal(50, result.MinContent);  // max(30, 50)
        Assert.Equal(100, result.MaxContent); // max(100, 80)
    }

    [Fact]
    public void Sum_AddsBothComponents()
    {
        var a = new IntrinsicAxisSize(30, 100);
        var b = new IntrinsicAxisSize(20, 50);

        var result = IntrinsicAxisSize.Sum(a, b);

        Assert.Equal(50, result.MinContent);  // 30 + 20
        Assert.Equal(150, result.MaxContent); // 100 + 50
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var size = new IntrinsicAxisSize(25.5, 100.3);
        var str = size.ToString();

        Assert.Contains("min:", str);
        Assert.Contains("max:", str);
    }
}

/// <summary>
/// Tests for IntrinsicSize struct operations.
/// </summary>
public class IntrinsicSizeTests
{
    [Fact]
    public void Constructor_FromAxisSizes()
    {
        var inline = new IntrinsicAxisSize(50, 100);
        var block = new IntrinsicAxisSize(20, 40);

        var size = new IntrinsicSize(inline, block);

        Assert.Equal(50, size.Inline.MinContent);
        Assert.Equal(100, size.Inline.MaxContent);
        Assert.Equal(20, size.Block.MinContent);
        Assert.Equal(40, size.Block.MaxContent);
    }

    [Fact]
    public void Constructor_FromFourValues()
    {
        var size = new IntrinsicSize(50, 100, 20, 40);

        Assert.Equal(50, size.Inline.MinContent);
        Assert.Equal(100, size.Inline.MaxContent);
        Assert.Equal(20, size.Block.MinContent);
        Assert.Equal(40, size.Block.MaxContent);
    }

    [Fact]
    public void Definite_SetsBothAxesToDefinite()
    {
        var size = IntrinsicSize.Definite(200, 100);

        Assert.Equal(200, size.Inline.MinContent);
        Assert.Equal(200, size.Inline.MaxContent);
        Assert.Equal(100, size.Block.MinContent);
        Assert.Equal(100, size.Block.MaxContent);
    }

    [Fact]
    public void Zero_ReturnsAllZeros()
    {
        var size = IntrinsicSize.Zero;

        Assert.Equal(0, size.Inline.MinContent);
        Assert.Equal(0, size.Inline.MaxContent);
        Assert.Equal(0, size.Block.MinContent);
        Assert.Equal(0, size.Block.MaxContent);
    }
}

/// <summary>
/// Tests for IntrinsicSizeContext.
/// </summary>
public class IntrinsicSizeContextTests
{
    [Fact]
    public void DefaultContext_HasExpectedDefaults()
    {
        var context = new IntrinsicSizeContext();

        // Default struct initialization sets all fields to zero/null
        Assert.Null(context.AvailableInline);
        Assert.Null(context.AvailableBlock);
        // SizeType default is MaxContent which is 1, but struct default is 0 (MinContent)
        // Direction/WritingMode are enums that default to their 0 values
        Assert.Equal(default(EWritingMode), context.WritingMode);
        Assert.Equal(default(EDirection), context.Direction);
        Assert.Equal(default(EIntrinsicSizeType), context.SizeType);
    }

    [Fact]
    public void ConstructorWithDefaults_HasCorrectParameterDefaults()
    {
        // When explicitly calling constructor with defaults, parameters apply
        var context = new IntrinsicSizeContext(
            availableInline: null,
            availableBlock: null);

        Assert.Equal(EWritingMode.Horizontal_TB, context.WritingMode);
        Assert.Equal(EDirection.LTR, context.Direction);
        Assert.Equal(EIntrinsicSizeType.MaxContent, context.SizeType);
    }

    [Fact]
    public void ForMinContent_ChangesSizeType()
    {
        var context = new IntrinsicSizeContext();
        var minContext = context.ForMinContent();

        Assert.True(minContext.IsMinContent);
        Assert.False(minContext.IsMaxContent);
        Assert.Equal(EIntrinsicSizeType.MinContent, minContext.SizeType);
    }

    [Fact]
    public void ForMaxContent_ChangesSizeType()
    {
        var context = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.MinContent);
        var maxContext = context.ForMaxContent();

        Assert.False(maxContext.IsMinContent);
        Assert.True(maxContext.IsMaxContent);
        Assert.Equal(EIntrinsicSizeType.MaxContent, maxContext.SizeType);
    }

    [Fact]
    public void WithAvailableSpace_UpdatesConstraints()
    {
        var context = new IntrinsicSizeContext();
        var constrained = context.WithAvailableSpace(500, 300);

        Assert.Equal(500, constrained.AvailableInline);
        Assert.Equal(300, constrained.AvailableBlock);
    }

    [Fact]
    public void IsMinContent_ReturnsTrueOnlyForMinContent()
    {
        var minContext = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.MinContent);
        var maxContext = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.MaxContent);
        var fitContext = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.FitContent);

        Assert.True(minContext.IsMinContent);
        Assert.False(maxContext.IsMinContent);
        Assert.False(fitContext.IsMinContent);
    }

    [Fact]
    public void IsMaxContent_ReturnsTrueOnlyForMaxContent()
    {
        var minContext = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.MinContent);
        var maxContext = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.MaxContent);
        var fitContext = new IntrinsicSizeContext(sizeType: EIntrinsicSizeType.FitContent);

        Assert.False(minContext.IsMaxContent);
        Assert.True(maxContext.IsMaxContent);
        Assert.False(fitContext.IsMaxContent);
    }
}
