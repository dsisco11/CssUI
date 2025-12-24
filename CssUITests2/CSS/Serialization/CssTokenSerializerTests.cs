using System.Collections.Generic;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Serialization;

/// <summary>
/// Tests for <see cref="CssTokenSerializer"/> per CSS Syntax Level 3 §10.
/// </summary>
/// <remarks>
/// Spec reference: https://www.w3.org/TR/css-syntax-3/#serialization
/// </remarks>
public class CssTokenSerializerTests
{
    #region Token Adjacency Tests

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.FunctionName, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.Url, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.Percentage, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.Dimension, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.CDC, null, true)]
    [InlineData(ECssTokenType.Ident, null, ECssTokenType.Parenth_Open, null, true)]
    public void RequiresSeparator_IdentFollowedByToken_ReturnsExpected(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.At_Keyword, null, ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.At_Keyword, null, ECssTokenType.FunctionName, null, true)]
    [InlineData(ECssTokenType.At_Keyword, null, ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.At_Keyword, null, ECssTokenType.CDC, null, true)]
    public void RequiresSeparator_AtKeywordFollowedByToken_ReturnsExpected(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.Hash, null, ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Hash, null, ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Hash, null, ECssTokenType.Dimension, null, true)]
    public void RequiresSeparator_HashFollowedByToken_ReturnsExpected(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.Dimension, null, ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Dimension, null, ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Dimension, null, ECssTokenType.Percentage, null, true)]
    public void RequiresSeparator_DimensionFollowedByToken_ReturnsExpected(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.Number, null, ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Number, null, ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Number, null, ECssTokenType.Percentage, null, true)]
    [InlineData(ECssTokenType.Number, null, ECssTokenType.Dimension, null, true)]
    public void RequiresSeparator_NumberFollowedByToken_ReturnsExpected(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.Delim, '#', ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Delim, '#', ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Delim, '-', ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Delim, '-', ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Delim, '@', ECssTokenType.Ident, null, true)]
    [InlineData(ECssTokenType.Delim, '.', ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Delim, '+', ECssTokenType.Number, null, true)]
    [InlineData(ECssTokenType.Delim, '/', ECssTokenType.Delim, '*', true)]
    public void RequiresSeparator_DelimFollowedByToken_ReturnsExpected(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "TokenAdjacency")]
    [InlineData(ECssTokenType.Whitespace, null, ECssTokenType.Ident, null, false)]
    [InlineData(ECssTokenType.Comma, null, ECssTokenType.Ident, null, false)]
    [InlineData(ECssTokenType.Semicolon, null, ECssTokenType.Ident, null, false)]
    [InlineData(ECssTokenType.Colon, null, ECssTokenType.Ident, null, false)]
    public void RequiresSeparator_SafePairs_ReturnsFalse(
        ECssTokenType firstType, char? firstDelim,
        ECssTokenType secondType, char? secondDelim,
        bool expected)
    {
        var result = CssTokenSerializer.RequiresSeparator(firstType, firstDelim, secondType, secondDelim);
        Assert.Equal(expected, result);
    }

    #endregion

    #region Single Token Serialization Tests

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_IdentToken_ReturnsValue()
    {
        var token = new IdentToken("color");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("color", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_IdentTokenWithEscape_ReturnsEscaped()
    {
        // Identifiers starting with digits need escaping
        var token = new IdentToken("123abc");
        var result = CssTokenSerializer.Serialize(token);
        // First digit should be escaped
        Assert.StartsWith("\\", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CustomPropertyIdent_PreservesCase()
    {
        var token = new IdentToken("--myCustomVar", preserveCase: true);
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("--myCustomVar", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_NumberTokenInteger_ReturnsInteger()
    {
        var token = new NumberToken(ENumericTokenType.Integer, "42", 42L);
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("42", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_NumberTokenFloat_ReturnsFloat()
    {
        var token = new NumberToken(ENumericTokenType.Number, "3.14", 3.14);
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("3.14", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_PercentageToken_IncludesPercent()
    {
        var token = new PercentageToken("50", 50.0);
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("50%", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_DimensionToken_IncludesUnit()
    {
        var token = new DimensionToken(ENumericTokenType.Integer, "10", 10L, "px");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("10px", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_HashTokenId_ReturnsHashWithValue()
    {
        var token = new HashToken(EHashTokenType.ID, "ffffff");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("#ffffff", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_HashTokenUnrestricted_ReturnsHashWithValue()
    {
        var token = new HashToken(EHashTokenType.Unrestricted, "123");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("#123", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_StringToken_ReturnsQuotedString()
    {
        var token = new StringToken("hello");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("\"hello\"", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_StringTokenWithQuote_EscapesQuote()
    {
        var token = new StringToken("say \"hi\"");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Contains("\\\"", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_UrlToken_ReturnsUrl()
    {
        var token = new UrlToken("image.png");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("url(\"image.png\")", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_DelimTokenBackslash_ReturnsBackslashNewline()
    {
        var token = new DelimToken('\\');
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("\\\n", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_DelimTokenPlus_ReturnsPlus()
    {
        var token = new DelimToken('+');
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("+", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_AtKeywordToken_ReturnsPrefixed()
    {
        var token = new AtToken("media");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("@media", result);
    }

    #endregion

    #region Token Stream Serialization Tests

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_TokenStream_JoinsTokens()
    {
        var tokens = new CssToken[]
        {
            new IdentToken("color"),
            ColonToken.Instance,
            new IdentToken("red"),
        };

        var result = CssTokenSerializer.Serialize(tokens);
        Assert.Equal("color:red", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_TokenStreamNeedingSeparator_InsertsComment()
    {
        // Two consecutive idents need a separator
        var tokens = new CssToken[]
        {
            new IdentToken("foo"),
            new IdentToken("bar"),
        };

        var result = CssTokenSerializer.Serialize(tokens);
        Assert.Equal("foo/**/bar", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_TokenStreamWithWhitespace_PreservesWhitespace()
    {
        var tokens = new CssToken[]
        {
            new IdentToken("color"),
            ColonToken.Instance,
            WhitespaceToken.Space,
            new IdentToken("red"),
        };

        var result = CssTokenSerializer.Serialize(tokens);
        Assert.Equal("color: red", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void SerializeWithCollapsedWhitespace_CollapsesDuplicates()
    {
        var tokens = new CssToken[]
        {
            new IdentToken("foo"),
            WhitespaceToken.Space,
            WhitespaceToken.Space,
            WhitespaceToken.Space,
            new IdentToken("bar"),
        };

        var result = CssTokenSerializer.SerializeWithCollapsedWhitespace(tokens);
        Assert.Equal("foo bar", result);
    }

    #endregion

    #region Component Value Serialization Tests

    // Note: CssFunction is internal so cannot be directly tested here.
    // Its serialization is tested indirectly through parser round-trip tests.

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssSimpleBlockCurly_ReturnsProperFormat()
    {
        var block = new CssSimpleBlock(BracketOpenToken.Instance);
        block.Values.Add(new IdentToken("color"));
        block.Values.Add(ColonToken.Instance);
        block.Values.Add(new IdentToken("red"));

        var result = CssTokenSerializer.Serialize(block);
        Assert.Equal("{color:red}", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssSimpleBlockParen_ReturnsProperFormat()
    {
        var block = new CssSimpleBlock(ParenthesisOpenToken.Instance);
        block.Values.Add(new NumberToken(ENumericTokenType.Integer, "1", 1L));

        var result = CssTokenSerializer.Serialize(block);
        Assert.Equal("(1)", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssSimpleBlockSquare_ReturnsProperFormat()
    {
        var block = new CssSimpleBlock(SqBracketOpenToken.Instance);
        block.Values.Add(new IdentToken("attr"));

        var result = CssTokenSerializer.Serialize(block);
        Assert.Equal("[attr]", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssAtRuleWithBlock_ReturnsProperFormat()
    {
        var atRule = new CssAtRule("media");
        atRule.Prelude.Add(new IdentToken("screen"));
        atRule.Block = new CssSimpleBlock(BracketOpenToken.Instance);

        var result = CssTokenSerializer.Serialize(atRule);
        Assert.Equal("@media screen{}", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_StringToken_ReturnsQuotedValue()
    {
        var token = new StringToken("utf-8");
        var result = CssTokenSerializer.Serialize(token);
        Assert.Equal("\"utf-8\"", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_TokenList_SingleStringToken_ReturnsQuotedValue()
    {
        var tokens = new List<CssToken> { new StringToken("utf-8") };
        var result = CssTokenSerializer.Serialize(tokens);
        Assert.Equal("\"utf-8\"", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssAtRuleWithoutBlock_ReturnsSemicolon()
    {
        var atRule = new CssAtRule("charset");
        atRule.Prelude.Add(new StringToken("utf-8"));

        var result = CssTokenSerializer.Serialize(atRule);
        // Serializer adds space after @keyword, and StringToken serializes as "value"
        Assert.Equal("@charset \"utf-8\";", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssDeclaration_ReturnsProperFormat()
    {
        var decl = new CssDecleration("color");
        decl.Values.Add(new IdentToken("red"));

        var result = CssTokenSerializer.Serialize(decl);
        Assert.Equal("color: red", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssDeclarationImportant_IncludesFlag()
    {
        var decl = new CssDecleration("color");
        decl.Values.Add(new IdentToken("red"));
        decl.Important = true;

        var result = CssTokenSerializer.Serialize(decl);
        Assert.Equal("color: red !important", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssDeclarationCustomProperty_PreservesCase()
    {
        var decl = new CssDecleration("--myCustomProp");
        decl.Values.Add(new NumberToken(ENumericTokenType.Integer, "42", 42L));

        var result = CssTokenSerializer.Serialize(decl);
        Assert.Equal("--myCustomProp: 42", result);
    }

    [Fact]
    [Trait("Category", "Serialization")]
    public void Serialize_CssQualifiedRule_ReturnsProperFormat()
    {
        var rule = new CssQualifiedRule();
        rule.Prelude.Add(new IdentToken("body"));
        rule.Block.Values.Add(new IdentToken("color"));
        rule.Block.Values.Add(ColonToken.Instance);
        rule.Block.Values.Add(new IdentToken("red"));

        var result = CssTokenSerializer.Serialize(rule);
        Assert.Equal("body{color:red}", result);
    }

    #endregion

    #region Round-Trip Tests

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("color")]
    [InlineData("background-color")]
    [InlineData("--my-custom-prop")]
    public void RoundTrip_Identifier_Preserved(string identifier)
    {
        var token = new IdentToken(identifier, preserveCase: identifier.StartsWith("--"));
        var serialized = CssTokenSerializer.Serialize(token);
        Assert.Equal(identifier, serialized);
    }

    [Theory]
    [Trait("Category", "Serialization")]
    [Trait("Category", "RoundTrip")]
    [InlineData("hello world")]
    [InlineData("contains \"quotes\"")]
    [InlineData("line\nbreak")]
    public void RoundTrip_StringToken_CanBeReparsed(string value)
    {
        var token = new StringToken(value);
        var serialized = CssTokenSerializer.Serialize(token);

        // Should be quoted
        Assert.StartsWith("\"", serialized);
        Assert.EndsWith("\"", serialized);
    }

    #endregion
}
