# CssUI Layout Pipeline - Detailed Action Plan

## Overview

This document provides a step-by-step implementation guide for completing the CSS layout pipeline in CssUI. The work is organized into 5 phases (14-18) with specific implementation details, code locations, and testing strategies.

**Current State:** The layout infrastructure exists but is disconnected. BoxModel calculations are commented out, BlockFormattingContext is a stub, and float/clear properties don't exist.

**Goal:** A fully functional CSS 2.2 visual formatting model with block layout, floats, margin collapsing, and inline formatting.

---

## Phase 14: Layout Pipeline Integration

**Objective:** Connect existing components and establish the test infrastructure.

### 14.1 Re-enable BoxModel.Resolve()

**Problem:** In `Document.cs`, the BoxModel.Resolve() call is commented out, breaking the style→layout pipeline.

**Files to Modify:**

-   `CssUI/DOM/Document/Document.cs` - Line ~1200 in Run_Event_Loop()

**Implementation Steps:**

1. **Locate the commented code:**

    ```csharp
    // Look for this pattern in Run_Event_Loop():
    if (current.GetFlag(ENodeFlags.NeedsStyleUpdate))
    {
        //BoxModel.Resolve(currentAsElement.Box, current.Style.Cascaded);  // <-- UNCOMMENT
        current.Unpropagate_Flag(...);
    }
    ```

2. **Add null checks before uncommenting:**

    ```csharp
    if (current.GetFlag(ENodeFlags.NeedsStyleUpdate))
    {
        if (currentAsElement?.Box != null && current.Style?.Cascaded != null)
        {
            BoxModel.Resolve(currentAsElement.Box, current.Style.Cascaded);
        }
        current.Unpropagate_Flag(ENodeFlags.NeedsStyleUpdate);
    }
    ```

3. **Debug the infinite loop:**

    - Add logging to identify which flag isn't being cleared
    - Check if NeedsStyleUpdate is being re-set during resolution
    - Verify Unpropagate_Flag is working correctly

4. **Test incrementally:**
    - First test with a single element
    - Then test parent-child relationships
    - Finally test the full tree

### 14.2 Fix the Infinite Loop

**Investigation Checklist:**

1. **Flag propagation issue:**

    - Check `SetFlag()` and `ClearFlag()` implementations in Node.cs
    - Verify `Unpropagate_Flag()` logic
    - Add breakpoint to see if flag is immediately re-set

2. **Potential causes:**

    - Style cascade triggering another cascade
    - Box generation triggering style update
    - Circular dependency in property resolution

3. **Diagnostic approach:**

    ```csharp
    // Add temporary logging:
    if (current.GetFlag(ENodeFlags.NeedsStyleUpdate))
    {
        Logger.Debug($"Processing NeedsStyleUpdate for {current.nodeName}");
        // ... resolve ...
        Logger.Debug($"After resolve, flag is: {current.GetFlag(ENodeFlags.NeedsStyleUpdate)}");
    }
    ```

### 14.3 Enable Test Fixture

Once the pipeline is connected:

1. Remove `Skip` attributes from `LayoutTestFixtureTests.cs`
2. Run tests to verify basic layout works
3. Add more integration tests as needed

---

## Phase 15: Float and Clear Properties

**Objective:** Define float/clear CSS properties and implement basic float layout.

### 15.1 Property Definition Checklist

**Step 1: Add Enum Values**

File: `CssUI/CSS/Enums/ECssPropertyID.cs`

```csharp
// Add to enum:
Float = <next_id>,
Clear = <next_id>,
```

**Step 2: Create EFloat Enum**

File: `CssUI/CSS/Enums/EFloat.cs` (new file)

```csharp
using EnumRecords;

namespace CssUI.CSS.Enums;

[EnumRecord<KeywordProperties>]
public enum EFloat
{
    [EnumData("none")] None = 0,
    [EnumData("left")] Left = 1,
    [EnumData("right")] Right = 2,
    [EnumData("inline-start")] InlineStart = 3,
    [EnumData("inline-end")] InlineEnd = 4,
}
```

