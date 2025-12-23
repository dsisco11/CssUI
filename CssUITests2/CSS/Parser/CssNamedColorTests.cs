using CssUI;
using CssUI.CSS;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUI.Tests.CSS.Parser;

/// <summary>
/// Tests for CSS named color keyword parsing.
/// Spec Reference: https://www.w3.org/TR/css-color-4/#named-colors
/// </summary>
[Trait("Category", "CSS")]
[Trait("Category", "Color")]
public class CssNamedColorTests
{
    private CssValue ParseColorValue(string cssColorValue)
    {
        var parser = new CssParser(cssColorValue);
        return parser.Parse_CssValue();
    }

    #region CssColor.TryFromNamedColor Tests

    [Fact]
    public void TryFromNamedColor_Red_ReturnsCorrectColor()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("red", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_Blue_ReturnsCorrectColor()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("blue", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_Green_ReturnsCorrectColor()
    {
        // Note: CSS "green" is #008000, not pure green (#00FF00)
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("green", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(0, color.R);
        Assert.Equal(128, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_Lime_ReturnsCorrectColor()
    {
        // "lime" is pure green (#00FF00)
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("lime", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(0, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_Black_ReturnsCorrectColor()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("black", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_White_ReturnsCorrectColor()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("white", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_Transparent_ReturnsTransparentBlack()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("transparent", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    public void TryFromNamedColor_CurrentColor_ReturnsSpecialMarker()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("currentColor", out CssColor color, out bool isCurrentColor);

        // Assert
        Assert.True(result);
        Assert.True(isCurrentColor);
    }

    [Fact]
    public void TryFromNamedColor_CaseInsensitive_ReturnsCorrectColor()
    {
        // Arrange & Act
        var result1 = CssColor.TryFromNamedColor("RED", out CssColor color1);
        var result2 = CssColor.TryFromNamedColor("Red", out CssColor color2);
        var result3 = CssColor.TryFromNamedColor("rEd", out CssColor color3);

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.True(result3);
        Assert.Equal(color1, color2);
        Assert.Equal(color2, color3);
    }

    [Fact]
    public void TryFromNamedColor_InvalidKeyword_ReturnsFalse()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("notacolor", out CssColor color);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryFromNamedColor_EmptyString_ReturnsFalse()
    {
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("", out CssColor color);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryFromNamedColor_Aliceblue_ReturnsCorrectColor()
    {
        // aliceblue = #F0F8FF (240, 248, 255)
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("aliceblue", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(240, color.R);
        Assert.Equal(248, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void TryFromNamedColor_Rebeccapurple_ReturnsCorrectColor()
    {
        // rebeccapurple = #663399 (102, 51, 153) - added in CSS Color Level 4
        // Arrange & Act
        var result = CssColor.TryFromNamedColor("rebeccapurple", out CssColor color);

        // Assert
        Assert.True(result);
        Assert.Equal(102, color.R);
        Assert.Equal(51, color.G);
        Assert.Equal(153, color.B);
        Assert.Equal(255, color.A);
    }

    #endregion

    #region CssColor.FromEColor Tests

    [Fact]
    public void FromEColor_Red_ReturnsCorrectColor()
    {
        // Arrange & Act
        var color = CssColor.FromEColor(EColor.Red);

        // Assert
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void FromEColor_CurrentColor_ReturnsTransparent()
    {
        // currentColor needs context resolution, so it returns Transparent as a placeholder
        // Arrange & Act
        var color = CssColor.FromEColor(EColor.CurrentColor);

        // Assert
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    public void FromEColor_Transparent_ReturnsTransparentBlack()
    {
        // Arrange & Act
        var color = CssColor.FromEColor(EColor.Transparent);

        // Assert
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    #endregion

    #region CssParser Named Color Integration Tests

    [Fact]
    public void ParseCssValue_RedKeyword_ReturnsColorValue()
    {
        // Arrange & Act
        var value = ParseColorValue("red");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void ParseCssValue_BlueKeyword_ReturnsColorValue()
    {
        // Arrange & Act
        var value = ParseColorValue("blue");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void ParseCssValue_TransparentKeyword_ReturnsColorValue()
    {
        // Arrange & Act
        var value = ParseColorValue("transparent");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    public void ParseCssValue_CurrentColorKeyword_ReturnsKeywordValue()
    {
        // currentColor should be returned as a KEYWORD, not COLOR, since it needs cascade resolution
        // Arrange & Act
        var value = ParseColorValue("currentColor");

        // Assert
        Assert.Equal(ECssValueTypes.KEYWORD, value.Type);
        // Keywords are normalized to lowercase
        Assert.Equal("currentcolor", value.AsString());
    }

    [Fact]
    public void ParseCssValue_CaseInsensitiveColor_ReturnsColorValue()
    {
        // CSS color keywords are case-insensitive
        // Arrange & Act
        var value1 = ParseColorValue("RED");
        var value2 = ParseColorValue("Red");
        var value3 = ParseColorValue("rEd");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value1.Type);
        Assert.Equal(ECssValueTypes.COLOR, value2.Type);
        Assert.Equal(ECssValueTypes.COLOR, value3.Type);

        Assert.Equal(value1.AsCssColor(), value2.AsCssColor());
        Assert.Equal(value2.AsCssColor(), value3.AsCssColor());
    }

    [Fact]
    public void ParseCssValue_NonColorKeyword_ReturnsKeywordValue()
    {
        // Non-color keywords should remain as KEYWORD type
        // Arrange & Act
        var value = ParseColorValue("inherit");

        // Assert
        Assert.Equal(ECssValueTypes.KEYWORD, value.Type);
        Assert.Equal("inherit", value.AsString());
    }

    [Fact]
    public void ParseCssValue_Coral_ReturnsCorrectColor()
    {
        // coral = #FF7F50 (255, 127, 80)
        // Arrange & Act
        var value = ParseColorValue("coral");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(127, color.G);
        Assert.Equal(80, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void ParseCssValue_DodgerBlue_ReturnsCorrectColor()
    {
        // dodgerblue = #1E90FF (30, 144, 255)
        // Arrange & Act
        var value = ParseColorValue("dodgerblue");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(30, color.R);
        Assert.Equal(144, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void ParseCssValue_HotPink_ReturnsCorrectColor()
    {
        // hotpink = #FF69B4 (255, 105, 180)
        // Arrange & Act
        var value = ParseColorValue("hotpink");

        // Assert
        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsCssColor();
        Assert.Equal(255, color.R);
        Assert.Equal(105, color.G);
        Assert.Equal(180, color.B);
        Assert.Equal(255, color.A);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ParseCssValue_PartialMatchNotAColor_ReturnsKeyword()
    {
        // "redo" starts with "red" but is not a valid color
        // Arrange & Act
        var value = ParseColorValue("redo");

        // Assert
        Assert.Equal(ECssValueTypes.KEYWORD, value.Type);
    }

    [Fact]
    public void ParseCssValue_ColorWithExtraText_ParsesOnlyColor()
    {
        // Should parse "red" and leave the rest
        // Arrange & Act
        var parser = new CssParser("red 10px");
        var value1 = parser.Parse_CssValue();
        var value2 = parser.Parse_CssValue();

        // Assert - first value is color
        Assert.Equal(ECssValueTypes.COLOR, value1.Type);
        Assert.Equal(255, value1.AsCssColor().R);

        // Assert - second value is dimension
        Assert.Equal(ECssValueTypes.DIMENSION, value2.Type);
    }

    #endregion
}
