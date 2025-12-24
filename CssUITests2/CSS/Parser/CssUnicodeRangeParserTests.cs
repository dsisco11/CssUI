using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS;

/// <summary>
/// Unit tests for <see cref="CssUnicodeRange"/> and <see cref="CssUnicodeRangeParser"/>.
/// </summary>
/// <remarks>
/// Tests cover the Unicode-Range microsyntax as defined in CSS Syntax Level 3 §7.
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#urange"/>
/// </remarks>
[Trait("Category", "CSS")]
[Trait("Category", "Parser")]
[Trait("Category", "UnicodeRange")]
public class CssUnicodeRangeParserTests
{
    #region Single Code Point Tests

    [Fact]
    public void Parse_SingleCodePoint_U0041_ReturnsCorrectRange()
    {
        // Arrange & Act
        var result = CssUnicodeRangeParser.Parse("U+0041");

        // Assert
        Assert.Equal(0x41, result.Start);
        Assert.Equal(0x41, result.End);
        Assert.True(result.IsSingleCodePoint);
    }

    [Fact]
    public void Parse_SingleCodePoint_U0001_ReturnsCorrectRange()
    {
        // Arrange & Act
        var result = CssUnicodeRangeParser.Parse("U+0001");

        // Assert
        Assert.Equal(1, result.Start);
        Assert.Equal(1, result.End);
    }

    [Fact]
    public void Parse_SingleCodePoint_Lowercase_u_Works()
    {
        // Arrange & Act
        var result = CssUnicodeRangeParser.Parse("u+0041");

        // Assert
        Assert.Equal(0x41, result.Start);
        Assert.Equal(0x41, result.End);
    }

    [Fact]
    public void Parse_SingleCodePoint_MaxCodePoint_Works()
    {
        // Arrange & Act
        var result = CssUnicodeRangeParser.Parse("U+10FFFF");

        // Assert
        Assert.Equal(0x10FFFF, result.Start);
        Assert.Equal(0x10FFFF, result.End);
    }

    [Fact]
    public void Parse_SingleCodePoint_MixedCaseHex_Works()
    {
        // Arrange & Act - Using a valid hex value within max code point
        var result = CssUnicodeRangeParser.Parse("U+AaBbC");

        // Assert - 0xAABBC = 699324 which is within max code point
        Assert.Equal(0xAABBC, result.Start);
        Assert.Equal(0xAABBC, result.End);
    }

    [Fact]
    public void Parse_SingleCodePoint_ShortForm_Works()
    {
        // Arrange & Act - single digit
        var result = CssUnicodeRangeParser.Parse("U+A");

        // Assert
        Assert.Equal(0xA, result.Start);
        Assert.Equal(0xA, result.End);
    }

    #endregion

    #region Range Tests (Start-End)

    [Fact]
    public void Parse_Range_U0001_00FF_ReturnsCorrectRange()
    {
        // Arrange & Act
        var result = CssUnicodeRangeParser.Parse("U+0001-00FF");

        // Assert
        Assert.Equal(0x0001, result.Start);
        Assert.Equal(0x00FF, result.End);
        Assert.False(result.IsSingleCodePoint);
    }

    [Fact]
    public void Parse_Range_BasicLatin_ReturnsCorrectRange()
    {
        // Arrange & Act - Basic Latin block
        var result = CssUnicodeRangeParser.Parse("U+0000-007F");

        // Assert
        Assert.Equal(0x0000, result.Start);
        Assert.Equal(0x007F, result.End);
        Assert.Equal(128, result.Count);
    }

    [Fact]
    public void Parse_Range_FullUnicode_ReturnsCorrectRange()
    {
        // Arrange & Act - Full Unicode range
        var result = CssUnicodeRangeParser.Parse("U+0-10FFFF");

        // Assert
        Assert.Equal(0x0, result.Start);
        Assert.Equal(0x10FFFF, result.End);
    }

    [Fact]
    public void Parse_Range_LowercaseHex_Works()
    {
        // Arrange & Act
        var result = CssUnicodeRangeParser.Parse("U+00aa-00ff");

        // Assert
        Assert.Equal(0x00AA, result.Start);
        Assert.Equal(0x00FF, result.End);
    }

    #endregion

