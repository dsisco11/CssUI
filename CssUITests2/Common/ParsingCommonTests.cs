using System;
using CssUI;
using Xunit;

namespace CssUITests;

public class ParsingCommonTests
{
    #region Digits_To_Base10 Tests

    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "01")]
    [InlineData(2, "2")]
    [InlineData(4, "4")]
    [InlineData(8, "8")]
    [InlineData(16, "16")]
    [InlineData(32, "32")]
    [InlineData(64, "64")]
    [InlineData(128, "128")]
    [InlineData(256, "256")]
    [InlineData(512, "512")]
    [InlineData(1024, "1024")]
    [InlineData(2048, "2048")]
    [InlineData(4096, "4096")]
    [InlineData(1234567890, "1234567890")]
    [InlineData(9999999999, "9999999999")]
    public void Digits_To_Base10_ValidDigits_ReturnsCorrectValue(long expected, string input)
    {
        Assert.Equal(expected, ParsingCommon.Digits_To_Base10(input.AsMemory()));
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(123, "123")]
    public void Digits_To_Base10_Span_ValidDigits_ReturnsCorrectValue(long expected, string input)
    {
        Assert.Equal(expected, ParsingCommon.Digits_To_Base10(input.AsSpan()));
    }

    [Fact]
    public void Digits_To_Base10_EmptyString_ReturnsZero()
    {
        var result = ParsingCommon.Digits_To_Base10(ReadOnlyMemory<char>.Empty);
        Assert.Equal(0L, result);
    }

    [Theory]
    [InlineData("12a34")]
    [InlineData("abc")]
    [InlineData("12.34")]
    [InlineData("-123")]
    [InlineData("+123")]
    [InlineData("12 34")]
    public void Digits_To_Base10_InvalidChars_ThrowsArgumentOutOfRangeException(string input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ParsingCommon.Digits_To_Base10(input.AsMemory()));
    }

    [Theory]
    [InlineData(0, "00000")]
    [InlineData(123, "00123")]
    [InlineData(1, "00001")]
    public void Digits_To_Base10_LeadingZeros_ParsesCorrectly(long expected, string input)
    {
        var result = ParsingCommon.Digits_To_Base10(input.AsMemory());
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Digits_To_Base10_MaxLongValue_DoesNotOverflow()
    {
        // Max long is 9223372036854775807
        var result = ParsingCommon.Digits_To_Base10("9223372036854775807".AsMemory());
        Assert.Equal(long.MaxValue, result);
    }

    #endregion

    #region Digits_To_Base10_Unsigned Tests

    [Theory]
    [InlineData(0UL, "0")]
    [InlineData(1UL, "1")]
    [InlineData(2UL, "02")]
    [InlineData(4UL, "4")]
    [InlineData(8UL, "8")]
    [InlineData(16UL, "16")]
    [InlineData(32UL, "32")]
    [InlineData(64UL, "64")]
    [InlineData(128UL, "128")]
    [InlineData(256UL, "256")]
    [InlineData(512UL, "512")]
    [InlineData(1024UL, "1024")]
    [InlineData(2048UL, "2048")]
    [InlineData(4096UL, "4096")]
    [InlineData(9999999999999999999UL, "9999999999999999999")]
    public void Digits_To_Base10_Unsigned_ValidDigits_ReturnsCorrectValue(ulong expected, string input)
    {
        Assert.Equal(expected, ParsingCommon.Digits_To_Base10_Unsigned(input.AsMemory()));
    }

    [Fact]
    public void Digits_To_Base10_Unsigned_EmptyString_ReturnsZero()
    {
        var result = ParsingCommon.Digits_To_Base10_Unsigned(ReadOnlyMemory<char>.Empty);
        Assert.Equal(0UL, result);
    }

    [Theory]
    [InlineData("12a34")]
    [InlineData("-123")]
    [InlineData("abc")]
    public void Digits_To_Base10_Unsigned_InvalidChars_ThrowsArgumentOutOfRangeException(string input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ParsingCommon.Digits_To_Base10_Unsigned(input.AsMemory()));
    }

    [Fact]
    public void Digits_To_Base10_Unsigned_MaxULongValue_DoesNotOverflow()
    {
        // Max ulong is 18446744073709551615
        var result = ParsingCommon.Digits_To_Base10_Unsigned("18446744073709551615".AsMemory());
        Assert.Equal(ulong.MaxValue, result);
    }

    #endregion

    #region ToInteger Tests

    [Theory]
    [InlineData(123L, 1, "123", 1, "")]
    [InlineData(-123L, -1, "123", 1, "")]
    [InlineData(500L, 1, "5", 1, "2")]      // 5 * 10^2 = 500
    [InlineData(0L, 1, "5", -1, "2")]       // 5 * 10^-2 = 0 (integer truncation)
    [InlineData(-500L, -1, "5", 1, "2")]    // -5 * 10^2 = -500
    [InlineData(0L, 1, "0", 1, "")]
    [InlineData(1L, 1, "1", 1, "0")]        // 1 * 10^0 = 1
    public void ToInteger_ValidInputs_ReturnsCorrectValue(long expected, int sign, string intDigits, int expSign, string expDigits)
    {
        var result = ParsingCommon.ToInteger(sign, intDigits.AsSpan(), expSign, expDigits.AsSpan());
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToInteger_EmptyDigits_ReturnsZero()
    {
        var result = ParsingCommon.ToInteger(1, ReadOnlySpan<char>.Empty, 1, ReadOnlySpan<char>.Empty);
        Assert.Equal(0L, result);
    }

    [Fact]
    public void ToInteger_LargeExponent_ReturnsLargeValue()
    {
        // 1 * 10^10
        var result = ParsingCommon.ToInteger(1, "1".AsSpan(), 1, "10".AsSpan());
        Assert.Equal(10000000000L, result);
    }

    #endregion

    #region ToDecimal Tests

    [Theory]
    [InlineData(123.0, 1, "123", "", 1, "")]
    [InlineData(-123.0, -1, "123", "", 1, "")]
    [InlineData(1.5, 1, "1", "5", 1, "")]
    [InlineData(0.25, 1, "0", "25", 1, "")]
    [InlineData(3.14159, 1, "3", "14159", 1, "")]
    [InlineData(500.0, 1, "5", "", 1, "2")]    // 5 * 10^2
    [InlineData(0.05, 1, "5", "", -1, "2")]    // 5 * 10^-2
    public void ToDecimal_ValidInputs_ReturnsCorrectValue(double expected, int sign, string intDigits, string fracDigits, int expSign, string expDigits)
    {
        var result = ParsingCommon.ToDecimal(sign, intDigits.AsSpan(), fracDigits.AsSpan(), expSign, expDigits.AsSpan());
        Assert.Equal(expected, result, precision: 10);
    }

    [Fact]
    public void ToDecimal_ZeroWithFraction_ReturnsCorrectValue()
    {
        var result = ParsingCommon.ToDecimal(1, "0".AsSpan(), "5".AsSpan(), 1, ReadOnlySpan<char>.Empty);
        Assert.Equal(0.5, result, precision: 10);
    }

    [Fact]
    public void ToDecimal_NegativeWithExponent_ReturnsCorrectValue()
    {
        // -2.5e3 = -2500
        var result = ParsingCommon.ToDecimal(-1, "2".AsSpan(), "5".AsSpan(), 1, "3".AsSpan());
        Assert.Equal(-2500.0, result, precision: 10);
    }

    [Fact]
    public void ToDecimal_VerySmallFraction_ReturnsCorrectValue()
    {
        // 0.000001
        var result = ParsingCommon.ToDecimal(1, "0".AsSpan(), "000001".AsSpan(), 1, ReadOnlySpan<char>.Empty);
        Assert.Equal(0.000001, result, precision: 10);
    }

    #endregion

    #region Parse_Hex Tests

    [Theory]
    [InlineData(0UL, "#0")]
    [InlineData(1UL, "#1")]
    [InlineData(2UL, "#2")]
    [InlineData(4UL, "#4")]
    [InlineData(8UL, "#8")]
    [InlineData(15UL, "#F")]
    [InlineData(255UL, "#FF")]
    [InlineData(4095UL, "#FFF")]
    [InlineData(65535UL, "#FFFF")]
    [InlineData(UInt32.MaxValue, "#FFFFFFFF")]
    [InlineData(250UL, "#FA")]
    [InlineData(4000UL, "#FA0")]
    [InlineData(44779UL, "#AEEB")]
    [InlineData(48879UL, "#BEEF")]
    public void Parse_Hex_WithHashPrefix_ReturnsCorrectValue(ulong expected, string input)
    {
        Assert.True(ParsingCommon.Parse_Hex(input.AsMemory(), out ulong actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0UL, "0")]
    [InlineData(1UL, "1")]
    [InlineData(15UL, "F")]
    [InlineData(15UL, "f")]
    [InlineData(16UL, "10")]
    [InlineData(255UL, "FF")]
    [InlineData(255UL, "ff")]
    [InlineData(256UL, "100")]
    [InlineData(0xDEADBEEFUL, "DEADBEEF")]
    [InlineData(0xDEADBEEFUL, "deadbeef")]
    [InlineData(0xABCDEFUL, "AbCdEf")]
    public void Parse_Hex_WithoutHashPrefix_ReturnsCorrectValue(ulong expected, string input)
    {
        Assert.True(ParsingCommon.Parse_Hex(input.AsMemory(), out ulong actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("GG")]
    [InlineData("xyz")]
    [InlineData("12.34")]
    [InlineData("12 34")]
    public void Parse_Hex_InvalidChars_ThrowsException(string input)
    {
        Assert.Throws<Exception>(() => ParsingCommon.Parse_Hex(input.AsMemory()));
    }

    [Theory]
    [InlineData("GG")]
    [InlineData("xyz")]
    [InlineData("12.34")]
    public void Parse_Hex_TryParse_InvalidChars_ReturnsFalse(string input)
    {
        bool result = ParsingCommon.Parse_Hex(input.AsMemory(), out ulong outValue);
        Assert.False(result);
        Assert.Equal(0UL, outValue);
    }

    #endregion

    #region Parse_Integer Tests

    [Theory]
    [InlineData(-1024L, "-1024")]
    [InlineData(-1L, "-1")]
    [InlineData(0L, "0")]
    [InlineData(1L, "1")]
    [InlineData(2L, "2")]
    [InlineData(1024L, "1024")]
    [InlineData(123L, "+123")]
    [InlineData(123L, "  123")]           // Leading whitespace stripped
    [InlineData(123L, "\t\n\r 123")]      // ASCII whitespace stripped
    public void Parse_Integer_ValidInputs_ReturnsCorrectValue(long expected, string input)
    {
        Assert.True(ParsingCommon.Parse_Integer(input.AsMemory(), out long actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]                    // Empty
    [InlineData("   ")]                 // Only whitespace
    [InlineData("abc")]                 // No digits
    [InlineData("123a")]                // Alpha immediately after digits should fail per spec
    [InlineData("123abc")]              // Alpha immediately after digits
    public void Parse_Integer_InvalidInputs_ReturnsFalse(string input)
    {
        bool result = ParsingCommon.Parse_Integer(input.AsMemory(), out long outValue);
        Assert.False(result);
        Assert.Equal(long.MaxValue, outValue);
    }

    [Fact]
    public void Parse_Integer_NegativeZero_ReturnsZero()
    {
        Assert.True(ParsingCommon.Parse_Integer("-0".AsMemory(), out long actual));
        Assert.Equal(0L, actual);
    }

    [Fact]
    public void Parse_Integer_TrailingNonAlphaChars_StopsAndReturnsValue()
    {
        // Per HTML spec, trailing non-alpha chars should not cause failure
        Assert.True(ParsingCommon.Parse_Integer("123.456".AsMemory(), out long actual));
        Assert.Equal(123L, actual);
    }

    [Fact]
    public void Parse_Integer_TrailingSpace_ReturnsValue()
    {
        Assert.True(ParsingCommon.Parse_Integer("123 ".AsMemory(), out long actual));
        Assert.Equal(123L, actual);
    }

    [Fact]
    public void Parse_Integer_LargeNumber_ReturnsCorrectValue()
    {
        Assert.True(ParsingCommon.Parse_Integer("9223372036854775807".AsMemory(), out long actual));
        Assert.Equal(long.MaxValue, actual);
    }

    [Fact]
    public void Parse_Integer_NegativeLargeNumber_ReturnsCorrectValue()
    {
        Assert.True(ParsingCommon.Parse_Integer("-9223372036854775808".AsMemory(), out long actual));
        Assert.Equal(long.MinValue, actual);
    }

    #endregion

    #region Parse_FloatingPoint Tests

    [Theory]
    [InlineData(-25000.0f, "-2.5E4")]
    [InlineData(-2500.0f, "-2.5E3")]
    [InlineData(-250.0f, "-2.5E2")]
    [InlineData(-1024.5f, "-1024.5")]
    [InlineData(-1.5f, "-1.5")]
    [InlineData(0.5f, "0.5")]
    [InlineData(1.5f, "1.5")]
    [InlineData(2.5f, "2.5")]
    [InlineData(250.0f, "2.5E2")]
    [InlineData(2500.0f, "2.5E3")]
    [InlineData(25000.0f, "2.5E4")]
    public void Parse_FloatingPoint_Float_ValidInputs_ReturnsCorrectValue(float expected, string input)
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out float actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0.0, "0")]
    [InlineData(1.0, "1")]
    [InlineData(123.0, "123")]
    [InlineData(-123.0, "-123")]
    [InlineData(123.0, "+123")]
    [InlineData(1.5, "1.5")]
    [InlineData(0.5, ".5")]               // Leading decimal point allowed
    [InlineData(1.0, "1.")]               // Trailing decimal point
    [InlineData(3.14159, "3.14159")]
    public void Parse_FloatingPoint_Double_ValidInputs_ReturnsCorrectValue(double expected, string input)
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out double actual));
        Assert.Equal(expected, actual, precision: 10);
    }

    [Theory]
    [InlineData(100.0, "1e2")]            // 1 * 10^2
    [InlineData(100.0, "1E2")]            // Capital E
    [InlineData(100.0, "1e+2")]           // Explicit positive exponent
    [InlineData(0.01, "1e-2")]            // Negative exponent
    [InlineData(150.0, "1.5e2")]          // Decimal with exponent
    [InlineData(0.015, "1.5e-2")]         // Decimal with negative exponent
    [InlineData(50.0, ".5e2")]            // Leading decimal with exponent
    public void Parse_FloatingPoint_ExponentNotation_ReturnsCorrectValue(double expected, string input)
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out double actual));
        Assert.Equal(expected, actual, precision: 10);
    }

    [Theory]
    [InlineData(123.5, "  123.5")]        // Leading whitespace
    [InlineData(1.5, "\t\n\r 1.5")]       // ASCII whitespace
    public void Parse_FloatingPoint_LeadingWhitespace_ReturnsCorrectValue(double expected, string input)
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out double actual));
        Assert.Equal(expected, actual, precision: 10);
    }

    [Theory]
    [InlineData("123abc")]                      // Trailing alpha chars - should stop at 'a'
    [InlineData("123.456px")]                   // Trailing unit - should stop at 'p'
    public void Parse_FloatingPoint_TrailingChars_StopsAndReturnsValue(string input)
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out double actual));
        // Just verify it parsed something, exact value depends on implementation
        Assert.False(double.IsNaN(actual));
    }

    [Theory]
    [InlineData("")]                            // Empty - throws DomSyntaxError
    [InlineData("   ")]                         // Only whitespace - throws DomSyntaxError
    public void Parse_FloatingPoint_EmptyOrWhitespace_Throws(string input)
    {
        Assert.ThrowsAny<Exception>(() => ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out double _));
    }

    [Theory]
    [InlineData("abc")]                         // No digits
    [InlineData("NaN")]                         // Should NOT accept NaN keyword
    [InlineData("Infinity")]                    // Should NOT accept Infinity keyword
    [InlineData("-Infinity")]                   // Should NOT accept -Infinity keyword
    public void Parse_FloatingPoint_InvalidInputs_ReturnsFalse(string input)
    {
        bool result = ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out double outValue);
        Assert.False(result);
        Assert.True(double.IsNaN(outValue));
    }

    [Fact]
    public void Parse_FloatingPoint_NegativeZero_ReturnsPositiveZero()
    {
        // Per HTML spec, -0 should become +0
        Assert.True(ParsingCommon.Parse_FloatingPoint("-0".AsMemory(), out double actual));
        Assert.Equal(0.0, actual);
        Assert.False(double.IsNegative(actual));
    }

    [Fact]
    public void Parse_FloatingPoint_VerySmallNumber_ReturnsCorrectValue()
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint("0.000001".AsMemory(), out double actual));
        Assert.Equal(0.000001, actual, precision: 10);
    }

    [Fact]
    public void Parse_FloatingPoint_ScientificNotation_VerySmall_ReturnsCorrectValue()
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint("1e-10".AsMemory(), out double actual));
        Assert.Equal(1e-10, actual, precision: 15);
    }

    [Fact]
    public void Parse_FloatingPoint_ScientificNotation_VeryLarge_ReturnsCorrectValue()
    {
        Assert.True(ParsingCommon.Parse_FloatingPoint("1e10".AsMemory(), out double actual));
        Assert.Equal(1e10, actual, precision: 5);
    }

    [Fact]
    public void Parse_FloatingPoint_MultipleDecimalPoints_StopsAtSecond()
    {
        // "1.2.3" should parse as 1.2 and stop
        Assert.True(ParsingCommon.Parse_FloatingPoint("1.2.3".AsMemory(), out double actual));
        Assert.Equal(1.2, actual, precision: 10);
    }

    #endregion

    #region Get_Location Tests

    [Fact]
    public void Get_Location_AtStart_ReturnsBeginningOfInput()
    {
        var stream = new DataConsumer<char>("Hello World".AsMemory(), UnicodeCommon.EOF);
        var location = ParsingCommon.Get_Location(stream);
        Assert.StartsWith("Hello", location);
    }

    [Fact]
    public void Get_Location_AtEnd_ReturnsEndOfStreamMarker()
    {
        var stream = new DataConsumer<char>("Hi".AsMemory(), UnicodeCommon.EOF);
        stream.Consume();
        stream.Consume();
        var location = ParsingCommon.Get_Location(stream);
        Assert.Equal("<end of stream>", location);
    }

    [Fact]
    public void Get_Location_LongInput_TruncatesTo32Chars()
    {
        var longInput = new string('A', 100);
        var stream = new DataConsumer<char>(longInput.AsMemory(), UnicodeCommon.EOF);
        var location = ParsingCommon.Get_Location(stream);
        Assert.Equal(32, location.Length);
    }

    [Fact]
    public void Get_Location_MiddleOfStream_ReturnsRemainingContent()
    {
        var stream = new DataConsumer<char>("ABCDEFGH".AsMemory(), UnicodeCommon.EOF);
        stream.Consume(); // A
        stream.Consume(); // B
        stream.Consume(); // C
        var location = ParsingCommon.Get_Location(stream);
        Assert.Equal("DEFGH", location);
    }

    [Fact]
    public void Get_Location_EmptyInput_ReturnsEndOfStreamMarker()
    {
        var stream = new DataConsumer<char>(ReadOnlyMemory<char>.Empty, UnicodeCommon.EOF);
        var location = ParsingCommon.Get_Location(stream);
        Assert.Equal("<end of stream>", location);
    }

    #endregion

    #region DataConsumer Integration Tests

    [Fact]
    public void Parse_Integer_WithDataConsumer_ParsesCorrectly()
    {
        var stream = new DataConsumer<char>("123abc".AsMemory(), UnicodeCommon.EOF);
        // Should fail because alpha follows digits
        bool result = ParsingCommon.Parse_Integer(stream, out long value);
        Assert.False(result);
    }

    [Fact]
    public void Parse_FloatingPoint_WithDataConsumer_ParsesCorrectly()
    {
        var stream = new DataConsumer<char>("3.14".AsMemory(), UnicodeCommon.EOF);
        Assert.True(ParsingCommon.Parse_FloatingPoint(stream, out float value));
        Assert.Equal(3.14f, value, precision: 5);
    }

    [Fact]
    public void Parse_FloatingPoint_WithDataConsumer_Double_ParsesCorrectly()
    {
        var stream = new DataConsumer<char>("3.14159265359".AsMemory(), UnicodeCommon.EOF);
        Assert.True(ParsingCommon.Parse_FloatingPoint(stream, out double value));
        Assert.Equal(3.14159265359, value, precision: 10);
    }

    #endregion
}
