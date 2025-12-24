using System;
using CssUI.CSS;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUI.Tests.CSS.Parser;

/// <summary>
/// Tests for URL parsing in CssParser.
/// Spec Reference: https://www.w3.org/TR/css-syntax-3/#url-token-diagram
/// </summary>
[Trait("Category", "CSS")]
[Trait("Category", "URL")]
public class CssUrlParsingTests
{
    private static CssValue ParseValue(string css)
    {
        var parser = new CssParser(css);
        return parser.Parse_CssValue();
    }

    #region Unquoted URL Token Tests (url-token)

    [Fact]
    public void Parse_UrlToken_SimpleUrl_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(example.png)");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("example.png", url.Value);
    }

    [Fact]
    public void Parse_UrlToken_RelativePath_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(../images/logo.png)");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("../images/logo.png", url.Value);
    }

    [Fact]
    public void Parse_UrlToken_AbsoluteUrl_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(https://example.com/image.png)");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("https://example.com/image.png", url.Value);
    }

    [Fact]
    public void Parse_UrlToken_DataUri_ReturnsUrlValue()
    {
        // Arrange & Act
        // Data URIs without quotes should work
        var value = ParseValue("url(data:image/png;base64,ABC123)");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("data:image/png;base64,ABC123", url.Value);
    }

    [Fact]
    public void Parse_UrlToken_WithWhitespace_ReturnsUrlValue()
    {
        // Per CSS Syntax Level 3, whitespace before and after is consumed
        // Arrange & Act
        var value = ParseValue("url(  example.png  )");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("example.png", url.Value);
    }

    [Fact]
    public void Parse_UrlToken_EmptyUrl_ReturnsEmptyUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url()");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.True(url.IsEmpty);
    }

    #endregion

    #region Quoted URL Function Tests (function-token)

    [Fact]
    public void Parse_UrlFunction_DoubleQuoted_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(\"example.png\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("example.png", url.Value);
    }

    [Fact]
    public void Parse_UrlFunction_SingleQuoted_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url('example.png')");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("example.png", url.Value);
    }

    [Fact]
    public void Parse_UrlFunction_QuotedWithSpaces_ReturnsUrlValue()
    {
        // Quoted URLs can contain spaces in the filename
        // Arrange & Act
        var value = ParseValue("url(\"my image.png\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("my image.png", url.Value);
    }

    [Fact]
    public void Parse_UrlFunction_QuotedWithSpecialChars_ReturnsUrlValue()
    {
        // Quoted URLs can contain special characters
        // Arrange & Act
        var value = ParseValue("url(\"path/to/file(1).png\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("path/to/file(1).png", url.Value);
    }

    [Fact]
    public void Parse_UrlFunction_QuotedAbsoluteUrl_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(\"https://example.com/path/image.png\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("https://example.com/path/image.png", url.Value);
    }

    [Fact]
    public void Parse_UrlFunction_QuotedDataUri_ReturnsUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(\"data:image/svg+xml,%3Csvg%3E%3C/svg%3E\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("data:image/svg+xml,%3Csvg%3E%3C/svg%3E", url.Value);
    }

    [Fact]
    public void Parse_UrlFunction_EmptyQuoted_ReturnsEmptyUrlValue()
    {
        // Arrange & Act
        var value = ParseValue("url(\"\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.True(url.IsEmpty);
    }

    #endregion

    #region CssUrl Type Tests

    [Fact]
    public void CssUrl_FromString_CreatesValidUrl()
    {
        // Arrange & Act
        var url = new CssUrl("test.png");

        // Assert
        Assert.Equal("test.png", url.Value);
        Assert.False(url.IsEmpty);
    }

    [Fact]
    public void CssUrl_Empty_IsEmptyTrue()
    {
        // Arrange & Act
        var url = CssUrl.Empty;

        // Assert
        Assert.True(url.IsEmpty);
        Assert.Equal(string.Empty, url.Value);
    }

    [Fact]
    public void CssUrl_ToCssString_ReturnsQuotedFormat()
    {
        // Arrange
        var url = new CssUrl("example.png");

        // Act
        var cssString = url.ToCssString();

        // Assert
        Assert.Equal("url(\"example.png\")", cssString);
    }

    [Fact]
    public void CssUrl_ToCssString_EscapesBackslash()
    {
        // Arrange
        var url = new CssUrl("path\\to\\file.png");

        // Act
        var cssString = url.ToCssString();

        // Assert
        Assert.Equal("url(\"path\\\\to\\\\file.png\")", cssString);
    }

    [Fact]
    public void CssUrl_ToCssString_EscapesQuotes()
    {
        // Arrange
        var url = new CssUrl("file\"name.png");

        // Act
        var cssString = url.ToCssString();

        // Assert
        Assert.Equal("url(\"file\\\"name.png\")", cssString);
    }

    [Fact]
    public void CssUrl_Equality_SameValue_AreEqual()
    {
        // Arrange
        var url1 = new CssUrl("test.png");
        var url2 = new CssUrl("test.png");

        // Assert
        Assert.Equal(url1, url2);
        Assert.True(url1.Equals(url2));
    }

    [Fact]
    public void CssUrl_Equality_DifferentValue_AreNotEqual()
    {
        // Arrange
        var url1 = new CssUrl("test1.png");
        var url2 = new CssUrl("test2.png");

        // Assert
        Assert.NotEqual(url1, url2);
        Assert.False(url1.Equals(url2));
    }

    [Fact]
    public void CssUrl_ImplicitConversion_FromString()
    {
        // Arrange & Act
        CssUrl url = "test.png";

        // Assert
        Assert.Equal("test.png", url.Value);
    }

    [Fact]
    public void CssUrl_ImplicitConversion_ToString()
    {
        // Arrange
        var url = new CssUrl("test.png");

        // Act
        string value = url;

        // Assert
        Assert.Equal("test.png", value);
    }

    #endregion

    #region CssValue URL Factory Method Tests

    [Fact]
    public void CssValue_FromCssUrl_CreatesUrlValue()
    {
        // Arrange
        var url = new CssUrl("image.png");

        // Act
        var value = CssValue.From(url);

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
    }

    [Fact]
    public void CssValue_FromUrl_CreatesUrlValue()
    {
        // Act
        var value = CssValue.From_Url("image.png");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Parse_BackgroundImage_UrlValue()
    {
        // Test URL in a property-like context
        // Arrange & Act
        var value = ParseValue("url(background.jpg)");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("background.jpg", url.Value);
    }

    [Fact]
    public void Parse_FontSrc_UrlValue()
    {
        // Test URL for @font-face src
        // Arrange & Act
        var value = ParseValue("url(\"fonts/myfont.woff2\")");

        // Assert
        Assert.Equal(ECssValueTypes.URL, value.Type);
        var url = value.AsUrl();
        Assert.Equal("fonts/myfont.woff2", url.Value);
    }

    #endregion
}
