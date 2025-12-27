using System;
using CssUI.CSS;
using CssUI.CSS.Media;
using Xunit;

namespace CssUITests.CSS.Media;

/// <summary>
/// Unit tests for MediaFeature serialization using ISpanFormattable.
/// Spec: https://www.w3.org/TR/mediaqueries-4/#mq-features
/// </summary>
public class MediaFeatureTests
{
    #region Boolean Context Tests

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_BooleanContext_Color_ReturnsParenthesizedName()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Color);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(color)", result);
    }

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_BooleanContext_Width_ReturnsParenthesizedName()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Width);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(width)", result);
    }

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_BooleanContext_Height_ReturnsParenthesizedName()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Height);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(height)", result);
    }

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_BooleanContext_Grid_ReturnsParenthesizedName()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Grid);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(grid)", result);
    }

    #endregion

    #region Range Context Tests

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_RangeContext_PlainRange_ReturnsNameColonValue()
    {
        // Arrange
        var values = new CssValue[]
        {
            CssValue.From(EMediaFeatureName.Width),
            CssValue.From_Dimension(800, ECssUnit.PX)
        };
        var ops = Array.Empty<EMediaOperator>();
        var feature = new MediaFeature(values, ops);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(width: 800px)", result);
    }

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_RangeContext_MinWidth_ReturnsNameColonValue()
    {
        // Arrange
        var values = new CssValue[]
        {
            CssValue.From(EMediaFeatureName.Min_Width),
            CssValue.From_Dimension(600, ECssUnit.PX)
        };
        var ops = Array.Empty<EMediaOperator>();
        var feature = new MediaFeature(values, ops);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(min-width: 600px)", result);
    }

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_RangeContext_MaxHeight_ReturnsNameColonValue()
    {
        // Arrange
        var values = new CssValue[]
        {
            CssValue.From(EMediaFeatureName.Max_Height),
            CssValue.From_Dimension(1080, ECssUnit.PX)
        };
        var ops = Array.Empty<EMediaOperator>();
        var feature = new MediaFeature(values, ops);

        // Act
        var result = feature.ToString();

        // Assert
        Assert.Equal("(max-height: 1080px)", result);
    }

    #endregion

    #region TryFormat Tests

    [Fact]
    [Trait("Category", "Media")]
    public void TryFormat_BooleanContext_WritesToSpan()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Color);
        Span<char> buffer = stackalloc char[64];

        // Act
        var success = feature.TryFormat(buffer, out int charsWritten);

        // Assert
        Assert.True(success);
        Assert.Equal("(color)", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "Media")]
    public void TryFormat_InsufficientBuffer_ReturnsFalse()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Color);
        Span<char> buffer = stackalloc char[2]; // Too small for "(color)"

        // Act
        var success = feature.TryFormat(buffer, out int charsWritten);

        // Assert
        Assert.False(success);
    }

    [Fact]
    [Trait("Category", "Media")]
    public void TryFormat_RangeContext_WritesToSpan()
    {
        // Arrange
        var values = new CssValue[]
        {
            CssValue.From(EMediaFeatureName.Width),
            CssValue.From_Dimension(800, ECssUnit.PX)
        };
        var ops = Array.Empty<EMediaOperator>();
        var feature = new MediaFeature(values, ops);
        Span<char> buffer = stackalloc char[64];

        // Act
        var success = feature.TryFormat(buffer, out int charsWritten);

        // Assert
        Assert.True(success);
        Assert.Equal("(width: 800px)", buffer[..charsWritten].ToString());
    }

    #endregion

    #region IFormattable Tests

    [Fact]
    [Trait("Category", "Media")]
    public void ToString_WithFormatProvider_ReturnsExpectedResult()
    {
        // Arrange
        var feature = new MediaFeature(EMediaFeatureName.Color);

        // Act
        var result = ((IFormattable)feature).ToString(null, null);

        // Assert
        Assert.Equal("(color)", result);
    }

    #endregion
}
