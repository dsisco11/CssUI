using System;
using CssUI.CSS;
using CssUI.CSS.Selectors;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for CSS Selector serialization (ISpanFormattable implementation).
/// Verifies that selectors serialize to their canonical CSS string representation.
/// </summary>
/// <seealso href="https://drafts.csswg.org/cssom/#serialize-a-selector"/>
[Trait("Category", "Selector")]
[Trait("Category", "Serialization")]
public class SelectorSerializationTests
{
    #region Simple Selector Serialization

    [Theory]
    [InlineData("div", "div")]
    [InlineData("span", "span")]
    [InlineData("input", "input")]
    [InlineData("*", "*")]
    public void TypeSelector_SerializesToTagName(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(".foo", ".foo")]
    [InlineData(".bar", ".bar")]
    [InlineData(".my-class", ".my-class")]
    public void ClassSelector_SerializesWithDot(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("#foo", "#foo")]
    [InlineData("#main", "#main")]
    [InlineData("#my-id", "#my-id")]
    public void IdSelector_SerializesWithHash(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Attribute Selector Serialization

    // Note: Attribute selector parsing has known issues in CssParser.
    // When fixed, add tests for:
    // - [href], [disabled], [data-value] (presence selectors)
    // - [type="text"], [class="foo"] (value selectors)

    #endregion

    #region Pseudo-Class Selector Serialization

    [Theory]
    [InlineData(":hover", ":hover")]
    [InlineData(":focus", ":focus")]
    [InlineData(":active", ":active")]
    [InlineData(":visited", ":visited")]
    [InlineData(":link", ":link")]
    [InlineData(":first-child", ":first-child")]
    [InlineData(":last-child", ":last-child")]
    public void PseudoClassSelector_SerializesWithColon(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Pseudo-Element Selector Serialization

    // Note: Pseudo-element selector parsing requires type selector prefix in this parser.
    // When standalone pseudo-element parsing is supported, add tests for:
    // - ::before, ::after, ::first-line, ::first-letter

    #endregion

    #region Compound Selector Serialization

    [Theory]
    [InlineData("div.foo", "div.foo")]
    [InlineData("span.bar", "span.bar")]
    [InlineData("input#main", "input#main")]
    [InlineData("div.foo.bar", "div.foo.bar")]
    public void CompoundSelector_SerializesInOrder(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("div.foo:hover", "div.foo:hover")]
    [InlineData("a.link:visited", "a.link:visited")]
    public void CompoundSelector_WithPseudoClass_SerializesCorrectly(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Complex Selector Serialization (Combinators)

    [Theory]
    [InlineData("div span", "div span")]
    [InlineData("ul li", "ul li")]
    [InlineData("body div p", "body div p")]
    public void DescendantCombinator_SerializesWithSpace(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("div > span", "div > span")]
    [InlineData("ul > li", "ul > li")]
    [InlineData("body > div > p", "body > div > p")]
    public void ChildCombinator_SerializesWithGreaterThan(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("h1 + p", "h1 + p")]
    [InlineData("div + div", "div + div")]
    public void AdjacentSiblingCombinator_SerializesWithPlus(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("h1 ~ p", "h1 ~ p")]
    [InlineData("div ~ span", "div ~ span")]
    public void GeneralSiblingCombinator_SerializesWithTilde(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Selector List Serialization

    [Theory]
    [InlineData("div, span", "div, span")]
    [InlineData("h1, h2, h3", "h1, h2, h3")]
    [InlineData(".foo, .bar, .baz", ".foo, .bar, .baz")]
    public void SelectorList_SerializesWithCommas(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("div span, ul li", "div span, ul li")]
    [InlineData("div > span, ul > li", "div > span, ul > li")]
    public void SelectorList_WithComplexSelectors_SerializesCorrectly(string input, string expected)
    {
        // Arrange
        var selector = new CssSelector(input);

        // Act
        var result = selector.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region Round-Trip Tests

    [Theory]
    [InlineData("div")]
    [InlineData(".foo")]
    [InlineData("#bar")]
    [InlineData(":hover")]
    [InlineData("div.foo")]
    [InlineData("div span")]
    [InlineData("div > span")]
    [InlineData("div, span")]
    public void Selector_RoundTrip_ParsesAndSerializesToSameValue(string input)
    {
        // Arrange
        var selector1 = new CssSelector(input);
        var serialized = selector1.ToString();

        // Act - parse the serialized output
        var selector2 = new CssSelector(serialized);
        var reserialized = selector2.ToString();

        // Assert - serialization should be idempotent
        Assert.Equal(serialized, reserialized);
    }

    #endregion

    #region TryFormat Buffer Tests

    [Fact]
    public void TryFormat_WithSufficientBuffer_ReturnsTrue()
    {
        // Arrange
        var selector = new CssSelector("div.foo");
        Span<char> buffer = stackalloc char[64];

        // Act
        bool success = selector.TryFormat(buffer, out int charsWritten, default, null);

        // Assert
        Assert.True(success);
        Assert.True(charsWritten > 0);
        Assert.Equal("div.foo", new string(buffer[..charsWritten]));
    }

    [Fact]
    public void TryFormat_WithInsufficientBuffer_ReturnsFalse()
    {
        // Arrange
        var selector = new CssSelector("div.very-long-class-name-that-wont-fit");
        Span<char> buffer = stackalloc char[4]; // Too small

        // Act
        bool success = selector.TryFormat(buffer, out int charsWritten, default, null);

        // Assert
        Assert.False(success);
    }

    #endregion
}
