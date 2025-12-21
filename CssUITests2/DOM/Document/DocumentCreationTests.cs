using CssUI.DOM;
using CssUI.DOM.Enums;
using Xunit;
using Document = CssUI.DOM.Document;

namespace CssUITests.DOM.Documents;

/// <summary>
/// Unit tests for Document creation operations (createDocument, createHTMLDocument).
/// Spec: https://dom.spec.whatwg.org/#interface-domimplementation
/// </summary>
public class DocumentCreationTests
{
    #region createDocument Tests

    [Fact]
    public void CreateDocument_WithNamespace_CreatesDocument()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createDocument("http://example.com/ns", "root");

        // Assert
        Assert.NotNull(doc);
        Assert.NotNull(doc.documentElement);
        Assert.Equal("root", doc.documentElement.localName);
    }

    [Fact]
    public void CreateDocument_HasCorrectNodeType()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createDocument("http://example.com/ns", "root");

        // Assert
        Assert.Equal(ENodeType.DOCUMENT_NODE, doc.nodeType);
    }

    [Fact]
    public void CreateDocument_HasCorrectNodeName()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createDocument("http://example.com/ns", "root");

        // Assert
        Assert.Equal("#document", doc.nodeName);
    }

    [Fact]
    public void CreateDocument_DocumentElement_HasCorrectNamespace()
    {
        // Arrange
        var dom = new DOMImplementation();
        var ns = "http://example.com/ns";

        // Act
        var doc = dom.createDocument(ns, "root");

        // Assert
        Assert.Equal(ns, doc.documentElement!.NamespaceURI);
    }

    [Fact]
    public void CreateDocument_WithDoctype_AppendsDoctype()
    {
        // Arrange
        var dom = new DOMImplementation();
        var doctype = dom.createDocumentType("root");

        // Act
        var doc = dom.createDocument("http://example.com/ns", "root", doctype);

        // Assert
        Assert.NotNull(doc.doctype);
        Assert.Equal("root", doc.doctype.name);
    }

    [Fact]
    public void CreateDocument_EmptyQualifiedName_NoDocumentElement()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createDocument("http://example.com/ns", string.Empty);

        // Assert
        Assert.Null(doc.documentElement);
    }

    #endregion

    #region createHTMLDocument Tests

    [Fact]
    public void CreateHTMLDocument_CreatesValidStructure()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("Test Title");

        // Assert
        Assert.NotNull(doc);
        Assert.NotNull(doc.documentElement);
    }

    [Fact]
    public void CreateHTMLDocument_HasDoctype()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("Test");

        // Assert
        Assert.NotNull(doc.doctype);
        Assert.Equal("html", doc.doctype.name);
    }

    [Fact]
    public void CreateHTMLDocument_HasHtmlElement()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("Test");

        // Assert
        Assert.NotNull(doc.documentElement);
        Assert.Equal("html", doc.documentElement.localName.ToLowerInvariant());
    }

    [Fact]
    public void CreateHTMLDocument_HasHeadElement()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("Test");
        var head = doc.documentElement!.querySelector("head");

        // Assert
        Assert.NotNull(head);
    }

    [Fact]
    public void CreateHTMLDocument_HasBodyElement()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("Test");
        var body = doc.documentElement!.querySelector("body");

        // Assert
        Assert.NotNull(body);
    }

    [Fact]
    public void CreateHTMLDocument_HasTitleElement()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("My Title");
        var title = doc.documentElement!.querySelector("title");

        // Assert
        Assert.NotNull(title);
        Assert.Equal("My Title", title.textContent);
    }

    [Fact]
    public void CreateHTMLDocument_EmptyTitle_CreatesNoTitleElement()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act - Note: behavior may vary depending on implementation
        var doc = dom.createHTMLDocument(null!);
        var title = doc.documentElement!.querySelector("title");

        // Assert
        Assert.Null(title);
    }

    [Fact]
    public void CreateHTMLDocument_ContentType_IsTextHtml()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doc = dom.createHTMLDocument("Test");

        // Assert
        Assert.Equal("text/html", doc.contentType);
    }

    #endregion

    #region createDocumentType Tests

    [Fact]
    public void CreateDocumentType_ValidName_CreatesDoctype()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doctype = dom.createDocumentType("html");

        // Assert
        Assert.NotNull(doctype);
        Assert.Equal("html", doctype.name);
    }

    [Fact]
    public void CreateDocumentType_WithPublicId_SetsPublicId()
    {
        // Arrange
        var dom = new DOMImplementation();
        var publicId = "-//W3C//DTD HTML 4.01//EN";

        // Act
        var doctype = dom.createDocumentType("html", publicId);

        // Assert
        Assert.Equal(publicId, doctype.publicId);
    }

    [Fact]
    public void CreateDocumentType_WithSystemId_SetsSystemId()
    {
        // Arrange
        var dom = new DOMImplementation();
        var systemId = "http://www.w3.org/TR/html4/strict.dtd";

        // Act
        var doctype = dom.createDocumentType("html", "", systemId);

        // Assert
        Assert.Equal(systemId, doctype.systemId);
    }

    [Fact]
    public void CreateDocumentType_HasCorrectNodeType()
    {
        // Arrange
        var dom = new DOMImplementation();

        // Act
        var doctype = dom.createDocumentType("html");

        // Assert
        Assert.Equal(ENodeType.DOCUMENT_TYPE_NODE, doctype.nodeType);
    }

    #endregion
}