**Step 3: Create EClear Enum**

File: `CssUI/CSS/Enums/EClear.cs` (new file)

```csharp
using EnumRecords;

namespace CssUI.CSS.Enums;

[EnumRecord<KeywordProperties>]
public enum EClear
{
    [EnumData("none")] None = 0,
    [EnumData("left")] Left = 1,
    [EnumData("right")] Right = 2,
    [EnumData("both")] Both = 3,
    [EnumData("inline-start")] InlineStart = 4,
    [EnumData("inline-end")] InlineEnd = 5,
}
```

**Step 4: Add StyleDefinitions**

File: `CssUI/CSS/Properties/Definitions/CssDefinitions.cs`

```csharp
// Add to the definitions list:
new StyleDefinition(
    ECssPropertyID.Float,
    inherits: false,
    EPropertyDirtFlags.Flow,
    CssValue.From(EFloat.None),
    ECssValueTypes.KEYWORD,
    allowedKeywords: Lookup.Get_Keywords<EFloat>(),
    Percentage_Resolver: null,
    Resolvers: null),

new StyleDefinition(
    ECssPropertyID.Clear,
    inherits: false,
    EPropertyDirtFlags.Flow,
    CssValue.From(EClear.None),
    ECssValueTypes.KEYWORD,
    allowedKeywords: Lookup.Get_Keywords<EClear>(),
    Percentage_Resolver: null,
    Resolvers: null),
```

**Step 5: Add to StyleProperties**

File: `CssUI/CSS/Properties/Style/StyleProperties.cs`

```csharp
// Add property accessors:
public EFloat Float => Cascaded.Float.Actual;
public EClear Clear => Cascaded.Clear.Actual;
```

**Step 6: Add to CssComputedStyle**

File: `CssUI/CSS/Properties/Style/CssComputedStyle.cs`

```csharp
// Add property accessors:
public EnumProperty<EFloat> Float => (EnumProperty<EFloat>)Get(ECssPropertyID.Float)!;
public EnumProperty<EClear> Clear => (EnumProperty<EClear>)Get(ECssPropertyID.Clear)!;
```

**Step 7: Update CssPrincipalBox.IsFloated**

File: `CssUI/CSS/Box Model/Base/CssPrincipalBox.cs`

```csharp
// Change from:
public bool IsFloated => false;  // Hardcoded

// To:
public bool IsFloated => owningElement?.Style?.Float != EFloat.None;
```

### 15.2 Float Layout Algorithm

**File:** `CssUI/CSS/Formatting/BlockFormattingContext.cs`

**Data Structures Needed:**

```csharp
public class BlockFormattingContext : IFormattingContext
{
    // Add float tracking:
    private readonly List<FloatBox> _leftFloats = new();
    private readonly List<FloatBox> _rightFloats = new();

    private record FloatBox(CssPrincipalBox Box, int Top, int Bottom, int OuterEdge);

    // ...
}
```

**Algorithm Outline:**

