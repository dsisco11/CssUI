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

        // Assert - Should not return a color value (may be FUNCTION type if parsing fails)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseHsl_EmptyFunction_ReturnsNull()
    {
        // Arrange & Act
        var value = ParseColorValue("hsl()");

        // Assert - Should not return a color value (may be FUNCTION type if parsing fails)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
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

    #region lab() Basic Syntax Tests

    [Fact]
    public void TryParseLab_BasicNumberSyntax_ReturnsColor()
    {
        // Arrange & Act - lab(50 0 0) = mid-gray
        var value = ParseColorValue("lab(50 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // L=50, a=0, b=0 should produce a mid-gray
        Assert.InRange(color.R, 115, 120); // Expected ~119
        Assert.InRange(color.G, 115, 120);
        Assert.InRange(color.B, 115, 120);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseLab_WithPercentageLightness_ReturnsColor()
    {
        // Arrange & Act - lab(50% 0 0) = lab(50 0 0) = mid-gray
        var value = ParseColorValue("lab(50% 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.R, 115, 120);
        Assert.InRange(color.G, 115, 120);
        Assert.InRange(color.B, 115, 120);
    }

    [Fact]
    public void TryParseLab_WithAlpha_ReturnsColorWithAlpha()
    {
        // Arrange & Act
        var value = ParseColorValue("lab(50 0 0 / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.A, 127, 128); // 50% alpha
    }

    [Fact]
    public void TryParseLab_WithPercentageAlpha_ReturnsColorWithAlpha()
    {
        // Arrange & Act
        var value = ParseColorValue("lab(50 0 0 / 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.A, 127, 128);
    }

    [Fact]
    public void TryParseLab_Black_ReturnsBlack()
    {
        // Arrange & Act - lab(0 0 0) = black (L=0)
        var value = ParseColorValue("lab(0 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseLab_White_ReturnsWhite()
    {
        // Arrange & Act - lab(100 0 0) = white (L=100)
        var value = ParseColorValue("lab(100 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseLab_PositiveA_ShiftsTowardRed()
    {
        // Arrange & Act - Positive 'a' shifts toward red/magenta
        var value = ParseColorValue("lab(50 80 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should have more red than gray
        Assert.True(color.R > color.G);
    }

    [Fact]
    public void TryParseLab_NegativeA_ShiftsTowardGreen()
    {
        // Arrange & Act - Negative 'a' shifts toward green
        var value = ParseColorValue("lab(50 -80 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should have more green than red (after gamut mapping/clamping)
        Assert.True(color.G > color.R);
    }

    [Fact]
    public void TryParseLab_PositiveB_ShiftsTowardYellow()
    {
        // Arrange & Act - Positive 'b' shifts toward yellow
        var value = ParseColorValue("lab(50 0 80)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Yellow = high R, high G, low B
        Assert.True(color.R > color.B);
        Assert.True(color.G > color.B);
    }

    [Fact]
    public void TryParseLab_NegativeB_ShiftsTowardBlue()
    {
        // Arrange & Act - Negative 'b' shifts toward blue
        var value = ParseColorValue("lab(50 0 -80)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should have more blue (after gamut mapping/clamping)
        Assert.True(color.B > color.R);
    }

    [Fact]
    public void TryParseLab_PercentageAB_MapsToCorrectRange()
    {
        // Arrange & Act - 100% on a/b axis = 125, so lab(50 100% 0) = lab(50 125 0)
        var value = ParseColorValue("lab(50 100% 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Very high positive 'a' - should be strongly red-shifted
        Assert.True(color.R > 200);
    }

    [Fact]
    public void TryParseLab_LightnessClampedToZero()
    {
        // Arrange & Act - Negative lightness clamped to 0
        var value = ParseColorValue("lab(-50 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be black (L clamped to 0)
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseLab_LightnessClampedTo100()
    {
        // Arrange & Act - Lightness > 100 clamped to 100
        var value = ParseColorValue("lab(150 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be white (L clamped to 100)
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseLab_LegacyCommas_ReturnsFalse()
    {
        // Arrange & Act - lab() does NOT support comma syntax
        var value = ParseColorValue("lab(50, 0, 0)");

        // Assert - Should fail to parse as color (no legacy comma syntax)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLab_CaseInsensitive()
    {
        // Arrange & Act
        var value = ParseColorValue("LAB(50 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region lch() Basic Syntax Tests

    [Fact]
    public void TryParseLch_BasicNumberSyntax_ReturnsColor()
    {
        // Arrange & Act - lch(50 0 0) = mid-gray (chroma=0 means no color, just gray)
        var value = ParseColorValue("lch(50 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.R, 115, 120);
        Assert.InRange(color.G, 115, 120);
        Assert.InRange(color.B, 115, 120);
    }

    [Fact]
    public void TryParseLch_WithPercentageLightness_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("lch(50% 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.R, 115, 120);
    }

    [Fact]
    public void TryParseLch_WithAlpha_ReturnsColorWithAlpha()
    {
        // Arrange & Act
        var value = ParseColorValue("lch(50 0 0 / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.A, 127, 128);
    }

    [Fact]
    public void TryParseLch_Black_ReturnsBlack()
    {
        // Arrange & Act
        var value = ParseColorValue("lch(0 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseLch_White_ReturnsWhite()
    {
        // Arrange & Act
        var value = ParseColorValue("lch(100 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseLch_Red_Hue0_ReturnsReddish()
    {
        // Arrange & Act - lch(50 100 40) should be roughly red-ish
        // LCH hue ~40 is in the red-orange range
        var value = ParseColorValue("lch(50 100 40)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.True(color.R > color.G && color.R > color.B);
    }

    [Fact]
    public void TryParseLch_Green_Hue130_ReturnsGreenish()
    {
        // Arrange & Act - lch(50 100 130) should be roughly green-ish
        var value = ParseColorValue("lch(50 100 130)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.True(color.G > color.R && color.G > color.B);
    }

    [Fact]
    public void TryParseLch_Blue_Hue300_ReturnsBluish()
    {
        // Arrange & Act - lch(50 100 300) should be roughly blue-ish
        var value = ParseColorValue("lch(50 100 300)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.True(color.B > color.R || color.B > color.G); // Some blue component
    }

    [Fact]
    public void TryParseLch_DegUnit_ParsesCorrectly()
    {
        // Arrange & Act
        var value = ParseColorValue("lch(50 100 40deg)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_RadUnit_ParsesCorrectly()
    {
        // Arrange & Act - 0.698rad ≈ 40deg
        var value = ParseColorValue("lch(50 100 0.698rad)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_TurnUnit_ParsesCorrectly()
    {
        // Arrange & Act - 0.5turn = 180deg
        var value = ParseColorValue("lch(50 100 0.5turn)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_GradUnit_ParsesCorrectly()
    {
        // Arrange & Act - 100grad = 90deg
        var value = ParseColorValue("lch(50 100 100grad)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_PercentageChroma_MapsCorrectly()
    {
        // Arrange & Act - 100% chroma = 150
        var value = ParseColorValue("lch(50 100% 40)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_NegativeChroma_ClampedToZero()
    {
        // Arrange & Act - Negative chroma clamped to 0
        var value = ParseColorValue("lch(50 -50 40)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // With chroma clamped to 0, should be gray
        Assert.InRange(color.R, 115, 120);
        Assert.InRange(color.G, 115, 120);
        Assert.InRange(color.B, 115, 120);
    }

    [Fact]
    public void TryParseLch_NegativeHue_NormalizesToPositive()
    {
        // Arrange & Act - -60deg = 300deg
        var value = ParseColorValue("lch(50 100 -60)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_HueOver360_NormalizesCorrectly()
    {
        // Arrange & Act - 400deg = 40deg
        var value = ParseColorValue("lch(50 100 400)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_LegacyCommas_ReturnsFalse()
    {
        // Arrange & Act - lch() does NOT support comma syntax
        var value = ParseColorValue("lch(50, 0, 0)");

        // Assert - Should fail to parse as color
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_CaseInsensitive()
    {
        // Arrange & Act
        var value = ParseColorValue("LCH(50 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region lab()/lch() W3C Spec Examples

    [Fact]
    public void TryParseLab_W3CExample_Yellow()
    {
        // From W3C spec: lab(97.607% -15.753 93.388) is sRGB yellow
        var value = ParseColorValue("lab(97.607 -15.753 93.388)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be close to yellow (high R, high G, low B)
        Assert.True(color.R > 240);
        Assert.True(color.G > 240);
        Assert.True(color.B < 50);
    }

    [Fact]
    public void TryParseLab_W3CExample_Blue()
    {
        // From W3C spec: lab(29.567% 68.298 -112.0294) is sRGB blue
        var value = ParseColorValue("lab(29.567 68.298 -112.0294)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be roughly blue-ish (though out of gamut gets clamped)
        Assert.True(color.B > color.R);
    }

    [Fact]
    public void TryParseLch_W3CExample_Basic()
    {
        // From W3C spec: lch(52.2345% 72.2 56.2)
        var value = ParseColorValue("lch(52.2345% 72.2 56.2)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseLch_W3CExample_WithPercentageChroma()
    {
        // From W3C spec: lch(29.69% 45.553% 327.1)
        // 45.553% of 150 = ~68.33 chroma
        var value = ParseColorValue("lch(29.69% 45.553% 327.1)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region lab()/lch() Property Parsing Tests

    [Fact]
    public void Parse_ColorProperty_WithLabFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: lab(50 0 0)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("lab", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_ColorProperty_WithLchFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: lch(50 100 40)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("lch", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_BackgroundColorProperty_WithLabAlpha_ReturnsColorValue()
    {
        // Arrange
        var css = "background-color: lab(50 25 -25 / 0.8)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("background-color", declaration.Name);
    }

    #endregion

    #region oklab() Basic Syntax Tests

    [Fact]
    public void TryParseOklab_BasicNumberSyntax_ReturnsColor()
    {
        // Arrange & Act - oklab(0.5 0 0) = mid-gray (L=0.5 in OKLab scale)
        var value = ParseColorValue("oklab(0.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // L=0.5 in OKLab is perceptually mid-gray, which maps to ~RGB 99
        // (OKLab is perceptually uniform, so L=0.5 corresponds to ~39% luminance)
        Assert.InRange(color.R, 95, 105);
        Assert.InRange(color.G, 95, 105);
        Assert.InRange(color.B, 95, 105);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseOklab_WithPercentageLightness_ReturnsColor()
    {
        // Arrange & Act - oklab(50% 0 0) = oklab(0.5 0 0) = perceptually mid-gray
        var value = ParseColorValue("oklab(50% 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.R, 95, 105);
        Assert.InRange(color.G, 95, 105);
        Assert.InRange(color.B, 95, 105);
    }

    [Fact]
    public void TryParseOklab_WithAlpha_ReturnsColorWithAlpha()
    {
        // Arrange & Act
        var value = ParseColorValue("oklab(0.5 0 0 / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.A, 127, 128); // 50% alpha
    }

    [Fact]
    public void TryParseOklab_WithPercentageAlpha_ReturnsColorWithAlpha()
    {
        // Arrange & Act
        var value = ParseColorValue("oklab(0.5 0 0 / 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.A, 127, 128);
    }

    [Fact]
    public void TryParseOklab_Black_ReturnsBlack()
    {
        // Arrange & Act - oklab(0 0 0) = black (L=0)
        var value = ParseColorValue("oklab(0 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseOklab_White_ReturnsWhite()
    {
        // Arrange & Act - oklab(1 0 0) = white (L=1)
        var value = ParseColorValue("oklab(1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseOklab_White_Percentage_ReturnsWhite()
    {
        // Arrange & Act - oklab(100% 0 0) = white (L=1)
        var value = ParseColorValue("oklab(100% 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseOklab_PositiveA_ShiftsTowardRed()
    {
        // Arrange & Act - Positive 'a' shifts toward red/magenta
        var value = ParseColorValue("oklab(0.5 0.2 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should have more red than green
        Assert.True(color.R > color.G);
    }

    [Fact]
    public void TryParseOklab_NegativeA_ShiftsTowardGreen()
    {
        // Arrange & Act - Negative 'a' shifts toward green
        var value = ParseColorValue("oklab(0.5 -0.2 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should have more green than red
        Assert.True(color.G > color.R);
    }

    [Fact]
    public void TryParseOklab_PositiveB_ShiftsTowardYellow()
    {
        // Arrange & Act - Positive 'b' shifts toward yellow
        var value = ParseColorValue("oklab(0.5 0 0.2)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Yellow = high R, high G, low B
        Assert.True(color.R > color.B);
        Assert.True(color.G > color.B);
    }

    [Fact]
    public void TryParseOklab_NegativeB_ShiftsTowardBlue()
    {
        // Arrange & Act - Negative 'b' shifts toward blue
        var value = ParseColorValue("oklab(0.5 0 -0.2)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should have more blue
        Assert.True(color.B > color.R);
    }

    [Fact]
    public void TryParseOklab_PercentageAB_MapsToCorrectRange()
    {
        // Arrange & Act - 100% on a/b axis = 0.4, so oklab(0.5 100% 0) = oklab(0.5 0.4 0)
        var value = ParseColorValue("oklab(0.5 100% 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Very high positive 'a' - should be strongly red-shifted
        Assert.True(color.R > color.G);
    }

    [Fact]
    public void TryParseOklab_LightnessClampedToZero()
    {
        // Arrange & Act - Negative lightness clamped to 0
        var value = ParseColorValue("oklab(-0.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be black (L clamped to 0)
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseOklab_LightnessClampedTo1()
    {
        // Arrange & Act - Lightness > 1 clamped to 1
        var value = ParseColorValue("oklab(1.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be white (L clamped to 1)
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseOklab_LegacyCommas_ReturnsFalse()
    {
        // Arrange & Act - oklab() does NOT support comma syntax
        var value = ParseColorValue("oklab(0.5, 0, 0)");

        // Assert - Should fail to parse as color (no legacy comma syntax)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseOklab_CaseInsensitive()
    {
        // Arrange & Act
        var value = ParseColorValue("OKLAB(0.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region oklch() Basic Syntax Tests

    [Fact]
    public void TryParseOklch_BasicNumberSyntax_ReturnsColor()
    {
        // Arrange & Act - oklch(0.5 0 0) = perceptually mid-gray (no chroma)
        var value = ParseColorValue("oklch(0.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // L=0.5, C=0 should produce a perceptually mid-gray regardless of hue (~RGB 99)
        Assert.InRange(color.R, 95, 105);
        Assert.InRange(color.G, 95, 105);
        Assert.InRange(color.B, 95, 105);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseOklch_WithPercentageLightness_ReturnsColor()
    {
        // Arrange & Act - oklch(50% 0 0) = oklch(0.5 0 0) = perceptually mid-gray
        var value = ParseColorValue("oklch(50% 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.R, 95, 105);
        Assert.InRange(color.G, 95, 105);
        Assert.InRange(color.B, 95, 105);
    }

    [Fact]
    public void TryParseOklch_WithAlpha_ReturnsColorWithAlpha()
    {
        // Arrange & Act
        var value = ParseColorValue("oklch(0.5 0 0 / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.InRange(color.A, 127, 128); // 50% alpha
    }

    [Fact]
    public void TryParseOklch_Black_ReturnsBlack()
    {
        // Arrange & Act - oklch(0 0 0) = black (L=0)
        var value = ParseColorValue("oklch(0 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseOklch_White_ReturnsWhite()
    {
        // Arrange & Act - oklch(1 0 0) = white (L=1, no chroma)
        var value = ParseColorValue("oklch(1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseOklch_Red_Hue0()
    {
        // Arrange & Act - oklch with chroma and hue ~29deg is roughly red
        var value = ParseColorValue("oklch(0.628 0.258 29.2)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be red-ish
        Assert.True(color.R > color.G);
        Assert.True(color.R > color.B);
    }

    [Fact]
    public void TryParseOklch_Green_Hue142()
    {
        // Arrange & Act - oklch at ~142deg hue is roughly green
        var value = ParseColorValue("oklch(0.866 0.295 142.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be green-ish (clamped/gamut-mapped)
        Assert.True(color.G > color.R || color.G > color.B);
    }

    [Fact]
    public void TryParseOklch_Blue_Hue264()
    {
        // Arrange & Act - oklch at ~264deg hue is roughly blue
        var value = ParseColorValue("oklch(0.452 0.313 264.1)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be blue-ish (clamped/gamut-mapped)
        Assert.True(color.B > color.R);
    }

    [Fact]
    public void TryParseOklch_WithAngleUnit_Deg()
    {
        // Arrange & Act - hue in degrees
        var value = ParseColorValue("oklch(0.5 0.15 180deg)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Hue at 180deg is cyan-ish
        Assert.True(color.G > color.R);
    }

    [Fact]
    public void TryParseOklch_WithAngleUnit_Rad()
    {
        // Arrange & Act - hue in radians (π = 180deg)
        var value = ParseColorValue("oklch(0.5 0.15 3.14159rad)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        // Should parse without error
    }

    [Fact]
    public void TryParseOklch_WithAngleUnit_Turn()
    {
        // Arrange & Act - hue in turns (0.5turn = 180deg)
        var value = ParseColorValue("oklch(0.5 0.15 0.5turn)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseOklch_PercentageChroma_MapsToCorrectRange()
    {
        // Arrange & Act - 100% chroma = 0.4
        var value = ParseColorValue("oklch(0.5 100% 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // High chroma should produce saturated color
        Assert.True(color.R > color.G || color.R > color.B);
    }

    [Fact]
    public void TryParseOklch_NegativeChromaClampedToZero()
    {
        // Arrange & Act - Negative chroma clamped to 0
        var value = ParseColorValue("oklch(0.5 -0.1 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be perceptually mid-gray (C clamped to 0), ~RGB 99
        Assert.InRange(color.R, 95, 105);
        Assert.InRange(color.G, 95, 105);
        Assert.InRange(color.B, 95, 105);
    }

    [Fact]
    public void TryParseOklch_LegacyCommas_ReturnsFalse()
    {
        // Arrange & Act - oklch() does NOT support comma syntax
        var value = ParseColorValue("oklch(0.5, 0.1, 180)");

        // Assert - Should fail to parse as color (no legacy comma syntax)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseOklch_CaseInsensitive()
    {
        // Arrange & Act
        var value = ParseColorValue("OKLCH(0.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region oklab()/oklch() W3C Spec Examples

    [Fact]
    public void TryParseOklch_W3CExample_1()
    {
        // From W3C spec: oklch(72.322% 0.12403 247.996)
        var value = ParseColorValue("oklch(72.322% 0.12403 247.996)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseOklch_W3CExample_WithPercentageChroma()
    {
        // From W3C spec: oklch(42.1% 48.25% 328.4)
        // 48.25% of 0.4 = ~0.193 chroma
        var value = ParseColorValue("oklch(42.1% 48.25% 328.4)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region oklab()/oklch() Property Parsing Tests

    [Fact]
    public void Parse_ColorProperty_WithOklabFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: oklab(0.5 0 0)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("oklab", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_ColorProperty_WithOklchFunction_ReturnsColorValue()
    {
        // Arrange
        var css = "color: oklch(0.5 0.1 180)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("color", declaration.Name);
        Assert.Single(declaration.Values);
        Assert.True(declaration.Values[0] is CssFunction);
        var func = (CssFunction)declaration.Values[0];
        Assert.Equal("oklch", func.Name, ignoreCase: true);
    }

    [Fact]
    public void Parse_BackgroundColorProperty_WithOklabAlpha_ReturnsColorValue()
    {
        // Arrange
        var css = "background-color: oklab(0.5 0.1 -0.1 / 0.8)";
        var parser = new CssParser(css);

        // Act
        var declaration = parser.Parse_Decleration();

        // Assert
        Assert.NotNull(declaration);
        Assert.Equal("background-color", declaration.Name);
    }

    #endregion

    #region color() Function - sRGB Color Space

    [Fact]
    public void TryParseColor_Srgb_BasicValues_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(srgb 1 0.5 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G); // 0.5 * 255 = 127.5, rounds to 128
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryParseColor_Srgb_WithPercentages_ReturnsColor()
    {
        // Arrange & Act - percentages map 100% = 1.0
        var value = ParseColorValue("color(srgb 100% 50% 0%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseColor_Srgb_WithAlpha_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(srgb 1 0 0 / 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A); // 0.5 * 255 = 127.5, rounds to 128
    }

    [Fact]
    public void TryParseColor_Srgb_WithAlphaPercentage_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(srgb 1 0 0 / 50%)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(128, color.A);
    }

    [Fact]
    public void TryParseColor_Srgb_WithNone_ReturnsColor()
    {
        // Arrange & Act - 'none' is treated as 0
        var value = ParseColorValue("color(srgb none 0.5 1)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(255, color.B);
    }

    #endregion

    #region color() Function - sRGB-Linear Color Space

    [Fact]
    public void TryParseColor_SrgbLinear_BasicValues_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(srgb-linear 1 0 0)");

        // Assert - linear 1 = gamma 1 (identity)
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    public void TryParseColor_SrgbLinear_MidValue_ReturnsColor()
    {
        // Arrange & Act - linear 0.5 converts to gamma ~0.735
        var value = ParseColorValue("color(srgb-linear 0.5 0.5 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // 0.5 linear -> ~0.735 gamma -> ~187 (approx)
        Assert.True(color.R >= 180 && color.R <= 195); // Allow some tolerance
    }

    #endregion

    #region color() Function - Display P3 Color Space

    [Fact]
    public void TryParseColor_DisplayP3_BasicValues_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(display-p3 1 0 0)");

        // Assert - display-p3 red should convert to sRGB
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        // P3 red is more saturated than sRGB, so G and B might be slightly different
        Assert.True(color.R == 255);
    }

    [Fact]
    public void TryParseColor_DisplayP3_W3CExample_ReturnsColor()
    {
        // From W3C spec: color(display-p3 0.43313 0.50108 0.37950)
        var value = ParseColorValue("color(display-p3 0.43313 0.50108 0.37950)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should convert to approximately the same sRGB color
        Assert.True(color.R > 0);
        Assert.True(color.G > 0);
        Assert.True(color.B > 0);
    }

    [Fact]
    public void TryParseColor_DisplayP3_WithAlpha_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(display-p3 1 0 0 / 0.8)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(204, color.A); // 0.8 * 255 = 204
    }

    #endregion

    #region color() Function - A98-RGB Color Space

    [Fact]
    public void TryParseColor_A98Rgb_BasicValues_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(a98-rgb 1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
    }

    [Fact]
    public void TryParseColor_A98Rgb_W3CExample_ReturnsColor()
    {
        // From W3C spec: color(a98-rgb 0.44091 0.49971 0.37408)
        var value = ParseColorValue("color(a98-rgb 0.44091 0.49971 0.37408)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region color() Function - ProPhoto-RGB Color Space

    [Fact]
    public void TryParseColor_ProPhotoRgb_BasicValues_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(prophoto-rgb 1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // ProPhoto red is very saturated, will be clamped
        Assert.Equal(255, color.R);
    }

    [Fact]
    public void TryParseColor_ProPhotoRgb_W3CExample_ReturnsColor()
    {
        // From W3C spec: color(prophoto-rgb 0.36589 0.41717 0.31333)
        var value = ParseColorValue("color(prophoto-rgb 0.36589 0.41717 0.31333)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region color() Function - Rec2020 Color Space

    [Fact]
    public void TryParseColor_Rec2020_BasicValues_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(rec2020 1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
    }

    [Fact]
    public void TryParseColor_Rec2020_W3CExample_ReturnsColor()
    {
        // From W3C spec: color(rec2020 0.42210 0.47580 0.35605)
        var value = ParseColorValue("color(rec2020 0.42210 0.47580 0.35605)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region color() Function - XYZ Color Spaces

    [Fact]
    public void TryParseColor_XyzD65_BasicValues_ReturnsColor()
    {
        // Arrange & Act - white in XYZ D65
        var value = ParseColorValue("color(xyz-d65 0.9505 1 1.089)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseColor_Xyz_AliasForXyzD65_ReturnsColor()
    {
        // Arrange & Act - 'xyz' is an alias for 'xyz-d65'
        var value = ParseColorValue("color(xyz 0.9505 1 1.089)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseColor_XyzD50_BasicValues_ReturnsColor()
    {
        // Arrange & Act - white in XYZ D50
        var value = ParseColorValue("color(xyz-d50 0.9643 1 0.8251)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    public void TryParseColor_XyzD65_W3CExample_ReturnsColor()
    {
        // From W3C spec: color(xyz-d65 0.21661 0.14602 0.59452)
        // This is equivalent to #7654CD
        var value = ParseColorValue("color(xyz-d65 0.21661 0.14602 0.59452)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        // Should be approximately #7654CD (118, 84, 205)
        Assert.True(color.R >= 110 && color.R <= 125);
        Assert.True(color.G >= 80 && color.G <= 90);
        Assert.True(color.B >= 200 && color.B <= 210);
    }

    #endregion

    #region color() Function - Error Cases

    [Fact]
    public void TryParseColor_WithCommas_DoesNotReturnColor()
    {
        // Arrange & Act - color() does NOT support legacy comma syntax
        var value = ParseColorValue("color(srgb, 1, 0, 0)");

        // Assert - Should not return a color value (may be FUNCTION type if parsing fails)
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseColor_UnknownColorSpace_DoesNotReturnColor()
    {
        // Arrange & Act - unknown color space should not return a color
        var value = ParseColorValue("color(unknown-space 1 0 0)");

        // Assert - Should not return a color value
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseColor_MissingComponents_DoesNotReturnColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(srgb 1 0)");

        // Assert - Should not return a color value
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseColor_TooManyComponents_DoesNotReturnColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(srgb 1 0 0 0.5)");

        // Assert - This should fail because there's no slash separator
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseColor_NoColorSpace_DoesNotReturnColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(1 0 0)");

        // Assert - Should not return a color value
        Assert.NotEqual(ECssValueTypes.COLOR, value.Type);
    }

    #endregion

    #region color() Function - Out of Gamut Values

    [Fact]
    public void TryParseColor_Srgb_OutOfGamutPositive_ClampedTo255()
    {
        // Arrange & Act - values > 1 should be clamped
        var value = ParseColorValue("color(srgb 1.5 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R); // Clamped from 1.5 to 1.0
    }

    [Fact]
    public void TryParseColor_Srgb_OutOfGamutNegative_ClampedTo0()
    {
        // Arrange & Act - negative values should be clamped
        var value = ParseColorValue("color(srgb -0.5 0.5 0.5)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R); // Clamped from -0.5 to 0
    }

    #endregion

    #region color() Function - Case Insensitivity

    [Fact]
    public void TryParseColor_ColorSpaceNameCaseInsensitive_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(SRGB 1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    [Fact]
    public void TryParseColor_DisplayP3_MixedCase_ReturnsColor()
    {
        // Arrange & Act
        var value = ParseColorValue("color(Display-P3 1 0 0)");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
    }

    #endregion
}
