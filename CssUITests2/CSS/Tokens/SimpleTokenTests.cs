using CssUI.CSS;
using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Tokens.Tests;

/// <summary>
/// Tests for simple punctuation tokens: Colon, Comma, Semicolon
/// </summary>
public class SimpleTokenTests
{
    #region ColonToken Tests
    [Fact]
    public void ColonToken_HasCorrectType()
    {
        var token = new ColonToken();
        Assert.Equal(ECssTokenType.Colon, token.Type);
    }

    [Fact]
    public void ColonToken_EncodesToColon()
    {
        var token = new ColonToken();
        Assert.Equal(":", token.Encode());
    }

    [Fact]
    public void ColonToken_InstanceIsSingleton()
    {
        Assert.NotNull(ColonToken.Instance);
        Assert.Equal(ECssTokenType.Colon, ColonToken.Instance.Type);
    }

    [Fact]
    public void ColonToken_EqualityWithSameType()
    {
        var token1 = new ColonToken();
        var token2 = new ColonToken();
        Assert.Equal(token1, token2);
    }

    [Fact]
    public void ColonToken_ToStringReturnsEncoded()
    {
        var token = new ColonToken();
        Assert.Equal(":", token.ToString());
    }
    #endregion

    #region CommaToken Tests
    [Fact]
    public void CommaToken_HasCorrectType()
    {
        var token = new CommaToken();
        Assert.Equal(ECssTokenType.Comma, token.Type);
    }

    [Fact]
    public void CommaToken_EncodesToComma()
    {
        var token = new CommaToken();
        Assert.Equal(",", token.Encode());
    }

    [Fact]
    public void CommaToken_InstanceIsSingleton()
    {
        Assert.NotNull(CommaToken.Instance);
        Assert.Equal(ECssTokenType.Comma, CommaToken.Instance.Type);
    }

    [Fact]
    public void CommaToken_EqualityWithSameType()
    {
        var token1 = new CommaToken();
        var token2 = new CommaToken();
        Assert.Equal(token1, token2);
    }

    [Fact]
    public void CommaToken_ToStringReturnsEncoded()
    {
        var token = new CommaToken();
        Assert.Equal(",", token.ToString());
    }
    #endregion

    #region SemicolonToken Tests
    [Fact]
    public void SemicolonToken_HasCorrectType()
    {
        var token = new SemicolonToken();
        Assert.Equal(ECssTokenType.Semicolon, token.Type);
    }

    [Fact]
    public void SemicolonToken_EncodesToSemicolon()
    {
        var token = new SemicolonToken();
        Assert.Equal(";", token.Encode());
    }

    [Fact]
    public void SemicolonToken_InstanceIsSingleton()
    {
        Assert.NotNull(SemicolonToken.Instance);
        Assert.Equal(ECssTokenType.Semicolon, SemicolonToken.Instance.Type);
    }

    [Fact]
    public void SemicolonToken_EqualityWithSameType()
    {
        var token1 = new SemicolonToken();
        var token2 = new SemicolonToken();
        Assert.Equal(token1, token2);
    }

    [Fact]
    public void SemicolonToken_ToStringReturnsEncoded()
    {
        var token = new SemicolonToken();
        Assert.Equal(";", token.ToString());
    }
    #endregion

    #region Cross-Token Inequality Tests
    [Fact]
    public void DifferentTokenTypes_AreNotEqual()
    {
        var colon = new ColonToken();
        var comma = new CommaToken();
        var semicolon = new SemicolonToken();

        Assert.NotEqual<CssToken>(colon, comma);
        Assert.NotEqual<CssToken>(colon, semicolon);
        Assert.NotEqual<CssToken>(comma, semicolon);
    }

    [Fact]
    public void TokenEquality_WithNull_ReturnsFalse()
    {
        var colon = new ColonToken();
        Assert.False(colon.Equals(null));
    }

    [Fact]
    public void TokenEquality_WithNonToken_ReturnsFalse()
    {
        var colon = new ColonToken();
        Assert.False(colon.Equals("not a token"));
    }
    #endregion
}