```csharp
public void Flow(CssBoxTreeNode Node)
{
    int currentY = 0;

    foreach (var child in Node.Children)
    {
        if (child is CssPrincipalBox box)
        {
            // 1. Handle clear property
            if (box.Clear != EClear.None)
            {
                currentY = CalculateClearance(box.Clear, currentY);
            }

            // 2. Handle floats
            if (box.IsFloated)
            {
                PositionFloat(box, currentY);
                continue;  // Floats don't advance currentY
            }

            // 3. Position normal flow box
            int availableWidth = CalculateAvailableWidth(currentY);
            box.Position = new Point2f(GetLeftEdge(currentY), currentY);

            // 4. Advance Y (with margin collapsing - Phase 16)
            currentY += box.MarginBox.Height;
        }
    }
}

private void PositionFloat(CssPrincipalBox box, int currentY)
{
    if (box.Float == EFloat.Left)
    {
        int x = GetLeftFloatPosition(currentY);
        box.Position = new Point2f(x, currentY);
        _leftFloats.Add(new FloatBox(box, currentY, currentY + box.MarginBox.Height, x + box.MarginBox.Width));
    }
    else if (box.Float == EFloat.Right)
    {
        int x = GetRightFloatPosition(currentY) - box.MarginBox.Width;
        box.Position = new Point2f(x, currentY);
        _rightFloats.Add(new FloatBox(box, currentY, currentY + box.MarginBox.Height, x));
    }
}

private int CalculateClearance(EClear clear, int currentY)
{
    int clearY = currentY;

    if (clear == EClear.Left || clear == EClear.Both)
    {
        foreach (var f in _leftFloats)
            clearY = Math.Max(clearY, f.Bottom);
    }

    if (clear == EClear.Right || clear == EClear.Both)
    {
        foreach (var f in _rightFloats)
            clearY = Math.Max(clearY, f.Bottom);
    }

    return clearY;
}
```

### 15.3 Float Tests

Create: `CssUITests2/CSS/Formatting/FloatLayoutTests.cs`

```csharp
[Fact]
public void Float_Left_PositionsAtLeftEdge()
{
    using var fixture = new LayoutTestFixture();
    var floated = fixture.CreateElement("div", style => {
        style.Float.Set(EFloat.Left);
        style.Width.Set(100);
        style.Height.Set(50);
    });

    fixture.ForceLayoutUpdate();

    Assert.Equal(0, floated.Box.Position.X);
}

[Fact]
public void Clear_Both_MovesBelowAllFloats()
{
    using var fixture = new LayoutTestFixture();
    var leftFloat = fixture.CreateElement("div", style => {
        style.Float.Set(EFloat.Left);
        style.Width.Set(100);
        style.Height.Set(100);
    });
    var rightFloat = fixture.CreateElement("div", style => {
        style.Float.Set(EFloat.Right);
        style.Width.Set(100);
        style.Height.Set(150);
    });
    var cleared = fixture.CreateElement("div", style => {
        style.Clear.Set(EClear.Both);
    });

    fixture.ForceLayoutUpdate();

    Assert.True(cleared.Box.Position.Y >= 150);
}
```

---

## Phase 16: Margin Collapsing

**Objective:** Implement CSS 2.2 margin collapsing rules.

### 16.1 Margin Collapse State

**Add to BlockFormattingContext:**

```csharp
public class BlockFormattingContext : IFormattingContext
{
    // Track margin state during flow:
    private double _pendingMargin = 0;
    private bool _previousWasCollapsible = false;

    // ...
}
```

### 16.2 Collapsing Algorithm

**Core Logic:**

```csharp
private double CollapseMargins(double margin1, double margin2)
{
    // Both positive: take the larger
    if (margin1 >= 0 && margin2 >= 0)
        return Math.Max(margin1, margin2);

    // Both negative: take the more negative
    if (margin1 < 0 && margin2 < 0)
        return Math.Min(margin1, margin2);

    // Mixed: add them (positive + negative)
    return margin1 + margin2;
}

private bool CanCollapseWith(CssPrincipalBox box)
{
    // Cannot collapse if:
    if (box.IsFloated) return false;
    if (box.Style.Positioning == EBoxPositioning.Absolute) return false;
    if (box.Style.Positioning == EBoxPositioning.Fixed) return false;
    if (box.Style.Display == EDisplayMode.INLINE_BLOCK) return false;
    if (box.Style.Overflow_X != EOverflowMode.Visible) return false;
    if (box.Style.Overflow_Y != EOverflowMode.Visible) return false;

    return true;
}
```

### 16.3 Parent/Child Collapsing

