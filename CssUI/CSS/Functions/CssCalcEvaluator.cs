using System;
using System.Collections.Generic;
using CssUI.CSS.Parser;

namespace CssUI.CSS.Functions;

/// <summary>
/// Evaluates CSS calc() expressions per CSS Values Level 4.
/// Spec: https://www.w3.org/TR/css-values-4/#calc-notation
/// Implements IEEE-754 semantics for infinity/NaN handling.
/// </summary>
public static class CssCalcEvaluator
{
    #region Numeric Constants (CSS Values Level 4 §10.7)
    /// <summary>Mathematical constant e (Euler's number)</summary>
    public const double E = Math.E;  // ≈2.7182818284590452354

    /// <summary>Mathematical constant π (pi)</summary>
    public const double Pi = Math.PI;  // ≈3.1415926535897932
    #endregion

    /// <summary>
    /// Evaluates a calc() function and returns a numeric result.
    /// Per CSS Values Level 4, returns NaN/Infinity for degenerate cases instead of throwing.
    /// </summary>
    /// <param name="function">The CssFunction representing calc()</param>
    /// <param name="unitResolver">Resolver for converting CSS units to pixels</param>
    /// <param name="percentageBase">The base value for percentage calculations (e.g., container width)</param>
    /// <returns>The evaluated numeric result, or null if the function is invalid</returns>
    internal static double? Evaluate(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase = 0)
    {
        if (function == null || !string.Equals(function.Name, "calc", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        try
        {
            var tokens = new Queue<CssToken>(function.Arguments);
            return EvaluateExpression(tokens, unitResolver, percentageBase);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Evaluates any math function (calc, min, max, clamp, sin, cos, etc.)
    /// </summary>
    internal static double? EvaluateMathFunction(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase = 0)
    {
        if (function == null) return null;

        string name = function.Name.ToLowerInvariant();
        var tokens = new Queue<CssToken>(function.Arguments);

        try
        {
            return name switch
            {
                "calc" => EvaluateExpression(tokens, unitResolver, percentageBase),
                "min" => EvaluateMinMax(function, unitResolver, percentageBase, isMin: true),
                "max" => EvaluateMinMax(function, unitResolver, percentageBase, isMin: false),
                "clamp" => EvaluateClamp(function, unitResolver, percentageBase),
                // Trigonometric functions (§10.4)
                "sin" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, Math.Sin, convertToRadians: true),
                "cos" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, Math.Cos, convertToRadians: true),
                "tan" => EvaluateTan(function, unitResolver, percentageBase),
                "asin" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, x => RadToDeg(Math.Asin(x))),
                "acos" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, x => RadToDeg(Math.Acos(x))),
                "atan" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, x => RadToDeg(Math.Atan(x))),
                "atan2" => EvaluateAtan2(function, unitResolver, percentageBase),
                // Exponential functions (§10.5)
                "pow" => EvaluatePow(function, unitResolver, percentageBase),
                "sqrt" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, Math.Sqrt),
                "hypot" => EvaluateHypot(function, unitResolver, percentageBase),
                "log" => EvaluateLog(function, unitResolver, percentageBase),
                "exp" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, Math.Exp),
                // Sign-related functions (§10.6)
                "abs" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, Math.Abs),
                "sign" => EvaluateSingleArgFunc(function, unitResolver, percentageBase, Sign),
                // Stepped value functions (§10.3)
                "round" => EvaluateRound(function, unitResolver, percentageBase),
                "mod" => EvaluateMod(function, unitResolver, percentageBase),
                "rem" => EvaluateRem(function, unitResolver, percentageBase),
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Evaluates an expression (handles addition and subtraction).
    /// </summary>
    private static double EvaluateExpression(Queue<CssToken> tokens, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        double result = EvaluateTerm(tokens, unitResolver, percentageBase);

        while (tokens.Count > 0)
        {
            var token = tokens.Peek();

            if (token.Type == ECssTokenType.Whitespace)
            {
                tokens.Dequeue();
                continue;
            }

            if (token.Type == ECssTokenType.Delim)
            {
                char delim = GetDelimChar(token);

                if (delim == '+')
                {
                    tokens.Dequeue();
                    SkipWhitespace(tokens);
                    result += EvaluateTerm(tokens, unitResolver, percentageBase);
                }
                else if (delim == '-')
                {
                    tokens.Dequeue();
                    SkipWhitespace(tokens);
                    result -= EvaluateTerm(tokens, unitResolver, percentageBase);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluates a term (handles multiplication and division).
    /// Per CSS Values Level 4 §10.9.1: Division by zero returns ±∞ (IEEE-754 semantics).
    /// </summary>
    private static double EvaluateTerm(Queue<CssToken> tokens, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        double result = EvaluateFactor(tokens, unitResolver, percentageBase);

        while (tokens.Count > 0)
        {
            var token = tokens.Peek();

            if (token.Type == ECssTokenType.Whitespace)
            {
                tokens.Dequeue();
                continue;
            }

            if (token.Type == ECssTokenType.Delim)
            {
                char delim = GetDelimChar(token);

                if (delim == '*')
                {
                    tokens.Dequeue();
                    SkipWhitespace(tokens);
                    double factor = EvaluateFactor(tokens, unitResolver, percentageBase);
                    result = Multiply(result, factor);
                }
                else if (delim == '/')
                {
                    tokens.Dequeue();
                    SkipWhitespace(tokens);
                    double divisor = EvaluateFactor(tokens, unitResolver, percentageBase);
                    result = Divide(result, divisor);
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Evaluates a factor (number, dimension, percentage, parenthesized expression, or keyword).
    /// </summary>
    private static double EvaluateFactor(Queue<CssToken> tokens, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        SkipWhitespace(tokens);

        if (tokens.Count == 0)
        {
            throw new InvalidOperationException("Unexpected end of calc() expression");
        }

        var token = tokens.Dequeue();

        switch (token.Type)
        {
            case ECssTokenType.Number:
                {
                    if (token is NumberToken numToken)
                    {
                        return numToken.AsNumber;
                    }
                    return 0;
                }

            case ECssTokenType.Dimension:
                {
                    if (token is DimensionToken dimToken)
                    {
                        double numericValue = dimToken.AsNumber;

                        // Try to resolve the unit
                        if (unitResolver != null && Lookup.TryEnum(dimToken.Unit!, out ECssUnit unit))
                        {
                            double multiplier = unitResolver(unit);
                            return numericValue * multiplier;
                        }
                        return numericValue;
                    }
                    return 0;
                }

            case ECssTokenType.Percentage:
                {
                    if (token is PercentageToken pctToken)
                    {
                        return (pctToken.Number / 100.0) * percentageBase;
                    }
                    return 0;
                }

            case ECssTokenType.Ident:
                {
                    // Handle numeric constants (§10.7)
                    if (token is IdentToken identToken)
                    {
                        return ResolveNumericConstant(identToken.Value!);
                    }
                    return 0;
                }

            case ECssTokenType.Parenth_Open:
                {
                    double result = EvaluateExpression(tokens, unitResolver, percentageBase);
                    // Expect closing paren
                    SkipWhitespace(tokens);
                    if (tokens.Count > 0 && tokens.Peek().Type == ECssTokenType.Parenth_Close)
                    {
                        tokens.Dequeue();
                    }
                    return result;
                }

            case ECssTokenType.Function:
                {
                    // Handle nested math functions
                    if (token is CssFunction nestedFunc)
                    {
                        return EvaluateMathFunction(nestedFunc, unitResolver, percentageBase) ?? 0;
                    }
                    throw new InvalidOperationException($"Unsupported function in calc()");
                }

            default:
                throw new InvalidOperationException($"Unexpected token type in calc(): {token.Type}");
        }
    }

    #region Numeric Constants (§10.7)
    /// <summary>
    /// Resolves CSS numeric constant keywords to their values.
    /// </summary>
    private static double ResolveNumericConstant(string identifier)
    {
        return identifier.ToLowerInvariant() switch
        {
            "e" => E,
            "pi" => Pi,
            "infinity" => double.PositiveInfinity,
            "-infinity" => double.NegativeInfinity,
            "nan" => double.NaN,
            _ => throw new InvalidOperationException($"Unknown constant: {identifier}")
        };
    }
    #endregion

    #region IEEE-754 Arithmetic (§10.9.1)
    /// <summary>
    /// Multiplies two values with IEEE-754 semantics.
    /// 0 * ∞ = NaN, ∞ * ∞ = ∞, etc.
    /// </summary>
    private static double Multiply(double a, double b)
    {
        // NaN is infectious
        if (double.IsNaN(a) || double.IsNaN(b)) return double.NaN;

        // 0 * ∞ = NaN
        if ((a == 0 && double.IsInfinity(b)) || (b == 0 && double.IsInfinity(a)))
            return double.NaN;

        return a * b;
    }

    /// <summary>
    /// Divides two values with IEEE-754 semantics.
    /// Per CSS Values Level 4 §10.9.1: x/0 = ±∞, 0/0 = NaN, ∞/∞ = NaN
    /// </summary>
    private static double Divide(double a, double b)
    {
        // NaN is infectious
        if (double.IsNaN(a) || double.IsNaN(b)) return double.NaN;

        // 0/0 = NaN, ∞/∞ = NaN
        if ((a == 0 && b == 0) || (double.IsInfinity(a) && double.IsInfinity(b)))
            return double.NaN;

        // x/0 = ±∞ (sign determined by operands)
        if (b == 0)
        {
            return a >= 0 ? double.PositiveInfinity : double.NegativeInfinity;
        }

        return a / b;
    }

    /// <summary>
    /// CSS sign() function - returns -1, 0, or 1.
    /// Per CSS Values Level 4 §10.6.
    /// </summary>
    private static double Sign(double value)
    {
        if (double.IsNaN(value)) return double.NaN;
        if (value > 0) return 1;
        if (value < 0) return -1;
        return 0;  // Preserves signed zero semantically
    }
    #endregion

    #region Comparison Functions (§10.2)
    /// <summary>
    /// Evaluates min() or max() function.
    /// </summary>
    private static double EvaluateMinMax(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase, bool isMin)
    {
        var values = new List<double>();
        var tokens = new Queue<CssToken>(function.Arguments);

        while (tokens.Count > 0)
        {
            SkipWhitespace(tokens);
            if (tokens.Count == 0) break;

            var token = tokens.Peek();
            if (token.Type == ECssTokenType.Comma)
            {
                tokens.Dequeue();
                continue;
            }

            double value = EvaluateExpression(tokens, unitResolver, percentageBase);
            values.Add(value);
        }

        if (values.Count == 0)
        {
            return double.NaN;
        }

        double result = values[0];
        for (int i = 1; i < values.Count; i++)
        {
            // NaN is infectious
            if (double.IsNaN(values[i])) return double.NaN;
            result = isMin ? Math.Min(result, values[i]) : Math.Max(result, values[i]);
        }
        return result;
    }

    /// <summary>
    /// Evaluates clamp(min, val, max) function.
    /// Equivalent to max(MIN, min(VAL, MAX)).
    /// Supports 'none' keyword for min/max (per CSS Values Level 4).
    /// </summary>
    private static double EvaluateClamp(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var tokens = new Queue<CssToken>(function.Arguments);
        var arguments = new List<double?>();  // null = 'none'

        int argIndex = 0;
        while (tokens.Count > 0 && argIndex < 3)
        {
            SkipWhitespace(tokens);
            if (tokens.Count == 0) break;

            var token = tokens.Peek();
            if (token.Type == ECssTokenType.Comma)
            {
                tokens.Dequeue();
                continue;
            }

            // Check for 'none' keyword
            if (token is IdentToken identToken &&
                string.Equals(identToken.Value, "none", StringComparison.OrdinalIgnoreCase))
            {
                tokens.Dequeue();
                arguments.Add(null);  // 'none' means unbounded
                argIndex++;
            }
            else
            {
                double value = EvaluateExpression(tokens, unitResolver, percentageBase);
                arguments.Add(value);
                argIndex++;
            }
        }

        if (arguments.Count < 3)
        {
            return double.NaN;  // Invalid clamp
        }

        double? minVal = arguments[0];
        double val = arguments[1] ?? double.NaN;
        double? maxVal = arguments[2];

        // NaN is infectious
        if (double.IsNaN(val)) return double.NaN;

        // clamp(none, VAL, MAX) = min(VAL, MAX)
        // clamp(MIN, VAL, none) = max(MIN, VAL)
        // clamp(none, VAL, none) = VAL
        if (minVal == null && maxVal == null)
            return val;
        if (minVal == null)
            return Math.Min(val, maxVal!.Value);
        if (maxVal == null)
            return Math.Max(minVal.Value, val);

        // Full clamp: max(MIN, min(VAL, MAX))
        // Per spec: MIN wins over MAX if they conflict
        return Math.Max(minVal.Value, Math.Min(val, maxVal.Value));
    }
    #endregion

    #region Trigonometric Functions (§10.4)
    private static double EvaluateSingleArgFunc(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver,
        double percentageBase, Func<double, double> mathFunc, bool convertToRadians = false)
    {
        var tokens = new Queue<CssToken>(function.Arguments);
        SkipWhitespace(tokens);

        double arg = EvaluateExpression(tokens, unitResolver, percentageBase);

        if (convertToRadians)
        {
            arg = DegToRad(arg);
        }

        // NaN is infectious
        if (double.IsNaN(arg)) return double.NaN;

        return mathFunc(arg);
    }

    /// <summary>
    /// tan() with special handling for asymptotes.
    /// </summary>
    private static double EvaluateTan(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var tokens = new Queue<CssToken>(function.Arguments);
        SkipWhitespace(tokens);

        double arg = EvaluateExpression(tokens, unitResolver, percentageBase);

        // Infinite argument -> NaN
        if (double.IsInfinity(arg)) return double.NaN;
        if (double.IsNaN(arg)) return double.NaN;

        double radians = DegToRad(arg);
        return Math.Tan(radians);
    }

    private static double EvaluateAtan2(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var args = GetTwoArgs(function, unitResolver, percentageBase);
        if (args == null) return double.NaN;

        double y = args.Value.Item1;
        double x = args.Value.Item2;

        if (double.IsNaN(y) || double.IsNaN(x)) return double.NaN;

        return RadToDeg(Math.Atan2(y, x));
    }

    private static double DegToRad(double degrees) => degrees * Math.PI / 180.0;
    private static double RadToDeg(double radians) => radians * 180.0 / Math.PI;
    #endregion

    #region Exponential Functions (§10.5)
    private static double EvaluatePow(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var args = GetTwoArgs(function, unitResolver, percentageBase);
        if (args == null) return double.NaN;

        double baseVal = args.Value.Item1;
        double exponent = args.Value.Item2;

        if (double.IsNaN(baseVal) || double.IsNaN(exponent)) return double.NaN;

        return Math.Pow(baseVal, exponent);
    }

    private static double EvaluateHypot(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var values = new List<double>();
        var tokens = new Queue<CssToken>(function.Arguments);

        while (tokens.Count > 0)
        {
            SkipWhitespace(tokens);
            if (tokens.Count == 0) break;

            var token = tokens.Peek();
            if (token.Type == ECssTokenType.Comma)
            {
                tokens.Dequeue();
                continue;
            }

            double value = EvaluateExpression(tokens, unitResolver, percentageBase);

            // If any argument is infinite, result is +∞
            if (double.IsInfinity(value)) return double.PositiveInfinity;
            if (double.IsNaN(value)) return double.NaN;

            values.Add(value);
        }

        if (values.Count == 0) return 0;

        double sumOfSquares = 0;
        foreach (var v in values)
        {
            sumOfSquares += v * v;
        }
        return Math.Sqrt(sumOfSquares);
    }

    private static double EvaluateLog(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var tokens = new Queue<CssToken>(function.Arguments);
        var args = new List<double>();

        while (tokens.Count > 0 && args.Count < 2)
        {
            SkipWhitespace(tokens);
            if (tokens.Count == 0) break;

            var token = tokens.Peek();
            if (token.Type == ECssTokenType.Comma)
            {
                tokens.Dequeue();
                continue;
            }

            args.Add(EvaluateExpression(tokens, unitResolver, percentageBase));
        }

        if (args.Count == 0) return double.NaN;

        double value = args[0];
        if (double.IsNaN(value)) return double.NaN;
        if (value < 0) return double.NaN;
        if (value == 0) return double.NegativeInfinity;
        if (double.IsPositiveInfinity(value)) return double.PositiveInfinity;

        if (args.Count == 1)
        {
            return Math.Log(value);  // Natural log
        }

        double baseVal = args[1];
        if (double.IsNaN(baseVal) || baseVal <= 0 || baseVal == 1) return double.NaN;

        return Math.Log(value, baseVal);
    }
    #endregion

    #region Stepped Value Functions (§10.3)
    private static double EvaluateRound(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var tokens = new Queue<CssToken>(function.Arguments);
        string strategy = "nearest";  // Default
        var args = new List<double>();

        // Check for rounding strategy keyword
        SkipWhitespace(tokens);
        if (tokens.Count > 0 && tokens.Peek() is IdentToken strategyToken)
        {
            string possibleStrategy = strategyToken.Value.ToLowerInvariant();
            if (possibleStrategy is "nearest" or "up" or "down" or "to-zero")
            {
                strategy = possibleStrategy;
                tokens.Dequeue();
                // Skip comma after strategy
                SkipWhitespace(tokens);
                if (tokens.Count > 0 && tokens.Peek().Type == ECssTokenType.Comma)
                    tokens.Dequeue();
            }
        }

        // Get arguments
        while (tokens.Count > 0 && args.Count < 2)
        {
            SkipWhitespace(tokens);
            if (tokens.Count == 0) break;

            var token = tokens.Peek();
            if (token.Type == ECssTokenType.Comma)
            {
                tokens.Dequeue();
                continue;
            }

            args.Add(EvaluateExpression(tokens, unitResolver, percentageBase));
        }

        if (args.Count == 0) return double.NaN;

        double value = args[0];
        double step = args.Count > 1 ? args[1] : 1;  // Default step is 1

        if (double.IsNaN(value) || double.IsNaN(step)) return double.NaN;
        if (step == 0) return double.NaN;
        if (double.IsInfinity(value) && double.IsInfinity(step)) return double.NaN;
        if (double.IsInfinity(value)) return value;  // Infinite value, finite step

        // Finite value, infinite step
        if (double.IsInfinity(step))
        {
            return strategy switch
            {
                "nearest" or "to-zero" => value >= 0 ? 0.0 : -0.0,
                "up" => value > 0 ? double.PositiveInfinity : (value == 0 ? 0.0 : -0.0),
                "down" => value < 0 ? double.NegativeInfinity : (value == 0 ? -0.0 : 0.0),
                _ => 0.0
            };
        }

        double quotient = value / step;

        return strategy switch
        {
            "nearest" => Math.Round(quotient) * step,
            "up" => Math.Ceiling(quotient) * step,
            "down" => Math.Floor(quotient) * step,
            "to-zero" => Math.Truncate(quotient) * step,
            _ => Math.Round(quotient) * step
        };
    }

    private static double EvaluateMod(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var args = GetTwoArgs(function, unitResolver, percentageBase);
        if (args == null) return double.NaN;

        double a = args.Value.Item1;
        double b = args.Value.Item2;

        if (double.IsNaN(a) || double.IsNaN(b)) return double.NaN;
        if (b == 0) return double.NaN;
        if (double.IsInfinity(a)) return double.NaN;

        // mod(A, B) - result has same sign as B
        double result = a % b;

        // Adjust sign to match B (modulus vs remainder)
        if (result != 0 && (result < 0) != (b < 0))
        {
            result += b;
        }

        return result;
    }

    private static double EvaluateRem(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var args = GetTwoArgs(function, unitResolver, percentageBase);
        if (args == null) return double.NaN;

        double a = args.Value.Item1;
        double b = args.Value.Item2;

        if (double.IsNaN(a) || double.IsNaN(b)) return double.NaN;
        if (b == 0) return double.NaN;
        if (double.IsInfinity(a)) return double.NaN;

        // rem(A, B) - result has same sign as A (C# % behavior)
        return a % b;
    }
    #endregion

    #region Helper Methods
    private static (double, double)? GetTwoArgs(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
    {
        var tokens = new Queue<CssToken>(function.Arguments);
        var args = new List<double>();

        while (tokens.Count > 0 && args.Count < 2)
        {
            SkipWhitespace(tokens);
            if (tokens.Count == 0) break;

            var token = tokens.Peek();
            if (token.Type == ECssTokenType.Comma)
            {
                tokens.Dequeue();
                continue;
            }

            args.Add(EvaluateExpression(tokens, unitResolver, percentageBase));
        }

        if (args.Count < 2) return null;
        return (args[0], args[1]);
    }

    private static void SkipWhitespace(Queue<CssToken> tokens)
    {
        while (tokens.Count > 0 && tokens.Peek().Type == ECssTokenType.Whitespace)
        {
            tokens.Dequeue();
        }
    }

    private static char GetDelimChar(CssToken token)
    {
        if (token is DelimToken delimToken)
        {
            return delimToken.Value;
        }
        return '\0';
    }
    #endregion
}

