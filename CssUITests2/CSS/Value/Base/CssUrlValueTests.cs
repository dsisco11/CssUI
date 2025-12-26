using System;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for <see cref="CssUrlValue"/> - the specialized CSS value type for URLs.
/// </summary>
public class CssUrlValueTests
{
    #region Factory Method Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void From_CssUrl_ReturnsCssUrlValue()
    {
        var url = new CssUrl("image.png");
        var value = CssValue.From(url);

        Assert.IsType<CssUrlValue>(value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void From_Url_String_ReturnsCssUrlValue()
    {
        var value = CssValue.From_Url("image.png");

        Assert.IsType<CssUrlValue>(value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void From_Url_SetsTypeToURL()
    {
        var value = CssValue.From_Url("test.png");

        Assert.Equal(ECssValueTypes.URL, value.Type);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void From_Url_HasValueReturnsTrue()
    {
        var value = CssValue.From_Url("test.png");

        Assert.True(value.HasValue);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void From_EmptyUrl_HasValueReturnsFalse()
    {
        var value = CssValue.From_Url(string.Empty);

        Assert.False(value.HasValue);
    }

    #endregion

    #region Value Access Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Value_ReturnsStoredCssUrl()
    {
        var urlValue = (CssUrlValue)CssValue.From_Url("path/to/image.png");

        Assert.Equal("path/to/image.png", urlValue.Value.Value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void AsUrl_ReturnsStoredCssUrl()
    {
        var value = CssValue.From_Url("test.png");

        var url = value.AsUrl();

        Assert.Equal("test.png", url.Value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void AsUrl_FromCssUrl_ReturnsOriginal()
    {
        var original = new CssUrl("original.png");
        var value = CssValue.From(original);

        var retrieved = value.AsUrl();

        Assert.Equal(original, retrieved);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void ToString_ReturnsUrlFunction()
    {
        var value = CssValue.From_Url("image.png");

        Assert.Equal("url(\"image.png\")", value.ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Serialize_ReturnsUrlFunction()
    {
        var value = CssValue.From_Url("test.png");

        Assert.Equal("url(\"test.png\")", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Serialize_WithPath_PreservesPath()
    {
        var value = CssValue.From_Url("images/icons/test.png");

        Assert.Equal("url(\"images/icons/test.png\")", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Serialize_AbsoluteUrl_PreservesUrl()
    {
        var value = CssValue.From_Url("https://example.com/image.png");

        Assert.Equal("url(\"https://example.com/image.png\")", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Serialize_DataUri_PreservesUri()
    {
        var value = CssValue.From_Url("data:image/png;base64,ABC123");

        Assert.Equal("url(\"data:image/png;base64,ABC123\")", value.Serialize());
    }

    #endregion

    #region TryFormat Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    [Trait("Category", "Formatting")]
    public void TryFormat_SimpleUrl_WritesUrlFunction()
    {
        var value = CssValue.From_Url("image.png");
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        var result = buffer[..charsWritten].ToString();
        Assert.StartsWith("url(", result);
        Assert.Contains("image.png", result);
        Assert.EndsWith(")", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    [Trait("Category", "Formatting")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        var value = CssValue.From_Url("image.png");
        Span<char> buffer = stackalloc char[3]; // Definitely too small for url("image.png")

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.False(success);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    [Trait("Category", "Formatting")]
    public void TryFormat_LargeBuffer_Succeeds()
    {
        var value = CssValue.From_Url("x.png");
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.True(charsWritten > 0);
        var result = buffer[..charsWritten].ToString();
        Assert.StartsWith("url(", result);
        Assert.Contains("x.png", result);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Equals_SameUrl_ReturnsTrue()
    {
        var value1 = CssValue.From_Url("test.png");
        var value2 = CssValue.From_Url("test.png");

        Assert.True(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Equals_DifferentUrl_ReturnsFalse()
    {
        var value1 = CssValue.From_Url("image1.png");
        var value2 = CssValue.From_Url("image2.png");

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Equals_CaseSensitive()
    {
        var value1 = CssValue.From_Url("Image.PNG");
        var value2 = CssValue.From_Url("image.png");

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void Equals_Null_ReturnsFalse()
    {
        var value = CssValue.From_Url("test.png");

        Assert.False(value.Equals(null));
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void GetHashCode_SameUrl_ReturnsSameHash()
    {
        var value1 = CssValue.From_Url("test.png");
        var value2 = CssValue.From_Url("test.png");

        Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void GetHashCode_DifferentUrl_ReturnsDifferentHash()
    {
        var value1 = CssValue.From_Url("image1.png");
        var value2 = CssValue.From_Url("image2.png");

        Assert.NotEqual(value1.GetHashCode(), value2.GetHashCode());
    }

    #endregion

    #region Edge Cases

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void EmptyUrl_SerializesCorrectly()
    {
        var value = CssValue.From_Url(string.Empty);

        Assert.Equal("url(\"\")", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void UrlWithSpaces_SerializesCorrectly()
    {
        var value = CssValue.From_Url("path/to/my image.png");

        var serialized = value.Serialize();

        Assert.StartsWith("url(\"", serialized);
        Assert.EndsWith("\")", serialized);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void UrlWithQueryString_PreservesQuery()
    {
        var value = CssValue.From_Url("image.png?v=123&size=large");

        Assert.Contains("?v=123&size=large", value.AsUrl().Value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void UrlWithFragment_PreservesFragment()
    {
        var value = CssValue.From_Url("sprite.svg#icon-home");

        Assert.Contains("#icon-home", value.AsUrl().Value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void BackgroundImage_TypicalUsage()
    {
        // Common use case: background-image property
        var value = CssValue.From_Url("../images/bg.jpg");

        Assert.Equal("url(\"../images/bg.jpg\")", value.Serialize());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "URL")]
    public void FontFace_TypicalUsage()
    {
        // Common use case: @font-face src property
        var value = CssValue.From_Url("fonts/myfont.woff2");

        Assert.Equal("url(\"fonts/myfont.woff2\")", value.Serialize());
    }

    #endregion
}
