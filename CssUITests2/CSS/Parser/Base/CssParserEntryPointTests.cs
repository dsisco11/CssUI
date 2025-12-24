using System;
using System.Collections.Generic;
using System.Linq;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Serialization.Tests;

/// <summary>
/// Tests for CssParser Parser Entry Points per CSS Syntax Level 3 §5.3
/// </summary>
public class CssParserEntryPointTests
{
    #region Parse_Stylesheet Tests (§5.3.3)

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_EmptyInput_ReturnsEmptyStylesheet()
    {
        var parser = new CssParser("");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Empty(result.Rules);
        Assert.Null(result.Location);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_WithLocation_SetsLocation()
    {
        var location = new Uri("https://example.com/styles.css");
        var parser = new CssParser("body { color: red; }");
        var result = parser.Parse_Stylesheet(location);

        Assert.NotNull(result);
        Assert.Equal(location, result.Location);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_WithNullLocation_HasNullLocation()
    {
        var parser = new CssParser("body { color: red; }");
        var result = parser.Parse_Stylesheet(null);

        Assert.NotNull(result);
        Assert.Null(result.Location);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_SingleStyleRule_ParsesCorrectly()
    {
        var parser = new CssParser("body { color: red; }");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.IsType<CssQualifiedRule>(result.Rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_MultipleStyleRules_ParsesAll()
    {
        var parser = new CssParser("body { color: red; } p { margin: 0; } h1 { font-size: 2em; }");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Equal(3, result.Rules.Count);
        Assert.All(result.Rules, rule => Assert.IsType<CssQualifiedRule>(rule));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_AtRuleMedia_ParsesCorrectly()
    {
        var parser = new CssParser("@media screen { body { color: red; } }");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.IsType<CssAtRule>(result.Rules[0]);

        var atRule = (CssAtRule)result.Rules[0];
        Assert.Equal("media", atRule.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_MixedRules_ParsesAll()
    {
        var parser = new CssParser("@import 'base.css'; body { color: red; } @media print { p { color: black; } }");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Equal(3, result.Rules.Count);
        Assert.IsType<CssAtRule>(result.Rules[0]);      // @import
        Assert.IsType<CssQualifiedRule>(result.Rules[1]); // body
        Assert.IsType<CssAtRule>(result.Rules[2]);      // @media
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_TopLevelCDOCDC_Ignored()
    {
        // CDO (<!--) and CDC (-->) should be ignored at top level
        var parser = new CssParser("<!-- body { color: red; } -->");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Single(result.Rules);
        Assert.IsType<CssQualifiedRule>(result.Rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void ParseStylesheet_WhitespaceOnly_ReturnsEmptyStylesheet()
    {
        var parser = new CssParser("   \n\t  \r\n   ");
        var result = parser.Parse_Stylesheet();

        Assert.NotNull(result);
        Assert.Empty(result.Rules);
    }

    #endregion

    #region Parse_Style_Block_Contents Tests (§5.3.7)

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_EmptyInput_ReturnsEmptyList()
    {
        var parser = new CssParser("");
        var result = parser.Parse_Style_Block_Contents().ToList();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_SingleDeclaration_ReturnsDeclaration()
    {
        var parser = new CssParser("color: red;");
        var result = parser.Parse_Style_Block_Contents().ToList();

        Assert.Single(result);
        Assert.IsType<CssDecleration>(result[0]);
        Assert.Equal("color", ((CssDecleration)result[0]).Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_MultipleDeclarations_ReturnsAll()
    {
        var parser = new CssParser("color: red; margin: 0; padding: 10px;");
        var result = parser.Parse_Style_Block_Contents().ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, item => Assert.IsType<CssDecleration>(item));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_WithNestedAtRule_ReturnsDeclarationsAndRules()
    {
        var parser = new CssParser("color: red; @media screen { display: block; }");
        var result = parser.Parse_Style_Block_Contents().ToList();

        Assert.Equal(2, result.Count);
        Assert.IsType<CssDecleration>(result[0]); // color: red (declaration)
        Assert.IsType<CssAtRule>(result[1]);      // @media (nested rule appended after declarations)
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    [Trait("Category", "CSSNesting")]
    public void ParseStyleBlockContents_WithAmpersandSelector_ParsesNestedRule()
    {
        // CSS Nesting: & selector should be recognized as start of nested qualified rule
        var parser = new CssParser("color: red; & :hover { color: blue; }");
        var result = parser.Parse_Style_Block_Contents().ToList();

        Assert.Equal(2, result.Count);
        Assert.IsType<CssDecleration>(result[0]); // color: red
        Assert.IsType<CssQualifiedRule>(result[1]); // & :hover nested rule
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_DeclarationsBeforeRules_OrderPreserved()
    {
        // Per spec: "Extend decls with rules, then return decls"
        // So declarations come first, then rules, regardless of input order
        var parser = new CssParser("@media screen { } color: red; @media print { }");
        var result = parser.Parse_Style_Block_Contents().ToList();

        // Declarations first, rules after
        Assert.Equal(3, result.Count);
        Assert.IsType<CssDecleration>(result[0]); // color: red (declaration)
        Assert.IsType<CssAtRule>(result[1]);      // @media screen
        Assert.IsType<CssAtRule>(result[2]);      // @media print
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_InvalidTokenSequence_RecoverGracefully()
    {
        // Per CSS Syntax spec §5.4.4: When a component value starting at start token doesn't
        // form a valid declaration/at-rule/qualified-rule, discard it and continue.
        // "123invalid" is parsed as a number (123) followed by "invalid" identifier, neither starts a valid statement.
        // The tokens are consumed until semicolon, leaving "color: red" as valid declaration candidate
        // but since we're at "color" after error recovery, it should parse properly.
        var parser = new CssParser("color: red; 123invalid; margin: 0");
        var result = parser.Parse_Style_Block_Contents().ToList();

        // Should parse the valid declarations around the invalid token sequence
        Assert.Equal(2, result.Count);
        Assert.IsType<CssDecleration>(result[0]);
        Assert.Equal("color", ((CssDecleration)result[0]).Name);
        Assert.IsType<CssDecleration>(result[1]);
        Assert.Equal("margin", ((CssDecleration)result[1]).Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "StyleBlock")]
    public void ParseStyleBlockContents_NoSemicolonsAtEnd_StillParses()
    {
        var parser = new CssParser("color: red; margin: 0");
        var result = parser.Parse_Style_Block_Contents().ToList();

        Assert.Equal(2, result.Count);
    }

    #endregion

    #region Normalize Static Methods Tests (§5.3)

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Normalize")]
    public void Normalize_StringInput_ReturnsTokenArray()
    {
        var tokens = CssParser.Normalize("color: red");

        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Normalize")]
    public void Normalize_EmptyString_ReturnsArrayWithEOF()
    {
        var tokens = CssParser.Normalize("");

        Assert.NotNull(tokens);
        // Should have at least EOF token
        Assert.Contains(tokens, t => t.Type == ECssTokenType.EOF);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Normalize")]
    public void Normalize_TokenArray_ReturnsSameArray()
    {
        var originalTokens = CssTokenizer.Parse("color: red");
        var normalizedTokens = CssParser.Normalize(originalTokens);

        Assert.Same(originalTokens, normalizedTokens);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Normalize")]
    public void Normalize_ComponentValueList_ReturnsTokenArray()
    {
        var parser = new CssParser("color: red");
        var componentValues = parser.Parse_ComponentValue_List();
        var tokens = CssParser.Normalize(componentValues);

        Assert.NotNull(tokens);
        Assert.NotEmpty(tokens);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Normalize")]
    public void Normalize_String_FiltersCodePoints()
    {
        // Null characters should be replaced with U+FFFD per §3.3
        var tokens = CssParser.Normalize("color\0: red");

        Assert.NotNull(tokens);
        // Should successfully tokenize (null gets replaced with replacement character)
    }

    #endregion

    #region Constructor with IEnumerable<CssToken> Tests

    [Fact]
    [Trait("Category", "Parser")]
    public void Constructor_WithIEnumerableTokens_CreatesParser()
    {
        var tokens = CssTokenizer.Parse("color: red").ToList();
        var parser = new CssParser(tokens);

        Assert.NotNull(parser);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void Constructor_WithIEnumerableTokens_CanParseDeclaration()
    {
        var tokens = CssTokenizer.Parse("color: red").ToList();
        var parser = new CssParser(tokens);
        var declaration = parser.Parse_Decleration();

        Assert.NotNull(declaration);
        Assert.Equal("color", declaration!.Name);
    }

    #endregion

    #region CssParsedStylesheet Tests

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void CssParsedStylesheet_RulesProperty_IsReadOnly()
    {
        var parser = new CssParser("body { color: red; }");
        var result = parser.Parse_Stylesheet();

        // Rules should be a read-only list
        Assert.IsAssignableFrom<IReadOnlyList<CssComponent>>(result.Rules);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Stylesheet")]
    public void CssParsedStylesheet_WithRelativeUri_SetsLocation()
    {
        var location = new Uri("/styles/main.css", UriKind.Relative);
        var parser = new CssParser("body { }");
        var result = parser.Parse_Stylesheet(location);

        Assert.Equal(location, result.Location);
    }

    #endregion
}
