using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUI.Tests.CSS.Parser;

/// <summary>
/// Tests for CssColorFunctionParser - parsing rgb()/rgba() and other color functions.
/// Spec Reference: https://www.w3.org/TR/css-color-4/
/// </summary>
[Trait("Category", "CSS")]
[Trait("Category", "Color")]
public class CssColorFunctionParserTests
{
    private CssValue ParseColorValue(string cssColorValue)
    {
        var parser = new CssParser(cssColorValue);
        return parser.Parse_CssValue();
    }

    #region rgb() Legacy Syntax Tests (Comma-Separated)

    [Fact]
    public void TryParseRgb_Legacy_NumberSyntax_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(255, 128, 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgb_Legacy_PercentageSyntax_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(100%, 50%, 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G); // 50% of 255 = 127.5, rounds to 128
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgba_Legacy_WithAlphaNumber_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgba(255, 128, 0, 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 0.5 * 255 = 127.5, rounds to 128
    }

    [Fact]
    public void TryParseRgba_Legacy_WithAlphaPercentage_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgba(255, 128, 0, 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 50% of 255 = 127.5, rounds to 128
    }

    #endregion

    #region rgb() Modern Syntax Tests (Space-Separated)

    [Fact]
    public void TryParseRgb_Modern_NumberSyntax_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(255 128 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgb_Modern_PercentageSyntax_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(100% 50% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgb_Modern_MixedUnits_ReturnsColor()
    {
        // Arrange & Act: rgb(255 50% 0) - modern syntax allows mixing number and percentage
        var value = ParseColorValue("rgb(255 50% 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G); // 50% of 255 = 127.5, rounds to 128
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgb_Modern_WithSlashAlpha_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(255 128 0 / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 0.5 * 255 = 127.5, rounds to 128
    }

    [Fact]
    public void TryParseRgb_Modern_WithSlashAlphaPercentage_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(255 128 0 / 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A);
    }

    #endregion

    #region Edge Cases and Clamping Tests

    [Fact]
    public void TryParseRgb_ValuesClamped_ToValidRange()
    {
        // Arrange & Act: rgb(300, -10, 128) - out of range values should be clamped
        var value = ParseColorValue("rgb(300, -10, 128)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R); // Clamped from 300
        Assert.Equal(0, color.G);   // Clamped from -10
        Assert.Equal(128, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgb_AlphaClamped_ToValidRange()
    {
        // Arrange & Act: rgba(255, 128, 0, 1.5) - alpha > 1 should be clamped
        var value = ParseColorValue("rgba(255, 128, 0, 1.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.A); // Clamped from 1.5
    }

    [Fact]
    public void TryParseRgb_Black_ReturnsBlackColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(0, 0, 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgb_White_ReturnsWhiteColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgb(255, 255, 255)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseRgba_FullyTransparent_ReturnsTransparentColor()
    {
        // Arrange & Act
        var value = ParseColorValue("rgba(255, 0, 0, 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    #endregion

    #region Decimal Values Tests

    [Fact]
    public void TryParseRgb_DecimalValues_RoundsCorrectly()
    {
        // Arrange & Act: rgb(127.6, 127.4, 128.5)
        var value = ParseColorValue("rgb(127.6, 127.4, 128.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(128, color.R); // Rounds 127.6 to 128 (banker's rounding or truncation)
        Assert.Equal(127, color.G); // Rounds 127.4 to 127
        Assert.Equal(128, color.B); // Rounds 128.5 to 128 (banker's rounding) or 129 (standard)
    }

    [Fact]
    public void TryParseRgb_DecimalPercentage_ConvertsCorrectly()
    {
        // Arrange & Act: rgb(50.5%, 25.25%, 75.75%)
        var value = ParseColorValue("rgb(50.5%, 25.25%, 75.75%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // 50.5% of 255 = 128.775 → 129
        // 25.25% of 255 = 64.3875 → 64
        // 75.75% of 255 = 193.1625 → 193
        Assert.Equal(129, color.R);
        Assert.Equal(64, color.G);
        Assert.Equal(193, color.B);
    }

    #endregion

    #region Case Insensitivity Tests

    [Fact]
    public void TryParseRgb_UppercaseFunctionName_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("RGB(255, 128, 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseRgba_MixedCaseFunctionName_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("RgBa(255, 128, 0, 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region hsl() Legacy Syntax Tests (Comma-Separated)

    [Fact]
    public void TryParseHsl_Legacy_BasicSyntax_ReturnsColor()
    {
        // Arrange & Act - Pure red: hue=0, sat=100%, light=50%
        var value = ParseColorValue("hsl(0, 100%, 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseHsl_Legacy_Green_ReturnsColor()
    {
        // Arrange & Act - Pure green: hue=120, sat=100%, light=50%
        var value = ParseColorValue("hsl(120, 100%, 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHsl_Legacy_Blue_ReturnsColor()
    {
        // Arrange & Act - Pure blue: hue=240, sat=100%, light=50%
        var value = ParseColorValue("hsl(240, 100%, 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHsla_Legacy_WithAlpha_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("hsla(0, 100%, 50%, 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 0.5 * 255 = 127.5, rounds to 128
    }

    [Fact]
    public void TryParseHsla_Legacy_WithAlphaPercentage_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("hsla(120, 100%, 50%, 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 50% of 255 = 127.5, rounds to 128
    }

    #endregion

    #region hsl() Modern Syntax Tests (Space-Separated)

    [Fact]
    public void TryParseHsl_Modern_BasicSyntax_ReturnsColor()
    {
        // Arrange & Act - Pure red: hue=0, sat=100%, light=50%
        var value = ParseColorValue("hsl(0 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseHsl_Modern_WithSlashAlpha_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("hsl(0 100% 50% / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A);
    }

    [Fact]
    public void TryParseHsl_Modern_WithSlashAlphaPercentage_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("hsl(120 100% 50% / 75%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(191, color.A); // 75% of 255 = 191.25, rounds to 191
    }

    [Fact]
    public void TryParseHsl_Modern_WithNumberSaturationAndLightness_ReturnsColor()
    {
        // Arrange & Act - Modern syntax allows numbers for S and L
        var value = ParseColorValue("hsl(0 100 50)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    #endregion

    #region HSL Hue Angle Unit Tests

    [Fact]
    public void TryParseHsl_HueWithDegUnit_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("hsl(120deg 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHsl_HueWithTurnUnit_ReturnsColor()
    {
        // Arrange & Act - 0.5turn = 180deg = cyan
        var value = ParseColorValue("hsl(0.5turn 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B); // Cyan
    }

    [Fact]
    public void TryParseHsl_HueWithRadUnit_ReturnsColor()
    {
        // Arrange & Act - π rad = 180deg = cyan
        var value = ParseColorValue("hsl(3.14159rad 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // At 180deg with 100% sat 50% light = cyan (0, 255, 255)
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHsl_HueWithGradUnit_ReturnsColor()
    {
        // Arrange & Act - 200grad = 180deg = cyan
        var value = ParseColorValue("hsl(200grad 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B); // Cyan
    }

    #endregion

    #region HSL Edge Cases and Normalization

    [Fact]
    public void TryParseHsl_NegativeHue_NormalizesToPositive()
    {
        // Arrange & Act - -120deg should normalize to 240deg (blue)
        var value = ParseColorValue("hsl(-120 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B); // Blue at 240deg
    }

    [Fact]
    public void TryParseHsl_HueOver360_NormalizesToRange()
    {
        // Arrange & Act - 480deg should normalize to 120deg (green)
        var value = ParseColorValue("hsl(480 100% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B); // Green at 120deg
    }

    [Fact]
    public void TryParseHsl_ZeroSaturation_ReturnsGray()
    {
        // Arrange & Act - 0% saturation = grayscale
        var value = ParseColorValue("hsl(0 0% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(128, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(128, color.B);
    }

    [Fact]
    public void TryParseHsl_ZeroLightness_ReturnsBlack()
    {
        // Arrange & Act - 0% lightness = black
        var value = ParseColorValue("hsl(0 100% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHsl_FullLightness_ReturnsWhite()
    {
        // Arrange & Act - 100% lightness = white
        var value = ParseColorValue("hsl(0 100% 100%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHsl_NegativeSaturation_ClampedToZero()
    {
        // Arrange & Act - Per spec, negative saturation is clamped to 0
        var value = ParseColorValue("hsl(0 -50% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // With 0% saturation, result is gray at 50% lightness
        Assert.Equal(128, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(128, color.B);
    }

    #endregion

    #region HSL Case Insensitivity Tests

    [Fact]
    public void TryParseHsl_UppercaseFunctionName_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("HSL(120, 100%, 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHsla_MixedCaseFunctionName_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("HsLa(240, 100%, 50%, 0.8)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(204, color.A); // 0.8 * 255 = 204
    }

    #endregion

    #region HSL Invalid Input Tests

    [Fact]
    public void TryParseHsl_InvalidArguments_ReturnsNull()
    {
        // Arrange & Act - Missing lightness
        var value = ParseColorValue("hsl(0, 100%)");

        // Assert - Should return null or default value
        Assert.True(value.Type == ECssValueTypes.NULL || value.Type == ECssValueTypes.DIMENSION);
    }

    [Fact]
    public void TryParseHsl_EmptyFunction_ReturnsNull()
    {
        // Arrange & Act
        var value = ParseColorValue("hsl()");

        // Assert
        Assert.True(value.Type == ECssValueTypes.NULL || value.Type == ECssValueTypes.DIMENSION);
    }

    #endregion

    #region Property Value Integration Tests

    [Fact]
    public void Parse_ColorProperty_WithRgbFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: rgb(255, 0, 0)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        // Note: In declarations, functions are stored as CssFunction tokens
        // We need to validate that the function name is correct
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("rgb", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_BackgroundColorProperty_WithRgbaFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "background-color: rgba(0, 128, 255, 0.75)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("background-color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("rgba", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_ColorProperty_WithHslFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: hsl(120, 100%, 50%)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("hsl", func.Name, ignoreCase: true);
    }

    #endregion

    #region hwb() Modern Syntax Tests (Space-Separated Only)

    [Fact]
    public void TryParseHwb_Modern_PercentageSyntax_ReturnsColor()
    {
        // Arrange & Act - hwb(0 100% 0%) = pure white
        var value = ParseColorValue("hwb(0 100% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseHwb_Modern_NumberSyntax_ReturnsColor()
    {
        // Arrange & Act - hwb(0 0 0) = pure red (0 whiteness, 0 blackness)
        var value = ParseColorValue("hwb(0 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseHwb_Modern_WithAlphaSlash_ReturnsColor()
    {
        // Arrange & Act - hwb(0 0% 0% / 0.5) = 50% transparent red
        var value = ParseColorValue("hwb(0 0% 0% / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 0.5 * 255 = 127.5, rounds to 128
    }

    [Fact]
    public void TryParseHwb_Modern_WithAlphaPercentage_ReturnsColor()
    {
        // Arrange & Act - hwb(0 0% 0% / 50%) = 50% transparent red
        var value = ParseColorValue("hwb(0 0% 0% / 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A);
    }

    [Fact]
    public void TryParseHwb_Green_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(120 0% 0%) = pure green
        var value = ParseColorValue("hwb(120 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHwb_Blue_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(240 0% 0%) = pure blue
        var value = ParseColorValue("hwb(240 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHwb_WithWhiteness_LightensColor()
    {
        // Arrange & Act - hwb(0 50% 0%) = red lightened with 50% white
        var value = ParseColorValue("hwb(0 50% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // With 50% whiteness: r = 1.0 * 0.5 + 0.5 = 1.0, g = 0 * 0.5 + 0.5 = 0.5, b = 0 * 0.5 + 0.5 = 0.5
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(128, color.B);
    }

    [Fact]
    public void TryParseHwb_WithBlackness_DarkensColor()
    {
        // Arrange & Act - hwb(0 0% 50%) = red darkened with 50% black
        var value = ParseColorValue("hwb(0 0% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // With 50% blackness: r = 1.0 * 0.5 + 0 = 0.5, g = 0 * 0.5 + 0 = 0, b = 0 * 0.5 + 0 = 0
        Assert.Equal(128, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHwb_AchromaticColor_WhiteAndBlackOver100_ReturnsGray()
    {
        // Arrange & Act - hwb(0 40% 80%) = achromatic, gray = 40 / (40 + 80) = 0.333...
        var value = ParseColorValue("hwb(0 40% 80%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Per spec: when W+B >= 100%, result is gray = W / (W + B) = 40 / 120 = 0.333...
        int expectedGray = (int)Math.Round(40.0 / 120.0 * 255);
        Assert.Equal(expectedGray, color.R);
        Assert.Equal(expectedGray, color.G);
        Assert.Equal(expectedGray, color.B);
    }

    [Fact]
    public void TryParseHwb_AchromaticColor_ExactlyAt100_ReturnsGray()
    {
        // Arrange & Act - hwb(0 50% 50%) = achromatic, gray = 50 / (50 + 50) = 0.5
        var value = ParseColorValue("hwb(0 50% 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        int expectedGray = (int)Math.Round(0.5 * 255); // 128
        Assert.Equal(expectedGray, color.R);
        Assert.Equal(expectedGray, color.G);
        Assert.Equal(expectedGray, color.B);
    }

    [Fact]
    public void TryParseHwb_DegUnit_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(120deg 0% 0%) = pure green
        var value = ParseColorValue("hwb(120deg 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHwb_TurnUnit_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(0.5turn 0% 0%) = cyan (180 degrees)
        var value = ParseColorValue("hwb(0.5turn 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHwb_NegativeHue_NormalizesCorrectly()
    {
        // Arrange & Act - hwb(-60 0% 0%) = magenta (300 degrees)
        var value = ParseColorValue("hwb(-60 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // -60 normalizes to 300 degrees = magenta
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHwb_HueOver360_NormalizesCorrectly()
    {
        // Arrange & Act - hwb(480 0% 0%) = green (480 - 360 = 120 degrees)
        var value = ParseColorValue("hwb(480 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHwb_CaseInsensitive_ReturnsColor()
    {
        // Arrange & Act - HWB in uppercase
        var value = ParseColorValue("HWB(0 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHwb_PureBlack_ReturnsBlack()
    {
        // Arrange & Act - hwb(0 0% 100%) = pure black
        var value = ParseColorValue("hwb(0 0% 100%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Per achromatic rule: 0 / (0 + 100) = 0
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseHwb_PureWhite_ReturnsWhite()
    {
        // Arrange & Act - hwb(0 100% 0%) = pure white
        var value = ParseColorValue("hwb(0 100% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Per achromatic rule: 100 / (100 + 0) = 1
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseHwb_SpecExample_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(150 20% 10%) is the same as hsl(150 77.78% 55%)
        // Per W3C spec example
        var value = ParseColorValue("hwb(150 20% 10%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Expected: rgb(20% 90% 55%) = rgb(51, 230, 140) approximately
        // Let's verify the calculation:
        // H=150, W=0.2, B=0.1, so scale factor = 1 - 0.2 - 0.1 = 0.7
        // Base HSL(150, 100%, 50%) = (0, 255, 128) approx
        // Actually for H=150: r=0, g=1, b=0.5 (roughly)
        // Result: r = 0*0.7 + 0.2 = 0.2, g = 1*0.7 + 0.2 = 0.9, b = 0.5*0.7 + 0.2 = 0.55
        Assert.InRange(color.R, 48, 56);   // ~51 (20%)
        Assert.InRange(color.G, 226, 234); // ~230 (90%)
        Assert.InRange(color.B, 136, 144); // ~140 (55%)
    }

    [Fact]
    public void TryParseHwb_LegacySyntaxWithCommas_ReturnsNull()
    {
        // Arrange & Act - HWB does NOT support legacy comma syntax per spec
        var value = ParseColorValue("hwb(0, 0%, 0%)");

        // Assert - should fail to parse as color (commas are invalid)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseHwb_RadUnit_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(3.14159rad 0% 0%) ≈ hwb(180deg 0% 0%) = cyan
        var value = ParseColorValue("hwb(3.14159rad 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.InRange(color.G, 254, 255); // Allow for rounding
        Assert.InRange(color.B, 254, 255);
    }

    [Fact]
    public void TryParseHwb_GradUnit_ReturnsCorrectColor()
    {
        // Arrange & Act - hwb(200grad 0% 0%) = hwb(180deg 0% 0%) = cyan
        // 200 grads = 200 * (360/400) = 180 degrees
        var value = ParseColorValue("hwb(200grad 0% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    #endregion

    #region hwb() Property Parsing Tests

    [Fact]
    public void Parse_ColorProperty_WithHwbFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: hwb(0 0% 0%)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("hwb", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_BackgroundColorProperty_WithHwbFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "background-color: hwb(240 20% 30% / 0.8)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("background-color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("hwb", func.Name, ignoreCase: true);
    }

    #endregion
}
