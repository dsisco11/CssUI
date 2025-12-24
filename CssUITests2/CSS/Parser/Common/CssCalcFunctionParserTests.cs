using CssUI.CSS;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using Xunit;

namespace CssUITests.CSS.Parser.Common;

/// <summary>
/// Unit tests for <see cref="CssCalcFunctionParser"/>.
/// Tests parsing of calc() expressions per CSS Values Level 4 §10.
/// </summary>
[Trait("Category", "Parser")]
[Trait("Category", "Calc")]
public class CssCalcFunctionParserTests
{
    #region Simple Values

    [Fact]
    public void Parse_SimpleNumber_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.NotNull(expr);
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_SimpleDimension_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.NotNull(expr);
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_SimplePercentage_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(50%)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Addition

    [Fact]
    public void Parse_Addition_TwoNumbers_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(10 + 20)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Addition_TwoDimensions_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100px + 50px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Addition_DimensionAndPercentage_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100px + 50%)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Subtraction

    [Fact]
    public void Parse_Subtraction_TwoNumbers_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100 - 30)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Subtraction_TwoDimensions_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100px - 30px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Multiplication

    [Fact]
    public void Parse_Multiplication_NumberTimesNumber_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(10 * 5)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Multiplication_DimensionTimesNumber_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100px * 2)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Multiplication_NumberTimesDimension_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(2 * 100px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Division

    [Fact]
    public void Parse_Division_NumberByNumber_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100 / 4)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Division_DimensionByNumber_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100px / 2)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Parentheses

    [Fact]
    public void Parse_Parentheses_Simple_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc((100px))";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Parentheses_GroupedAddition_ReturnsCalcExpression()
    {
        // Arrange - (100px + 50px) * 2
        var css = "calc((100px + 50px) * 2)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Parentheses_Nested_ReturnsCalcExpression()
    {
        // Arrange - ((10 + 5) * 2)
        var css = "calc(((10 + 5) * 2))";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Complex Expressions

    [Fact]
    public void Parse_Complex_MultipleOperations_ReturnsCalcExpression()
    {
        // Arrange - 100% - 50px
        var css = "calc(100% - 50px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Complex_MixedOperators_ReturnsCalcExpression()
    {
        // Arrange - 100px + 50px * 2
        var css = "calc(100px + 50px * 2)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Complex_SubtractAndDivide_ReturnsCalcExpression()
    {
        // Arrange - 100% - 20px / 2
        var css = "calc(100% - 20px / 2)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region CSS Constants

    [Fact]
    public void Parse_Constant_Pi_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(pi)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Constant_E_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(e)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Constant_Infinity_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(infinity)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_Constant_NegativeInfinity_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(-infinity)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Unary Operations

    [Fact]
    public void Parse_UnaryMinus_Number_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(-50)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_UnaryMinus_Dimension_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(-50px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Multiple Units

    [Fact]
    public void Parse_MultipleUnits_Em_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(2em + 10px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_MultipleUnits_Rem_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(1rem * 2)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_MultipleUnits_Vw_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(100vw - 20px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    [Fact]
    public void Parse_MultipleUnits_Vh_ReturnsCalcExpression()
    {
        // Arrange
        var css = "calc(50vh + 100px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Case Insensitivity

    [Fact]
    public void Parse_CaseInsensitive_CALC_ReturnsCalcExpression()
    {
        // Arrange
        var css = "CALC(100px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
    }

    [Fact]
    public void Parse_CaseInsensitive_Calc_ReturnsCalcExpression()
    {
        // Arrange
        var css = "Calc(100px + 50px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
    }

    #endregion
}
