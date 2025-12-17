using System;
using System.Collections.Generic;
using CssUI.CSS.Parser;
using CssUI.CSS.Internal;

namespace CssUI.CSS.Functions
{
    /// <summary>
    /// Evaluates CSS calc() expressions.
    /// Spec: https://www.w3.org/TR/css-values-4/#calc-notation
    /// </summary>
    public static class CssCalcEvaluator
    {
        /// <summary>
        /// Evaluates a calc() function and returns a numeric result.
        /// </summary>
        /// <param name="function">The CssFunction representing calc()</param>
        /// <param name="unitResolver">Resolver for converting CSS units to pixels</param>
        /// <param name="percentageBase">The base value for percentage calculations (e.g., container width)</param>
        /// <returns>The evaluated numeric result, or null if evaluation fails</returns>
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
                        result *= EvaluateFactor(tokens, unitResolver, percentageBase);
                    }
                    else if (delim == '/')
                    {
                        tokens.Dequeue();
                        SkipWhitespace(tokens);
                        double divisor = EvaluateFactor(tokens, unitResolver, percentageBase);
                        if (divisor == 0)
                        {
                            throw new DivideByZeroException("Division by zero in calc()");
                        }
                        result /= divisor;
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
        /// Evaluates a factor (number, dimension, percentage, or parenthesized expression).
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
                            return Convert.ToDouble(numToken.Number);
                        }
                        return 0;
                    }

                case ECssTokenType.Dimension:
                    {
                        if (token is DimensionToken dimToken)
                        {
                            double numericValue = Convert.ToDouble(dimToken.Number);
                            
                            // Try to resolve the unit
                            if (unitResolver != null && Lookup.TryEnum(dimToken.Unit, out ECssUnit unit))
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
                        // Handle nested calc() or other math functions
                        if (token is CssFunction nestedFunc)
                        {
                            if (string.Equals(nestedFunc.Name, "calc", StringComparison.OrdinalIgnoreCase))
                            {
                                return Evaluate(nestedFunc, unitResolver, percentageBase) ?? 0;
                            }
                            else if (string.Equals(nestedFunc.Name, "min", StringComparison.OrdinalIgnoreCase))
                            {
                                return EvaluateMinMax(nestedFunc, unitResolver, percentageBase, isMin: true);
                            }
                            else if (string.Equals(nestedFunc.Name, "max", StringComparison.OrdinalIgnoreCase))
                            {
                                return EvaluateMinMax(nestedFunc, unitResolver, percentageBase, isMin: false);
                            }
                            else if (string.Equals(nestedFunc.Name, "clamp", StringComparison.OrdinalIgnoreCase))
                            {
                                return EvaluateClamp(nestedFunc, unitResolver, percentageBase);
                            }
                        }
                        throw new InvalidOperationException($"Unsupported function in calc()");
                    }

                default:
                    throw new InvalidOperationException($"Unexpected token type in calc(): {token.Type}");
            }
        }

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
                return 0;
            }

            double result = values[0];
            for (int i = 1; i < values.Count; i++)
            {
                result = isMin ? Math.Min(result, values[i]) : Math.Max(result, values[i]);
            }
            return result;
        }

        /// <summary>
        /// Evaluates clamp(min, val, max) function.
        /// </summary>
        private static double EvaluateClamp(CssFunction function, CssValue.StyleUnitResolverDelegate unitResolver, double percentageBase)
        {
            var values = new List<double>();
            var tokens = new Queue<CssToken>(function.Arguments);

            while (tokens.Count > 0 && values.Count < 3)
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

            if (values.Count < 3)
            {
                throw new InvalidOperationException("clamp() requires exactly 3 arguments");
            }

            double min = values[0];
            double val = values[1];
            double max = values[2];

            return Math.Max(min, Math.Min(val, max));
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
    }
}
