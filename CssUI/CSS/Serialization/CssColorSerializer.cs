using System;
using System.Globalization;
using System.Text;

namespace CssUI.CSS.Serialization;

/// <summary>
/// Provides canonical serialization of CSS color values per CSS Color Level 4 specification.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/#serializing-color-values
///
/// Key serialization rules from the spec:
/// - sRGB colors serialize using legacy comma-separated rgb()/rgba() form for compatibility
/// - Alpha is omitted when exactly 1 (fully opaque)
/// - Non-opaque colors use rgba() form with explicit alpha
/// - HDR colors serialize in their original color space format
/// - Trailing zeroes are omitted from all numeric values
/// - "." is used as decimal separator regardless of locale
/// </remarks>
public static class CssColorSerializer
{
    #region Constants
    private const string Space = " ";
    private const string Comma = ", ";
    private const string SlashWithSpaces = " / ";
    #endregion

    #region sRGB Serialization

    /// <summary>
    /// Serializes a <see cref="CssColor"/> to its canonical CSS string representation.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.2.2:
    /// - Uses rgb() for opaque colors, rgba() for non-opaque
    /// - Legacy comma-separated syntax for compatibility
    /// - Component values as numbers in [0-255] range
    /// - Alpha omitted when 1, else serialized as a number
    /// </remarks>
    public static string Serialize(CssColor color)
    {
        // Per spec: alpha exactly 1 → rgb(), otherwise → rgba()
        if (color.A == 255)
        {
            return $"rgb({color.R}, {color.G}, {color.B})";
        }

        string alpha = SerializeAlpha8Bit(color.A);
        return $"rgba({color.R}, {color.G}, {color.B}, {alpha})";
    }

    /// <summary>
    /// Serializes a <see cref="CssColor"/> to its HTML-compatible hex representation.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.2.1:
    /// - Only valid when alpha is 1
    /// - Returns lowercase 6-digit hex: #rrggbb
    /// - For non-opaque colors, falls back to CSS serialization
    /// </remarks>
    public static string SerializeHtmlCompatible(CssColor color)
    {
        // HTML-compatible serialization only works for fully opaque colors
        if (color.A != 255)
        {
            return Serialize(color);
        }

        return $"#{color.R:x2}{color.G:x2}{color.B:x2}";
    }

    /// <summary>
    /// Serializes the computed/used value of the transparent keyword.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.2:
    /// - Declared value of 'transparent' is "transparent"
    /// - Computed/used value is "rgba(0, 0, 0, 0)"
    /// </remarks>
    public static string SerializeTransparentComputed()
    {
        return "rgba(0, 0, 0, 0)";
    }

    /// <summary>
    /// Serializes the declared value of the transparent keyword.
    /// </summary>
    public static string SerializeTransparentDeclared()
    {
        return "transparent";
    }

    /// <summary>
    /// Serializes the currentColor keyword.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.6:
    /// - Serialized form is "currentcolor" (all lowercase)
    /// </remarks>
    public static string SerializeCurrentColor()
    {
        return "currentcolor";
    }

    #endregion

    #region HDR Color Serialization

    /// <summary>
    /// Serializes a <see cref="CssColorHdr"/> to its canonical CSS string representation.
    /// </summary>
    /// <remarks>
    /// HDR colors serialize in their original color space:
    /// - lab(), lch() for Lab/LCH color spaces
    /// - oklab(), oklch() for OkLab/OkLCH color spaces
    /// - color() for all other predefined color spaces
    ///
    /// Alpha is omitted when 1, else appended with " / ".
    /// </remarks>
    public static string Serialize(CssColorHdr color)
    {
        return color.ColorSpace switch
        {
            EColorSpace.Lab => SerializeLab(color),
            EColorSpace.Lch => SerializeLch(color),
            EColorSpace.OkLab => SerializeOkLab(color),
            EColorSpace.OkLCh => SerializeOkLCh(color),
            _ => SerializeColorFunction(color)
        };
    }

