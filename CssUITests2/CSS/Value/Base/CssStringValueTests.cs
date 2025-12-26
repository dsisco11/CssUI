using System;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for <see cref="CssStringValue"/> - the specialized CSS value type for strings.
/// </summary>
public class CssStringValueTests
{
    #region Factory Method Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void From_String_ReturnsCssStringValue()
    {
        var value = CssValue.From_String("hello");

        Assert.IsType<CssStringValue>(value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void From_String_SetsTypeToSTRING()
    {
        var value = CssValue.From_String("test");

        Assert.Equal(ECssValueTypes.STRING, value.Type);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void From_String_HasValueReturnsTrue()
    {
        var value = CssValue.From_String("test");

        Assert.True(value.HasValue);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void From_String_EmptyString_HasValueReturnsTrue()
    {
        var value = CssValue.From_String(string.Empty);

        Assert.True(value.HasValue);
    }

    #endregion

    #region Value Access Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Value_ReturnsStoredString()
    {
        var stringValue = (CssStringValue)CssValue.From_String("hello world");

        Assert.Equal("hello world", stringValue.Value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void AsString_ReturnsStoredString()
    {
        var value = CssValue.From_String("test string");

        Assert.Equal("test string", value.AsString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void AsString_EmptyString_ReturnsEmpty()
    {
        var value = CssValue.From_String(string.Empty);

        Assert.Equal(string.Empty, value.AsString());
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void ToString_ReturnsQuotedString()
    {
        var value = CssValue.From_String("hello");

        Assert.Equal("\"hello\"", value.ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void ToString_EmptyString_ReturnsEmptyQuoted()
    {
        var value = CssValue.From_String(string.Empty);

        Assert.Equal("\"\"", value.ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_ReturnsQuotedString()
    {
        var value = CssValue.From_String("test");

        Assert.Equal("\"test\"", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_WithSpaces_PreservesSpaces()
    {
        var value = CssValue.From_String("hello world");

        Assert.Equal("\"hello world\"", value.Serialize());
    }

    #endregion

    #region Escaping Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_WithDoubleQuote_EscapesQuote()
    {
        var value = CssValue.From_String("say \"hello\"");

        var serialized = value.Serialize();

        Assert.Equal("\"say \\\"hello\\\"\"", serialized);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_WithBackslash_EscapesBackslash()
    {
        var value = CssValue.From_String("path\\to\\file");

        var serialized = value.Serialize();

        Assert.Equal("\"path\\\\to\\\\file\"", serialized);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_WithNewline_EscapesAsHex()
    {
        var value = CssValue.From_String("line1\nline2");

        var serialized = value.Serialize();

        // CSS escapes newline as \a followed by space
        Assert.Equal("\"line1\\a line2\"", serialized);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_WithCarriageReturn_EscapesAsHex()
    {
        var value = CssValue.From_String("line1\rline2");

        var serialized = value.Serialize();

        // CSS escapes CR as \d followed by space
        Assert.Equal("\"line1\\d line2\"", serialized);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Serialize_WithFormFeed_EscapesAsHex()
    {
        var value = CssValue.From_String("page1\fpage2");

        var serialized = value.Serialize();

        // CSS escapes FF as \c followed by space
        Assert.Equal("\"page1\\c page2\"", serialized);
    }

    #endregion

    #region TryFormat Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    [Trait("Category", "Formatting")]
    public void TryFormat_SimpleString_WritesQuotedString()
    {
        var value = CssValue.From_String("hello");
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("\"hello\"", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    [Trait("Category", "Formatting")]
    public void TryFormat_EmptyString_WritesEmptyQuoted()
    {
        var value = CssValue.From_String(string.Empty);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("\"\"", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    [Trait("Category", "Formatting")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        var value = CssValue.From_String("hello");
        Span<char> buffer = stackalloc char[3]; // Too small for "hello" + quotes

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    [Trait("Category", "Formatting")]
    public void TryFormat_ExactSize_Succeeds()
    {
        var value = CssValue.From_String("hi");
        Span<char> buffer = stackalloc char[4]; // Exactly "hi" + 2 quotes

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal(4, charsWritten);
        Assert.Equal("\"hi\"", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    [Trait("Category", "Formatting")]
    public void TryFormat_WithEscaping_WritesEscapedString()
    {
        var value = CssValue.From_String("say \"hi\"");
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("\"say \\\"hi\\\"\"", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Equals_SameValue_ReturnsTrue()
    {
        var value1 = CssValue.From_String("test");
        var value2 = CssValue.From_String("test");

        Assert.True(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var value1 = CssValue.From_String("hello");
        var value2 = CssValue.From_String("world");

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Equals_CaseSensitive()
    {
        var value1 = CssValue.From_String("Hello");
        var value2 = CssValue.From_String("hello");

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Equals_EmptyStrings_ReturnsTrue()
    {
        var value1 = CssValue.From_String(string.Empty);
        var value2 = CssValue.From_String(string.Empty);

        Assert.True(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void Equals_Null_ReturnsFalse()
    {
        var value = CssValue.From_String("test");

        Assert.False(value.Equals(null));
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        var value1 = CssValue.From_String("test");
        var value2 = CssValue.From_String("test");

        Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void GetHashCode_DifferentValue_ReturnsDifferentHash()
    {
        var value1 = CssValue.From_String("hello");
        var value2 = CssValue.From_String("world");

        // Different strings should typically have different hashes
        // (not guaranteed but highly likely for different strings)
        Assert.NotEqual(value1.GetHashCode(), value2.GetHashCode());
    }

    #endregion

    #region Edge Cases

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void UnicodeString_PreservesContent()
    {
        var value = CssValue.From_String("Hello 世界 🌍");

        Assert.Equal("Hello 世界 🌍", value.AsString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void UnicodeString_SerializesCorrectly()
    {
        var value = CssValue.From_String("café");

        Assert.Equal("\"café\"", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void LongString_HandlesCorrectly()
    {
        var longString = new string('x', 10000);
        var value = CssValue.From_String(longString);

        Assert.Equal(longString, value.AsString());
        Assert.StartsWith("\"", value.Serialize());
        Assert.EndsWith("\"", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void ContentProperty_TypicalUsage()
    {
        // Common use case: CSS content property
        var value = CssValue.From_String("→");

        Assert.Equal("\"→\"", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "String")]
    public void FontFamilyQuoted_TypicalUsage()
    {
        // Common use case: quoted font family name
        var value = CssValue.From_String("Times New Roman");

        Assert.Equal("\"Times New Roman\"", value.Serialize());
    }

    #endregion
}
