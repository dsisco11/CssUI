using System;
using System.Globalization;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Types;

/// <summary>
/// Tests for <see cref="CssColor"/> ISpanFormattable, IFormattable, IParsable, and ISpanParsable implementations.
/// </summary>
public class CssColorFormattableTests
{
    #region ISpanFormattable Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_OpaqueBlack_WritesRgb()
    {
        var color = CssColor.FromRgba(0, 0, 0, 255);
        Span<char> buffer = stackalloc char[32];

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("rgb(0, 0, 0)", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_OpaqueWhite_WritesRgb()
    {
        var color = CssColor.FromRgba(255, 255, 255, 255);
        Span<char> buffer = stackalloc char[32];

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("rgb(255, 255, 255)", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_SemiTransparent_WritesRgba()
    {
        var color = CssColor.FromRgba(255, 0, 0, 128);
        Span<char> buffer = stackalloc char[32];

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        var result = buffer[..charsWritten].ToString();
        Assert.StartsWith("rgba(255, 0, 0, ", result);
        Assert.EndsWith(")", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_FullyTransparent_WritesRgbaWithZeroAlpha()
    {
        var color = CssColor.FromRgba(100, 150, 200, 0);
        Span<char> buffer = stackalloc char[32];

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("rgba(100, 150, 200, 0)", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        var color = CssColor.FromRgba(255, 255, 255, 255);
        Span<char> buffer = stackalloc char[5]; // Too small for "rgb(255, 255, 255)"

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_ExactBufferSize_Succeeds()
    {
        var color = CssColor.FromRgba(0, 0, 0, 255);
        var expected = "rgb(0, 0, 0)";
        Span<char> buffer = stackalloc char[expected.Length];

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal(expected.Length, charsWritten);
        Assert.Equal(expected, buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void TryFormat_MatchesToString()
    {
        var colors = new[]
        {
            CssColor.Black,
            CssColor.White,
            CssColor.Transparent,
            CssColor.FromRgba(128, 64, 32, 255),
            CssColor.FromRgba(128, 64, 32, 128),
            CssColor.FromRgba(0, 255, 0, 1),
        };

        foreach (var color in colors)
        {
            Span<char> buffer = stackalloc char[64];
            bool success = color.TryFormat(buffer, out int charsWritten);

            Assert.True(success);
            Assert.Equal(color.ToString(), buffer[..charsWritten].ToString());
        }
    }

    #endregion

    #region IFormattable Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void ToString_WithFormatAndProvider_ReturnsStandardFormat()
    {
        var color = CssColor.FromRgba(255, 128, 64, 255);

        // Format and provider are ignored for CSS colors
        string result = color.ToString("X", CultureInfo.InvariantCulture);

        Assert.Equal("rgb(255, 128, 64)", result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    public void ToString_WithNullFormat_ReturnsStandardFormat()
    {
        var color = CssColor.FromRgba(255, 128, 64, 255);

        string result = color.ToString(null, null);

        Assert.Equal("rgb(255, 128, 64)", result);
    }

    #endregion

    #region IParsable Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_HexRgb_ReturnsColor()
    {
        var result = CssColor.Parse("#ff0000", null);

        Assert.Equal(255, result.R);
        Assert.Equal(0, result.G);
        Assert.Equal(0, result.B);
        Assert.Equal(255, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_HexRgba_ReturnsColor()
    {
        var result = CssColor.Parse("#ff000080", null);

        Assert.Equal(255, result.R);
        Assert.Equal(0, result.G);
        Assert.Equal(0, result.B);
        Assert.Equal(128, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_ShortHex_ReturnsColor()
    {
        var result = CssColor.Parse("#f00", null);

        Assert.Equal(255, result.R);
        Assert.Equal(0, result.G);
        Assert.Equal(0, result.B);
        Assert.Equal(255, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_NamedColor_ReturnsColor()
    {
        var result = CssColor.Parse("red", null);

        Assert.Equal(255, result.R);
        Assert.Equal(0, result.G);
        Assert.Equal(0, result.B);
        Assert.Equal(255, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_InvalidFormat_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => CssColor.Parse("invalid", null));
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_NullString_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => CssColor.Parse(null!, null));
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_EmptyString_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => CssColor.Parse("", null));
    }

    #endregion

    #region TryParse Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_HexRgb_ReturnsTrue()
    {
        bool success = CssColor.TryParse("#00ff00", null, out var result);

        Assert.True(success);
        Assert.Equal(0, result.R);
        Assert.Equal(255, result.G);
        Assert.Equal(0, result.B);
        Assert.Equal(255, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_NamedColor_ReturnsTrue()
    {
        bool success = CssColor.TryParse("blue", null, out var result);

        Assert.True(success);
        Assert.Equal(0, result.R);
        Assert.Equal(0, result.G);
        Assert.Equal(255, result.B);
        Assert.Equal(255, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_Transparent_ReturnsTrue()
    {
        bool success = CssColor.TryParse("transparent", null, out var result);

        Assert.True(success);
        Assert.Equal(0, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_InvalidFormat_ReturnsFalse()
    {
        bool success = CssColor.TryParse("not-a-color", null, out var result);

        Assert.False(success);
        Assert.Equal(CssColor.Transparent, result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_NullString_ReturnsFalse()
    {
        bool success = CssColor.TryParse(null, null, out var result);

        Assert.False(success);
        Assert.Equal(CssColor.Transparent, result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        bool success = CssColor.TryParse("", null, out var result);

        Assert.False(success);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_WhitespaceString_ReturnsFalse()
    {
        bool success = CssColor.TryParse("   ", null, out var result);

        Assert.False(success);
    }

    #endregion

    #region ISpanParsable Tests

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void Parse_Span_HexRgb_ReturnsColor()
    {
        ReadOnlySpan<char> input = "#0000ff";
        var result = CssColor.Parse(input, null);

        Assert.Equal(0, result.R);
        Assert.Equal(0, result.G);
        Assert.Equal(255, result.B);
        Assert.Equal(255, result.A);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_Span_HexRgb_ReturnsTrue()
    {
        ReadOnlySpan<char> input = "#ffffff";
        bool success = CssColor.TryParse(input, null, out var result);

        Assert.True(success);
        Assert.Equal(CssColor.White, result);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_Span_WithLeadingWhitespace_Succeeds()
    {
        ReadOnlySpan<char> input = "  #ff0000";
        bool success = CssColor.TryParse(input, null, out var result);

        Assert.True(success);
        Assert.Equal(255, result.R);
    }

    [Fact]
    [Trait("Category", "Color")]
    [Trait("Category", "Parsing")]
    public void TryParse_Span_WithTrailingWhitespace_Succeeds()
    {
        ReadOnlySpan<char> input = "#ff0000  ";
        bool success = CssColor.TryParse(input, null, out var result);

        Assert.True(success);
        Assert.Equal(255, result.R);
    }

    #endregion

    #region Alpha Serialization Tests

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Formatting")]
    [InlineData(255, "1")]         // Full opacity → "1"
    [InlineData(0, "0")]           // Zero opacity → "0"
    [InlineData(128, "0.5")]       // 50% → "0.5" (128 rounds from 50*2.55=127.5)
    public void TryFormat_AlphaValues_FormatsCorrectly(byte alpha, string expectedAlphaInOutput)
    {
        var color = CssColor.FromRgba(100, 100, 100, alpha);
        Span<char> buffer = stackalloc char[64];

        bool success = color.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        var result = buffer[..charsWritten].ToString();

        if (alpha == 255)
        {
            Assert.StartsWith("rgb(", result);
        }
        else
        {
            Assert.Contains(expectedAlphaInOutput, result);
        }
    }

    #endregion

    #region Roundtrip Tests

    [Theory]
    [Trait("Category", "Color")]
    [Trait("Category", "Roundtrip")]
    [InlineData("#000000")]
    [InlineData("#ffffff")]
    [InlineData("#ff0000")]
    [InlineData("#00ff00")]
    [InlineData("#0000ff")]
    [InlineData("#123456")]
    [InlineData("#abcdef")]
    public void Roundtrip_HexColors_PreservesValues(string hexInput)
    {
        // Parse the hex color
        var parsed = CssColor.Parse(hexInput, null);

        // Format back to string (will be rgb() format)
        Span<char> buffer = stackalloc char[64];
        bool success = parsed.TryFormat(buffer, out int charsWritten);
        Assert.True(success);

        // Verify the RGB values match
        Assert.Equal(parsed.ToString(), buffer[..charsWritten].ToString());
    }

    #endregion
}
