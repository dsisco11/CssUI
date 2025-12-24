using System;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parser for CSS hex color values (#RGB, #RGBA, #RRGGBB, #RRGGBBAA).
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/#hex-notation
/// </remarks>
public static class CssHexColorParser
{
    /// <summary>
    /// Attempts to parse a hexadecimal color string.
    /// </summary>
    /// <param name="hex">Hex string in format: RGB, RGBA, RRGGBB, or RRGGBBAA (without leading #)</param>
    /// <param name="color">The parsed color on success.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParse(ReadOnlySpan<char> hex, out CssColor color)
    {
        color = CssColor.Transparent;

        if (hex.IsEmpty)
            return false;

        // Skip leading '#' if present
        if (hex[0] == '#')
            hex = hex[1..];

        return hex.Length switch
        {
            3 => TryParseHex3(hex, out color),    // #RGB
            4 => TryParseHex4(hex, out color),    // #RGBA
            6 => TryParseHex6(hex, out color),    // #RRGGBB
            8 => TryParseHex8(hex, out color),    // #RRGGBBAA
            _ => false
        };
    }

    /// <summary>
    /// Parses a hexadecimal color string.
    /// </summary>
    /// <param name="hex">Hex string in format: RGB, RGBA, RRGGBB, or RRGGBBAA (without leading #)</param>
    /// <returns>The parsed color.</returns>
    /// <exception cref="FormatException">Thrown when the hex string is invalid.</exception>
    public static CssColor Parse(ReadOnlySpan<char> hex)
    {
        if (!TryParse(hex, out CssColor color))
        {
            throw new FormatException($"Invalid hex color format: {hex.ToString()}");
        }

        return color;
    }

    #region Private Parsing Methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHex3(ReadOnlySpan<char> hex, out CssColor color)
    {
        if (TryParseHexNibble(hex[0], out byte r) &&
            TryParseHexNibble(hex[1], out byte g) &&
            TryParseHexNibble(hex[2], out byte b))
        {
            color = new CssColor((byte)(r | (r << 4)), (byte)(g | (g << 4)), (byte)(b | (b << 4)), 255);
            return true;
        }

        color = CssColor.Transparent;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHex4(ReadOnlySpan<char> hex, out CssColor color)
    {
        if (TryParseHexNibble(hex[0], out byte r) &&
            TryParseHexNibble(hex[1], out byte g) &&
            TryParseHexNibble(hex[2], out byte b) &&
            TryParseHexNibble(hex[3], out byte a))
        {
            color = new CssColor((byte)(r | (r << 4)), (byte)(g | (g << 4)), (byte)(b | (b << 4)), (byte)(a | (a << 4)));
            return true;
        }

        color = CssColor.Transparent;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHex6(ReadOnlySpan<char> hex, out CssColor color)
    {
        if (TryParseHexByte(hex[0], hex[1], out byte r) &&
            TryParseHexByte(hex[2], hex[3], out byte g) &&
            TryParseHexByte(hex[4], hex[5], out byte b))
        {
            color = new CssColor(r, g, b, 255);
            return true;
        }

        color = CssColor.Transparent;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHex8(ReadOnlySpan<char> hex, out CssColor color)
    {
        if (TryParseHexByte(hex[0], hex[1], out byte r) &&
            TryParseHexByte(hex[2], hex[3], out byte g) &&
            TryParseHexByte(hex[4], hex[5], out byte b) &&
            TryParseHexByte(hex[6], hex[7], out byte a))
        {
            color = new CssColor(r, g, b, a);
            return true;
        }

        color = CssColor.Transparent;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHexNibble(char c, out byte value)
    {
        value = c switch
        {
            >= '0' and <= '9' => (byte)(c - '0'),
            >= 'a' and <= 'f' => (byte)(c - 'a' + 10),
            >= 'A' and <= 'F' => (byte)(c - 'A' + 10),
            _ => 0
        };

        return c switch
        {
            >= '0' and <= '9' => true,
            >= 'a' and <= 'f' => true,
            >= 'A' and <= 'F' => true,
            _ => false
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryParseHexByte(char high, char low, out byte value)
    {
        if (TryParseHexNibble(high, out byte h) && TryParseHexNibble(low, out byte l))
        {
            value = (byte)((h << 4) | l);
            return true;
        }

        value = 0;
        return false;
    }
    #endregion
}
