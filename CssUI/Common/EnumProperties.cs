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
