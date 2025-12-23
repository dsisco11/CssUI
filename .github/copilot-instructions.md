# CssUI - AI Coding Agent Instructions

## Project Overview

CssUI is a C# CSS rendering engine implementing modern CSS specifications (Flexbox, Grid, CSS Values Level 4). It includes a DOM implementation, CSS parser, box tree model, and formatting contexts for layout.

**Code Standards:** Target .NET 8 with modern C# patterns. Prioritize performance-optimized abstractions: `Span<T>`, `ReadOnlySpan<T>`, `stackalloc`, pooled collections, and avoid allocations in hot paths.

**Important:** Do NOT use PowerShell or command line to manipulate files. Always use VS Code's built-in tools for file operations (create, edit, find-replace, rename, delete).

## Build & Test Commands

```bash
# Build (use "Core - Debug" for headless mode without font system)
dotnet build CssUI/CssUI.csproj --configuration "Core - Debug"

# Run tests
dotnet test CssUITests2/CssUITests.csproj --configuration "Core - Debug"
```

**Build Configurations:**

-   `Core - Debug/Release` — Headless mode, no HTML support
-   `CssUI - Debug/Release` — Full build with HTML support

## Architecture

```
CssUI/
├── CSS/
│   ├── Box Model/       # CssPrincipalBox - box tree nodes for layout
│   ├── Formatting/      # FlexFormattingContext, GridFormattingContext, BlockFormattingContext
│   ├── Properties/      # StyleDefinition, CssProperty - property cascade system
│   ├── Value/           # CssValue - immutable CSS value type with units
│   └── Parser/          # CSS tokenizer and parser (CSS Syntax Level 3)
├── DOM/                 # W3C DOM implementation (Node → Element → HTMLElement)
├── CORE/                # Platform abstractions, MetaTables, DI infrastructure
└── Fonts/               # IFontService abstraction
```

**Key Classes:**

-   `CssValue` — Immutable CSS value with type, unit, and boxed value (`CssUI/CSS/Value/Base/`)
-   `StyleDefinition` — Property metadata: initial value, inheritance, resolvers (`CssUI/CSS/Properties/Base/`)
-   `CssPrincipalBox` — Box tree node for layout (`CssUI/CSS/Box Model/Base/`)
-   `IFormattingContext` — Layout algorithm interface (`CssUI/CSS/Formatting/`)

## Critical Conventions

### Enum Naming (E-Prefix Pattern)

All enums use **E-prefix**: `EDisplayMode`, `ECssUnit`, `EFlexDirection`, `ENodeType`

### MetaEnum System for Keyword Mapping

Enums with CSS keyword mappings use `[MetaEnum]` + `[MetaKeyword]` attributes:

```csharp
[MetaEnum]
public enum EDisplayMode : int
{
    [MetaKeyword("none")] NONE = (1 << 0),
    [MetaKeyword("block")] BLOCK = (1 << 4),
    [MetaKeyword("flex")] FLEX = (1 << 6),
}
```

Use `Lookup.Get_Keywords<TEnum>()` to retrieve keyword↔enum mappings.

### CSS Property Definition Pattern

Define properties in `CssUI/CSS/Properties/Definitions/CssDefinitions.cs`:

```csharp
new StyleDefinition(
    ECssPropertyID.FlexGrow,
    inherits: false,
    EPropertyDirtFlags.Flow,
    CssValue.From(0.0),           // Initial value
    ECssValueTypes.NUMBER,         // Allowed types
    allowedKeywords: null,
    Percentage_Resolver: null,
    Resolvers: null)
```

### CssValue Factory Methods

Use static factory methods, not constructors:

-   `CssValue.From(double)`, `CssValue.From(double, ECssUnit)`, `CssValue.From<TEnum>()`
-   Singleton constants: `CssValue.Auto`, `CssValue.None`, `CssValue.Null`, `CssValue.Inherit`

### Service Abstraction Pattern

Services use interface + null object pattern for testability:

-   `IFontService` / `NullFontService`
-   `ITextureService` / `NullTextureService`
-   `IRenderService` / `NullRenderService`

Register via DI: `services.AddCssUI(options => { options.FontService = new MyFontService(); })`

## Testing Patterns

**Framework:** xUnit v3 with traits
**Location:** `CssUITests2/` mirrors `CssUI/` structure
**Naming:** `{ClassName}Tests.cs`, methods use descriptive names or `MethodName_Condition_ExpectedResult`

```csharp
[Fact]
[Trait("Category", "Flex")]
public void FlexFormattingContext_Flow_DistributesSpaceByGrowFactor()
{
    // Arrange → Act → Assert
}
```

## DOM Exception Hierarchy

DOM errors inherit from `DOMException` with specific types: `HierarchyRequestError`, `NotFoundError`, `InvalidStateError`, etc. Each maps to `EDOMExceptionCode`.

## Key File Locations

| Concept                  | Location                                                                    |
| ------------------------ | --------------------------------------------------------------------------- |
| CSS property definitions | `CssUI/CSS/Properties/Definitions/CssDefinitions.cs`                        |
| CSS value types          | `CssUI/CSS/Value/Base/CssValue.cs`                                          |
| Layout algorithms        | `CssUI/CSS/Formatting/FlexFormattingContext.cs`, `GridFormattingContext.cs` |
| Enum metadata system     | `CssUI/CORE/MetaTables/EnumMetaTable.cs`                                    |
| DOM Element base         | `CssUI/DOM/Elements/Base/Element.cs`                                        |
| Service registration     | `CssUI/CORE/DependencyInjection/`                                           |

## Specification References

**Always re-read the relevant W3C specification when working on any CSS/DOM system.** Fetch specs directly from official sources:

-   **W3C**: https://www.w3.org/TR/ (e.g., `css-flexbox-1`, `css-grid-1`, `css-values-4`)
-   **CSSWG Drafts**: https://drafts.csswg.org/
-   **WHATWG HTML**: https://html.spec.whatwg.org/
