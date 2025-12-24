using CssUI.CSS;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Serialization;

/// <summary>
/// Tests for <see cref="CssColorSerializer"/> per CSS Color Level 4 specification.
/// </summary>
/// <remarks>
/// Spec reference: https://www.w3.org/TR/css-color-4/#serializing-color-values
/// </remarks>
public class CssColorSerializerTests
{
    #region sRGB Serialization Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OpaqueBlack_ReturnsRgb()
    {
        var color = CssColor.FromRgba(0, 0, 0, 255);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("rgb(0, 0, 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OpaqueWhite_ReturnsRgb()
    {
        var color = CssColor.FromRgba(255, 255, 255, 255);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("rgb(255, 255, 255)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OpaqueRed_ReturnsRgb()
    {
        var color = CssColor.FromRgba(255, 0, 0, 255);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("rgb(255, 0, 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_SemiTransparent_ReturnsRgba()
    {
        // 50% opacity = 127 or 128 alpha
        var color = CssColor.FromRgba(255, 0, 0, 128);
        var result = CssColorSerializer.Serialize(color);
        // Alpha 128/255 ≈ 0.502, nearest clean % is 50% which maps to 127.5 → 128
        Assert.StartsWith("rgba(255, 0, 0, ", result);
        Assert.EndsWith(")", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_FullyTransparent_ReturnsRgbaWithZeroAlpha()
    {
        var color = CssColor.FromRgba(0, 0, 0, 0);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("rgba(0, 0, 0, 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_Alpha93Percent_ReturnsExpectedAlpha()
    {
        // Per spec example: alpha 237 (93 * 2.55 = 237.15 → 237) serializes as "0.93"
        var color = CssColor.FromRgba(255, 0, 255, 237);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("rgba(255, 0, 255, 0.93)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_Alpha70Percent_ReturnsExpectedAlpha()
    {
        // 70% * 2.55 = 178.5 → 179 → 179/255 = 0.70196...
        var color = CssColor.FromRgba(0, 128, 255, 179);
        var result = CssColorSerializer.Serialize(color);
        // Alpha is serialized with full precision for round-trip accuracy
        Assert.StartsWith("rgba(0, 128, 255, 0.7", result);
        Assert.Contains(")", result);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [InlineData(0, "0")]
    [InlineData(255, "1")]
    [InlineData(128, "0.5")] // 50% maps to 127.5 → 128 → 128/255 = 0.50196...
    [InlineData(51, "0.2")]  // 20% maps to 51 → 51/255 = 0.2
    [InlineData(204, "0.8")] // 80% maps to 204 → 204/255 = 0.8
    public void Serialize_AlphaPercentages_RoundTripsCorrectly(byte alpha, string expectedAlpha)
    {
        var color = CssColor.FromRgba(100, 100, 100, alpha);
        var result = CssColorSerializer.Serialize(color);

        if (alpha == 255)
        {
            Assert.Equal("rgb(100, 100, 100)", result);
        }
        else
        {
            // Alpha starts with expected value (may have more precision digits)
            Assert.StartsWith($"rgba(100, 100, 100, {expectedAlpha}", result);
        }
    }

    #endregion

    #region HTML-Compatible Serialization Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeHtmlCompatible_OpaqueColor_ReturnsHex()
    {
        var color = CssColor.FromRgba(255, 0, 255, 255);
        var result = CssColorSerializer.SerializeHtmlCompatible(color);
        Assert.Equal("#ff00ff", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeHtmlCompatible_TransparentColor_FallsBackToRgba()
    {
        var color = CssColor.FromRgba(255, 0, 255, 128);
        var result = CssColorSerializer.SerializeHtmlCompatible(color);
        // Non-opaque colors cannot use hex format
        Assert.StartsWith("rgba(", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeHtmlCompatible_Black_ReturnsLowercaseHex()
    {
        var color = CssColor.Black;
        var result = CssColorSerializer.SerializeHtmlCompatible(color);
        Assert.Equal("#000000", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeHtmlCompatible_White_ReturnsLowercaseHex()
    {
        var color = CssColor.White;
        var result = CssColorSerializer.SerializeHtmlCompatible(color);
        Assert.Equal("#ffffff", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeHtmlCompatible_MixedCase_ReturnsLowercaseHex()
    {
        // Hex digits should always be lowercase per spec
        var color = CssColor.FromRgba(0xAB, 0xCD, 0xEF, 255);
        var result = CssColorSerializer.SerializeHtmlCompatible(color);
        Assert.Equal("#abcdef", result);
    }

    #endregion

    #region Special Keywords Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeTransparentDeclared_ReturnsTransparent()
    {
        var result = CssColorSerializer.SerializeTransparentDeclared();
        Assert.Equal("transparent", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeTransparentComputed_ReturnsRgbaZero()
    {
        var result = CssColorSerializer.SerializeTransparentComputed();
        Assert.Equal("rgba(0, 0, 0, 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void SerializeCurrentColor_ReturnsLowercase()
    {
        var result = CssColorSerializer.SerializeCurrentColor();
        Assert.Equal("currentcolor", result);
    }

    #endregion

    #region HDR Color Serialization - Lab/LCH

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_LabColor_OpaqueReturnsLabFormat()
    {
        var color = new CssColorHdr(56.2f, 0f, 83.6f, 1f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("lab(56.2 0 83.6)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_LabColor_WithAlphaIncludesSlash()
    {
        var color = new CssColorHdr(56.2f, 0f, 83.6f, 0.5f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("lab(56.2 0 83.6 / 0.5)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_LchColor_OpaqueReturnsLchFormat()
    {
        var color = new CssColorHdr(37f, 105f, 305f, 1f, EColorSpace.Lch);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("lch(37 105 305)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_LchColor_WithAlphaIncludesSlash()
    {
        var color = new CssColorHdr(56.2f, 83.6f, 357.4f, 0.93f, EColorSpace.Lch);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("lch(56.2 83.6 357.4 / 0.93)", result);
    }

    #endregion

    #region HDR Color Serialization - OkLab/OkLCH

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OkLabColor_OpaqueReturnsOklabFormat()
    {
        var color = new CssColorHdr(0.5f, 0.1f, -0.1f, 1f, EColorSpace.OkLab);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("oklab(0.5 0.1 -0.1)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OkLabColor_WithAlphaIncludesSlash()
    {
        var color = new CssColorHdr(0.5385f, 0.1725f, -0.1f, 0.7f, EColorSpace.OkLab);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("oklab(0.5385 0.1725 -0.1 / 0.7)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OkLChColor_OpaqueReturnsOklchFormat()
    {
        // Fully opaque HDR colors omit the alpha channel per W3C convention
        var color = new CssColorHdr(0.5385f, 0.1725f, 320.67f, 1f, EColorSpace.OkLCh);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("oklch(0.5385 0.1725 320.67)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OkLChColor_WithAlphaIncludesSlash()
    {
        var color = new CssColorHdr(0.5385f, 0.1725f, 320.67f, 0.7f, EColorSpace.OkLCh);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("oklch(0.5385 0.1725 320.67 / 0.7)", result);
    }

    #endregion

    #region HDR Color Serialization - color() Function

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_SrgbColor_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(1f, 0f, 0f, 1f, EColorSpace.sRGB);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(srgb 1 0 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_SrgbLinearColor_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(0.5f, 0.5f, 0.5f, 1f, EColorSpace.sRGBLinear);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(srgb-linear 0.5 0.5 0.5)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_DisplayP3Color_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(0.96f, 0.76f, 0.79f, 1f, EColorSpace.DisplayP3);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(display-p3 0.96 0.76 0.79)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_A98RgbColor_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(1f, 0.5f, 0.25f, 1f, EColorSpace.A98Rgb);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(a98-rgb 1 0.5 0.25)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_ProPhotoRgbColor_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(0.2804f, 0.4028f, 0.4226f, 1f, EColorSpace.ProPhotoRgb);
        var result = CssColorSerializer.Serialize(color);
        // 4 decimal places for prophoto-rgb
        Assert.StartsWith("color(prophoto-rgb 0.28", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_Rec2020Color_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(0.4f, 0.66f, 0.34f, 1f, EColorSpace.Rec2020);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(rec2020 0.4 0.66 0.34)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_XyzD50Color_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(0.5f, 0.5f, 0.5f, 1f, EColorSpace.XyzD50);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(xyz-d50 0.5 0.5 0.5)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_XyzD65Color_ReturnsColorFunctionFormat()
    {
        var color = new CssColorHdr(0.5f, 0.5f, 0.5f, 1f, EColorSpace.XyzD65);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("color(xyz-d65 0.5 0.5 0.5)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_ColorFunctionWithAlpha_IncludesSlash()
    {
        var color = new CssColorHdr(0.2804f, 0.4028f, 0.4226f, 0.85f, EColorSpace.ProPhotoRgb);
        var result = CssColorSerializer.Serialize(color);
        Assert.Contains(" / 0.85)", result);
    }

    #endregion

    #region Trailing Zero Removal Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_IntegerValues_OmitsDecimalPoint()
    {
        var color = new CssColorHdr(1f, 0f, 0f, 1f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);
        // Should be "lab(1 0 0)" not "lab(1.0 0.0 0.0)"
        Assert.Equal("lab(1 0 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_TrailingZeros_AreOmitted()
    {
        var color = new CssColorHdr(56.200f, 0.000f, 83.600f, 1f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);
        // Per spec example: "lab(56.2 0 83.6)" not "lab(56.200 0.000 83.600)"
        Assert.Equal("lab(56.2 0 83.6)", result);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_GoldenrodColor_SerializesAsInteger()
    {
        // Per spec example: goldenrod serializes as "rgb(218, 165, 32)"
        var color = CssColor.FromRgba(218, 165, 32, 255);
        var result = CssColorSerializer.Serialize(color);
        Assert.Equal("rgb(218, 165, 32)", result);
    }

    #endregion
}
