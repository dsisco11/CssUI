using System;
using System.Globalization;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Value;

/// <summary>
/// Tests for <see cref="CssValue"/> ISpanFormattable and IFormattable implementations.
/// </summary>
public class CssValueFormattableTests
{
    #region Integer Formatting Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Integer_WritesValue()
    {
        var value = CssValue.From(42);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("42", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_NegativeInteger_WritesValue()
    {
        var value = CssValue.From(-100);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("-100", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Zero_WritesZero()
    {
        var value = CssValue.Zero;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("0", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Number Formatting Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Number_WritesValue()
    {
        var value = CssValue.From(3.14159);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        // Format uses "0.###" so max 3 decimal places
        Assert.Equal("3.142", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_WholeNumber_OmitsDecimal()
    {
        var value = CssValue.From(5.0);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("5", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_NegativeNumber_WritesValue()
    {
        var value = CssValue.From(-2.5);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("-2.5", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Percentage Formatting Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Percent50_WritesValue()
    {
        var value = CssValue.Percent_50;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("50%", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Percent100_WritesValue()
    {
        var value = CssValue.Percent_100;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("100%", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_FractionalPercent_WritesValue()
    {
        var value = CssValue.From_Percent(33.333);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("33.333%", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Dimension Formatting Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_PixelDimension_WritesValueWithUnit()
    {
        var value = CssValue.From_Dimension(100.0, ECssUnit.PX);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("100px", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_EmDimension_WritesValueWithUnit()
    {
        var value = CssValue.From_Dimension(1.5, ECssUnit.EM);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("1.5em", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_RemDimension_WritesValueWithUnit()
    {
        var value = CssValue.From_Dimension(2.0, ECssUnit.REM);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("2rem", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_DimensionNoUnit_WritesNoneMarker()
    {
        var value = CssValue.From_Dimension(10.0, ECssUnit.None);
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("10<none>", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Color Formatting Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Color_WritesRgbFormat()
    {
        var color = CssColor.FromRgba(255, 128, 64, 255);
        var value = CssValue.From(color);
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("rgb(255, 128, 64)", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_TransparentColor_WritesRgbaFormat()
    {
        var color = CssColor.FromRgba(255, 128, 64, 128);
        var value = CssValue.From(color);
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.StartsWith("rgba(255, 128, 64, ", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Keyword Formatting Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Auto_WritesKeyword()
    {
        var value = CssValue.Auto;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("auto", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_None_WritesKeyword()
    {
        var value = CssValue.None;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("none", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Inherit_WritesKeyword()
    {
        var value = CssValue.Inherit;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("inherit", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Initial_WritesKeyword()
    {
        var value = CssValue.Initial;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("initial", buffer[..charsWritten].ToString());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Unset_WritesKeyword()
    {
        var value = CssValue.Unset;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal("unset", buffer[..charsWritten].ToString());
    }

    #endregion

    #region Null Value Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_Null_WritesEmptyString()
    {
        var value = CssValue.Null;
        Span<char> buffer = stackalloc char[32];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal(0, charsWritten);
    }

    #endregion

    #region Buffer Size Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        var value = CssValue.From_Dimension(100.0, ECssUnit.PX); // "100px" = 5 chars
        Span<char> buffer = stackalloc char[3]; // Too small

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.False(success);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_ExactBufferSize_Succeeds()
    {
        var value = CssValue.From(10);
        var expected = "10";
        Span<char> buffer = stackalloc char[expected.Length];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        Assert.Equal(expected.Length, charsWritten);
    }

    #endregion

    #region IFormattable Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void ToString_WithFormatAndProvider_ReturnsStandardFormat()
    {
        var value = CssValue.From(42);

        // Format and provider are ignored for CSS values
        string result = value.ToString("G", CultureInfo.InvariantCulture);

        Assert.Equal("42", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void ToString_WithNullFormat_ReturnsStandardFormat()
    {
        var value = CssValue.From(3.14);

        string result = value.ToString(null, null);

        Assert.Equal("3.14", result);
    }

    #endregion

    #region Consistency Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Formatting")]
    public void TryFormat_ProducesCorrectCssSerialization_ForAllValueTypes()
    {
        // CSS-wide keywords should serialize as lowercase keywords
        var cssWideKeywords = new (CssValue value, string expected)[]
        {
            (CssValue.Auto, "auto"),
            (CssValue.None, "none"),
            (CssValue.Inherit, "inherit"),
            (CssValue.Initial, "initial"),
            (CssValue.Unset, "unset"),
        };

        foreach (var (value, expected) in cssWideKeywords)
        {
            Span<char> buffer = stackalloc char[128];
            bool success = value.TryFormat(buffer, out int charsWritten);

            Assert.True(success, $"TryFormat failed for value type: {value.Type}");
            Assert.Equal(expected, buffer[..charsWritten].ToString());
        }

        // Other value types should match ToString() output
        var otherValues = new[]
        {
            CssValue.Zero,
            CssValue.From(42),
            CssValue.From(-100),
            CssValue.From(3.14159),
            CssValue.From_Percent(50.0),
            CssValue.From_Dimension(100.0, ECssUnit.PX),
            CssValue.From_Dimension(1.5, ECssUnit.EM),
            CssValue.From(CssColor.FromRgba(255, 0, 0, 255)),
        };

        foreach (var value in otherValues)
        {
            Span<char> buffer = stackalloc char[128];
            bool success = value.TryFormat(buffer, out int charsWritten);

            Assert.True(success, $"TryFormat failed for value type: {value.Type}");
            Assert.Equal(value.ToString(), buffer[..charsWritten].ToString());
        }
    }

    #endregion
}
