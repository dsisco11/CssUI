using EnumRecords;

namespace CssUI;

/// <summary>
/// Property record struct for enums that only need a keyword mapping.
/// Used by ~60 CSS/HTML/DOM enums for enum↔keyword conversion.
/// </summary>
public readonly record struct KeywordProperties(
    [ReverseLookup(IgnoreCase = true)] string Keyword
);

/// <summary>
/// Property record struct for URL scheme types (EUrlScheme).
/// Includes the scheme keyword and default port number (-1 for no default).
/// </summary>
public readonly record struct UrlSchemeProperties(
    [ReverseLookup(IgnoreCase = true)] string Keyword,
    int DefaultPort
);

/// <summary>
/// Property record struct for named CSS colors (EColor).
/// Includes keyword, hex value, and RGB components.
/// </summary>
public readonly record struct NamedColorProperties(
    [ReverseLookup(IgnoreCase = true)] string Keyword,
    int HexValue = 0,
    byte R = 0,
    byte G = 0,
    byte B = 0
);

/// <summary>
/// Property record struct for keyboard codes (EKeyboardCode).
/// Includes the code string and the character it represents.
/// </summary>
public readonly record struct KeyboardCodeProperties(
    [ReverseLookup(IgnoreCase = true)] string Keyword,
    char KeyChar = '\0'
);

/// <summary>
/// Property record struct for autofill tokens (EAutofill).
/// Includes keyword, max tokens allowed, and category (as int matching EAutofillCategory).
/// </summary>
public readonly record struct AutofillProperties(
    [ReverseLookup(IgnoreCase = true)] string Keyword,
    int MaxTokens = 0,
    int Category = 0  // Maps to EAutofillCategory enum value
);
