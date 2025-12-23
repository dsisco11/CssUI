using System.Linq;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using CssUI.DOM;
using Xunit;

namespace CssUITests.CSS.Serialization.Tests;

/// <summary>
/// Tests for CssParser - CSS Syntax Level 3 compliant parser
/// </summary>
public class CssParserTests
{
    #region Setup
    private static readonly Document document;
    private static readonly DOMImplementation DOM;

    static CssParserTests()
    {
        // Many of the parser's functions will require a DOM and document
        DOM = new DOMImplementation();
        document = DOM.createDocument("CssUI", "cssui");
    }
    #endregion

    #region Constructor Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void Constructor_WithValidCssText_CreatesParser()
    {
        var parser = new CssParser("color: red");
        Assert.NotNull(parser);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void Constructor_WithEmptyString_CreatesParser()
    {
        var parser = new CssParser("");
        Assert.NotNull(parser);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void Constructor_WithTokenArray_CreatesParser()
    {
        var tokens = CssTokenizer.Parse("color: red");
        var parser = new CssParser(tokens);
        Assert.NotNull(parser);
    }
    #endregion

    #region Declaration Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseDecleration_SimpleProperty_ReturnsDeclaration()
    {
        var parser = new CssParser("color: red");
        var declaration = parser.Parse_Decleration();

        Assert.NotNull(declaration);
        Assert.Equal("color", declaration!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_SingleDeclaration_ReturnsListWithOneItem()
    {
        var parser = new CssParser("color: red;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_MultipleDeclarations_ReturnsAllItems()
    {
        var parser = new CssParser("color: red; background: blue; font-size: 14px;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Equal(3, declarations.Count);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_DeclarationWithImportant_SetsImportantFlag()
    {
        var parser = new CssParser("color: red !important;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.True(decl!.Important);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_DeclarationWithoutImportant_ImportantIsFalse()
    {
        var parser = new CssParser("color: red;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.False(decl!.Important);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_EmptyInput_ReturnsEmptyList()
    {
        var parser = new CssParser("");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Empty(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_WhitespaceOnly_ReturnsEmptyList()
    {
        var parser = new CssParser("   \t\n  ");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Empty(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_SemicolonOnly_ReturnsEmptyList()
    {
        var parser = new CssParser(";;;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Empty(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_DeclarationWithUnitValue_ParsesCorrectly()
    {
        var parser = new CssParser("width: 100px;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("width", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_DeclarationWithPercentageValue_ParsesCorrectly()
    {
        var parser = new CssParser("width: 50%;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("width", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_DeclarationWithMultipleValues_ParsesAllValues()
    {
        var parser = new CssParser("margin: 10px 20px 30px 40px;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("margin", decl!.Name);
        // Values list should contain all 4 dimension tokens (plus whitespace)
        Assert.True(decl.Values.Count >= 4);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_DeclarationWithCustomProperty_ParsesCorrectly()
    {
        var parser = new CssParser("--my-color: blue;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("--my-color", decl!.Name);
    }
    #endregion

    #region Value List Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_SingleIdent_ReturnsSingleValue()
    {
        var parser = new CssParser("red");
        var values = parser.Parse_ComponentValue_List();

        // Should contain at least one ident token
        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.Ident);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_MultipleIdents_ReturnsAllValues()
    {
        var parser = new CssParser("red green blue");
        var values = parser.Parse_ComponentValue_List();

        // Should contain 3 ident tokens (plus whitespace)
        var identCount = values.Count(v => v.Type == ECssTokenType.Ident);
        Assert.Equal(3, identCount);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_NumberValue_ReturnsNumberToken()
    {
        var parser = new CssParser("42");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.Number);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_DimensionValue_ReturnsDimensionToken()
    {
        var parser = new CssParser("100px");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.Dimension);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_PercentageValue_ReturnsPercentageToken()
    {
        var parser = new CssParser("50%");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.Percentage);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_StringValue_ReturnsStringToken()
    {
        var parser = new CssParser("\"hello world\"");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.String);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_EmptyInput_ReturnsEmptyList()
    {
        var parser = new CssParser("");
        var values = parser.Parse_ComponentValue_List();

        Assert.Empty(values);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValue_SingleToken_ReturnsToken()
    {
        var parser = new CssParser("red");
        var value = parser.Parse_ComponentValue();

        Assert.NotNull(value);
        Assert.Equal(ECssTokenType.Ident, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValue_WithTrailingWhitespace_ReturnsToken()
    {
        var parser = new CssParser("red   ");
        var value = parser.Parse_ComponentValue();

        Assert.NotNull(value);
        Assert.Equal(ECssTokenType.Ident, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValue_WithLeadingWhitespace_ReturnsToken()
    {
        var parser = new CssParser("   red");
        var value = parser.Parse_ComponentValue();

        Assert.NotNull(value);
        Assert.Equal(ECssTokenType.Ident, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValue_EmptyInput_ThrowsCssSyntaxError()
    {
        var parser = new CssParser("");

        Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_ComponentValue());
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValue_WhitespaceOnly_ThrowsCssSyntaxError()
    {
        var parser = new CssParser("   ");

        Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_ComponentValue());
    }
    #endregion

    #region Function Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_FunctionWithNoArgs_ReturnsFunction()
    {
        var parser = new CssParser("attr()");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.Function);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_FunctionWithSingleArg_ReturnsFunction()
    {
        var parser = new CssParser("rgb(255)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("rgb", func!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_RgbFunction_ParsesCorrectly()
    {
        var parser = new CssParser("rgb(255, 128, 64)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("rgb", func!.Name);
        // Arguments should contain numbers and commas
        Assert.True(func.Arguments.Count > 0);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_RgbaFunction_ParsesCorrectly()
    {
        var parser = new CssParser("rgba(255, 128, 64, 0.5)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("rgba", func!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_CalcFunction_ParsesCorrectly()
    {
        var parser = new CssParser("calc(100% - 20px)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("calc", func!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_VarFunction_ParsesCorrectly()
    {
        var parser = new CssParser("var(--my-variable)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("var", func!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseComponentValueList_NestedFunctions_ParsesCorrectly()
    {
        var parser = new CssParser("calc(var(--width) * 2)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("calc", func!.Name);
        // Should contain nested var function
        Assert.Contains(func.Arguments, arg => arg.Type == ECssTokenType.Function);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_LinearGradient_ParsesCorrectly()
    {
        var parser = new CssParser("linear-gradient(to right, red, blue)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("linear-gradient", func!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_TransformFunctions_ParsesCorrectly()
    {
        var parser = new CssParser("rotate(45deg)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("rotate", func!.Name);
    }
    #endregion

    #region CssValue Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_Number_ReturnsNumberValue()
    {
        var parser = new CssParser("42");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.NUMBER, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_Dimension_ReturnsDimensionValue()
    {
        var parser = new CssParser("100px");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.DIMENSION, value.Type);
        Assert.Equal(ECssUnit.PX, value.Unit);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_EmDimension_ReturnsCorrectUnit()
    {
        var parser = new CssParser("2em");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.DIMENSION, value.Type);
        Assert.Equal(ECssUnit.EM, value.Unit);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_RemDimension_ReturnsCorrectUnit()
    {
        var parser = new CssParser("1.5rem");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.DIMENSION, value.Type);
        Assert.Equal(ECssUnit.REM, value.Unit);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_VwDimension_ReturnsCorrectUnit()
    {
        var parser = new CssParser("50vw");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.DIMENSION, value.Type);
        Assert.Equal(ECssUnit.VW, value.Unit);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_VhDimension_ReturnsCorrectUnit()
    {
        var parser = new CssParser("100vh");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.DIMENSION, value.Type);
        Assert.Equal(ECssUnit.VH, value.Unit);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_Percentage_ReturnsPercentValue()
    {
        var parser = new CssParser("75%");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.PERCENT, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_Keyword_ReturnsKeywordValue()
    {
        var parser = new CssParser("auto");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.KEYWORD, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_String_ReturnsStringValue()
    {
        var parser = new CssParser("\"hello\"");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.STRING, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_Function_ReturnsFunctionValue()
    {
        var parser = new CssParser("rgb(255, 0, 0)");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.FUNCTION, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_EmptyInput_ReturnsNullValue()
    {
        var parser = new CssParser("");
        var value = parser.Parse_CssValue();

        Assert.True(value.IsNull);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_WhitespaceOnly_ReturnsNullValue()
    {
        var parser = new CssParser("   ");
        var value = parser.Parse_CssValue();

        Assert.True(value.IsNull);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_ZeroWithoutUnit_ReturnsNumberValue()
    {
        var parser = new CssParser("0");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.NUMBER, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_NegativeNumber_ReturnsNumberValue()
    {
        var parser = new CssParser("-42");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.NUMBER, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_DecimalNumber_ReturnsNumberValue()
    {
        var parser = new CssParser("3.14159");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.NUMBER, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_ScientificNotation_ReturnsNumberValue()
    {
        var parser = new CssParser("1e10");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.NUMBER, value.Type);
    }
    #endregion

    #region Rule Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRuleList_EmptyInput_ReturnsEmptyList()
    {
        var parser = new CssParser("");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Empty(rules);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRuleList_SingleAtRule_ReturnsSingleRule()
    {
        var parser = new CssParser("@charset \"UTF-8\";");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssAtRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRuleList_AtRuleWithBlock_ParsesBlock()
    {
        var parser = new CssParser("@media screen { body { color: red; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.NotNull(atRule!.Block);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRule_SingleRule_ReturnsRule()
    {
        var parser = new CssParser("div { color: red; }");
        var rule = parser.Parse_Rule();

        Assert.NotNull(rule);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRule_EmptyInput_ThrowsCssSyntaxError()
    {
        var parser = new CssParser("");

        Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_Rule());
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRule_WhitespaceOnly_ThrowsCssSyntaxError()
    {
        var parser = new CssParser("   ");

        Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_Rule());
    }
    #endregion

    #region Simple Block Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_CurlyBraceBlock_ReturnsSimpleBlock()
    {
        var parser = new CssParser("{ color: red }");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.SimpleBlock);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_ParenthesisBlock_ReturnsSimpleBlock()
    {
        var parser = new CssParser("(1 + 2)");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.SimpleBlock);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_SquareBracketBlock_ReturnsSimpleBlock()
    {
        var parser = new CssParser("[attr=value]");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.SimpleBlock);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_NestedBlocks_ParsesCorrectly()
    {
        var parser = new CssParser("{ { nested } }");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        var block = values.FirstOrDefault(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;
        Assert.NotNull(block);
    }
    #endregion

    #region Media Query Tests (Known Bug)
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_SingleCondition_ReturnsMediaQueryList()
    {
        var parser = new CssParser("@media (width <= 320px)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_NotCondition_ReturnsMediaQueryList()
    {
        var parser = new CssParser("@media not (color)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_MultipleConditions_ReturnsMediaQueryList()
    {
        var parser = new CssParser("@media (width <= 320px) or (height <= 100px)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_NestedConditions_ReturnsMediaQueryList()
    {
        var parser = new CssParser("@media (not (color)) or (width <= 320px)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_ComplexQuery_ReturnsMediaQueryList()
    {
        var parser = new CssParser("@media (not ((color) or (grid)) or ((width <= 320px) and (height < 1080px))");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_UnsupportedFeature_ThrowsCssSyntaxError()
    {
        var parser = new CssParser("@media (hover)");
        Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Bug")]
    public void ParseMediaQueryList_MixedCombinators_ThrowsCssSyntaxError()
    {
        var parser = new CssParser("@media not (color) or (grid)");
        Assert.Throws<CssSyntaxErrorException>(() => parser.Parse_Media_Query_List(document));
    }
    #endregion

    #region Edge Cases
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_MissingSemicolon_StillParses()
    {
        var parser = new CssParser("color: red");
        var declarations = parser.Parse_Decleration_List().ToList();

        // Parser should handle missing semicolon gracefully
        Assert.Single(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_ExtraSemicolons_IgnoresEmpty()
    {
        var parser = new CssParser("color: red;;; background: blue;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Equal(2, declarations.Count);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_MixedWhitespace_ParsesCorrectly()
    {
        var parser = new CssParser("  color  :  red  ;  ");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_UnclosedFunction_HandleGracefully()
    {
        // Parser should return what it can parse
        var parser = new CssParser("rgb(255, 0, 0");
        var value = parser.Parse_CssValue();

        // Should still return a function value even if unclosed
        Assert.Equal(ECssValueTypes.FUNCTION, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_HashToken_ReturnsHashToken()
    {
        var parser = new CssParser("#ffffff");
        var values = parser.Parse_ComponentValue_List();

        Assert.NotEmpty(values);
        Assert.Contains(values, v => v.Type == ECssTokenType.Hash);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_AtRuleInList_ParsesAsAtRule()
    {
        // Per spec, at-rules in declaration list should be parsed
        var parser = new CssParser("@apply --my-theme; color: red;");
        var components = parser.Parse_Decleration_List().ToList();

        Assert.Equal(2, components.Count);
        Assert.IsType<CssAtRule>(components[0]);
    }
    #endregion

    #region Encode/Serialization Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void CssDecleration_Encode_ReturnsCorrectString()
    {
        var parser = new CssParser("color: red;");
        var declarations = parser.Parse_Decleration_List().ToList();
        var decl = declarations[0] as CssDecleration;

        var encoded = decl!.Encode();

        Assert.Contains("color", encoded);
        Assert.Contains(":", encoded);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void CssDecleration_EncodeWithImportant_ContainsImportantFlag()
    {
        var parser = new CssParser("color: red !important;");
        var declarations = parser.Parse_Decleration_List().ToList();
        var decl = declarations[0] as CssDecleration;

        var encoded = decl!.Encode();

        Assert.Contains("!important", encoded);
    }
    #endregion
}
