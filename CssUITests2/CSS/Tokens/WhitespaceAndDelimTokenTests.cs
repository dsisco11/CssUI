using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Tokens.Tests;

/// <summary>
/// Tests for WhitespaceToken and DelimToken
/// </summary>
public class WhitespaceAndDelimTokenTests
{
    #region WhitespaceToken Tests
    [Fact]
    public void WhitespaceToken_HasCorrectType()
    {
        var token = new WhitespaceToken(" ");
        Assert.Equal(ECssTokenType.Whitespace, token.Type);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("  ")]
    [InlineData("\t\t")]
    [InlineData("\n\n")]
    [InlineData(" \t\n")]
    public void WhitespaceToken_EncodesToOriginalValue(string whitespace)
    {
        var token = new WhitespaceToken(whitespace);
        Assert.Equal(whitespace, token.Encode());
    }

    [Fact]
    public void WhitespaceToken_SpaceInstance_IsCorrect()
    {
        Assert.NotNull(WhitespaceToken.Space);
        Assert.Equal(" ", WhitespaceToken.Space.Encode());
        Assert.Equal(ECssTokenType.Whitespace, WhitespaceToken.Space.Type);
    }

    [Fact]
    public void WhitespaceToken_TabInstance_IsCorrect()
    {
        Assert.NotNull(WhitespaceToken.Tab);
        Assert.Equal("\t", WhitespaceToken.Tab.Encode());
        Assert.Equal(ECssTokenType.Whitespace, WhitespaceToken.Tab.Type);
    }

    [Fact]
    public void WhitespaceToken_LFInstance_IsCorrect()
    {
        Assert.NotNull(WhitespaceToken.LF);
        Assert.Equal("\n", WhitespaceToken.LF.Encode());
        Assert.Equal(ECssTokenType.Whitespace, WhitespaceToken.LF.Type);
    }

    [Fact]
    public void WhitespaceToken_LFLFInstance_IsCorrect()
    {
        Assert.NotNull(WhitespaceToken.LFLF);
        Assert.Equal("\n\n", WhitespaceToken.LFLF.Encode());
        Assert.Equal(ECssTokenType.Whitespace, WhitespaceToken.LFLF.Type);
    }

    [Fact]
    public void WhitespaceToken_StoresValueProperty()
    {
        var token = new WhitespaceToken("   ");
        Assert.Equal("   ", token.Value);
    }

    [Fact]
    public void WhitespaceToken_EqualityByType()
    {
        var token1 = new WhitespaceToken(" ");
        var token2 = new WhitespaceToken("\t");
        // Base CssToken equality is by Type only
        Assert.Equal(token1, token2);
    }

    [Fact]
    public void WhitespaceToken_ToStringReturnsEncoded()
    {
        var token = new WhitespaceToken("  \t");
        Assert.Equal("  \t", token.ToString());
    }
    #endregion

    #region DelimToken Tests
    [Fact]
    public void DelimToken_HasCorrectType()
    {
        var token = new DelimToken('=');
        Assert.Equal(ECssTokenType.Delim, token.Type);
    }

    [Theory]
    [InlineData('=')]
    [InlineData('>')]
    [InlineData('<')]
    [InlineData('+')]
    [InlineData('-')]
    [InlineData('*')]
    [InlineData('/')]
    [InlineData('!')]
    [InlineData('~')]
    [InlineData('|')]
    [InlineData('^')]
    [InlineData('$')]
    [InlineData('.')]
    [InlineData('#')]
    public void DelimToken_EncodesToCharacter(char value)
    {
        var token = new DelimToken(value);
        Assert.Equal(value.ToString(), token.Encode());
    }

    [Fact]
    public void DelimToken_StoresValueProperty()
    {
        var token = new DelimToken('@');
        Assert.Equal('@', token.Value);
    }

    [Fact]
    public void DelimToken_EqualityByValue()
    {
        var token1 = new DelimToken('=');
        var token2 = new DelimToken('=');
        Assert.Equal(token1, token2);
    }

    [Fact]
    public void DelimToken_InequalityByValue()
    {
        var token1 = new DelimToken('=');
        var token2 = new DelimToken('>');
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void DelimToken_ToStringReturnsEncoded()
    {
        var token = new DelimToken('!');
        Assert.Equal("!", token.ToString());
    }

    [Fact]
    public void DelimToken_GetHashCode_DifferentForDifferentValues()
    {
        var token1 = new DelimToken('a');
        var token2 = new DelimToken('b');
        Assert.NotEqual(token1.GetHashCode(), token2.GetHashCode());
    }

    [Fact]
    public void DelimToken_GetHashCode_SameForSameValues()
    {
        var token1 = new DelimToken('=');
        var token2 = new DelimToken('=');
        Assert.Equal(token1.GetHashCode(), token2.GetHashCode());
    }
    #endregion

    #region Cross-Token Tests
    [Fact]
    public void WhitespaceToken_NotEqualToDelimToken()
    {
        var whitespace = new WhitespaceToken(" ");
        var delim = new DelimToken(' ');
        Assert.NotEqual<CssToken>(whitespace, delim);
    }
    #endregion
}
