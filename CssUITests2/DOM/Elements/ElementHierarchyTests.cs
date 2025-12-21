using CssUI.DOM;
using Xunit;

namespace CssUITests.DOM.Elements;

/// <summary>
/// Unit tests for DOM element hierarchy operations (appendChild, removeChild, insertBefore).
/// Spec: https://dom.spec.whatwg.org/#interface-node
/// </summary>
public class ElementHierarchyTests
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

    #region appendChild Tests

    [Fact]
    public void AppendChild_SingleChild_AddsChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");

        // Act
        parent.appendChild(child);

        // Assert
        Assert.Single(parent.childNodes);
        Assert.Same(child, parent.childNodes[0]);
    }

    [Fact]
    public void AppendChild_MultipleChildren_AddsInOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        var child3 = CreateTestElement(doc, "a");

        // Act
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);

        // Assert
        Assert.Equal(3, parent.childNodes.Count);
        Assert.Same(child1, parent.childNodes[0]);
        Assert.Same(child2, parent.childNodes[1]);
        Assert.Same(child3, parent.childNodes[2]);
    }

    [Fact]
    public void AppendChild_SetsParentNode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");

        // Act
        parent.appendChild(child);

        // Assert
        Assert.Same(parent, child.parentNode);
    }

    [Fact]
    public void AppendChild_ReturnsAppendedChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");

        // Act
        var result = parent.appendChild(child);

        // Assert
        Assert.Same(child, result);
    }

    [Fact]
    public void AppendChild_MovesFromOtherParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent1 = CreateTestElement(doc, "div");
        var parent2 = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        parent1.appendChild(child);

        // Act
        parent2.appendChild(child);

        // Assert
        Assert.Empty(parent1.childNodes);
        Assert.Single(parent2.childNodes);
        Assert.Same(parent2, child.parentNode);
    }

    [Fact]
    public void AppendChild_Text_AddsTextNode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var text = doc.createTextNode("Hello World");

        // Act
        parent.appendChild(text);

        // Assert
        Assert.Single(parent.childNodes);
        Assert.Equal("Hello World", parent.textContent);
    }

    #endregion

    #region removeChild Tests

    [Fact]
    public void RemoveChild_ExistingChild_RemovesChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act
        parent.removeChild(child);

        // Assert
        Assert.Empty(parent.childNodes);
    }

    [Fact]
    public void RemoveChild_ClearsParentNode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act
        parent.removeChild(child);

        // Assert
        Assert.Null(child.parentNode);
    }

    [Fact]
    public void RemoveChild_ReturnsRemovedChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act
        var result = parent.removeChild(child);

        // Assert
        Assert.Same(child, result);
    }

    [Fact]
    public void RemoveChild_MiddleChild_PreservesOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        var child3 = CreateTestElement(doc, "a");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);

        // Act
        parent.removeChild(child2);

        // Assert
        Assert.Equal(2, parent.childNodes.Count);
        Assert.Same(child1, parent.childNodes[0]);
        Assert.Same(child3, parent.childNodes[1]);
    }

    #endregion

    #region insertBefore Tests

    [Fact]
    public void InsertBefore_InsertsAtCorrectPosition()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var existing = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(existing);

        // Act
        parent.insertBefore(newChild, existing);

        // Assert
        Assert.Equal(2, parent.childNodes.Count);
        Assert.Same(newChild, parent.childNodes[0]);
        Assert.Same(existing, parent.childNodes[1]);
    }

    [Fact]
    public void InsertBefore_NullReference_AppendsChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var existing = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(existing);

        // Act
        parent.insertBefore(newChild, null!);

        // Assert
        Assert.Equal(2, parent.childNodes.Count);
        Assert.Same(existing, parent.childNodes[0]);
        Assert.Same(newChild, parent.childNodes[1]);
    }

    [Fact]
    public void InsertBefore_SetsParentNode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var existing = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(existing);

        // Act
        parent.insertBefore(newChild, existing);

        // Assert
        Assert.Same(parent, newChild.parentNode);
    }

    [Fact]
    public void InsertBefore_ReturnsInsertedNode()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var existing = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(existing);

        // Act
        var result = parent.insertBefore(newChild, existing);

        // Assert
        Assert.Same(newChild, result);
    }

    [Fact]
    public void InsertBefore_MovesFromOtherParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent1 = CreateTestElement(doc, "div");
        var parent2 = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        var reference = CreateTestElement(doc, "p");
        parent1.appendChild(child);
        parent2.appendChild(reference);

        // Act
        parent2.insertBefore(child, reference);

        // Assert
        Assert.Empty(parent1.childNodes);
        Assert.Equal(2, parent2.childNodes.Count);
        Assert.Same(child, parent2.childNodes[0]);
    }

    #endregion

    #region replaceChild Tests

    [Fact]
    public void ReplaceChild_ReplacesExistingChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var oldChild = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(oldChild);

        // Act
        parent.replaceChild(newChild, oldChild);

        // Assert
        Assert.Single(parent.childNodes);
        Assert.Same(newChild, parent.childNodes[0]);
    }

    [Fact]
    public void ReplaceChild_ReturnsOldChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var oldChild = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(oldChild);

        // Act
        var result = parent.replaceChild(newChild, oldChild);

        // Assert
        Assert.Same(oldChild, result);
    }

    [Fact]
    public void ReplaceChild_ClearsOldChildParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var oldChild = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(oldChild);

        // Act
        parent.replaceChild(newChild, oldChild);

        // Assert
        Assert.Null(oldChild.parentNode);
    }

    [Fact]
    public void ReplaceChild_SetsNewChildParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var oldChild = CreateTestElement(doc, "span");
        var newChild = CreateTestElement(doc, "p");
        parent.appendChild(oldChild);

        // Act
        parent.replaceChild(newChild, oldChild);

        // Assert
        Assert.Same(parent, newChild.parentNode);
    }

    [Fact]
    public void ReplaceChild_PreservesPosition()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        var child3 = CreateTestElement(doc, "a");
        var newChild = CreateTestElement(doc, "strong");
        parent.appendChild(child1);
        parent.appendChild(child2);
        parent.appendChild(child3);

        // Act
        parent.replaceChild(newChild, child2);

        // Assert
        Assert.Equal(3, parent.childNodes.Count);
        Assert.Same(child1, parent.childNodes[0]);
        Assert.Same(newChild, parent.childNodes[1]);
        Assert.Same(child3, parent.childNodes[2]);
    }

    #endregion

    #region contains Tests

    [Fact]
    public void Contains_DirectChild_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act & Assert
        Assert.True(parent.contains(child));
    }

    [Fact]
    public void Contains_NestedDescendant_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var grandparent = CreateTestElement(doc, "div");
        var parent = CreateTestElement(doc, "section");
        var child = CreateTestElement(doc, "span");
        grandparent.appendChild(parent);
        parent.appendChild(child);

        // Act & Assert
        Assert.True(grandparent.contains(child));
    }

    [Fact]
    public void Contains_Self_ReturnsTrue()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element = CreateTestElement(doc, "div");

        // Act & Assert
        Assert.True(element.contains(element));
    }

    [Fact]
    public void Contains_Sibling_ReturnsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        parent.appendChild(child1);
        parent.appendChild(child2);

        // Act & Assert
        Assert.False(child1.contains(child2));
        Assert.False(child2.contains(child1));
    }

    [Fact]
    public void Contains_Ancestor_ReturnsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act & Assert
        Assert.False(child.contains(parent));
    }

    [Fact]
    public void Contains_Unrelated_ReturnsFalse()
    {
        // Arrange
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");

        // Act & Assert
        Assert.False(element1.contains(element2));
        Assert.False(element2.contains(element1));
    }

    #endregion

    #region firstChild / lastChild Tests

    [Fact]
    public void FirstChild_WithChildren_ReturnsFirstChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        parent.appendChild(child1);
        parent.appendChild(child2);

        // Act & Assert
        Assert.Same(child1, parent.firstChild);
    }

    [Fact]
    public void FirstChild_NoChildren_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");

        // Act & Assert
        Assert.Null(parent.firstChild);
    }

    [Fact]
    public void LastChild_WithChildren_ReturnsLastChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        parent.appendChild(child1);
        parent.appendChild(child2);

        // Act & Assert
        Assert.Same(child2, parent.lastChild);
    }

    [Fact]
    public void LastChild_NoChildren_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");

        // Act & Assert
        Assert.Null(parent.lastChild);
    }

    #endregion

    #region previousSibling / nextSibling Tests

    [Fact]
    public void NextSibling_HasSibling_ReturnsNextSibling()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        parent.appendChild(child1);
        parent.appendChild(child2);

        // Act & Assert
        Assert.Same(child2, child1.nextSibling);
    }

    [Fact]
    public void NextSibling_LastChild_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act & Assert
        Assert.Null(child.nextSibling);
    }

    [Fact]
    public void PreviousSibling_HasSibling_ReturnsPreviousSibling()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        parent.appendChild(child1);
        parent.appendChild(child2);

        // Act & Assert
        Assert.Same(child1, child2.previousSibling);
    }

    [Fact]
    public void PreviousSibling_FirstChild_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var parent = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        parent.appendChild(child);

        // Act & Assert
        Assert.Null(child.previousSibling);
    }

    #endregion
}
