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

        // @todo: Add hsl, hsla, hwb, lab, lch, oklab, oklch, color() parsing

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
