#pragma warning disable CS0618 // Tests intentionally use obsolete CssValue.From_CSS method
using System;
using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.Common;

/// <summary>
/// Unit tests for <see cref="CssVarFunctionParser"/>.
/// Tests parsing of var() expressions per CSS Custom Properties Level 1.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-variables-1/#using-variables
/// </remarks>
[Trait("Category", "Parser")]
[Trait("Category", "Var")]
public class CssVarFunctionParserTests
{
    #region Basic Parsing

    [Fact]
    public void Parse_SimpleVar_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--main-color)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--main-color", varFunc.PropertyName);
        Assert.False(varFunc.HasFallback);
        Assert.True(varFunc.IsValid);
    }

    [Fact]
    public void Parse_VarWithSingleDashPropertyName_ReturnsFunction()
    {
        // Arrange - single dash is not a valid custom property name
        var css = "var(-color)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert - Should return as regular function, not VAR
        Assert.Equal(ECssValueTypes.FUNCTION, result.Type);
    }

    [Fact]
    public void Parse_VarWithNoArguments_ReturnsFunction()
    {
        // Arrange - var() with no args is invalid
        var css = "var()";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert - Should return as regular function since parsing fails
        Assert.Equal(ECssValueTypes.FUNCTION, result.Type);
    }

    #endregion

    #region Property Name Validation

    [Fact]
    public void Parse_VarWithDoubleDashOnly_ReturnsFunction()
    {
        // Arrange - "--" alone is reserved and invalid
        var css = "var(--)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert - Should return as regular function since parsing fails
        Assert.Equal(ECssValueTypes.FUNCTION, result.Type);
    }

    [Fact]
    public void Parse_VarWithLongPropertyName_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--my-very-long-custom-property-name-123)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--my-very-long-custom-property-name-123", varFunc.PropertyName);
    }

    [Fact]
    public void Parse_VarWithCamelCasePropertyName_ReturnsCssValue()
    {
        // Arrange - Property names are case-sensitive
        var css = "var(--backgroundColor)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--backgroundColor", varFunc.PropertyName);
    }

    [Fact]
    public void Parse_VarPropertyName_IsCaseSensitive()
    {
        // Arrange - Per spec, --foo and --FOO are different properties
        var css1 = "var(--color)";
        var css2 = "var(--COLOR)";
        var parser1 = new CssParser(css1);
        var parser2 = new CssParser(css2);

        // Act
        var result1 = parser1.Parse_CssValue();
        var result2 = parser2.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result1.Type);
        Assert.Equal(ECssValueTypes.VAR, result2.Type);
        var varFunc1 = result1.AsVarFunction();
        var varFunc2 = result2.AsVarFunction();
        Assert.NotEqual(varFunc1.PropertyName, varFunc2.PropertyName);
    }

    #endregion

    #region Fallback Values - Numbers

    [Fact]
    public void Parse_VarWithNumberFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--gap, 20)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--gap", varFunc.PropertyName);
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
        Assert.Equal(ECssValueTypes.NUMBER, varFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_VarWithDimensionFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--spacing, 10px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--spacing", varFunc.PropertyName);
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
        Assert.Equal(ECssValueTypes.DIMENSION, varFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_VarWithPercentageFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--width, 50%)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
        Assert.Equal(ECssValueTypes.PERCENT, varFunc.Fallback!.Type);
    }

    #endregion

    #region Fallback Values - Colors

    [Fact]
    public void Parse_VarWithHexColorFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--color, #ff0000)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
        Assert.Equal(ECssValueTypes.COLOR, varFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_VarWithKeywordFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--font, sans-serif)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
        Assert.Equal(ECssValueTypes.KEYWORD, varFunc.Fallback!.Type);
    }

    #endregion

    #region Fallback Values - Strings

    [Fact]
    public void Parse_VarWithStringFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "var(--content, \"hello\")";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
        Assert.Equal(ECssValueTypes.STRING, varFunc.Fallback!.Type);
    }

    #endregion

    #region Fallback Values - Empty Fallback

    [Fact]
    public void Parse_VarWithEmptyFallback_IsValid()
    {
        // Arrange - var(--a,) is valid per spec (empty fallback)
        var css = "var(--prop,)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--prop", varFunc.PropertyName);
        Assert.True(varFunc.HasFallback);
    }

    #endregion

    #region Complex Fallbacks

    [Fact]
    public void Parse_VarWithMultipleCommasFallback_PreservesRawTokens()
    {
        // Arrange - Fallback can contain commas: var(--font, Arial, sans-serif)
        var css = "var(--font, Arial, sans-serif)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.Equal("--font", varFunc.PropertyName);
        Assert.True(varFunc.HasFallback);
        // Complex fallbacks with multiple values preserve raw tokens
        Assert.NotNull(varFunc.FallbackTokens);
    }

    #endregion

    #region Function Name Case Insensitivity

    [Fact]
    public void Parse_VarUpperCase_ReturnsCssValue()
    {
        // Arrange - function name is case-insensitive
        var css = "VAR(--color)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
    }

    [Fact]
    public void Parse_VarMixedCase_ReturnsCssValue()
    {
        // Arrange
        var css = "Var(--color)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
    }

    #endregion

    #region CssVarFunction Struct Tests

    [Fact]
    public void CssVarFunction_Empty_HasNoPropertyName()
    {
        // Arrange & Act
        var varFunc = CssVarFunction.Empty;

        // Assert
        Assert.False(varFunc.IsValid);
        Assert.False(varFunc.HasFallback);
    }

    [Fact]
    public void CssVarFunction_WithPropertyOnly_IsValid()
    {
        // Arrange & Act
        var varFunc = new CssVarFunction("--test");

        // Assert
        Assert.True(varFunc.IsValid);
        Assert.Equal("--test", varFunc.PropertyName);
        Assert.False(varFunc.HasFallback);
    }

    [Fact]
    public void CssVarFunction_WithFallback_HasFallback()
    {
        // Arrange & Act
        var varFunc = new CssVarFunction("--test", CssValue.From(10));

        // Assert
        Assert.True(varFunc.IsValid);
        Assert.True(varFunc.HasFallback);
        Assert.NotNull(varFunc.Fallback);
    }

    [Fact]
    public void CssVarFunction_ToString_ReturnsVarSyntax()
    {
        // Arrange
        var varFunc = new CssVarFunction("--color");

        // Act
        var result = varFunc.ToString();

        // Assert
        Assert.Equal("var(--color)", result);
    }

    [Fact]
    public void CssVarFunction_Equality_WorksCorrectly()
    {
        // Arrange
        var var1 = new CssVarFunction("--color");
        var var2 = new CssVarFunction("--color");
        var var3 = new CssVarFunction("--background");

        // Assert
        Assert.Equal(var1, var2);
        Assert.NotEqual(var1, var3);
    }

    #endregion

    #region CssValue Factory and Accessor Tests

    [Fact]
    public void CssValue_FromVarFunction_CreatesCssValue()
    {
        // Arrange
        var varFunc = new CssVarFunction("--test");

        // Act
        var value = CssValue.From(varFunc);

        // Assert
        Assert.Equal(ECssValueTypes.VAR, value.Type);
    }

    [Fact]
    public void CssValue_AsVarFunction_ReturnsCorrectValue()
    {
        // Arrange
        var original = new CssVarFunction("--test", CssValue.From(100));
        var value = CssValue.From(original);

        // Act
        var retrieved = value.AsVarFunction();

        // Assert
        Assert.Equal("--test", retrieved.PropertyName);
        Assert.True(retrieved.HasFallback);
    }

    [Fact]
    public void CssValue_AsVarFunction_ThrowsForNonVarType()
    {
        // Arrange
        var value = CssValue.From(42);

        // Act & Assert
        Assert.Throws<CssException>(() => value.AsVarFunction());
    }

    #endregion

    #region Integration with From_CSS

    [Fact]
    public void FromCSS_ParsesVarFunction()
    {
        // Arrange & Act
        var value = CssValue.From_CSS("var(--primary)");

        // Assert
        Assert.Equal(ECssValueTypes.VAR, value.Type);
        var varFunc = value.AsVarFunction();
        Assert.Equal("--primary", varFunc.PropertyName);
    }

    [Fact]
    public void FromCSS_ParsesVarWithFallback()
    {
        // Arrange & Act
        var value = CssValue.From_CSS("var(--gap, 16px)");

        // Assert
        Assert.Equal(ECssValueTypes.VAR, value.Type);
        var varFunc = value.AsVarFunction();
        Assert.Equal("--gap", varFunc.PropertyName);
        Assert.True(varFunc.HasFallback);
    }

    #endregion

    #region Fallback Validation - <declaration-value> Production

    [Fact]
    public void Parse_VarWithValidFallback_IsFallbackValidIsTrue()
    {
        // Arrange - simple valid fallback
        var css = "var(--color, red)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.IsFallbackValid);
        Assert.True(varFunc.FallbackValidation?.IsMatch ?? true);
    }

    [Fact]
    public void Parse_VarWithNoFallback_IsFallbackValidIsTrue()
    {
        // Arrange - no fallback means validation passes by default
        var css = "var(--color)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.IsFallbackValid);
        Assert.Null(varFunc.FallbackValidation);
    }

    [Fact]
    public void Parse_VarWithComplexValidFallback_IsFallbackValidIsTrue()
    {
        // Arrange - complex fallback with multiple tokens
        var css = "var(--font, Arial, sans-serif)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.IsFallbackValid);
    }

    [Fact]
    public void Parse_VarWithNestedFallback_IsFallbackValidIsTrue()
    {
        // Arrange - nested parentheses are valid if balanced
        var css = "var(--func, calc(100% - 20px))";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.VAR, result.Type);
        var varFunc = result.AsVarFunction();
        Assert.True(varFunc.IsFallbackValid);
    }

    [Fact]
    public void CssVarFunction_IsFallbackValid_DefaultsToTrue()
    {
        // Arrange - constructor without validation result
        var varFunc = new CssVarFunction("--test", CssValue.From(10));

        // Assert - IsFallbackValid should be true when FallbackValidation is null
        Assert.True(varFunc.IsFallbackValid);
        Assert.Null(varFunc.FallbackValidation);
    }

    [Fact]
    public void CssVarFunction_WithValidFallbackTokens_IsFallbackValidIsTrue()
    {
        // Arrange
        var tokens = new CssToken[]
        {
            new IdentToken("red")
        };
        var validationResult = CssProductionMatcher.MatchDeclarationValue(tokens);
        var varFunc = new CssVarFunction("--color", new ReadOnlyMemory<CssToken>(tokens), validationResult);

        // Assert
        Assert.True(varFunc.IsFallbackValid);
        Assert.True(varFunc.FallbackValidation?.IsMatch);
    }

    [Fact]
    public void CssVarFunction_FallbackValidation_MatchesProductionMatcherResult()
    {
        // Arrange - create valid tokens and verify validation result matches
        var tokens = new CssToken[]
        {
            new NumberToken(ENumericTokenType.Integer, "42", 42L)
        };
        var validationResult = CssProductionMatcher.MatchDeclarationValue(tokens);
        var varFunc = new CssVarFunction("--value", new ReadOnlyMemory<CssToken>(tokens), validationResult);

        // Assert
        Assert.Equal(validationResult.IsMatch, varFunc.FallbackValidation?.IsMatch);
        Assert.Equal(validationResult.FailureType, varFunc.FallbackValidation?.FailureType);
    }

    #endregion
}
