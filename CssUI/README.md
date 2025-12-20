# CssUI

A comprehensive C# CSS rendering engine with DOM integration, implementing modern CSS specifications including Flexbox, Grid Layout, and CSS Values Level 4 math functions.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Features

### ✅ CSS Layout Engines

-   **Flexbox Layout** (CSS Flexible Box Layout Module Level 1)

    -   All flex container properties (`flex-direction`, `flex-wrap`, `justify-content`, `align-items`, `align-content`)
    -   All flex item properties (`flex-grow`, `flex-shrink`, `flex-basis`, `order`, `align-self`)
    -   Multi-line wrapping with proper cross-axis alignment
    -   Gap support (`row-gap`, `column-gap`)

-   **Grid Layout** (CSS Grid Layout Module Level 1)

    -   Explicit grid definition (`grid-template-columns`, `grid-template-rows`)
    -   Flexible `fr` unit support with proper space distribution
    -   Auto-placement algorithm (row/column flow, dense packing)
    -   Grid item positioning (`grid-column-start/end`, `grid-row-start/end`)
    -   Track sizing: `auto`, `min-content`, `max-content`, `minmax()`, `fit-content()`
    -   Gap support (`row-gap`, `column-gap`)

-   **Intrinsic Sizing**
    -   `min-content`, `max-content`, `fit-content` sizing
    -   Proper intrinsic size calculation for text, blocks, and replaced elements

### ✅ CSS Values & Units (Level 4)

-   **Math Functions**

    -   Basic: `calc()`, `min()`, `max()`, `clamp()`
    -   Trigonometric: `sin()`, `cos()`, `tan()`, `asin()`, `acos()`, `atan()`, `atan2()`
    -   Exponential: `pow()`, `sqrt()`, `hypot()`, `log()`, `exp()`
    -   Sign-related: `abs()`, `sign()`
    -   Stepped values: `round()`, `mod()`, `rem()`

-   **Numeric Constants**: `e`, `pi`, `infinity`, `-infinity`, `NaN`

-   **IEEE-754 Semantics**: Division by zero returns ±∞, NaN propagation, signed zero handling

-   **Custom Properties**: `var()` with fallback values, inheritance, cycle detection

### ✅ CSS Properties (50+)

| Category      | Properties                                                                                                                                        |
| ------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Box Model** | `display`, `box-sizing`, `position`, `top/right/bottom/left`, `width/height`, `min-*/max-*`, `padding-*`, `margin-*`, `border-*`                  |
| **Flexbox**   | `flex-direction`, `flex-wrap`, `flex-grow`, `flex-shrink`, `flex-basis`, `order`, `justify-content`, `align-items`, `align-self`, `align-content` |
| **Grid**      | `grid-template-columns/rows`, `grid-auto-flow`, `grid-column/row-start/end`                                                                       |
| **Gap**       | `row-gap`, `column-gap`                                                                                                                           |
| **Font**      | `font-family`, `font-size`, `font-weight`, `font-style`, `line-height`                                                                            |
| **Visual**    | `color`, `opacity`, `background-*`, `border-*-color/style/width`, `overflow-x/y`                                                                  |
| **Sizing**    | `object-fit`, `object-position`                                                                                                                   |

### ✅ CSS Parser

-   Complete CSS Syntax Level 3 tokenizer and parser
-   Selector parsing with pseudo-classes/elements
-   At-rules and qualified rules
-   Function token parsing

### ✅ DOM Integration

-   CSS Box Tree generation from DOM
-   Style cascade and inheritance
-   Computed style resolution

## Architecture

```
CssUI/
├── CSS/
│   ├── Box Model/          # Box tree nodes, principal boxes
│   ├── Formatting/         # FlexFormattingContext, GridFormattingContext
│   ├── Functions/          # CssCalcEvaluator, CssVarResolver
│   ├── Parser/             # CSS tokenizer and parser
│   ├── Properties/         # Property definitions and computed styles
│   ├── Sizing/             # Intrinsic sizing infrastructure
│   ├── Types/              # GridTrackList, value types
│   └── Value/              # CssValue, unit handling
├── DOM/                    # DOM element hierarchy
├── RenderEngine/           # Plugin architecture for rendering
└── Fonts/                  # Font handling abstraction
```

## Usage

### Basic Setup

```csharp
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Formatting;

// Create a DOM element with styles
var element = new CssElement();
element.Style.Display = EDisplayMode.Flex;
element.Style.FlexDirection = EFlexDirection.Row;
element.Style.Width = 800;
element.Style.Height = 600;

// Add child elements
var child1 = new CssElement();
child1.Style.FlexGrow = 1;
element.appendChild(child1);

var child2 = new CssElement();
child2.Style.FlexGrow = 2;
element.appendChild(child2);

// Layout will be performed automatically
// child1 gets 1/3 of width, child2 gets 2/3
```

### Using calc() Functions

