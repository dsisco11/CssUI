using System;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for <see cref="CssListValue"/> - the specialized CSS value type for lists.
/// </summary>
public class CssListValueTests
{
    #region Construction Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void From_WithMultipleValues_ReturnsCssListValue()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);

        // Act
        var result = CssValue.From(v1, v2);

        // Assert
        Assert.IsType<CssListValue>(result);
        Assert.Equal(ECssValueTypes.COLLECTION, result.Type);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void CssListValue_Count_ReturnsCorrectCount()
    {
        // Arrange
        var values = new[] { CssValue.From(1), CssValue.From(2), CssValue.From(3) };
        var listValue = (CssListValue)CssValue.From(values);

        // Act & Assert
        Assert.Equal(3, listValue.Count);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void CssListValue_Indexer_ReturnsCorrectValue()
    {
        // Arrange
        var v1 = CssValue.From(10);
        var v2 = CssValue.From(20);
        var v3 = CssValue.From(30);
        var listValue = (CssListValue)CssValue.From(v1, v2, v3);

        // Act & Assert
        Assert.Equal(10, listValue[0].AsInteger());
        Assert.Equal(20, listValue[1].AsInteger());
        Assert.Equal(30, listValue[2].AsInteger());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void CssListValue_Values_ReturnsSpan()
    {
        // Arrange
        var v1 = CssValue.From(1);
        var v2 = CssValue.From(2);
        var listValue = (CssListValue)CssValue.From(v1, v2);

        // Act
        var span = listValue.Values;

        // Assert
        Assert.Equal(2, span.Length);
        Assert.Equal(1, span[0].AsInteger());
        Assert.Equal(2, span[1].AsInteger());
    }

    #endregion

    #region HasValue Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void HasValue_WithValues_ReturnsTrue()
    {
        // Arrange
        var listValue = (CssListValue)CssValue.From(CssValue.From(1));

        // Act & Assert
        Assert.True(listValue.HasValue);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Serialize_SpaceSeparated_JoinsWithSpaces()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);
        var listValue = (CssListValue)CssValue.From(v1, v2);

        // Act
        var result = listValue.Serialize();

        // Assert
        Assert.Equal("10px 20px", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Serialize_CommaSeparated_JoinsWithCommas()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);
        var listValue = (CssListValue)CssValue.FromList(ECssListSeparator.Comma, v1, v2);

        // Act
        var result = listValue.Serialize();

        // Assert
        Assert.Equal("10px, 20px", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Serialize_SlashSeparated_JoinsWithSlashes()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);
        var listValue = (CssListValue)CssValue.FromList(ECssListSeparator.Slash, v1, v2);

        // Act
        var result = listValue.Serialize();

        // Assert
        Assert.Equal("10px / 20px", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Serialize_SingleValue_ReturnsValueOnly()
    {
        // Arrange
        var listValue = (CssListValue)CssValue.From(CssValue.From(42));

        // Act
        var result = listValue.Serialize();

        // Assert
        Assert.Equal("42", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void ToString_ReturnsSerializedValue()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.Auto;
        var listValue = (CssListValue)CssValue.From(v1, v2);

        // Act
        var result = listValue.ToString();

        // Assert
        Assert.Equal("10px auto", result);
    }

    #endregion

    #region TryFormat Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void TryFormat_SpaceSeparated_FormatsCorrectly()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);
        var listValue = (CssListValue)CssValue.From(v1, v2);
        Span<char> buffer = stackalloc char[32];

        // Act
        var success = listValue.TryFormat(buffer, out int charsWritten);

        // Assert
        Assert.True(success);
        Assert.Equal("10px 20px", buffer.Slice(0, charsWritten).ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void TryFormat_CommaSeparated_FormatsCorrectly()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);
        var listValue = (CssListValue)CssValue.FromList(ECssListSeparator.Comma, v1, v2);
        Span<char> buffer = stackalloc char[32];

        // Act
        var success = listValue.TryFormat(buffer, out int charsWritten);

        // Assert
        Assert.True(success);
        Assert.Equal("10px, 20px", buffer.Slice(0, charsWritten).ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        // Arrange
        var v1 = CssValue.From_Dimension(10, ECssUnit.PX);
        var v2 = CssValue.From_Dimension(20, ECssUnit.PX);
        var listValue = (CssListValue)CssValue.From(v1, v2);
        Span<char> buffer = stackalloc char[5]; // Too small for "10px 20px"

        // Act
        var success = listValue.TryFormat(buffer, out int charsWritten);

        // Assert
        Assert.False(success);
    }

    #endregion

    #region AsCollection Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void AsCollection_ReturnsCachedCollection()
    {
        // Arrange
        var v1 = CssValue.From(1);
        var v2 = CssValue.From(2);
        var listValue = (CssListValue)CssValue.From(v1, v2);

        // Act
        var collection1 = listValue.AsCollection();
        var collection2 = listValue.AsCollection();

        // Assert
        Assert.Same(collection1, collection2); // Should be cached
        Assert.Equal(2, collection1.Count);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var list1 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(2));
        var list2 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(2));

        // Act & Assert
        Assert.Equal(list1, list2);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var list1 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(2));
        var list2 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(3));

        // Act & Assert
        Assert.NotEqual(list1, list2);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void Equals_DifferentLengths_ReturnsFalse()
    {
        // Arrange
        var list1 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(2));
        var list2 = (CssListValue)CssValue.From(CssValue.From(1));

        // Act & Assert
        Assert.NotEqual(list1, list2);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        // Arrange
        var list1 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(2));
        var list2 = (CssListValue)CssValue.From(CssValue.From(1), CssValue.From(2));

        // Act & Assert
        Assert.Equal(list1.GetHashCode(), list2.GetHashCode());
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void MixedValueTypes_SerializesCorrectly()
    {
        // Arrange - simulate a typical margin/padding value
        var values = new[]
        {
            CssValue.From_Dimension(10, ECssUnit.PX),
            CssValue.Auto,
            CssValue.From_Dimension(5, ECssUnit.PX),
            CssValue.Auto
        };
        var listValue = (CssListValue)CssValue.From(values);

        // Act
        var result = listValue.Serialize();

        // Assert
        Assert.Equal("10px auto 5px auto", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "CssListValue")]
    public void NestedList_SerializesCorrectly()
    {
        // Arrange - a list containing a list (e.g., for complex shorthand properties)
        var innerList = CssValue.From(CssValue.From(1), CssValue.From(2));
        var outerList = (CssListValue)CssValue.From(innerList, CssValue.From(3));

        // Act
        var result = outerList.Serialize();

        // Assert
        // The inner list serializes with spaces, outer adds another value
        Assert.Equal("1 2 3", result);
    }

    #endregion
}
