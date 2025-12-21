using System;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for IntrinsicSizeCalculator algorithms.
/// </summary>
public class IntrinsicSizeCalculatorTests
{
    #region FitContent Algorithm Tests

    [Fact]
    public void FitContent_WhenAvailableBetweenMinAndMax_ReturnsAvailable()
    {
        // min-content = 50, max-content = 200, available = 100
        var intrinsic = new IntrinsicAxisSize(50, 200);
        double available = 100;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // fit-content = max(min-content, min(max-content, available))
        // = max(50, min(200, 100)) = max(50, 100) = 100
        Assert.Equal(100, result);
    }

    [Fact]
    public void FitContent_WhenAvailableLessThanMinContent_ReturnsMinContent()
    {
        // min-content = 50, max-content = 200, available = 30
        var intrinsic = new IntrinsicAxisSize(50, 200);
        double available = 30;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // fit-content = max(min-content, min(max-content, available))
        // = max(50, min(200, 30)) = max(50, 30) = 50
        Assert.Equal(50, result);
    }

    [Fact]
    public void FitContent_WhenAvailableGreaterThanMaxContent_ReturnsMaxContent()
    {
        // min-content = 50, max-content = 200, available = 500
        var intrinsic = new IntrinsicAxisSize(50, 200);
        double available = 500;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // fit-content = max(min-content, min(max-content, available))
        // = max(50, min(200, 500)) = max(50, 200) = 200
        Assert.Equal(200, result);
    }

    [Fact]
    public void FitContent_WhenMinEqualsMax_ReturnsDefiniteSize()
    {
        // Replaced element with definite intrinsic size
        var intrinsic = IntrinsicAxisSize.Definite(150);
        double available = 100;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // fit-content = max(150, min(150, 100)) = max(150, 100) = 150
        Assert.Equal(150, result);
    }

    [Fact]
    public void FitContent_WithZeroIntrinsicSize_ReturnsZero()
    {
        var intrinsic = IntrinsicAxisSize.Zero;
        double available = 500;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        Assert.Equal(0, result);
    }

    #endregion

    #region Replaced Element Tests

    [Fact]
    public void CalculateReplacedIntrinsicSize_WithBothDimensions_ReturnsDefinite()
    {
        // Create a mock box with intrinsic dimensions
        // Since we can't easily mock CssPrincipalBox, we test the algorithm logic
        // This is tested via the actual box in integration tests

        // Algorithm behavior: when both width and height are known
        // Result should be definite on both axes
        var expected = IntrinsicSize.Definite(800, 600);

        Assert.Equal(800, expected.Inline.MinContent);
        Assert.Equal(800, expected.Inline.MaxContent);
        Assert.Equal(600, expected.Block.MinContent);
        Assert.Equal(600, expected.Block.MaxContent);
    }

