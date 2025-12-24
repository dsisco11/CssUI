using CssUI;
using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.Common;

/// <summary>
/// Unit tests for <see cref="CssEnvFunctionParser"/>.
/// Tests parsing of env() expressions per CSS Environment Variables Level 1.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-env-1/#env-function
/// 
/// Grammar: env() = env( &lt;custom-ident&gt; &lt;integer [0,∞]&gt;* [, &lt;declaration-value&gt;]? )
/// </remarks>
[Trait("Category", "Parser")]
[Trait("Category", "Env")]
public class CssEnvFunctionParserTests
{
    #region Basic Parsing

    [Fact]
    public void Parse_SimpleEnv_ReturnsCssValue()
    {
        // Arrange
        var css = "env(safe-area-inset-top)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("safe-area-inset-top", envFunc.VariableName);
        Assert.False(envFunc.HasFallback);
        Assert.True(envFunc.IsValid);
        Assert.False(envFunc.IsCustomVariable);
        Assert.False(envFunc.IsIndexed);
    }

    [Fact]
    public void Parse_EnvWithNoArguments_ReturnsFunction()
    {
        // Arrange - env() with no args is invalid
        var css = "env()";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert - Should return as regular function since parsing fails
        Assert.Equal(ECssValueTypes.FUNCTION, result.Type);
    }

    [Fact]
    public void Parse_EnvWithAllSafeAreaInsets_ReturnsCssValues()
    {
        // Arrange
        var names = new[]
        {
            "safe-area-inset-top",
            "safe-area-inset-right",
            "safe-area-inset-bottom",
            "safe-area-inset-left"
        };

        foreach (var name in names)
        {
            var css = $"env({name})";
            var parser = new CssParser(css);

            // Act
            var result = parser.Parse_CssValue();

            // Assert
            Assert.Equal(ECssValueTypes.ENV, result.Type);
            var envFunc = result.AsEnvFunction();
            Assert.Equal(name, envFunc.VariableName);
        }
    }

    [Fact]
    public void Parse_EnvWithSafeAreaMaxInsets_ReturnsCssValues()
    {
        // Arrange
        var names = new[]
        {
            "safe-area-max-inset-top",
            "safe-area-max-inset-right",
            "safe-area-max-inset-bottom",
            "safe-area-max-inset-left"
        };

        foreach (var name in names)
        {
            var css = $"env({name})";
            var parser = new CssParser(css);

            // Act
            var result = parser.Parse_CssValue();

            // Assert
            Assert.Equal(ECssValueTypes.ENV, result.Type);
            var envFunc = result.AsEnvFunction();
            Assert.Equal(name, envFunc.VariableName);
        }
    }

    #endregion

    #region Case Sensitivity

    [Fact]
    public void Parse_EnvVariableName_IsCaseInsensitiveForUAVariables()
    {
        // Arrange - Per spec, UA-defined variables are case-insensitive
        var css1 = "env(safe-area-inset-top)";
        var css2 = "env(SAFE-AREA-INSET-TOP)";
        var css3 = "env(Safe-Area-Inset-Top)";

        var parser1 = new CssParser(css1);
        var parser2 = new CssParser(css2);
        var parser3 = new CssParser(css3);

        // Act
        var result1 = parser1.Parse_CssValue();
        var result2 = parser2.Parse_CssValue();
        var result3 = parser3.Parse_CssValue();

        // Assert - All should parse successfully
        Assert.Equal(ECssValueTypes.ENV, result1.Type);
        Assert.Equal(ECssValueTypes.ENV, result2.Type);
        Assert.Equal(ECssValueTypes.ENV, result3.Type);

        // Variable names should be equal (case-insensitive comparison)
        var env1 = result1.AsEnvFunction();
        var env2 = result2.AsEnvFunction();
        var env3 = result3.AsEnvFunction();

        Assert.True(env1.Equals(env2));
        Assert.True(env2.Equals(env3));
    }

    [Fact]
    public void Parse_EnvCustomVariable_IsCaseSensitive()
    {
        // Arrange - Author-defined (--*) variables are case-sensitive
        var css1 = "env(--myVar)";
        var css2 = "env(--MYVAR)";

        var parser1 = new CssParser(css1);
        var parser2 = new CssParser(css2);

        // Act
        var result1 = parser1.Parse_CssValue();
        var result2 = parser2.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result1.Type);
        Assert.Equal(ECssValueTypes.ENV, result2.Type);

        var env1 = result1.AsEnvFunction();
        var env2 = result2.AsEnvFunction();

