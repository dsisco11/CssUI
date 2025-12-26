using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS;

public class EnumRecordsIntegrationTests
{
    [Fact]
    public void EColor_TryFromKeyword_FindsTransparent()
    {
        var result = EColorExtensions.TryFromKeyword("transparent", out EColor? color);
        Assert.True(result, $"TryFromKeyword should find 'transparent'. Transparent keyword is: '{EColor.Transparent.Keyword()}'");
        Assert.Equal(EColor.Transparent, color);
    }

    [Fact]
    public void EColor_TryFromKeyword_FindsRed()
    {
        var result = EColorExtensions.TryFromKeyword("red", out EColor? color);
        Assert.True(result);
        Assert.Equal(EColor.Red, color);
    }

    [Fact]
    public void EColor_Keyword_ReturnsCorrectValue()
    {
        var keyword = EColor.Red.Keyword();
        Assert.Equal("red", keyword);
    }

    [Fact]
    public void EColor_Keyword_TransparentReturnsCorrectValue()
    {
        var keyword = EColor.Transparent.Keyword();
        Assert.Equal("transparent", keyword);
    }

    [Fact]
    public void EColor_RGB_ReturnsCorrectValues()
    {
        Assert.Equal(255, EColor.Red.R());
        Assert.Equal(0, EColor.Red.G());
        Assert.Equal(0, EColor.Red.B());
    }

    [Fact]
    public void EColor_RGB_TransparentReturnsZero()
    {
        // Transparent only has keyword, RGB should be default (0)
        Assert.Equal(0, EColor.Transparent.R());
        Assert.Equal(0, EColor.Transparent.G());
        Assert.Equal(0, EColor.Transparent.B());
    }
}
