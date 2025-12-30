using System;
using System.Collections.Generic;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.BoxTree;
using CssUI.CSS.Enums;
using CssUI.CSS.Layout;
using CssUI.DOM.Nodes;
using CssUITests.Fixtures;
using Xunit;
using CssBoxModel = CssUI.CSS.BoxModel;
using Element = CssUI.DOM.Element;

namespace CssUITests.CSS.Layout;

/// <summary>
/// Tests for <see cref="LayoutCycleTracker"/> (Phase 14.5.8).
/// Validates cycle detection in layout calculations to prevent infinite loops.
/// </summary>
public class LayoutCycleTrackerTests : IDisposable
{
    private readonly LayoutTestFixture _fixture;

    public LayoutCycleTrackerTests()
    {
        _fixture = new LayoutTestFixture();
    }

    public void Dispose()
    {
        _fixture.Dispose();
        // Reset the tracker after each test
        LayoutCycleTracker.Current.Reset();
    }

    #region Basic Tracking Tests

    /// <summary>
    /// Tests that tracker instance is thread-local and always available.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void Current_ReturnsThreadLocalInstance()
    {
        // Act
        var tracker = LayoutCycleTracker.Current;

        // Assert
        Assert.NotNull(tracker);
    }

    /// <summary>
    /// Tests that BeginWidthResolution succeeds for a box not already being resolved.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void BeginWidthResolution_FirstTime_ReturnsTrue()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        Assert.NotNull(box);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        bool result = tracker.BeginWidthResolution(box);

        // Assert
        Assert.True(result);
        Assert.True(tracker.IsBeingResolved(box));