```csharp
private double HandleParentChildCollapse(CssPrincipalBox parent, CssPrincipalBox firstChild)
{
    // Parent's top margin collapses with first child's top margin if:
    // - No top border on parent
    // - No top padding on parent
    // - No clearance

    bool canCollapse =
        parent.Style.Border_Top_Width == 0 &&
        parent.Style.Padding_Top == 0 &&
        CanCollapseWith(firstChild);

    if (canCollapse)
    {
        return CollapseMargins(parent.Style.Margin_Top, firstChild.Style.Margin_Top);
    }

    return parent.Style.Margin_Top;
}
```

### 16.4 Margin Collapse Tests

```csharp
[Fact]
public void AdjacentSiblings_MarginsCollapse()
{
    using var fixture = new LayoutTestFixture();
    var first = fixture.CreateBlockWithMargins(100, 50, 0, 0, 30, 0);  // margin-bottom: 30
    var second = fixture.CreateBlockWithMargins(100, 50, 20, 0, 0, 0); // margin-top: 20

    fixture.ForceLayoutUpdate();

    // Gap should be max(30, 20) = 30, not 50
    int gap = second.Box.Position.Y - (first.Box.Position.Y + first.Box.Height);
    Assert.Equal(30, gap);
}
```

---

## Phase 17: Block Formatting Context

**Objective:** Complete BFC implementation with all features integrated.

### 17.1 Complete Flow Algorithm

**Final BlockFormattingContext.Flow():**

```csharp
public void Flow(CssBoxTreeNode Node)
{
    ArgumentNullException.ThrowIfNull(Node);

    double currentY = 0;
    double pendingMarginTop = 0;
    CssPrincipalBox? previousBox = null;

    // Clear float lists for this BFC
    _leftFloats.Clear();
    _rightFloats.Clear();

    var children = GetFlowChildren(Node);

    foreach (var child in children)
    {
        if (child is not CssPrincipalBox box) continue;

        // Skip display: none
        if (box.DisplayType.Outer == EOuterDisplayType.None) continue;

        // Handle floats separately
        if (box.IsFloated)
        {
            PositionFloat(box, currentY);
            continue;
        }

        // Calculate clearance
        if (box.Clear != EClear.None)
        {
            currentY = Math.Max(currentY, CalculateClearance(box.Clear));
        }

        // Calculate margin with collapsing
        double marginTop = box.Style.Margin_Top;
        if (previousBox != null && CanCollapseWith(box) && CanCollapseWith(previousBox))
        {
            double previousMarginBottom = previousBox.Style.Margin_Bottom;
            double collapsedMargin = CollapseMargins(previousMarginBottom, marginTop);
            currentY -= previousMarginBottom;  // Remove previous margin
            currentY += collapsedMargin;       // Add collapsed margin
        }
        else
        {
            currentY += marginTop;
        }

        // Calculate available width accounting for floats
        double availableWidth = CalculateAvailableWidth(currentY, box.MarginBox.Height);
        double leftEdge = GetLeftEdge(currentY);

        // Position the box
        box.Position = new Point2f((float)leftEdge, (float)currentY);

        // Advance Y
        currentY += box.BorderBox.Height;
        currentY += box.Style.Margin_Bottom;

        previousBox = box;
    }

    // Handle parent/last-child margin collapse
    // (Set the BFC height accounting for collapsed bottom margin)
}
```

### 17.2 BFC Establishment Tracking

```csharp
public static bool EstablishesBFC(CssPrincipalBox box)
{
    // Root element
    if (box.owningElement?.isRoot == true) return true;

    // Floats
    if (box.IsFloated) return true;

    // Absolutely positioned
    if (box.Style.Positioning == EBoxPositioning.Absolute ||
        box.Style.Positioning == EBoxPositioning.Fixed) return true;

    // Inline-blocks
    if (box.Style.Display == EDisplayMode.INLINE_BLOCK) return true;

    // Overflow not visible
    if (box.Style.Overflow_X != EOverflowMode.Visible ||
        box.Style.Overflow_Y != EOverflowMode.Visible) return true;

    // Flex/Grid items
    // display: flow-root
    if (box.Style.Display == EDisplayMode.FLOW_ROOT) return true;

    return false;
}
```

