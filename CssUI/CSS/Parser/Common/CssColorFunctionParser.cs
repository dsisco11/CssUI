using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parses CSS color function syntax (rgb, rgba, hsl, hsla, etc.) per CSS Color Level 4.
/// </summary>
/// <seealso href="https://www.w3.org/TR/css-color-4/#rgb-functions"/>
internal static class CssColorFunctionParser
{
    #region Constants
    private const double BYTE_MAX = 255.0;
    private const double PERCENT_MAX = 100.0;
    #endregion

    /// <summary>
    /// Attempts to parse a color function (rgb, rgba, hsl, hsla, hwb, etc.) and return a CssColor.
    /// </summary>
    /// <param name="function">The CSS function to parse.</param>
    /// <param name="color">The resulting color if successful.</param>
    /// <returns>True if the function was a valid color function and parsing succeeded.</returns>
    public static bool TryParseColorFunction(CssFunction function, out CssColor color)
    {
        ArgumentNullException.ThrowIfNull(function);

        color = CssColor.Transparent;

        // Check function name (case-insensitive)
        var name = function.Name;
        if (name.Equals("rgb", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("rgba", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseRgb(function.Arguments, out color);
        }

        if (name.Equals("hsl", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("hsla", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseHsl(function.Arguments, out color);
        }

        if (name.Equals("hwb", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseHwb(function.Arguments, out color);
        }

        if (name.Equals("lab", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseLab(function.Arguments, out color);
        }

        if (name.Equals("lch", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseLch(function.Arguments, out color);
        }

        // @todo: Add oklab, oklch, color() parsing

        return false;
    }

    /// <summary>
    /// Parses rgb() and rgba() color functions.
    /// Supports both legacy (comma-separated) and modern (space-separated) syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#rgb-functions"/>
    private static bool TryParseRgb(List<CssToken> arguments, out CssColor color)
    {
        color = CssColor.Transparent;

        // Extract meaningful tokens (skip whitespace)
        var tokens = ExtractMeaningfulTokens(arguments);
        if (tokens.Count == 0)
        {
            return false;
        }

        // Detect syntax type: legacy uses commas, modern uses spaces
        bool isLegacySyntax = ContainsCommas(arguments);

        if (isLegacySyntax)
        {
            return TryParseRgbLegacy(tokens, out color);
        }
        else
        {
            return TryParseRgbModern(tokens, out color);
        }
    }

    /// <summary>
    /// Parses legacy comma-separated rgb()/rgba() syntax.
    /// Legacy syntax: rgb(r, g, b) or rgba(r, g, b, a)
    /// Components must be all numbers (0-255) or all percentages.
    /// </summary>
    private static bool TryParseRgbLegacy(List<CssToken> tokens, out CssColor color)
    {
        color = CssColor.Transparent;

        // Legacy syntax requires 3 or 4 values separated by commas
        // After filtering, we should have 3 or 4 meaningful tokens
        if (tokens.Count < 3 || tokens.Count > 4)
        {
            return false;
        }

        // Determine if we're using percentages or numbers based on first token
        bool usePercentages = tokens[0].Type == ECssTokenType.Percentage;

        // Validate that all RGB components use the same type (all % or all numbers)
        for (int i = 0; i < 3; i++)
        {
            if (usePercentages && tokens[i].Type != ECssTokenType.Percentage)
            {
                return false; // Mixed types not allowed in legacy syntax
            }
            if (!usePercentages && tokens[i].Type != ECssTokenType.Number)
            {
                return false;
            }
        }

        // Parse R, G, B
        double r, g, b;
        if (usePercentages)
        {
            if (!TryGetPercentage(tokens[0], out r) ||
                !TryGetPercentage(tokens[1], out g) ||
                !TryGetPercentage(tokens[2], out b))
            {
                return false;
            }
            // Convert percentage (0-100) to byte (0-255)
            r = PercentToRgbComponent(r);
            g = PercentToRgbComponent(g);
            b = PercentToRgbComponent(b);
        }
        else
        {
            if (!TryGetNumber(tokens[0], out r) ||
                !TryGetNumber(tokens[1], out g) ||
                !TryGetNumber(tokens[2], out b))
            {
                return false;
            }
            // Clamp to valid range
            r = ClampRgbComponent(r);
            g = ClampRgbComponent(g);
            b = ClampRgbComponent(b);
        }

        // Parse alpha if present
        double a = BYTE_MAX; // Default to fully opaque
        if (tokens.Count == 4)
        {
            if (!TryGetAlphaValue(tokens[3], out a))
            {
                return false;
            }
        }

        color = new CssColor((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b), (byte)Math.Round(a));
        return true;
    }

    /// <summary>
    /// Parses modern space-separated rgb()/rgba() syntax.
    /// Modern syntax: rgb(r g b) or rgb(r g b / alpha)
    /// Components can freely mix numbers and percentages.
    /// </summary>
    private static bool TryParseRgbModern(List<CssToken> tokens, out CssColor color)
    {
        color = CssColor.Transparent;

        // Modern syntax: at least 3 components, optionally alpha after '/'
        if (tokens.Count < 3)
        {
            return false;
        }

        // Find the slash separator for alpha
        int slashIndex = -1;
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] is DelimToken delim && delim.Value == '/')
            {
                slashIndex = i;
                break;
            }
        }

        // Determine RGB token count
        int rgbCount = slashIndex >= 0 ? slashIndex : tokens.Count;
        if (rgbCount < 3)
        {
            return false;
        }

        // Parse R, G, B (first 3 tokens before slash or at indices 0,1,2)
        double r, g, b;
        if (!TryGetRgbComponent(tokens[0], out r) ||
            !TryGetRgbComponent(tokens[1], out g) ||
            !TryGetRgbComponent(tokens[2], out b))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double a = BYTE_MAX; // Default to fully opaque
        if (slashIndex >= 0 && slashIndex + 1 < tokens.Count)
        {
            if (!TryGetAlphaValue(tokens[slashIndex + 1], out a))
            {
                return false;
            }
        }

        color = new CssColor((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b), (byte)Math.Round(a));
        return true;
    }

    #region HSL Parsing

    /// <summary>
    /// Parses hsl() and hsla() color functions.
    /// Supports both legacy (comma-separated) and modern (space-separated) syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#the-hsl-notation"/>
    private static bool TryParseHsl(List<CssToken> arguments, out CssColor color)
    {
        color = CssColor.Transparent;

        // Extract meaningful tokens (skip whitespace and commas for detection)
        var tokens = ExtractMeaningfulTokens(arguments);
        if (tokens.Count == 0)
        {
            return false;
        }

        // Detect syntax type: legacy uses commas, modern uses spaces
        bool isLegacySyntax = ContainsCommas(arguments);

        if (isLegacySyntax)
        {
            return TryParseHslLegacy(tokens, out color);
        }
        else
        {
            return TryParseHslModern(tokens, out color);
        }
    }

    /// <summary>
    /// Parses legacy comma-separated hsl()/hsla() syntax.
    /// Legacy syntax: hsl(hue, saturation%, lightness%) or hsla(hue, sat%, light%, alpha)
    /// Saturation and lightness must be percentages in legacy syntax.
    /// </summary>
    private static bool TryParseHslLegacy(List<CssToken> tokens, out CssColor color)
    {
        color = CssColor.Transparent;

        // Legacy syntax requires 3 or 4 values separated by commas
        if (tokens.Count < 3 || tokens.Count > 4)
        {
            return false;
        }

        // Parse hue (number or angle)
        if (!TryGetHueValue(tokens[0], out double hue))
        {
            return false;
        }

        // In legacy syntax, saturation and lightness must be percentages
        if (tokens[1].Type != ECssTokenType.Percentage ||
            tokens[2].Type != ECssTokenType.Percentage)
        {
            return false;
        }

        if (!TryGetPercentage(tokens[1], out double saturation) ||
            !TryGetPercentage(tokens[2], out double lightness))
        {
            return false;
        }

        // Per spec: negative saturation is clamped to 0 at parse time
        saturation = Math.Max(0, saturation);

        // Parse alpha if present
        double alpha = 1.0; // Default to fully opaque
        if (tokens.Count == 4)
        {
            if (!TryGetAlphaValueNormalized(tokens[3], out alpha))
            {
                return false;
            }
        }

        // Convert HSL to RGB
        return HslToRgb(hue, saturation, lightness, alpha, out color);
    }

    /// <summary>
    /// Parses modern space-separated hsl()/hsla() syntax.
    /// Modern syntax: hsl(hue saturation lightness) or hsl(hue sat light / alpha)
    /// Saturation and lightness can be percentages or numbers.
    /// </summary>
    private static bool TryParseHslModern(List<CssToken> tokens, out CssColor color)
    {
        color = CssColor.Transparent;

        // Modern syntax: at least 3 components, optionally alpha after '/'
        if (tokens.Count < 3)
        {
            return false;
        }

        // Find the slash separator for alpha
        int slashIndex = -1;
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] is DelimToken delim && delim.Value == '/')
            {
                slashIndex = i;
                break;
            }
        }

        // Determine HSL token count (should be at least 3 before slash)
        int hslCount = slashIndex >= 0 ? slashIndex : tokens.Count;
        if (hslCount < 3)
        {
            return false;
        }

        // Parse hue (number or angle)
        if (!TryGetHueValue(tokens[0], out double hue))
        {
            return false;
        }

        // Parse saturation (percentage or number, where number is treated as percentage)
        if (!TryGetSaturationOrLightness(tokens[1], out double saturation))
        {
            return false;
        }

        // Parse lightness (percentage or number, where number is treated as percentage)
        if (!TryGetSaturationOrLightness(tokens[2], out double lightness))
        {
            return false;
        }

        // Per spec: negative saturation is clamped to 0 at parse time
        saturation = Math.Max(0, saturation);

        // Parse alpha if present (after the slash)
        double alpha = 1.0; // Default to fully opaque
        if (slashIndex >= 0 && slashIndex + 1 < tokens.Count)
        {
            if (!TryGetAlphaValueNormalized(tokens[slashIndex + 1], out alpha))
            {
                return false;
            }
        }

        // Convert HSL to RGB
        return HslToRgb(hue, saturation, lightness, alpha, out color);
    }

    /// <summary>
    /// Converts HSL color values to an RGB CssColor.
    /// Algorithm per CSS Color Level 4 specification.
    /// </summary>
    /// <param name="hue">Hue angle in degrees (will be normalized to [0,360))</param>
    /// <param name="saturation">Saturation percentage (0-100)</param>
    /// <param name="lightness">Lightness percentage (0-100)</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    /// <param name="color">The resulting RGB color</param>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#hsl-to-rgb"/>
    private static bool HslToRgb(double hue, double saturation, double lightness, double alpha, out CssColor color)
    {
        // Normalize hue to [0, 360)
        hue = NormalizeHue(hue);

        // Convert to [0,1] range
        double sat = saturation / 100.0;
        double light = lightness / 100.0;

        // HSL to RGB conversion per W3C spec
        // https://www.w3.org/TR/css-color-4/#hsl-to-rgb
        double r = HslF(0, hue, sat, light);
        double g = HslF(8, hue, sat, light);
        double b = HslF(4, hue, sat, light);

        // Convert to 0-255 range and clamp
        int red = (int)Math.Round(Math.Clamp(r, 0, 1) * 255);
        int green = (int)Math.Round(Math.Clamp(g, 0, 1) * 255);
        int blue = (int)Math.Round(Math.Clamp(b, 0, 1) * 255);
        int a = (int)Math.Round(Math.Clamp(alpha, 0, 1) * 255);

        color = new CssColor((byte)red, (byte)green, (byte)blue, (byte)a);
        return true;
    }

    /// <summary>
    /// Helper function for HSL to RGB conversion per W3C spec.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double HslF(int n, double hue, double sat, double light)
    {
        double k = (n + hue / 30.0) % 12.0;
        double a = sat * Math.Min(light, 1.0 - light);
        return light - a * Math.Max(-1.0, Math.Min(Math.Min(k - 3.0, 9.0 - k), 1.0));
    }

    /// <summary>
    /// Normalizes a hue angle to the range [0, 360).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double NormalizeHue(double hue)
    {
        hue = hue % 360.0;
        if (hue < 0)
        {
            hue += 360.0;
        }
        return hue;
    }

    /// <summary>
    /// Tries to get a hue value from a token.
    /// Hue can be a number (interpreted as degrees), or an angle with unit (deg, rad, grad, turn).
    /// </summary>
    private static bool TryGetHueValue(CssToken token, out double hue)
    {
        hue = 0;

        // Number token: interpreted as degrees
        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out hue))
            {
                return false;
            }
            return true;
        }

        // Dimension token: angle with unit
        if (token is DimensionToken dimToken)
        {
            double value = Convert.ToDouble(dimToken.Number);
            var unit = dimToken.Unit;

            // Convert to degrees based on unit
            if (unit.Equals("deg", StringComparison.OrdinalIgnoreCase))
            {
                hue = value;
            }
            else if (unit.Equals("rad", StringComparison.OrdinalIgnoreCase))
            {
                hue = value * (180.0 / Math.PI);
            }
            else if (unit.Equals("grad", StringComparison.OrdinalIgnoreCase))
            {
                hue = value * (360.0 / 400.0);
            }
            else if (unit.Equals("turn", StringComparison.OrdinalIgnoreCase))
            {
                hue = value * 360.0;
            }
            else
            {
                return false; // Unknown angle unit
            }
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            hue = 0; // 'none' hue is treated as 0 for calculation purposes
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get a saturation or lightness value from a token.
    /// Can be a percentage (0-100%) or a number (interpreted as 0-100).
    /// </summary>
    private static bool TryGetSaturationOrLightness(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            return true;
        }

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // Numbers are interpreted as the same scale as percentages
            // Per spec: 0% = 0.0, 100% = 100.0 for S and L
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get a normalized alpha value (0-1) from a token.
    /// Alpha can be a number (0-1) or percentage (0-100%).
    /// </summary>
    private static bool TryGetAlphaValueNormalized(CssToken token, out double alpha)
    {
        alpha = 1.0; // Default fully opaque

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out var num))
            {
                return false;
            }
            alpha = Math.Clamp(num, 0, 1);
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out var pct))
            {
                return false;
            }
            alpha = Math.Clamp(pct / 100.0, 0, 1);
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            alpha = 1.0; // 'none' alpha defaults to fully opaque
            return true;
        }

        return false;
    }

    #endregion

    #region HWB Parsing

    /// <summary>
    /// Parses hwb() color function.
    /// HWB only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#the-hwb-notation"/>
    private static bool TryParseHwb(List<CssToken> arguments, out CssColor color)
    {
        color = CssColor.Transparent;

        // HWB does NOT support legacy comma syntax per spec
        // "Using commas inside hwb() is an error."
        if (ContainsCommas(arguments))
        {
            return false;
        }

        // Extract meaningful tokens (skip whitespace)
        var tokens = ExtractMeaningfulTokens(arguments);
        if (tokens.Count < 3)
        {
            return false;
        }

        // Find the slash separator for alpha
        int slashIndex = -1;
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] is DelimToken delim && delim.Value == '/')
            {
                slashIndex = i;
                break;
            }
        }

        // Determine HWB token count (should be at least 3 before slash)
        int hwbCount = slashIndex >= 0 ? slashIndex : tokens.Count;
        if (hwbCount < 3)
        {
            return false;
        }

        // Parse hue (number or angle) - same as HSL
        if (!TryGetHueValue(tokens[0], out double hue))
        {
            return false;
        }

        // Parse whiteness (percentage or number, where number is treated as percentage)
        if (!TryGetWhitenessOrBlackness(tokens[1], out double whiteness))
        {
            return false;
        }

        // Parse blackness (percentage or number, where number is treated as percentage)
        if (!TryGetWhitenessOrBlackness(tokens[2], out double blackness))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0; // Default to fully opaque
        if (slashIndex >= 0 && slashIndex + 1 < tokens.Count)
        {
            if (!TryGetAlphaValueNormalized(tokens[slashIndex + 1], out alpha))
            {
                return false;
            }
        }

        // Convert HWB to RGB
        return HwbToRgb(hue, whiteness, blackness, alpha, out color);
    }

    /// <summary>
    /// Converts HWB color values to an RGB CssColor.
    /// Algorithm per CSS Color Level 4 specification.
    /// </summary>
    /// <param name="hue">Hue angle in degrees (will be normalized to [0,360))</param>
    /// <param name="whiteness">Whiteness percentage (0-100)</param>
    /// <param name="blackness">Blackness percentage (0-100)</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    /// <param name="color">The resulting RGB color</param>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#hwb-to-rgb"/>
    private static bool HwbToRgb(double hue, double whiteness, double blackness, double alpha, out CssColor color)
    {
        // Normalize whiteness and blackness to [0, 1]
        double white = whiteness / 100.0;
        double black = blackness / 100.0;

        // Per spec: If white + black >= 1, the result is an achromatic (gray) color
        // The hue becomes powerless and the result is gray = white / (white + black)
        if (white + black >= 1.0)
        {
            double gray = white / (white + black);
            int grayByte = (int)Math.Round(Math.Clamp(gray, 0, 1) * 255);
            int a = (int)Math.Round(Math.Clamp(alpha, 0, 1) * 255);
            color = new CssColor((byte)grayByte, (byte)grayByte, (byte)grayByte, (byte)a);
            return true;
        }

        // First, convert HSL(hue, 100%, 50%) to get the base RGB color
        // This is the fully saturated color at the given hue
        hue = NormalizeHue(hue);
        double r = HslF(0, hue, 1.0, 0.5);
        double g = HslF(8, hue, 1.0, 0.5);
        double b = HslF(4, hue, 1.0, 0.5);

        // Apply whiteness and blackness
        // Per spec: rgb[i] = rgb[i] * (1 - white - black) + white
        r = r * (1.0 - white - black) + white;
        g = g * (1.0 - white - black) + white;
        b = b * (1.0 - white - black) + white;

        // Convert to 0-255 range and clamp
        int red = (int)Math.Round(Math.Clamp(r, 0, 1) * 255);
        int green = (int)Math.Round(Math.Clamp(g, 0, 1) * 255);
        int blue = (int)Math.Round(Math.Clamp(b, 0, 1) * 255);
        int aVal = (int)Math.Round(Math.Clamp(alpha, 0, 1) * 255);

        color = new CssColor((byte)red, (byte)green, (byte)blue, (byte)aVal);
        return true;
    }

    /// <summary>
    /// Tries to get a whiteness or blackness value from a token.
    /// Can be a percentage (0-100%) or a number (interpreted as 0-100).
    /// </summary>
    private static bool TryGetWhitenessOrBlackness(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // Per spec: whiteness and blackness are not clamped until conversion
            return true;
        }

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // Numbers are interpreted as the same scale as percentages
            // Per spec: 0% = 0.0, 100% = 100.0 for W and B
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = 0;
            return true;
        }

        return false;
    }

    #endregion

    #region Lab/LCH Parsing

    /// <summary>
    /// Reference white point D50 for CIE Lab (used by CSS Color 4).
    /// </summary>
    private static readonly double[] D50 = { 0.3457 / 0.3585, 1.0, (1.0 - 0.3457 - 0.3585) / 0.3585 };

    /// <summary>
    /// Reference white point D65 for sRGB conversions.
    /// </summary>
    private static readonly double[] D65 = { 0.3127 / 0.3290, 1.0, (1.0 - 0.3127 - 0.3290) / 0.3290 };

    /// <summary>
    /// Parses lab() color function.
    /// Lab only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#specifying-lab-lch"/>
    private static bool TryParseLab(List<CssToken> arguments, out CssColor color)
    {
        color = CssColor.Transparent;

        // Lab does NOT support legacy comma syntax per spec
        // "Using commas inside lab() is an error."
        if (ContainsCommas(arguments))
        {
            return false;
        }

        // Extract meaningful tokens (skip whitespace)
        var tokens = ExtractMeaningfulTokens(arguments);
        if (tokens.Count < 3)
        {
            return false;
        }

        // Find the slash separator for alpha
        int slashIndex = FindSlashIndex(tokens);

        // Determine Lab token count (should be at least 3 before slash)
        int labCount = slashIndex >= 0 ? slashIndex : tokens.Count;
        if (labCount < 3)
        {
            return false;
        }

        // Parse L (Lightness) - 0-100 or 0%-100%, clamped to [0,100]
        if (!TryGetLabLightness(tokens[0], out double lightness))
        {
            return false;
        }

        // Parse a - signed value, percentage maps to [-125, 125]
        if (!TryGetLabAB(tokens[1], out double a))
        {
            return false;
        }

        // Parse b - signed value, percentage maps to [-125, 125]
        if (!TryGetLabAB(tokens[2], out double b))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        if (slashIndex >= 0 && slashIndex + 1 < tokens.Count)
        {
            if (!TryGetAlphaValueNormalized(tokens[slashIndex + 1], out alpha))
            {
                return false;
            }
        }

        // Convert Lab to sRGB
        return LabToRgb(lightness, a, b, alpha, out color);
    }

    /// <summary>
    /// Parses lch() color function.
    /// LCH only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#specifying-lab-lch"/>
    private static bool TryParseLch(List<CssToken> arguments, out CssColor color)
    {
        color = CssColor.Transparent;

        // LCH does NOT support legacy comma syntax per spec
        // "Using commas inside lch() is an error."
        if (ContainsCommas(arguments))
        {
            return false;
        }

        // Extract meaningful tokens (skip whitespace)
        var tokens = ExtractMeaningfulTokens(arguments);
        if (tokens.Count < 3)
        {
            return false;
        }

        // Find the slash separator for alpha
        int slashIndex = FindSlashIndex(tokens);

        // Determine LCH token count (should be at least 3 before slash)
        int lchCount = slashIndex >= 0 ? slashIndex : tokens.Count;
        if (lchCount < 3)
        {
            return false;
        }

        // Parse L (Lightness) - 0-100 or 0%-100%, clamped to [0,100]
        if (!TryGetLabLightness(tokens[0], out double lightness))
        {
            return false;
        }

        // Parse C (Chroma) - >= 0, percentage maps to [0, 150], clamped to >= 0
        if (!TryGetLchChroma(tokens[1], out double chroma))
        {
            return false;
        }

        // Parse H (Hue) - same as HSL/HWB
        if (!TryGetHueValue(tokens[2], out double hue))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        if (slashIndex >= 0 && slashIndex + 1 < tokens.Count)
        {
            if (!TryGetAlphaValueNormalized(tokens[slashIndex + 1], out alpha))
            {
                return false;
            }
        }

        // Convert LCH to Lab, then Lab to sRGB
        return LchToRgb(lightness, chroma, hue, alpha, out color);
    }

    /// <summary>
    /// Finds the index of the slash separator in the token list.
    /// </summary>
    private static int FindSlashIndex(List<CssToken> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] is DelimToken delim && delim.Value == '/')
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Tries to get a Lab/LCH Lightness value from a token.
    /// Can be a number (0-100) or percentage (0%-100%).
    /// Clamped to [0, 100] at parse time per spec.
    /// </summary>
    private static bool TryGetLabLightness(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // Clamp to [0, 100] at parse time per spec
            value = Math.Clamp(value, 0, 100);
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // Percentage: 0% = 0, 100% = 100
            // Clamp to [0, 100] at parse time per spec
            value = Math.Clamp(value, 0, 100);
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get a Lab 'a' or 'b' axis value from a token.
    /// Can be a number (unbounded, typically -160 to +160) or percentage (-100% to 100% maps to -125 to +125).
    /// </summary>
    private static bool TryGetLabAB(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // No clamping - values are signed and theoretically unbounded
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // Percentage: -100% = -125, 0% = 0, 100% = 125
            value = value * 1.25;
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get an LCH Chroma value from a token.
    /// Can be a number (>= 0, typically 0-230) or percentage (0-100% maps to 0-150).
    /// Negative values are clamped to 0 at parse time per spec.
    /// </summary>
    private static bool TryGetLchChroma(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // Clamp negative to 0 at parse time per spec
            value = Math.Max(0, value);
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // Percentage: 0% = 0, 100% = 150
            value = value * 1.5;
            // Clamp negative to 0 at parse time per spec
            value = Math.Max(0, value);
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts CIE Lab color values to an sRGB CssColor.
    /// Algorithm per CSS Color Level 4 specification.
    /// </summary>
    /// <param name="lightness">L* lightness (0-100)</param>
    /// <param name="a">a* axis (typically -160 to +160)</param>
    /// <param name="b">b* axis (typically -160 to +160)</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    /// <param name="color">The resulting sRGB color</param>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#lab-to-predefined"/>
    private static bool LabToRgb(double lightness, double a, double b, double alpha, out CssColor color)
    {
        // Step 1: Convert Lab to D50-adapted XYZ
        var xyz = LabToXyz(lightness, a, b);

        // Step 2: Chromatic adaptation from D50 to D65 (Bradford transform)
        xyz = D50ToD65(xyz);

        // Step 3: Convert D65 XYZ to linear sRGB
        var linearRgb = XyzToLinearSrgb(xyz);

        // Step 4: Apply sRGB gamma encoding
        var srgb = LinearSrgbToSrgb(linearRgb);

        // Step 5: Clamp to [0, 1] and convert to bytes
        // Note: Out-of-gamut colors are allowed but must be gamut-mapped for display
        int red = (int)Math.Round(Math.Clamp(srgb[0], 0, 1) * 255);
        int green = (int)Math.Round(Math.Clamp(srgb[1], 0, 1) * 255);
        int blue = (int)Math.Round(Math.Clamp(srgb[2], 0, 1) * 255);
        int aVal = (int)Math.Round(Math.Clamp(alpha, 0, 1) * 255);

        color = new CssColor((byte)red, (byte)green, (byte)blue, (byte)aVal);
        return true;
    }

    /// <summary>
    /// Converts CIE LCH color values to an sRGB CssColor.
    /// First converts LCH to Lab, then Lab to sRGB.
    /// </summary>
    /// <param name="lightness">L* lightness (0-100)</param>
    /// <param name="chroma">C* chroma (>= 0)</param>
    /// <param name="hue">H hue angle in degrees</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    /// <param name="color">The resulting sRGB color</param>
    private static bool LchToRgb(double lightness, double chroma, double hue, double alpha, out CssColor color)
    {
        // Convert LCH to Lab
        // a = C * cos(H)
        // b = C * sin(H)
        hue = NormalizeHue(hue);
        double hueRad = hue * Math.PI / 180.0;
        double a = chroma * Math.Cos(hueRad);
        double b = chroma * Math.Sin(hueRad);

        // Now convert Lab to sRGB
        return LabToRgb(lightness, a, b, alpha, out color);
    }

    /// <summary>
    /// Converts CIE Lab to D50-adapted XYZ.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#lab-to-predefined"/>
    private static double[] LabToXyz(double L, double a, double b)
    {
        // CIE constants
        const double kappa = 24389.0 / 27.0;   // 29^3/3^3
        const double epsilon = 216.0 / 24389.0; // 6^3/29^3

        // Compute f values
        double f1 = (L + 16.0) / 116.0;
        double f0 = a / 500.0 + f1;
        double f2 = f1 - b / 200.0;

        // Compute xyz (scaled relative to reference white)
        double x = Math.Pow(f0, 3) > epsilon
            ? Math.Pow(f0, 3)
            : (116.0 * f0 - 16.0) / kappa;

        double y = L > kappa * epsilon
            ? Math.Pow((L + 16.0) / 116.0, 3)
            : L / kappa;

        double z = Math.Pow(f2, 3) > epsilon
            ? Math.Pow(f2, 3)
            : (116.0 * f2 - 16.0) / kappa;

        // Scale by D50 reference white
        return new double[]
        {
            x * D50[0],
            y * D50[1],
            z * D50[2]
        };
    }

    /// <summary>
    /// Chromatic adaptation from D50 to D65 using linear Bradford transform.
    /// </summary>
    private static double[] D50ToD65(double[] xyz)
    {
        // Bradford matrix for D50 -> D65
        // Pre-computed matrix from W3C spec
        double[,] M = {
            {  0.9554734527042182,  -0.023098536874261423,  0.0632593086610217  },
            { -0.028369706963208136, 1.0099954580106629,    0.021041398966943008 },
            {  0.012314001688319899, -0.020507696433477912, 1.3303659366080753   }
        };

        return MultiplyMatrix3x3(M, xyz);
    }

    /// <summary>
    /// Converts D65-adapted XYZ to linear sRGB.
    /// </summary>
    private static double[] XyzToLinearSrgb(double[] xyz)
    {
        // Matrix from W3C spec (inverse of sRGB to XYZ matrix)
        double[,] M = {
            {  3.2404541621141054,  -1.5371385940306089, -0.49853140955601579 },
            { -0.96926603050518312,  1.8760108454466942,  0.041556017530349834 },
            {  0.055643430959114726, -0.20397695888897652, 1.0569715142428786  }
        };

        return MultiplyMatrix3x3(M, xyz);
    }

    /// <summary>
    /// Converts linear sRGB to gamma-encoded sRGB.
    /// </summary>
    private static double[] LinearSrgbToSrgb(double[] linear)
    {
        return new double[]
        {
            LinearToGamma(linear[0]),
            LinearToGamma(linear[1]),
            LinearToGamma(linear[2])
        };
    }

    /// <summary>
    /// Applies sRGB gamma encoding to a single component.
    /// Extended transfer function handles negative values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double LinearToGamma(double c)
    {
        double sign = c < 0 ? -1.0 : 1.0;
        double abs = Math.Abs(c);

        if (abs > 0.0031308)
        {
            return sign * (1.055 * Math.Pow(abs, 1.0 / 2.4) - 0.055);
        }

        return 12.92 * c;
    }

    /// <summary>
    /// Multiplies a 3x3 matrix by a 3-element vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double[] MultiplyMatrix3x3(double[,] matrix, double[] vector)
    {
        return new double[]
        {
            matrix[0, 0] * vector[0] + matrix[0, 1] * vector[1] + matrix[0, 2] * vector[2],
            matrix[1, 0] * vector[0] + matrix[1, 1] * vector[1] + matrix[1, 2] * vector[2],
            matrix[2, 0] * vector[0] + matrix[2, 1] * vector[1] + matrix[2, 2] * vector[2]
        };
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Extracts meaningful tokens from the arguments list, skipping whitespace and commas.
    /// </summary>
    private static List<CssToken> ExtractMeaningfulTokens(List<CssToken> arguments)
    {
        var result = new List<CssToken>();
        foreach (var token in arguments)
        {
            // Skip whitespace and commas
            if (token.Type == ECssTokenType.Whitespace ||
                token.Type == ECssTokenType.Comma)
            {
                continue;
            }
            result.Add(token);
        }
        return result;
    }

    /// <summary>
    /// Checks if the arguments contain comma separators (indicating legacy syntax).
    /// </summary>
    private static bool ContainsCommas(List<CssToken> arguments)
    {
        foreach (var token in arguments)
        {
            if (token.Type == ECssTokenType.Comma)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Tries to get an RGB component value from a token (number or percentage).
    /// Numbers are expected in range 0-255, percentages in range 0-100.
    /// </summary>
    private static bool TryGetRgbComponent(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // Clamp to valid range [0, 255]
            value = ClampRgbComponent(value);
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // Convert percentage to 0-255 range
            value = PercentToRgbComponent(value);
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = 0;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get an alpha value from a token.
    /// Alpha can be a number (0-1) or percentage (0-100%).
    /// </summary>
    private static bool TryGetAlphaValue(CssToken token, out double value)
    {
        value = BYTE_MAX; // Default fully opaque

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out var num))
            {
                return false;
            }
            // Alpha as number is 0-1 range, convert to 0-255
            value = Math.Clamp(num, 0, 1) * BYTE_MAX;
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out var pct))
            {
                return false;
            }
            // Alpha as percentage is 0-100, convert to 0-255
            value = Math.Clamp(pct, 0, 100) / PERCENT_MAX * BYTE_MAX;
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = BYTE_MAX; // 'none' alpha defaults to fully opaque per spec
            return true;
        }

        return false;
    }

    /// <summary>
    /// Extracts a numeric value from a NumberToken.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetNumber(CssToken token, out double value)
    {
        value = 0;
        if (token is not NumberToken numToken || numToken.Number is null)
        {
            return false;
        }

        value = Convert.ToDouble(numToken.Number);
        return true;
    }

    /// <summary>
    /// Extracts a percentage value from a PercentageToken.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetPercentage(CssToken token, out double value)
    {
        value = 0;
        if (token is not PercentageToken pctToken)
        {
            return false;
        }

        value = pctToken.Number;
        return true;
    }

    /// <summary>
    /// Converts a percentage (0-100) to an RGB component (0-255).
    /// Values are clamped to valid range.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double PercentToRgbComponent(double percent)
    {
        return Math.Clamp(percent / PERCENT_MAX * BYTE_MAX, 0, BYTE_MAX);
    }

    /// <summary>
    /// Clamps an RGB component to the valid range [0, 255].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double ClampRgbComponent(double value)
    {
        return Math.Clamp(value, 0, BYTE_MAX);
    }

    #endregion
}
