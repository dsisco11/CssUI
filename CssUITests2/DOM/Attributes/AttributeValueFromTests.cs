using CssUI;
using CssUI.DOM;
using CssUI.DOM.Enums;
using Xunit;

namespace CssUITests.DOM.Attributes;

/// <summary>
/// Unit tests for AttributeValue.From() behavior with various input types.
/// Investigates potential issues with empty string handling.
/// </summary>
public class AttributeValueFromTests
{
    #region String Values

    [Fact]
    public void From_EmptyString_CreatesValidValue()
    {
        // Act
        var value = AttributeValue.From(string.Empty);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(string.Empty, value.Data);
    }

    [Fact]
    public void From_NonEmptyString_CreatesValidValue()
    {
        // Act
        var value = AttributeValue.From("test-string");

        // Assert
        Assert.NotNull(value);
        Assert.Equal("test-string", value.Data);
    }

    [Fact]
    public void From_WhitespaceString_CreatesValidValue()
    {
        // Act
        var value = AttributeValue.From("   ");

        // Assert
        Assert.NotNull(value);
        Assert.Equal("   ", value.Data);
    }

    [Fact]
    public void From_StringWithSpecialCharacters_PreservesCharacters()
    {
        // Arrange
        var input = "<script>alert('xss')</script>";

        // Act
        var value = AttributeValue.From(input);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(input, value.Data);
    }

    #endregion

    #region Integer Values

    [Fact]
    public void From_Int32_CreatesIntegerType()
    {
        // Act
        var value = AttributeValue.From(42);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Integer, value.Type);
        Assert.Equal("42", value.Data);
    }

    [Fact]
    public void From_NegativeInt32_CreatesIntegerType()
    {
        // Act
        var value = AttributeValue.From(-42);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Integer, value.Type);
        Assert.Equal("-42", value.Data);
    }

    [Fact]
    public void From_ZeroInt32_CreatesIntegerType()
    {
        // Act
        var value = AttributeValue.From(0);

        // Assert
        Assert.NotNull(value);
        Assert.Equal("0", value.Data);
    }

    [Fact]
    public void From_UInt32_CreatesNonNegativeIntegerType()
    {
        // Act
        var value = AttributeValue.From((uint)42);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.NonNegative_Integer, value.Type);
    }

    [Fact]
    public void From_Int64_CreatesIntegerType()
    {
        // Act
        var value = AttributeValue.From(42L);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Integer, value.Type);
    }

    [Fact]
    public void From_UInt64_CreatesNonNegativeIntegerType()
    {
        // Act
        var value = AttributeValue.From((ulong)42);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.NonNegative_Integer, value.Type);
    }

    #endregion

    #region Floating Point Values

    [Fact]
    public void From_Double_CreatesFloatingPointType()
    {
        // Act
        var value = AttributeValue.From(3.14);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.FloatingPoint, value.Type);
    }

    [Fact]
    public void From_NegativeDouble_CreatesFloatingPointType()
    {
        // Act
        var value = AttributeValue.From(-3.14);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.FloatingPoint, value.Type);
        Assert.Equal("-3.14", value.Data);
    }

    [Fact]
    public void From_ZeroDouble_CreatesFloatingPointType()
    {
        // Act
        var value = AttributeValue.From(0.0);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.FloatingPoint, value.Type);
        Assert.Equal("0", value.Data);
    }

    #endregion

    #region Boolean Values

    [Fact]
    public void From_TrueBoolean_CreatesBooleanType()
    {
        // Act
        var value = AttributeValue.From(true);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Boolean, value.Type);
    }

    [Fact]
    public void From_FalseBoolean_CreatesBooleanType()
    {
        // Act
        var value = AttributeValue.From(false);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Boolean, value.Type);
    }

    #endregion

    #region Special Type Values

    [Fact]
    public void From_Length_CreatesLengthType()
    {
        // Act
        var value = AttributeValue.From_Length(100.0);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Length, value.Type);
        Assert.Equal("100", value.Data);
    }

    [Fact]
    public void From_Percent_CreatesPercentageType()
    {
        // Act
        var value = AttributeValue.From_Percent(50.0);

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Percentage, value.Type);
        Assert.Equal("50", value.Data);
    }

    #endregion

    #region Static Constants

    [Fact]
    public void Zero_IsNonNegativeIntegerZero()
    {
        // Act
        var value = AttributeValue.Zero;

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.NonNegative_Integer, value.Type);
        Assert.Equal("0", value.Data);
    }

    [Fact]
    public void One_IsNonNegativeIntegerOne()
    {
        // Act
        var value = AttributeValue.One;

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.NonNegative_Integer, value.Type);
        Assert.Equal("1", value.Data);
    }

    [Fact]
    public void NegativeOne_IsIntegerNegativeOne()
    {
        // Act
        var value = AttributeValue.NegativeOne;

        // Assert
        Assert.NotNull(value);
        Assert.Equal(EAttributeType.Integer, value.Type);
        Assert.Equal("-1", value.Data);
    }

    #endregion

    #region Value Retrieval

    [Fact]
    public void AsInt_ReturnsCorrectValue()
    {
        // Arrange
        var value = AttributeValue.From(42);

        // Act
        var result = value.AsInt();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void AsString_ReturnsCorrectValue()
    {
        // Arrange
        var value = AttributeValue.From("test");

        // Act
        var result = value.AsString();

        // Assert
        Assert.Equal("test", result);
    }

    [Fact]
    public void AsRAW_ReturnsBackingValue()
    {
        // Arrange
        var value = AttributeValue.From(42);

        // Act
        var result = value.AsRAW();

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(42, result);
    }

    #endregion

    #region Parse Tests

    [Fact]
    public void Parse_ValidInput_CreatesValue()
    {
        // Arrange
        var attrName = "data-test";

        // Act
        var value = AttributeValue.Parse(attrName, "test-value");

        // Assert
        Assert.NotNull(value);
    }

    [Fact]
    public void Parse_EmptyString_CreatesValue()
    {
        // Arrange
        var attrName = "data-test";

        // Act
        var value = AttributeValue.Parse(attrName, string.Empty);

        // Assert
        Assert.NotNull(value);
    }

    #endregion
}
