using System.Linq;
using CssUI.DOM;
using CssUI.DOM.Enums;
using Xunit;

namespace CssUITests.DOM.Elements;

/// <summary>
/// Unit tests for DOM element query operations (getElementById, getElementsByClassName, getElementsByTagName).
/// Spec: https://dom.spec.whatwg.org/#interface-document
/// </summary>
public class ElementQueryTests
{
    #region Test Infrastructure

    private static Document CreateTestDocument()
    {
        var dom = new DOMImplementation();
        return dom.createDocument("CssUI", "cssui");
    }

    private static Document CreateHTMLDocument()
    {
        var dom = new DOMImplementation();
        return dom.createHTMLDocument("Test");
    }

    private static Element CreateTestElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    #endregion

    #region getElementById Tests

    [Fact(Skip = "DOM query tests")]
    public void GetElementByID_ExistingId_ReturnsElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "test-id";
        doc.documentElement!.appendChild(element);

        // Act
        var result = doc.getElementByID("test-id");

        // Assert
        Assert.NotNull(result);
        Assert.Same(element, result);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementByID_NonExistentId_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var result = doc.getElementByID("non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementByID_DuplicateIds_ReturnsFirst()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        element1.id = "duplicate-id";
        element2.id = "duplicate-id";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);

        // Act
        var result = doc.getElementByID("duplicate-id");

        // Assert
        // Per spec, first element in tree order should be returned
        Assert.NotNull(result);
        Assert.Same(element1, result);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementByID_NestedElement_FindsElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        child.id = "nested-id";
        parent.appendChild(child);
        doc.documentElement!.appendChild(parent);

        // Act
        var result = doc.getElementByID("nested-id");

        // Assert
        Assert.NotNull(result);
        Assert.Same(child, result);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementByID_AfterIdChange_FindsNewId()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "old-id";
        doc.documentElement!.appendChild(element);
        element.id = "new-id";

        // Act
        var oldResult = doc.getElementByID("old-id");
        var newResult = doc.getElementByID("new-id");

        // Assert
        Assert.Null(oldResult);
        Assert.NotNull(newResult);
        Assert.Same(element, newResult);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementByID_AfterRemoval_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.id = "removed-id";
        doc.documentElement!.appendChild(element);
        doc.documentElement!.removeChild(element);

