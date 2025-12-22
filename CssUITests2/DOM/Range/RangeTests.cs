using System;
using System.Collections.Generic;
using CssUI.DOM;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.DOM.Nodes;
using Xunit;

namespace CssUITests.DOM.Range;

/// <summary>
/// Unit tests for DOM Range implementation.
/// Spec: https://dom.spec.whatwg.org/#interface-range
/// </summary>
public class RangeTests
{
    #region Test Infrastructure

    private static Document CreateHTMLDocument()
    {
        var dom = new DOMImplementation();
        return dom.createHTMLDocument("Test");
    }

    private static Element CreateElement(Document doc, string tagName)
    {
        return doc.createElement(tagName, new ElementCreationOptions(string.Empty));
    }

    private static Text CreateTextNode(Document doc, string data)
    {
        return doc.createTextNode(data);
    }

    private static Comment CreateComment(Document doc, string data)
    {
        return doc.createComment(data);
    }

    /// <summary>
    /// Creates a simple DOM structure for testing:
    /// <body>
    ///   <div>
    ///     <p>Hello World</p>
    ///     <p>Goodbye World</p>
    ///   </div>
    /// </body>
    /// </summary>
    private static (Document doc, Element container, Element para1, Element para2, Text text1, Text text2) CreateTestDOM()
    {
        var doc = CreateHTMLDocument();
        var body = doc.body!;

        var container = CreateElement(doc, "div");
        body.appendChild(container);

        var para1 = CreateElement(doc, "p");
        var text1 = CreateTextNode(doc, "Hello World");
        para1.appendChild(text1);
        container.appendChild(para1);

        var para2 = CreateElement(doc, "p");
        var text2 = CreateTextNode(doc, "Goodbye World");
        para2.appendChild(text2);
        container.appendChild(para2);

        return (doc, container, para1, para2, text1, text2);
    }

    #endregion

    #region createRange Tests

    [Fact]
    public void CreateRange_ReturnsNewRange()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var range = doc.createRange();

