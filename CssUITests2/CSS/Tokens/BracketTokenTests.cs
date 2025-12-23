using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Tokens.Tests;

/// <summary>
/// Tests for bracket tokens: Curly braces, Square brackets, Parentheses
/// </summary>
public class BracketTokenTests
{
    #region Curly Bracket Tests
    [Fact]
    public void BracketOpenToken_HasCorrectType()
    {
        var token = new BracketOpenToken();
        Assert.Equal(ECssTokenType.Bracket_Open, token.Type);
    }

    [Fact]
    public void BracketOpenToken_EncodesToOpenCurly()
    {
        var token = new BracketOpenToken();
        Assert.Equal("{", token.Encode());
    }

    [Fact]
    public void BracketOpenToken_InstanceIsSingleton()
    {
        Assert.NotNull(BracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.Bracket_Open, BracketOpenToken.Instance.Type);
    }

    [Fact]
    public void BracketCloseToken_HasCorrectType()
    {
        var token = new BracketCloseToken();
        Assert.Equal(ECssTokenType.Bracket_Close, token.Type);
    }

    [Fact]
    public void BracketCloseToken_EncodesToCloseCurly()
    {
        var token = new BracketCloseToken();
        Assert.Equal("}", token.Encode());
    }

    [Fact]
    public void BracketCloseToken_InstanceIsSingleton()
    {
        Assert.NotNull(BracketCloseToken.Instance);
        Assert.Equal(ECssTokenType.Bracket_Close, BracketCloseToken.Instance.Type);
    }

    [Fact]
    public void CurlyBrackets_OpenAndClose_AreNotEqual()
    {
        var open = new BracketOpenToken();
        var close = new BracketCloseToken();
        Assert.NotEqual<CssToken>(open, close);
    }
    #endregion

    #region Square Bracket Tests
    [Fact]
    public void SqBracketOpenToken_HasCorrectType()
    {
        var token = new SqBracketOpenToken();
        Assert.Equal(ECssTokenType.SqBracket_Open, token.Type);
    }

    [Fact]
    public void SqBracketOpenToken_EncodesToOpenSquare()
    {
        var token = new SqBracketOpenToken();
        Assert.Equal("[", token.Encode());
    }

    [Fact]
    public void SqBracketOpenToken_InstanceIsSingleton()
    {
        Assert.NotNull(SqBracketOpenToken.Instance);
        Assert.Equal(ECssTokenType.SqBracket_Open, SqBracketOpenToken.Instance.Type);
    }

    [Fact]
    public void SqBracketCloseToken_HasCorrectType()
    {
        var token = new SqBracketCloseToken();
        Assert.Equal(ECssTokenType.SqBracket_Close, token.Type);
    }

    [Fact]
    public void SqBracketCloseToken_EncodesToCloseSquare()
    {
        var token = new SqBracketCloseToken();
        Assert.Equal("]", token.Encode());
    }

    [Fact]
    public void SqBracketCloseToken_InstanceIsSingleton()
    {
        Assert.NotNull(SqBracketCloseToken.Instance);
        Assert.Equal(ECssTokenType.SqBracket_Close, SqBracketCloseToken.Instance.Type);
    }

    [Fact]
    public void SquareBrackets_OpenAndClose_AreNotEqual()
    {
        var open = new SqBracketOpenToken();
        var close = new SqBracketCloseToken();
        Assert.NotEqual<CssToken>(open, close);
    }
    #endregion

    #region Parenthesis Tests
    [Fact]
    public void ParenthesisOpenToken_HasCorrectType()
    {
        var token = new ParenthesisOpenToken();
        Assert.Equal(ECssTokenType.Parenth_Open, token.Type);
    }

    [Fact]
    public void ParenthesisOpenToken_EncodesToOpenParen()
    {
        var token = new ParenthesisOpenToken();
        Assert.Equal("(", token.Encode());
    }

    [Fact]
    public void ParenthesisOpenToken_InstanceIsSingleton()
    {
        Assert.NotNull(ParenthesisOpenToken.Instance);
        Assert.Equal(ECssTokenType.Parenth_Open, ParenthesisOpenToken.Instance.Type);
    }

    [Fact]
    public void ParenthesisCloseToken_HasCorrectType()
    {
        var token = new ParenthesisCloseToken();
        Assert.Equal(ECssTokenType.Parenth_Close, token.Type);
    }

    [Fact]
    public void ParenthesisCloseToken_EncodesToCloseParen()
    {
        var token = new ParenthesisCloseToken();
        Assert.Equal(")", token.Encode());
    }

    [Fact]
    public void ParenthesisCloseToken_InstanceIsSingleton()
    {
        Assert.NotNull(ParenthesisCloseToken.Instance);
        Assert.Equal(ECssTokenType.Parenth_Close, ParenthesisCloseToken.Instance.Type);
    }

    [Fact]
    public void Parentheses_OpenAndClose_AreNotEqual()
    {
        var open = new ParenthesisOpenToken();
        var close = new ParenthesisCloseToken();
        Assert.NotEqual<CssToken>(open, close);
    }
    #endregion

    #region Cross-Bracket Type Tests
    [Fact]
    public void DifferentBracketTypes_AreNotEqual()
    {
        var curlyOpen = new BracketOpenToken();
        var squareOpen = new SqBracketOpenToken();
        var parenOpen = new ParenthesisOpenToken();

        Assert.NotEqual<CssToken>(curlyOpen, squareOpen);
        Assert.NotEqual<CssToken>(curlyOpen, parenOpen);
        Assert.NotEqual<CssToken>(squareOpen, parenOpen);
    }

    [Fact]
    public void AllOpenBrackets_HaveDistinctEncodings()
    {
        var curly = new BracketOpenToken().Encode();
        var square = new SqBracketOpenToken().Encode();
        var paren = new ParenthesisOpenToken().Encode();

        Assert.NotEqual(curly, square);
        Assert.NotEqual(curly, paren);
        Assert.NotEqual(square, paren);
    }

    [Fact]
    public void AllCloseBrackets_HaveDistinctEncodings()
    {
        var curly = new BracketCloseToken().Encode();
        var square = new SqBracketCloseToken().Encode();
        var paren = new ParenthesisCloseToken().Encode();

        Assert.NotEqual(curly, square);
        Assert.NotEqual(curly, paren);
        Assert.NotEqual(square, paren);
    }
    #endregion

    #region Matching Pairs Tests
    [Theory]
    [InlineData("{", "}")]
    [InlineData("[", "]")]
    [InlineData("(", ")")]
    public void BracketPairs_HaveMatchingEncodings(string open, string close)
    {
        CssToken openToken = open switch
        {
            "{" => new BracketOpenToken(),
            "[" => new SqBracketOpenToken(),
            "(" => new ParenthesisOpenToken(),
            _ => throw new System.ArgumentException($"Unknown bracket: {open}")
        };

        CssToken closeToken = close switch
        {
            "}" => new BracketCloseToken(),
            "]" => new SqBracketCloseToken(),
            ")" => new ParenthesisCloseToken(),
            _ => throw new System.ArgumentException($"Unknown bracket: {close}")
        };

        Assert.Equal(open, openToken.Encode());
        Assert.Equal(close, closeToken.Encode());
    }
    #endregion
}
