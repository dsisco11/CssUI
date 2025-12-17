# CssUI Revival Roadmap

This document outlines the phased implementation plan for reviving CssUI with modern CSS layout support, prioritizing Flexbox and Grid over legacy features.

---

## Overview

| Phase | Focus             | Dependencies             | Estimated Complexity |
| ----- | ----------------- | ------------------------ | -------------------- |
| 0     | Intrinsic Sizing  | None                     | Medium               |
| 1     | Flexbox           | Phase 0                  | High                 |
| 2     | Grid              | Phase 0, partial Phase 3 | High                 |
| 3     | Alignment         | Phase 0                  | Medium               |
| 4     | Advanced Features | Phases 1-3               | Low-Medium           |

---

## Phase 0: Intrinsic Sizing Infrastructure

**Goal**: Ensure `min-content` and `max-content` size calculations work correctly for all box types.

**Spec Reference**: [CSS Sizing Level 3 §5](https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes)

### Tasks

- [ ] **0.1** Audit existing intrinsic sizing code

  - Review `CssAlgorithms.cs` for `Intrinsic_Ratio`, `Intrinsic_Width`, `Intrinsic_Height`
  - Verify `EBoxSize` enum values are properly handled in property resolution

- [ ] **0.2** Implement `IIntrinsicSizable` interface

  ```csharp
  interface IIntrinsicSizable
  {
      float GetMinContentWidth(LayoutContext ctx);
      float GetMaxContentWidth(LayoutContext ctx);
      float GetMinContentHeight(LayoutContext ctx);
      float GetMaxContentHeight(LayoutContext ctx);
  }
  ```

- [ ] **0.3** Implement intrinsic size calculation for text content

  - Min-content: width of longest word (no wrapping)
  - Max-content: width of all text on single line

- [ ] **0.4** Implement intrinsic size calculation for replaced elements

  - Use intrinsic dimensions from the element
  - Handle aspect ratio constraints

- [ ] **0.5** Implement intrinsic size calculation for block containers

  - Recurse through children
  - Account for margins, padding, borders

- [ ] **0.6** Add unit tests for intrinsic sizing
  - Text blocks with varying content
  - Nested containers
  - Replaced elements with/without intrinsic dimensions

### Deliverables

- `IIntrinsicSizable` interface
- Intrinsic size methods on `cssElement` or box classes
- Test coverage for sizing calculations

---

## Phase 1: Flexbox Layout

**Goal**: Implement complete CSS Flexbox Level 1 support.