        // Assert
        Assert.NotNull(range);
    }

    [Fact]
    public void CreateRange_StartContainerIsBody()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var range = doc.createRange();

        // Assert
        Assert.Same(doc.body, range.startContainer);
    }

    [Fact]
    public void CreateRange_EndContainerIsBody()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var range = doc.createRange();

        // Assert
        Assert.Same(doc.body, range.endContainer);
    }

    [Fact]
    public void CreateRange_StartOffsetIsZero()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var range = doc.createRange();

        // Assert
        Assert.Equal(0, range.startOffset);
    }

    [Fact]
    public void CreateRange_EndOffsetIsZero()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var range = doc.createRange();

        // Assert
        Assert.Equal(0, range.endOffset);
    }

    [Fact]
    public void CreateRange_IsCollapsed()
    {
        // Arrange
        var doc = CreateHTMLDocument();

        // Act
        var range = doc.createRange();

        // Assert
        Assert.True(range.collapsed);
    }

    #endregion

    #region setStart Tests

    [Fact]
    public void SetStart_SetsStartContainer()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setStart(para1, 0);

        // Assert
        Assert.Same(para1, range.startContainer);
    }

    [Fact]
    public void SetStart_SetsStartOffset()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setStart(text1, 5);

        // Assert
        Assert.Equal(5, range.startOffset);
    }

    [Fact]
    public void SetStart_WithDocumentType_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.setStart(doc.doctype!, 0));
    }

    [Fact]
    public void SetStart_WithOffsetGreaterThanLength_ThrowsIndexSizeError()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act & Assert
        Assert.Throws<IndexSizeError>(() => range.setStart(text1, 100));
    }

    [Fact]
    public void SetStart_AfterEnd_SetsEndToStart()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act - set start after current end
        range.setStart(text1, 10);

        // Assert - end should be set to the new start
        Assert.Equal(10, range.endOffset);
        Assert.Same(text1, range.endContainer);
    }

    #endregion

    #region setEnd Tests

    [Fact]
    public void SetEnd_SetsEndContainer()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setEnd(para2, 1);

        // Assert
        Assert.Same(para2, range.endContainer);
    }

    [Fact]
    public void SetEnd_SetsEndOffset()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setEnd(text2, 7);

        // Assert
        Assert.Equal(7, range.endOffset);
    }

    [Fact]
    public void SetEnd_WithDocumentType_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.setEnd(doc.doctype!, 0));
    }

    [Fact]
    public void SetEnd_WithOffsetGreaterThanLength_ThrowsIndexSizeError()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act & Assert
        Assert.Throws<IndexSizeError>(() => range.setEnd(text1, 100));
    }

    [Fact]
    public void SetEnd_BeforeStart_SetsStartToEnd()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 10);

        // Act - set end before current start
        range.setEnd(text1, 2);

        // Assert - start should be set to the new end
        Assert.Equal(2, range.startOffset);
        Assert.Same(text1, range.startContainer);
    }

    #endregion

    #region setStartBefore/setStartAfter Tests

    [Fact]
    public void SetStartBefore_SetsStartToParentAtIndex()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setStartBefore(para2);

        // Assert
        Assert.Same(container, range.startContainer);
        Assert.Equal(para2.index, range.startOffset);
    }

    [Fact]
    public void SetStartAfter_SetsStartToParentAtIndexPlusOne()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setStartAfter(para1);

        // Assert
        Assert.Same(container, range.startContainer);
        Assert.Equal(para1.index + 1, range.startOffset);
    }

    [Fact]
    public void SetStartBefore_NodeWithNoParent_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();
        var orphan = CreateElement(doc, "div");

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.setStartBefore(orphan));
    }

    #endregion

    #region setEndBefore/setEndAfter Tests

    [Fact]
    public void SetEndBefore_SetsEndToParentAtIndex()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setEndBefore(para2);

        // Assert
        Assert.Same(container, range.endContainer);
        Assert.Equal(para2.index, range.endOffset);
    }

    [Fact]
    public void SetEndAfter_SetsEndToParentAtIndexPlusOne()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.setEndAfter(para2);

        // Assert
        Assert.Same(container, range.endContainer);
        Assert.Equal(para2.index + 1, range.endOffset);
    }

    [Fact]
    public void SetEndBefore_NodeWithNoParent_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();
        var orphan = CreateElement(doc, "div");

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.setEndBefore(orphan));
    }

    #endregion

    #region collapse Tests

    [Fact]
    public void Collapse_ToStart_SetsEndToStart()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);

        // Act
        range.collapse(toStart: true);

        // Assert
        Assert.Same(text1, range.endContainer);
        Assert.Equal(2, range.endOffset);
        Assert.True(range.collapsed);
    }

    [Fact]
    public void Collapse_ToEnd_SetsStartToEnd()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);

        // Act
        range.collapse(toStart: false);

        // Assert
        Assert.Same(text1, range.startContainer);
        Assert.Equal(8, range.startOffset);
        Assert.True(range.collapsed);
    }

    [Fact]
    public void Collapse_DefaultIsFalse_CollapsesToEnd()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);

        // Act
        range.collapse();

        // Assert
        Assert.Equal(8, range.startOffset);
    }

    #endregion

    #region selectNode Tests

    [Fact]
    public void SelectNode_SetsRangeAroundNode()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.selectNode(para1);

        // Assert
        Assert.Same(container, range.startContainer);
        Assert.Same(container, range.endContainer);
        Assert.Equal(para1.index, range.startOffset);
        Assert.Equal(para1.index + 1, range.endOffset);
    }

    [Fact]
    public void SelectNode_NodeWithNoParent_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();
        var orphan = CreateElement(doc, "div");

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.selectNode(orphan));
    }

    #endregion

    #region selectNodeContents Tests

    [Fact]
    public void SelectNodeContents_SetsRangeToNodeContents()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.selectNodeContents(container);

        // Assert
        Assert.Same(container, range.startContainer);
        Assert.Same(container, range.endContainer);
        Assert.Equal(0, range.startOffset);
        Assert.Equal(container.childNodes.Count, range.endOffset);
    }

    [Fact]
    public void SelectNodeContents_TextNode_SetsRangeToTextLength()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();

        // Act
        range.selectNodeContents(text1);

        // Assert
        Assert.Same(text1, range.startContainer);
        Assert.Same(text1, range.endContainer);
        Assert.Equal(0, range.startOffset);
        Assert.Equal(text1.Length, range.endOffset);
    }

    [Fact]
    public void SelectNodeContents_DocumentType_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.selectNodeContents(doc.doctype!));
    }

    #endregion

    #region commonAncestorContainer Tests

    [Fact]
    public void CommonAncestorContainer_SameNode_ReturnsNode()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act & Assert
        Assert.Same(text1, range.commonAncestorContainer);
    }

    [Fact]
    public void CommonAncestorContainer_SiblingNodes_ReturnsParent()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(para1, 0);
        range.setEnd(para2, 0);

        // Act & Assert
        Assert.Same(container, range.commonAncestorContainer);
    }

    [Fact]
    public void CommonAncestorContainer_CrossingNodes_ReturnsCommonAncestor()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text2, 5);

        // Act & Assert
        Assert.Same(container, range.commonAncestorContainer);
    }

    #endregion

    #region compareBoundaryPoints Tests

    [Fact]
    public void CompareBoundaryPoints_StartToStart_BeforeReturnsNegative()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range1 = doc.createRange();
        range1.setStart(text1, 0);
        range1.setEnd(text1, 5);

        var range2 = doc.createRange();
        range2.setStart(text1, 5);
        range2.setEnd(text1, 10);

        // Act
        var result = range1.compareBoundaryPoints(EBoundaryComparison.START_TO_START, range2);

        // Assert
        Assert.Equal(EBoundaryPosition.Before, result);
    }

    [Fact]
    public void CompareBoundaryPoints_StartToStart_EqualReturnsEqual()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range1 = doc.createRange();
        range1.setStart(text1, 5);
        range1.setEnd(text1, 10);

        var range2 = doc.createRange();
        range2.setStart(text1, 5);
        range2.setEnd(text1, 8);

        // Act
        var result = range1.compareBoundaryPoints(EBoundaryComparison.START_TO_START, range2);

        // Assert
        Assert.Equal(EBoundaryPosition.Equal, result);
    }

    [Fact]
    public void CompareBoundaryPoints_EndToEnd_AfterReturnsPositive()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range1 = doc.createRange();
        range1.setStart(text1, 0);
        range1.setEnd(text1, 10);

        var range2 = doc.createRange();
        range2.setStart(text1, 0);
        range2.setEnd(text1, 5);

        // Act
        var result = range1.compareBoundaryPoints(EBoundaryComparison.END_TO_END, range2);

        // Assert
        Assert.Equal(EBoundaryPosition.After, result);
    }

    [Fact]
    public void CompareBoundaryPoints_DifferentRoots_ThrowsWrongDocumentError()
    {
        // Arrange
        var doc1 = CreateHTMLDocument();
        var doc2 = CreateHTMLDocument();
        var range1 = doc1.createRange();
        var range2 = doc2.createRange();

        // Act & Assert
        Assert.Throws<WrongDocumentError>(() => range1.compareBoundaryPoints(EBoundaryComparison.START_TO_START, range2));
    }

    #endregion

    #region cloneRange Tests

    [Fact]
    public void CloneRange_CreatesNewRange()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);

        // Act
        var clone = range.cloneRange();

        // Assert
        Assert.NotSame(range, clone);
    }

    [Fact]
    public void CloneRange_HasSameStartContainer()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);

        // Act
        var clone = range.cloneRange();

        // Assert
        Assert.Same(range.startContainer, clone.startContainer);
    }

    [Fact]
    public void CloneRange_HasSameOffsets()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);

        // Act
        var clone = range.cloneRange();

        // Assert
        Assert.Equal(range.startOffset, clone.startOffset);
        Assert.Equal(range.endOffset, clone.endOffset);
    }

    [Fact]
    public void CloneRange_ModifyingOriginalDoesNotAffectClone()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 2);
        range.setEnd(text1, 8);
        var clone = range.cloneRange();

        // Act
        range.setStart(text1, 0);

        // Assert
        Assert.Equal(2, clone.startOffset);
    }

    #endregion

    #region isPointInRange Tests

    [Fact]
    public void IsPointInRange_PointInsideRange_ReturnsTrue()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 10);

        // Act
        var result = range.isPointInRange(text1, 5);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPointInRange_PointBeforeRange_ReturnsFalse()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 10);

        // Act
        var result = range.isPointInRange(text1, 2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsPointInRange_PointAfterRange_ReturnsFalse()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var result = range.isPointInRange(text1, 8);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsPointInRange_DifferentRoot_ReturnsFalse()
    {
        // Arrange
        var doc1 = CreateHTMLDocument();
        var doc2 = CreateHTMLDocument();
        var range = doc1.createRange();
        var otherText = CreateTextNode(doc2, "test");
        doc2.body!.appendChild(otherText);

        // Act
        var result = range.isPointInRange(otherText, 0);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsPointInRange_DocumentType_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.isPointInRange(doc.doctype!, 0));
    }

    [Fact]
    public void IsPointInRange_OffsetGreaterThanLength_ThrowsIndexSizeError()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 10);

        // Act & Assert
        Assert.Throws<IndexSizeError>(() => range.isPointInRange(text1, 100));
    }

    #endregion

    #region comparePoint Tests

    [Fact]
    public void ComparePoint_PointBeforeRange_ReturnsNegative()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 10);

        // Act
        var result = range.comparePoint(text1, 2);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void ComparePoint_PointInsideRange_ReturnsZero()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 10);

        // Act
        var result = range.comparePoint(text1, 5);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void ComparePoint_PointAfterRange_ReturnsPositive()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var result = range.comparePoint(text1, 8);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void ComparePoint_DifferentRoot_ThrowsWrongDocumentError()
    {
        // Arrange
        var doc1 = CreateHTMLDocument();
        var doc2 = CreateHTMLDocument();
        var range = doc1.createRange();
        var otherText = CreateTextNode(doc2, "test");
        doc2.body!.appendChild(otherText);

        // Act & Assert
        Assert.Throws<WrongDocumentError>(() => range.comparePoint(otherText, 0));
    }

    #endregion

    #region intersectsNode Tests

    [Fact]
    public void IntersectsNode_NodeInRange_ReturnsTrue()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNodeContents(container);

        // Act
        var result = range.intersectsNode(para1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IntersectsNode_NodeOutsideRange_ReturnsFalse()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNode(para1);

        // Act
        var result = range.intersectsNode(para2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IntersectsNode_DifferentRoot_ReturnsFalse()
    {
        // Arrange
        var doc1 = CreateHTMLDocument();
        var doc2 = CreateHTMLDocument();
        var range = doc1.createRange();
        var otherDiv = CreateElement(doc2, "div");
        doc2.body!.appendChild(otherDiv);

        // Act
        var result = range.intersectsNode(otherDiv);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_NodeFullyInRange_ReturnsTrue()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNodeContents(container);

        // Act
        var result = range.Contains(para1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_NodeOutsideRange_ReturnsFalse()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNode(para1);

        // Act - para2 is outside the range
        var result = range.Contains(para2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_DifferentRoot_ReturnsFalse()
    {
        // Arrange
        var doc1 = CreateHTMLDocument();
        var doc2 = CreateHTMLDocument();
        var range = doc1.createRange();
        var otherDiv = CreateElement(doc2, "div");
        doc2.body!.appendChild(otherDiv);

        // Act
        var result = range.Contains(otherDiv);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region PartiallyContains Tests

    [Fact]
    public void PartiallyContains_AncestorOfStartNotEnd_ReturnsTrue()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text2, 5);

        // Act - para1 is an ancestor of start (text1) but not end (text2)
        var result = range.PartiallyContains(para1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void PartiallyContains_AncestorOfBothStartAndEnd_ReturnsFalse()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text2, 5);

        // Act - container is an ancestor of both start and end
        var result = range.PartiallyContains(container);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void PartiallyContains_NotAncestorOfEither_ReturnsFalse()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNode(para1);

        // Act - para2 is not an ancestor of para1
        var result = range.PartiallyContains(para2);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region deleteContents Tests

    [Fact]
    public void DeleteContents_CollapsedRange_DoesNothing()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 5);
        var originalText = text1.data;

        // Act
        range.deleteContents();

        // Assert
        Assert.Equal(originalText, text1.data);
    }

    [Fact]
    public void DeleteContents_SameTextNode_DeletesPartOfText()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        range.deleteContents();

        // Assert - "Hello" should be removed from "Hello World"
        Assert.Equal(" World", text1.data);
    }

    [Fact]
    public void DeleteContents_RangeBecomesCollapsed()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        range.deleteContents();

        // Assert
        Assert.True(range.collapsed);
    }

    #endregion

    #region cloneContents Tests

    [Fact]
    public void CloneContents_CollapsedRange_ReturnsEmptyFragment()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 5);

        // Act
        var fragment = range.cloneContents();

        // Assert
        Assert.NotNull(fragment);
        Assert.False(fragment!.hasChildNodes());
    }

    [Fact]
    public void CloneContents_SameTextNode_ReturnsTextFragment()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var fragment = range.cloneContents();

        // Assert
        Assert.NotNull(fragment);
        Assert.True(fragment!.hasChildNodes());
        Assert.IsType<Text>(fragment.firstChild);
        Assert.Equal("Hello", ((Text)fragment.firstChild!).data);
    }

    [Fact]
    public void CloneContents_DoesNotModifyOriginal()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);
        var originalText = text1.data;

        // Act
        var fragment = range.cloneContents();

        // Assert
        Assert.Equal(originalText, text1.data);
    }

    #endregion

    #region extractContents Tests

    [Fact]
    public void ExtractContents_CollapsedRange_ReturnsEmptyFragment()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 5);

        // Act
        var fragment = range.extractContents();

        // Assert
        Assert.NotNull(fragment);
        Assert.False(fragment!.hasChildNodes());
    }

    [Fact]
    public void ExtractContents_SameTextNode_ExtractsText()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var fragment = range.extractContents();

        // Assert - fragment should contain "Hello"
        Assert.NotNull(fragment);
        Assert.True(fragment!.hasChildNodes());
        Assert.IsType<Text>(fragment.firstChild);
        Assert.Equal("Hello", ((Text)fragment.firstChild!).data);
    }

    [Fact]
    public void ExtractContents_SameTextNode_RemovesFromOriginal()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var fragment = range.extractContents();

        // Assert - original text should have "Hello" removed
        Assert.Equal(" World", text1.data);
    }

    [Fact]
    public void ExtractContents_RangeBecomesCollapsed()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var fragment = range.extractContents();

        // Assert
        Assert.True(range.collapsed);
    }

    #endregion

    #region insertNode Tests

    [Fact]
    public void InsertNode_InsertsNodeAtStart()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNodeContents(container);
        var newDiv = CreateElement(doc, "span");

        // Act
        range.insertNode(newDiv);

        // Assert
        Assert.Same(newDiv, container.firstChild);
    }

    [Fact]
    public void InsertNode_InTextNode_SplitsText()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 5);
        var newSpan = CreateElement(doc, "span");

        // Act
        range.insertNode(newSpan);

        // Assert - text should be split at offset 5
        Assert.Equal("Hello", ((Text)para1.firstChild!).data);
        Assert.Same(newSpan, para1.childNodes[1]);
        Assert.Equal(" World", ((Text)para1.lastChild!).data);
    }

    [Fact]
    public void InsertNode_ProcessingInstructionAsContainer_ThrowsHierarchyRequestError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var pi = doc.createProcessingInstruction("test", "data");
        doc.appendChild(pi);
        var range = new CssUI.DOM.Range(pi, 0, pi, 0);
        var newDiv = CreateElement(doc, "div");

        // Act & Assert
        Assert.Throws<HierarchyRequestError>(() => range.insertNode(newDiv));
    }

    #endregion

    #region surroundContents Tests

    [Fact]
    public void SurroundContents_WrapsContentsInNewParent()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 11); // "Hello World"
        var wrapper = CreateElement(doc, "span");

        // Act
        range.surroundContents(wrapper);

        // Assert - the text should now be wrapped in the span
        Assert.Contains(wrapper, para1.childNodes);
        Assert.Equal("Hello World", wrapper.textContent);
    }

    [Fact]
    public void SurroundContents_Document_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();
        var newDoc = new DOMImplementation().createDocument("CssUI", "root");

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.surroundContents(newDoc));
    }

    [Fact]
    public void SurroundContents_DocumentType_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();
        var doctype = doc.doctype!;

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.surroundContents(doctype));
    }

    [Fact]
    public void SurroundContents_DocumentFragment_ThrowsInvalidNodeTypeError()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();
        var fragment = doc.createDocumentFragment();

        // Act & Assert
        Assert.Throws<InvalidNodeTypeError>(() => range.surroundContents(fragment));
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_CollapsedRange_ReturnsEmptyString()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 5);
        range.setEnd(text1, 5);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToString_SameTextNode_ReturnsSubstring()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.setStart(text1, 0);
        range.setEnd(text1, 5);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("Hello", result);
    }

    [Fact]
    public void ToString_EntireTextNode_ReturnsFullText()
    {
        // Arrange
        var (doc, container, para1, para2, text1, text2) = CreateTestDOM();
        var range = doc.createRange();
        range.selectNodeContents(text1);

        // Act
        var result = range.ToString();

        // Assert
        Assert.Equal("Hello World", result);
    }

    #endregion

    #region root Property Tests

    [Fact]
    public void Root_ReturnsRootNode()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();

        // Act
        var root = range.root;

        // Assert
        Assert.Same(doc, root);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var doc = CreateHTMLDocument();
        var range = doc.createRange();

        // Act & Assert - should not throw
        range.Dispose();
        range.Dispose();
    }

    #endregion
}