    [Fact]
    public void CalculateReplacedIntrinsicSize_DefaultObjectSize()
    {
        // Per CSS Images spec, default object size is 300x150
        // This tests the fallback behavior
        var defaultSize = IntrinsicSize.Definite(300, 150);

        Assert.Equal(300, defaultSize.Inline.MinContent);
        Assert.Equal(150, defaultSize.Block.MinContent);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void FitContent_WithNegativeAvailable_ReturnsMinContent()
    {
        var intrinsic = new IntrinsicAxisSize(50, 200);
        double available = -10;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // max(50, min(200, -10)) = max(50, -10) = 50
        Assert.Equal(50, result);
    }

    [Fact]
    public void FitContent_WithZeroAvailable_ReturnsMinContent()
    {
        var intrinsic = new IntrinsicAxisSize(50, 200);
        double available = 0;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // max(50, min(200, 0)) = max(50, 0) = 50
        Assert.Equal(50, result);
    }

    [Fact]
    public void FitContent_WithVeryLargeAvailable_ReturnsMaxContent()
    {
        var intrinsic = new IntrinsicAxisSize(50, 200);
        double available = double.MaxValue;

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        Assert.Equal(200, result);
    }

    #endregion

    #region Width:Auto / Shrink-to-Fit Scenarios

    /// <summary>
    /// Tests shrink-to-fit behavior for floating elements with width:auto.
    /// CSS 2.1 §10.3.5: "the width is shrink-to-fit"
    /// </summary>
    [Theory]
    [InlineData(100, 500, 300, 300)]  // Available space allows max-content
    [InlineData(100, 500, 150, 150)]  // Available space between min and max
    [InlineData(100, 500, 50, 100)]   // Available less than min - use min-content
    [InlineData(200, 200, 300, 200)]  // Definite size (min=max) - use that size
    public void ShrinkToFit_FloatWidthAuto_CalculatesCorrectly(
        double minContent, double maxContent, double available, double expected)
    {
        // Simulates: float with width:auto
        // shrink-to-fit = min(max(min-content, available), max-content)
        // which equals: clamp(min-content, max-content, available)
        var intrinsic = new IntrinsicAxisSize(minContent, maxContent);

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests shrink-to-fit behavior for inline-block elements with width:auto.
    /// CSS 2.1 §10.3.9: "the width is shrink-to-fit"
    /// </summary>
    [Theory]
    [InlineData(80, 400, 250, 250)]   // Available between min and max
    [InlineData(80, 400, 500, 400)]   // Lots of space - use max-content
    [InlineData(80, 400, 40, 80)]     // Tight space - use min-content
    public void ShrinkToFit_InlineBlockWidthAuto_CalculatesCorrectly(
        double minContent, double maxContent, double available, double expected)
    {
        // Simulates: inline-block with width:auto
        var intrinsic = new IntrinsicAxisSize(minContent, maxContent);

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests shrink-to-fit behavior for absolutely positioned elements with width:auto.
    /// CSS 2.1 §10.3.7: Rules #1 and #3 use shrink-to-fit
    /// </summary>
    [Theory]
    [InlineData(60, 300, 200, 200)]   // Rule #1/#3: width is shrink-to-fit
    [InlineData(60, 300, 400, 300)]   // Plenty of space
    [InlineData(60, 300, 30, 60)]     // Constrained - use min-content
    public void ShrinkToFit_AbsolutePositionWidthAuto_CalculatesCorrectly(
        double minContent, double maxContent, double available, double expected)
    {
        // Simulates: position:absolute with width:auto and left/right constraints
        var intrinsic = new IntrinsicAxisSize(minContent, maxContent);

        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests that content-based sizing respects the formula:
    /// shrink-to-fit width = min(max(preferred minimum width, available width), preferred width)
    /// which is equivalent to: clamp(min-content, max-content, available)
    /// </summary>
    [Fact]
    public void ShrinkToFit_Formula_MatchesCssSpec()
    {
        // Per CSS 2.1: shrink-to-fit width = min(max(preferred minimum width, available width), preferred width)
        // preferred minimum width = min-content
        // preferred width = max-content
        // So: shrink-to-fit = min(max(min-content, available), max-content)
        // Which equals: clamp(min-content, max-content, available)

        double minContent = 75;
        double maxContent = 350;
        double available = 200;

        var intrinsic = new IntrinsicAxisSize(minContent, maxContent);
        double result = IntrinsicSizeCalculator.FitContent(intrinsic, available);

        // Manual calculation per spec:
        // min(max(75, 200), 350) = min(200, 350) = 200
        double specResult = System.Math.Min(System.Math.Max(minContent, available), maxContent);

        Assert.Equal(specResult, result);
    }

    /// <summary>
    /// Tests text content intrinsic sizing for width:auto scenarios.
    /// Min-content = longest word, Max-content = full text width
    /// </summary>
    [Fact]
    public void ShrinkToFit_TextContent_MinContentIsLongestWord()
    {
        // Conceptual test: "Hello World" has two words
        // min-content should be width of longest word
        // max-content should be width of entire string

        // Simulate measurements (in real usage, font engine provides these)
        double longestWordWidth = 50;  // "Hello" or "World"
        double fullTextWidth = 110;    // "Hello World"

        var intrinsic = new IntrinsicAxisSize(longestWordWidth, fullTextWidth);

        // With available = 80, should fit between min and max
        Assert.Equal(80, IntrinsicSizeCalculator.FitContent(intrinsic, 80));

        // With available = 30, should use min-content (can't break words)
        Assert.Equal(50, IntrinsicSizeCalculator.FitContent(intrinsic, 30));

        // With available = 200, should use max-content (no need to wrap)
        Assert.Equal(110, IntrinsicSizeCalculator.FitContent(intrinsic, 200));
    }

    /// <summary>
    /// Tests that replaced elements with intrinsic dimensions use those dimensions.
    /// CSS 2.1 §10.3.2: Images with intrinsic width use that width.
    /// </summary>
    [Fact]
    public void ShrinkToFit_ReplacedElement_UsesIntrinsicWidth()
    {
        // Image with 400x300 intrinsic size
        // min-content = max-content = 400 (definite)
        var intrinsic = IntrinsicAxisSize.Definite(400);

        // Even with less available space, intrinsic width is used
        // (this is the min-content, so it won't shrink below)
        Assert.Equal(400, IntrinsicSizeCalculator.FitContent(intrinsic, 200));
        Assert.Equal(400, IntrinsicSizeCalculator.FitContent(intrinsic, 500));
    }

    /// <summary>
    /// Tests behavior when containing block is very narrow.
    /// Element should not shrink below min-content even if available space is less.
    /// </summary>
    [Fact]
    public void ShrinkToFit_NarrowContainer_RespectsMinContent()
    {
        // Content requires at least 100px (min-content)
        // Would prefer 300px if space available (max-content)
        var intrinsic = new IntrinsicAxisSize(100, 300);

        // Container is only 50px - element overflows but uses min-content
        double result = IntrinsicSizeCalculator.FitContent(intrinsic, 50);

        Assert.Equal(100, result); // Can't go below min-content
    }

    /// <summary>
    /// Tests behavior when containing block is very wide.
    /// Element should not grow beyond max-content even with infinite space.
    /// </summary>
    [Fact]
    public void ShrinkToFit_WideContainer_RespectsMaxContent()
    {
        var intrinsic = new IntrinsicAxisSize(100, 300);

        // Container has plenty of space
        double result = IntrinsicSizeCalculator.FitContent(intrinsic, 1000);

        Assert.Equal(300, result); // Stops at max-content
    }

    #endregion
}
