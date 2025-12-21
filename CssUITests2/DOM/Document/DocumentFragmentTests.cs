using System.Linq;
using CssUI.DOM;
using CssUI.DOM.Enums;
using Xunit;
using Document = CssUI.DOM.Document;

namespace CssUITests.DOM.Documents;

/// <summary>
/// Unit tests for DocumentFragment operations.
/// Spec: https://dom.spec.whatwg.org/#interface-documentfragment
/// </summary>
public class DocumentFragmentTests
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

    #region Creation Tests

    [Fact]
    public void CreateDocumentFragment_ReturnsFragment()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var fragment = doc.createDocumentFragment();

        // Assert
        Assert.NotNull(fragment);
    }

    [Fact]
    public void CreateDocumentFragment_HasCorrectNodeType()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var fragment = doc.createDocumentFragment();

        // Assert
        Assert.Equal(ENodeType.DOCUMENT_FRAGMENT_NODE, fragment.nodeType);
    }

    [Fact]
    public void CreateDocumentFragment_HasCorrectNodeName()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var fragment = doc.createDocumentFragment();

        // Assert
        Assert.Equal("#document-fragment", fragment.nodeName);
    }

    [Fact]
    public void CreateDocumentFragment_HasNoChildren()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var fragment = doc.createDocumentFragment();

        // Assert
        Assert.Empty(fragment.childNodes);
    }

    [Fact]
    public void CreateDocumentFragment_HasNoParent()
    {
        // Arrange
        var doc = CreateTestDocument();

        // Act
        var fragment = doc.createDocumentFragment();

        // Assert
        Assert.Null(fragment.parentNode);
    }

    #endregion

    #region Child Node Tests

    [Fact]
    public void DocumentFragment_AppendChild_AddsChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");

        // Act
        fragment.appendChild(child);

        // Assert
        Assert.Single(fragment.childNodes);
        Assert.Same(child, fragment.childNodes[0]);
    }

    [Fact]
    public void DocumentFragment_AppendMultipleChildren_AddsAll()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child1 = CreateTestElement(doc, "div");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "p");

        // Act
        fragment.appendChild(child1);
        fragment.appendChild(child2);
        fragment.appendChild(child3);

        // Assert
        Assert.Equal(3, fragment.childNodes.Count);
    }

    [Fact]
    public void DocumentFragment_Children_HaveFragmentAsParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");

        // Act
        fragment.appendChild(child);

        // Assert
        Assert.Same(fragment, child.parentNode);
    }

    #endregion

    #region Appending Fragment to Element Tests

    [Fact]
    public void AppendFragment_ToElement_MovesChildren()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child1 = CreateTestElement(doc, "div");
        var child2 = CreateTestElement(doc, "span");
        fragment.appendChild(child1);
        fragment.appendChild(child2);
        var target = CreateTestElement(doc, "section");
        doc.documentElement!.appendChild(target);

        // Act
        target.appendChild(fragment);

        // Assert - Children should be moved to target
        Assert.Equal(2, target.childNodes.Count);
        Assert.Same(child1, target.childNodes[0]);
        Assert.Same(child2, target.childNodes[1]);
    }

    [Fact]
    public void AppendFragment_ToElement_FragmentBecomesEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");
        fragment.appendChild(child);
        var target = CreateTestElement(doc, "section");
        doc.documentElement!.appendChild(target);

        // Act
        target.appendChild(fragment);

        // Assert - Fragment should be empty
        Assert.Empty(fragment.childNodes);
    }

    [Fact]
    public void AppendFragment_Children_HaveNewParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");
        fragment.appendChild(child);
        var target = CreateTestElement(doc, "section");
        doc.documentElement!.appendChild(target);

        // Act
        target.appendChild(fragment);

        // Assert - Child should have target as parent
        Assert.Same(target, child.parentNode);
    }

    [Fact]
    public void InsertBefore_Fragment_InsertsAllChildren()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var fragmentChild1 = CreateTestElement(doc, "div");
        var fragmentChild2 = CreateTestElement(doc, "span");
        fragment.appendChild(fragmentChild1);
        fragment.appendChild(fragmentChild2);

        var target = CreateTestElement(doc, "section");
        var existing = CreateTestElement(doc, "p");
        target.appendChild(existing);
        doc.documentElement!.appendChild(target);

        // Act
        target.insertBefore(fragment, existing);

        // Assert
        Assert.Equal(3, target.childNodes.Count);
        Assert.Same(fragmentChild1, target.childNodes[0]);
        Assert.Same(fragmentChild2, target.childNodes[1]);
        Assert.Same(existing, target.childNodes[2]);
    }

    #endregion

    #region Query Tests

    [Fact]
    public void DocumentFragment_QuerySelector_FindsDescendants()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");
        child.className = "target";
        fragment.appendChild(child);

        // Act
        var result = fragment.querySelector(".target");

        // Assert
        Assert.Same(child, result);
    }

    [Fact]
    public void DocumentFragment_QuerySelectorAll_FindsAllDescendants()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child1 = CreateTestElement(doc, "div");
        var child2 = CreateTestElement(doc, "div");
        child1.className = "target";
        child2.className = "target";
        fragment.appendChild(child1);
        fragment.appendChild(child2);

        // Act
        var results = fragment.querySelectorAll(".target");

        // Assert
        Assert.Equal(2, results.Count());
    }

    #endregion

    #region textContent Tests

    [Fact]
    public void DocumentFragment_TextContent_ReturnsChildText()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var text = doc.createTextNode("Hello World");
        fragment.appendChild(text);

        // Act & Assert
        Assert.Equal("Hello World", fragment.textContent);
    }

    [Fact]
    public void DocumentFragment_TextContent_ConcatenatesDescendantText()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "span");
        child1.appendChild(doc.createTextNode("Hello "));
        child2.appendChild(doc.createTextNode("World"));
        fragment.appendChild(child1);
        fragment.appendChild(child2);

        // Act & Assert
        Assert.Equal("Hello World", fragment.textContent);
    }

    [Fact]
    public void DocumentFragment_TextContent_Set_ReplacesChildren()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");
        fragment.appendChild(child);

        // Act
        fragment.textContent = "New Text";

        // Assert
        Assert.Single(fragment.childNodes);
        Assert.Equal("New Text", fragment.textContent);
    }

    #endregion

    #region Clone Tests

    [Fact]
    public void DocumentFragment_CloneNode_Shallow_ClonesEmpty()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");
        fragment.appendChild(child);

        // Act
        var clone = fragment.cloneNode(false);

        // Assert
        Assert.Empty(clone.childNodes);
        Assert.IsType<DocumentFragment>(clone);
    }

    [Fact]
    public void DocumentFragment_CloneNode_Deep_ClonesChildren()
    {
        // Arrange
        var doc = CreateTestDocument();
        var fragment = doc.createDocumentFragment();
        var child = CreateTestElement(doc, "div");
        fragment.appendChild(child);

        // Act
        var clone = fragment.cloneNode(true);

        // Assert
        Assert.Single(clone.childNodes);
        Assert.IsType<DocumentFragment>(clone);
        Assert.NotSame(child, clone.childNodes[0]);
    }

    #endregion
}
