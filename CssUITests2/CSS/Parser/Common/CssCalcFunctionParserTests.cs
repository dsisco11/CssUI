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

    #region Min Function

    [Fact]
    public void Parse_Min_SingleValue_ReturnsCalcExpression()
    {
        // Arrange
        var css = "min(100px)";
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
    public void Parse_Min_TwoValues_ReturnsCalcExpression()
    {
        // Arrange
        var css = "min(100px, 50px)";
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
    public void Parse_Min_ThreeValues_ReturnsCalcExpression()
    {
        // Arrange
        var css = "min(200px, 100px, 50px)";
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
    public void Parse_Min_WithCalcExpression_ReturnsCalcExpression()
    {
        // Arrange - min() can contain full calc-sum expressions
        var css = "min(100px + 50px, 200px)";
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
    public void Parse_Min_CaseInsensitive_ReturnsCalcExpression()
    {
        // Arrange
        var css = "MIN(100px, 50px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
    }

    #endregion

    #region Max Function

    [Fact]
    public void Parse_Max_SingleValue_ReturnsCalcExpression()
    {
        // Arrange
        var css = "max(100px)";
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
    public void Parse_Max_TwoValues_ReturnsCalcExpression()
    {
        // Arrange
        var css = "max(100px, 50px)";
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
    public void Parse_Max_ThreeValues_ReturnsCalcExpression()
    {
        // Arrange
        var css = "max(50px, 100px, 200px)";
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
    public void Parse_Max_WithCalcExpression_ReturnsCalcExpression()
    {
        // Arrange - max() can contain full calc-sum expressions
        var css = "max(100px - 50px, 200px)";
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
    public void Parse_Max_CaseInsensitive_ReturnsCalcExpression()
    {
        // Arrange
        var css = "MAX(100px, 50px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
    }

    #endregion

    #region Clamp Function

    [Fact]
    public void Parse_Clamp_AllValues_ReturnsCalcExpression()
    {
        // Arrange
        var css = "clamp(10px, 50px, 100px)";
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
    public void Parse_Clamp_NoneMin_ReturnsCalcExpression()
    {
        // Arrange - clamp(none, VAL, MAX) is equivalent to min(VAL, MAX)
        var css = "clamp(none, 50px, 100px)";
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
    public void Parse_Clamp_NoneMax_ReturnsCalcExpression()
    {
        // Arrange - clamp(MIN, VAL, none) is equivalent to max(MIN, VAL)
        var css = "clamp(10px, 50px, none)";
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
    public void Parse_Clamp_BothNone_ReturnsCalcExpression()
    {
        // Arrange - clamp(none, VAL, none) is equivalent to calc(VAL)
        var css = "clamp(none, 50px, none)";
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
    public void Parse_Clamp_WithCalcExpressions_ReturnsCalcExpression()
    {
        // Arrange - clamp() can contain full calc-sum expressions
        var css = "clamp(10px + 5px, 50px * 2, 100px - 10px)";
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
    public void Parse_Clamp_CaseInsensitive_ReturnsCalcExpression()
    {
        // Arrange
        var css = "CLAMP(10px, 50px, 100px)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
    }

    [Fact]
    public void Parse_Clamp_CaseInsensitiveNone_ReturnsCalcExpression()
    {
        // Arrange - 'none' keyword should be case-insensitive
        var css = "clamp(NONE, 50px, None)";
        var parser = new CssParser(css);

        // Act
        var result = parser.Parse_CssValue();

        // Assert
        Assert.Equal(ECssValueTypes.CALC, result.Type);
        var expr = result.AsCalcExpression();
        Assert.NotNull(expr);
        Assert.True(expr.IsValid);
    }

    #endregion

    #region Comparison Function Evaluation

    [Fact]
    public void Evaluate_Min_ReturnsSmallestValue()
    {
        // Arrange
        var css = "min(100, 50, 75)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var expr = result.AsCalcExpression();

        // Act
        var evaluated = expr.Evaluate(null);

        // Assert
        Assert.NotNull(evaluated);
        Assert.Equal(50.0, evaluated.Value);
    }

    [Fact]
    public void Evaluate_Max_ReturnsLargestValue()
    {
        // Arrange
        var css = "max(100, 50, 75)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var expr = result.AsCalcExpression();

        // Act
        var evaluated = expr.Evaluate(null);

        // Assert
        Assert.NotNull(evaluated);
        Assert.Equal(100.0, evaluated.Value);
    }

    [Fact]
    public void Evaluate_Clamp_ValueBelowMin_ReturnsMin()
    {
        // Arrange - value 5 is below min 10
        var css = "clamp(10, 5, 100)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var expr = result.AsCalcExpression();

        // Act
        var evaluated = expr.Evaluate(null);

        // Assert
        Assert.NotNull(evaluated);
        Assert.Equal(10.0, evaluated.Value);
    }

    [Fact]
    public void Evaluate_Clamp_ValueAboveMax_ReturnsMax()
    {
        // Arrange - value 150 is above max 100
        var css = "clamp(10, 150, 100)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var expr = result.AsCalcExpression();

        // Act
        var evaluated = expr.Evaluate(null);

        // Assert
        Assert.NotNull(evaluated);
        Assert.Equal(100.0, evaluated.Value);
    }

    [Fact]
    public void Evaluate_Clamp_ValueInRange_ReturnsValue()
    {
        // Arrange - value 50 is within range [10, 100]
        var css = "clamp(10, 50, 100)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var expr = result.AsCalcExpression();

        // Act
        var evaluated = expr.Evaluate(null);

        // Assert
        Assert.NotNull(evaluated);
        Assert.Equal(50.0, evaluated.Value);
    }

    [Fact]
    public void Evaluate_Clamp_MinExceedsMax_MinWins()
    {
        // Arrange - Per spec, min wins when it conflicts with max
        // clamp(100, 50, 50) should return 100 because min > max
        var css = "clamp(100, 50, 50)";
        var parser = new CssParser(css);
        var result = parser.Parse_CssValue();
        var expr = result.AsCalcExpression();

        // Act
        var evaluated = expr.Evaluate(null);

        // Assert
        Assert.NotNull(evaluated);
        Assert.Equal(100.0, evaluated.Value); // min wins
    }

    #endregion
}