    /// <summary>
    /// Serializes a color in the Lab color space.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.3:
    /// - Format: lab(L a b) or lab(L a b / alpha)
    /// - L is in [0-100] range
    /// - a, b are typically in [-125, 125] range
    /// - Space-separated values
    /// </remarks>
    private static string SerializeLab(CssColorHdr color)
    {
        var sb = new StringBuilder(32);
        sb.Append("lab(");
        sb.Append(FormatNumber(color.C1));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C2));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C3));

        if (!IsAlphaOne(color.Alpha))
        {
            sb.Append(SlashWithSpaces);
            sb.Append(FormatAlpha(color.Alpha));
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Serializes a color in the LCH color space.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.3:
    /// - Format: lch(L C H) or lch(L C H / alpha)
    /// - L is in [0-100] range
    /// - C (chroma) is >= 0
    /// - H (hue) is in [0-360] range
    /// </remarks>
    private static string SerializeLch(CssColorHdr color)
    {
        var sb = new StringBuilder(32);
        sb.Append("lch(");
        sb.Append(FormatNumber(color.C1));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C2));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C3));

        if (!IsAlphaOne(color.Alpha))
        {
            sb.Append(SlashWithSpaces);
            sb.Append(FormatAlpha(color.Alpha));
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Serializes a color in the OkLab color space.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.4:
    /// - Format: oklab(L a b) or oklab(L a b / alpha)
    /// - L is in [0-1] range (percentage maps to 0-1)
    /// - a, b are typically in [-0.4, 0.4] range
    /// </remarks>
    private static string SerializeOkLab(CssColorHdr color)
    {
        var sb = new StringBuilder(32);
        sb.Append("oklab(");
        sb.Append(FormatNumber(color.C1, 5));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C2, 5));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C3, 5));

        if (!IsAlphaOne(color.Alpha))
        {
            sb.Append(SlashWithSpaces);
            sb.Append(FormatAlpha(color.Alpha));
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Serializes a color in the OkLCH color space.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.4:
    /// - Format: oklch(L C H) or oklch(L C H / alpha)
    /// - L is in [0-1] range
    /// - C (chroma) is >= 0
    /// - H (hue) is in [0-360] range
    /// </remarks>
    private static string SerializeOkLCh(CssColorHdr color)
    {
        var sb = new StringBuilder(32);
        sb.Append("oklch(");
        sb.Append(FormatNumber(color.C1, 5));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C2, 5));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C3, 2));

        if (!IsAlphaOne(color.Alpha))
        {
            sb.Append(SlashWithSpaces);
            sb.Append(FormatAlpha(color.Alpha));
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Serializes a color using the color() function for predefined RGB color spaces.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.5:
    /// - Format: color(space c1 c2 c3) or color(space c1 c2 c3 / alpha)
    /// - Color space name is always explicit
    /// - Components as numbers (not percentages)
    /// </remarks>
    private static string SerializeColorFunction(CssColorHdr color)
    {
        string colorSpaceName = Lookup.Keyword(color.ColorSpace);
        int precision = GetColorSpacePrecision(color.ColorSpace);

        var sb = new StringBuilder(48);
        sb.Append("color(");
        sb.Append(colorSpaceName);
        sb.Append(Space);
        sb.Append(FormatNumber(color.C1, precision));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C2, precision));
        sb.Append(Space);
        sb.Append(FormatNumber(color.C3, precision));

        if (!IsAlphaOne(color.Alpha))
        {
            sb.Append(SlashWithSpaces);
            sb.Append(FormatAlpha(color.Alpha));
        }

        sb.Append(')');
        return sb.ToString();
    }

    /// <summary>
    /// Gets the recommended decimal precision for a color space.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.5, minimum precision for round-tripping:
    /// - srgb, display-p3, a98-rgb: 10 bits → 3 decimal places
    /// - srgb-linear, prophoto-rgb, rec2020: 12 bits → 4 decimal places
    /// - xyz, xyz-d50, xyz-d65: 16 bits → 5 decimal places
    /// </remarks>
    private static int GetColorSpacePrecision(EColorSpace colorSpace)
    {
        return colorSpace switch
        {
            EColorSpace.sRGB => 3,
            EColorSpace.DisplayP3 => 3,
            EColorSpace.A98Rgb => 3,
            EColorSpace.sRGBLinear => 4,
            EColorSpace.ProPhotoRgb => 4,
            EColorSpace.Rec2020 => 4,
            EColorSpace.XyzD50 => 5,
            EColorSpace.XyzD65 => 5,
            _ => 4
        };
    }

    #endregion

    #region Number Formatting

    /// <summary>
    /// Formats a number for CSS serialization with trailing zeroes removed.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15:
    /// - "." is used as decimal separator regardless of locale
    /// - Trailing zeroes must be omitted
    /// - If fractional part is all zeroes, decimal point is omitted
    /// </remarks>
    private static string FormatNumber(float value, int maxDecimals = 3)
    {
        // Use round-towards-infinity per spec
        string format = "0." + new string('#', maxDecimals);
        return value.ToString(format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Checks if alpha is effectively 1 (fully opaque).
    /// </summary>
    private static bool IsAlphaOne(float alpha)
    {
        return MathF.Abs(alpha - 1f) < 1e-6f;
    }

    /// <summary>
    /// Formats an alpha value for CSS serialization.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.1:
    /// - Serialized as a number, not a percentage
    /// - Leading zero must not be omitted
    /// - Trailing zeroes must be omitted
    /// - At least 2 decimal places for round-tripping
    /// </remarks>
    private static string FormatAlpha(float alpha)
    {
        // Clamp to [0, 1] range
        alpha = Math.Clamp(alpha, 0f, 1f);
        return FormatNumber(alpha, 3);
    }

    /// <summary>
    /// Serializes an 8-bit alpha value following the CSS Color 4 algorithm.
    /// </summary>
    /// <remarks>
    /// Per CSS Color 4 §15.1:
    /// 1. If there exists an integer 0-100 that when multiplied by 2.55 and rounded equals alpha,
    ///    return that integer / 100
    /// 2. Otherwise, return alpha / 0.255 rounded to nearest integer / 1000
    /// </remarks>
    private static string SerializeAlpha8Bit(byte alpha)
    {
        if (alpha == 255)
        {
            return "1";
        }

        if (alpha == 0)
        {
            return "0";
        }

        // Try to find an integer percentage that rounds to this alpha value
        for (int i = 1; i < 100; i++)
        {
            int rounded = (int)Math.Round(i * 2.55);
            if (rounded == alpha)
            {
                // Found a clean percentage representation
                float result = i / 100f;
                return FormatNumber(result, 2);
            }
        }

        // No clean percentage, use the fallback algorithm
        float value = alpha / 255f;
        return FormatNumber(value, 5);
    }

    #endregion
}
