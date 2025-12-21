using System;
using System.Collections.Generic;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.DOM.Traversal;

/// <summary>
/// Unit tests for NodeIterator to verify iterator traversal.
/// Spec: https://dom.spec.whatwg.org/#nodeiterator
/// </summary>
public class NodeIteratorTests
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

    private static Text CreateTextNode(Document doc, string data)
    {
        return doc.createTextNode(data);
    }

    /// <summary>
    /// Creates a NodeIterator from a root node and its descendants.
    /// This helper collects all descendants into a list for the iterator.
    /// </summary>
    private static NodeIterator CreateNodeIterator(Node root, ENodeFilterMask whatToShow, NodeFilter? filter = null)
    {
        var nodes = new List<Node>();
        CollectNodes(root, nodes);
        return new NodeIterator(root, nodes, whatToShow, filter);
    }

    private static void CollectNodes(Node node, List<Node> nodes)
    {
        nodes.Add(node);
        foreach (var child in node.childNodes)
        {
            CollectNodes(child, nodes);
        }
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_SetsRootCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");

        // Act
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Assert
        Assert.Same(root, iterator.root);
    }

    [Fact]
    public void Constructor_SetsWhatToShowCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");

        // Act
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_TEXT);

        // Assert
        Assert.Equal(ENodeFilterMask.SHOW_TEXT, iterator.whatToShow);
    }

    [Fact]
    public void Constructor_SetsFilterCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var filter = new TagNameFilter("div");

        // Act
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT, filter);

        // Assert
        Assert.Same(filter, iterator.Filter);
    }

    #endregion

    #region nextNode Tests

    [Fact]
    public void NextNode_EmptyRoot_ReturnsRoot()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        var result = iterator.nextNode();

        // Assert - First call returns the root itself
        Assert.Same(root, result);
    }

    [Fact]
    public void NextNode_AfterRoot_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        iterator.nextNode(); // Get root
        var result = iterator.nextNode(); // Should be null

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void NextNode_SingleChild_ReturnsInOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        root.appendChild(child);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child, iterator.nextNode());
        Assert.Null(iterator.nextNode());
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
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child1, iterator.nextNode());
        Assert.Same(child2, iterator.nextNode());
        Assert.Same(child3, iterator.nextNode());
        Assert.Null(iterator.nextNode());
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
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert - Should be depth-first order
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child1, iterator.nextNode());
        Assert.Same(grandchild, iterator.nextNode());
        Assert.Same(child2, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void NextNode_DeeplyNested_DoesNotInfiniteLoop()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var current = root;
        // Create a chain of 10 nested elements
        for (int i = 0; i < 10; i++)
        {
            var child = CreateTestElement(doc, $"level{i}");
            current.appendChild(child);
            current = child;
        }
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act - Count all elements with safety limit
        int count = 0;
        Node? node = iterator.nextNode();
        while (node is not null && count < 100)
        {
            count++;
            node = iterator.nextNode();
        }

        // Assert - root + 10 nested = 11 elements
        Assert.Equal(11, count);
    }

    #endregion

    #region previousNode Tests

    [Fact]
    public void PreviousNode_AtStart_ReturnsNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        var result = iterator.previousNode();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void PreviousNode_AfterNextNode_ReturnsPrevious()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        root.appendChild(child);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        iterator.nextNode(); // root
        iterator.nextNode(); // child
        var result = iterator.previousNode();

        // Assert
        Assert.Same(root, result);
    }

    [Fact]
    public void PreviousNode_TraverseBackward_ReturnsReverseOrder()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        root.appendChild(child1);
        root.appendChild(child2);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Forward traverse
        iterator.nextNode(); // root
        iterator.nextNode(); // child1
        iterator.nextNode(); // child2

        // Act & Assert - Backward traverse
        Assert.Same(child1, iterator.previousNode());
        Assert.Same(root, iterator.previousNode());
        Assert.Null(iterator.previousNode());
    }

    [Fact]
    public void PreviousAndNext_Alternating_WorksCorrectly()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        root.appendChild(child1);
        root.appendChild(child2);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child1, iterator.nextNode());
        Assert.Same(root, iterator.previousNode());
        Assert.Same(child1, iterator.nextNode());
        Assert.Same(child2, iterator.nextNode());
        Assert.Same(child1, iterator.previousNode());
    }

    #endregion

    #region whatToShow Filter Tests

    [Fact]
    public void NextNode_ShowElement_SkipsTextNodes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var text = CreateTextNode(doc, "Hello");
        var child = CreateTestElement(doc, "span");
        root.appendChild(text);
        root.appendChild(child);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act & Assert - Should only see elements, not text
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void NextNode_ShowText_SkipsElements()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var text1 = CreateTextNode(doc, "Hello");
        var child = CreateTestElement(doc, "span");
        var text2 = CreateTextNode(doc, "World");
        root.appendChild(text1);
        root.appendChild(child);
        child.appendChild(text2);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_TEXT);

        // Act & Assert - Should only see text nodes
        Assert.Same(text1, iterator.nextNode());
        Assert.Same(text2, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void NextNode_ShowAll_ReturnsAllNodes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var text = CreateTextNode(doc, "Hello");
        var child = CreateTestElement(doc, "span");
        root.appendChild(text);
        root.appendChild(child);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ALL);

        // Act & Assert - Should see all nodes
        Assert.Same(root, iterator.nextNode());
        Assert.Same(text, iterator.nextNode());
        Assert.Same(child, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void NextNode_ShowElementAndText_ReturnsElementsAndText()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var text = CreateTextNode(doc, "Hello");
        var child = CreateTestElement(doc, "span");
        root.appendChild(text);
        root.appendChild(child);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT | ENodeFilterMask.SHOW_TEXT);

        // Act & Assert
        Assert.Same(root, iterator.nextNode());
        Assert.Same(text, iterator.nextNode());
        Assert.Same(child, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    #endregion

    #region NodeFilter Tests

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
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT, filter);

        // Act
        var result = iterator.nextNode();

        // Assert - Should skip div and spans, return p
        Assert.Same(child2, result);

        // Should be empty - no more p elements after child2
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void NextNode_WithFilter_AcceptsMatchingNodes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child1 = CreateTestElement(doc, "div");
        var child2 = CreateTestElement(doc, "span");
        var child3 = CreateTestElement(doc, "div");
        root.appendChild(child1);
        root.appendChild(child2);
        root.appendChild(child3);

        var filter = new TagNameFilter("div");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT, filter);

        // Act & Assert - Should return all div elements
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child1, iterator.nextNode());
        Assert.Same(child3, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void PreviousNode_WithFilter_SkipsFilteredNodes()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "p"); // Make root match filter
        var child1 = CreateTestElement(doc, "span");
        var child2 = CreateTestElement(doc, "p");
        root.appendChild(child1);
        root.appendChild(child2);

        var filter = new TagNameFilter("p");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT, filter);

        // Forward traverse to end
        iterator.nextNode(); // root (p)
        iterator.nextNode(); // child2 (p)

        // Act & Assert - Backward traverse should skip span
        Assert.Same(root, iterator.previousNode());
        Assert.Null(iterator.previousNode());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void NextNode_NullFilter_AcceptsAllMatchingWhatToShow()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var child = CreateTestElement(doc, "span");
        root.appendChild(child);
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT, null);

        // Act & Assert
        Assert.Same(root, iterator.nextNode());
        Assert.Same(child, iterator.nextNode());
        Assert.Null(iterator.nextNode());
    }

    [Fact]
    public void NextNode_RepeatedCalls_ReturnsSameNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        iterator.nextNode(); // root
        var null1 = iterator.nextNode();
        var null2 = iterator.nextNode();
        var null3 = iterator.nextNode();

        // Assert
        Assert.Null(null1);
        Assert.Null(null2);
        Assert.Null(null3);
    }

    [Fact]
    public void PreviousNode_RepeatedCalls_ReturnsSameNull()
    {
        // Arrange
        var doc = CreateTestDocument();
        var root = CreateTestElement(doc, "div");
        var iterator = CreateNodeIterator(root, ENodeFilterMask.SHOW_ELEMENT);

        // Act
        var null1 = iterator.previousNode();
        var null2 = iterator.previousNode();
        var null3 = iterator.previousNode();

        // Assert
        Assert.Null(null1);
        Assert.Null(null2);
        Assert.Null(null3);
    }

    #endregion

    #region Filter Classes

    private class TagNameFilter : NodeFilter
    {
        private readonly string _tagName;

        public TagNameFilter(string tagName)
        {
            _tagName = tagName.ToUpperInvariant();
        }

        public override ENodeFilterResult acceptNode(Node node)
        {
            if (node is Element element && string.Equals(element.tagName, _tagName, StringComparison.OrdinalIgnoreCase))
                return ENodeFilterResult.FILTER_ACCEPT;
            return ENodeFilterResult.FILTER_SKIP;
        }
    }

    private class AcceptAllFilter : NodeFilter
    {
        public override ENodeFilterResult acceptNode(Node node)
        {
            return ENodeFilterResult.FILTER_ACCEPT;
        }
    }

    private class RejectAllFilter : NodeFilter
    {
        public override ENodeFilterResult acceptNode(Node node)
        {
            return ENodeFilterResult.FILTER_REJECT;
        }
    }

    #endregion
}
