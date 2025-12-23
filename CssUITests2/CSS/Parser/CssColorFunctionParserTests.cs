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

    #endregion
}