        Assert.True(env1.IsCustomVariable);
        Assert.True(env2.IsCustomVariable);
        Assert.False(env1.Equals(env2)); // Case-sensitive, so not equal
    }

    #endregion

    #region Custom Variables

    [Fact]
    public void Parse_EnvWithCustomVariable_ReturnsCssValue()
    {
        // Arrange - Author-defined variables start with --
        var css = "env(--custom-padding)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("--custom-padding", envFunc.VariableName);
        Assert.True(envFunc.IsCustomVariable);
    }

    #endregion

    #region Indexed Variables

    [Fact]
    public void Parse_EnvWithSingleIndex_ReturnsCssValue()
    {
        // Arrange - Indexed environment variable with one dimension
        var css = "env(viewport-segment-width 0)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("viewport-segment-width", envFunc.VariableName);
        Assert.True(envFunc.IsIndexed);
        Assert.Single(envFunc.Indices.Span.ToArray());
        Assert.Equal(0, envFunc.Indices.Span[0]);
    }

    [Fact]
    public void Parse_EnvWithTwoIndices_ReturnsCssValue()
    {
        // Arrange - Per spec: env(viewport-segment-width 0 0)
        var css = "env(viewport-segment-width 0 0)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("viewport-segment-width", envFunc.VariableName);
        Assert.True(envFunc.IsIndexed);
        Assert.Equal(2, envFunc.Indices.Length);
        Assert.Equal(0, envFunc.Indices.Span[0]);
        Assert.Equal(0, envFunc.Indices.Span[1]);
    }

    [Fact]
    public void Parse_EnvWithNonZeroIndices_ReturnsCssValue()
    {
        // Arrange - e.g., env(viewport-segment-width 1 0) for right segment
        var css = "env(viewport-segment-width 1 0)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal(2, envFunc.Indices.Length);
        Assert.Equal(1, envFunc.Indices.Span[0]);
        Assert.Equal(0, envFunc.Indices.Span[1]);
    }

    [Fact]
    public void Parse_EnvWithViewportSegmentVariables_ReturnsCssValues()
    {
        // Arrange - Per spec, viewport-segment-* have 2 dimensions
        var names = new[]
        {
            "viewport-segment-width",
            "viewport-segment-height",
            "viewport-segment-top",
            "viewport-segment-left",
            "viewport-segment-bottom",
            "viewport-segment-right"
        };

        foreach (var name in names)
        {
            var css = $"env({name} 0 0)";
            var parser = new CssParser(css);

            // Act
            var result = parser.Parse_CssValue();

            // Assert
            Assert.Equal(ECssValueTypes.ENV, result.Type);
            var envFunc = result.AsEnvFunction();
            Assert.Equal(name, envFunc.VariableName);
            Assert.Equal(2, envFunc.Indices.Length);
        }
    }

    [Fact]
    public void Parse_EnvWithNegativeIndex_ReturnsFunction()
    {
        // Arrange - Negative indices are invalid per spec
        var css = "env(viewport-segment-width -1 0)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert - Should return as regular function since parsing fails
        Assert.Equal(ECssValueTypes.FUNCTION, result.Type);
    }

    [Fact]
    public void Parse_EnvWithDecimalIndex_ReturnsFunction()
    {
        // Arrange - Indices must be integers
        var css = "env(viewport-segment-width 0.5 0)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert - Should return as regular function since parsing fails
        Assert.Equal(ECssValueTypes.FUNCTION, result.Type);
    }

    #endregion

    #region Fallback Values - Simple

    [Fact]
    public void Parse_EnvWithNumberFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "env(safe-area-inset-top, 0)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("safe-area-inset-top", envFunc.VariableName);
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.NUMBER, envFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_EnvWithDimensionFallback_ReturnsCssValue()
    {
        // Arrange - env(safe-area-inset-top, 0px)
        var css = "env(safe-area-inset-top, 0px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.DIMENSION, envFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_EnvWithPercentageFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "env(preferred-text-scale, 100%)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.PERCENT, envFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_EnvWithKeywordFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "env(foo, auto)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.KEYWORD, envFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_EnvWithColorFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "env(foo, #ff0000)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.COLOR, envFunc.Fallback!.Type);
    }

    #endregion

    #region Fallback Values - Indexed

    [Fact]
    public void Parse_EnvWithIndicesAndFallback_ReturnsCssValue()
    {
        // Arrange - Per spec: env(viewport-segment-width 0 0, 300px)
        var css = "env(viewport-segment-width 0 0, 300px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("viewport-segment-width", envFunc.VariableName);
        Assert.True(envFunc.IsIndexed);
        Assert.Equal(2, envFunc.Indices.Length);
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.DIMENSION, envFunc.Fallback!.Type);
    }

    #endregion

    #region Fallback Values - Complex

    [Fact]
    public void Parse_EnvWithEmptyFallback_ReturnsCssValue()
    {
        // Arrange - Per spec, env(foo,) is valid with empty fallback
        var css = "env(foo,)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        // Empty fallback results in Null value
        Assert.Equal(ECssValueTypes.NULL, envFunc.Fallback!.Type);
    }

    [Fact]
    public void Parse_EnvWithCommaInFallback_HasFallbackTokens()
    {
        // Arrange - Per spec: env(foo, red, blue) has fallback "red, blue"
        var css = "env(foo, red, blue)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        // Complex fallback preserves raw tokens
        Assert.NotNull(envFunc.FallbackTokens);
    }

    [Fact]
    public void Parse_EnvWithStringFallback_ReturnsCssValue()
    {
        // Arrange
        var css = "env(foo, \"default value\")";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.True(envFunc.HasFallback);
        Assert.NotNull(envFunc.Fallback);
        Assert.Equal(ECssValueTypes.STRING, envFunc.Fallback!.Type);
    }

    #endregion

    #region Equality

    [Fact]
    public void EnvFunction_Equality_SameValues_AreEqual()
    {
        // Arrange
        var css1 = "env(safe-area-inset-top, 0px)";
        var css2 = "env(safe-area-inset-top, 0px)";
        var parser1 = new CssParser(css1);
        var parser2 = new CssParser(css2);

        // Act
        var result1 = parser1.Parse_CssValue();
        var result2 = parser2.Parse_CssValue();

        // Assert
        var env1 = result1.AsEnvFunction();
        var env2 = result2.AsEnvFunction();
        Assert.Equal(env1, env2);
    }

    [Fact]
    public void EnvFunction_Equality_DifferentVariables_AreNotEqual()
    {
        // Arrange
        var css1 = "env(safe-area-inset-top)";
        var css2 = "env(safe-area-inset-bottom)";
        var parser1 = new CssParser(css1);
        var parser2 = new CssParser(css2);

        // Act
        var result1 = parser1.Parse_CssValue();
        var result2 = parser2.Parse_CssValue();

        // Assert
        var env1 = result1.AsEnvFunction();
        var env2 = result2.AsEnvFunction();
        Assert.NotEqual(env1, env2);
    }

    [Fact]
    public void EnvFunction_Equality_DifferentIndices_AreNotEqual()
    {
        // Arrange
        var css1 = "env(viewport-segment-width 0 0)";
        var css2 = "env(viewport-segment-width 1 0)";
        var parser1 = new CssParser(css1);
        var parser2 = new CssParser(css2);

        // Act
        var result1 = parser1.Parse_CssValue();
        var result2 = parser2.Parse_CssValue();

        // Assert
        var env1 = result1.AsEnvFunction();
        var env2 = result2.AsEnvFunction();
        Assert.NotEqual(env1, env2);
    }

    #endregion

    #region ToString

    [Fact]
    public void EnvFunction_ToString_SimpleVariable()
    {
        // Arrange
        var css = "env(safe-area-inset-top)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var envFunc = result.AsEnvFunction();

        // Act
        var str = envFunc.ToString();

        // Assert
        Assert.Equal("env(safe-area-inset-top)", str);
    }

    [Fact]
    public void EnvFunction_ToString_WithIndices()
    {
        // Arrange
        var css = "env(viewport-segment-width 0 1)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var envFunc = result.AsEnvFunction();

        // Act
        var str = envFunc.ToString();

        // Assert
        Assert.Equal("env(viewport-segment-width 0 1)", str);
    }

    #endregion

    #region Integration - Property Values

    [Fact]
    public void Parse_PaddingWithEnv_ReturnsCssValue()
    {
        // Arrange - Common use case for safe area insets
        var css = "env(safe-area-inset-top)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
    }

    [Fact]
    public void Parse_PreferredTextScale_ReturnsCssValue()
    {
        // Arrange - Per spec, preferred-text-scale is a <number>
        var css = "env(preferred-text-scale)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.ENV, result.Type);
        var envFunc = result.AsEnvFunction();
        Assert.Equal("preferred-text-scale", envFunc.VariableName);
        Assert.False(envFunc.IsIndexed);
    }

    #endregion
}
