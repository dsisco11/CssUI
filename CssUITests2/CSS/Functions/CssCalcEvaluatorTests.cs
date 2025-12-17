using Microsoft.VisualStudio.TestTools.UnitTesting;
using CssUI.CSS;
using CssUI.CSS.Functions;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;

namespace CssUITests.CSS.Functions
{
    /// <summary>
    /// Unit tests for CSS calc() function and math functions per CSS Values Level 4.
    /// Tests IEEE-754 semantics, numeric constants, and all math functions.
    /// </summary>
    [TestClass]
    public class CssCalcEvaluatorTests
    {
        #region Infrastructure Tests

        [TestMethod]
        [TestCategory("Calc")]
        public void ECssValueTypes_HasFunctionType()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ECssValueTypes), "FUNCTION"));
        }

        [TestMethod]
        [TestCategory("Calc")]
        public void ECssValueTypes_FunctionValue()
        {
            Assert.AreEqual(1 << 23, (int)ECssValueTypes.FUNCTION);
        }

        #endregion

        #region Numeric Constants Tests (CSS Values Level 4 §10.7)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Constants")]
        public void Constants_E_HasCorrectValue()
        {
            Assert.AreEqual(Math.E, CssCalcEvaluator.E, 0.0000001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Constants")]
        public void Constants_Pi_HasCorrectValue()
        {
            Assert.AreEqual(Math.PI, CssCalcEvaluator.Pi, 0.0000001);
        }

        #endregion

        #region IEEE-754 Semantics Tests (CSS Values Level 4 §10.9.1)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("IEEE754")]
        public void Division_ByZero_ReturnsPositiveInfinity()
        {
            // Test: 1 / 0 = +∞
            var func = CreateCalcFunction(1, "/", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsPositiveInfinity(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("IEEE754")]
        public void Division_NegativeByZero_ReturnsNegativeInfinity()
        {
            // Test: -1 / 0 = -∞
            var func = CreateCalcFunction(-1, "/", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNegativeInfinity(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("IEEE754")]
        public void Division_ZeroByZero_ReturnsNaN()
        {
            // Test: 0 / 0 = NaN
            var func = CreateCalcFunction(0, "/", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("IEEE754")]
        public void Multiplication_ZeroTimesInfinity_ReturnsNaN()
        {
            // Test: 0 * ∞ = NaN
            var func = CreateCalcWithIdentOp(0, "*", "infinity");
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("IEEE754")]
        public void NaN_IsInfectious()
        {
            // Test: NaN + 5 = NaN
            var func = CreateCalcWithIdentOp("nan", "+", 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        #endregion

        #region Comparison Functions Tests (CSS Values Level 4 §10.2)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Comparison")]
        public void Min_WithMultipleValues_ReturnsSmallest()
        {
            var func = CreateMinMaxFunction("min", 10, 5, 20);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Comparison")]
        public void Max_WithMultipleValues_ReturnsLargest()
        {
            var func = CreateMinMaxFunction("max", 10, 5, 20);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Comparison")]
        public void Clamp_ValueInRange_ReturnsValue()
        {
            // clamp(10, 15, 20) = 15
            var func = CreateClampFunction(10, 15, 20);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Comparison")]
        public void Clamp_ValueBelowMin_ReturnsMin()
        {
            // clamp(10, 5, 20) = 10
            var func = CreateClampFunction(10, 5, 20);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Comparison")]
        public void Clamp_ValueAboveMax_ReturnsMax()
        {
            // clamp(10, 25, 20) = 20
            var func = CreateClampFunction(10, 25, 20);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Comparison")]
        public void Clamp_MinGreaterThanMax_MinWins()
        {
            // clamp(100, 50, 20) = 100 (MIN wins per spec)
            var func = CreateClampFunction(100, 50, 20);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.Value);
        }

        #endregion

        #region Trigonometric Functions Tests (CSS Values Level 4 §10.4)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Sin_Of0_Returns0()
        {
            var func = CreateSingleArgFunction("sin", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Sin_Of90Degrees_Returns1()
        {
            var func = CreateSingleArgFunction("sin", 90);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Cos_Of0_Returns1()
        {
            var func = CreateSingleArgFunction("cos", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Cos_Of90Degrees_Returns0()
        {
            var func = CreateSingleArgFunction("cos", 90);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Tan_OfInfinity_ReturnsNaN()
        {
            var func = CreateSingleArgFunctionWithIdent("tan", "infinity");
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Asin_Of0_Returns0()
        {
            var func = CreateSingleArgFunction("asin", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Acos_Of1_Returns0()
        {
            var func = CreateSingleArgFunction("acos", 1);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Trig")]
        public void Asin_OutOfRange_ReturnsNaN()
        {
            // asin(2) is undefined, should return NaN
            var func = CreateSingleArgFunction("asin", 2);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        #endregion

        #region Exponential Functions Tests (CSS Values Level 4 §10.5)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Pow_2To3_Returns8()
        {
            var func = CreateTwoArgFunction("pow", 2, 3);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Sqrt_Of16_Returns4()
        {
            var func = CreateSingleArgFunction("sqrt", 16);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Sqrt_OfNegative_ReturnsNaN()
        {
            var func = CreateSingleArgFunction("sqrt", -4);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Hypot_3And4_Returns5()
        {
            // hypot(3, 4) = sqrt(3² + 4²) = 5
            var func = CreateTwoArgFunction("hypot", 3, 4);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Log_OfE_Returns1()
        {
            // ln(e) = 1
            var func = CreateSingleArgFunctionWithIdent("log", "e");
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value, 0.0001);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Log_Of0_ReturnsNegativeInfinity()
        {
            var func = CreateSingleArgFunction("log", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNegativeInfinity(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Exponential")]
        public void Exp_Of0_Returns1()
        {
            var func = CreateSingleArgFunction("exp", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value, 0.0001);
        }

        #endregion

        #region Sign Functions Tests (CSS Values Level 4 §10.6)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Sign")]
        public void Abs_OfNegative_ReturnsPositive()
        {
            var func = CreateSingleArgFunction("abs", -42);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Sign")]
        public void Abs_OfPositive_ReturnsPositive()
        {
            var func = CreateSingleArgFunction("abs", 42);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(42, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Sign")]
        public void Sign_OfPositive_Returns1()
        {
            var func = CreateSingleArgFunction("sign", 42);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Sign")]
        public void Sign_OfNegative_ReturnsNegative1()
        {
            var func = CreateSingleArgFunction("sign", -42);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(-1, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Sign")]
        public void Sign_Of0_Returns0()
        {
            var func = CreateSingleArgFunction("sign", 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value);
        }

        #endregion

        #region Stepped Value Functions Tests (CSS Values Level 4 §10.3)

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Round_Nearest_18By5_Returns20()
        {
            // round(nearest, 18, 5) = 20 (18 is closer to 20 than 15)
            var func = CreateRoundFunction("nearest", 18, 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Round_Up_18By5_Returns20()
        {
            // round(up, 18, 5) = 20
            var func = CreateRoundFunction("up", 18, 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Round_Down_18By5_Returns15()
        {
            // round(down, 18, 5) = 15
            var func = CreateRoundFunction("down", 18, 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Mod_18By5_Returns3()
        {
            // mod(18, 5) = 3
            var func = CreateTwoArgFunction("mod", 18, 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Mod_ByZero_ReturnsNaN()
        {
            var func = CreateTwoArgFunction("mod", 18, 0);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.IsTrue(double.IsNaN(result.Value));
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Rem_18By5_Returns3()
        {
            // rem(18, 5) = 3
            var func = CreateTwoArgFunction("rem", 18, 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Value);
        }

        [TestMethod]
        [TestCategory("Calc")]
        [TestCategory("Stepped")]
        public void Rem_Negative18By5_ReturnsNegative3()
        {
            // rem(-18, 5) = -3 (sign matches dividend)
            var func = CreateTwoArgFunction("rem", -18, 5);
            var result = CssCalcEvaluator.EvaluateMathFunction(func, null!, 0);
            Assert.IsNotNull(result);
            Assert.AreEqual(-3, result.Value);
        }

        #endregion

        #region Helper Methods

        private static CssFunction CreateCalcFunction(double a, string op, double b)
        {
            var func = new CssFunction("calc".AsSpan());
            func.Arguments.Add(CreateNumberToken(a));
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(new DelimToken(op[0]));
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(b));
            return func;
        }

        private static CssFunction CreateCalcWithIdentOp(double num, string op, string ident)
        {
            var func = new CssFunction("calc".AsSpan());
            func.Arguments.Add(CreateNumberToken(num));
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(new DelimToken(op[0]));
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(new IdentToken(ident));
            return func;
        }

        private static CssFunction CreateCalcWithIdentOp(string ident, string op, double num)
        {
            var func = new CssFunction("calc".AsSpan());
            func.Arguments.Add(new IdentToken(ident));
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(new DelimToken(op[0]));
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(num));
            return func;
        }

        private static CssFunction CreateMinMaxFunction(string name, params double[] values)
        {
            var func = new CssFunction(name.AsSpan());
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    func.Arguments.Add(new CommaToken());
                    func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
                }
                func.Arguments.Add(CreateNumberToken(values[i]));
            }
            return func;
        }

        private static CssFunction CreateClampFunction(double min, double val, double max)
        {
            var func = new CssFunction("clamp".AsSpan());
            func.Arguments.Add(CreateNumberToken(min));
            func.Arguments.Add(new CommaToken());
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(val));
            func.Arguments.Add(new CommaToken());
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(max));
            return func;
        }

        private static CssFunction CreateSingleArgFunction(string name, double value)
        {
            var func = new CssFunction(name.AsSpan());
            func.Arguments.Add(CreateNumberToken(value));
            return func;
        }

        private static CssFunction CreateSingleArgFunctionWithIdent(string name, string ident)
        {
            var func = new CssFunction(name.AsSpan());
            func.Arguments.Add(new IdentToken(ident));
            return func;
        }

        private static CssFunction CreateTwoArgFunction(string name, double a, double b)
        {
            var func = new CssFunction(name.AsSpan());
            func.Arguments.Add(CreateNumberToken(a));
            func.Arguments.Add(new CommaToken());
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(b));
            return func;
        }

        private static CssFunction CreateRoundFunction(string strategy, double value, double step)
        {
            var func = new CssFunction("round".AsSpan());
            func.Arguments.Add(new IdentToken(strategy));
            func.Arguments.Add(new CommaToken());
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(value));
            func.Arguments.Add(new CommaToken());
            func.Arguments.Add(new WhitespaceToken(" ".AsSpan()));
            func.Arguments.Add(CreateNumberToken(step));
            return func;
        }

        private static NumberToken CreateNumberToken(double value)
        {
            string strVal = value.ToString();
            bool isInteger = value == Math.Floor(value) && !double.IsInfinity(value);
            return new NumberToken(
                isInteger ? ENumericTokenType.Integer : ENumericTokenType.Number,
                strVal.AsSpan(),
                isInteger ? (object)(int)value : (object)value
            );
        }

        #endregion
    }
}

