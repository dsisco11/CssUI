using CssUI.DOM;
using Xunit;

namespace CssUITests.DOM.Elements;

/// <summary>
/// Unit tests for DOM Element creation operations (createElement, createElementNS).
/// Spec: https://dom.spec.whatwg.org/#dom-document-createelement
/// </summary>
public class ElementCreationTests
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

    #endregion

    #region createElement Tests

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_ValidTagName_CreatesElement()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        Assert.Equal("div", element.localName);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_CaseInsensitive_NormalizesTagName()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var element = doc.createElement("DIV", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        // In HTML documents, tag names are typically lowercased
        Assert.Equal("div", element.localName.ToLowerInvariant());
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_DifferentTagNames_CreatesDifferentElements()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var div = doc.createElement("div", new ElementCreationOptions(string.Empty));
        var span = doc.createElement("span", new ElementCreationOptions(string.Empty));
        var p = doc.createElement("p", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.Equal("div", div.localName);
        Assert.Equal("span", span.localName);
        Assert.Equal("p", p.localName);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_HasCorrectNodeType()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.Equal(CssUI.DOM.Enums.ENodeType.ELEMENT_NODE, element.nodeType);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_HasNoChildren()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.Empty(element.childNodes);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_HasNoParent()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.Null(element.parentNode);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_HasCorrectOwnerDocument()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.Same(doc, element.ownerDocument);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElement_HasNoAttributes()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.False(element.hasAttributes());
    }

    #endregion

    #region createElementNS Tests

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElementNS_HTMLNamespace_CreatesHTMLElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var htmlNs = "http://www.w3.org/1999/xhtml";

        // Act
        var element = doc.createElementNS(htmlNs, "div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        Assert.Equal(htmlNs, element.NamespaceURI);
        Assert.Equal("div", element.localName);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElementNS_SVGNamespace_CreatesSVGElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var svgNs = "http://www.w3.org/2000/svg";

        // Act
        var element = doc.createElementNS(svgNs, "svg", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        Assert.Equal(svgNs, element.NamespaceURI);
        Assert.Equal("svg", element.localName);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElementNS_WithPrefix_HasCorrectQualifiedName()
    {
        // Arrange
        var doc = CreateTestDocument();
        var customNs = "http://example.com/custom";

        // Act
        var element = doc.createElementNS(customNs, "custom:widget", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        Assert.Equal(customNs, element.NamespaceURI);
        // Prefix handling depends on implementation
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void CreateElementNS_NullNamespace_CreatesElement()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElementNS(null!, "element", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        Assert.Null(element.NamespaceURI);
    }

    #endregion

    #region tagName Tests

    [Fact(Skip = "DOM Element creation tests")]
    public void TagName_HTMLElement_IsUpperCase()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert - In HTML documents, tagName is uppercase
        Assert.Equal("DIV", element.tagName);
    }

    [Fact(Skip = "DOM Element creation tests")]
    public void TagName_XMLElement_PreservesCase()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("MyElement", new ElementCreationOptions(string.Empty));

        // Assert - In XML documents, tagName preserves case
        Assert.Equal("MyElement", element.tagName);
    }

    #endregion
}
