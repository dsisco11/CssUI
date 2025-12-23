using System;
using System.Runtime.CompilerServices;

namespace CssUI.CSS;

/// <summary>
/// Provides color space conversion utilities for CSS color values.
/// </summary>
/// <remarks>
/// Docs: https://www.w3.org/TR/css-color-4/#color-conversion
/// </remarks>
public static class ColorConversion
{
    #region Constants
    private const float Epsilon = 1e-6f;

    // sRGB gamma curve constants
    private const float SrgbGammaThreshold = 0.04045f;
    private const float SrgbGammaLinearThreshold = 0.0031308f;
    private const float SrgbGammaFactor = 12.92f;
    private const float SrgbGammaExponent = 2.4f;
    private const float SrgbGammaOffset = 0.055f;
    #endregion

    #region sRGB Gamma Conversion
    /// <summary>
    /// Converts a single sRGB gamma-encoded value to linear light.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GammaToLinear(float value)
    {
        // sRGB transfer function (gamma to linear)
        // Docs: https://www.w3.org/TR/css-color-4/#color-conversion
        return value <= SrgbGammaThreshold
            ? value / SrgbGammaFactor
            : MathF.Pow((value + SrgbGammaOffset) / (1f + SrgbGammaOffset), SrgbGammaExponent);
    }

    /// <summary>
    /// Converts a single linear light value to sRGB gamma-encoded.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float LinearToGamma(float value)
    {
        // sRGB inverse transfer function (linear to gamma)
        return value <= SrgbGammaLinearThreshold
            ? value * SrgbGammaFactor
            : (1f + SrgbGammaOffset) * MathF.Pow(value, 1f / SrgbGammaExponent) - SrgbGammaOffset;
    }

    /// <summary>
    /// Converts sRGB gamma-encoded values to linear light.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float R, float G, float B) SrgbToLinear(float r, float g, float b)
    {
        return (GammaToLinear(r), GammaToLinear(g), GammaToLinear(b));
    }

    /// <summary>
    /// Converts linear light values to sRGB gamma-encoded.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float R, float G, float B) LinearToSrgb(float r, float g, float b)
    {
        return (LinearToGamma(r), LinearToGamma(g), LinearToGamma(b));
    }
    #endregion

    #region OkLab Conversion
    // OkLab conversion matrices (from Björn Ottosson's original paper)
    // https://bottosson.github.io/posts/oklab/

    /// <summary>
    /// Converts linear sRGB to OkLab color space.
    /// </summary>
    public static CssColorHdr LinearSrgbToOkLab(float r, float g, float b, float alpha)
    {
        // Linear sRGB to LMS (cone response)
        float l = 0.4122214708f * r + 0.5363325363f * g + 0.0514459929f * b;
        float m = 0.2119034982f * r + 0.6806995451f * g + 0.1073969566f * b;
        float s = 0.0883024619f * r + 0.2817188376f * g + 0.6299787005f * b;

        // Apply cube root (approximate perceptual response)
        float l_ = MathF.Cbrt(l);
        float m_ = MathF.Cbrt(m);
        float s_ = MathF.Cbrt(s);

        // LMS' to OkLab
        float L = 0.2104542553f * l_ + 0.7936177850f * m_ - 0.0040720468f * s_;
        float A = 1.9779984951f * l_ - 2.4285922050f * m_ + 0.4505937099f * s_;
        float B = 0.0259040371f * l_ + 0.7827717662f * m_ - 0.8086757660f * s_;

        return new CssColorHdr(L, A, B, alpha, EColorSpace.OkLab);
    }

    /// <summary>
    /// Converts OkLab to linear sRGB color space.
    /// </summary>
    public static CssColorHdr OkLabToLinearSrgb(float L, float a, float b, float alpha)
    {
        // OkLab to LMS'
        float l_ = L + 0.3963377774f * a + 0.2158037573f * b;
        float m_ = L - 0.1055613458f * a - 0.0638541728f * b;
        float s_ = L - 0.0894841775f * a - 1.2914855480f * b;

        // Cube to get LMS
        float l = l_ * l_ * l_;
        float m = m_ * m_ * m_;
        float s = s_ * s_ * s_;

        // LMS to linear sRGB
        float r = +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s;
        float g = -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s;
        float bl = -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s;

        return new CssColorHdr(r, g, bl, alpha, EColorSpace.sRGBLinear);
    }

    /// <summary>
    /// Converts OkLab to sRGB (gamma-encoded) color space.
    /// </summary>
    public static CssColorHdr OkLabToSrgb(float L, float a, float b, float alpha)
    {
        var linear = OkLabToLinearSrgb(L, a, b, alpha);
        return new CssColorHdr(
            LinearToGamma(linear.C1),
            LinearToGamma(linear.C2),
            LinearToGamma(linear.C3),
            alpha,
            EColorSpace.sRGB
        );
    }

    /// <summary>
    /// Converts sRGB (gamma-encoded) to OkLab color space.
    /// </summary>
    public static CssColorHdr SrgbToOkLab(float r, float g, float b, float alpha)
    {
        return LinearSrgbToOkLab(GammaToLinear(r), GammaToLinear(g), GammaToLinear(b), alpha);
    }
    #endregion

