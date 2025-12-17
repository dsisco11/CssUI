using System.Collections.Concurrent;
using System.Collections.Generic;
using CssUI.CSS;

namespace CssUI.Fonts;

/// <summary>
/// Provides generic font family mappings.
/// Works in both full and headless modes.
/// </summary>
public static class GenericFontFamilies
{
    /// <summary>
    /// Maps generic font family keywords to lists of specific font family names.
    /// </summary>
    public static readonly ConcurrentDictionary<EGenericFontFamily, List<CssValue>> Map = new();

    static GenericFontFamilies()
    {
        // Default Latin script mappings
        // These can be overridden by FontManager when the full font system is enabled
        SetupLatinDefaults();
    }

    private static void SetupLatinDefaults()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Times New Roman"),
            CssValue.From_String("Georgia"),
            CssValue.From_String("Garamond")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("Arial"),
            CssValue.From_String("Helvetica"),
            CssValue.From_String("Verdana"),
            CssValue.From_String("Trebuchet MS")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>
        {
            CssValue.From_String("Courier New"),
            CssValue.From_String("Consolas"),
            CssValue.From_String("Monaco")
        });

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>
        {
            CssValue.From_String("Comic Sans MS"),
            CssValue.From_String("Brush Script MT")
        });

        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>
        {
            CssValue.From_String("Impact"),
            CssValue.From_String("Papyrus")
        });
    }

    /// <summary>
    /// Get font family names for a generic family keyword.
    /// </summary>
    public static bool TryGetFamilies(EGenericFontFamily genericFamily, out List<CssValue> families)
    {
        return Map.TryGetValue(genericFamily, out families);
    }
}
