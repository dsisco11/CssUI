# CssUI Implementation Status

This document provides a comprehensive overview of the current implementation state of the CssUI C# CSS interpreter library, last active circa 2019.

## Summary

CssUI is a C# implementation of a CSS rendering engine with DOM integration. The project includes a complete CSS tokenizer and parser following CSS Syntax Level 3, a partial box model implementation, and basic layout algorithms. The primary gaps are in modern layout modes (Flexbox, Grid) and some CSS3 features.

---

## ✅ Implemented Features

### CSS Tokenizer (`CSS/Tokenizer/`)

- **Complete** implementation per [CSS Syntax Level 3](https://www.w3.org/TR/css-syntax-3/)
- 876 lines of tokenizer code in `CssTokenizer.cs`
- Full token type enumeration (`ECssTokenType.cs`)
- Handles escaped characters, unicode, whitespace normalization
- Preprocesses input per CSS specification

### CSS Parser (`CSS/Parser/`)

- **Complete** rule parsing (at-rules, qualified rules)
- Declaration parsing and declaration lists
- Component value consumption
- Function token parsing (`CssFunction` class)
- Selector parsing with pseudo-classes/elements
- 902 lines of parser code in `CssParser.cs`

### CSS Properties (42 implemented)

#### Font Properties (5)

| Property      | Status                               |
| ------------- | ------------------------------------ |
| `line-height` | ✅ Inherited, percentage resolution  |
| `font-family` | ✅ Generic families, string values   |
| `font-weight` | ✅ Keywords + numeric (100-900)      |
| `font-style`  | ✅ normal/italic/oblique             |
| `font-size`   | ✅ Keywords, dimensions, percentages |

#### Visual/Rendering Properties (13)

| Property          | Status                                |
| ----------------- | ------------------------------------- |
| `color`           | ✅ Keywords, hex, rgb values          |
| `opacity`         | ✅ 0.0-1.0 range                      |
| `border-*-color`  | ✅ All four sides, currentColor       |
| `border-*-style`  | ✅ All four sides (none, solid, etc.) |
| `transform`       | ✅ Transform list support             |
| `scroll-behavior` | ✅ auto/smooth                        |
| `overflow-x/y`    | ✅ visible/hidden/scroll/auto         |

#### Box Model Properties (22)

| Property                      | Status                                |
| ----------------------------- | ------------------------------------- |
| `display`                     | ✅ Enum with flex/grid values defined |
| `box-sizing`                  | ✅ content-box/border-box             |
| `position` (as `positioning`) | ✅ relative/absolute/fixed            |
| `top/right/bottom/left`       | ✅ Auto, dimensions, percentages      |
| `width/height`                | ✅ Auto, dimensions, percentages      |
| `min-width/min-height`        | ✅ With `EBoxSize` keywords           |
| `max-width/max-height`        | ✅ With `EBoxSize` keywords           |
| `padding-*`                   | ✅ All four sides                     |
| `border-*-width`              | ✅ All four sides, keywords           |
| `margin-*`                    | ✅ All four sides, auto               |

#### Layout Properties (3)

| Property       | Status                         |
| -------------- | ------------------------------ |
| `direction`    | ✅ ltr/rtl                     |
| `writing-mode` | ✅ horizontal-tb, etc.         |
| `text-align`   | ✅ start/end/left/right/center |

#### Sizing Properties (2)

| Property          | Status                     |
| ----------------- | -------------------------- |
| `object-fit`      | ✅ fill/contain/cover/etc. |
| `object-position` | ✅ Keyword + percentage    |

### Display Mode Enum (`EDisplayMode`)

All modern display values are **defined** (as enum flags):

- `none`, `contents`, `inline`, `block`, `run-in`
- `flex`, `inline-flex` ⚠️ (enum only, no algorithm)
- `grid`, `inline-grid` ⚠️ (enum only, no algorithm)
- `flow`, `flow-root`
- `table-*` variants (10 values)
- `list-item`

### Box Size Keywords (`EBoxSize`)

- `min-content` ✅ Defined
- `max-content` ✅ Defined
- `fit-content` ✅ Defined

### Layout Directors (`CSS/Layouts/`)

| Director                      | Status                                    |
| ----------------------------- | ----------------------------------------- |
| `LayoutDirectorBase`          | ✅ Abstract base with line-box management |
| `Layout_BoxModel`             | ✅ Basic CSS2 box model layout            |
| `Layout_StackModel`           | ✅ Simple stacking layout                 |
| `LineBox` / `LineBox_Element` | ✅ Inline formatting support              |

### Box Model (`CSS/Box Model/`)

- Box area calculations
- Content/padding/border/margin edges
- Fragmentation types defined
- Factory patterns for box creation

### DOM Integration (`DOM/`)

Comprehensive DOM implementation:

- Document, Element, Node hierarchies
- Window/Browsing context
- Events and event handling
- Custom elements support
- Range and traversal APIs
- Mutation observers

### CSS Value System (`CSS/Value/`)

- `CssValue` class with multiple value types
- Type coercion and conversion
- Percentage resolution framework
- Property stage resolvers (Specified → Computed → Used)
- Color value handling

### Media Queries (`CSS/Media/`)

- Media feature definitions (width, height, orientation, etc.)
- Range and discrete feature types
- Resolution queries

---

## ⚠️ Partial / Incomplete

### Intrinsic Sizing

- `EBoxSize` enum has `min-content`/`max-content`/`fit-content` **keywords defined**
- Intrinsic ratio calculations exist in `CssAlgorithms.cs` (for replaced elements)
- **Missing**: Full intrinsic size computation for non-replaced elements (required for flex/grid)

### CSS Functions

- `CssFunction` class exists and parses function tokens
- Test case shows `calc(1 / 100 + 5)` parsing
- **Missing**: Actual `calc()` evaluation/resolution
- **Missing**: `var()` custom property support

### Background Properties

- Position algorithms exist (`Solve_Object_Axis_Position`)
- Referenced in code comments
- **Missing**: `background-color`, `background-image`, `background-*` property definitions

---

## ❌ Not Implemented

### Flexbox Layout Algorithm

- `EDisplayMode.FLEX` / `INLINE_FLEX` enum values exist
- **Missing**: `FlexLayoutDirector` class
- **Missing**: Flex properties (`flex-direction`, `flex-wrap`, `flex-grow`, `flex-shrink`, `flex-basis`, `flex-flow`, `flex`)
- **Missing**: Flex layout algorithm (9 steps per [CSS Flexbox Level 1](https://www.w3.org/TR/css-flexbox-1/#layout-algorithm))

### Grid Layout Algorithm

- `EDisplayMode.GRID` / `INLINE_GRID` enum values exist
- **Missing**: `GridLayoutDirector` class
- **Missing**: Grid properties (`grid-template-*`, `grid-auto-*`, `grid-column`, `grid-row`, `grid-area`)
- **Missing**: Track sizing algorithm per [CSS Grid Level 1](https://www.w3.org/TR/css-grid-1/#layout-algorithm)

### Alignment Properties

- **Missing**: `align-items`, `align-self`, `align-content`
- **Missing**: `justify-items`, `justify-self`, `justify-content`
- **Missing**: `place-*` shorthands
- **Missing**: `gap`, `row-gap`, `column-gap`

### Other Missing Features

- CSS Custom Properties (`--*` / `var()`)
- CSS Animations (`@keyframes`, `animation-*`)
- CSS Transitions (`transition-*`)
- CSS Filters (`filter`, `backdrop-filter`)
- CSS Shapes
- CSS Containment
- Logical properties (`margin-inline-*`, `padding-block-*`, etc.)

---

## Test Coverage (`CssUITests2/`)

Existing test directories:

- `CSS/Media/` - Media query tests
- `CSS/Parser/` - Parser tests
- `CSS/Tokenizer/` - Tokenizer tests
- `CSS/Tokens/` - Token type tests
- `CSS/Value/` - Value system tests
- `DOM/` - DOM implementation tests
- `CORE/` - Core utilities tests
- `RenderEngine/` - Rendering tests

---

## Architecture Notes

### Property Resolution Pipeline

```
Specified Value → Computed Value → Used Value → Actual Value
```

Each stage has resolver functions defined in `CssPropertyResolver*.cs` files.

### Typed Property System

Located in `CSS/Properties/Typed Properties/`:

- `ColorProperty`, `EnumProperty`, `IntProperty`
- `LengthProperty`, `NumberProperty`, `PositionProperty`
- `StringProperty`, `TransformListProperty`

### Dependency Tracking

Properties declare dirt flags (`EPropertyDirtFlags`) for:

- `Flow` - Affects layout flow
- `Visual` - Affects rendering only
- `Content_Area`, `Padding_Area`, `Border_Area`, `Margin_Area` - Box model regions
- `Text` - Affects text rendering
- `Replaced_Area` - Affects replaced element sizing

---

## Recommendations for Revival

1. **Phase 0**: Audit and complete intrinsic sizing (`min-content`/`max-content` computation)
2. **Phase 1**: Implement Flexbox properties and `FlexLayoutDirector`
3. **Phase 2**: Implement Grid properties and `GridLayoutDirector`
4. **Phase 3**: Add alignment properties (`align-*`, `justify-*`, `gap`)
5. **Phase 4**: CSS Functions (`calc()`, `var()`)

See [ROADMAP.md](ROADMAP.md) for detailed implementation plan.