    #region Wildcard Tests (?)

    [Fact]
    public void Parse_Wildcard_SingleQuestion_ReturnsRange()
    {
        // Arrange & Act - U+00?? means U+0000-00FF
        var result = CssUnicodeRangeParser.Parse("U+00??");

        // Assert
        Assert.Equal(0x0000, result.Start);
        Assert.Equal(0x00FF, result.End);
    }

    [Fact]
    public void Parse_Wildcard_FourQuestions_ReturnsFullRange()
    {
        // Arrange & Act - U+???? means U+0000-FFFF
        var result = CssUnicodeRangeParser.Parse("U+????");

        // Assert
        Assert.Equal(0x0000, result.Start);
        Assert.Equal(0xFFFF, result.End);
    }

    [Fact]
    public void Parse_Wildcard_ThreeHexOneQuestion_Works()
    {
        // Arrange & Act - U+030? means U+0300-030F
        var result = CssUnicodeRangeParser.Parse("U+030?");

        // Assert
        Assert.Equal(0x0300, result.Start);
        Assert.Equal(0x030F, result.End);
    }

    [Fact]
    public void Parse_Wildcard_AllQuestions_ExceedsMaxCodePoint_Throws()
    {
        // Arrange & Act - U+?????? means U+000000-FFFFFF but FFFFFF > 10FFFF (max code point)
        // This is an invalid range per the spec
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+??????"));
    }

    [Fact]
    public void Parse_Wildcard_FiveHexOneQuestion_Works()
    {
        // Arrange & Act - U+10FFF? means U+10FFF0-10FFFF
        var result = CssUnicodeRangeParser.Parse("U+10FFF?");

        // Assert
        Assert.Equal(0x10FFF0, result.Start);
        Assert.Equal(0x10FFFF, result.End);
    }

    #endregion

    #region Invalid Input Tests

