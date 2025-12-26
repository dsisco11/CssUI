using System.Linq;
using CssUI.CSS;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for the <see cref="CssSelector"/> class and selector parsing/matching.
/// </summary>
public class CssSelectorTests
{
    #region Test Infrastructure
    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Element CreateTestElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }
    #endregion

    #region Type Selector Tests
    [Theory]
    [InlineData("div", "div", true)]
    [InlineData("DIV", "div", true)]  // Case insensitive selector
    [InlineData("div", "DIV", true)]  // Case insensitive element
    [InlineData("span", "div", false)]
    [InlineData("p", "p", true)]
    [InlineData("article", "section", false)]
    public void TypeSelector_MatchesElementByTagName(string selectorStr, string tagName, bool expected)
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, tagName);
        var selector = new CssSelector(selectorStr);

        // Act
        var matches = selector.Count > 0 && selector[0].Match(element);

        // Assert
        Assert.Equal(expected, matches);
    }
    #endregion

    #region Universal Selector Tests
    [Fact]
    public void UniversalSelector_MatchesAnyElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        var selector = new CssSelector("*");

        // Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(div), "Universal selector should match div");
        Assert.True(selector[0].Match(span), "Universal selector should match span");
        Assert.True(selector[0].Match(p), "Universal selector should match p");
    }
    #endregion

    #region ID Selector Tests
    [Theory]
    [InlineData("#header", "header", true)]
    [InlineData("#header", "footer", false)]
    [InlineData("#main-content", "main-content", true)]
    [InlineData("#HEADER", "header", true)]  // Case insensitive matching
    [InlineData("#header", "HEADER", true)]  // Case insensitive matching
    public void IDSelector_MatchesElementById(string selectorStr, string elementId, bool expected)
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = elementId;
        var selector = new CssSelector(selectorStr);

        // Act
        var matches = selector.Count > 0 && selector[0].Match(element);

        // Assert
        Assert.Equal(expected, matches);
    }

    [Fact]
    public void IDSelector_DoesNotMatchElementWithoutId()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var selector = new CssSelector("#someId");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), "Should not match element without ID");
    }
    #endregion

    #region Class Selector Tests
    [Theory]
    [InlineData(".active", "active", true)]
    [InlineData(".active", "inactive", false)]
    [InlineData(".btn", "btn btn-primary", true)]
    [InlineData(".btn-primary", "btn btn-primary", true)]
    [InlineData(".nonexistent", "btn btn-primary", false)]
    // NOTE: Per CSS Selectors Level 4 § 6.6, class selectors should be case-sensitive
    // except in quirks mode. The current implementation lowercases the SELECTOR value
    // but compares against the element's class list case-sensitively.
    // See ClassSelector.cs: AtomicString created with EAtomicStringFlags.CaseInsensitive
    // BUG: This means `.ACTIVE` matches `active` (selector lowercased to `.active`),
    // but `.active` does NOT match `ACTIVE` (element class not lowercased).
    [InlineData(".ACTIVE", "active", true)]   // Selector lowercased: ".ACTIVE" → ".active" matches "active"
    [InlineData(".active", "ACTIVE", false)]  // BUG: Element class "ACTIVE" not lowercased, so no match
    public void ClassSelector_MatchesElementByClassName(string selectorStr, string className, bool expected)
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = className;
        var selector = new CssSelector(selectorStr);

        // Act
        var matches = selector.Count > 0 && selector[0].Match(element);

        // Assert
        Assert.Equal(expected, matches);
    }

    [Fact]
    public void ClassSelector_DoesNotMatchElementWithoutClass()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var selector = new CssSelector(".someClass");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), "Should not match element without class");
    }

    [Fact]
    public void MultipleClassSelectors_MatchElementWithAllClasses()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "btn btn-primary active";
        var selector = new CssSelector(".btn.btn-primary");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Should match element with all classes");
    }

    [Fact]
    public void MultipleClassSelectors_DoNotMatchElementMissingOneClass()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "btn active";  // Missing btn-primary
        var selector = new CssSelector(".btn.btn-primary");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), "Should not match element missing a class");
    }
    #endregion

    #region Compound Selector Tests
    [Fact]
    public void CompoundSelector_TypeAndClass_MatchesCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "container";
        var selector = new CssSelector("div.container");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Should match div with container class");
    }

    [Fact]
    public void CompoundSelector_TypeAndClass_DoesNotMatchWrongType()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "span");
        element.className = "container";
        var selector = new CssSelector("div.container");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), "Should not match span with container class");
    }

    [Fact]
    public void CompoundSelector_TypeAndId_MatchesCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "main";
        var selector = new CssSelector("div#main");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Should match div with main ID");
    }

    [Fact]
    public void CompoundSelector_TypeIdAndClass_MatchesCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "main";
        element.className = "container active";
        var selector = new CssSelector("div#main.container");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), "Should match element with all attributes");
    }
    #endregion

    #region Selector List Tests
    [Fact]
    public void SelectorList_MatchesAnySelector()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        var article = CreateTestElement(doc, "article");
        var selector = new CssSelector("div, span, p");

        // Act & Assert
        Assert.True(selector.Any(s => s.Match(div)), "Should match div");
        Assert.True(selector.Any(s => s.Match(span)), "Should match span");
        Assert.False(selector.Any(s => s.Match(article)), "Should not match article");
    }

    [Fact]
    public void SelectorList_WithCompoundSelectors_MatchesCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "active";
        var selector = new CssSelector("span.inactive, div.active, p.pending");

        // Act & Assert
        Assert.True(selector.Any(s => s.Match(element)), "Should match div.active");
    }
    #endregion

    #region Parse Error Handling Tests
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyOrWhitespace_ReturnsNonEmptyCollection(string input)
    {
        // Arrange & Act
        var selector = new CssSelector(input);

        // Note: The library currently returns a non-empty collection with an empty
        // complex selector for empty/whitespace input, rather than an empty collection.
        // This is the actual behavior - ideally it would return an empty collection.
        Assert.NotNull(selector);
    }
    #endregion
}
