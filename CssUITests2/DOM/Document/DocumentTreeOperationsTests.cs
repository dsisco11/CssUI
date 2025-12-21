using System.Linq;
using CssUI.DOM;
using Xunit;
using Document = CssUI.DOM.Document;

namespace CssUITests.DOM.Documents;

/// <summary>
/// Unit tests for Document tree operations.
/// Spec: https://dom.spec.whatwg.org/#interface-document
/// </summary>
public class DocumentTreeOperationsTests
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

    #region documentElement Tests

    [Fact]
    public void DocumentElement_NewDocument_IsNotNull()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createDocument("http://example.com/ns", "root");

        // Assert
        Assert.NotNull(doc.documentElement);
    }

    [Fact]
    public void DocumentElement_IsFirstElementChild()
    {
        // Arrange
        var dom = new DOMImplementation();
        var doc = dom.createDocument("http://example.com/ns", "root");

        // Assert
        Assert.Same(doc.documentElement, doc.childNodes.OfType<Element>().First());
    }

    [Fact]
    public void DocumentElement_ParentIsDocument()
    {
        // Arrange
        var dom = new DOMImplementation();
        var doc = dom.createDocument("http://example.com/ns", "root");

        // Assert
        Assert.Same(doc, doc.documentElement!.parentNode);
    }

    #endregion

    #region createElement Tests (on Document)

    [Fact]
    public void Document_CreateElement_CreatesOrphanElement()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Assert
        Assert.NotNull(element);
        Assert.Null(element.parentNode);
        Assert.Same(doc, element.ownerDocument);
    }

    [Fact]
    public void Document_CreateElement_CanBeAppendedToDocumentElement()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = doc.createElement("div", new ElementCreationOptions(string.Empty));

        // Act
        doc.documentElement!.appendChild(element);

        // Assert
        Assert.Same(doc.documentElement, element.parentNode);
    }

    #endregion

    #region createTextNode Tests

    [Fact]
    public void Document_CreateTextNode_CreatesTextNode()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var textNode = doc.createTextNode("Hello World");

        // Assert
        Assert.NotNull(textNode);
        Assert.Equal("Hello World", textNode.data);
    }

    [Fact]
    public void Document_CreateTextNode_HasCorrectOwnerDocument()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var textNode = doc.createTextNode("Text");

        // Assert
        Assert.Same(doc, textNode.ownerDocument);
    }

    [Fact]
    public void Document_CreateTextNode_CanBeAppended()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var textNode = doc.createTextNode("Hello");
        doc.documentElement!.appendChild(element);

        // Act
        element.appendChild(textNode);

        // Assert
        Assert.Single(element.childNodes);
        Assert.Equal("Hello", element.textContent);
    }

    [Fact]
    public void Document_CreateTextNode_EmptyString_CreatesEmptyTextNode()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var textNode = doc.createTextNode(string.Empty);

        // Assert
        Assert.NotNull(textNode);
        Assert.Equal(string.Empty, textNode.data);
    }

    #endregion

    #region textContent Tests

    [Fact]
    public void TextContent_SingleElement_ReturnsDescendantText()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var text = doc.createTextNode("Hello World");
        element.appendChild(text);
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.Equal("Hello World", element.textContent);
    }

    [Fact]
    public void TextContent_NestedElements_ConcatenatesText()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        child1.appendChild(doc.createTextNode("Hello "));
        child2.appendChild(doc.createTextNode("World"));
        parent.appendChild(child1);
        parent.appendChild(child2);
        doc.documentElement!.appendChild(parent);

        // Act & Assert
        Assert.Equal("Hello World", parent.textContent);
    }

    [Fact]
    public void TextContent_Set_ReplacesAllChildren()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        child.appendChild(doc.createTextNode("Old Text"));
        element.appendChild(child);
        doc.documentElement!.appendChild(element);

        // Act
        element.textContent = "New Text";

        // Assert
        Assert.Single(element.childNodes);
        Assert.Equal("New Text", element.textContent);
    }

    [Fact]
    public void TextContent_NoChildren_ReturnsEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(element);

        // Act & Assert
        Assert.Equal(string.Empty, element.textContent);
    }

    #endregion

    #region append / prepend Tests

    [Fact]
    public void Append_MultipleNodes_AppendsAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        doc.documentElement!.appendChild(parent);

        // Act
        parent.append(child1, child2);

        // Assert
        Assert.Equal(2, parent.childNodes.Count);
        Assert.Same(child1, parent.childNodes[0]);
        Assert.Same(child2, parent.childNodes[1]);
    }

    [Fact]
    public void Append_StringNodes_CreatesTextNodes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        doc.documentElement!.appendChild(parent);

        // Act
        parent.append("Hello ", "World");

        // Assert
        Assert.Equal("Hello World", parent.textContent);
    }

    [Fact]
    public void Append_MixedNodesAndStrings_AddsAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(parent);

        // Act
        parent.append("Hello ", child, " World");

        // Assert
        Assert.Equal(3, parent.childNodes.Count);
    }

    #endregion

    #region Document nodeValue / textContent Tests

    [Fact]
    public void Document_NodeValue_IsNull()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act & Assert
        Assert.Null(doc.nodeValue);
    }

    [Fact]
    public void Document_TextContent_IsNull()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act & Assert
        Assert.Null(doc.textContent);
    }

    [Fact]
    public void Document_NodeValue_SetDoesNothing()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        doc.nodeValue = "test";

        // Assert - Should still be null
        Assert.Null(doc.nodeValue);
    }

    [Fact]
    public void Document_TextContent_SetDoesNothing()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        doc.textContent = "test";

        // Assert - Should still be null
        Assert.Null(doc.textContent);
    }

    #endregion
}
