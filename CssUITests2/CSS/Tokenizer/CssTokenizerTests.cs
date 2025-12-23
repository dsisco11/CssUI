using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.Difference;
using Xunit;

namespace CssUITests.CSS.Parser.Tests;

/// <summary>
/// Comprehensive test suite for CssTokenizer per CSS Syntax Level 3 specification.
/// Tests are organized by token type and algorithm as defined in the spec.
/// </summary>
public class CssTokenizerTests
{
    #region Helper Methods
    private static CssToken[] Tokenize(string input) => CssTokenizer.Parse(input);

    private static CssToken FirstToken(string input)
    {
        var tokens = Tokenize(input);
        return tokens.Length > 0 ? tokens[0] : EOFToken.Instance;
    }
    #endregion

    #region EOF Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "EOF")]
    public void Parse_EmptyString_ReturnsOnlyEOFToken()
    {
        var tokens = Tokenize("");

        Assert.Single(tokens);
        Assert.Equal(ECssTokenType.EOF, tokens[0].Type);
    }
    #endregion

    #region Whitespace Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Whitespace")]
    public void Parse_SingleSpace_ReturnsWhitespaceToken()
    {
        var token = FirstToken(" ");

        Assert.Equal(ECssTokenType.Whitespace, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Whitespace")]
    public void Parse_MultipleSpaces_ReturnsSingleWhitespaceToken()
    {
        var tokens = Tokenize("    ");

        Assert.Equal(2, tokens.Length); // Whitespace + EOF
        Assert.Equal(ECssTokenType.Whitespace, tokens[0].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Whitespace")]
    public void Parse_Tab_ReturnsWhitespaceToken()
    {
        var token = FirstToken("\t");

        Assert.Equal(ECssTokenType.Whitespace, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Whitespace")]
    public void Parse_Newline_ReturnsWhitespaceToken()
    {
        var token = FirstToken("\n");

        Assert.Equal(ECssTokenType.Whitespace, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Whitespace")]
    public void Parse_CarriageReturn_ReturnsWhitespaceToken()
    {
        var token = FirstToken("\r");

        Assert.Equal(ECssTokenType.Whitespace, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Whitespace")]
    public void Parse_MixedWhitespace_ReturnsSingleWhitespaceToken()
    {
        var tokens = Tokenize(" \t\n\r ");

        Assert.Equal(2, tokens.Length); // Whitespace + EOF
        Assert.Equal(ECssTokenType.Whitespace, tokens[0].Type);
    }
    #endregion

    #region String Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_DoubleQuotedString_ReturnsStringToken()
    {
        var token = FirstToken("\"hello\"");

        Assert.Equal(ECssTokenType.String, token.Type);
        Assert.IsType<StringToken>(token);
        Assert.Equal("hello", ((StringToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_SingleQuotedString_ReturnsStringToken()
    {
        var token = FirstToken("'hello'");

        Assert.Equal(ECssTokenType.String, token.Type);
        Assert.IsType<StringToken>(token);
        Assert.Equal("hello", ((StringToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_EmptyDoubleQuotedString_ReturnsStringToken()
    {
        var token = FirstToken("\"\"");

        Assert.Equal(ECssTokenType.String, token.Type);
        Assert.Equal("", ((StringToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_EmptySingleQuotedString_ReturnsStringToken()
    {
        var token = FirstToken("''");

        Assert.Equal(ECssTokenType.String, token.Type);
        Assert.Equal("", ((StringToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_StringWithSpaces_ReturnsStringToken()
    {
        var token = FirstToken("\"hello world\"");

        Assert.Equal(ECssTokenType.String, token.Type);
        Assert.Equal("hello world", ((StringToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_StringWithEscapedQuote_ReturnsStringToken()
    {
        var token = FirstToken("\"hello\\\"world\"");

        Assert.Equal(ECssTokenType.String, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_StringWithEscapedNewline_ReturnsStringToken()
    {
        var token = FirstToken("\"hello\\\nworld\"");

        Assert.Equal(ECssTokenType.String, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_StringWithUnescapedNewline_ReturnsBadStringToken()
    {
        var token = FirstToken("\"hello\nworld\"");

        Assert.Equal(ECssTokenType.Bad_String, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_UnterminatedString_ReturnsStringToken()
    {
        // Per CSS spec, unterminated strings at EOF are valid
        var token = FirstToken("\"hello");

        Assert.Equal(ECssTokenType.String, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "String")]
    public void Parse_StringWithHexEscape_ReturnsStringToken()
    {
        var token = FirstToken("\"\\41\""); // \41 = 'A'

        Assert.Equal(ECssTokenType.String, token.Type);
        Assert.Equal("A", ((StringToken)token).Value);
    }
    #endregion

    #region Number Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_Integer_ReturnsNumberToken()
    {
        var token = FirstToken("42");

        Assert.Equal(ECssTokenType.Number, token.Type);
        Assert.IsType<NumberToken>(token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_Zero_ReturnsNumberToken()
    {
        var token = FirstToken("0");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_NegativeInteger_ReturnsNumberToken()
    {
        var token = FirstToken("-42");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_PositiveInteger_ReturnsNumberToken()
    {
        var token = FirstToken("+42");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_Decimal_ReturnsNumberToken()
    {
        var token = FirstToken("3.14");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_DecimalStartingWithDot_ReturnsNumberToken()
    {
        var token = FirstToken(".5");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_NegativeDecimal_ReturnsNumberToken()
    {
        var token = FirstToken("-3.14");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_ScientificNotation_ReturnsNumberToken()
    {
        var token = FirstToken("1e10");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_ScientificNotationUpperE_ReturnsNumberToken()
    {
        var token = FirstToken("1E10");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_ScientificNotationNegativeExponent_ReturnsNumberToken()
    {
        var token = FirstToken("1e-10");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_ScientificNotationPositiveExponent_ReturnsNumberToken()
    {
        var token = FirstToken("1e+10");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Number")]
    public void Parse_DecimalWithExponent_ReturnsNumberToken()
    {
        var token = FirstToken("2.5e3");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }
    #endregion

    #region Percentage Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Percentage")]
    public void Parse_Percentage_ReturnsPercentageToken()
    {
        var token = FirstToken("50%");

        Assert.Equal(ECssTokenType.Percentage, token.Type);
        Assert.IsType<PercentageToken>(token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Percentage")]
    public void Parse_ZeroPercentage_ReturnsPercentageToken()
    {
        var token = FirstToken("0%");

        Assert.Equal(ECssTokenType.Percentage, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Percentage")]
    public void Parse_DecimalPercentage_ReturnsPercentageToken()
    {
        var token = FirstToken("33.33%");

        Assert.Equal(ECssTokenType.Percentage, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Percentage")]
    public void Parse_NegativePercentage_ReturnsPercentageToken()
    {
        var token = FirstToken("-50%");

        Assert.Equal(ECssTokenType.Percentage, token.Type);
    }
    #endregion

    #region Dimension Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_PixelDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("100px");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.IsType<DimensionToken>(token);
        Assert.Equal("px", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_EmDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("1em");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("em", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_RemDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("2rem");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("rem", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_ViewportWidthDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("50vw");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("vw", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_ViewportHeightDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("100vh");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("vh", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_ChDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("10ch");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("ch", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_SecondsDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("2s");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("s", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_MillisecondsDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("500ms");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("ms", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_DegreesDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("90deg");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("deg", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_RadiansDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("3.14rad");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("rad", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_DpiDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("96dpi");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
        Assert.Equal("dpi", ((DimensionToken)token).Unit);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_NegativeDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("-10px");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_DecimalDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("1.5em");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Dimension")]
    public void Parse_ZeroDimension_ReturnsDimensionToken()
    {
        var token = FirstToken("0px");

        Assert.Equal(ECssTokenType.Dimension, token.Type);
    }
    #endregion

    #region Ident Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_SimpleIdent_ReturnsIdentToken()
    {
        var token = FirstToken("hello");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.IsType<IdentToken>(token);
        Assert.Equal("hello", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_IdentWithHyphen_ReturnsIdentToken()
    {
        var token = FirstToken("font-family");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("font-family", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_IdentWithUnderscore_ReturnsIdentToken()
    {
        var token = FirstToken("_private");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("_private", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_IdentWithNumbers_ReturnsIdentToken()
    {
        var token = FirstToken("h1");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("h1", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_IdentStartingWithHyphen_ReturnsIdentToken()
    {
        var token = FirstToken("-webkit-transform");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("-webkit-transform", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_IdentWithEscape_ReturnsIdentToken()
    {
        var token = FirstToken("\\41 BC"); // \41 = 'A', so "ABC"

        Assert.Equal(ECssTokenType.Ident, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_IdentWithNonAscii_ReturnsIdentToken()
    {
        var token = FirstToken("über");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("über", ((IdentToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Ident")]
    public void Parse_CssKeyword_ReturnsIdentToken()
    {
        var token = FirstToken("inherit");

        Assert.Equal(ECssTokenType.Ident, token.Type);
        Assert.Equal("inherit", ((IdentToken)token).Value);
    }
    #endregion

    #region Function Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Function")]
    public void Parse_Function_ReturnsFunctionNameToken()
    {
        var token = FirstToken("rgb(");

        Assert.Equal(ECssTokenType.FunctionName, token.Type);
        Assert.IsType<FunctionNameToken>(token);
        Assert.Equal("rgb", ((FunctionNameToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Function")]
    public void Parse_CalcFunction_ReturnsFunctionNameToken()
    {
        var token = FirstToken("calc(");

        Assert.Equal(ECssTokenType.FunctionName, token.Type);
        Assert.Equal("calc", ((FunctionNameToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Function")]
    public void Parse_VarFunction_ReturnsFunctionNameToken()
    {
        var token = FirstToken("var(");

        Assert.Equal(ECssTokenType.FunctionName, token.Type);
        Assert.Equal("var", ((FunctionNameToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Function")]
    public void Parse_LinearGradientFunction_ReturnsFunctionNameToken()
    {
        var token = FirstToken("linear-gradient(");

        Assert.Equal(ECssTokenType.FunctionName, token.Type);
        Assert.Equal("linear-gradient", ((FunctionNameToken)token).Value);
    }
    #endregion

    #region URL Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_UrlWithQuotes_ReturnsUrlToken()
    {
        var token = FirstToken("url(\"http://example.com\")");

        Assert.Equal(ECssTokenType.Url, token.Type);
        Assert.IsType<UrlToken>(token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_UrlWithSingleQuotes_ReturnsUrlToken()
    {
        var token = FirstToken("url('http://example.com')");

        Assert.Equal(ECssTokenType.Url, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_UrlWithoutQuotes_ReturnsUrlToken()
    {
        var token = FirstToken("url(http://example.com)");

        Assert.Equal(ECssTokenType.Url, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_UrlUppercase_ReturnsUrlToken()
    {
        var token = FirstToken("URL(\"http://example.com\")");

        Assert.Equal(ECssTokenType.Url, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_EmptyUrl_ReturnsUrlToken()
    {
        var token = FirstToken("url()");

        Assert.Equal(ECssTokenType.Url, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_UrlWithWhitespace_ReturnsUrlToken()
    {
        var token = FirstToken("url(  \"test\"  )");

        Assert.Equal(ECssTokenType.Url, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "URL")]
    public void Parse_BadUrl_ReturnsBadUrlToken()
    {
        // URL with unescaped quote in unquoted context
        var token = FirstToken("url(bad\"url)");

        Assert.Equal(ECssTokenType.Bad_Url, token.Type);
    }
    #endregion

    #region Hash Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Hash")]
    public void Parse_HashId_ReturnsHashToken()
    {
        var token = FirstToken("#myId");

        Assert.Equal(ECssTokenType.Hash, token.Type);
        Assert.IsType<HashToken>(token);
        Assert.Equal("myId", ((HashToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Hash")]
    public void Parse_HashColor_ReturnsHashToken()
    {
        var token = FirstToken("#ff0000");

        Assert.Equal(ECssTokenType.Hash, token.Type);
        Assert.Equal("ff0000", ((HashToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Hash")]
    public void Parse_HashShortColor_ReturnsHashToken()
    {
        var token = FirstToken("#f00");

        Assert.Equal(ECssTokenType.Hash, token.Type);
        Assert.Equal("f00", ((HashToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Hash")]
    public void Parse_HashStartingWithNumber_ReturnsHashToken()
    {
        var token = FirstToken("#123abc");

        Assert.Equal(ECssTokenType.Hash, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Hash")]
    public void Parse_HashAlone_ReturnsDelimToken()
    {
        var token = FirstToken("# ");

        Assert.Equal(ECssTokenType.Delim, token.Type);
    }
    #endregion

    #region At-Keyword Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "At")]
    public void Parse_AtKeyword_ReturnsAtToken()
    {
        var token = FirstToken("@media");

        Assert.Equal(ECssTokenType.At_Keyword, token.Type);
        Assert.IsType<AtToken>(token);
        Assert.Equal("media", ((AtToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "At")]
    public void Parse_AtImport_ReturnsAtToken()
    {
        var token = FirstToken("@import");

        Assert.Equal(ECssTokenType.At_Keyword, token.Type);
        Assert.Equal("import", ((AtToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "At")]
    public void Parse_AtCharset_ReturnsAtToken()
    {
        var token = FirstToken("@charset");

        Assert.Equal(ECssTokenType.At_Keyword, token.Type);
        Assert.Equal("charset", ((AtToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "At")]
    public void Parse_AtKeyframes_ReturnsAtToken()
    {
        var token = FirstToken("@keyframes");

        Assert.Equal(ECssTokenType.At_Keyword, token.Type);
        Assert.Equal("keyframes", ((AtToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "At")]
    public void Parse_AtFontFace_ReturnsAtToken()
    {
        var token = FirstToken("@font-face");

        Assert.Equal(ECssTokenType.At_Keyword, token.Type);
        Assert.Equal("font-face", ((AtToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "At")]
    public void Parse_AtAlone_ReturnsDelimToken()
    {
        var token = FirstToken("@ ");

        Assert.Equal(ECssTokenType.Delim, token.Type);
    }
    #endregion

    #region Punctuation Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_Colon_ReturnsColonToken()
    {
        var token = FirstToken(":");

        Assert.Equal(ECssTokenType.Colon, token.Type);
        Assert.Same(ColonToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_Semicolon_ReturnsSemicolonToken()
    {
        var token = FirstToken(";");

        Assert.Equal(ECssTokenType.Semicolon, token.Type);
        Assert.Same(SemicolonToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_Comma_ReturnsCommaToken()
    {
        var token = FirstToken(",");

        Assert.Equal(ECssTokenType.Comma, token.Type);
        Assert.Same(CommaToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_OpenParen_ReturnsParenOpenToken()
    {
        var token = FirstToken("(");

        Assert.Equal(ECssTokenType.Parenth_Open, token.Type);
        Assert.Same(ParenthesisOpenToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_CloseParen_ReturnsParenCloseToken()
    {
        var token = FirstToken(")");

        Assert.Equal(ECssTokenType.Parenth_Close, token.Type);
        Assert.Same(ParenthesisCloseToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_OpenBracket_ReturnsBracketOpenToken()
    {
        var token = FirstToken("{");

        Assert.Equal(ECssTokenType.Bracket_Open, token.Type);
        Assert.Same(BracketOpenToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_CloseBracket_ReturnsBracketCloseToken()
    {
        var token = FirstToken("}");

        Assert.Equal(ECssTokenType.Bracket_Close, token.Type);
        Assert.Same(BracketCloseToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_OpenSquareBracket_ReturnsSqBracketOpenToken()
    {
        var token = FirstToken("[");

        Assert.Equal(ECssTokenType.SqBracket_Open, token.Type);
        Assert.Same(SqBracketOpenToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Punctuation")]
    public void Parse_CloseSquareBracket_ReturnsSqBracketCloseToken()
    {
        var token = FirstToken("]");

        Assert.Equal(ECssTokenType.SqBracket_Close, token.Type);
        Assert.Same(SqBracketCloseToken.Instance, token);
    }
    #endregion

    #region Match Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Match")]
    public void Parse_PrefixMatch_ReturnsPrefixMatchToken()
    {
        var token = FirstToken("^=");

        Assert.Equal(ECssTokenType.Prefix_Match, token.Type);
        Assert.Same(PrefixMatchToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Match")]
    public void Parse_SuffixMatch_ReturnsSuffixMatchToken()
    {
        var token = FirstToken("$=");

        Assert.Equal(ECssTokenType.Suffix_Match, token.Type);
        Assert.Same(SuffixMatchToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Match")]
    public void Parse_SubstringMatch_ReturnsSubstringMatchToken()
    {
        var token = FirstToken("*=");

        Assert.Equal(ECssTokenType.Substring_Match, token.Type);
        Assert.Same(SubstringMatchToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Match")]
    public void Parse_DashMatch_ReturnsDashMatchToken()
    {
        var token = FirstToken("|=");

        Assert.Equal(ECssTokenType.Dash_Match, token.Type);
        Assert.Same(DashMatchToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Match")]
    public void Parse_IncludeMatch_ReturnsIncludeMatchToken()
    {
        var token = FirstToken("~=");

        Assert.Equal(ECssTokenType.Include_Match, token.Type);
        Assert.Same(IncludeMatchToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Match")]
    public void Parse_Column_ReturnsColumnToken()
    {
        var token = FirstToken("||");

        Assert.Equal(ECssTokenType.Column, token.Type);
        Assert.Same(ColumnToken.Instance, token);
    }
    #endregion

    #region CDO/CDC Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "CDO")]
    public void Parse_CDO_ReturnsCdoToken()
    {
        var token = FirstToken("<!--");

        Assert.Equal(ECssTokenType.CDO, token.Type);
        Assert.Same(CdoToken.Instance, token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "CDC")]
    public void Parse_CDC_ReturnsCdcToken()
    {
        var token = FirstToken("-->");

        Assert.Equal(ECssTokenType.CDC, token.Type);
        Assert.Same(CdcToken.Instance, token);
    }
    #endregion

    #region Unicode Range Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "UnicodeRange")]
    public void Parse_UnicodeRangeSingleValue_ReturnsUnicodeRangeToken()
    {
        var token = FirstToken("U+0041");

        Assert.Equal(ECssTokenType.Unicode_Range, token.Type);
        Assert.IsType<UnicodeRangeToken>(token);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "UnicodeRange")]
    public void Parse_UnicodeRangeWithRange_ReturnsUnicodeRangeToken()
    {
        var token = FirstToken("U+0000-00FF");

        Assert.Equal(ECssTokenType.Unicode_Range, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "UnicodeRange")]
    public void Parse_UnicodeRangeWithWildcard_ReturnsUnicodeRangeToken()
    {
        var token = FirstToken("U+00??");

        Assert.Equal(ECssTokenType.Unicode_Range, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "UnicodeRange")]
    public void Parse_UnicodeRangeLowercase_ReturnsUnicodeRangeToken()
    {
        var token = FirstToken("u+0041");

        Assert.Equal(ECssTokenType.Unicode_Range, token.Type);
    }
    #endregion

    #region Delim Token Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Asterisk_ReturnsDelimToken()
    {
        var token = FirstToken("*");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.IsType<DelimToken>(token);
        Assert.Equal('*', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Period_ReturnsDelimToken()
    {
        var token = FirstToken(".");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('.', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Plus_ReturnsDelimToken()
    {
        var token = FirstToken("+");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('+', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_GreaterThan_ReturnsDelimToken()
    {
        var token = FirstToken(">");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('>', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Tilde_ReturnsDelimToken()
    {
        var token = FirstToken("~");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('~', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Equals_ReturnsDelimToken()
    {
        var token = FirstToken("=");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('=', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Slash_ReturnsDelimToken()
    {
        var token = FirstToken("/");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('/', ((DelimToken)token).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Delim")]
    public void Parse_Pipe_ReturnsDelimToken()
    {
        var token = FirstToken("|");

        Assert.Equal(ECssTokenType.Delim, token.Type);
        Assert.Equal('|', ((DelimToken)token).Value);
    }
    #endregion

    #region Comment Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Comment")]
    public void Parse_Comment_IsSkipped()
    {
        var tokens = Tokenize("/* comment */");

        // Comment should be consumed and not returned as a token
        Assert.Single(tokens);
        Assert.Equal(ECssTokenType.EOF, tokens[0].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Comment")]
    public void Parse_CommentBetweenTokens_IsSkipped()
    {
        var tokens = Tokenize("a /* comment */ b");

        // Per CSS Syntax spec, comments are discarded but don't merge surrounding whitespace
        // Tokens: ident("a"), whitespace(before), whitespace(after), ident("b"), EOF = 5 tokens
        Assert.Equal(5, tokens.Length);
        Assert.Equal(ECssTokenType.Ident, tokens[0].Type);
        Assert.Equal(ECssTokenType.Whitespace, tokens[1].Type);
        Assert.Equal(ECssTokenType.Whitespace, tokens[2].Type);
        Assert.Equal(ECssTokenType.Ident, tokens[3].Type);
        Assert.Equal(ECssTokenType.EOF, tokens[4].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Comment")]
    public void Parse_MultiLineComment_IsSkipped()
    {
        var tokens = Tokenize("a /* multi\nline\ncomment */ b");

        // Per CSS Syntax spec, comments are discarded but don't merge surrounding whitespace
        // Tokens: a, whitespace(before comment), whitespace(after comment), b, EOF = 5 tokens
        Assert.Equal(5, tokens.Length);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("TokenType", "Comment")]
    public void Parse_UnterminatedComment_ConsumesToEnd()
    {
        var tokens = Tokenize("a /* unterminated");

        // Should consume to EOF
        Assert.True(tokens.Length >= 1);
    }
    #endregion

    #region Complex CSS Tests
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_SimpleRule_ProducesCorrectTokens()
    {
        var tokens = Tokenize("body { color: red; }");

        // body, space, {, space, color, :, space, red, ;, space, }, EOF
        Assert.True(tokens.Length > 5);
        Assert.Equal(ECssTokenType.Ident, tokens[0].Type);
        Assert.Equal("body", ((IdentToken)tokens[0]).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_ClassSelector_ProducesCorrectTokens()
    {
        var tokens = Tokenize(".class");

        // ., class, EOF
        Assert.Equal(3, tokens.Length);
        Assert.Equal(ECssTokenType.Delim, tokens[0].Type);
        Assert.Equal(ECssTokenType.Ident, tokens[1].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_IdSelector_ProducesCorrectTokens()
    {
        var tokens = Tokenize("#myId");

        Assert.Equal(2, tokens.Length);
        Assert.Equal(ECssTokenType.Hash, tokens[0].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_AttributeSelector_ProducesCorrectTokens()
    {
        var tokens = Tokenize("[type=\"text\"]");

        // [, type, =, "text", ], EOF
        Assert.True(tokens.Length >= 5);
        Assert.Equal(ECssTokenType.SqBracket_Open, tokens[0].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_MediaQuery_ProducesCorrectTokens()
    {
        var tokens = Tokenize("@media screen and (max-width: 600px)");

        Assert.Equal(ECssTokenType.At_Keyword, tokens[0].Type);
        Assert.Equal("media", ((AtToken)tokens[0]).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_RgbFunction_ProducesCorrectTokens()
    {
        var tokens = Tokenize("rgb(255, 0, 0)");

        Assert.Equal(ECssTokenType.FunctionName, tokens[0].Type);
        Assert.Equal("rgb", ((FunctionNameToken)tokens[0]).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_CalcFunction_ProducesCorrectTokens()
    {
        var tokens = Tokenize("calc(100% - 20px)");

        Assert.Equal(ECssTokenType.FunctionName, tokens[0].Type);
        Assert.Equal("calc", ((FunctionNameToken)tokens[0]).Value);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_PseudoClass_ProducesCorrectTokens()
    {
        var tokens = Tokenize(":hover");

        // :, hover, EOF
        Assert.Equal(3, tokens.Length);
        Assert.Equal(ECssTokenType.Colon, tokens[0].Type);
        Assert.Equal(ECssTokenType.Ident, tokens[1].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void Parse_PseudoElement_ProducesCorrectTokens()
    {
        var tokens = Tokenize("::before");

        // :, :, before, EOF
        Assert.Equal(4, tokens.Length);
        Assert.Equal(ECssTokenType.Colon, tokens[0].Type);
        Assert.Equal(ECssTokenType.Colon, tokens[1].Type);
    }
    #endregion

    #region Consume_Number Tests
    [Theory]
    [Trait("Category", "Tokenizer")]
    [Trait("Algorithm", "Consume_Number")]
    [InlineData("0", 0L, ENumericTokenType.Integer)]
    [InlineData("1", 1L, ENumericTokenType.Integer)]
    [InlineData("-1", -1L, ENumericTokenType.Integer)]
    [InlineData("+1", 1L, ENumericTokenType.Integer)]
    [InlineData("123", 123L, ENumericTokenType.Integer)]
    [InlineData("-123", -123L, ENumericTokenType.Integer)]
    public void Consume_Number_Integer(string input, long expectedValue, ENumericTokenType expectedType)
    {
        var stream = new DataConsumer<char>(input.AsMemory());
        CssTokenizer.Consume_Number(stream, out _, out object number, out ENumericTokenType type);

        Assert.Equal(expectedType, type);
        Assert.Equal(expectedValue, number);
    }

    [Theory]
    [Trait("Category", "Tokenizer")]
    [Trait("Algorithm", "Consume_Number")]
    [InlineData("0.0", 0.0, ENumericTokenType.Number)]
    [InlineData("1.0", 1.0, ENumericTokenType.Number)]
    [InlineData("-1.5", -1.5, ENumericTokenType.Number)]
    [InlineData("3.14", 3.14, ENumericTokenType.Number)]
    [InlineData(".5", 0.5, ENumericTokenType.Number)]
    public void Consume_Number_Decimal(string input, double expectedValue, ENumericTokenType expectedType)
    {
        var stream = new DataConsumer<char>(input.AsMemory());
        CssTokenizer.Consume_Number(stream, out _, out object number, out ENumericTokenType type);

        Assert.Equal(expectedType, type);
        Assert.Equal(expectedValue, (double)number, 5);
    }

    [Theory]
    [Trait("Category", "Tokenizer")]
    [Trait("Algorithm", "Consume_Number")]
    [InlineData("1e2", 100.0, ENumericTokenType.Number)]
    [InlineData("1E2", 100.0, ENumericTokenType.Number)]
    [InlineData("1e+2", 100.0, ENumericTokenType.Number)]
    [InlineData("1e-2", 0.01, ENumericTokenType.Number)]
    [InlineData("2.5e3", 2500.0, ENumericTokenType.Number)]
    public void Consume_Number_Scientific(string input, double expectedValue, ENumericTokenType expectedType)
    {
        var stream = new DataConsumer<char>(input.AsMemory());
        CssTokenizer.Consume_Number(stream, out _, out object number, out ENumericTokenType type);

        Assert.Equal(expectedType, type);
        Assert.Equal(expectedValue, (double)number, 5);
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_HyphenFollowedByDigit_ReturnsNumberToken()
    {
        var token = FirstToken("-5");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_HyphenFollowedByIdent_ReturnsIdentToken()
    {
        var token = FirstToken("-webkit");

        Assert.Equal(ECssTokenType.Ident, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_HyphenAlone_ReturnsDelimToken()
    {
        var token = FirstToken("- ");

        Assert.Equal(ECssTokenType.Delim, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_DotFollowedByDigit_ReturnsNumberToken()
    {
        var token = FirstToken(".5");

        Assert.Equal(ECssTokenType.Number, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_DotFollowedByIdent_ReturnsDelimThenIdent()
    {
        var tokens = Tokenize(".class");

        Assert.Equal(ECssTokenType.Delim, tokens[0].Type);
        Assert.Equal(ECssTokenType.Ident, tokens[1].Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_BackslashEscape_ReturnsIdentToken()
    {
        var token = FirstToken("\\30 ");  // \30 followed by space = '0'

        Assert.Equal(ECssTokenType.Ident, token.Type);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "EdgeCase")]
    public void Parse_InvalidBackslash_ReturnsDelimToken()
    {
        var token = FirstToken("\\\n");  // backslash followed by newline is invalid escape

        Assert.Equal(ECssTokenType.Delim, token.Type);
    }
    #endregion

    #region Full CSS Document Tokenization Tests (with DiffEngine validation)
    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void ParseDeclarationBlockTest()
    {
        const string CssTestStr = @"@charset ""UTF-8"";
/*! Hello World this is a CSS comment */
html {
  font-family: sans-serif;
        }

";
        // Tokenize a CSS string and make sure it spits out the correct token sequence
        CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
        CssToken[] Expected = new CssToken[] { new AtToken("charset"), WhitespaceToken.Space, new StringToken("UTF-8"), SemicolonToken.Instance, WhitespaceToken.LFLF,
            WhitespaceToken.LFLF,
            new IdentToken("html"), WhitespaceToken.Space, BracketOpenToken.Instance, WhitespaceToken.LFLF,
            new IdentToken("font-family"), ColonToken.Instance, WhitespaceToken.Space, new IdentToken("sans-serif"), SemicolonToken.Instance, WhitespaceToken.LFLF,
            BracketCloseToken.Instance, WhitespaceToken.LFLF, EOFToken.Instance
        };

        var Engine = new DiffEngine<CssToken>();
        var diff = Engine.Compile(Expected, Actual);
        if (diff.Count > 1)
        {
            Engine.DisplayHTML(diff);
        }

        Assert.Equal(Expected, Actual);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void ParseMultiSpecifierTest()
    {
        const string CssTestStr = @"a:active,
a:hover {
  outline: 0; }";
        // Tokenize a CSS string and make sure it spits out the correct token sequence
        CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
        CssToken[] Expected = new CssToken[] { new IdentToken("a"), ColonToken.Instance, new IdentToken("active"), CommaToken.Instance, WhitespaceToken.LFLF,
            new IdentToken("a"), ColonToken.Instance, new IdentToken("hover"), WhitespaceToken.Space, BracketOpenToken.Instance, WhitespaceToken.LFLF,
            new IdentToken("outline"), ColonToken.Instance, WhitespaceToken.Space, new NumberToken(ENumericTokenType.Integer, "0", 0), SemicolonToken.Instance, WhitespaceToken.Space, BracketCloseToken.Instance, EOFToken.Instance
        };

        var Engine = new DiffEngine<CssToken>();
        var diff = Engine.Compile(Expected, Actual);
        if (diff.Count > 1)
        {
            Engine.DisplayHTML(diff);
        }

        Assert.Equal(Expected, Actual);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void ParseComplexSelectorTest()
    {
        const string CssTestStr = @"input[type=""checkbox""].filter-class-cb:checked + label.filter-class-lb, input[type=""checkbox""].filter-class-cb:active + label.filter-class-lb {}";
        // Tokenize a CSS string and make sure it spits out the correct token sequence
        CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
        CssToken[] Expected = new CssToken[] { new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
            new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new IdentToken("checked"),
            WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
            new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), CommaToken.Instance, WhitespaceToken.Space,
            new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
            new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new IdentToken("active"),
            WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
            new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), WhitespaceToken.Space,
            BracketOpenToken.Instance, BracketCloseToken.Instance,
            EOFToken.Instance
        };

        var Engine = new DiffEngine<CssToken>();
        var diff = Engine.Compile(Expected, Actual);
        if (diff.Count > 1)
        {
            Engine.DisplayHTML(diff);
        }

        Assert.Equal(Expected, Actual);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void ParsePseudoElementTest()
    {
        const string CssTestStr = @"input[type=""checkbox""].filter-class-cb:checked + label.filter-class-lb[data-class-idx=""0""]::after {}";
        // Tokenize a CSS string and make sure it spits out the correct token sequence
        CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
        CssToken[] Expected = new CssToken[] { new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
            new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new IdentToken("checked"), WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
            new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), SqBracketOpenToken.Instance, new IdentToken("data-class-idx"), new DelimToken('='), new StringToken("0"), SqBracketCloseToken.Instance,
            ColonToken.Instance, ColonToken.Instance, new IdentToken("after"), WhitespaceToken.Space,
            BracketOpenToken.Instance, BracketCloseToken.Instance,
            EOFToken.Instance
        };

        var Engine = new DiffEngine<CssToken>();
        var diff = Engine.Compile(Expected, Actual);
        if (diff.Count > 1)
        {
            Engine.DisplayHTML(diff);
        }

        Assert.Equal(Expected, Actual);
    }

    [Fact]
    [Trait("Category", "Tokenizer")]
    [Trait("Category", "Integration")]
    public void ParseNotPseudoClassTest()
    {
        const string CssTestStr = @"input[type=""checkbox""].filter-class-cb:not(:checked) + label.filter-class-lb[data-class-idx=""0""]::after {}";
        // Tokenize a CSS string and make sure it spits out the correct token sequence
        CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
        CssToken[] Expected = new CssToken[] { new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
            new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new FunctionNameToken("not"), ColonToken.Instance, new IdentToken("checked"), ParenthesisCloseToken.Instance,
            WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
            new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), SqBracketOpenToken.Instance, new IdentToken("data-class-idx"), new DelimToken('='), new StringToken("0"), SqBracketCloseToken.Instance,
            ColonToken.Instance, ColonToken.Instance, new IdentToken("after"), WhitespaceToken.Space,
            BracketOpenToken.Instance, BracketCloseToken.Instance,
            EOFToken.Instance
        };

        var Engine = new DiffEngine<CssToken>();
        var diff = Engine.Compile(Expected, Actual);
        if (diff.Count > 1)
        {
            Engine.DisplayHTML(diff);
        }

        Assert.Equal(Expected, Actual);
    }

    [Theory]
    [Trait("Category", "Tokenizer")]
    [Trait("Algorithm", "Consume_Number")]
    [InlineData(-1L, "-1", ENumericTokenType.Integer), InlineData(0L, "0", ENumericTokenType.Integer), InlineData(1L, "1", ENumericTokenType.Integer)]
    [InlineData(-20000.0D, "-2.0E4", ENumericTokenType.Number), InlineData(-1.5D, "-1.5", ENumericTokenType.Number), InlineData(0.0D, "0.0", ENumericTokenType.Number), InlineData(1.0D, "1.0", ENumericTokenType.Number), InlineData(20000.0D, "2.0E4", ENumericTokenType.Number)]
    public void Consume_NumberTest(object expected, string input, ENumericTokenType tokenType)
    {
        var Stream = new DataConsumer<char>(input.AsMemory());
        CssTokenizer.Consume_Number(Stream, out ReadOnlyMemory<char> outResult, out object outNumber, out ENumericTokenType outTokenType);

        Assert.Equal(tokenType, outTokenType);
        Assert.Equal(expected, outNumber);
    }
    #endregion
}