---

## Phase 18: Inline Formatting Context

**Objective:** Implement line boxes and inline layout.

### 18.1 LineBox Class

Create: `CssUI/CSS/Formatting/LineBox.cs`

```csharp
namespace CssUI.CSS.Formatting;

/// <summary>
/// Represents a horizontal line of inline content within an IFC.
/// </summary>
public class LineBox
{
    public double Top { get; set; }
    public double Left { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public double Baseline { get; set; }
    public double CurrentX { get; set; }

    private readonly List<InlineBoxFragment> _fragments = new();
    public IReadOnlyList<InlineBoxFragment> Fragments => _fragments;

    public bool CanFit(double width) => (CurrentX + width) <= Width;

    public void Add(InlineBoxFragment fragment)
    {
        fragment.X = CurrentX;
        _fragments.Add(fragment);
        CurrentX += fragment.Width;
        Height = Math.Max(Height, fragment.Height);
    }
}

public record InlineBoxFragment(
    CssBox Box,
    double X,
    double Width,
    double Height,
    double Baseline);
```

### 18.2 InlineFormattingContext

Create: `CssUI/CSS/Formatting/InlineFormattingContext.cs`

```csharp
namespace CssUI.CSS.Formatting;

public class InlineFormattingContext : IFormattingContext
{
    private readonly List<LineBox> _lines = new();
    private LineBox? _currentLine;

    public void Flow(CssBoxTreeNode Node)
    {
        double containerWidth = Node.Size.Width;
        _currentLine = new LineBox { Width = containerWidth };
        _lines.Add(_currentLine);

        foreach (var child in Node.Children)
        {
            if (child is CssPrincipalBox box)
            {
                LayoutInlineBox(box, containerWidth);
            }
            else if (child is CssTextRun text)
            {
                LayoutText(text, containerWidth);
            }
        }

        // Position all line boxes vertically
        PositionLines();
    }

    private void LayoutInlineBox(CssPrincipalBox box, double containerWidth)
    {
        double width = box.MarginBox.Width;

        if (!_currentLine!.CanFit(width))
        {
            StartNewLine(containerWidth);
        }

        _currentLine.Add(new InlineBoxFragment(
            box,
            _currentLine.CurrentX,
            width,
            box.MarginBox.Height,
            CalculateBaseline(box)));
    }

    private void StartNewLine(double containerWidth)
    {
        _currentLine = new LineBox
        {
            Width = containerWidth,
            Top = _lines.Sum(l => l.Height)
        };
        _lines.Add(_currentLine);
    }

    private void PositionLines()
    {
        double y = 0;
        foreach (var line in _lines)
        {
            line.Top = y;
            // Position fragments based on vertical-align
            foreach (var fragment in line.Fragments)
            {
                fragment.Box.Position = new Point2f(
                    (float)(line.Left + fragment.X),
                    (float)(y + CalculateVerticalOffset(fragment, line)));
            }
            y += line.Height;
        }
    }

    private double CalculateBaseline(CssPrincipalBox box)
    {
        // For replaced elements, baseline is bottom edge
        // For inline-blocks, use last line box baseline or bottom margin edge
        return box.MarginBox.Height;
    }

    private double CalculateVerticalOffset(InlineBoxFragment fragment, LineBox line)
    {
        // Default: baseline alignment
        return line.Baseline - fragment.Baseline;
    }
}
```

### 18.3 Text Integration

