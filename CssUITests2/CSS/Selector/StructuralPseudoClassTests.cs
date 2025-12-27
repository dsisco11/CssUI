using System.Linq;
using CssUI.CSS;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for structural pseudo-class selectors.
/// See: https://www.w3.org/TR/selectors-4/#structural-pseudos
/// </summary>
public class StructuralPseudoClassTests
{
    #region Test Infrastructure
    private const string SkipReason = "Pseudo-class selector not yet implemented or has parsing issues";

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

    #region :root Tests
    [Fact]
    public void Root_MatchesDocumentRootElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var selector = new CssSelector(":root");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(doc.documentElement), ":root should match document element");
    }

    [Fact]
    public void Root_DoesNotMatchNonRootElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var child = CreateTestElement(doc, "div");
        doc.documentElement.appendChild(child);
        var selector = new CssSelector(":root");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(child), ":root should not match child elements");
    }
    #endregion

    #region :empty Tests
    [Fact]
    public void Empty_MatchesElementWithNoChildren()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var selector = new CssSelector(":empty");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), ":empty should match element with no children");
    }

    [Fact]
    public void Empty_DoesNotMatchElementWithTextContent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.textContent = "Some text";
        var selector = new CssSelector(":empty");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(element), ":empty should not match element with text content");
    }

    [Fact]
    public void Empty_DoesNotMatchElementWithChildElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        var selector = new CssSelector(":empty");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(parent), ":empty should not match element with child elements");
    }

    [Fact(Skip = "Comment-only empty matching not yet implemented")]
    public void Empty_MatchesElementWithOnlyComments()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var comment = doc.createComment("This is a comment");
        element.appendChild(comment);
        var selector = new CssSelector(":empty");

        // Act & Assert - Per CSS spec, :empty should match elements with only comments
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(element), ":empty should match element with only comments");
    }
    #endregion

    #region :first-child Tests
    [Fact]
    public void FirstChild_MatchesFirstChildOfParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        var second = CreateTestElement(doc, "span");
        parent.appendChild(first);
        parent.appendChild(second);
        var selector = new CssSelector(":first-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(first), ":first-child should match first child");
    }

    [Fact]
    public void FirstChild_DoesNotMatchSecondChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        var second = CreateTestElement(doc, "span");
        parent.appendChild(first);
        parent.appendChild(second);
        var selector = new CssSelector(":first-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(second), ":first-child should not match second child");
    }

    [Fact]
    public void FirstChild_CombinedWithTypeSelector()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(span);  // First child is span
        parent.appendChild(p);
        var selector = new CssSelector("p:first-child");

        // Act & Assert - p is not first child, so should not match
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(p), "p:first-child should not match p that is not first child");
    }
    #endregion

    #region :last-child Tests
    [Fact]
    public void LastChild_MatchesLastChildOfParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        var last = CreateTestElement(doc, "span");
        parent.appendChild(first);
        parent.appendChild(last);
        var selector = new CssSelector(":last-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(last), ":last-child should match last child");
    }

    [Fact]
    public void LastChild_DoesNotMatchFirstChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        var last = CreateTestElement(doc, "span");
        parent.appendChild(first);
        parent.appendChild(last);
        var selector = new CssSelector(":last-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(first), ":last-child should not match first child");
    }
    #endregion

    #region :only-child Tests
    [Fact]
    public void OnlyChild_MatchesSoleChildOfParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var only = CreateTestElement(doc, "span");
        parent.appendChild(only);
        var selector = new CssSelector(":only-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(only), ":only-child should match sole child");
    }

    [Fact]
    public void OnlyChild_DoesNotMatchWithSiblings()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var first = CreateTestElement(doc, "span");
        var second = CreateTestElement(doc, "span");
        parent.appendChild(first);
        parent.appendChild(second);
        var selector = new CssSelector(":only-child");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(first), ":only-child should not match when there are siblings");
        Assert.False(selector[0].Match(second), ":only-child should not match when there are siblings");
    }
    #endregion

    #region :nth-child() Tests
    [Fact]
    public void NthChild_MatchesSpecificPosition()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "span");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);
        var selector = new CssSelector(":nth-child(2)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(child1), ":nth-child(2) should not match first child");
        Assert.True(selector[0].Match(child2), ":nth-child(2) should match second child");
        Assert.False(selector[0].Match(child3), ":nth-child(2) should not match third child");
    }

    [Fact]
    public void NthChild_OddKeyword()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "span");
        var child4 = CreateTestElement(doc, "span");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);
        parent.appendChild(child4);
        var selector = new CssSelector(":nth-child(odd)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child1), ":nth-child(odd) should match 1st child");
        Assert.False(selector[0].Match(child2), ":nth-child(odd) should not match 2nd child");
        Assert.True(selector[0].Match(child3), ":nth-child(odd) should match 3rd child");
        Assert.False(selector[0].Match(child4), ":nth-child(odd) should not match 4th child");
    }

    [Fact]
    public void NthChild_EvenKeyword()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "span");
        var child4 = CreateTestElement(doc, "span");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);
        parent.appendChild(child4);
        var selector = new CssSelector(":nth-child(even)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(child1), ":nth-child(even) should not match 1st child");
        Assert.True(selector[0].Match(child2), ":nth-child(even) should match 2nd child");
        Assert.False(selector[0].Match(child3), ":nth-child(even) should not match 3rd child");
        Assert.True(selector[0].Match(child4), ":nth-child(even) should match 4th child");
    }

    [Fact]
    public void NthChild_AnPlusB_Formula()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var children = Enumerable.Range(0, 6).Select(_ => CreateTestElement(doc, "span")).ToList();
        foreach (var child in children) parent.appendChild(child);
        var selector = new CssSelector(":nth-child(2n+1)");  // Matches 1, 3, 5 (odd)

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(children[0]), "2n+1 should match 1st");
        Assert.False(selector[0].Match(children[1]), "2n+1 should not match 2nd");
        Assert.True(selector[0].Match(children[2]), "2n+1 should match 3rd");
    }

    [Fact]
    public void NthChild_NegativeOffset()
    {
        // Arrange: -n+3 matches first 3 children (3, 2, 1)
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var children = Enumerable.Range(0, 5).Select(_ => CreateTestElement(doc, "span")).ToList();
        foreach (var child in children) parent.appendChild(child);
        var selector = new CssSelector(":nth-child(-n+3)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(children[0]), "-n+3 should match 1st");
        Assert.True(selector[0].Match(children[1]), "-n+3 should match 2nd");
        Assert.True(selector[0].Match(children[2]), "-n+3 should match 3rd");
        Assert.False(selector[0].Match(children[3]), "-n+3 should not match 4th");
        Assert.False(selector[0].Match(children[4]), "-n+3 should not match 5th");
    }
    #endregion

    #region :nth-last-child() Tests
    [Fact]
    public void NthLastChild_MatchesFromEnd()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "span");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);
        var selector = new CssSelector(":nth-last-child(1)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child3), ":nth-last-child(1) should match last child");
        Assert.False(selector[0].Match(child2), ":nth-last-child(1) should not match second to last");
    }

    [Fact]
    public void NthLastChild_MatchesSecondFromEnd()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "span");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);
        var selector = new CssSelector(":nth-last-child(2)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child2), ":nth-last-child(2) should match second to last");
        Assert.False(selector[0].Match(child3), ":nth-last-child(2) should not match last");
    }
    #endregion

    #region :first-of-type Tests
    [Fact]
    public void FirstOfType_MatchesFirstOfTypeAmongSiblings()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span1 = CreateTestElement(doc, "span");
        var p1 = CreateTestElement(doc, "p");
        var span2 = CreateTestElement(doc, "span");
        parent.appendChild(span1);
        parent.appendChild(p1);
        parent.appendChild(span2);
        var selector = new CssSelector("span:first-of-type");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(span1), "span:first-of-type should match first span");
        Assert.False(selector[0].Match(span2), "span:first-of-type should not match second span");
    }

    [Fact]
    public void FirstOfType_IndependentOfPosition()
    {
        // Arrange: First p is not first child but is first-of-type
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(span);  // span is first child
        parent.appendChild(p);     // p is second child but first-of-type for p
        var selector = new CssSelector("p:first-of-type");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "p:first-of-type should match p even if not first child");
    }
    #endregion

    #region :last-of-type Tests
    [Fact]
    public void LastOfType_MatchesLastOfTypeAmongSiblings()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span1 = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        var span2 = CreateTestElement(doc, "span");
        parent.appendChild(span1);
        parent.appendChild(p);
        parent.appendChild(span2);
        var selector = new CssSelector("span:last-of-type");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(span2), "span:last-of-type should match last span");
        Assert.False(selector[0].Match(span1), "span:last-of-type should not match first span");
    }
    #endregion

    #region :only-of-type Tests
    [Fact]
    public void OnlyOfType_MatchesOnlyOfTypeAmongSiblings()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(span);
        parent.appendChild(p);
        var selector = new CssSelector("p:only-of-type");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "p:only-of-type should match when p is the only p");
    }

    [Fact]
    public void OnlyOfType_CanHaveOtherTypeSiblings()
    {
        // Arrange: p is only-of-type even with span siblings
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span1 = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        var span2 = CreateTestElement(doc, "span");
        parent.appendChild(span1);
        parent.appendChild(p);
        parent.appendChild(span2);
        var selector = new CssSelector("p:only-of-type");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "p:only-of-type should match even with other type siblings");
    }
    #endregion

    #region :nth-of-type() Tests
    [Fact]
    public void NthOfType_MatchesNthOfTypeAmongSiblings()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var span1 = CreateTestElement(doc, "span");
        var p1 = CreateTestElement(doc, "p");
        var span2 = CreateTestElement(doc, "span");
        var p2 = CreateTestElement(doc, "p");
        parent.appendChild(span1);
        parent.appendChild(p1);
        parent.appendChild(span2);
        parent.appendChild(p2);
        var selector = new CssSelector("span:nth-of-type(2)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(span1), "span:nth-of-type(2) should not match first span");
        Assert.True(selector[0].Match(span2), "span:nth-of-type(2) should match second span");
    }

    [Fact]
    public void NthOfType_WithFormula()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var p1 = CreateTestElement(doc, "p");
        var p2 = CreateTestElement(doc, "p");
        var p3 = CreateTestElement(doc, "p");
        var p4 = CreateTestElement(doc, "p");
        parent.appendChild(p1);
        parent.appendChild(p2);
        parent.appendChild(p3);
        parent.appendChild(p4);
        var selector = new CssSelector("p:nth-of-type(2n)");  // Even positions: 2, 4

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(p1), "p:nth-of-type(2n) should not match 1st p");
        Assert.True(selector[0].Match(p2), "p:nth-of-type(2n) should match 2nd p");
        Assert.False(selector[0].Match(p3), "p:nth-of-type(2n) should not match 3rd p");
        Assert.True(selector[0].Match(p4), "p:nth-of-type(2n) should match 4th p");
    }
    #endregion

    #region :nth-last-of-type() Tests
    [Fact]
    public void NthLastOfType_MatchesFromEndOfType()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var p1 = CreateTestElement(doc, "p");
        var span = CreateTestElement(doc, "span");
        var p2 = CreateTestElement(doc, "p");
        var p3 = CreateTestElement(doc, "p");
        parent.appendChild(p1);
        parent.appendChild(span);
        parent.appendChild(p2);
        parent.appendChild(p3);
        var selector = new CssSelector("p:nth-last-of-type(1)");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p3), "p:nth-last-of-type(1) should match last p");
        Assert.False(selector[0].Match(p2), "p:nth-last-of-type(1) should not match second-to-last p");
        Assert.False(selector[0].Match(p1), "p:nth-last-of-type(1) should not match first p");
    }
    #endregion
}
