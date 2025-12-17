# CSS Specifications Reference

This document compiles W3C CSS specifications relevant to implementing modern layout features in CssUI. Specifications are organized by implementation priority.

---

## Phase 0: Intrinsic Sizing Prerequisites

### CSS Sizing Level 3

**URL**: <https://www.w3.org/TR/css-sizing-3/>

**Key Sections**:

- [§4 Sizing Keywords](https://www.w3.org/TR/css-sizing-3/#sizing-values) — `min-content`, `max-content`, `fit-content()`, `stretch`
- [§5 Intrinsic Size Determination](https://www.w3.org/TR/css-sizing-3/#intrinsic-sizes) — How to calculate intrinsic sizes
- [§5.1 Intrinsic Sizes](https://www.w3.org/TR/css-sizing-3/#intrinsic-size-input) — min-content/max-content definitions

**Required for**: Flex item sizing, grid track sizing, `auto` keyword resolution

---

## Phase 1: Flexbox Implementation

### CSS Flexible Box Layout Module Level 1

**URL**: <https://www.w3.org/TR/css-flexbox-1/>

**Status**: W3C Candidate Recommendation (stable)

**Key Sections**:

| Section                                                             | Topic                   | Implementation Notes                                |
| ------------------------------------------------------------------- | ----------------------- | --------------------------------------------------- |
| [§2](https://www.w3.org/TR/css-flexbox-1/#box-model)                | Box Model & Terminology | Main/cross axis, flex lines                         |
| [§3](https://www.w3.org/TR/css-flexbox-1/#flex-containers)          | Flex Containers         | `display: flex \| inline-flex`                      |
| [§4](https://www.w3.org/TR/css-flexbox-1/#flex-items)               | Flex Items              | Anonymous boxes, abspos handling                    |
| [§5](https://www.w3.org/TR/css-flexbox-1/#ordering-and-orientation) | Ordering & Orientation  | `flex-direction`, `flex-wrap`, `flex-flow`, `order` |
| [§7](https://www.w3.org/TR/css-flexbox-1/#flexibility)              | Flexibility             | `flex-grow`, `flex-shrink`, `flex-basis`, `flex`    |
| [§8](https://www.w3.org/TR/css-flexbox-1/#alignment)                | Alignment               | Uses CSS Box Alignment                              |
| [§9](https://www.w3.org/TR/css-flexbox-1/#layout-algorithm)         | **Layout Algorithm**    | 9-step algorithm (critical)                         |

**Layout Algorithm Steps** (§9):

1. §9.1 — Initial Setup
2. §9.2 — Line Length Determination
3. §9.3 — Main Size Determination
4. §9.4 — Cross Size Determination
5. §9.5 — Main-Axis Alignment
6. §9.6 — Cross-Axis Alignment
7. §9.7 — Resolving Flexible Lengths
8. §9.8 — Definite and Indefinite Sizes
9. §9.9 — Intrinsic Sizes

**Properties to Implement**:

```
flex-direction: row | row-reverse | column | column-reverse
flex-wrap: nowrap | wrap | wrap-reverse
flex-flow: <flex-direction> || <flex-wrap>
order: <integer>
flex-grow: <number>
flex-shrink: <number>
flex-basis: content | <width>
flex: none | [ <flex-grow> <flex-shrink>? || <flex-basis> ]
```

---

## Phase 2: Grid Implementation

### CSS Grid Layout Module Level 1

**URL**: <https://www.w3.org/TR/css-grid-1/>

**Status**: W3C Candidate Recommendation (stable)

**Key Sections**:

| Section                                                       | Topic                     | Implementation Notes           |
| ------------------------------------------------------------- | ------------------------- | ------------------------------ |
| [§5](https://www.w3.org/TR/css-grid-1/#grid-model)            | Grid Containers           | `display: grid \| inline-grid` |
| [§6](https://www.w3.org/TR/css-grid-1/#grid-items)            | Grid Items                | Anonymous items, ordering      |
| [§7](https://www.w3.org/TR/css-grid-1/#grid-definition)       | Defining the Grid         | Explicit vs implicit grid      |
| [§7.2](https://www.w3.org/TR/css-grid-1/#track-sizing)        | Track Sizing              | `grid-template-rows/columns`   |
| [§7.2.4](https://www.w3.org/TR/css-grid-1/#fr-unit)           | Flexible Lengths          | The `fr` unit                  |
| [§8](https://www.w3.org/TR/css-grid-1/#placement)             | Placing Grid Items        | Line-based placement           |
| [§8.5](https://www.w3.org/TR/css-grid-1/#auto-placement-algo) | Auto-Placement Algorithm  | Sparse vs dense packing        |
| [§10](https://www.w3.org/TR/css-grid-1/#alignment)            | Alignment                 | Uses CSS Box Alignment         |
| [§11](https://www.w3.org/TR/css-grid-1/#layout-algorithm)     | **Grid Sizing Algorithm** | Track sizing (critical)        |

**Grid Sizing Algorithm Steps** (§11):

1. §11.1 — Grid Sizing Algorithm overview
2. §11.2 — Track Sizing Terminology
3. §11.3 — Track Sizing Algorithm
4. §11.4 — Initialize Track Sizes
5. §11.5 — Resolve Intrinsic Track Sizes
6. §11.6 — Maximize Tracks
7. §11.7 — Expand Flexible Tracks
8. §11.8 — Stretch `auto` Tracks

**Properties to Implement**:

```
grid-template-columns: none | <track-list> | <auto-track-list>
grid-template-rows: none | <track-list> | <auto-track-list>
grid-template-areas: none | <string>+
grid-template: shorthand
grid-auto-columns: <track-size>+
grid-auto-rows: <track-size>+
grid-auto-flow: [ row | column ] || dense
grid: shorthand

grid-row-start: <grid-line>
grid-column-start: <grid-line>
grid-row-end: <grid-line>
grid-column-end: <grid-line>
grid-row: <grid-line> [ / <grid-line> ]?
grid-column: <grid-line> [ / <grid-line> ]?
grid-area: <grid-line> [ / <grid-line> ]{0,3}
```

### CSS Grid Layout Module Level 2

**URL**: <https://www.w3.org/TR/css-grid-2/>

**Status**: W3C Candidate Recommendation

**Additional Features**:

- Subgrid (`grid-template-*: subgrid`)
- Masonry layout (experimental)

_Consider for future phases_

---

## Phase 3: Alignment Properties

### CSS Box Alignment Module Level 3

**URL**: <https://www.w3.org/TR/css-align-3/>

**Status**: W3C Working Draft (but widely implemented)

**Key Sections**:

| Section                                                        | Topic                 | Applies To                                                 |
| -------------------------------------------------------------- | --------------------- | ---------------------------------------------------------- |
| [§4](https://www.w3.org/TR/css-align-3/#alignment-values)      | Alignment Keywords    | All alignment properties                                   |
| [§4.1](https://www.w3.org/TR/css-align-3/#positional-values)   | Positional Alignment  | `start`, `end`, `center`, `flex-start`, `flex-end`         |
| [§4.2](https://www.w3.org/TR/css-align-3/#baseline-values)     | Baseline Alignment    | `baseline`, `first baseline`, `last baseline`              |
| [§4.3](https://www.w3.org/TR/css-align-3/#distribution-values) | Distributed Alignment | `space-between`, `space-around`, `space-evenly`, `stretch` |
| [§5](https://www.w3.org/TR/css-align-3/#content-distribution)  | Content Distribution  | `align-content`, `justify-content`                         |
| [§6](https://www.w3.org/TR/css-align-3/#self-alignment)        | Self-Alignment        | `align-self`, `justify-self`                               |
| [§7](https://www.w3.org/TR/css-align-3/#default-alignment)     | Default Alignment     | `align-items`, `justify-items`                             |
| [§8](https://www.w3.org/TR/css-align-3/#gaps)                  | Gaps                  | `gap`, `row-gap`, `column-gap`                             |

**Properties to Implement**:

```
align-content: normal | <baseline-position> | <content-distribution> | <content-position>
justify-content: normal | <content-distribution> | <content-position>
place-content: <align-content> <justify-content>?

align-items: normal | stretch | <baseline-position> | <self-position>
justify-items: normal | stretch | <baseline-position> | <self-position> | legacy
place-items: <align-items> <justify-items>?

align-self: auto | normal | stretch | <baseline-position> | <self-position>
justify-self: auto | normal | stretch | <baseline-position> | <self-position>
place-self: <align-self> <justify-self>?

row-gap: normal | <length-percentage>
column-gap: normal | <length-percentage>
gap: <row-gap> <column-gap>?
```

---

## Phase 4: Supporting Specifications

### CSS Display Module Level 3

**URL**: <https://www.w3.org/TR/css-display-3/>

**Key Sections**:

- [§2](https://www.w3.org/TR/css-display-3/#the-display-properties) — The `display` property (two-value syntax)
- [§2.7](https://www.w3.org/TR/css-display-3/#transformations) — Blockification and inlinification
- [§3](https://www.w3.org/TR/css-display-3/#order-property) — The `order` property

### CSS Box Model Module Level 4

**URL**: <https://www.w3.org/TR/css-box-4/>

**Key Sections**:

- Box edges and areas (content, padding, border, margin)
- Margin collapsing rules

### CSS Values and Units Module Level 4

**URL**: <https://www.w3.org/TR/css-values-4/>

**Key Sections**:

- [§8](https://www.w3.org/TR/css-values-4/#calc-notation) — `calc()` function
- [§10](https://www.w3.org/TR/css-values-4/#lengths) — Length units

### CSS Custom Properties for Cascading Variables Module Level 1

**URL**: <https://www.w3.org/TR/css-variables-1/>

**Key Sections**:

- `--*` custom property definitions
- `var()` function

---

## Test Suites

### Web Platform Tests (WPT)

- **Flexbox**: <https://wpt.fyi/results/css/css-flexbox>
- **Grid**: <https://wpt.fyi/results/css/css-grid>
- **Alignment**: <https://wpt.fyi/results/css/css-align>

### CSS Working Group Test Suites

- <https://test.csswg.org/suites/>

---

## Quick Reference: Value Types

### Track Size Values (Grid)

```
<track-size> = <track-breadth> | minmax( <inflexible-breadth> , <track-breadth> )
<track-breadth> = <length-percentage> | <flex> | min-content | max-content | auto
<inflexible-breadth> = <length-percentage> | min-content | max-content | auto
<flex> = <number>fr
```

### Alignment Values

```
<baseline-position> = [ first | last ]? baseline
<content-distribution> = space-between | space-around | space-evenly | stretch
<content-position> = center | start | end | flex-start | flex-end
<self-position> = center | start | end | self-start | self-end | flex-start | flex-end
```

---

## Editor's Drafts (Latest)

For the most current specification text:

- Flexbox: <https://drafts.csswg.org/css-flexbox-1/>
- Grid: <https://drafts.csswg.org/css-grid-1/>
- Alignment: <https://drafts.csswg.org/css-align/>
- Sizing: <https://drafts.csswg.org/css-sizing-3/>
- Display: <https://drafts.csswg.org/css-display/>
