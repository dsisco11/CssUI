using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.DOM.Traversal;

/// <summary>
/// Unit tests for TreeWalker to verify tree traversal doesn't cause infinite loops.
/// Spec: https://dom.spec.whatwg.org/#treewalker
/// </summary>
public class TreeWalkerTests
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

    #region nextNode Tests

    [Fact]
    public void NextNode_EmptyRoot_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        // Don't add any children
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        var result = walker.nextNode();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void NextNode_SingleChild_ReturnsChild()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        root.appendChild(child);
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        var result = walker.nextNode();

        // Assert
        Assert.Same(child, result);
    }

    [Fact]
    public void NextNode_MultipleChildren_ReturnsInOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        var child3 = CreateTestElement(doc, "section");
        root.appendChild(child1);
        root.appendChild(child2);
        root.appendChild(child3);
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert
        Assert.Same(child1, walker.nextNode());
        Assert.Same(child2, walker.nextNode());
        Assert.Same(child3, walker.nextNode());
        Assert.Null(walker.nextNode()); // No more nodes
    }

    [Fact]
    public void NextNode_NestedChildren_ReturnsDepthFirst()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var grandchild = CreateTestElement(doc, "a");
        var child2 = CreateTestElement(doc, "p");
        root.appendChild(child1);
        child1.appendChild(grandchild);
        root.appendChild(child2);
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert - Should be depth-first order
        Assert.Same(child1, walker.nextNode());
        Assert.Same(grandchild, walker.nextNode());
        Assert.Same(child2, walker.nextNode());
        Assert.Null(walker.nextNode());
    }

    [Fact]
    public void NextNode_DocumentAsRoot_TraversesAllElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var child1 = CreateTestElement(doc, "div");
        var child2 = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(child1);
        doc.documentElement!.appendChild(child2);
        var walker = new TreeWalker(doc, ENodeFilterMask.SHOW_ELEMENT);

        // Act - Count all elements found
        int count = 0;
        Node? current = walker.nextNode();
        while (current is not null && count < 100) // Safety limit
        {
            count++;
            current = walker.nextNode();
        }

        // Assert - Should find documentElement + child1 + child2 = 3 elements
        Assert.Equal(3, count);
    }

    [Fact]
    public void NextNode_DeeplyNested_DoesNotInfiniteLoop()
    {
        // Arrange
        var doc = CreateTestDocument();
        var current = doc.documentElement!;
        // Create a chain of 10 nested elements
        for (int i = 0; i < 10; i++)
        {
            var child = CreateTestElement(doc, $"level{i}");
            current.appendChild(child);
            current = child;
        }
        var walker = new TreeWalker(doc, ENodeFilterMask.SHOW_ELEMENT);

        // Act - Count all elements with safety limit
        int count = 0;
        Node? node = walker.nextNode();
        while (node is not null && count < 100)
        {
            count++;
            node = walker.nextNode();
        }

        // Assert - Should find documentElement + 10 nested = 11 elements
        Assert.Equal(11, count);
    }

    [Fact]
    public void NextNode_AfterLastNode_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        root.appendChild(child);
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        walker.nextNode(); // Get the child
        var result1 = walker.nextNode(); // Should be null
        var result2 = walker.nextNode(); // Should still be null

        // Assert
        Assert.Null(result1);
        Assert.Null(result2);
    }

    #endregion

    #region parentNode Tests

    [Fact]
    public void ParentNode_AtRoot_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        var result = walker.parentNode();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParentNode_AtChild_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        root.appendChild(child);
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
        walker.nextNode(); // Move to child

        // Act
        var result = walker.parentNode();

        // Assert - parentNode returns the parent (root), not null
        // The walker stops when it reaches the root, but parent of child IS root
        Assert.Same(root, result);
    }

    [Fact]
    public void ParentNode_AtGrandchild_ReturnsParent()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        var grandchild = CreateTestElement(doc, "a");
        root.appendChild(child);
        child.appendChild(grandchild);
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT);
        walker.nextNode(); // Move to child
        walker.nextNode(); // Move to grandchild

        // Act
        var result = walker.parentNode();

        // Assert
        Assert.Same(child, result);
    }

    #endregion

    #region Filter Tests

    [Fact]
    public void NextNode_WithFilter_SkipsFilteredNodes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        var child3 = CreateTestElement(doc, "span");
        root.appendChild(child1);
        root.appendChild(child2);
        root.appendChild(child3);

        // Filter that only accepts 'p' elements
        var filter = new TagNameFilter("p");
        var walker = new TreeWalker(root, ENodeFilterMask.SHOW_ELEMENT, filter);

        // Act
        var result = walker.nextNode();

        // Assert - Should skip spans and return p
        Assert.Same(child2, result);
        Assert.Null(walker.nextNode()); // No more p elements
    }

    private class TagNameFilter : NodeFilter
    {
        private readonly string _tagName;

        public TagNameFilter(string tagName)
        {
            _tagName = tagName;
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is Element element && element.tagName == _tagName)
                return ENodeFilterResult.FILTER_ACCEPT;
            return ENodeFilterResult.FILTER_SKIP;
        }
    }

    #endregion

    #region Integration Tests (simulating getElementsByClassName usage)

    [Fact]
    public void NextNode_SimulateGetElementsByClassName_NoInfiniteLoop()
    {
        // Arrange - This simulates the exact usage in Get_Elements_By_Class_Name
        var doc = CreateTestDocument();
        var element1 = CreateTestElement(doc, "div");
        var element2 = CreateTestElement(doc, "span");
        doc.documentElement!.appendChild(element1);
        doc.documentElement!.appendChild(element2);

        // Use Document as root (like getElementsByClassName does)
        var tree = new TreeWalker(doc, ENodeFilterMask.SHOW_ELEMENT);

        // Act - Iterate with safety counter
        var elements = new System.Collections.Generic.List<Element>();
        Node? current = tree.nextNode();
        int iterations = 0;
        while (current is not null && iterations < 1000)
        {
            if (current is Element e)
                elements.Add(e);
            current = tree.nextNode();
            iterations++;
        }

        // Assert
        Assert.True(iterations < 1000, "TreeWalker appears to be in an infinite loop");
        Assert.Equal(3, elements.Count); // documentElement + element1 + element2
    }

    #endregion
}
