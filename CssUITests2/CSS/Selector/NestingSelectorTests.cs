using System;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Selectors;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for the CSS Nesting Selector (&amp;).
/// See: https://www.w3.org/TR/css-nesting-1/#nest-selector
/// </summary>
public class NestingSelectorTests
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

    private static Element CreateElementWithClass(Document doc, string tagName, string className)
    {
        var element = CreateTestElement(doc, tagName);
        element.className = className;
        return element;
    }

    private static Element CreateElementWithId(Document doc, string tagName, string id)
    {
        var element = CreateTestElement(doc, tagName);
        element.id = id;
        return element;
    }
    #endregion

    #region Parsing Tests
    [Fact]
    public void NestingSelector_ParsesSingleAmpersand()
    {
        // Arrange & Act
        var selector = new CssSelector("&");

        // Assert
        Assert.NotEmpty(selector);
        Assert.Single(selector);
        Assert.Single(selector[0]); // One RelativeSelector
        Assert.Single(selector[0][0]); // One SimpleSelector (NestingSelector)
        Assert.IsType<NestingSelector>(selector[0][0][0]);
    }

    [Fact]
    public void NestingSelector_ParsesWithPseudoClass()
    {
        // Arrange & Act - &:hover should parse as nesting selector followed by pseudo-class
        var selector = new CssSelector("&:hover");

        // Assert
        Assert.NotEmpty(selector);
        Assert.Single(selector);
        Assert.Equal(2, selector[0][0].Count); // Two simple selectors: & and :hover
        Assert.IsType<NestingSelector>(selector[0][0][0]);
        Assert.IsType<PseudoClassSelector>(selector[0][0][1]);
    }

    [Fact]
    public void NestingSelector_ParsesWithClassSelector()
    {
        // Arrange & Act - &.foo should parse as nesting selector followed by class
        var selector = new CssSelector("&.foo");

        // Assert
        Assert.NotEmpty(selector);
        Assert.Equal(2, selector[0][0].Count);
        Assert.IsType<NestingSelector>(selector[0][0][0]);
        Assert.IsType<ClassSelector>(selector[0][0][1]);
    }

    [Fact]
    public void NestingSelector_ParsesAtEndOfCompound()
    {
        // Arrange & Act - .foo& should parse (per CSS Nesting spec, & can appear anywhere)
        // Note: This is valid per spec but may put & after class in compound
        var selector = new CssSelector(".foo&");

        // Assert
        Assert.NotEmpty(selector);
        // The order may vary based on parser implementation
        var simpleSelectors = selector[0][0];
        Assert.Contains(simpleSelectors, s => s is NestingSelector);
        Assert.Contains(simpleSelectors, s => s is ClassSelector);
    }

    [Fact]
    public void NestingSelector_ParsesInCombinedSelector()
    {
        // Arrange & Act - "& > .bar" should parse
        var selector = new CssSelector("& > .bar");

        // Assert
        Assert.NotEmpty(selector);
        Assert.Equal(2, selector[0].Count); // Two RelativeSelectors: "&" and "> .bar"
        Assert.IsType<NestingSelector>(selector[0][0][0]);
    }

    [Fact]
    public void NestingSelector_ParsesWithDescendantCombinator()
    {
        // Arrange & Act - "& div" (descendant combinator)
        var selector = new CssSelector("& div");

        // Assert
        Assert.NotEmpty(selector);
        Assert.Equal(2, selector[0].Count); // & and div with descendant combinator between
    }

    #endregion

    #region Serialization Tests
    [Fact]
    public void NestingSelector_SerializesAsAmpersand()
    {
        // Arrange
        var selector = new NestingSelector();

        // Act
        var serialized = selector.ToString();

        // Assert
        Assert.Equal("&", serialized);
    }

    [Fact]
    public void NestingSelector_TryFormatSucceeds()
    {
        // Arrange
        var selector = new NestingSelector();
        Span<char> buffer = stackalloc char[10];

        // Act
        bool success = selector.TryFormat(buffer, out int charsWritten, ReadOnlySpan<char>.Empty, null);

        // Assert
        Assert.True(success);
        Assert.Equal(1, charsWritten);
        Assert.Equal('&', buffer[0]);
    }

    [Fact]
    public void NestingSelector_TryFormatFailsWithSmallBuffer()
    {
        // Arrange
        var selector = new NestingSelector();
        Span<char> buffer = stackalloc char[0];

        // Act
        bool success = selector.TryFormat(buffer, out int charsWritten, ReadOnlySpan<char>.Empty, null);

        // Assert
        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }
    #endregion

    #region Matching Tests - Without Parent Selector
    [Fact]
    public void NestingSelector_WithoutParent_BehavesLikeScope()
    {
        // Arrange - When no parent selector is set, & behaves like :scope
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var nestingSelector = new NestingSelector();

        // Act - Pass the element as scope
        bool matches = nestingSelector.Matches(element, element);

        // Assert
        Assert.True(matches, "Nesting selector without parent should match scope elements");
    }

    [Fact]
    public void NestingSelector_WithoutParent_DoesNotMatchNonScopeElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var scopeElement = CreateTestElement(doc, "div");
        var otherElement = CreateTestElement(doc, "span");
        var nestingSelector = new NestingSelector();

        // Act
        bool matches = nestingSelector.Matches(otherElement, scopeElement);

        // Assert
        Assert.False(matches, "Nesting selector should not match non-scope elements");
    }
    #endregion

    #region Matching Tests - With Parent Selector
    [Fact]
    public void NestingSelector_WithParent_MatchesParentSelectorElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateElementWithClass(doc, "div", "foo");
        var parentSelector = new CssSelector(".foo");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act
        bool matches = nestingSelector.Matches(element);

        // Assert
        Assert.True(matches, "Nesting selector should match elements matched by parent selector");
    }

    [Fact]
    public void NestingSelector_WithParent_DoesNotMatchNonMatchingElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateElementWithClass(doc, "div", "bar");
        var parentSelector = new CssSelector(".foo");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act
        bool matches = nestingSelector.Matches(element);

        // Assert
        Assert.False(matches, "Nesting selector should not match elements not matched by parent");
    }

    [Fact]
    public void NestingSelector_SetParentSelector_UpdatesMatching()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fooElement = CreateElementWithClass(doc, "div", "foo");
        var barElement = CreateElementWithClass(doc, "div", "bar");
        var nestingSelector = new NestingSelector();

        // Initially, & behaves like :scope - neither matches without scope
        Assert.False(nestingSelector.Matches(fooElement));

        // Act - Set parent selector
        var parentSelector = new CssSelector(".foo");
        nestingSelector.SetParentSelector(parentSelector);

        // Assert
        Assert.True(nestingSelector.Matches(fooElement), "Should match .foo element after setting parent");
        Assert.False(nestingSelector.Matches(barElement), "Should not match .bar element");
    }
    #endregion

    #region Specificity Tests
    [Fact]
    public void NestingSelector_WithoutParent_HasScopeSpecificity()
    {
        // Arrange - :scope has specificity (0,1,0)
        var nestingSelector = new NestingSelector();

        // Act
        long specificity = nestingSelector.GetSpecificity();

        // Assert - Should have B=1 component (pseudo-class specificity)
        long expectedB = 1L << 16;
        Assert.Equal(expectedB, specificity);
    }

    [Fact]
    public void NestingSelector_WithIdParent_InheritsIdSpecificity()
    {
        // Arrange - #id has specificity (1,0,0)
        var parentSelector = new CssSelector("#myId");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act
        long specificity = nestingSelector.GetSpecificity();

        // Assert - Should have A=1 component
        long expectedA = 1L << 32;
        Assert.Equal(expectedA, specificity);
    }

    [Fact]
    public void NestingSelector_WithClassParent_InheritsClassSpecificity()
    {
        // Arrange - .class has specificity (0,1,0)
        var parentSelector = new CssSelector(".myClass");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act
        long specificity = nestingSelector.GetSpecificity();

        // Assert - Should have B=1 component
        long expectedB = 1L << 16;
        Assert.Equal(expectedB, specificity);
    }

    [Fact]
    public void NestingSelector_WithTypeParent_InheritsTypeSpecificity()
    {
        // Arrange - div has specificity (0,0,1)
        var parentSelector = new CssSelector("div");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act
        long specificity = nestingSelector.GetSpecificity();

        // Assert - Should have C=1 component
        Assert.Equal(1L, specificity);
    }

    [Fact]
    public void NestingSelector_WithMultipleSelectorParent_UsesMaxSpecificity()
    {
        // Arrange - "#id, .class" has max specificity from #id (1,0,0)
        var parentSelector = new CssSelector("#myId, .myClass");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act
        long specificity = nestingSelector.GetSpecificity();

        // Assert - Should use the higher specificity (from #id)
        long expectedA = 1L << 32;
        Assert.Equal(expectedA, specificity);
    }

    [Fact]
    public void NestingSelector_SpecificityIsCached()
    {
        // Arrange
        var parentSelector = new CssSelector("#myId");
        var nestingSelector = new NestingSelector(parentSelector);

        // Act - Call GetSpecificity multiple times
        long spec1 = nestingSelector.GetSpecificity();
        long spec2 = nestingSelector.GetSpecificity();
        long spec3 = nestingSelector.GetSpecificity();

        // Assert - All should return the same value
        Assert.Equal(spec1, spec2);
        Assert.Equal(spec2, spec3);
    }

    [Fact]
    public void NestingSelector_SpecificityResetOnParentChange()
    {
        // Arrange
        var nestingSelector = new NestingSelector();
        long initialSpec = nestingSelector.GetSpecificity(); // :scope specificity (0,1,0)

        // Act - Set parent with different specificity
        var idParent = new CssSelector("#myId");
        nestingSelector.SetParentSelector(idParent);
        long newSpec = nestingSelector.GetSpecificity();

        // Assert
        Assert.NotEqual(initialSpec, newSpec);
        Assert.Equal(1L << 32, newSpec); // ID specificity
    }
    #endregion

    #region Complex Selector Integration Tests
    [Fact]
    public void ComplexSelector_WithNestingSelector_CalculatesSpecificityCorrectly()
    {
        // Arrange - Parse "&:hover" which should add nesting specificity + :hover specificity
        var selector = new CssSelector("&:hover");
        Assert.NotEmpty(selector);

        // Act - Get specificity (nesting selector without parent = (0,1,0), :hover = (0,1,0))
        long specificity = selector[0].Get_Specificity();

        // Assert - Should be (0,2,0) = 2 * (1 << 16) = 131072
        long expectedB = 2L << 16;
        Assert.Equal(expectedB, specificity);
    }

    [Fact]
    public void ComplexSelector_WithNestingAndType_CalculatesSpecificityCorrectly()
    {
        // Arrange - Parse "& div" = nesting selector (0,1,0) + type (0,0,1) = (0,1,1)
        var selector = new CssSelector("& div");
        Assert.NotEmpty(selector);

        // Act
        long specificity = selector[0].Get_Specificity();

        // Assert - (0,1,1) = (1 << 16) + 1 = 65537
        long expected = (1L << 16) + 1;
        Assert.Equal(expected, specificity);
    }
    #endregion

    #region Enum Type Tests
    [Fact]
    public void NestingSelector_HasCorrectType()
    {
        // Arrange
        var selector = new NestingSelector();

        // Act & Assert
        Assert.Equal(ESimpleSelectorType.NestingSelector, selector.Type);
    }
    #endregion
}