```csharp
using CssUI.CSS.Functions;

// Evaluate calc expressions
var evaluator = new CssCalcEvaluator();
double result = evaluator.Evaluate("calc(100px + 50%)".AsFunction(),
    percentageBase: 200);  // Returns 200 (100 + 50% of 200)

// Math functions
result = evaluator.Evaluate("calc(sin(pi / 2))".AsFunction());  // Returns 1
result = evaluator.Evaluate("calc(pow(2, 10))".AsFunction());   // Returns 1024
result = evaluator.Evaluate("calc(clamp(10, 50, 100))".AsFunction()); // Returns 50
```

### Grid Layout

```csharp
var grid = new CssElement();
grid.Style.Display = EDisplayMode.Grid;
grid.Style.GridTemplateColumns = "1fr 2fr 100px";  // Three columns
grid.Style.GridTemplateRows = "auto 1fr";          // Two rows
grid.Style.ColumnGap = 10;
grid.Style.RowGap = 10;

// Items are auto-placed or explicitly positioned
var item = new CssElement();
item.Style.GridColumnStart = 1;
item.Style.GridColumnEnd = 3;  // Spans 2 columns
```

### Custom Properties (CSS Variables)

```csharp
using CssUI.CSS.Functions;

var registry = new CssCustomPropertyRegistry();
registry.Set("--primary-color", CssValue.From(ReadOnlyColor.Blue));
registry.Set("--spacing", CssValue.From(16, ECssUnit.PX));

var resolver = new CssVarResolver(registry);
var value = resolver.Resolve("var(--spacing)");  // Returns 16px
var fallback = resolver.Resolve("var(--undefined, 10px)");  // Returns 10px
```

## Service Plugin Architecture

CssUI uses Microsoft.Extensions.DependencyInjection for service registration and resolution:

```csharp
using CssUI;
using CssUI.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

// Register CssUI services with your DI container
var services = new ServiceCollection();
services.AddCssUI(options =>
{
    options.FontService = new MyFontService();
    options.TextureService = new MyTextureService();
    options.RenderService = new MyRenderService();
});
var provider = services.BuildServiceProvider();

// Or use the extension methods
services.AddCssUI();
services.AddFontService<MyFontService>();
services.AddTextureService<MyTextureService>();
services.AddRenderService<MyRenderService>();
```

### Custom Service Implementations

```csharp
// Implement custom font service
public class MyFontService : IFontService
{
    public FontHandle ResolveFont(ReadOnlySpan<string> familyNames, float size,
        EFontWeight weight, EFontStyle style) { ... }
    public FontMetricsData GetMetrics(FontHandle font) { ... }
    public TextMeasurement MeasureText(FontHandle font, ReadOnlySpan<char> text) { ... }
    // ... other methods
}

// Implement custom texture service
public class MyTextureService : ITextureService
{
    public ImageData DecodeImage(ReadOnlySpan<byte> data) { ... }
    public TextureHandle CreateTexture(int width, int height, ReadOnlySpan<byte> rgbaPixels) { ... }
    // ... other methods
}

// Implement custom render service
public class MyRenderService : IRenderService
{
    public void BeginFrame() { ... }
    public void FillRect(RenderRect rect, Color color) { ... }
    public void DrawText(FontHandle font, ReadOnlySpan<char> text, RenderPoint position, Color color) { ... }
    // ... other methods
}
```

### Document Scoped Services

Each `Document` maintains its own service scope for proper isolation:

```csharp
// Services are resolved from the document's scope
var fontService = document.Services.GetRequiredService<IFontService>();
```

For headless operation (unit testing, server-side), null implementations (`NullFontService`, `NullTextureService`, `NullRenderService`) are used by default.

## Building

```bash
# Build the library
dotnet build CssUI/CssUI.csproj --configuration "Core - Debug"

# Run tests
dotnet test CssUITests2/CssUITests.csproj --configuration "Core - Debug"
```

## Test Coverage

-   **Intrinsic Sizing**: 45 tests
-   **Flexbox**: 9 tests
-   **Grid**: 36 tests (9 formatting + 27 track list)
-   **CSS calc()**: 42 tests
-   **CSS var()**: 15 tests
-   **Total**: 150+ unit tests

## Specifications Implemented

| Specification                                                   | Status              |
| --------------------------------------------------------------- | ------------------- |
| [CSS Syntax Level 3](https://www.w3.org/TR/css-syntax-3/)       | ✅ Complete         |
| [CSS Box Model Level 3](https://www.w3.org/TR/css-box-3/)       | ✅ Complete         |
| [CSS Flexbox Level 1](https://www.w3.org/TR/css-flexbox-1/)     | ✅ Complete         |
| [CSS Grid Level 1](https://www.w3.org/TR/css-grid-1/)           | ✅ Complete         |
| [CSS Values Level 4](https://www.w3.org/TR/css-values-4/)       | ✅ Math functions   |
| [CSS Custom Properties](https://www.w3.org/TR/css-variables-1/) | ✅ Complete         |
| [CSS Sizing Level 3](https://www.w3.org/TR/css-sizing-3/)       | ✅ Intrinsic sizing |

## License

MIT License - see [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please see the [project.todo](project.todo) file for current work items and priorities.
