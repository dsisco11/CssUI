using System.Linq;
using CssUI.CSS;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Selector.Tests;

/// <summary>
/// Tests for combinator selectors (descendant, child, adjacent sibling, general sibling).
/// These tests verify that complex selectors with combinators correctly match elements
/// based on their relationships in the DOM tree.
/// </summary>
public class CombinatorSelectorTests
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

    #region Descendant Combinator Tests
    [Fact]
    public void DescendantCombinator_MatchesDirectChild()
    {
        // Arrange: div > span (direct child)
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        var selector = new CssSelector("div span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Descendant selector should match direct child");
    }

    [Fact]
    public void DescendantCombinator_MatchesDeepDescendant()
    {
        // Arrange: div > section > span (deep descendant)
        var doc = CreateTestDocument();
        var grandparent = CreateTestElement(doc, "div");
        var parent = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        grandparent.appendChild(parent);
        parent.appendChild(child);
        var selector = new CssSelector("div span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Descendant selector should match deep descendant");
    }

    [Fact]
    public void DescendantCombinator_DoesNotMatchNonDescendant()
    {
        // Arrange: Two separate elements, not related
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        // span is NOT a child of div
        var selector = new CssSelector("div span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(span), "Descendant selector should not match non-descendant");
    }

    [Fact]
    public void DescendantCombinator_MultipleLevel()
    {
        // Arrange: div > ul > li > a
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var ul = CreateTestElement(doc, "ul");
        var li = CreateTestElement(doc, "li");
        var a = CreateTestElement(doc, "a");
        div.appendChild(ul);
        ul.appendChild(li);
        li.appendChild(a);
        var selector = new CssSelector("div ul li a");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "Multi-level descendant selector should match");
    }

    [Fact]
    public void DescendantCombinator_WithClass()
    {
        // Arrange: div.container > span.text
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        parent.className = "container";
        var child = CreateTestElement(doc, "span");
        child.className = "text";
        parent.appendChild(child);
        var selector = new CssSelector("div.container span.text");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Descendant selector with classes should match");
    }
    #endregion

    #region Child Combinator Tests
    [Fact]
    public void ChildCombinator_MatchesDirectChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        var selector = new CssSelector("div > span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Child combinator should match direct child");
    }

    [Fact]
    public void ChildCombinator_DoesNotMatchDeepDescendant()
    {
        // Arrange: div > section > span (span is grandchild, not direct child)
        var doc = CreateTestDocument();
        var grandparent = CreateTestElement(doc, "div");
        var parent = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        grandparent.appendChild(parent);
        parent.appendChild(child);
        var selector = new CssSelector("div > span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(child), "Child combinator should NOT match grandchild");
    }

    [Fact]
    public void ChildCombinator_ChainedSelectors()
    {
        // Arrange: div > ul > li
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var ul = CreateTestElement(doc, "ul");
        var li = CreateTestElement(doc, "li");
        div.appendChild(ul);
        ul.appendChild(li);
        var selector = new CssSelector("div > ul > li");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(li), "Chained child combinators should match");
    }

    [Fact]
    public void ChildCombinator_WithTypeAndClass()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "nav");
        parent.className = "main-nav";
        var child = CreateTestElement(doc, "ul");
        child.className = "menu";
        parent.appendChild(child);
        var selector = new CssSelector("nav.main-nav > ul.menu");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(child), "Child combinator with type and class should match");
    }
    #endregion

    #region Adjacent Sibling Combinator Tests
    [Fact]
    public void AdjacentSiblingCombinator_MatchesImmediateNextSibling()
    {
        // Arrange: parent > [h1, p] - p is immediately after h1
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var h1 = CreateTestElement(doc, "h1");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(h1);
        parent.appendChild(p);
        var selector = new CssSelector("h1 + p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "Adjacent sibling selector should match immediate next sibling");
    }

    [Fact]
    public void AdjacentSiblingCombinator_DoesNotMatchNonAdjacentSibling()
    {
        // Arrange: parent > [h1, div, p] - p is NOT immediately after h1
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var h1 = CreateTestElement(doc, "h1");
        var divider = CreateTestElement(doc, "div");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(h1);
        parent.appendChild(divider);
        parent.appendChild(p);
        var selector = new CssSelector("h1 + p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(p), "Adjacent sibling selector should NOT match non-adjacent sibling");
    }

    [Fact]
    public void AdjacentSiblingCombinator_MatchesHeaderFollowedByMain()
    {
        // Arrange: body > [header, main]
        var doc = CreateTestDocument();
        var body = CreateTestElement(doc, "body");
        var header = CreateTestElement(doc, "header");
        var main = CreateTestElement(doc, "main");
        body.appendChild(header);
        body.appendChild(main);
        var selector = new CssSelector("header + main");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(main), "Adjacent sibling should match main after header");
    }
    #endregion

    #region General Sibling Combinator Tests
    [Fact]
    public void GeneralSiblingCombinator_MatchesAnySibling()
    {
        // Arrange: parent > [h1, div, p] - p is after h1 (but not immediately)
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var h1 = CreateTestElement(doc, "h1");
        var divider = CreateTestElement(doc, "div");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(h1);
        parent.appendChild(divider);
        parent.appendChild(p);
        var selector = new CssSelector("h1 ~ p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "General sibling selector should match any following sibling");
    }

    [Fact]
    public void GeneralSiblingCombinator_MatchesImmediateSiblingToo()
    {
        // Arrange: parent > [h1, p] - immediate sibling also matches ~
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var h1 = CreateTestElement(doc, "h1");
        var p = CreateTestElement(doc, "p");
        parent.appendChild(h1);
        parent.appendChild(p);
        var selector = new CssSelector("h1 ~ p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(p), "General sibling selector should also match immediate sibling");
    }

    [Fact]
    public void GeneralSiblingCombinator_DoesNotMatchPrecedingSibling()
    {
        // Arrange: parent > [p, h1] - p comes BEFORE h1
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var p = CreateTestElement(doc, "p");
        var h1 = CreateTestElement(doc, "h1");
        parent.appendChild(p);
        parent.appendChild(h1);
        var selector = new CssSelector("h1 ~ p");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.False(selector[0].Match(p), "General sibling selector should NOT match preceding sibling");
    }
    #endregion

    #region Mixed Combinator Tests
    [Fact]
    public void MixedCombinators_DescendantAndChild()
    {
        // Arrange: article > section > p (using "article p > span")
        var doc = CreateTestDocument();
        var article = CreateTestElement(doc, "article");
        var section = CreateTestElement(doc, "section");
        var p = CreateTestElement(doc, "p");
        var span = CreateTestElement(doc, "span");
        article.appendChild(section);
        section.appendChild(p);
        p.appendChild(span);
        var selector = new CssSelector("article p > span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(span), "Mixed descendant and child combinator should match");
    }

    [Fact]
    public void MixedCombinators_ChildAndDescendant()
    {
        // Arrange: nav > ul containing nested li > a
        var doc = CreateTestDocument();
        var nav = CreateTestElement(doc, "nav");
        var ul = CreateTestElement(doc, "ul");
        var li = CreateTestElement(doc, "li");
        var a = CreateTestElement(doc, "a");
        nav.appendChild(ul);
        ul.appendChild(li);
        li.appendChild(a);
        var selector = new CssSelector("nav > ul a");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(a), "Mixed child and descendant combinator should match");
    }

    [Fact]
    public void MixedCombinators_WithSiblings()
    {
        // Arrange: div > [h1, p, span] using "div > h1 + p ~ span"
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var h1 = CreateTestElement(doc, "h1");
        var p = CreateTestElement(doc, "p");
        var span = CreateTestElement(doc, "span");
        div.appendChild(h1);
        div.appendChild(p);
        div.appendChild(span);
        var selector = new CssSelector("div > h1 + p ~ span");

        // Act & Assert
        Assert.True(selector.Count > 0, "Selector should parse");
        Assert.True(selector[0].Match(span), "Mixed child and sibling combinators should match");
    }
    #endregion
}