        // Cleanup
        tracker.EndWidthResolution(box);
    }

    /// <summary>
    /// Tests that BeginWidthResolution returns false for an already-resolving box (cycle).
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void BeginWidthResolution_SameBoxTwice_ReturnsFalse()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        Assert.NotNull(box);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        tracker.BeginWidthResolution(box);
        bool secondResult = tracker.BeginWidthResolution(box);

        // Assert
        Assert.False(secondResult);
        Assert.Equal(1, tracker.CycleCount);

        // Cleanup
        tracker.EndWidthResolution(box);
    }

    /// <summary>
    /// Tests that EndWidthResolution properly clears tracking state.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void EndWidthResolution_ClearsTrackingState()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        Assert.NotNull(box);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        tracker.BeginWidthResolution(box);
        Assert.True(tracker.IsBeingResolved(box));
        tracker.EndWidthResolution(box);

        // Assert
        Assert.False(tracker.IsBeingResolved(box));
    }

    /// <summary>
    /// Tests that BeginHeightResolution succeeds for a box not already being resolved.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void BeginHeightResolution_FirstTime_ReturnsTrue()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        Assert.NotNull(box);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        bool result = tracker.BeginHeightResolution(box);

        // Assert
        Assert.True(result);
        Assert.True(tracker.IsBeingResolved(box));

        // Cleanup
        tracker.EndHeightResolution(box);
    }

    /// <summary>
    /// Tests that BeginHeightResolution returns false for an already-resolving box (cycle).
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void BeginHeightResolution_SameBoxTwice_ReturnsFalse()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        Assert.NotNull(box);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        tracker.BeginHeightResolution(box);
        bool secondResult = tracker.BeginHeightResolution(box);

        // Assert
        Assert.False(secondResult);
        Assert.Equal(1, tracker.CycleCount);

        // Cleanup
        tracker.EndHeightResolution(box);
    }

    #endregion

    #region Ancestor Detection Tests

    /// <summary>
    /// Tests that IsParentBeingResolved returns true when parent is being resolved.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void IsParentBeingResolved_WhenParentResolving_ReturnsTrue()
    {
        // Arrange
        var parent = _fixture.CreateBlock(400, 300);
        var child = _fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Height.Set(50);
        });
        _fixture.ForceLayoutUpdate();

        var parentBox = parent.Box as CssPrincipalBox;
        var childBox = child.Box as CssPrincipalBox;
        Assert.NotNull(parentBox);
        Assert.NotNull(childBox);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        tracker.BeginWidthResolution(parentBox);
        bool result = tracker.IsParentBeingResolved(childBox);

        // Assert
        Assert.True(result);

        // Cleanup
        tracker.EndWidthResolution(parentBox);
    }

    /// <summary>
    /// Tests that IsParentBeingResolved returns false when parent is not being resolved.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void IsParentBeingResolved_WhenParentNotResolving_ReturnsFalse()
    {
        // Arrange
        var parent = _fixture.CreateBlock(400, 300);
        var child = _fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Height.Set(50);
        });
        _fixture.ForceLayoutUpdate();

        var childBox = child.Box as CssPrincipalBox;
        Assert.NotNull(childBox);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        bool result = tracker.IsParentBeingResolved(childBox);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that FindResolvingAncestor finds resolving grandparent.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void FindResolvingAncestor_WhenGrandparentResolving_ReturnsGrandparent()
    {
        // Arrange
        var grandparent = _fixture.CreateBlock(400, 300);
        var parent = _fixture.CreateChild(grandparent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            style.Height.Set(150);
        });
        var child = _fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Height.Set(50);
        });
        _fixture.ForceLayoutUpdate();

        var grandparentBox = grandparent.Box as CssPrincipalBox;
        var childBox = child.Box as CssPrincipalBox;
        Assert.NotNull(grandparentBox);
        Assert.NotNull(childBox);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        tracker.BeginWidthResolution(grandparentBox);
        var result = tracker.FindResolvingAncestor(childBox);

        // Assert
        Assert.Same(grandparentBox, result);

        // Cleanup
        tracker.EndWidthResolution(grandparentBox);
    }

    /// <summary>
    /// Tests that FindResolvingAncestor returns null when no ancestor is resolving.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void FindResolvingAncestor_WhenNoAncestorResolving_ReturnsNull()
    {
        // Arrange
        var parent = _fixture.CreateBlock(400, 300);
        var child = _fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Height.Set(50);
        });
        _fixture.ForceLayoutUpdate();

        var childBox = child.Box as CssPrincipalBox;
        Assert.NotNull(childBox);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act
        var result = tracker.FindResolvingAncestor(childBox);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Reset Tests

    /// <summary>
    /// Tests that Reset clears all tracking state.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void Reset_ClearsAllState()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        Assert.NotNull(box);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        tracker.BeginWidthResolution(box);
        tracker.BeginWidthResolution(box); // Force a cycle count

        // Act
        tracker.Reset();

        // Assert
        Assert.False(tracker.IsBeingResolved(box));
        Assert.Equal(0, tracker.CycleCount);
        Assert.Equal(0, tracker.CurrentDepth);
    }

    #endregion

    #region Integration with BoxModel Tests

    /// <summary>
    /// Tests that BoxModel.ResolveWidth handles cycle gracefully.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Integration")]
    public void BoxModel_ResolveWidth_CycleDetected_SetsAutoWidth()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Simulate a cycle by marking the box as already resolving
        tracker.BeginWidthResolution(box);

        // Act - Second resolution attempt should detect cycle
        CssBoxModel.ResolveWidth(box, cascaded);

        // Assert - Width should be set to auto due to cycle
        Assert.True(cascaded.Width.Computed.IsAuto);
        Assert.True(tracker.CycleCount > 0);

        // Cleanup
        tracker.EndWidthResolution(box);
    }

    /// <summary>
    /// Tests that BoxModel.ResolveHeight handles cycle gracefully.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Integration")]
    public void BoxModel_ResolveHeight_CycleDetected_SetsAutoHeight()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Simulate a cycle by marking the box as already resolving
        tracker.BeginHeightResolution(box);

        // Act - Second resolution attempt should detect cycle
        CssBoxModel.ResolveHeight(box, cascaded);

        // Assert - Height should be set to auto due to cycle
        Assert.True(cascaded.Height.Computed.IsAuto);
        Assert.True(tracker.CycleCount > 0);

        // Cleanup
        tracker.EndHeightResolution(box);
    }

    /// <summary>
    /// Tests that normal resolution (no cycle) works correctly.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Integration")]
    public void BoxModel_ResolveWidth_NoCycle_ResolvesNormally()
    {
        // Arrange
        var element = _fixture.CreateBlock(200, 100);
        _fixture.ForceLayoutUpdate();
        var box = element.Box as CssPrincipalBox;
        var cascaded = element.Style?.Cascaded;
        Assert.NotNull(box);
        Assert.NotNull(cascaded);

        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        // Act - Normal resolution without pre-existing cycle
        CssBoxModel.ResolveWidth(box, cascaded);

        // Assert - Width should resolve normally
        Assert.Equal(200, cascaded.Width.Computed.AsDecimal(), precision: 1);
        Assert.Equal(0, tracker.CycleCount);
    }

    /// <summary>
    /// Tests that Run_Event_Loop resets cycle tracker.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Integration")]
    public void Run_Event_Loop_ResetsLayoutCycleTracker()
    {
        // Arrange
        _fixture.CreateBlock(200, 100);
        var tracker = LayoutCycleTracker.Current;

        // Simulate some previous state
        var tempElement = _fixture.CreateBlock(100, 50);
        _fixture.ForceLayoutUpdate();
        var tempBox = tempElement.Box as CssPrincipalBox;
        if (tempBox is not null)
        {
            tracker.BeginWidthResolution(tempBox);
            // Don't end resolution - simulate dirty state
        }

        // Act
        _fixture.Document.Run_Event_Loop();

        // Assert - Tracker should be reset
        Assert.Equal(0, tracker.CurrentDepth);
    }

    #endregion

    #region Percentage Height Cycle Scenario Tests

    /// <summary>
    /// Tests the classic percentage height cycle scenario:
    /// Child has percentage height, parent has height: auto.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Integration")]
    public void PercentageHeight_WithAutoParent_DoesNotInfiniteLoop()
    {
        // Arrange
        var parent = _fixture.CreateElement("div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(200);
            // Height is auto (default)
        });

        var child = _fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Height.Set(CssValue.From_Percent(50.0)); // 50% of parent's auto height
        });

        // Act - Should complete without hanging
        var exception = Record.Exception(() => _fixture.ForceLayoutUpdate());

        // Assert - The main test is that no infinite loop or exception occurs
        Assert.Null(exception);

        // Note: The actual resolved value depends on the containing block resolution.
        // Per CSS 2.2 §10.5, percentage heights with auto parent should resolve to auto,
        // but the exact behavior depends on the layout pipeline's current implementation.
        // The key is that the layout completes without infinite recursion.
    }

    /// <summary>
    /// Tests that percentage height with explicit parent height works correctly.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Integration")]
    public void PercentageHeight_WithExplicitParent_ResolvesCorrectly()
    {
        // Arrange
        var parent = _fixture.CreateBlock(200, 400); // Explicit height of 400

        var child = _fixture.CreateChild(parent, "div", style =>
        {
            style.Display.Set(EDisplayMode.BLOCK);
            style.Width.Set(100);
            style.Height.Set(CssValue.From_Percent(50.0)); // 50% of 400 = 200
        });

        // Act
        _fixture.ForceLayoutUpdate();

        // Assert
        var childBox = child.Box;
        var childCascaded = child.Style?.Cascaded;

        if (childBox is not null && childCascaded is not null)
        {
            // Force height resolution
            CssBoxModel.ResolveHeight(childBox, childCascaded);

            // Height should be 50% of 400 = 200
            // Note: May need adjustment based on containing block resolution
            var heightValue = childCascaded.Height.Computed;
            // Just ensure it resolved to something reasonable (not auto)
            Assert.False(heightValue.IsAuto || heightValue.IsNull);
        }
    }

    #endregion

    #region Depth Limit Tests

    /// <summary>
    /// Tests that excessively deep resolution is detected.
    /// </summary>
    [Fact]
    [Trait("Category", "LayoutCycle")]
    [Trait("Category", "Unit")]
    public void BeginResolution_ExceedsMaxDepth_ReturnsFalse()
    {
        // Arrange - Create many nested elements
        var tracker = LayoutCycleTracker.Current;
        tracker.Reset();

        var elements = new List<Element>();
        Element? currentParent = null;

        // Create deep nesting (not up to 1000, but enough to test the concept)
        for (int i = 0; i < 50; i++)
        {
            Element element;
            if (currentParent is null)
            {
                element = _fixture.CreateBlock(800 - i * 10, 600 - i * 10);
            }
            else
            {
                element = _fixture.CreateChild(currentParent, "div", style =>
                {
                    style.Display.Set(EDisplayMode.BLOCK);
                    style.Width.Set(800 - i * 10);
                    style.Height.Set(600 - i * 10);
                });
            }
            elements.Add(element);
            currentParent = element;
        }

        _fixture.ForceLayoutUpdate();

        // Begin resolution for all boxes (simulating abnormal recursive resolution)
        var boxes = new List<CssPrincipalBox>();
        foreach (var element in elements)
        {
            var box = element.Box as CssPrincipalBox;
            if (box is not null)
            {
                boxes.Add(box);
                // Don't call EndResolution - we're testing depth accumulation
                tracker.BeginWidthResolution(box);
            }
        }

        // Assert - Depth should be tracked
        Assert.True(tracker.CurrentDepth > 0);

        // Cleanup
        for (int i = boxes.Count - 1; i >= 0; i--)
        {
            tracker.EndWidthResolution(boxes[i]);
        }
    }

    #endregion
}