    [Fact]
    public void Parse_Empty_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse(""));
    }

    [Fact]
    public void Parse_Whitespace_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("   "));
    }

    [Fact]
    public void Parse_MissingPlus_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U0041"));
    }

    [Fact]
    public void Parse_EndGreaterThanMax_ThrowsException()
    {
        // Arrange & Act & Assert - U+110000 exceeds max code point
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+110000"));
    }

    [Fact]
    public void Parse_StartGreaterThanEnd_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+00FF-0001"));
    }

    [Fact]
    public void Parse_MoreThanSixDigits_ThrowsException()
    {
        // Arrange & Act & Assert - 7 hex digits is invalid
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+1234567"));
    }

    [Fact]
    public void Parse_InvalidHexCharacter_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+00GG"));
    }

    [Fact]
    public void Parse_WildcardAfterRange_ThrowsException()
    {
        // Arrange & Act & Assert - mixing wildcards and ranges is invalid
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+00??-00FF"));
    }

    [Fact]
    public void Parse_OnlyUPlus_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<CssParserException>(() => CssUnicodeRangeParser.Parse("U+"));
    }

    #endregion

    #region TryParse Tests

    [Fact]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        // Arrange & Act
        bool success = CssUnicodeRangeParser.TryParse("U+0041", out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(0x41, result.Value.Start);
    }

    [Fact]
    public void TryParse_InvalidInput_ReturnsFalse()
    {
        // Arrange & Act
        bool success = CssUnicodeRangeParser.TryParse("invalid", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_Empty_ReturnsFalse()
    {
        // Arrange & Act
        bool success = CssUnicodeRangeParser.TryParse("", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region CssUnicodeRange Struct Tests

    [Fact]
    public void CssUnicodeRange_Constructor_ValidValues_Works()
    {
        // Arrange & Act
        var range = new CssUnicodeRange(0x0041, 0x005A);

        // Assert
        Assert.Equal(0x0041, range.Start);
        Assert.Equal(0x005A, range.End);
    }

    [Fact]
    public void CssUnicodeRange_SingleCodePoint_Works()
    {
        // Arrange & Act
        var range = CssUnicodeRange.SingleCodePoint(0x0041);

        // Assert
        Assert.Equal(0x0041, range.Start);
        Assert.Equal(0x0041, range.End);
        Assert.True(range.IsSingleCodePoint);
    }

    [Fact]
    public void CssUnicodeRange_Contains_InRange_ReturnsTrue()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x005A);

        // Act & Assert
        Assert.True(range.Contains(0x0041)); // Start
        Assert.True(range.Contains(0x004D)); // Middle
        Assert.True(range.Contains(0x005A)); // End
    }

    [Fact]
    public void CssUnicodeRange_Contains_OutOfRange_ReturnsFalse()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x005A);

        // Act & Assert
        Assert.False(range.Contains(0x0040)); // Before start
        Assert.False(range.Contains(0x005B)); // After end
    }

    [Fact]
    public void CssUnicodeRange_Count_ReturnsCorrectValue()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x005A);

        // Act & Assert
        Assert.Equal(26, range.Count); // A-Z = 26 characters
    }

    [Fact]
    public void CssUnicodeRange_Serialize_SingleCodePoint_Works()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x0041);

        // Act
        var serialized = range.Serialize();

        // Assert
        Assert.Equal("U+41", serialized);
    }

    [Fact]
    public void CssUnicodeRange_Serialize_Range_Works()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x005A);

        // Act
        var serialized = range.Serialize();

        // Assert
        Assert.Equal("U+41-5A", serialized);
    }

    [Fact]
    public void CssUnicodeRange_Equality_Works()
    {
        // Arrange
        var range1 = new CssUnicodeRange(0x0041, 0x005A);
        var range2 = new CssUnicodeRange(0x0041, 0x005A);
        var range3 = new CssUnicodeRange(0x0041, 0x005B);

        // Act & Assert
        Assert.Equal(range1, range2);
        Assert.NotEqual(range1, range3);
        Assert.True(range1 == range2);
        Assert.True(range1 != range3);
    }

    [Fact]
    public void CssUnicodeRange_Constructor_StartGreaterThanEnd_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new CssUnicodeRange(0x005A, 0x0041));
    }

    [Fact]
    public void CssUnicodeRange_Constructor_EndGreaterThanMax_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new CssUnicodeRange(0x0000, 0x110000));
    }

    [Fact]
    public void CssUnicodeRange_Constructor_NegativeStart_ThrowsException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new CssUnicodeRange(-1, 0x0041));
    }

    #endregion

    #region CssValue Integration Tests

    [Fact]
    public void CssValue_From_CssUnicodeRange_Works()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x005A);

        // Act
        var value = CssValue.From(range);

        // Assert
        Assert.Equal(ECssValueTypes.UNICODE_RANGE, value.Type);
    }

    [Fact]
    public void CssValue_AsUnicodeRange_Works()
    {
        // Arrange
        var range = new CssUnicodeRange(0x0041, 0x005A);
        var value = CssValue.From(range);

        // Act
        var result = value.AsUnicodeRange();

        // Assert
        Assert.Equal(range, result);
    }

    #endregion

    #region Real-World Use Case Tests

    [Fact]
    public void Parse_LatinSupplementBlock_Works()
    {
        // Latin-1 Supplement block: U+0080-00FF
        var result = CssUnicodeRangeParser.Parse("U+0080-00FF");

        Assert.Equal(0x0080, result.Start);
        Assert.Equal(0x00FF, result.End);
        Assert.Equal(128, result.Count);
    }

    [Fact]
    public void Parse_GreekBlock_Works()
    {
        // Greek and Coptic block: U+0370-03FF
        var result = CssUnicodeRangeParser.Parse("U+0370-03FF");

        Assert.Equal(0x0370, result.Start);
        Assert.Equal(0x03FF, result.End);
    }

    [Fact]
    public void Parse_CJKUnifiedIdeographs_Works()
    {
        // CJK Unified Ideographs block: U+4E00-9FFF
        var result = CssUnicodeRangeParser.Parse("U+4E00-9FFF");

        Assert.Equal(0x4E00, result.Start);
        Assert.Equal(0x9FFF, result.End);
    }

    [Fact]
    public void Parse_Emoji_Works()
    {
        // Some emoji are in U+1F600-1F64F
        var result = CssUnicodeRangeParser.Parse("U+1F600-1F64F");

        Assert.Equal(0x1F600, result.Start);
        Assert.Equal(0x1F64F, result.End);
    }

    #endregion
}
