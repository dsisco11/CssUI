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
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_SingleCondition_ReturnsMediaQueryList()
    {
        var parser = new CssParser("(width <= 320px)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_NotCondition_ReturnsMediaQueryList()
    {
        var parser = new CssParser("not (color)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_MultipleConditions_ReturnsMediaQueryList()
    {
        var parser = new CssParser("(width <= 320px) or (height <= 100px)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_NestedConditions_ReturnsMediaQueryList()
    {
        var parser = new CssParser("(not (color)) or (width <= 320px)");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_ComplexQuery_ReturnsMediaQueryList()
    {
        // Note: This is a complex nested condition with mixed and/or at different levels
        var parser = new CssParser("(not ((color) or (grid))) or ((width <= 320px) and (height < 1080px))");
        Assert.NotNull(parser.Parse_Media_Query_List(document));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_BooleanFeature_ReturnsMediaQueryList()
    {
        // (hover) is a valid boolean media feature
        var parser = new CssParser("(hover)");
        var result = parser.Parse_Media_Query_List(document);
        Assert.NotNull(result);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "MediaQuery")]
    public void ParseMediaQueryList_MixedCombinators_ThrowsCssSyntaxError()
    {
        // Mixing 'and' and 'or' at the same level is invalid per spec
        var parser = new CssParser("(color) and (pointer) or (hover)");
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

    #region At-Rule Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_CharsetAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@charset \"UTF-8\";");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("charset", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_ImportAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@import url('styles.css');");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("import", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_FontFaceAtRule_ParsesBlockCorrectly()
    {
        var parser = new CssParser("@font-face { font-family: MyFont; src: url(myfont.woff); }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("font-face", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_KeyframesAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("keyframes", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_SupportsAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@supports (display: grid) { .container { display: grid; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("supports", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_NamespaceAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@namespace svg url(http://www.w3.org/2000/svg);");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("namespace", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_PageAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@page :first { margin: 2cm; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("page", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_MultipleAtRules_ParsesAll()
    {
        var parser = new CssParser("@charset \"UTF-8\"; @import url('reset.css'); @import url('styles.css');");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Equal(3, rules.Count);
        Assert.All(rules, r => Assert.IsType<CssAtRule>(r));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_NestedMediaRule_ParsesCorrectly()
    {
        var parser = new CssParser("@media print { @page { margin: 1cm; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("media", atRule!.Name);
        Assert.NotNull(atRule.Block);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void ParseRuleList_LayerAtRule_ParsesCorrectly()
    {
        var parser = new CssParser("@layer base { body { margin: 0; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        var atRule = rules[0] as CssAtRule;
        Assert.NotNull(atRule);
        Assert.Equal("layer", atRule!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "AtRule")]
    public void CssAtRule_Encode_ContainsAtSymbol()
    {
        var parser = new CssParser("@media screen { body { color: red; } }");
        var rules = parser.Parse_Rule_List().ToList();
        var atRule = rules[0] as CssAtRule;

        var encoded = atRule!.Encode();

        Assert.StartsWith("@", encoded);
        Assert.Contains("media", encoded);
    }
    #endregion

    #region QualifiedRule Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_SimpleSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_ClassSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser(".my-class { font-size: 14px; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_IdSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("#my-id { background: blue; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_ComplexSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("div.container > p.text { line-height: 1.5; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_PseudoClassSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("a:hover { color: blue; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_PseudoElementSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("p::first-line { text-transform: uppercase; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_AttributeSelector_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("[data-active] { display: block; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_MultipleRules_ParsesAll()
    {
        var parser = new CssParser("div { color: red; } span { color: blue; } p { color: green; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Equal(3, rules.Count);
        Assert.All(rules, r => Assert.IsType<CssQualifiedRule>(r));
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_MixedRulesAndAtRules_ParsesBothTypes()
    {
        var parser = new CssParser("@import url('styles.css'); div { color: red; } @media print { body { font-size: 12pt; } }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Equal(3, rules.Count);
        Assert.IsType<CssAtRule>(rules[0]);
        Assert.IsType<CssQualifiedRule>(rules[1]);
        Assert.IsType<CssAtRule>(rules[2]);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void ParseRuleList_SelectorList_ParsesAsQualifiedRule()
    {
        var parser = new CssParser("h1, h2, h3 { font-weight: bold; }");
        var rules = parser.Parse_Rule_List().ToList();

        Assert.Single(rules);
        Assert.IsType<CssQualifiedRule>(rules[0]);
        var rule = rules[0] as CssQualifiedRule;
        // Prelude should contain multiple selectors separated by commas
        Assert.True(rule!.Prelude.Count > 1);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void CssQualifiedRule_HasBlock()
    {
        var parser = new CssParser("div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();
        var rule = rules[0] as CssQualifiedRule;

        Assert.NotNull(rule!.Block);
        Assert.True(rule.Block.Values.Count > 0);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "QualifiedRule")]
    public void CssQualifiedRule_Encode_ReturnsCorrectFormat()
    {
        var parser = new CssParser("div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();
        var rule = rules[0] as CssQualifiedRule;

        var encoded = rule!.Encode();

        Assert.Contains("div", encoded);
        Assert.Contains("{", encoded);
    }
    #endregion

    #region SimpleBlock Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "SimpleBlock")]
    public void CssSimpleBlock_CurlyBrace_HasCorrectStartToken()
    {
        var parser = new CssParser("{ color: red }");
        var values = parser.Parse_ComponentValue_List();
        var block = values.First(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;

        Assert.NotNull(block);
        Assert.Equal(ECssTokenType.Bracket_Open, block!.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "SimpleBlock")]
    public void CssSimpleBlock_Parenthesis_HasCorrectStartToken()
    {
        var parser = new CssParser("(1 + 2)");
        var values = parser.Parse_ComponentValue_List();
        var block = values.First(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;

        Assert.NotNull(block);
        Assert.Equal(ECssTokenType.Parenth_Open, block!.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "SimpleBlock")]
    public void CssSimpleBlock_SquareBracket_HasCorrectStartToken()
    {
        var parser = new CssParser("[attr=value]");
        var values = parser.Parse_ComponentValue_List();
        var block = values.First(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;

        Assert.NotNull(block);
        Assert.Equal(ECssTokenType.SqBracket_Open, block!.StartToken.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "SimpleBlock")]
    public void CssSimpleBlock_ContainsValues()
    {
        var parser = new CssParser("{ color: red }");
        var values = parser.Parse_ComponentValue_List();
        var block = values.First(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;

        Assert.NotNull(block);
        Assert.True(block!.Values.Count > 0);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "SimpleBlock")]
    public void CssSimpleBlock_Encode_ReturnsCorrectFormat()
    {
        var parser = new CssParser("{ color: red }");
        var values = parser.Parse_ComponentValue_List();
        var block = values.First(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;

        var encoded = block!.Encode();

        Assert.Contains("{", encoded);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "SimpleBlock")]
    public void CssSimpleBlock_EmptyBlock_ParsesCorrectly()
    {
        var parser = new CssParser("{}");
        var values = parser.Parse_ComponentValue_List();
        var block = values.First(v => v.Type == ECssTokenType.SimpleBlock) as CssSimpleBlock;

        Assert.NotNull(block);
        Assert.Empty(block!.Values);
    }
    #endregion

    #region CssFunction Encoding Tests
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Function")]
    public void CssFunction_Encode_ContainsParentheses()
    {
        var parser = new CssParser("rgb(255, 0, 0)");
        var values = parser.Parse_ComponentValue_List();
        var func = values.First(v => v.Type == ECssTokenType.Function) as CssFunction;

        var encoded = func!.Encode();

        Assert.Contains("(", encoded);
        Assert.Contains(")", encoded);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Function")]
    public void CssFunction_Encode_ContainsName()
    {
        var parser = new CssParser("rgb(255, 0, 0)");
        var values = parser.Parse_ComponentValue_List();
        var func = values.First(v => v.Type == ECssTokenType.Function) as CssFunction;

        var encoded = func!.Encode();

        Assert.StartsWith("rgb", encoded);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Function")]
    public void CssFunction_EmptyFunction_EncodesCorrectly()
    {
        var parser = new CssParser("attr()");
        var values = parser.Parse_ComponentValue_List();
        var func = values.First(v => v.Type == ECssTokenType.Function) as CssFunction;

        var encoded = func!.Encode();

        Assert.Equal("attr()", encoded);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Function")]
    public void CssFunction_WithArguments_HasArguments()
    {
        var parser = new CssParser("rgb(255, 128, 64)");
        var values = parser.Parse_ComponentValue_List();
        var func = values.First(v => v.Type == ECssTokenType.Function) as CssFunction;

        Assert.True(func!.Arguments.Count > 0);
    }
    #endregion

    #region Advanced Declaration Parsing Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_VendorPrefixedProperty_ParsesCorrectly()
    {
        var parser = new CssParser("-webkit-transform: rotate(45deg);");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.NotNull(decl);
        Assert.Equal("-webkit-transform", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_CaseInsensitiveImportant_ParsesCorrectly()
    {
        var parser = new CssParser("color: red !IMPORTANT;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.True(decl!.Important);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_ImportantWithExtraWhitespace_ParsesCorrectly()
    {
        var parser = new CssParser("color: red   !   important;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.True(decl!.Important);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_MultipleFunctionsInValue_ParsesAllFunctions()
    {
        var parser = new CssParser("background: linear-gradient(to right, red, blue), url('image.png');");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        // Values should contain multiple components
        Assert.True(decl!.Values.Count >= 2);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_CalcWithNesting_ParsesCorrectly()
    {
        var parser = new CssParser("width: calc(100% - calc(20px + 10px));");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.Equal("width", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_ColorFormats_ParseCorrectly()
    {
        var parser = new CssParser("color: #fff; background: rgb(0,0,0); border-color: hsl(120, 100%, 50%);");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Equal(3, declarations.Count);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_ShorthandProperty_ParsesAllValues()
    {
        var parser = new CssParser("border: 1px solid red;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.Equal("border", decl!.Name);
        // Should have multiple tokens for width, style, and color
        Assert.True(decl.Values.Count >= 3);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_FontProperty_ParsesCorrectly()
    {
        var parser = new CssParser("font: bold 16px/1.5 Arial, sans-serif;");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.Equal("font", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_GridProperty_ParsesCorrectly()
    {
        var parser = new CssParser("grid-template-columns: repeat(3, 1fr);");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.Equal("grid-template-columns", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_InvalidDeclarationSkipped_ContinuesParsing()
    {
        // Invalid: missing colon - should be skipped, but next declaration should parse
        var parser = new CssParser("invalid-property; color: red;");
        var declarations = parser.Parse_Decleration_List().ToList();

        // The second valid declaration should still be parsed
        Assert.Single(declarations);
    }
    #endregion

    #region Unicode and Special Character Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_UnicodeContent_ParsesCorrectly()
    {
        var parser = new CssParser("content: \"\\2713\";");
        var declarations = parser.Parse_Decleration_List().ToList();

        Assert.Single(declarations);
        var decl = declarations[0] as CssDecleration;
        Assert.Equal("content", decl!.Name);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseCssValue_UrlWithQuotes_ReturnsKeywordOrString()
    {
        // url() values are special - currently throws NotSupportedException per the code
        var parser = new CssParser("\"path/to/file.css\"");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.STRING, value.Type);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_CommentsStripped_ReturnsCleanTokens()
    {
        // CSS comments should be stripped during tokenization
        var parser = new CssParser("/* comment */ red /* another */");
        var values = parser.Parse_ComponentValue_List();

        // Should only have the ident token, comments are filtered
        var identTokens = values.Where(v => v.Type == ECssTokenType.Ident).ToList();
        Assert.Single(identTokens);
    }
    #endregion

    #region Error Recovery Tests
    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRuleList_UnclosedBlock_RecoversGracefully()
    {
        // Unclosed block at end
        var parser = new CssParser("div { color: red");
        var rules = parser.Parse_Rule_List().ToList();

        // Should still parse what it can
        Assert.Single(rules);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseComponentValueList_UnterminatedString_RecoversGracefully()
    {
        var parser = new CssParser("\"unterminated");
        var values = parser.Parse_ComponentValue_List();

        // Should still return some tokens
        Assert.NotEmpty(values);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseDeclerationList_MissingValue_SkipsDeclaration()
    {
        var parser = new CssParser("color:;");
        var declarations = parser.Parse_Decleration_List().ToList();

        // Declaration with empty value should still be returned
        Assert.Single(declarations);
    }

    [Fact]
    [Trait("Category", "Parser")]
    public void ParseRuleList_InvalidAtRuleRecovers_ContinuesParsing()
    {
        var parser = new CssParser("@invalid-rule; div { color: red; }");
        var rules = parser.Parse_Rule_List().ToList();

        // Should parse both the at-rule and the qualified rule
        Assert.Equal(2, rules.Count);
    }
    #endregion

    #region Hex Color Parsing Tests (Phase 7.1)
    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColor3Digit_ReturnsColorValue()
    {
        // #RGB format (3-digit hex)
        var parser = new CssParser("#f00");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColor4Digit_ReturnsColorValueWithAlpha()
    {
        // #RGBA format (4-digit hex)
        var parser = new CssParser("#f008");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(136, color.A); // 0x88 = 136
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColor6Digit_ReturnsColorValue()
    {
        // #RRGGBB format (6-digit hex)
        var parser = new CssParser("#ff5500");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(85, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColor8Digit_ReturnsColorValueWithAlpha()
    {
        // #RRGGBBAA format (8-digit hex)
        var parser = new CssParser("#ff550080");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(85, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(128, color.A);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorUppercase_ReturnsColorValue()
    {
        // Uppercase hex should work
        var parser = new CssParser("#FF5500");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(85, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorMixedCase_ReturnsColorValue()
    {
        // Mixed case hex should work
        var parser = new CssParser("#fF5500");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(85, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorBlack_ReturnsBlack()
    {
        var parser = new CssParser("#000000");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorWhite_ReturnsWhite()
    {
        var parser = new CssParser("#ffffff");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorShortBlack_ReturnsBlack()
    {
        var parser = new CssParser("#000");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorShortWhite_ReturnsWhite()
    {
        var parser = new CssParser("#fff");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorTransparent_ReturnsTransparent()
    {
        var parser = new CssParser("#00000000");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    public void ParseCssValue_HexColorShortTransparent_ReturnsTransparent()
    {
        var parser = new CssParser("#0000");
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Theory]
    [Trait("Category", "Parser")]
    [Trait("Category", "Color")]
    [InlineData("#abc", 170, 187, 204, 255)]      // #RGB
    [InlineData("#abcd", 170, 187, 204, 221)]     // #RGBA
    [InlineData("#aabbcc", 170, 187, 204, 255)]   // #RRGGBB
    [InlineData("#aabbccdd", 170, 187, 204, 221)] // #RRGGBBAA
    public void ParseCssValue_HexColorVariousFormats_ReturnsCorrectColor(string hex, int r, int g, int b, int a)
    {
        var parser = new CssParser(hex);
        var value = parser.Parse_CssValue();

        Assert.Equal(ECssValueTypes.COLOR, value.Type);
        var color = value.AsColor();
        Assert.Equal(r, color.R);
        Assert.Equal(g, color.G);
        Assert.Equal(b, color.B);
        Assert.Equal(a, color.A);
    }
    #endregion
}
