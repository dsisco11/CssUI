using System;
using CssUI.CSS;
using Xunit;

namespace CssUITests.CSS.Tests;

/// <summary>
/// Tests for <see cref="CssCalcValue"/> - the specialized CSS value type for calc() expressions.
/// </summary>
public class CssCalcValueTests
{
    #region Factory Method Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void From_CssCalcExpression_ReturnsCssCalcValue()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        Assert.IsType<CssCalcValue>(value);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void From_CssCalcExpression_SetsTypeToCALC()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        Assert.Equal(ECssValueTypes.CALC, value.Type);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void From_CssCalcExpression_HasValueReturnsTrue()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        Assert.True(value.HasValue);
    }

    #endregion

    #region Value Access Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Expression_ReturnsStoredExpression()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(50, ECssUnit.None, isPercentage: true));
        var calcValue = (CssCalcValue)CssValue.From(expression);

        Assert.Same(expression, calcValue.Expression);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void AsCalcExpression_ReturnsStoredExpression()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        var retrieved = value.AsCalcExpression();

        Assert.Same(expression, retrieved);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void ToString_SimpleValue_ReturnsCalcFunction()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        var result = value.ToString();

        Assert.StartsWith("calc", result);
        Assert.Contains("100", result);
        Assert.Contains("px", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Serialize_SimpleValue_ReturnsCalcFunction()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        var result = value.Serialize();

        Assert.StartsWith("calc", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Serialize_Sum_ReturnsCalcWithAddition()
    {
        var sum = new CssCalcSumNode(
            new CssCalcValueNode(100, ECssUnit.PX),
            new CssCalcValueNode(50, ECssUnit.None, isPercentage: true)
        );
        var expression = new CssCalcExpression(sum);
        var value = CssValue.From(expression);

        var result = value.Serialize();

        Assert.StartsWith("calc", result);
        Assert.Contains("100", result);
        Assert.Contains("50", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Serialize_Percentage_IncludesPercentSign()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(50, ECssUnit.None, isPercentage: true));
        var value = CssValue.From(expression);

        var result = value.Serialize();

        Assert.Contains("%", result);
    }

    #endregion

    #region TryFormat Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    [Trait("Category", "Formatting")]
    public void TryFormat_SimpleCalc_WritesCalcFunction()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);
        Span<char> buffer = stackalloc char[64];

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.True(success);
        var result = buffer[..charsWritten].ToString();
        Assert.StartsWith("calc", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    [Trait("Category", "Formatting")]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);
        Span<char> buffer = stackalloc char[3]; // Too small

        bool success = value.TryFormat(buffer, out int charsWritten);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Equals_SameExpression_ReturnsTrue()
    {
        var expression1 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var expression2 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value1 = CssValue.From(expression1);
        var value2 = CssValue.From(expression2);

        Assert.True(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Equals_DifferentExpression_ReturnsFalse()
    {
        var expression1 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var expression2 = new CssCalcExpression(new CssCalcValueNode(200, ECssUnit.PX));
        var value1 = CssValue.From(expression1);
        var value2 = CssValue.From(expression2);

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Equals_DifferentUnit_ReturnsFalse()
    {
        var expression1 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var expression2 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.EM));
        var value1 = CssValue.From(expression1);
        var value2 = CssValue.From(expression2);

        Assert.False(value1.Equals(value2));
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void Equals_Null_ReturnsFalse()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value = CssValue.From(expression);

        Assert.False(value.Equals(null));
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void GetHashCode_SameExpression_ReturnsSameHash()
    {
        var expression1 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var expression2 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var value1 = CssValue.From(expression1);
        var value2 = CssValue.From(expression2);

        Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void GetHashCode_DifferentExpression_ReturnsDifferentHash()
    {
        var expression1 = new CssCalcExpression(new CssCalcValueNode(100, ECssUnit.PX));
        var expression2 = new CssCalcExpression(new CssCalcValueNode(200, ECssUnit.PX));
        var value1 = CssValue.From(expression1);
        var value2 = CssValue.From(expression2);

        Assert.NotEqual(value1.GetHashCode(), value2.GetHashCode());
    }

    #endregion

    #region Complex Expression Tests

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void ComplexExpression_SumWithMultipleTerms()
    {
        // calc(100px + 50% + 2em)
        var sum = new CssCalcSumNode(
            new CssCalcValueNode(100, ECssUnit.PX),
            new CssCalcValueNode(50, ECssUnit.None, isPercentage: true),
            new CssCalcValueNode(2, ECssUnit.EM)
        );
        var expression = new CssCalcExpression(sum);
        var value = CssValue.From(expression);

        Assert.IsType<CssCalcValue>(value);
        Assert.Equal(ECssValueTypes.CALC, value.Type);

        var result = value.Serialize();
        Assert.Contains("100", result);
        Assert.Contains("px", result);
        Assert.Contains("50", result);
        Assert.Contains("%", result);
        Assert.Contains("2", result);
        Assert.Contains("em", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void NestedExpression_Product()
    {
        // calc(2 * 50px)
        var product = new CssCalcProductNode(
            new CssCalcValueNode(2, ECssUnit.None),
            new CssCalcValueNode(50, ECssUnit.PX)
        );
        var expression = new CssCalcExpression(product);
        var value = CssValue.From(expression);

        var result = value.Serialize();
        Assert.Contains("2", result);
        Assert.Contains("50", result);
    }

    #endregion

    #region Edge Cases

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void SimpleNumber_NoUnit()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(42, ECssUnit.None));
        var value = CssValue.From(expression);

        var result = value.Serialize();
        Assert.Contains("42", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void NegativeValue_SerializesCorrectly()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(-100, ECssUnit.PX));
        var value = CssValue.From(expression);

        var result = value.Serialize();
        Assert.Contains("-100", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void DecimalValue_SerializesCorrectly()
    {
        var expression = new CssCalcExpression(new CssCalcValueNode(12.5, ECssUnit.EM));
        var value = CssValue.From(expression);

        var result = value.Serialize();
        Assert.Contains("12.5", result);
    }

    [Fact]
    [Trait("Category", "CssValue")]
    [Trait("Category", "Calc")]
    public void WidthCalc_TypicalUsage()
    {
        // Common use case: width: calc(100% - 20px)
        var sum = new CssCalcSumNode(
            new CssCalcValueNode(100, ECssUnit.None, isPercentage: true),
            new CssCalcNegateNode(new CssCalcValueNode(20, ECssUnit.PX))
        );
        var expression = new CssCalcExpression(sum);
        var value = CssValue.From(expression);

        Assert.Equal(ECssValueTypes.CALC, value.Type);
        var result = value.Serialize();
        Assert.Contains("100", result);
        Assert.Contains("%", result);
        Assert.Contains("20", result);
        Assert.Contains("px", result);
    }

    #endregion
}
