using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using CssUI.CSS;
using CssUI.Enums;

namespace CssUI.Fonts;

/// <summary>
/// Provides generic font family mappings based on script/culture.
/// Works in both full and headless modes.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/2018/REC-css-fonts-3-20180920/
/// </remarks>
public static class GenericFontFamilies
{
    /// <summary>
    /// Maps generic font family keywords to lists of specific font family names.
    /// </summary>
    public static readonly ConcurrentDictionary<EGenericFontFamily, List<CssValue>> Map = new();

    /// <summary>
    /// Fallback font family names to try when no specified font works.
    /// </summary>
    public static readonly List<string> Fallbacks = new()
    {
        "Verdana",
        "Courier",
        "Arial",
        "Calibri",
        "sans-serif"
    };

    static GenericFontFamilies()
    {
        // Auto-detect culture and setup appropriate font families
        SetupForCurrentCulture();
    }

    /// <summary>
    /// Setup font families based on current thread culture.
    /// </summary>
    public static void SetupForCurrentCulture()
    {
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        SetupForCulture(currentCulture.Name);
    }

    /// <summary>
    /// Setup font families for a specific culture/script.
    /// </summary>
    public static void SetupForCulture(string cultureName)
    {
        // Clear existing mappings
        Map.Clear();

        switch (cultureName?.ToLowerInvariant())
        {
            case EISO_15924.Greek:
                SetupGreekScript();
                break;
            case EISO_15924.Cyrillic:
                SetupCyrillicScript();
                break;
            case EISO_15924.Japanese_alias_for_Han__Hiragana__Katakana:
                SetupJapaneseScript();
                break;
            case EISO_15924.Hebrew:
                SetupHebrewScript();
                break;
            case EISO_15924.Cherokee:
                SetupCherokeeScript();
                break;
            case EISO_15924.Arabic:
                SetupArabicScript();
                break;
            case EISO_15924.Latin:
            default:
                SetupLatinScript();
                break;
        }
    }

    #region Script-Specific Font Family Setup