        // Act
        var result = doc.getElementByID("removed-id");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region getElementsByClassName Tests

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByClassName_SingleClass_ReturnsMatching()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        element1.className = "test-class";
        element2.className = "other-class";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);

        // Act
        var results = doc.getElementsByClassName("test-class").ToList();

        // Assert
        Assert.Single(results);
        Assert.Same(element1, results[0]);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByClassName_MultipleMatches_ReturnsAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        var element3 = CreateTestElement(doc, "p");
        element1.className = "target";
        element2.className = "target";
        element3.className = "other";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);
        doc.documentElement!.appendChild(element3);

        // Act
        var results = doc.getElementsByClassName("target").ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByClassName_NoMatches_ReturnsEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        element.className = "some-class";
        doc.documentElement!.appendChild(element);

        // Act
        var results = doc.getElementsByClassName("non-existent");

        // Assert
        Assert.Empty(results);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByClassName_MultipleClasses_MatchesAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        element1.className = "class1 class2";
        element2.className = "class1";
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);

        // Act - Query for elements with both classes
        var results = doc.getElementsByClassName("class1 class2").ToList();

        // Assert
        Assert.Single(results);
        Assert.Same(element1, results[0]);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByClassName_NestedElements_FindsAll()
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
        var results = doc.getElementsByClassName("target").ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByClassName_OnElement_SearchesDescendants()
    {
        // Arrange
        var doc = CreateTestDocument();
        var container = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        var outside = CreateTestElement(doc, "section");
        child1.className = "target";
        outside.className = "target";
        container.appendChild(child1);
        container.appendChild(child2);
        doc.documentElement!.appendChild(container);
        doc.documentElement!.appendChild(outside);

        // Act - Query on container element only
        var results = container.getElementsByClassName("target").ToList();

        // Assert - Should only find child1, not outside
        Assert.Single(results);
        Assert.Same(child1, results[0]);
    }

    #endregion

    #region getElementsByTagName Tests

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagName_SingleMatch_ReturnsElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(div);
        doc.documentElement!.appendChild(span);

        // Act
        var results = doc.documentElement!.getElementsByTagName("div").ToList();

        // Assert
        Assert.Single(results);
        Assert.Same(div, results[0]);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagName_MultipleMatches_ReturnsAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div1 = CreateTestElement(doc, "div");
        var div2 = CreateTestElement(doc, "div");
        var div3 = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(div1);
        doc.documentElement!.appendChild(div2);
        doc.documentElement!.appendChild(div3);

        // Act
        var results = doc.documentElement!.getElementsByTagName("div").ToList();

        // Assert
        Assert.Equal(3, results.Count);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagName_NoMatches_ReturnsEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(div);

        // Act
        var results = doc.documentElement!.getElementsByTagName("span");

        // Assert
        Assert.Empty(results);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagName_Wildcard_ReturnsAllDescendants()
    {
        // Arrange
        var doc = CreateTestDocument();
        var div = CreateTestElement(doc, "div");
        var span = CreateTestElement(doc, "span");
        var p = CreateTestElement(doc, "p");
        doc.documentElement!.appendChild(div);
        doc.documentElement!.appendChild(span);
        doc.documentElement!.appendChild(p);

        // Act
        var results = doc.documentElement!.getElementsByTagName("*").ToList();

        // Assert
        Assert.Equal(3, results.Count);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagName_NestedElements_ReturnsTreeOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var outer = CreateTestElement(doc, "div");
        var inner = CreateTestElement(doc, "div");
        outer.appendChild(inner);
        doc.documentElement!.appendChild(outer);

        // Act
        var results = doc.documentElement!.getElementsByTagName("div").ToList();

        // Assert - Should be in tree order (depth-first)
        Assert.Equal(2, results.Count);
        Assert.Same(outer, results[0]);
        Assert.Same(inner, results[1]);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagName_CaseInsensitive_MatchesHTMLElements()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var body = doc.documentElement!.getElementsByTagName("body").First();
        var div = CreateTestElement(doc, "div");
        body.appendChild(div);

        // Act
        var resultsLower = body.getElementsByTagName("div").ToList();
        var resultsUpper = body.getElementsByTagName("DIV").ToList();

        // Assert - In HTML documents, should be case-insensitive
        Assert.Single(resultsLower);
        Assert.Single(resultsUpper);
    }

    #endregion

    #region getElementsByTagNameNS Tests

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagNameNS_MatchingNamespace_ReturnsElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var svgNs = "http://www.w3.org/2000/svg";
        var svgElement = doc.createElementNS(svgNs, "rect", new ElementCreationOptions(string.Empty));
        doc.documentElement!.appendChild(svgElement);

        // Act
        var results = doc.documentElement!.getElementsByTagNameNS(svgNs, "rect").ToList();

        // Assert
        Assert.Single(results);
        Assert.Same(svgElement, results[0]);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagNameNS_WildcardNamespace_MatchesAllNamespaces()
    {
        // Arrange
        var doc = CreateTestDocument();
        var svgNs = "http://www.w3.org/2000/svg";
        var customNs = "http://example.com/custom";
        var svgRect = doc.createElementNS(svgNs, "rect", new ElementCreationOptions(string.Empty));
        var customRect = doc.createElementNS(customNs, "rect", new ElementCreationOptions(string.Empty));
        doc.documentElement!.appendChild(svgRect);
        doc.documentElement!.appendChild(customRect);

        // Act
        var results = doc.documentElement!.getElementsByTagNameNS("*", "rect").ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }

    [Fact(Skip = "DOM query tests")]
    public void GetElementsByTagNameNS_WildcardLocalName_MatchesAllLocalNames()
    {
        // Arrange
        var doc = CreateTestDocument();
        var svgNs = "http://www.w3.org/2000/svg";
        var rect = doc.createElementNS(svgNs, "rect", new ElementCreationOptions(string.Empty));
        var circle = doc.createElementNS(svgNs, "circle", new ElementCreationOptions(string.Empty));
        doc.documentElement!.appendChild(rect);
        doc.documentElement!.appendChild(circle);

        // Act
        var results = doc.documentElement!.getElementsByTagNameNS(svgNs, "*").ToList();

        // Assert
        Assert.Equal(2, results.Count);
    }

    #endregion
}
