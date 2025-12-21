using System.Linq;
using CssUI.CSS;
using CssUI.DOM;
using Xunit;

namespace CssUITests.DOM.Elements;

/// <summary>
/// Unit tests for DOM element matching operations (matches, closest, querySelector, querySelectorAll).
/// Spec: https://dom.spec.whatwg.org/#dom-element-matches
/// </summary>
public class ElementMatchingTests
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

    #region matches Tests

    [Fact]
    public void Matches_TypeSelector_MatchesCorrectElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(div);
        doc.documentElement!.appendChild(span);

        // Act & Assert
        Assert.True(div.matches("div"));
        Assert.False(div.matches("span"));
    }

    [Fact]
    public void Matches_ClassSelector_MatchesElementWithClass()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "my-class";
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.True(element.matches(".my-class"));
        Assert.False(element.matches(".other-class"));
    }

    [Fact]
    public void Matches_IdSelector_MatchesElementWithId()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "my-id";
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.True(element.matches("#my-id"));
        Assert.False(element.matches("#other-id"));
    }

    [Fact]
    public void Matches_CombinedSelectors_MatchesCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "my-class";
        element.id = "my-id";
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.True(element.matches("div.my-class"));
        Assert.True(element.matches("div#my-id"));
        Assert.True(element.matches("div.my-class#my-id"));
        Assert.False(element.matches("span.my-class"));
    }

    [Fact]
    public void Matches_UniversalSelector_MatchesAnyElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(div);
        doc.documentElement!.appendChild(span);

        // Act & Assert
        Assert.True(div.matches("*"));
        Assert.True(span.matches("*"));
    }

    [Fact]
    public void Matches_DescendantCombinator_MatchesNestedElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act & Assert
        Assert.True(child.matches("div span"));
        Assert.False(parent.matches("div span"));
    }

    [Fact]
    public void Matches_ChildCombinator_MatchesDirectChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        var grandchild = CreateTestElement(doc, "p");
        parent.appendChild(child);
        child.appendChild(grandchild);
        doc.documentElement!.appendChild(parent);

        // Act & Assert
        Assert.True(child.matches("div > span"));
        Assert.False(grandchild.matches("div > p")); // Not a direct child of div
    }

    #endregion

    #region closest Tests

    [Fact]
    public void Closest_Self_ReturnsSelf()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "target";
        doc.documentElement!.appendChild(element);

        // Act
        var result = element.closest(".target");

        // Assert
        Assert.Same(element, result);
    }

    [Fact]
    public void Closest_Parent_ReturnsParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.className = "target";
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act
        var result = child.closest(".target");

        // Assert
        Assert.Same(parent, result);
    }

    [Fact]
    public void Closest_Ancestor_ReturnsClosestAncestor()
    {
        // Arrange
        var doc = CreateTestDocument();
        var grandparent = CreateTestElement(doc, "div");
        var parent = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        grandparent.className = "target";
        parent.className = "target";
        grandparent.appendChild(parent);
        parent.appendChild(child);
        doc.documentElement!.appendChild(grandparent);

        // Act
        var result = child.closest(".target");

        // Assert - Should return the closest matching ancestor (parent, not grandparent)
        Assert.Same(parent, result);
    }

    [Fact]
    public void Closest_NoMatch_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act
        var result = child.closest(".non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Closest_TypeSelector_MatchesType()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "article");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act
        var result = child.closest("article");

        // Assert
        Assert.Same(parent, result);
    }

    #endregion

    #region querySelector Tests

    [Fact]
    public void QuerySelector_SingleMatch_ReturnsFirst()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "div");
        element1.className = "target";
        element2.className = "target";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);

        // Act
        var result = doc.documentElement!.querySelector(".target");

        // Assert
        Assert.Same(element1, result);
    }

    [Fact]
    public void QuerySelector_NoMatch_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(element);

        // Act
        var result = doc.documentElement!.querySelector(".non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void QuerySelector_NestedElement_FindsDescendant()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        child.className = "target";
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act
        var result = doc.documentElement!.querySelector(".target");

        // Assert
        Assert.Same(child, result);
    }

    [Fact]
    public void QuerySelector_ComplexSelector_MatchesCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act - Use type selector with descendant combinator which we know works
        var result = doc.documentElement!.querySelector("div span");

        // Assert
        Assert.Same(child, result);
    }

    [Fact]
    public void QuerySelector_OnElement_SearchesDescendants()
    {
        // Arrange
        var doc = CreateTestDocument();
        var container = CreateTestElement(doc, "div");
        var insideTarget = CreateTestElement(doc, "span");
        var outsideTarget = CreateTestElement(doc, "span");
        insideTarget.className = "target";
        outsideTarget.className = "target";
        container.appendChild(insideTarget);
        doc.documentElement!.appendChild(container);
        doc.documentElement!.appendChild(outsideTarget);

        // Act - Query on container element
        var result = container.querySelector(".target");

        // Assert - Should find insideTarget, not outsideTarget
        Assert.Same(insideTarget, result);
    }

    #endregion

    #region querySelectorAll Tests

    [Fact]
    public void QuerySelectorAll_MultipleMatches_ReturnsAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "div");
        var element3 = CreateTestElement(doc, "span");
        element1.className = "target";
        element2.className = "target";
        element3.className = "other";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);
        doc.documentElement!.appendChild(element3);

        // Act
        var results = doc.documentElement!.querySelectorAll(".target").ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void QuerySelectorAll_NoMatch_ReturnsEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(element);

        // Act
        var results = doc.documentElement!.querySelectorAll(".non-existent");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void QuerySelectorAll_TreeOrder_ReturnsInOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.className = "target";
        child.className = "target";
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act
        var results = doc.documentElement!.querySelectorAll(".target").ToList();

        // Assert - Should be in tree order (parent before child)
        Assert.Equal(2, results.Count);
        Assert.Same(parent, results[0]);
        Assert.Same(child, results[1]);
    }

    [Fact]
    public void QuerySelectorAll_MultipleSelectors_ReturnsUnion()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(div);
        doc.documentElement!.appendChild(span);

        // Act - Comma-separated selectors
        var results = doc.documentElement!.querySelectorAll("div, span").ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void QuerySelectorAll_OnElement_OnlySearchesDescendants()
    {
        // Arrange
        var doc = CreateTestDocument();
        var container = CreateTestElement(doc, "div");
        var inside1 = CreateTestElement(doc, "span");
        var inside2 = CreateTestElement(doc, "span");
        var outside = CreateTestElement(doc, "span");
        inside1.className = "target";
        inside2.className = "target";
        outside.className = "target";
        container.appendChild(inside1);
        container.appendChild(inside2);
        doc.documentElement!.appendChild(container);
        doc.documentElement!.appendChild(outside);

        // Act
        var results = container.querySelectorAll(".target").ToList();

        // Assert - Should only find inside1 and inside2
        Assert.Equal(2, results.Count);
    }

    #endregion
}
