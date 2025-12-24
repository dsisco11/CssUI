using System;
using System.Collections.Generic;
using System.Linq;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser;

/// <summary>
/// Tests for <see cref="CssProductionMatcher"/> - validates the <c>&lt;declaration-value&gt;</c>
/// and <c>&lt;any-value&gt;</c> productions per CSS Syntax Level 3 §8.2.
/// </summary>
[Trait("Category", "CSS Parser")]
[Trait("Spec", "CSS Syntax 3 §8.2")]
public class CssProductionMatcherTests
{
    #region Helper Methods
    private static CssToken[] Tokenize(string css)
    {
        var tokenizer = new CssTokenizer(css.AsSpan());
        return tokenizer.Tokens.ToArray();
    }
    #endregion

    #region <declaration-value> Tests

    #region Valid Sequences
    [Fact]
    public void MatchDeclarationValue_SimpleIdent_ReturnsSuccess()
    {
        var tokens = Tokenize("red");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.None, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_NumberWithUnit_ReturnsSuccess()
    {
        var tokens = Tokenize("10px");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_Function_ReturnsSuccess()
    {
        var tokens = Tokenize("rgb(255, 0, 0)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_NestedFunctions_ReturnsSuccess()
    {
        var tokens = Tokenize("calc(100% - 10px)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_MatchedBrackets_ReturnsSuccess()
    {
        var tokens = Tokenize("(a) [b] {c}");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_NestedBrackets_ReturnsSuccess()
    {
        var tokens = Tokenize("((a) [b {c}])");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_SemicolonInsideBrackets_ReturnsSuccess()
    {
        // Semicolon inside brackets is OK - only top-level semicolons are forbidden
        var tokens = Tokenize("(a; b)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_ExclamationInsideBrackets_ReturnsSuccess()
    {
        // "!" inside brackets is OK - only top-level "!" is forbidden
        var tokens = Tokenize("(a ! b)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_ComplexValue_ReturnsSuccess()
    {
        var tokens = Tokenize("1px solid rgb(0, 0, 0)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_VarFunction_ReturnsSuccess()
    {
        var tokens = Tokenize("var(--custom-property)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchDeclarationValue_CalcWithMath_ReturnsSuccess()
    {
        var tokens = Tokenize("calc(100% / 3 - 10px * 2)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }
    #endregion

    #region Invalid Sequences - Bad Tokens

    // Note: Testing bad-string-token and bad-url-token requires deliberately malformed CSS
    // that the tokenizer handles specifically. These may be difficult to produce in tests
    // as the tokenizer typically recovers gracefully.

    #endregion

    #region Invalid Sequences - Unmatched Brackets
    [Fact]
    public void MatchDeclarationValue_UnmatchedCloseParen_ReturnsFalse()
    {
        var tokens = Tokenize("a)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_UnmatchedCloseSquare_ReturnsFalse()
    {
        var tokens = Tokenize("a]");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_UnmatchedCloseCurly_ReturnsFalse()
    {
        var tokens = Tokenize("a}");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_CloseParenWithOnlySquareOpen_ReturnsFalse()
    {
        var tokens = Tokenize("[a)");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }
    #endregion

    #region Invalid Sequences - Top-Level Semicolon
    [Fact]
    public void MatchDeclarationValue_TopLevelSemicolon_ReturnsFalse()
    {
        var tokens = Tokenize("a; b");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.TopLevelSemicolon, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_TrailingSemicolon_ReturnsFalse()
    {
        var tokens = Tokenize("red;");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.TopLevelSemicolon, result.FailureType);
    }
    #endregion

    #region Invalid Sequences - Top-Level Exclamation
    [Fact]
    public void MatchDeclarationValue_TopLevelExclamation_ReturnsFalse()
    {
        var tokens = Tokenize("red !important");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.TopLevelExclamation, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_TopLevelExclamationAlone_ReturnsFalse()
    {
        var tokens = Tokenize("!");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.TopLevelExclamation, result.FailureType);
    }
    #endregion

    #region Edge Cases
    [Fact]
    public void MatchDeclarationValue_EmptySequence_ReturnsFalse()
    {
        var result = CssProductionMatcher.MatchDeclarationValue(new List<CssToken>());

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.EmptySequence, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_NullSequence_ReturnsFalse()
    {
        var result = CssProductionMatcher.MatchDeclarationValue(null!);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.EmptySequence, result.FailureType);
    }

    [Fact]
    public void MatchDeclarationValue_OnlyWhitespace_ReturnsSuccess()
    {
        // Whitespace-only IS valid (though weird)
        var tokens = Tokenize("   ");
        var result = CssProductionMatcher.MatchDeclarationValue(tokens);

        Assert.True(result.IsMatch);
    }
    #endregion

    #region Shorthand Helper Method
    [Fact]
    public void IsDeclarationValue_ValidInput_ReturnsTrue()
    {
        var tokens = Tokenize("10px solid black");
        Assert.True(CssProductionMatcher.IsDeclarationValue(tokens));
    }

    [Fact]
    public void IsDeclarationValue_InvalidInput_ReturnsFalse()
    {
        var tokens = Tokenize("red;");
        Assert.False(CssProductionMatcher.IsDeclarationValue(tokens));
    }
    #endregion

    #endregion

    #region <any-value> Tests

    #region Valid Sequences
    [Fact]
    public void MatchAnyValue_SimpleIdent_ReturnsSuccess()
    {
        var tokens = Tokenize("red");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.None, result.FailureType);
    }

    [Fact]
    public void MatchAnyValue_TopLevelSemicolon_ReturnsSuccess()
    {
        // <any-value> DOES allow top-level semicolons (unlike <declaration-value>)
        var tokens = Tokenize("a; b");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchAnyValue_TopLevelExclamation_ReturnsSuccess()
    {
        // <any-value> DOES allow top-level "!" (unlike <declaration-value>)
        var tokens = Tokenize("red !important");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchAnyValue_ExclamationAlone_ReturnsSuccess()
    {
        var tokens = Tokenize("!");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchAnyValue_MultipleSemicolons_ReturnsSuccess()
    {
        var tokens = Tokenize("a; b; c;");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchAnyValue_MatchedBrackets_ReturnsSuccess()
    {
        var tokens = Tokenize("(a) [b] {c}");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
    }

    [Fact]
    public void MatchAnyValue_NestedBrackets_ReturnsSuccess()
    {
        var tokens = Tokenize("((a) [b {c}])");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.True(result.IsMatch);
    }
    #endregion

    #region Invalid Sequences - Unmatched Brackets
    [Fact]
    public void MatchAnyValue_UnmatchedCloseParen_ReturnsFalse()
    {
        var tokens = Tokenize("a)");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }

    [Fact]
    public void MatchAnyValue_UnmatchedCloseSquare_ReturnsFalse()
    {
        var tokens = Tokenize("a]");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }

    [Fact]
    public void MatchAnyValue_UnmatchedCloseCurly_ReturnsFalse()
    {
        var tokens = Tokenize("a}");
        var result = CssProductionMatcher.MatchAnyValue(tokens);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.UnmatchedCloseBracket, result.FailureType);
    }
    #endregion

    #region Edge Cases
    [Fact]
    public void MatchAnyValue_EmptySequence_ReturnsFalse()
    {
        var result = CssProductionMatcher.MatchAnyValue(new List<CssToken>());

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.EmptySequence, result.FailureType);
    }

    [Fact]
    public void MatchAnyValue_NullSequence_ReturnsFalse()
    {
        var result = CssProductionMatcher.MatchAnyValue(null!);

        Assert.False(result.IsMatch);
        Assert.Equal(ECssProductionMatchFailure.EmptySequence, result.FailureType);
    }
    #endregion

    #region Shorthand Helper Method
    [Fact]
    public void IsAnyValue_ValidInput_ReturnsTrue()
    {
        var tokens = Tokenize("a; b !important");
        Assert.True(CssProductionMatcher.IsAnyValue(tokens));
    }

    [Fact]
    public void IsAnyValue_InvalidInput_ReturnsFalse()
    {
        var tokens = Tokenize("a)");
        Assert.False(CssProductionMatcher.IsAnyValue(tokens));
    }
    #endregion

    #endregion

    #region Difference between <declaration-value> and <any-value>
    [Fact]
    public void DeclarationValueRejectsSemicolon_AnyValueAccepts()
    {
        var tokens = Tokenize("a; b");

        Assert.False(CssProductionMatcher.IsDeclarationValue(tokens));
        Assert.True(CssProductionMatcher.IsAnyValue(tokens));
    }

    [Fact]
    public void DeclarationValueRejectsExclamation_AnyValueAccepts()
    {
        var tokens = Tokenize("red !important");

        Assert.False(CssProductionMatcher.IsDeclarationValue(tokens));
        Assert.True(CssProductionMatcher.IsAnyValue(tokens));
    }

    [Fact]
    public void BothProductionsRejectUnmatchedBrackets()
    {
        var tokens = Tokenize("a}");

        Assert.False(CssProductionMatcher.IsDeclarationValue(tokens));
        Assert.False(CssProductionMatcher.IsAnyValue(tokens));
    }
    #endregion
}