    #region OkLCH Conversion
    /// <summary>
    /// Converts OkLab to OkLCH (cylindrical representation).
    /// </summary>
    public static CssColorHdr OkLabToOkLCh(float L, float a, float b, float alpha)
    {
        float c = MathF.Sqrt(a * a + b * b);
        float h = MathF.Atan2(b, a) * (180f / MathF.PI);

        // Normalize hue to [0, 360)
        if (h < 0f) h += 360f;

        return new CssColorHdr(L, c, h, alpha, EColorSpace.OkLCh);
    }

    /// <summary>
    /// Converts OkLCH to OkLab.
    /// </summary>
    public static CssColorHdr OkLChToOkLab(float L, float c, float h, float alpha)
    {
        float hRad = h * (MathF.PI / 180f);
        float a = c * MathF.Cos(hRad);
        float b = c * MathF.Sin(hRad);

        return new CssColorHdr(L, a, b, alpha, EColorSpace.OkLab);
    }

    /// <summary>
    /// Converts OkLCH to sRGB (gamma-encoded) color space.
    /// </summary>
    public static CssColorHdr OkLChToSrgb(float L, float c, float h, float alpha)
    {
        var oklab = OkLChToOkLab(L, c, h, alpha);
        return OkLabToSrgb(oklab.C1, oklab.C2, oklab.C3, alpha);
    }

    /// <summary>
    /// Converts sRGB (gamma-encoded) to OkLCH color space.
    /// </summary>
    public static CssColorHdr SrgbToOkLCh(float r, float g, float b, float alpha)
    {
        var oklab = SrgbToOkLab(r, g, b, alpha);
        return OkLabToOkLCh(oklab.C1, oklab.C2, oklab.C3, alpha);
    }
    #endregion

    #region Gamut Mapping
    /// <summary>
    /// Performs gamut mapping to bring out-of-gamut colors into sRGB.
    /// Uses a simple chroma reduction in OkLCH space.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/css-color-4/#css-gamut-mapping
    /// </remarks>
    public static CssColorHdr GamutMapToSrgb(CssColorHdr color)
    {
        // First convert to OkLCH for perceptual gamut mapping
        var oklch = color.ToOkLCh();

        // Check if already in gamut
        var srgb = OkLChToSrgb(oklch.C1, oklch.C2, oklch.C3, oklch.Alpha);
        if (IsInSrgbGamut(srgb.C1, srgb.C2, srgb.C3))
        {
            return srgb;
        }

        // Binary search to find maximum in-gamut chroma
        float minChroma = 0f;
        float maxChroma = oklch.C2;
        float L = oklch.C1;
        float h = oklch.C3;
        float alpha = oklch.Alpha;

        const int MaxIterations = 16;
        const float ChromaTolerance = 0.0001f;

        for (int i = 0; i < MaxIterations && (maxChroma - minChroma) > ChromaTolerance; i++)
        {
            float midChroma = (minChroma + maxChroma) * 0.5f;
            var testSrgb = OkLChToSrgb(L, midChroma, h, alpha);

            if (IsInSrgbGamut(testSrgb.C1, testSrgb.C2, testSrgb.C3))
            {
                minChroma = midChroma;
            }
            else
            {
                maxChroma = midChroma;
            }
        }

        return OkLChToSrgb(L, minChroma, h, alpha);
    }

    /// <summary>
    /// Returns true if the given sRGB values are within gamut.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInSrgbGamut(float r, float g, float b)
    {
        return r >= -Epsilon && r <= 1f + Epsilon &&
               g >= -Epsilon && g <= 1f + Epsilon &&
               b >= -Epsilon && b <= 1f + Epsilon;
    }

    /// <summary>
    /// Clamps sRGB values to the valid range [0, 1].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (float R, float G, float B) ClampSrgb(float r, float g, float b)
    {
        return (Math.Clamp(r, 0f, 1f), Math.Clamp(g, 0f, 1f), Math.Clamp(b, 0f, 1f));
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Calculates the relative luminance of an sRGB color.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/WCAG21/#dfn-relative-luminance
    /// </remarks>
    public static float RelativeLuminance(float r, float g, float b)
    {
        // Convert to linear sRGB first
        float rLin = GammaToLinear(r);
        float gLin = GammaToLinear(g);
        float bLin = GammaToLinear(b);

        // Calculate luminance
        return 0.2126f * rLin + 0.7152f * gLin + 0.0722f * bLin;
    }

    /// <summary>
    /// Calculates the contrast ratio between two relative luminance values.
    /// </summary>
    /// <remarks>
    /// Docs: https://www.w3.org/TR/WCAG21/#dfn-contrast-ratio
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ContrastRatio(float luminance1, float luminance2)
    {
        float lighter = Math.Max(luminance1, luminance2);
        float darker = Math.Min(luminance1, luminance2);
        return (lighter + 0.05f) / (darker + 0.05f);
    }

    /// <summary>
    /// Calculates the WCAG contrast ratio between two sRGB colors.
    /// </summary>
    public static float ContrastRatio(CssColor color1, CssColor color2)
    {
        var (r1, g1, b1, _) = color1.ToFloats();
        var (r2, g2, b2, _) = color2.ToFloats();

        float lum1 = RelativeLuminance(r1, g1, b1);
        float lum2 = RelativeLuminance(r2, g2, b2);

        return ContrastRatio(lum1, lum2);
    }
    #endregion
}
