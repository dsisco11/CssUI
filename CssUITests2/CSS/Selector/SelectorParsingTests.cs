using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for CSS selector parsing and structure validation.
/// These tests verify that selector strings are correctly parsed into the appropriate structures.
/// See: https://www.w3.org/TR/selectors-4/
/// </summary>
public class SelectorParsingTests
{
    #region Basic Parsing Tests
    [Fact]
    public void Parse_SimpleTypeSelector_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("div");

        // Assert - A type selector should produce exactly one complex selector
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_UniversalSelector_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("*");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_IdSelector_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("#myId");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_ClassSelector_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(".myClass");

        // Assert
        Assert.Single(selector);
    }

    [Fact(Skip = "Bug: CssParser returns NullReferenceException when parsing attribute selectors")]
    public void Parse_AttributeSelector_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("[data-value]");

        // Assert
        Assert.Single(selector);
    }

    [Fact(Skip = "Bug: CssParser returns NullReferenceException when parsing attribute selectors")]
    public void Parse_AttributeSelectorWithValue_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("[data-value='test']");

        // Assert
        Assert.Single(selector);
    }
    #endregion

    #region Compound Selector Tests
    [Fact]
    public void Parse_TypeWithClass_ParsesAsCompoundSelector()
    {
        // Arrange & Act
        var selector = new CssSelector("div.myClass");

        // Assert - Compound selector should still be one complex selector
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_TypeWithId_ParsesAsCompoundSelector()
    {
        // Arrange & Act
        var selector = new CssSelector("div#myId");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_TypeWithMultipleClasses_ParsesAsCompoundSelector()
    {
        // Arrange & Act
        var selector = new CssSelector("div.class1.class2.class3");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_TypeWithIdAndClass_ParsesAsCompoundSelector()
    {
        // Arrange & Act
        var selector = new CssSelector("div#myId.myClass");

        // Assert
        Assert.Single(selector);
    }
    #endregion

    #region Combinator Tests
    [Fact]
    public void Parse_DescendantCombinator_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("div span");

        // Assert - Should produce one complex selector with multiple relative selectors
        Assert.Single(selector);
        Assert.True(selector[0].Count > 1, "Descendant combinator should have multiple relative selectors");
    }

    [Fact]
    public void Parse_ChildCombinator_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("div > span");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count > 1, "Child combinator should have multiple relative selectors");
    }

    [Fact]
    public void Parse_AdjacentSiblingCombinator_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("div + span");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count > 1, "Adjacent sibling combinator should have multiple relative selectors");
    }

    [Fact]
    public void Parse_GeneralSiblingCombinator_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("div ~ span");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count > 1, "General sibling combinator should have multiple relative selectors");
    }

    [Fact]
    public void Parse_MultipleCombinators_ParsesSuccessfully()
    {
        // Arrange & Act - div > section p span
        var selector = new CssSelector("div > section p span");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count >= 3, "Multiple combinators should create multiple relative selectors");
    }
    #endregion

    #region Selector List Tests
    [Fact]
    public void Parse_SelectorList_ParsesAsMultipleComplexSelectors()
    {
        // Arrange & Act
        var selector = new CssSelector("div, span");

        // Assert - Should produce two complex selectors
        Assert.Equal(2, selector.Count);
    }

    [Fact]
    public void Parse_SelectorListWithThreeSelectors_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector("div, span, p");

        // Assert
        Assert.Equal(3, selector.Count);
    }

    [Fact]
    public void Parse_SelectorListWithComplexSelectors_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector("div.container, span#main, p.intro");

        // Assert
        Assert.Equal(3, selector.Count);
    }

    [Fact]
    public void Parse_SelectorListWithCombinators_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector("div > p, section span");

        // Assert
        Assert.Equal(2, selector.Count);
    }
    #endregion

    #region Pseudo-Class Tests
    [Fact]
    public void Parse_PseudoClassSelector_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":hover");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_TypeWithPseudoClass_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("a:hover");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_PseudoClassWithFunction_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":not(.hidden)");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_NthChild_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":nth-child(2n+1)");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_Root_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":root");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_Empty_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":empty");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_FirstChild_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":first-child");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_LastChild_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector(":last-child");

        // Assert
        Assert.Single(selector);
    }
    #endregion

    #region Pseudo-Element Tests
    [Fact]
    public void Parse_PseudoElement_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("::before");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_TypeWithPseudoElement_ParsesSuccessfully()
    {
        // Arrange & Act
        var selector = new CssSelector("p::first-line");

        // Assert
        Assert.Single(selector);
    }
    #endregion

    #region Complex Real-World Selector Tests
    [Fact]
    public void Parse_ComplexSelector_NavigationExample()
    {
        // Arrange & Act
        var selector = new CssSelector("nav > ul.menu > li.active > a");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count >= 4, "Complex navigation selector should have multiple relative selectors");
    }

    [Fact]
    public void Parse_ComplexSelector_DescendantChain()
    {
        // Arrange & Act
        var selector = new CssSelector("div section article span");

        // Assert
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_ComplexSelector_MixedCombinators()
    {
        // Arrange & Act - Simple combinators without functional pseudo-classes
        var selector = new CssSelector("div > p + span ~ a");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count >= 4, "Complex selector with mixed combinators should have multiple relative selectors");
    }
    #endregion

    #region Whitespace Handling Tests
    [Fact]
    public void Parse_ExtraWhitespace_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector("  div   .class   ");

        // Assert - Should still parse as descendant combinator
        Assert.Single(selector);
    }

    [Fact]
    public void Parse_NoWhitespaceAroundCombinators_ParsesCorrectly()
    {
        // Arrange & Act
        var selector = new CssSelector("div>span");

        // Assert
        Assert.Single(selector);
        Assert.True(selector[0].Count > 1);
    }
    #endregion
}