```csharp
private void LayoutText(CssTextRun text, double containerWidth)
{
    var fontService = text.OwningElement?.nodeDocument?.GetService<IFontService>();
    if (fontService == null) return;

    var font = text.Style.Font;
    var remainingText = text.Text;

    while (!string.IsNullOrEmpty(remainingText))
    {
        // Measure how much text fits on current line
        double availableWidth = _currentLine!.Width - _currentLine.CurrentX;
        var (fittingText, width) = fontService.MeasureToFit(font, remainingText, availableWidth);

        if (string.IsNullOrEmpty(fittingText) && _currentLine.CurrentX > 0)
        {
            // Nothing fits, start new line
            StartNewLine(containerWidth);
            continue;
        }

        // Add text fragment to line
        _currentLine.Add(new TextFragment(fittingText, width, fontService.GetLineHeight(font)));

        remainingText = remainingText.Substring(fittingText.Length);
    }
}
```

---

## Implementation Order

**Recommended sequence:**

```
Week 1: Phase 14
├── Day 1-2: Debug infinite loop, re-enable BoxModel.Resolve()
├── Day 3-4: Fix pipeline issues, enable basic tests
└── Day 5: Write BoxModel integration tests

Week 2: Phase 15
├── Day 1: Define float/clear enums and properties
├── Day 2-3: Implement float positioning
├── Day 4: Implement clear/clearance
└── Day 5: Write float/clear tests

Week 3: Phase 16
├── Day 1-2: Implement margin collapse detection
├── Day 3: Implement collapse calculation
├── Day 4: Implement collapse prevention rules
└── Day 5: Write margin collapse tests

Week 4: Phase 17
├── Day 1-2: Complete BFC Flow() algorithm
├── Day 3: Implement anonymous block boxes
├── Day 4: Implement BFC establishment tracking
└── Day 5: Write BFC integration tests

Week 5: Phase 18
├── Day 1: Create LineBox infrastructure
├── Day 2-3: Implement InlineFormattingContext
├── Day 4: Integrate text layout
└── Day 5: Write IFC tests
```

---

## Testing Strategy

**For each phase:**

1. **Unit tests first** - Test algorithms in isolation
2. **Integration tests** - Test with LayoutTestFixture
3. **Visual tests** - Consider a simple renderer for debugging
4. **Regression tests** - Ensure previous phases still work

**Test file locations:**

-   `CssUITests2/CSS/Formatting/BlockFormattingContextTests.cs`
-   `CssUITests2/CSS/Formatting/FloatLayoutTests.cs`
-   `CssUITests2/CSS/Formatting/MarginCollapsingTests.cs`
-   `CssUITests2/CSS/Formatting/InlineFormattingContextTests.cs`
-   `CssUITests2/CSS/Layout/LayoutIntegrationTests.cs`

---

## Debugging Tips

1. **Add visualization helper:**

    ```csharp
    public static string DumpBoxTree(CssPrincipalBox root, int indent = 0)
    {
        var sb = new StringBuilder();
        var prefix = new string(' ', indent * 2);
        sb.AppendLine($"{prefix}<{root.owningElement?.localName}> pos=({root.Position.X},{root.Position.Y}) size=({root.Size.Width},{root.Size.Height})");
        foreach (var child in root.Children)
            if (child is CssPrincipalBox childBox)
                sb.Append(DumpBoxTree(childBox, indent + 1));
        return sb.ToString();
    }
    ```

2. **Use conditional breakpoints** on flag changes

3. **Log float/margin state** during Flow() for debugging

4. **Compare with browser DevTools** for expected results

---

## References

-   CSS 2.2 Visual Formatting Model: <https://www.w3.org/TR/CSS2/visuren.html>
-   CSS 2.2 Box Model: <https://www.w3.org/TR/CSS2/box.html>
-   CSS 2.2 Floats: <https://www.w3.org/TR/CSS2/visuren.html#floats>
-   CSS 2.2 Margin Collapsing: <https://www.w3.org/TR/CSS2/box.html#collapsing-margins>