**Spec Reference**: [CSS Flexbox Level 1](https://www.w3.org/TR/css-flexbox-1/)

### 1.1 Property Definitions

- [ ] Add to `ECssPropertyID` enum:

  ```
  FlexDirection, FlexWrap, FlexFlow,
  FlexGrow, FlexShrink, FlexBasis, Flex,
  Order
  ```

- [ ] Add to `CssDefinitions.cs`:

  ```csharp
  // flex-direction: row | row-reverse | column | column-reverse
  new StyleDefinition(ECssPropertyID.FlexDirection, false, EPropertyDirtFlags.Flow,
      CssValue.From(EFlexDirection.Row), ECssValueTypes.KEYWORD,
      Lookup.Get_Keywords<EFlexDirection>()),

  // flex-wrap: nowrap | wrap | wrap-reverse
  new StyleDefinition(ECssPropertyID.FlexWrap, false, EPropertyDirtFlags.Flow,
      CssValue.From(EFlexWrap.NoWrap), ECssValueTypes.KEYWORD,
      Lookup.Get_Keywords<EFlexWrap>()),

  // flex-grow: <number> (default 0)
  new StyleDefinition(ECssPropertyID.FlexGrow, false, EPropertyDirtFlags.Flow,
      CssValue.From(0.0), ECssValueTypes.NUMBER),

  // flex-shrink: <number> (default 1)
  new StyleDefinition(ECssPropertyID.FlexShrink, false, EPropertyDirtFlags.Flow,
      CssValue.From(1.0), ECssValueTypes.NUMBER),

  // flex-basis: content | <width> (default auto)
  new StyleDefinition(ECssPropertyID.FlexBasis, false, EPropertyDirtFlags.Flow,
      CssValue.Auto, ECssValueTypes.AUTO | ECssValueTypes.DIMENSION | ECssValueTypes.PERCENT | ECssValueTypes.KEYWORD),

  // order: <integer> (default 0)
  new StyleDefinition(ECssPropertyID.Order, false, EPropertyDirtFlags.Flow,
      CssValue.From(0), ECssValueTypes.INTEGER),
  ```

- [ ] Create enums:
  - `EFlexDirection` { Row, RowReverse, Column, ColumnReverse }
  - `EFlexWrap` { NoWrap, Wrap, WrapReverse }

### 1.2 Flex Container Detection

- [ ] Update display mode handling to detect flex containers
- [ ] Create `IsFlexContainer` property/method
- [ ] Handle `display: flex` and `display: inline-flex`

### 1.3 Flex Layout Director

- [ ] Create `CSS/Layouts/FlexLayoutDirector.cs`

```csharp
public class FlexLayoutDirector : LayoutDirectorBase
{
    // §9.1 Initial Setup
    private void GenerateAnonymousFlexItems();

    // §9.2 Line Length Determination
    private float DetermineMainSize();
    private void CollectFlexItemsIntoLines();

    // §9.3 Main Size Determination
    private void ResolveFlexibleLengths(FlexLine line);

    // §9.4 Cross Size Determination
    private void DetermineCrossSizes();

    // §9.5 Main-Axis Alignment
    private void AlignOnMainAxis();

    // §9.6 Cross-Axis Alignment
    private void AlignOnCrossAxis();

    // Internal types
    private class FlexItem { ... }
    private class FlexLine { ... }
}
```

### 1.4 Layout Algorithm Implementation

Implement per [§9 Flex Layout Algorithm](https://www.w3.org/TR/css-flexbox-1/#layout-algorithm):

- [ ] **§9.1** Initial Setup — Generate anonymous flex items
- [ ] **§9.2** Line Length Determination
  - Determine available main/cross space
  - Calculate flex base size and hypothetical main size
  - Collect items into flex lines
- [ ] **§9.3** Resolve Flexible Lengths (critical complexity)
  - Determine used flex factor
  - Size inflexible items
  - Calculate remaining free space
  - Distribute free space (grow/shrink)
  - Fix min/max violations
- [ ] **§9.4** Cross Size Determination
  - Calculate hypothetical cross size
  - Calculate cross size of flex lines
  - Handle `align-content: stretch`
  - Determine used cross size
- [ ] **§9.5** Main-Axis Alignment
  - Distribute remaining space per `justify-content`
  - Handle `auto` margins
- [ ] **§9.6** Cross-Axis Alignment
  - Align items per `align-items`/`align-self`
  - Align flex lines per `align-content`

### 1.5 Integration

- [ ] Hook `FlexLayoutDirector` into layout system
- [ ] Update `cssElement` to use flex layout when appropriate
- [ ] Handle flex item blockification

### 1.6 Testing

- [ ] Unit tests for each algorithm step
- [ ] Integration tests for common flex patterns:
  - Horizontal centering
  - Equal-width columns
  - Sticky footer
  - Holy grail layout
- [ ] Run against WPT css-flexbox tests

### Deliverables

- 7 new CSS properties
- 2 new enums
- `FlexLayoutDirector` class
- Comprehensive test suite

---

## Phase 2: Grid Layout

**Goal**: Implement CSS Grid Level 1 support.

**Spec Reference**: [CSS Grid Level 1](https://www.w3.org/TR/css-grid-1/)

### 2.1 Property Definitions

- [ ] Add to `ECssPropertyID` enum:

  ```
  GridTemplateColumns, GridTemplateRows, GridTemplateAreas,
  GridAutoColumns, GridAutoRows, GridAutoFlow,
  GridColumnStart, GridColumnEnd, GridRowStart, GridRowEnd,
  GridColumn, GridRow, GridArea
  ```

- [ ] Create value types:
  - `GridTrackSize` — `<length-percentage>`, `<flex>`, `min-content`, `max-content`, `auto`
  - `GridTrackList` — List of track sizes with line names
  - `GridLine` — `<integer>`, `<custom-ident>`, `span`

### 2.2 Grid Container Detection

- [ ] Handle `display: grid` and `display: inline-grid`
- [ ] Create `IsGridContainer` property/method

### 2.3 Grid Layout Director

- [ ] Create `CSS/Layouts/GridLayoutDirector.cs`

```csharp
public class GridLayoutDirector : LayoutDirectorBase
{
    // Grid structure
    private GridTrack[] columnTracks;
    private GridTrack[] rowTracks;
    private GridArea[,] gridAreas;

    // §7 Grid Definition
    private void DefineExplicitGrid();
    private void DefineImplicitGrid();

    // §8 Item Placement
    private void PlaceGridItems();
    private void RunAutoPlacementAlgorithm();

    // §11 Track Sizing
    private void InitializeTrackSizes();
    private void ResolveIntrinsicTrackSizes();
    private void MaximizeTracks();
    private void ExpandFlexibleTracks();
    private void StretchAutoTracks();

    // Internal types
    private class GridTrack { ... }
    private class GridArea { ... }
    private class GridItem { ... }
}
```

### 2.4 Track Sizing Algorithm

Implement per [§11 Grid Sizing Algorithm](https://www.w3.org/TR/css-grid-1/#layout-algorithm):

- [ ] **§11.4** Initialize Track Sizes
- [ ] **§11.5** Resolve Intrinsic Track Sizes
  - Size tracks to fit content
  - Handle spanning items
- [ ] **§11.6** Maximize Tracks
- [ ] **§11.7** Expand Flexible Tracks (fr units)
- [ ] **§11.8** Stretch `auto` Tracks

### 2.5 Item Placement Algorithm

Implement per [§8.5 Grid Item Placement Algorithm](https://www.w3.org/TR/css-grid-1/#auto-placement-algo):

- [ ] Place explicitly positioned items
- [ ] Process items locked to a row/column
- [ ] Place remaining items (sparse/dense packing)

### 2.6 Testing

- [ ] Unit tests for track sizing
- [ ] Unit tests for item placement
- [ ] Integration tests for common patterns:
  - Basic grid
  - Named areas
  - Auto-placement
  - Responsive grids
- [ ] Run against WPT css-grid tests

### Deliverables

- 13+ new CSS properties
- Grid value types
- `GridLayoutDirector` class
- Comprehensive test suite

---

## Phase 3: Alignment Properties

**Goal**: Implement CSS Box Alignment properties for use with flex and grid.

**Spec Reference**: [CSS Box Alignment Level 3](https://www.w3.org/TR/css-align-3/)

### 3.1 Property Definitions

- [ ] Add alignment properties to `ECssPropertyID`:

  ```
  AlignContent, JustifyContent, PlaceContent,
  AlignItems, JustifyItems, PlaceItems,
  AlignSelf, JustifySelf, PlaceSelf,
  RowGap, ColumnGap, Gap
  ```

- [ ] Create alignment enums:
  - `EAlignContent` { Normal, Start, End, Center, SpaceBetween, SpaceAround, SpaceEvenly, Stretch, Baseline }
  - `EJustifyContent` { similar values }
  - `EAlignItems` { Normal, Start, End, Center, Stretch, Baseline }
  - `EAlignSelf` { Auto, Normal, Start, End, Center, Stretch, Baseline }

### 3.2 Alignment Resolution

- [ ] Implement alignment calculation helpers
- [ ] Handle baseline alignment
- [ ] Handle `auto` margins interaction

### 3.3 Gap Implementation

- [ ] Add gap calculations to flex layout
- [ ] Add gap calculations to grid layout
- [ ] Handle percentage gaps

### Deliverables

- 12 new CSS properties
- Alignment enums
- Alignment calculation utilities

---

## Phase 4: Advanced Features

**Goal**: Add supporting features and polish.

### 4.1 CSS Functions

- [ ] Implement `calc()` evaluation
  - Parse math expressions
  - Handle unit mixing
  - Support nested `calc()`
- [ ] Implement `var()` custom properties
  - Custom property storage
  - Fallback values
  - Inheritance

### 4.2 Fragmentation Support

- [ ] Flex fragmentation (multi-line, page breaks)
- [ ] Grid fragmentation

### 4.3 Performance Optimization

- [ ] Cache intrinsic size calculations
- [ ] Optimize re-layout paths
- [ ] Profile and optimize hot paths

### 4.4 Documentation

- [ ] API documentation
- [ ] Usage examples
- [ ] Migration guide from legacy layout

---

## Implementation Notes

### Maintaining Backwards Compatibility

Per project decisions:

- Keep `CssLayoutDirector` (box model) and `StackLayoutDirector` separate
- New flex/grid directors are independent implementations
- Existing display modes continue to work as before

### Testing Strategy

1. **Unit tests first** — Test each algorithm step in isolation
2. **Integration tests** — Test complete layouts
3. **WPT conformance** — Validate against Web Platform Tests
4. **Visual regression** — Compare rendered output

### File Structure

```
CSS/
├── Layouts/
│   ├── ILayoutDirector.cs
│   ├── LayoutDirectorBase.cs
│   ├── Layout_BoxModel.cs        (existing)
│   ├── Layout_StackModel.cs      (existing)
│   ├── FlexLayoutDirector.cs     (new)
│   └── GridLayoutDirector.cs     (new)
├── Enums/
│   └── Style Properties/
│       ├── EFlexDirection.cs     (new)
│       ├── EFlexWrap.cs          (new)
│       ├── EAlignContent.cs      (new)
│       └── ...
└── Properties/
    └── Definitions/
        └── CssDefinitions.cs     (extend)
```

---

## Success Criteria

- [ ] All flex properties parse and apply correctly
- [ ] Flex layout matches browser behavior for common patterns
- [ ] All grid properties parse and apply correctly
- [ ] Grid layout matches browser behavior for common patterns
- [ ] Alignment works consistently across flex and grid
- [ ] > 80% pass rate on relevant WPT tests
- [ ] No regressions in existing layout modes
