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

    #region Round-Trip Parsing/Serialization Tests

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("rgb(255, 0, 0)")]
    [InlineData("rgb(0, 255, 0)")]
    [InlineData("rgb(0, 0, 255)")]
    [InlineData("rgb(128, 128, 128)")]
    [InlineData("rgb(0, 0, 0)")]
    [InlineData("rgb(255, 255, 255)")]
    public void RoundTrip_OpaqueRgb_ParseSerializeParse_Equals(string input)
    {
        // Parse the input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.Equal(color1.A, color2.A);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("rgba(255, 0, 0, 0.5)")]
    [InlineData("rgba(0, 255, 0, 0.25)")]
    [InlineData("rgba(0, 0, 255, 0.75)")]
    [InlineData("rgba(128, 128, 128, 0)")]
    public void RoundTrip_TransparentRgba_ParseSerializeParse_Equals(string input)
    {
        // Parse the input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal (within rounding tolerance for alpha)
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        // Alpha might differ slightly due to 8-bit → float → 8-bit conversion
        Assert.InRange(System.Math.Abs(color1.A - color2.A), 0, 1);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("rgb(255 128 0)")]          // Modern syntax
    [InlineData("rgb(100% 50% 0%)")]        // Percentage syntax
    [InlineData("rgb(255 128 0 / 0.5)")]    // Modern with alpha
    [InlineData("rgb(100% 50% 0% / 50%)")]  // Percentage with percentage alpha
    public void RoundTrip_ModernRgbSyntax_ParseSerializeParse_PreservesColor(string input)
    {
        // Parse the input (modern syntax)
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize (uses legacy syntax per spec)
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.InRange(System.Math.Abs(color1.A - color2.A), 0, 1);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("hsl(0, 100%, 50%)")]     // Red
    [InlineData("hsl(120, 100%, 50%)")]   // Green
    [InlineData("hsl(240, 100%, 50%)")]   // Blue
    [InlineData("hsl(60, 100%, 50%)")]    // Yellow
    [InlineData("hsl(180, 100%, 50%)")]   // Cyan
    [InlineData("hsl(300, 100%, 50%)")]   // Magenta
    public void RoundTrip_HslColor_ParseSerializeParse_PreservesColor(string input)
    {
        // Parse HSL input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize (converts to rgb())
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.Equal(color1.A, color2.A);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("hwb(0 0% 0%)")]      // Red
    [InlineData("hwb(120 0% 0%)")]    // Green
    [InlineData("hwb(240 0% 0%)")]    // Blue
    [InlineData("hwb(0 50% 50%)")]    // Gray (achromatic)
    public void RoundTrip_HwbColor_ParseSerializeParse_PreservesColor(string input)
    {
        // Parse HWB input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize (converts to rgb())
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.Equal(color1.A, color2.A);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("#ff0000")]
    [InlineData("#00ff00")]
    [InlineData("#0000ff")]
    [InlineData("#abcdef")]
    [InlineData("#123456")]
    public void RoundTrip_HexColor_ParseSerializeParse_PreservesColor(string input)
    {
        // Parse hex input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize (converts to rgb())
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.Equal(color1.A, color2.A);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("#f00")]     // Short hex red
    [InlineData("#0f0")]     // Short hex green
    [InlineData("#00f")]     // Short hex blue
    [InlineData("#abc")]     // Short hex mixed
    [InlineData("#fff")]     // Short hex white
    [InlineData("#000")]     // Short hex black
    public void RoundTrip_ShortHexColor_ParseSerializeParse_PreservesColor(string input)
    {
        // Parse short hex input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.Equal(color1.A, color2.A);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("#ff000080")]  // Red with 50% alpha
    [InlineData("#00ff0040")]  // Green with 25% alpha
    [InlineData("#0000ffbf")]  // Blue with 75% alpha
    public void RoundTrip_HexColorWithAlpha_ParseSerializeParse_PreservesColor(string input)
    {
        // Parse 8-digit hex input
        var parser = new CssParser(input);
        var value1 = parser.Parse_CssValue();
        var color1 = value1.AsCssColor();

        // Serialize
        var serialized = CssColorSerializer.Serialize(color1);

        // Parse the serialized output
        var parser2 = new CssParser(serialized);
        var value2 = parser2.Parse_CssValue();
        var color2 = value2.AsCssColor();

        // Assert the colors are equal
        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.InRange(System.Math.Abs(color1.A - color2.A), 0, 1);
    }

    #endregion

    #region Canonical Form Serialization Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_OpaqueColor_UsesRgbNotRgba()
    {
        var color = CssColor.FromRgba(255, 128, 64, 255);
        var result = CssColorSerializer.Serialize(color);

        // Per spec: opaque colors use rgb() not rgba()
        Assert.StartsWith("rgb(", result);
        Assert.DoesNotContain("rgba", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_NonOpaqueColor_UsesRgba()
    {
        var color = CssColor.FromRgba(255, 128, 64, 128);
        var result = CssColorSerializer.Serialize(color);

        // Per spec: non-opaque colors use rgba()
        Assert.StartsWith("rgba(", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_LegacySyntax_UsesCommas()
    {
        var color = CssColor.FromRgba(255, 128, 64, 255);
        var result = CssColorSerializer.Serialize(color);

        // Per spec: sRGB uses legacy comma-separated syntax
        Assert.Contains(", ", result);
        Assert.DoesNotContain(" / ", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_LabColor_UsesModernSyntax()
    {
        var color = new CssColorHdr(50f, 25f, -25f, 1f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);

        // HDR colors use modern space-separated syntax
        Assert.DoesNotContain(",", result);
        Assert.Equal("lab(50 25 -25)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_ColorFunction_UsesModernSyntax()
    {
        var color = new CssColorHdr(1f, 0f, 0f, 1f, EColorSpace.DisplayP3);
        var result = CssColorSerializer.Serialize(color);

        // color() function uses modern space-separated syntax
        Assert.DoesNotContain(",", result);
        Assert.Equal("color(display-p3 1 0 0)", result);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [InlineData(EColorSpace.sRGB, "srgb")]
    [InlineData(EColorSpace.sRGBLinear, "srgb-linear")]
    [InlineData(EColorSpace.DisplayP3, "display-p3")]
    [InlineData(EColorSpace.A98Rgb, "a98-rgb")]
    [InlineData(EColorSpace.ProPhotoRgb, "prophoto-rgb")]
    [InlineData(EColorSpace.Rec2020, "rec2020")]
    [InlineData(EColorSpace.XyzD50, "xyz-d50")]
    [InlineData(EColorSpace.XyzD65, "xyz-d65")]
    public void Serialize_ColorFunction_UsesCorrectColorSpaceName(EColorSpace colorSpace, string expectedName)
    {
        var color = new CssColorHdr(0.5f, 0.5f, 0.5f, 1f, colorSpace);
        var result = CssColorSerializer.Serialize(color);

        Assert.StartsWith($"color({expectedName} ", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_IntegerComponentValues_OmitsDecimalPoint()
    {
        var color = CssColor.FromRgba(100, 200, 50, 255);
        var result = CssColorSerializer.Serialize(color);

        // Integer values should not have decimal points
        Assert.Equal("rgb(100, 200, 50)", result);
        Assert.DoesNotContain(".", result);
    }

    #endregion

    #region Alpha Channel Omission Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_Alpha255_OmitsAlpha()
    {
        var color = CssColor.FromRgba(255, 128, 0, 255);
        var result = CssColorSerializer.Serialize(color);

        // Alpha exactly 1 (255) should be omitted
        Assert.Equal("rgb(255, 128, 0)", result);
        Assert.DoesNotContain(",", result.Substring(result.LastIndexOf(')')));
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_Alpha254_IncludesAlpha()
    {
        var color = CssColor.FromRgba(255, 128, 0, 254);
        var result = CssColorSerializer.Serialize(color);

        // Alpha not exactly 1 should be included
        Assert.StartsWith("rgba(", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_Alpha0_IncludesAlpha()
    {
        var color = CssColor.FromRgba(255, 128, 0, 0);
        var result = CssColorSerializer.Serialize(color);

        // Alpha 0 should be included
        Assert.Equal("rgba(255, 128, 0, 0)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_HdrOpaqueAlpha_OmitsAlpha()
    {
        var color = new CssColorHdr(50f, 25f, -25f, 1f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);

        // Opaque HDR colors should omit alpha
        Assert.Equal("lab(50 25 -25)", result);
        Assert.DoesNotContain("/", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    public void Serialize_HdrNonOpaqueAlpha_IncludesSlashAlpha()
    {
        var color = new CssColorHdr(50f, 25f, -25f, 0.5f, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);

        // Non-opaque HDR colors should include slash alpha
        Assert.Equal("lab(50 25 -25 / 0.5)", result);
        Assert.Contains(" / ", result);
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [InlineData(1f, false)]   // Opaque - no alpha
    [InlineData(0.999f, true)] // Nearly opaque - includes alpha
    [InlineData(0.5f, true)]   // Semi-transparent - includes alpha
    [InlineData(0f, true)]     // Transparent - includes alpha
    public void Serialize_HdrLabAlpha_OmittedOnlyWhenExactlyOne(float alpha, bool shouldIncludeAlpha)
    {
        var color = new CssColorHdr(50f, 25f, -25f, alpha, EColorSpace.Lab);
        var result = CssColorSerializer.Serialize(color);

        if (shouldIncludeAlpha)
        {
            Assert.Contains(" / ", result);
        }
        else
        {
            Assert.DoesNotContain(" / ", result);
        }
    }

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Serialization")]
    [InlineData(1f, false)]
    [InlineData(0.999f, true)]
    [InlineData(0.5f, true)]
    [InlineData(0f, true)]
    public void Serialize_ColorFunctionAlpha_OmittedOnlyWhenExactlyOne(float alpha, bool shouldIncludeAlpha)
    {
        var color = new CssColorHdr(0.5f, 0.5f, 0.5f, alpha, EColorSpace.DisplayP3);
        var result = CssColorSerializer.Serialize(color);

        if (shouldIncludeAlpha)
        {
            Assert.Contains(" / ", result);
        }
        else
        {
            Assert.DoesNotContain(" / ", result);
        }
    }

    #endregion
}
