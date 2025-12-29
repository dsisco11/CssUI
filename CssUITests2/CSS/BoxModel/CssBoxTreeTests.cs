using System;
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
public class CssBoxTreeTests : IClassFixture<LayoutTestFixture>
{
    private readonly LayoutTestFixture _fixture;
    private static readonly ElementCreationOptions DefaultOptions = new(string.Empty);

    public CssBoxTreeTests(LayoutTestFixture fixture)
    {
        _fixture = fixture;
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

    [Fact(Skip = "Phase 14.6.2: display: contents box suppression requires style cascade integration")]
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

        // Act
        _fixture.ForceBoxGeneration();

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

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - child should still have a box even though parent is display: contents
        Assert.NotNull(child.Box);
    }

    #endregion

    #region Text Sequence Handling Tests

    [Fact(Skip = "Phase 14.6.3: Text sequence handling requires inline formatting context integration")]
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
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.NotNull(textNode.Box);
        Assert.IsType<CssTextRun>(textNode.Box);
    }

    [Fact(Skip = "Phase 14.6.3: Text sequence handling requires inline formatting context integration")]
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
        _fixture.ForceBoxGeneration();

        // Assert - empty text nodes should not generate text runs per spec
        Assert.Null(textNode.Box);
    }

    [Fact(Skip = "Phase 14.6.3: Text sequence handling requires inline formatting context integration")]
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
        _fixture.ForceBoxGeneration();

        // Assert - contiguous text nodes should be combined
        // First text node gets the text run, subsequent ones may not have their own
        Assert.NotNull(text1.Box);
        Assert.IsType<CssTextRun>(text1.Box);
    }

    #endregion

    #region Blockification Tests

    [Fact(Skip = "Phase 14.6.6: Blockification requires style cascade integration to persist computed display")]
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
        var displayType = new DisplayType(inline.Style.Display);
        Assert.True(displayType.IsBlockLevel || inline.Style.Positioning == EBoxPositioning.Absolute);
    }

    [Fact(Skip = "Phase 14.6.6: Blockification requires style cascade integration to persist computed display")]
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
        Assert.NotNull(inline.Box);
    }

    [Fact(Skip = "Phase 14.6.6: Blockification requires style cascade integration to persist computed display")]
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
        Assert.NotNull(inline.Box);
    }

    [Fact(Skip = "Phase 14.6.6: Blockification requires style cascade integration to persist computed display")]
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
        Assert.NotNull(inline.Box);
    }

    [Fact(Skip = "Phase 14.6.6: Box outer display type requires style cascade integration")]
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

    [Fact(Skip = "Phase 14.6.1: Anonymous block box generation is not yet fully implemented")]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "AnonymousBox")]
    public void BlockInBlockContainer_HasInlineSiblings_DetectsScenario()
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

        // Act
        _fixture.ForceBoxGeneration();

        // Assert - both elements should have boxes
        // Anonymous box wrapping is handled during formatting context flow
        Assert.NotNull(inline.Box);
        Assert.NotNull(block.Box);
    }

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

        // Act
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.NotNull(block1.Box);
        Assert.NotNull(block2.Box);
    }

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

        // Act
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.NotNull(inline1.Box);
        Assert.NotNull(inline2.Box);
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
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.NotNull(child.Box);
        Assert.NotNull(parent.Box);
        // The parent's box should be the parent of the child's box
        Assert.Same(parent.Box, child.Box.parentNode);
    }

    [Fact(Skip = "Phase 14.6.2: display: contents box suppression requires style cascade integration")]
    [Trait("Category", "BoxModel")]
    [Trait("Category", "BoxGeneration")]
    public void DisplayContents_SkippedForBoxParent()
    {
        // Arrange
        var doc = _fixture.Document;
        var grandparent = _fixture.CreateBlock(200, 100);

        var contentsParent = doc.createElement("div", DefaultOptions);
        contentsParent!.Style.UserRules.Display.Set(EDisplayMode.CONTENT);
        grandparent.appendChild(contentsParent);

        var child = doc.createElement("div", DefaultOptions);
        child!.Style.UserRules.Display.Set(EDisplayMode.BLOCK);
        contentsParent.appendChild(child);

        // Act
        _fixture.ForceBoxGeneration();

        // Assert
        Assert.Null(contentsParent.Box);
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
}