    /// <summary>
    /// Latin script font families (default).
    /// </summary>
    public static void SetupLatinScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Times New Roman"),
            CssValue.From_String("Bodoni"),
            CssValue.From_String("Garamond"),
            CssValue.From_String("Minion Web"),
            CssValue.From_String("ITC Stone Serif"),
            CssValue.From_String("MS Georgia"),
            CssValue.From_String("Bitstream Cyberbit")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("MS Trebuchet"),
            CssValue.From_String("ITC Avant Garde Gothic"),
            CssValue.From_String("MS Arial"),
            CssValue.From_String("MS Verdana"),
            CssValue.From_String("Univers"),
            CssValue.From_String("Futura"),
            CssValue.From_String("ITC Stone Sans"),
            CssValue.From_String("Gill Sans"),
            CssValue.From_String("Akzidenz Grotesk"),
            CssValue.From_String("Helvetica")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>
        {
            CssValue.From_String("Courier"),
            CssValue.From_String("MS Courier New"),
            CssValue.From_String("Prestige"),
            CssValue.From_String("Everson Mono")
        });

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>
        {
            CssValue.From_String("Caflisch Script"),
            CssValue.From_String("Adobe Poetica"),
            CssValue.From_String("Sanvito"),
            CssValue.From_String("Ex Ponto"),
            CssValue.From_String("Snell Roundhand"),
            CssValue.From_String("Zapf-Chancery")
        });

        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>
        {
            CssValue.From_String("Alpha Geometrique"),
            CssValue.From_String("Critter"),
            CssValue.From_String("Cottonwood"),
            CssValue.From_String("FB Reactor"),
            CssValue.From_String("Studz")
        });
    }

    /// <summary>
    /// Greek script font families.
    /// </summary>
    public static void SetupGreekScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Bitstream Cyberbit")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("Attika"),
            CssValue.From_String("Typiko New Era"),
            CssValue.From_String("MS Tahoma"),
            CssValue.From_String("Monotype Gill Sans 571"),
            CssValue.From_String("Helvetica Greek")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>
        {
            CssValue.From_String("MS Courier New"),
            CssValue.From_String("Everson Mono")
        });

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>());
        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>());
    }

    /// <summary>
    /// Cyrillic script font families.
    /// </summary>
    public static void SetupCyrillicScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Adobe Minion Cyrillic"),
            CssValue.From_String("Excelsior Cyrillic Upright"),
            CssValue.From_String("Monotype Albion 70"),
            CssValue.From_String("Bitstream Cyberbit"),
            CssValue.From_String("ER Bukinist")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("Helvetica Cyrillic"),
            CssValue.From_String("ER Univers"),
            CssValue.From_String("Lucida Sans Unicode"),
            CssValue.From_String("Bastion")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>
        {
            CssValue.From_String("ER Kurier"),
            CssValue.From_String("Everson Mono")
        });

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>
        {
            CssValue.From_String("ER Architekt")
        });

        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>());
    }

    /// <summary>
    /// Japanese script font families.
    /// </summary>
    public static void SetupJapaneseScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Ryumin Light-KL"),
            CssValue.From_String("Kyokasho ICA"),
            CssValue.From_String("Futo Min A101")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("Shin Go"),
            CssValue.From_String("Heisei Kaku Gothic W5")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>
        {
            CssValue.From_String("Osaka Monospaced")
        });

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>());
        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>());
    }

    /// <summary>
    /// Hebrew script font families.
    /// </summary>
    public static void SetupHebrewScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("New Peninim"),
            CssValue.From_String("Raanana"),
            CssValue.From_String("Bitstream Cyberbit")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("Arial Hebrew"),
            CssValue.From_String("MS Tahoma")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>());

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>
        {
            CssValue.From_String("Corsiva")
        });

        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>());
    }

    /// <summary>
    /// Cherokee script font families.
    /// </summary>
    public static void SetupCherokeeScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Lo Cicero Cherokee")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>());

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>
        {
            CssValue.From_String("Everson Mono")
        });

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>());
        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>());
    }

    /// <summary>
    /// Arabic script font families.
    /// </summary>
    public static void SetupArabicScript()
    {
        Map.TryAdd(EGenericFontFamily.Serif, new List<CssValue>
        {
            CssValue.From_String("Bitstream Cyberbit")
        });

        Map.TryAdd(EGenericFontFamily.SansSerif, new List<CssValue>
        {
            CssValue.From_String("MS Tahoma")
        });

        Map.TryAdd(EGenericFontFamily.Monospace, new List<CssValue>());

        Map.TryAdd(EGenericFontFamily.Cursive, new List<CssValue>
        {
            CssValue.From_String("DecoType Naskh"),
            CssValue.From_String("Monotype Urdu 507")
        });

        Map.TryAdd(EGenericFontFamily.Fantasy, new List<CssValue>());
    }

    #endregion

    #region Lookup Methods

    /// <summary>
    /// Get font family names for a generic family keyword.
    /// </summary>
    public static bool TryGetFamilies(EGenericFontFamily genericFamily, out List<CssValue>? families)
    {
        return Map.TryGetValue(genericFamily, out families);
    }

    /// <summary>
    /// Translates a font weight value (100-900) into common font subfamily names.
    /// </summary>
    public static string[] TranslateFontWeightToNames(int weight)
    {
        if (weight <= 100) return new[] { "Thin" };
        if (weight <= 200) return new[] { "Extra Light", "Ultra Light" };
        if (weight <= 300) return new[] { "Light" };
        if (weight <= 400) return new[] { "Normal", "Regular" };
        if (weight <= 500) return new[] { "Medium" };
        if (weight <= 600) return new[] { "Semi Bold", "Demi Bold" };
        if (weight <= 700) return new[] { "Bold" };
        if (weight <= 800) return new[] { "Extra Bold", "Ultra Bold" };
        if (weight <= 900) return new[] { "Black", "Heavy" };

        return System.Array.Empty<string>();
    }

    #endregion
}

