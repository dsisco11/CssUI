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
        var arguments = function.Arguments;

        // Pre-check for legacy syntax (comma-separated) vs modern syntax (space-separated)
        bool isLegacySyntax = ContainsCommas(arguments);

        // Create the token stream once for all parsing
        var stream = CssParsingHelpers.CreateTokenStream(arguments);
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Check function name (case-insensitive)
        var name = function.Name;
        if (name.Equals("rgb", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("rgba", StringComparison.OrdinalIgnoreCase))
        {
            return isLegacySyntax
                ? TryParseRgbLegacy(stream, out color)
                : TryParseRgbModern(stream, out color);
        }

        if (name.Equals("hsl", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("hsla", StringComparison.OrdinalIgnoreCase))
        {
            return isLegacySyntax
                ? TryParseHslLegacy(stream, out color)
                : TryParseHslModern(stream, out color);
        }

        // Modern-only functions reject legacy comma syntax
        if (isLegacySyntax)
        {
            return false; // hwb, lab, lch, oklab, oklch, color() do not support comma syntax
        }

        if (name.Equals("hwb", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseHwb(stream, out color);
        }

        if (name.Equals("lab", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseLab(stream, out color);
        }

        if (name.Equals("lch", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseLch(stream, out color);
        }

        if (name.Equals("oklab", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseOklab(stream, out color);
        }

        if (name.Equals("oklch", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseOklch(stream, out color);
        }

        if (name.Equals("color", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseColorFunction(stream, out color);
        }

        return false;
    }

    /// <summary>
    /// Parses legacy comma-separated rgb()/rgba() syntax.
    /// Legacy syntax: rgb(r, g, b) or rgba(r, g, b, a)
    /// Components must be all numbers (0-255) or all percentages.
    /// Per CSS Color 4 §4.1.2: the 'none' value is NOT allowed in legacy syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#legacy-color-syntax"/>
    private static bool TryParseRgbLegacy(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Collect meaningful tokens (skip whitespace and commas) to validate type consistency
        // Legacy syntax requires validation that all RGB components use same type
        var tokens = new List<CssToken>(4);
        while (!CssParsingHelpers.IsAtEnd(stream))
        {
            var token = stream.Consume();
            // Skip whitespace and commas - commas are separators in legacy syntax
            if (token.Type == ECssTokenType.Whitespace || token.Type == ECssTokenType.Comma)
            {
                continue;
            }
            tokens.Add(token);
        }

        // Legacy syntax requires 3 or 4 values separated by commas
        if (tokens.Count < 3 || tokens.Count > 4)
        {
            return false;
        }

        // Per spec: 'none' keyword is NOT allowed in legacy syntax
        foreach (var token in tokens)
        {
            if (CssParsingHelpers.IsNoneKeyword(token))
            {
                return false;
            }
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
    private static bool TryParseRgbModern(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Modern syntax: at least 3 components, optionally alpha after '/'
        // Parse R component
        var rToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (rToken == null || !TryGetRgbComponent(rToken, out double r))
        {
            return false;
        }

        // Parse G component
        var gToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (gToken == null || !TryGetRgbComponent(gToken, out double g))
        {
            return false;
        }

        // Parse B component
        var bToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (bToken == null || !TryGetRgbComponent(bToken, out double b))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double a = BYTE_MAX; // Default to fully opaque
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValue(alphaToken, out a))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after RGB components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        color = new CssColor((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b), (byte)Math.Round(a));
        return true;
    }

    #region HSL Parsing

    /// <summary>
    /// Parses legacy comma-separated hsl()/hsla() syntax.
    /// Legacy syntax: hsl(hue, saturation%, lightness%) or hsla(hue, sat%, light%, alpha)
    /// Saturation and lightness must be percentages in legacy syntax.
    /// Per CSS Color 4 §4.1.2: the 'none' value is NOT allowed in legacy syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#legacy-color-syntax"/>
    private static bool TryParseHslLegacy(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Collect meaningful tokens (skip whitespace and commas) for validation
        var tokens = new List<CssToken>(4);
        while (!CssParsingHelpers.IsAtEnd(stream))
        {
            var token = stream.Consume();
            // Skip whitespace and commas - commas are separators in legacy syntax
            if (token.Type == ECssTokenType.Whitespace || token.Type == ECssTokenType.Comma)
            {
                continue;
            }
            tokens.Add(token);
        }

        // Legacy syntax requires 3 or 4 values separated by commas
        if (tokens.Count < 3 || tokens.Count > 4)
        {
            return false;
        }

        // Per spec: 'none' keyword is NOT allowed in legacy syntax
        foreach (var token in tokens)
        {
            if (CssParsingHelpers.IsNoneKeyword(token))
            {
                return false;
            }
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
    private static bool TryParseHslModern(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Modern syntax: at least 3 components, optionally alpha after '/'
        // Parse hue (number or angle)
        var hueToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (hueToken == null || !TryGetHueValue(hueToken, out double hue))
        {
            return false;
        }

        // Parse saturation (percentage or number, where number is treated as percentage)
        var satToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (satToken == null || !TryGetSaturationOrLightness(satToken, out double saturation))
        {
            return false;
        }

        // Parse lightness (percentage or number, where number is treated as percentage)
        var lightToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (lightToken == null || !TryGetSaturationOrLightness(lightToken, out double lightness))
        {
            return false;
        }

        // Per spec: negative saturation is clamped to 0 at parse time
        saturation = Math.Max(0, saturation);

        // Parse alpha if present (after the slash)
        double alpha = 1.0; // Default to fully opaque
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after HSL components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetHueValue(CssToken token, out double hue)
        => CssParsingHelpers.TryGetHueValue(token, out hue);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetAlphaValueNormalized(CssToken token, out double alpha)
        => CssParsingHelpers.TryGetAlphaValueNormalized(token, out alpha);

    #endregion

    #region HWB Parsing

    /// <summary>
    /// Parses hwb() color function.
    /// HWB only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#the-hwb-notation"/>
    private static bool TryParseHwb(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Parse hue (number or angle) - same as HSL
        var hueToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (hueToken == null || !TryGetHueValue(hueToken, out double hue))
        {
            return false;
        }

        // Parse whiteness (percentage or number, where number is treated as percentage)
        var whiteToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (whiteToken == null || !TryGetWhitenessOrBlackness(whiteToken, out double whiteness))
        {
            return false;
        }

        // Parse blackness (percentage or number, where number is treated as percentage)
        var blackToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (blackToken == null || !TryGetWhitenessOrBlackness(blackToken, out double blackness))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0; // Default to fully opaque
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after HWB components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
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
    private static bool TryParseLab(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Parse L (Lightness) - 0-100 or 0%-100%, clamped to [0,100]
        var lToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (lToken == null || !TryGetLabLightness(lToken, out double lightness))
        {
            return false;
        }

        // Parse a - signed value, percentage maps to [-125, 125]
        var aToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (aToken == null || !TryGetLabAB(aToken, out double a))
        {
            return false;
        }

        // Parse b - signed value, percentage maps to [-125, 125]
        var bToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (bToken == null || !TryGetLabAB(bToken, out double b))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after Lab components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Convert Lab to sRGB
        return LabToRgb(lightness, a, b, alpha, out color);
    }

    /// <summary>
    /// Parses lch() color function.
    /// LCH only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#specifying-lab-lch"/>
    private static bool TryParseLch(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Parse L (Lightness) - 0-100 or 0%-100%, clamped to [0,100]
        var lToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (lToken == null || !TryGetLabLightness(lToken, out double lightness))
        {
            return false;
        }

        // Parse C (Chroma) - >= 0, percentage maps to [0, 150], clamped to >= 0
        var cToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (cToken == null || !TryGetLchChroma(cToken, out double chroma))
        {
            return false;
        }

        // Parse H (Hue) - same as HSL/HWB
        var hToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (hToken == null || !TryGetHueValue(hToken, out double hue))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after LCH components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Convert LCH to Lab, then Lab to sRGB
        return LchToRgb(lightness, chroma, hue, alpha, out color);
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

    #region OKLab/OKLCh Parsing

    /// <summary>
    /// Parses oklab() color function.
    /// OKLab only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#specifying-oklab-oklch"/>
    private static bool TryParseOklab(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Parse L (Lightness) - 0-1 or 0%-100%, clamped to [0,1]
        var lToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (lToken == null || !TryGetOklabLightness(lToken, out double lightness))
        {
            return false;
        }

        // Parse a - signed value, percentage maps to [-0.4, 0.4]
        var aToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (aToken == null || !TryGetOklabAB(aToken, out double a))
        {
            return false;
        }

        // Parse b - signed value, percentage maps to [-0.4, 0.4]
        var bToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (bToken == null || !TryGetOklabAB(bToken, out double b))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after OKLab components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Convert OKLab to sRGB
        return OklabToRgb(lightness, a, b, alpha, out color);
    }

    /// <summary>
    /// Parses oklch() color function.
    /// OKLCh only supports modern (space-separated) syntax - no legacy comma syntax.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#specifying-oklab-oklch"/>
    private static bool TryParseOklch(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // Parse L (Lightness) - 0-1 or 0%-100%, clamped to [0,1]
        var lToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (lToken == null || !TryGetOklabLightness(lToken, out double lightness))
        {
            return false;
        }

        // Parse C (Chroma) - >= 0, percentage maps to [0, 0.4], clamped to >= 0
        var cToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (cToken == null || !TryGetOklchChroma(cToken, out double chroma))
        {
            return false;
        }

        // Parse H (Hue) - same as HSL/HWB/LCH
        var hToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (hToken == null || !TryGetHueValue(hToken, out double hue))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after OKLCh components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Convert OKLCh to OKLab, then OKLab to sRGB
        return OklchToRgb(lightness, chroma, hue, alpha, out color);
    }

    /// <summary>
    /// Tries to get an OKLab/OKLCh Lightness value from a token.
    /// Can be a number (0-1) or percentage (0%-100%).
    /// Clamped to [0, 1] at parse time per spec.
    /// </summary>
    private static bool TryGetOklabLightness(CssToken token, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // Clamp to [0, 1] at parse time per spec
            value = Math.Clamp(value, 0, 1);
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // Percentage: 0% = 0, 100% = 1
            value = value / 100.0;
            // Clamp to [0, 1] at parse time per spec
            value = Math.Clamp(value, 0, 1);
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
    /// Tries to get an OKLab 'a' or 'b' axis value from a token.
    /// Can be a number (unbounded, typically -0.5 to +0.5) or percentage (-100% to 100% maps to -0.4 to +0.4).
    /// </summary>
    private static bool TryGetOklabAB(CssToken token, out double value)
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
            // Percentage: -100% = -0.4, 0% = 0, 100% = 0.4
            value = value / 100.0 * 0.4;
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
    /// Tries to get an OKLCh Chroma value from a token.
    /// Can be a number (>= 0, typically 0-0.5) or percentage (0-100% maps to 0-0.4).
    /// Negative values are clamped to 0 at parse time per spec.
    /// </summary>
    private static bool TryGetOklchChroma(CssToken token, out double value)
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
            // Percentage: 0% = 0, 100% = 0.4
            value = value / 100.0 * 0.4;
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
    /// Converts OKLab color values to an sRGB CssColor.
    /// Algorithm per CSS Color Level 4 specification.
    /// OKLab uses D65 white point natively, so no chromatic adaptation is needed.
    /// </summary>
    /// <param name="lightness">L lightness (0-1)</param>
    /// <param name="a">a axis (typically -0.5 to +0.5)</param>
    /// <param name="b">b axis (typically -0.5 to +0.5)</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    /// <param name="color">The resulting sRGB color</param>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#oklab-to-predefined"/>
    private static bool OklabToRgb(double lightness, double a, double b, double alpha, out CssColor color)
    {
        // Step 1: Convert OKLab to D65-adapted XYZ
        var xyz = OklabToXyz(lightness, a, b);

        // Step 2: Convert D65 XYZ to linear sRGB (no chromatic adaptation needed - same white point)
        var linearRgb = XyzToLinearSrgb(xyz);

        // Step 3: Apply sRGB gamma encoding
        var srgb = LinearSrgbToSrgb(linearRgb);

        // Step 4: Clamp to [0, 1] and convert to bytes
        // Note: Out-of-gamut colors are allowed but must be gamut-mapped for display
        int red = (int)Math.Round(Math.Clamp(srgb[0], 0, 1) * 255);
        int green = (int)Math.Round(Math.Clamp(srgb[1], 0, 1) * 255);
        int blue = (int)Math.Round(Math.Clamp(srgb[2], 0, 1) * 255);
        int aVal = (int)Math.Round(Math.Clamp(alpha, 0, 1) * 255);

        color = new CssColor((byte)red, (byte)green, (byte)blue, (byte)aVal);
        return true;
    }

    /// <summary>
    /// Converts OKLCh color values to an sRGB CssColor.
    /// First converts OKLCh to OKLab, then OKLab to sRGB.
    /// </summary>
    /// <param name="lightness">L lightness (0-1)</param>
    /// <param name="chroma">C chroma (>= 0)</param>
    /// <param name="hue">H hue angle in degrees</param>
    /// <param name="alpha">Alpha value (0-1)</param>
    /// <param name="color">The resulting sRGB color</param>
    private static bool OklchToRgb(double lightness, double chroma, double hue, double alpha, out CssColor color)
    {
        // Convert OKLCh to OKLab
        // a = C * cos(H)
        // b = C * sin(H)
        hue = NormalizeHue(hue);
        double hueRad = hue * Math.PI / 180.0;
        double a = chroma * Math.Cos(hueRad);
        double b = chroma * Math.Sin(hueRad);

        // Now convert OKLab to sRGB
        return OklabToRgb(lightness, a, b, alpha, out color);
    }

    /// <summary>
    /// Converts OKLab to D65-adapted XYZ.
    /// Uses matrices from CSS Color 4 spec / color.js library.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#color-conversion-code"/>
    private static double[] OklabToXyz(double L, double a, double b)
    {
        // Matrix to convert OKLab to non-linear LMS (LMS^(1/3))
        // From W3C spec: https://www.w3.org/TR/css-color-4/#color-conversion-code
        double[,] OKLabToLMS = {
            { 1.0000000000000000,  0.3963377773761749,  0.2158037573099136 },
            { 1.0000000000000000, -0.1055613458156586, -0.0638541728258133 },
            { 1.0000000000000000, -0.0894841775298119, -1.2914855480194092 }
        };

        // Matrix to convert linear LMS to XYZ (D65)
        // From W3C spec: https://www.w3.org/TR/css-color-4/#color-conversion-code
        double[,] LMSToXYZ = {
            {  1.2268798758459243, -0.5578149944602171,  0.2813910456659647 },
            { -0.0405757452148008,  1.1122868032803170, -0.0717110580655164 },
            { -0.0763729366746601, -0.4214933324022432,  1.5869240198367816 }
        };

        // Step 1: Convert OKLab to non-linear LMS
        double[] oklab = { L, a, b };
        double[] lmsNonLinear = MultiplyMatrix3x3(OKLabToLMS, oklab);

        // Step 2: Cube to get linear LMS (undo the cube root)
        double[] lmsLinear = {
            lmsNonLinear[0] * lmsNonLinear[0] * lmsNonLinear[0],
            lmsNonLinear[1] * lmsNonLinear[1] * lmsNonLinear[1],
            lmsNonLinear[2] * lmsNonLinear[2] * lmsNonLinear[2]
        };

        // Step 3: Convert linear LMS to XYZ
        return MultiplyMatrix3x3(LMSToXYZ, lmsLinear);
    }

    #endregion

    #region color() Function Parsing

    /// <summary>
    /// Parses the CSS color() function for predefined color spaces.
    /// Syntax: color(&lt;color-space&gt; c1 c2 c3 [ / alpha ])
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-color-4/#color-function"/>
    private static bool TryParseColorFunction(DataConsumer<CssToken> stream, out CssColor color)
    {
        color = CssColor.Transparent;

        // First token must be an identifier for the color space
        var colorSpaceToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (colorSpaceToken is not IdentToken identToken)
        {
            return false;
        }

        // Parse the color space name
        if (!TryParseColorSpaceName(identToken.Value, out var colorSpace))
        {
            return false; // Unknown color space = invalid color
        }

        // Parse the three color components
        var c1Token = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (c1Token == null || !TryGetColorComponent(c1Token, colorSpace, 0, out double c1))
        {
            return false;
        }

        var c2Token = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (c2Token == null || !TryGetColorComponent(c2Token, colorSpace, 1, out double c2))
        {
            return false;
        }

        var c3Token = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (c3Token == null || !TryGetColorComponent(c3Token, colorSpace, 2, out double c3))
        {
            return false;
        }

        // Parse alpha if present (after the slash)
        double alpha = 1.0;
        var nextToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
        if (nextToken != null)
        {
            if (CssParsingHelpers.IsSlash(nextToken))
            {
                // Alpha value expected after slash
                var alphaToken = CssParsingHelpers.ConsumeNonWhitespace(stream);
                if (alphaToken == null || !TryGetAlphaValueNormalized(alphaToken, out alpha))
                {
                    return false;
                }
            }
            else
            {
                // Extra tokens after color components (not a slash) - invalid
                return false;
            }
        }

        // Verify no extra tokens remain
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Convert from the specified color space to sRGB
        return ConvertColorSpaceToSrgb(colorSpace, c1, c2, c3, alpha, out color);
    }

    /// <summary>
    /// Attempts to parse a color space name from a string.
    /// </summary>
    private static bool TryParseColorSpaceName(string name, out EColorSpace colorSpace)
    {
        colorSpace = EColorSpace.sRGB;

        // Case-insensitive matching per spec
        if (name.Equals("srgb", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.sRGB;
            return true;
        }

        if (name.Equals("srgb-linear", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.sRGBLinear;
            return true;
        }

        if (name.Equals("display-p3", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.DisplayP3;
            return true;
        }

        if (name.Equals("a98-rgb", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.A98Rgb;
            return true;
        }

        if (name.Equals("prophoto-rgb", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.ProPhotoRgb;
            return true;
        }

        if (name.Equals("rec2020", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.Rec2020;
            return true;
        }

        if (name.Equals("xyz", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("xyz-d65", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.XyzD65;
            return true;
        }

        if (name.Equals("xyz-d50", StringComparison.OrdinalIgnoreCase))
        {
            colorSpace = EColorSpace.XyzD50;
            return true;
        }

        // Unknown color space
        return false;
    }

    /// <summary>
    /// Attempts to get a color component value from a token for the color() function.
    /// RGB spaces: percentage maps to [0,1]; XYZ spaces: percentage maps to [0,1].
    /// </summary>
    private static bool TryGetColorComponent(CssToken token, EColorSpace colorSpace, int componentIndex, out double value)
    {
        value = 0;

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out value))
            {
                return false;
            }
            // No clamping - out of gamut values are allowed per spec
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out value))
            {
                return false;
            }
            // For all predefined RGB and XYZ color spaces: 0% = 0.0, 100% = 1.0
            value = value / 100.0;
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
    /// Converts a color from a predefined color space to sRGB.
    /// </summary>
    private static bool ConvertColorSpaceToSrgb(EColorSpace colorSpace, double c1, double c2, double c3, double alpha, out CssColor color)
    {
        double[] rgb;

        switch (colorSpace)
        {
            case EColorSpace.sRGB:
                // Already in sRGB, just clamp and convert
                rgb = new[] { c1, c2, c3 };
                break;

            case EColorSpace.sRGBLinear:
                // Apply sRGB gamma encoding
                rgb = LinearSrgbToSrgb(new[] { c1, c2, c3 });
                break;

            case EColorSpace.DisplayP3:
                rgb = DisplayP3ToSrgb(c1, c2, c3);
                break;

            case EColorSpace.A98Rgb:
                rgb = A98RgbToSrgb(c1, c2, c3);
                break;

            case EColorSpace.ProPhotoRgb:
                rgb = ProPhotoRgbToSrgb(c1, c2, c3);
                break;

            case EColorSpace.Rec2020:
                rgb = Rec2020ToSrgb(c1, c2, c3);
                break;

            case EColorSpace.XyzD65:
                rgb = XyzD65ToSrgb(c1, c2, c3);
                break;

            case EColorSpace.XyzD50:
                rgb = XyzD50ToSrgb(c1, c2, c3);
                break;

            default:
                color = CssColor.Transparent;
                return false;
        }

        // Clamp to [0, 1] and convert to bytes
        int red = (int)Math.Round(Math.Clamp(rgb[0], 0, 1) * 255);
        int green = (int)Math.Round(Math.Clamp(rgb[1], 0, 1) * 255);
        int blue = (int)Math.Round(Math.Clamp(rgb[2], 0, 1) * 255);
        int a = (int)Math.Round(Math.Clamp(alpha, 0, 1) * 255);

        color = new CssColor((byte)red, (byte)green, (byte)blue, (byte)a);
        return true;
    }

    #endregion

    #region Predefined Color Space Conversions

    /// <summary>
    /// Converts Display P3 to sRGB via XYZ D65.
    /// Display P3 uses the same transfer function as sRGB.
    /// </summary>
    private static double[] DisplayP3ToSrgb(double r, double g, double b)
    {
        // Step 1: Undo gamma to get linear P3
        double[] linear = {
            GammaToLinear(r),
            GammaToLinear(g),
            GammaToLinear(b)
        };

        // Step 2: Linear P3 to XYZ (D65)
        double[,] P3ToXYZ = {
            { 0.4865709486482162, 0.26566769316909306, 0.1982172852343625 },
            { 0.2289745640697488, 0.6917385218365064,  0.079286914093745   },
            { 0.0000000000000000, 0.04511338185890264, 1.043944368900976   }
        };
        var xyz = MultiplyMatrix3x3(P3ToXYZ, linear);

        // Step 3: XYZ to linear sRGB
        var linearSrgb = XyzToLinearSrgb(xyz);

        // Step 4: Apply sRGB gamma
        return LinearSrgbToSrgb(linearSrgb);
    }

    /// <summary>
    /// Converts A98 RGB to sRGB via XYZ D65.
    /// A98 RGB uses a gamma of 563/256 ≈ 2.2.
    /// </summary>
    private static double[] A98RgbToSrgb(double r, double g, double b)
    {
        // Step 1: Undo A98 gamma (563/256)
        double[] linear = {
            A98RgbGammaToLinear(r),
            A98RgbGammaToLinear(g),
            A98RgbGammaToLinear(b)
        };

        // Step 2: Linear A98 to XYZ (D65)
        double[,] A98ToXYZ = {
            { 0.5766690429101305,  0.1855582379065463,  0.1882286462349947 },
            { 0.29734497525053605, 0.6273635662554661,  0.07529145849399788 },
            { 0.02703136138641234, 0.07068885253582723, 0.9913375368376388 }
        };
        var xyz = MultiplyMatrix3x3(A98ToXYZ, linear);

        // Step 3: XYZ to linear sRGB
        var linearSrgb = XyzToLinearSrgb(xyz);

        // Step 4: Apply sRGB gamma
        return LinearSrgbToSrgb(linearSrgb);
    }

    /// <summary>
    /// Converts ProPhoto RGB to sRGB via XYZ.
    /// ProPhoto RGB uses D50 white point and gamma 1.8 with a linear portion.
    /// </summary>
    private static double[] ProPhotoRgbToSrgb(double r, double g, double b)
    {
        // Step 1: Undo ProPhoto gamma
        double[] linear = {
            ProPhotoGammaToLinear(r),
            ProPhotoGammaToLinear(g),
            ProPhotoGammaToLinear(b)
        };

        // Step 2: Linear ProPhoto to XYZ (D50)
        double[,] ProPhotoToXYZ = {
            { 0.7977666449006423,  0.13518129740053308, 0.0313477341283922  },
            { 0.2880748288194013,  0.7118352342418731,  0.00008993693872564 },
            { 0.0000000000000000,  0.0000000000000000,  0.8251046025104602  }
        };
        var xyzD50 = MultiplyMatrix3x3(ProPhotoToXYZ, linear);

        // Step 3: Chromatic adaptation from D50 to D65
        var xyzD65 = D50ToD65(xyzD50);

        // Step 4: XYZ to linear sRGB
        var linearSrgb = XyzToLinearSrgb(xyzD65);

        // Step 5: Apply sRGB gamma
        return LinearSrgbToSrgb(linearSrgb);
    }

    /// <summary>
    /// Converts Rec. 2020 to sRGB via XYZ D65.
    /// Rec. 2020 has its own transfer function defined in ITU-R BT.2020-2.
    /// </summary>
    private static double[] Rec2020ToSrgb(double r, double g, double b)
    {
        // Step 1: Undo Rec2020 gamma
        double[] linear = {
            Rec2020GammaToLinear(r),
            Rec2020GammaToLinear(g),
            Rec2020GammaToLinear(b)
        };

        // Step 2: Linear Rec2020 to XYZ (D65)
        double[,] Rec2020ToXYZ = {
            { 0.6369580483012914,  0.14461690358620832, 0.1688809751641721  },
            { 0.2627002120112671,  0.6779980715188708,  0.05930171646986196 },
            { 0.0000000000000000,  0.028072693049087428, 1.0609850577107909 }
        };
        var xyz = MultiplyMatrix3x3(Rec2020ToXYZ, linear);

        // Step 3: XYZ to linear sRGB
        var linearSrgb = XyzToLinearSrgb(xyz);

        // Step 4: Apply sRGB gamma
        return LinearSrgbToSrgb(linearSrgb);
    }

    /// <summary>
    /// Converts XYZ (D65) to sRGB.
    /// </summary>
    private static double[] XyzD65ToSrgb(double x, double y, double z)
    {
        // XYZ to linear sRGB
        var linearSrgb = XyzToLinearSrgb(new[] { x, y, z });

        // Apply sRGB gamma
        return LinearSrgbToSrgb(linearSrgb);
    }

    /// <summary>
    /// Converts XYZ (D50) to sRGB.
    /// </summary>
    private static double[] XyzD50ToSrgb(double x, double y, double z)
    {
        // Step 1: Chromatic adaptation from D50 to D65
        var xyzD65 = D50ToD65(new[] { x, y, z });

        // Step 2: XYZ to linear sRGB
        var linearSrgb = XyzToLinearSrgb(xyzD65);

        // Step 3: Apply sRGB gamma
        return LinearSrgbToSrgb(linearSrgb);
    }

    #endregion

    #region Transfer Functions

    /// <summary>
    /// Removes sRGB gamma encoding (gamma to linear).
    /// Extended transfer function handles negative values.
    /// </summary>
    private static double GammaToLinear(double c)
    {
        double sign = c < 0 ? -1.0 : 1.0;
        double abs = Math.Abs(c);

        if (abs <= 0.04045)
        {
            return c / 12.92;
        }

        return sign * Math.Pow((abs + 0.055) / 1.055, 2.4);
    }

    /// <summary>
    /// Applies A98 RGB gamma encoding.
    /// A98 uses a simple gamma of 256/563.
    /// </summary>
    private static double A98RgbGammaToLinear(double c)
    {
        double sign = c < 0 ? -1.0 : 1.0;
        double abs = Math.Abs(c);
        return sign * Math.Pow(abs, 563.0 / 256.0);
    }

    /// <summary>
    /// Removes ProPhoto RGB gamma encoding.
    /// ProPhoto uses gamma 1.8 with a small linear portion near black.
    /// </summary>
    private static double ProPhotoGammaToLinear(double c)
    {
        const double Et2 = 16.0 / 512.0;
        double sign = c < 0 ? -1.0 : 1.0;
        double abs = Math.Abs(c);

        if (abs <= Et2)
        {
            return c / 16.0;
        }

        return sign * Math.Pow(abs, 1.8);
    }

    /// <summary>
    /// Removes Rec. 2020 gamma encoding per ITU-R BT.2020-2.
    /// </summary>
    private static double Rec2020GammaToLinear(double c)
    {
        const double alpha = 1.09929682680944;
        const double beta = 0.018053968510807;

        double sign = c < 0 ? -1.0 : 1.0;
        double abs = Math.Abs(c);

        if (abs < beta * 4.5)
        {
            return c / 4.5;
        }

        return sign * Math.Pow((abs + alpha - 1) / alpha, 1.0 / 0.45);
    }

    #endregion

    #region Helper Methods

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetAlphaValue(CssToken token, out double value)
        => CssParsingHelpers.TryGetAlphaValue(token, out value);

    /// <summary>
    /// Extracts a numeric value from a NumberToken.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetNumber(CssToken token, out double value)
        => CssParsingHelpers.TryGetNumber(token, out value);

    /// <summary>
    /// Extracts a percentage value from a PercentageToken.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryGetPercentage(CssToken token, out double value)
        => CssParsingHelpers.TryGetPercentage(token, out value);

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
