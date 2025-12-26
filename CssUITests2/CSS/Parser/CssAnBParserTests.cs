#pragma warning disable CS0618 // Tests intentionally use obsolete CssAnBParser methods
using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS;

/// <summary>
/// Unit tests for <see cref="CssAnB"/> and <see cref="CssAnBParser"/>.
/// </summary>
/// <remarks>
/// Tests cover the An+B microsyntax as defined in CSS Syntax Level 3 §6.
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#anb-microsyntax"/>
/// </remarks>
[Trait("Category", "CSS")]
[Trait("Category", "Parser")]
[Trait("Category", "AnB")]
public class CssAnBParserTests
{
    #region Keyword Tests (odd/even)

    [Fact]
    public void Parse_Odd_Returns_2n_Plus_1()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("odd");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(1, result.B);
    }

    [Fact]
    public void Parse_Even_Returns_2n()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("even");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_ODD_CaseInsensitive_Returns_2n_Plus_1()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("ODD");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(1, result.B);
    }

    [Fact]
    public void Parse_EVEN_CaseInsensitive_Returns_2n()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("EVEN");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(0, result.B);
    }

    #endregion

    #region Integer-Only Tests (A=0)

    [Fact]
    public void Parse_PositiveInteger_Returns_A0_BValue()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("5");

        // Assert
        Assert.Equal(0, result.A);
        Assert.Equal(5, result.B);
    }

    [Fact]
    public void Parse_Zero_Returns_A0_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("0");

        // Assert
        Assert.Equal(0, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_NegativeInteger_Returns_A0_NegativeB()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("-3");

        // Assert
        Assert.Equal(0, result.A);
        Assert.Equal(-3, result.B);
    }

    #endregion

    #region n-Only Tests (B=0)

    [Fact]
    public void Parse_n_Returns_A1_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("n");

        // Assert
        Assert.Equal(1, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_MinusN_Returns_AMinus1_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("-n");

        // Assert
        Assert.Equal(-1, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_PlusN_Returns_A1_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("+n");

        // Assert
        Assert.Equal(1, result.A);
        Assert.Equal(0, result.B);
    }

    #endregion

    #region Dimension-Based Tests (An)

    [Fact]
    public void Parse_2n_Returns_A2_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("2n");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_Minus3n_Returns_AMinus3_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("-3n");

        // Assert
        Assert.Equal(-3, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_Plus4n_Returns_A4_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("+4n");

        // Assert
        Assert.Equal(4, result.A);
        Assert.Equal(0, result.B);
    }

    #endregion

    #region An+B Tests (Full Syntax)

    [Fact]
    public void Parse_2nPlus1_Returns_A2_B1()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("2n+1");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(1, result.B);
    }

    [Fact]
    public void Parse_3nMinus2_Returns_A3_BMinus2()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("3n-2");

        // Assert
        Assert.Equal(3, result.A);
        Assert.Equal(-2, result.B);
    }

    [Fact]
    public void Parse_nPlus5_Returns_A1_B5()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("n+5");

        // Assert
        Assert.Equal(1, result.A);
        Assert.Equal(5, result.B);
    }

    [Fact]
    public void Parse_MinusNPlus6_Returns_AMinus1_B6()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("-n+6");

        // Assert
        Assert.Equal(-1, result.A);
        Assert.Equal(6, result.B);
    }

    [Fact]
    public void Parse_Minus1nPlus6_Returns_AMinus1_B6()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("-1n+6");

        // Assert
        Assert.Equal(-1, result.A);
        Assert.Equal(6, result.B);
    }

    #endregion

    #region Whitespace Tests

    [Fact]
    public void Parse_WithWhitespaceAroundSign_3n_Plus_1_Returns_A3_B1()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("3n + 1");

        // Assert
        Assert.Equal(3, result.A);
        Assert.Equal(1, result.B);
    }

    [Fact]
    public void Parse_WithWhitespaceAroundSign_3n_Minus_2_Returns_A3_BMinus2()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("3n - 2");

        // Assert
        Assert.Equal(3, result.A);
        Assert.Equal(-2, result.B);
    }

    [Fact]
    public void Parse_WithLeadingWhitespace_Returns_Correct()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("  2n+1");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(1, result.B);
    }

    [Fact]
    public void Parse_WithTrailingWhitespace_Returns_Correct()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("2n+1  ");

        // Assert
        Assert.Equal(2, result.A);
        Assert.Equal(1, result.B);
    }

    #endregion

    #region Matching Tests

    [Fact]
    public void Matches_Odd_MatchesOddIndexes()
    {
        // Arrange
        var anb = CssAnB.Odd;

        // Assert
        Assert.True(anb.Matches(1));
        Assert.False(anb.Matches(2));
        Assert.True(anb.Matches(3));
        Assert.False(anb.Matches(4));
        Assert.True(anb.Matches(5));
    }

    [Fact]
    public void Matches_Even_MatchesEvenIndexes()
    {
        // Arrange
        var anb = CssAnB.Even;

        // Assert
        Assert.False(anb.Matches(1));
        Assert.True(anb.Matches(2));
        Assert.False(anb.Matches(3));
        Assert.True(anb.Matches(4));
        Assert.False(anb.Matches(5));
    }

    [Fact]
    public void Matches_5_MatchesOnlyFifthElement()
    {
        // Arrange
        var anb = new CssAnB(0, 5);

        // Assert
        Assert.False(anb.Matches(1));
        Assert.False(anb.Matches(4));
        Assert.True(anb.Matches(5));
        Assert.False(anb.Matches(6));
    }

    [Fact]
    public void Matches_3nPlus1_MatchesCorrectElements()
    {
        // Arrange - matches 1st, 4th, 7th, 10th, etc.
        var anb = new CssAnB(3, 1);

        // Assert
        Assert.True(anb.Matches(1));   // 3*0+1 = 1
        Assert.False(anb.Matches(2));
        Assert.False(anb.Matches(3));
        Assert.True(anb.Matches(4));   // 3*1+1 = 4
        Assert.False(anb.Matches(5));
        Assert.False(anb.Matches(6));
        Assert.True(anb.Matches(7));   // 3*2+1 = 7
    }

    [Fact]
    public void Matches_Minus1nPlus6_MatchesFirst6Elements()
    {
        // Arrange - :nth-child(-n+6) matches first 6 elements
        var anb = new CssAnB(-1, 6);

        // Assert
        Assert.True(anb.Matches(1));   // -1*5+6 = 1
        Assert.True(anb.Matches(2));   // -1*4+6 = 2
        Assert.True(anb.Matches(3));   // -1*3+6 = 3
        Assert.True(anb.Matches(4));   // -1*2+6 = 4
        Assert.True(anb.Matches(5));   // -1*1+6 = 5
        Assert.True(anb.Matches(6));   // -1*0+6 = 6
        Assert.False(anb.Matches(7));  // Would need n=-1
    }

    [Fact]
    public void Matches_ZeroZero_MatchesNothing()
    {
        // Arrange
        var anb = new CssAnB(0, 0);

        // Assert
        Assert.False(anb.Matches(1));
        Assert.False(anb.Matches(2));
        Assert.False(anb.Matches(100));
    }

    #endregion

    #region Serialization Tests

    [Fact]
    public void Serialize_A0_B5_Returns_5()
    {
        // Arrange
        var anb = new CssAnB(0, 5);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("5", result);
    }

    [Fact]
    public void Serialize_A0_BMinus3_Returns_Minus3()
    {
        // Arrange
        var anb = new CssAnB(0, -3);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("-3", result);
    }

    [Fact]
    public void Serialize_A1_B0_Returns_n()
    {
        // Arrange
        var anb = new CssAnB(1, 0);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("n", result);
    }

    [Fact]
    public void Serialize_AMinus1_B0_Returns_MinusN()
    {
        // Arrange
        var anb = new CssAnB(-1, 0);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("-n", result);
    }

    [Fact]
    public void Serialize_A2_B0_Returns_2n()
    {
        // Arrange
        var anb = new CssAnB(2, 0);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("2n", result);
    }

    [Fact]
    public void Serialize_A2_B1_Returns_2nPlus1()
    {
        // Arrange
        var anb = new CssAnB(2, 1);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("2n+1", result);
    }

    [Fact]
    public void Serialize_A3_BMinus2_Returns_3nMinus2()
    {
        // Arrange
        var anb = new CssAnB(3, -2);

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("3n-2", result);
    }

    [Fact]
    public void Serialize_Odd_Returns_2nPlus1()
    {
        // Arrange
        var anb = CssAnB.Odd;

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("2n+1", result);
    }

    [Fact]
    public void Serialize_Even_Returns_2n()
    {
        // Arrange
        var anb = CssAnB.Even;

        // Act
        var result = anb.ToString();

        // Assert
        Assert.Equal("2n", result);
    }

    #endregion

    #region TryParse Tests

    [Fact]
    public void TryParse_ValidInput_ReturnsTrue()
    {
        // Arrange & Act
        var success = CssAnBParser.TryParse("2n+1", out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(2, result.Value.A);
        Assert.Equal(1, result.Value.B);
    }

    [Fact]
    public void TryParse_EmptyString_ReturnsFalse()
    {
        // Arrange & Act
        var success = CssAnBParser.TryParse("", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_WhitespaceOnly_ReturnsFalse()
    {
        // Arrange & Act
        var success = CssAnBParser.TryParse("   ", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void TryParse_InvalidKeyword_ReturnsFalse()
    {
        // Arrange & Act
        var success = CssAnBParser.TryParse("invalid", out var result);

        // Assert
        Assert.False(success);
        Assert.Null(result);
    }

    #endregion

    #region Equality Tests

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        // Arrange
        var anb1 = new CssAnB(2, 1);
        var anb2 = new CssAnB(2, 1);

        // Assert
        Assert.Equal(anb1, anb2);
        Assert.True(anb1 == anb2);
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        // Arrange
        var anb1 = new CssAnB(2, 1);
        var anb2 = new CssAnB(3, 1);

        // Assert
        Assert.NotEqual(anb1, anb2);
        Assert.True(anb1 != anb2);
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHash()
    {
        // Arrange
        var anb1 = new CssAnB(2, 1);
        var anb2 = new CssAnB(2, 1);

        // Assert
        Assert.Equal(anb1.GetHashCode(), anb2.GetHashCode());
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Parse_LargeCoefficient_Returns_Correct()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("100n+50");

        // Assert
        Assert.Equal(100, result.A);
        Assert.Equal(50, result.B);
    }

    [Fact]
    public void Parse_0n_Returns_A0_B0()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("0n");

        // Assert
        Assert.Equal(0, result.A);
        Assert.Equal(0, result.B);
    }

    [Fact]
    public void Parse_0nPlus5_Returns_A0_B5()
    {
        // Arrange & Act
        var result = CssAnBParser.Parse("0n+5");

        // Assert
        Assert.Equal(0, result.A);
        Assert.Equal(5, result.B);
    }

    #endregion
}
