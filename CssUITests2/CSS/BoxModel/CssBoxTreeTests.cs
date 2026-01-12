using System;
using System.Linq;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;

namespace CssUITests.CSS.BoxModel;

/// <summary>
/// Unit tests for CssBoxTree box generation logic.
/// Tests the CSS Display 3 box generation rules including:
/// - display: contents support
/// - Text sequence handling
/// - Automatic box type transformations (blockification/inlinification)
/// - Anonymous block box generation
///
/// See: https://www.w3.org/TR/css-display-3/#box-generation
/// See: https://www.w3.org/TR/CSS22/visuren.html#box-gen
/// </summary>
public class CssBoxTreeTests : IDisposable
{
    private readonly LayoutTestFixture _fixture;
    private static readonly ElementCreationOptions DefaultOptions = new(string.Empty);

    public CssBoxTreeTests()
    {
        _fixture = new LayoutTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }

    #region DisplayType Contents Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayType_Contents_HasContentsOuterAndNoneInner()
    {
        // Arrange & Act
        var displayType = new DisplayType(EDisplayMode.CONTENT);

        // Assert
        Assert.Equal(EOuterDisplayType.Contents, displayType.Outer);
        Assert.Equal(EInnerDisplayType.None, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayContents_Element_GeneratesNoBox()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var contentsDiv = doc.createElement("div", DefaultOptions);
        contentsDiv!.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        parent.appendChild(contentsDiv);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert - element with display: contents should have no box
        Assert.Null(contentsDiv.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayContents_ChildrenStillGenerateBoxes()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var contentsDiv = doc.createElement("div", DefaultOptions);
        contentsDiv!.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        parent.appendChild(contentsDiv);

        var child = doc.createElement("div", DefaultOptions);
        child!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        contentsDiv.appendChild(child);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert - child should still have a box even though parent is display: contents
        Assert.NotNull(child.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    [Trait("Category", "DisplayContents")]
    public void DisplayContents_ReplacedElement_ComputesToDisplayNone()
    {
        // Arrange - Per CSS Display 3 §2.5: "display: contents" computes to "display: none"
        // on replaced elements and other elements whose rendering is not entirely controlled by CSS.
        // Spec: https://www.w3.org/TR/css-display-3/#valdef-display-contents
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        // Create an img element (replaced element per HTML spec)
        var img = doc.createElement("img", DefaultOptions);
        img!.SetFlag(ENodeFlags.IsReplaced, true); // Mark as replaced element
        img.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        parent.appendChild(img);

        // Act - trigger style cascade
        img.Style.Cascade();

        // Assert - for replaced elements, display: contents should compute to display: none
        Assert.Equal(EDisplayMode.NONE, img.Style.Display);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    [Trait("Category", "DisplayContents")]
    public void DisplayContents_ReplacedElement_GeneratesNoBox()
    {
        // Arrange - Per CSS Display 3 §2.5: replaced elements with display: contents
        // should be treated as display: none and generate no boxes
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        // Create an img element (replaced element per HTML spec)
        var img = doc.createElement("img", DefaultOptions);
        img!.SetFlag(ENodeFlags.IsReplaced, true); // Mark as replaced element
        img.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        parent.appendChild(img);

        // Act - use ForceLayoutUpdate to ensure cascade and box generation
        _fixture.ForceLayoutUpdate();

        // Assert - replaced element with display: contents should generate no box
        Assert.Null(img.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    [Trait("Category", "DisplayContents")]
    public void DisplayContents_NonReplacedElement_StaysAsContents()
    {
        // Arrange - Non-replaced elements should NOT have display: contents converted
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        // Create a div element (non-replaced element)
        var div = doc.createElement("div", DefaultOptions);
        div!.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        parent.appendChild(div);

        // Act - trigger style cascade
        div.Style.Cascade();

        // Assert - non-replaced elements should keep display: contents
        Assert.Equal(EDisplayMode.CONTENT, div.Style.Display);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    [Trait("Category", "DisplayContents")]
    public void DisplayContents_ReplacedElement_ChildrenAlsoHidden()
    {
        // Arrange - Per CSS Display 3 §2.5: When display: contents computes to display: none
        // on replaced elements, the element AND its descendants generate no boxes.
        // This is different from non-replaced elements where children still generate boxes.
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);

        // Create an object element (replaced element that can have fallback content)
        var objectEl = doc.createElement("object", DefaultOptions);
        objectEl!.SetFlag(ENodeFlags.IsReplaced, true); // Mark as replaced element
        objectEl.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        parent.appendChild(objectEl);

        // Add a fallback child element
        var fallback = doc.createElement("div", DefaultOptions);
        fallback!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        objectEl.appendChild(fallback);

        // Act - use ForceLayoutUpdate to ensure cascade and box generation
        _fixture.ForceLayoutUpdate();

        // Assert - because display: contents computes to display: none for replaced elements,
        // both the element and its descendants should have no boxes (like display: none)
        Assert.Null(objectEl.Box);
        Assert.Null(fallback.Box);
    }

    #endregion

    #region Text Sequence Handling Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void TextNode_WithContent_GeneratesTextRun()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var textNode = doc.createTextNode("Hello World");
        parent.appendChild(textNode);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(textNode.Box);
        Assert.IsType<CssTextRun>(textNode.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void TextNode_EmptyContent_GeneratesNoTextRun()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var textNode = doc.createTextNode("");
        parent.appendChild(textNode);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - empty text nodes should not generate text runs per spec
        Assert.Null(textNode.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void ContiguousTextNodes_GenerateSingleTextRun()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var text1 = doc.createTextNode("Hello ");
        var text2 = doc.createTextNode("World");
        parent.appendChild(text1);
        parent.appendChild(text2);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - contiguous text nodes should be combined into single text run
        // First text node gets the text run, subsequent ones share it
        Assert.NotNull(text1.Box);
        Assert.IsType<CssTextRun>(text1.Box);
        // Both text nodes should reference the same text run
        Assert.Same(text1.Box, text2.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void WhitespaceOnlyTextNode_GeneratesTextRun()
    {
        // Per CSS Display 3 §1: "If the sequence contains no text, however, it does not generate a text sequence."
        // Whitespace IS text content (not "no text"), so whitespace-only nodes correctly generate text sequences.
        // The white-space property (CSS Text 3 §4) controls how whitespace is rendered/collapsed during layout,
        // but that's handled separately from box tree generation.
        // @todo Phase 18.3: When white-space property is implemented, whitespace collapsing will be handled
        // during inline formatting context layout, not during box generation.
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var textNode = doc.createTextNode("   ");
        parent.appendChild(textNode);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - whitespace content is not empty, so it generates a text run
        Assert.NotNull(textNode.Box);
        Assert.IsType<CssTextRun>(textNode.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void MixedWhitespaceAndContent_GeneratesTextRun()
    {
        // Arrange - text nodes with mixed whitespace and content generate text runs
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var textNode = doc.createTextNode("  Hello  ");
        parent.appendChild(textNode);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(textNode.Box);
        Assert.IsType<CssTextRun>(textNode.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void TextNodesBetweenElements_GenerateSeparateTextRuns()
    {
        // Arrange - text nodes separated by elements should generate separate text runs
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var text1 = doc.createTextNode("Before ");
        var element = doc.createElement("span", DefaultOptions);
        var text2 = doc.createTextNode(" After");
        parent.appendChild(text1);
        parent.appendChild(element);
        parent.appendChild(text2);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - text nodes are not contiguous (separated by element)
        Assert.NotNull(text1.Box);
        Assert.NotNull(text2.Box);
        Assert.IsType<CssTextRun>(text1.Box);
        Assert.IsType<CssTextRun>(text2.Box);
        // They should be different text runs
        Assert.NotSame(text1.Box, text2.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "TextSequence")]
    public void TextRun_ParentIsContainerBox()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var textNode = doc.createTextNode("Hello");
        parent.appendChild(textNode);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert - text run's parent should be the parent element's box
        Assert.NotNull(textNode.Box);
        Assert.Same(parent.Box, textNode.Box.parentNode);
    }

    #endregion

    #region Blockification Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Blockification")]
    public void AbsolutelyPositioned_InlineElement_IsBlockified()
    {
        // Arrange
        var doc = _fixture.Document;
        var container = _fixture.CreateContainingBlock(500, 500);
        var inline = doc.createElement("span", DefaultOptions);
        inline!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        inline.Style.UserRules.Positioning.Set(EBoxPositioning.Absolute);
        container.appendChild(inline);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - absolutely positioned inline should be blockified
        // The computed display value should have block outer display
        var displayType = new DisplayType(inline.Style.Display);
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.NotNull(inline.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Blockification")]
    public void FixedPositioned_InlineElement_IsBlockified()
    {
        // Arrange
        var doc = _fixture.Document;
        var inline = doc.createElement("span", DefaultOptions);
        inline!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        inline.Style.UserRules.Positioning.Set(EBoxPositioning.Fixed);
        _fixture.Body.appendChild(inline);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - fixed positioned inline should be blockified
        var displayType = new DisplayType(inline.Style.Display);
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.NotNull(inline.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Blockification")]
    public void FlexContainer_InlineChild_IsBlockified()
    {
        // Arrange
        var doc = _fixture.Document;
        var flex = _fixture.CreateFlexContainer();
        var inline = doc.createElement("span", DefaultOptions);
        inline!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        flex.appendChild(inline);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - inline children of flex containers should be blockified
        var displayType = new DisplayType(inline.Style.Display);
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.NotNull(inline.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Blockification")]
    public void GridContainer_InlineChild_IsBlockified()
    {
        // Arrange
        var doc = _fixture.Document;
        var grid = _fixture.CreateGridContainer();
        var inline = doc.createElement("span", DefaultOptions);
        inline!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        grid.appendChild(inline);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - inline children of grid containers should be blockified
        var displayType = new DisplayType(inline.Style.Display);
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.NotNull(inline.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Blockification")]
    public void BlockElement_NotBlockified_RemainsBlock()
    {
        // Arrange
        var block = _fixture.CreateBlock(200, 100);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert
        var displayType = new DisplayType(block.Style.Display);
        Assert.Equal(EOuterDisplayType.Block, displayType.Outer);
        Assert.NotNull(block.Box);
    }

    #endregion

    #region Root Element Box Generation Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void RootElement_AlwaysGeneratesBlockLevelBox()
    {
        // Arrange & Act
        _fixture.ForceBoxGeneration();

        // Assert - root element should always have a box
        Assert.NotNull(_fixture.DocumentElement.Box);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void BodyElement_GeneratesBox()
    {
        // Arrange & Act
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.NotNull(_fixture.Body.Box);
    }

    #endregion

    #region Display None Tests

    [Fact(Skip = "Phase 14.6.4: display: none box suppression requires style cascade integration")]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayNone_GeneratesNoBox()
    {
        // Arrange
        var doc = _fixture.Document;
        var hidden = doc.createElement("div", DefaultOptions);
        hidden!.Style.UserRules.Display.Set(EDisplayMode.NONE);
        _fixture.Body.appendChild(hidden);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.Null(hidden.Box);
    }

    [Fact(Skip = "Phase 14.6.4: display: none box suppression requires style cascade integration")]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayNone_ChildrenGenerateNoBoxes()
    {
        // Arrange
        var doc = _fixture.Document;
        var hidden = doc.createElement("div", DefaultOptions);
        hidden!.Style.UserRules.Display.Set(EDisplayMode.NONE);
        _fixture.Body.appendChild(hidden);

        var child = doc.createElement("div", DefaultOptions);
        child!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        hidden.appendChild(child);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - children of display: none elements should also have no boxes
        Assert.Null(hidden.Box);
        Assert.Null(child.Box);
    }

    #endregion

    #region Anonymous Box Generation Tests

    /// <summary>
    /// CSS 2.2 §9.2.1.1: When a block container box has both inline-level and block-level
    /// children, anonymous block boxes are created to wrap the inline content.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void BlockInBlockContainer_HasInlineSiblings_WrapsInlineInAnonymousBox()
    {
        // Arrange
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(200, 100);

        // Add inline content first
        var inline = doc.createElement("span", DefaultOptions);
        inline!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline);

        // Add block element (triggers anonymous box scenario)
        var block = doc.createElement("div", DefaultOptions);
        block!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert - both elements should have boxes
        Assert.NotNull(inline.Box);
        Assert.NotNull(block.Box);
        Assert.NotNull(container.Box);

        // Debug: Check container.Box properties
        var containerBox = container.Box;
        Assert.True(containerBox.IsBlockContainer, "Container box should be a block container");

        // Debug: Check what children we have
        var childBoxes = containerBox.childNodes.OfType<CssBox>().ToList();
        Assert.NotEmpty(childBoxes);

        // Debug: Check each child's display properties
        foreach (var child in childBoxes)
        {
            var displayType = child.DisplayType;
            // Log info about each child for debugging
            var info = $"Child type: {child.GetType().Name}, Outer: {displayType.Outer}, IsBlockLevel: {child.IsBlockLevel}, IsInlineLevel: {child.IsInlineLevel}";

            // All direct children of the container should be block-level
            Assert.True(child.IsBlockLevel, $"Child box should be block-level. {info}");
        }
    }

    /// <summary>
    /// CSS 2.2 §9.2.1.1: When all children are block-level, no anonymous boxes are needed.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void BlockOnlyChildren_NoAnonymousBoxNeeded()
    {
        // Arrange
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(200, 100);

        var block1 = doc.createElement("div", DefaultOptions);
        block1!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block1);

        var block2 = doc.createElement("div", DefaultOptions);
        block2!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block2);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(block1.Box);
        Assert.NotNull(block2.Box);
        Assert.NotNull(container.Box);

        // No anonymous boxes should exist - both children should be direct children of container
        Assert.Equal(2, container.Box.childNodes.Count);
        Assert.Same(block1.Box, container.Box.childNodes[0]);
        Assert.Same(block2.Box, container.Box.childNodes[1]);
    }

    /// <summary>
    /// CSS 2.2 §9.2.1.1: When all children are inline-level, no anonymous boxes are needed.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void InlineOnlyChildren_NoAnonymousBoxNeeded()
    {
        // Arrange
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(200, 100);

        var inline1 = doc.createElement("span", DefaultOptions);
        inline1!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline1);

        var inline2 = doc.createElement("span", DefaultOptions);
        inline2!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline2);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(inline1.Box);
        Assert.NotNull(inline2.Box);
        Assert.NotNull(container.Box);

        // No anonymous boxes should exist - both children should be direct children of container
        Assert.Equal(2, container.Box.childNodes.Count);
        Assert.Same(inline1.Box, container.Box.childNodes[0]);
        Assert.Same(inline2.Box, container.Box.childNodes[1]);
    }

    /// <summary>
    /// CSS 2.2 §9.2.1.1: Multiple consecutive inline boxes should be wrapped in a single
    /// anonymous block box when interspersed with block content.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void MultipleInlineSiblings_WrappedInSingleAnonymousBox()
    {
        // Arrange: inline, inline, block, inline, inline
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(400, 100);

        var inline1 = doc.createElement("span", DefaultOptions);
        inline1!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline1);

        var inline2 = doc.createElement("span", DefaultOptions);
        inline2!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline2);

        var block = doc.createElement("div", DefaultOptions);
        block!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block);

        var inline3 = doc.createElement("span", DefaultOptions);
        inline3!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline3);

        var inline4 = doc.createElement("span", DefaultOptions);
        inline4!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline4);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(container.Box);

        // Should have 3 direct children: anon block (inline1+inline2), block, anon block (inline3+inline4)
        Assert.Equal(3, container.Box.childNodes.Count);

        // All direct children should be block-level
        foreach (var child in container.Box.childNodes.OfType<CssBox>())
        {
            Assert.True(child.IsBlockLevel);
        }

        // First and last should be anonymous boxes
        Assert.IsType<CssAnonymousBox>(container.Box.childNodes[0]);
        Assert.Same(block.Box, container.Box.childNodes[1]);
        Assert.IsType<CssAnonymousBox>(container.Box.childNodes[2]);

        // Anonymous boxes should contain the inline boxes
        var firstAnon = (CssAnonymousBox)container.Box.childNodes[0];
        var lastAnon = (CssAnonymousBox)container.Box.childNodes[2];

        Assert.Equal(2, firstAnon.childNodes.Count);
        Assert.Equal(2, lastAnon.childNodes.Count);
    }

    /// <summary>
    /// CSS 2.2 §9.2.1.1: Anonymous block boxes are instances of CssAnonymousBox.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void AnonymousBox_IsTypeOfCssAnonymousBox()
    {
        // Arrange
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(200, 100);

        var inline = doc.createElement("span", DefaultOptions);
        inline!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline);

        var block = doc.createElement("div", DefaultOptions);
        block!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert
        var anonymousBox = container.Box.childNodes.OfType<CssAnonymousBox>().FirstOrDefault();

        Assert.NotNull(anonymousBox);
        // CssAnonymousBox is the type used for anonymous boxes
        Assert.IsType<CssAnonymousBox>(anonymousBox);
    }

    /// <summary>
    /// CSS 2.2 §9.2.1.1: Block between two inlines creates two anonymous boxes.
    /// Pattern: inline, block, inline
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void BlockBetweenInlines_CreatesTwoAnonymousBoxes()
    {
        // Arrange: inline, block, inline
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(200, 100);

        var inline1 = doc.createElement("span", DefaultOptions);
        inline1!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline1);

        var block = doc.createElement("div", DefaultOptions);
        block!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block);

        var inline2 = doc.createElement("span", DefaultOptions);
        inline2!.Style.UserRules.Display.Set(EDisplayMode.INLINE);
        container.appendChild(inline2);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert
        // Should have 3 children: anon(inline1), block, anon(inline2)
        Assert.Equal(3, container.Box.childNodes.Count);
        Assert.IsType<CssAnonymousBox>(container.Box.childNodes[0]);
        Assert.Same(block.Box, container.Box.childNodes[1]);
        Assert.IsType<CssAnonymousBox>(container.Box.childNodes[2]);
    }

    /// <summary>
    /// CSS 2.2 §9.2.1.1: Inline-block is inline-level (outer=inline), so it needs wrapping
    /// when mixed with block content.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void InlineBlockWithBlock_InlineBlockIsWrapped()
    {
        // Arrange
        var doc = _fixture.Document;
        var container = _fixture.CreateBlock(200, 100);

        // Note: inline-block is inline-level (outer=inline), so it DOES need wrapping
        // when mixed with block content
        var inlineBlock = doc.createElement("div", DefaultOptions);
        inlineBlock!.Style.UserRules.Display.Set(EDisplayMode.INLINE_BLOCK);
        container.appendChild(inlineBlock);

        var block = doc.createElement("div", DefaultOptions);
        block!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        container.appendChild(block);

        // Act - use ForceLayoutUpdate to ensure cascade happens for Display property
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(container.Box);

        // inline-block is inline-level, so it should be wrapped in anonymous box
        Assert.Equal(2, container.Box.childNodes.Count);
        Assert.IsType<CssAnonymousBox>(container.Box.childNodes[0]);
        Assert.Same(block.Box, container.Box.childNodes[1]);
    }

    #endregion

    #region Closest Box Generating Ancestor Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void NestedElements_BoxParentIsNearestAncestorWithBox()
    {
        // Arrange
        var doc = _fixture.Document;
        var parent = _fixture.CreateBlock(200, 100);
        var child = doc.createElement("div", DefaultOptions);
        child!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        parent.appendChild(child);

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        Assert.NotNull(child.Box);
        Assert.NotNull(parent.Box);
        // The parent's box should be the parent of the child's box
        Assert.Same(parent.Box, child.Box.parentNode);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayContents_SkippedForBoxParent()
    {
        // Arrange - test display: contents behavior
        var doc = _fixture.Document;
        var grandparent = _fixture.CreateBlock(200, 100);

        // Create a parent with display: contents (generates no box)
        var contentsParent = doc.createElement("div", DefaultOptions);
        contentsParent!.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        grandparent.appendChild(contentsParent);

        // Create child that should skip display: contents parent
        var child = doc.createElement("div", DefaultOptions);
        child!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        contentsParent.appendChild(child);

        // Verify DOM structure
        Assert.Same(grandparent, contentsParent.parentElement);
        Assert.Same(contentsParent, child.parentElement);

        // Act
        _fixture.ForceLayoutUpdate();

        // Verify display: contents behavior
        Assert.NotNull(grandparent.Box);
        Assert.Null(contentsParent.Box); // display: contents generates no box
        Assert.NotNull(child.Box);

        // Child's box parent should skip display: contents and go to grandparent
        Assert.Same(grandparent.Box, child.Box.parentNode);
    }

    #endregion

    #region DisplayType Helper Method Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_From_CreatesCorrectDisplayType()
    {
        // Arrange & Act
        var displayType = DisplayType.From(EDisplayMode.INLINE_BLOCK);

        // Assert
        Assert.Equal(EOuterDisplayType.Inline, displayType.Outer);
        Assert.Equal(EInnerDisplayType.Flow_Root, displayType.Inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_GetOuter_ReturnsCorrectOuter()
    {
        // Arrange & Act
        var outer = DisplayType.Get_Outer(EDisplayMode.FLEX);

        // Assert
        Assert.Equal(EOuterDisplayType.Block, outer);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_GetInner_ReturnsCorrectInner()
    {
        // Arrange & Act
        var inner = DisplayType.Get_Inner(EDisplayMode.GRID);

        // Assert
        Assert.Equal(EInnerDisplayType.Grid, inner);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsBlockLevel_TrueForBlockOuter()
    {
        // Arrange
        var displayType = new DisplayType(EDisplayMode.BLOCK);

        // Assert
        Assert.True(displayType.IsBlockLevel);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsInlineLevel_TrueForInlineOuter()
    {
        // Arrange
        var displayType = new DisplayType(EDisplayMode.INLINE);

        // Assert
        Assert.True(displayType.IsInlineLevel);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsBlockContainer_TrueForFlowRoot()
    {
        // Arrange - block containers are elements that establish a new block formatting context
        // This includes flow-root and inline-block (which has flow-root inner)
        var inlineBlockDisplay = new DisplayType(EDisplayMode.INLINE_BLOCK);
        var flowRootDisplay = new DisplayType(EDisplayMode.FLOW_ROOT);

        // Assert - IsBlockContainer is true for flow-root inner display types
        Assert.True(inlineBlockDisplay.IsBlockContainer);
        Assert.True(flowRootDisplay.IsBlockContainer);
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Display")]
    public void DisplayType_IsBlockContainer_FalseForFlowInner()
    {
        // Arrange - flow inner is also a block container (inline is not)
        var blockDisplay = new DisplayType(EDisplayMode.BLOCK);
        var inlineDisplay = new DisplayType(EDisplayMode.INLINE);

        // Assert - block has flow-root inner per CSS Display Level 3, so it IS a block container
        // Only pure inline (flow inner) is NOT a block container
        Assert.True(blockDisplay.IsBlockContainer); // block -> flow-root inner
        Assert.False(inlineDisplay.IsBlockContainer); // inline -> flow inner
    }

    #endregion

    #region EOuterDisplayType Tests

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Enum")]
    public void EOuterDisplayType_HasContentsValue()
    {
        // Assert
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), EOuterDisplayType.Contents));
    }

    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "Enum")]
    public void EOuterDisplayType_HasExpectedValues()
    {
        // Assert
        Assert.Equal(0, (int)EOuterDisplayType.None);
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), EOuterDisplayType.Block));
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), EOuterDisplayType.Inline));
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), EOuterDisplayType.Run_In));
        Assert.True(System.Enum.IsDefined(typeof(EOuterDisplayType), EOuterDisplayType.Contents));
    }

    #endregion

    #region Run-In Box Tests (Phase 14.6.7 - DEFERRED)

    /// <summary>
    /// Tests that run-in display mode is recognized by the parser.
    /// Implementation of run-in box merging is deferred per Phase 14.6.7.
    /// Run-in is marked "at-risk" in CSS Display Level 3 and has no browser support.
    /// </summary>
    [Fact]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    [Trait("Category", "RunIn")]
    [Trait("Category", "Deferred")]
    public void RunIn_DisplayMode_IsRecognizedByParser()
    {
        // Arrange & Act
        var displayType = new DisplayType(EDisplayMode.RUN_IN);

        // Assert - run-in is recognized and has Run_In outer display type
        Assert.Equal(EOuterDisplayType.Run_In, displayType.Outer);
    }

    /// <summary>
    /// Documents that run-in box merging behavior is not yet implemented.
    /// Run-in boxes should merge into subsequent block containers, but this is deferred.
    /// For now, run-in elements generate boxes without special merging behavior.
    /// </summary>
    [Fact(Skip = "Run-in box merging is deferred per Phase 14.6.7 (at-risk feature)")]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    [Trait("Category", "RunIn")]
    [Trait("Category", "Deferred")]
    public void RunIn_MergesIntoSubsequentBlock_WhenImplemented()
    {
        // This test documents the expected behavior when run-in is implemented:
        // <div style="display: run-in">Run-in</div>
        // <div style="display: block">Block content</div>
        //
        // Expected result: Run-in content appears at the start of the block,
        // as if the markup was: <div style="display: block"><span>Run-in</span> Block content</div>
        //
        // Implementation requirements per CSS Display 3 §5:
        // 1. Detect run-in sequences (consecutive run-in boxes + whitespace/out-of-flow)
        // 2. If followed by block box that doesn't establish BFC, reparent run-in as first inline child
        // 3. Insert after ::marker (if any), before other content including ::before
        // 4. Otherwise generate anonymous block wrapper around run-in sequence
        Assert.True(false, "Run-in merging not implemented - deferred per Phase 14.6.7");
    }

    #endregion
}
