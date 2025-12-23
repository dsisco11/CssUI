using System;
using System.Linq;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.ComplexTokens.Tests;

/// <summary>
/// Tests for CssFunction - represents a CSS function like rgb(), calc(), etc.
/// </summary>
public class CssFunctionTests
{
    #region Constructor Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Constructor_WithName_SetsNameProperty()
    {
        var func = new CssFunction("rgb".AsSpan());
        Assert.Equal("rgb", func.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Constructor_WithName_SetsTypeToFunction()
    {
        var func = new CssFunction("calc".AsSpan());
        Assert.Equal(ECssTokenType.Function, func.Type);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Constructor_WithEmptyName_CreatesFunction()
    {
        var func = new CssFunction("".AsSpan());
        Assert.Equal("", func.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Constructor_WithHyphenatedName_PreservesName()
    {
        var func = new CssFunction("linear-gradient".AsSpan());
        Assert.Equal("linear-gradient", func.Name);
    }
    #endregion

    #region Arguments Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Arguments_InitiallyEmpty()
    {
        var func = new CssFunction("test".AsSpan());
        Assert.Empty(func.Arguments);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Arguments_CanAddSingleToken()
    {
        var func = new CssFunction("attr".AsSpan());
        func.Arguments.Add(new IdentToken("href"));

        Assert.Single(func.Arguments);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Arguments_CanAddMultipleTokens()
    {
        var func = new CssFunction("rgb".AsSpan());
        func.Arguments.Add(new NumberToken(ENumericTokenType.Integer, "255".AsSpan(), 255));
        func.Arguments.Add(CommaToken.Instance);
        func.Arguments.Add(new NumberToken(ENumericTokenType.Integer, "128".AsSpan(), 128));
        func.Arguments.Add(CommaToken.Instance);
        func.Arguments.Add(new NumberToken(ENumericTokenType.Integer, "64".AsSpan(), 64));

        Assert.Equal(5, func.Arguments.Count);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Arguments_CanContainNestedFunction()
    {
        var outerFunc = new CssFunction("calc".AsSpan());
        var innerFunc = new CssFunction("var".AsSpan());
        innerFunc.Arguments.Add(new IdentToken("--my-width"));

        outerFunc.Arguments.Add(innerFunc);

        Assert.Single(outerFunc.Arguments);
        Assert.IsType<CssFunction>(outerFunc.Arguments[0]);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Arguments_CanContainMixedTokenTypes()
    {
        var func = new CssFunction("calc".AsSpan());
        func.Arguments.Add(new PercentageToken("100".AsSpan(), 100.0));
        func.Arguments.Add(new DelimToken('-'));
        func.Arguments.Add(new DimensionToken(ENumericTokenType.Integer, "20".AsSpan(), 20, "px".AsSpan()));

        Assert.Equal(3, func.Arguments.Count);
        Assert.IsType<PercentageToken>(func.Arguments[0]);
        Assert.IsType<DelimToken>(func.Arguments[1]);
        Assert.IsType<DimensionToken>(func.Arguments[2]);
    }
    #endregion

    #region Encode Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Encode_EmptyFunction_ReturnsNameWithParens()
    {
        var func = new CssFunction("attr".AsSpan());
        Assert.Equal("attr()", func.Encode());
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Encode_WithSingleIdent_ReturnsCorrectFormat()
    {
        var func = new CssFunction("attr".AsSpan());
        func.Arguments.Add(new IdentToken("href"));

        var encoded = func.Encode();
        Assert.Equal("attr(href)", encoded);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Encode_WithNumber_ReturnsCorrectFormat()
    {
        var func = new CssFunction("rotate".AsSpan());
        func.Arguments.Add(new DimensionToken(ENumericTokenType.Integer, "45".AsSpan(), 45, "deg".AsSpan()));

        var encoded = func.Encode();
        Assert.Equal("rotate(45deg)", encoded);
    }
    #endregion

    #region Parser Integration Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesSimpleFunction()
    {
        var parser = new CssParser("rgb(255, 0, 0)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("rgb", func!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesFunctionWithNumbers()
    {
        var parser = new CssParser("rgba(255, 128, 64, 0.5)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("rgba", func!.Name);
        Assert.True(func.Arguments.Count > 0);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesCalcFunction()
    {
        var parser = new CssParser("calc(100% - 20px)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("calc", func!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesVarFunction()
    {
        var parser = new CssParser("var(--primary-color)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("var", func!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesNestedFunctions()
    {
        var parser = new CssParser("calc(var(--size) * 2)");
        var values = parser.Parse_ComponentValue_List();

        var calcFunc = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(calcFunc);
        Assert.Equal("calc", calcFunc!.Name);

        // Should contain nested var function
        var nestedVar = calcFunc.Arguments.FirstOrDefault(a => a.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(nestedVar);
        Assert.Equal("var", nestedVar!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesLinearGradient()
    {
        var parser = new CssParser("linear-gradient(to right, red, blue)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("linear-gradient", func!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesTransformFunctions()
    {
        var parser = new CssParser("translateX(100px)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        // Function names are lowercased by the tokenizer
        Assert.Equal("translatex", func!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesMinMaxFunctions()
    {
        var parser = new CssParser("min(100px, 50%)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("min", func!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Parser_ParsesClampFunction()
    {
        var parser = new CssParser("clamp(10px, 5%, 100px)");
        var values = parser.Parse_ComponentValue_List();

        var func = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;
        Assert.NotNull(func);
        Assert.Equal("clamp", func!.Name);
    }
    #endregion

    #region Roundtrip Tests
    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Roundtrip_EmptyFunction_PreservesName()
    {
        var func = new CssFunction("empty".AsSpan());
        var encoded = func.Encode();

        var parser = new CssParser(encoded);
        var values = parser.Parse_ComponentValue_List();
        var reparsed = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;

        Assert.NotNull(reparsed);
        Assert.Equal(func.Name, reparsed!.Name);
    }

    [Fact]
    [Trait("Category", "ComplexTokens")]
    [Trait("Category", "CssFunction")]
    public void Roundtrip_FunctionWithArgs_PreservesStructure()
    {
        var parser = new CssParser("rgb(255, 0, 128)");
        var values = parser.Parse_ComponentValue_List();
        var original = values.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;

        Assert.NotNull(original);
        var encoded = original!.Encode();

        var parser2 = new CssParser(encoded);
        var values2 = parser2.Parse_ComponentValue_List();
        var reparsed = values2.FirstOrDefault(v => v.Type == ECssTokenType.Function) as CssFunction;

        Assert.NotNull(reparsed);
        Assert.Equal(original.Name, reparsed!.Name);
    }
    #endregion
}
