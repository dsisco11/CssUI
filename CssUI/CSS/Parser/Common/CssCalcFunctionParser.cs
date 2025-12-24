using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parses CSS calc() function syntax per CSS Values Level 4 §10.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-values-4/#calc-func
/// 
/// Grammar:
/// <code>
/// &lt;calc()&gt;  = calc( &lt;calc-sum&gt; )
/// &lt;calc-sum&gt; = &lt;calc-product&gt; [ [ '+' | '-' ] &lt;calc-product&gt; ]*
/// &lt;calc-product&gt; = &lt;calc-value&gt; [ [ '*' | '/' ] &lt;calc-value&gt; ]*
/// &lt;calc-value&gt; = &lt;number&gt; | &lt;dimension&gt; | &lt;percentage&gt; |
///                &lt;calc-keyword&gt; | ( &lt;calc-sum&gt; )
/// </code>
/// 
/// Important: Whitespace is required on both sides of + and - operators.
/// The * and / operators can be used without whitespace.
/// </remarks>
internal static class CssCalcFunctionParser
{
    #region Public API

    /// <summary>
    /// Attempts to parse a calc() function and return a <see cref="CssCalcExpression"/>.
    /// </summary>
    /// <param name="function">The CSS function to parse.</param>
    /// <param name="expression">The resulting calc expression if successful.</param>
    /// <returns>True if the function was a valid calc() and parsing succeeded.</returns>
    public static bool TryParseCalcFunction(CssFunction function, out CssCalcExpression? expression)
    {
        ArgumentNullException.ThrowIfNull(function);

        expression = null;

        // Check function name (case-insensitive)
        if (!function.Name.Equals("calc", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = function.Arguments;
        if (arguments is null || arguments.Count == 0)
        {
            return false;
        }

        // Create token stream from arguments
        var stream = CssParsingHelpers.CreateTokenStream(arguments, preserveWhitespace: true);
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // Parse the calc-sum production
        var root = ParseCalcSum(stream);
        if (root is null)
        {
            return false;
        }

        // Skip trailing whitespace
        SkipWhitespace(stream);

        // Ensure we consumed all tokens
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return false; // Extra tokens after expression
        }

        expression = new CssCalcExpression(root);
        return expression.IsValid;
    }

    /// <summary>
    /// Attempts to parse a min(), max(), or clamp() function.
    /// </summary>
    /// <param name="function">The CSS function to parse.</param>
    /// <param name="expression">The resulting calc expression if successful.</param>
    /// <returns>True if parsing succeeded.</returns>
    public static bool TryParseComparisonFunction(CssFunction function, out CssCalcExpression? expression)
    {
        ArgumentNullException.ThrowIfNull(function);

        expression = null;
        var name = function.Name;

        // Check function name (case-insensitive)
        if (name.Equals("min", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseMinFunction(function, out expression);
        }
        if (name.Equals("max", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseMaxFunction(function, out expression);
        }
        if (name.Equals("clamp", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseClampFunction(function, out expression);
        }

        return false;
    }

    /// <summary>
    /// Attempts to parse a min() function.
    /// Grammar: min( &lt;calc-sum&gt;# )
    /// </summary>
    public static bool TryParseMinFunction(CssFunction function, out CssCalcExpression? expression)
    {
        ArgumentNullException.ThrowIfNull(function);

        expression = null;

        if (!function.Name.Equals("min", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = function.Arguments;
        if (arguments is null || arguments.Count == 0)
        {
            return false;
        }

        // Parse comma-separated arguments
        var children = ParseCommaDelimitedCalcSums(arguments);
        if (children is null || children.Count == 0)
        {
            return false;
        }

        var root = new CssCalcMinNode(children);
        expression = new CssCalcExpression(root);
        return expression.IsValid;
    }

    /// <summary>
    /// Attempts to parse a max() function.
    /// Grammar: max( &lt;calc-sum&gt;# )
    /// </summary>
    public static bool TryParseMaxFunction(CssFunction function, out CssCalcExpression? expression)
    {
        ArgumentNullException.ThrowIfNull(function);

        expression = null;

        if (!function.Name.Equals("max", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = function.Arguments;
        if (arguments is null || arguments.Count == 0)
        {
            return false;
        }

        // Parse comma-separated arguments
        var children = ParseCommaDelimitedCalcSums(arguments);
        if (children is null || children.Count == 0)
        {
            return false;
        }

        var root = new CssCalcMaxNode(children);
        expression = new CssCalcExpression(root);
        return expression.IsValid;
    }

    /// <summary>
    /// Attempts to parse a clamp() function.
    /// Grammar: clamp( [ &lt;calc-sum&gt; | none ], &lt;calc-sum&gt;, [ &lt;calc-sum&gt; | none ] )
    /// </summary>
    public static bool TryParseClampFunction(CssFunction function, out CssCalcExpression? expression)
    {
        ArgumentNullException.ThrowIfNull(function);

        expression = null;

        if (!function.Name.Equals("clamp", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = function.Arguments;
        if (arguments is null || arguments.Count == 0)
        {
            return false;
        }

        // Parse the three arguments
        var args = ParseCommaDelimitedCalcSumsWithNone(arguments);
        if (args is null || args.Count != 3)
        {
            return false; // clamp() requires exactly 3 arguments
        }

        // args[0] = min (can be null for 'none')
        // args[1] = value (must not be null)
        // args[2] = max (can be null for 'none')

        if (args[1] is null)
        {
            return false; // Central value cannot be 'none'
        }

        var root = new CssCalcClampNode(args[0], args[1], args[2]);
        expression = new CssCalcExpression(root);
        return expression.IsValid;
    }

    /// <summary>
    /// Parses a comma-delimited list of calc-sum expressions.
    /// Used for min() and max() functions.
    /// </summary>
    private static List<CssCalcNode>? ParseCommaDelimitedCalcSums(List<CssToken> arguments)
    {
        var results = new List<CssCalcNode>();
        var currentGroup = new List<CssToken>();

        foreach (var token in arguments)
        {
            // Check for comma delimiter
            if (token.Type == ECssTokenType.Comma)
            {
                if (currentGroup.Count == 0)
                {
                    return null; // Empty argument before comma
                }

                // Parse this argument group
                var node = ParseCalcSumFromComponents(currentGroup);
                if (node is null)
                {
                    return null;
                }
                results.Add(node);
                currentGroup.Clear();
            }
            else
            {
                currentGroup.Add(token);
            }
        }

        // Parse the last argument group
        if (currentGroup.Count > 0)
        {
            var node = ParseCalcSumFromComponents(currentGroup);
            if (node is null)
            {
                return null;
            }
            results.Add(node);
        }

        return results;
    }

    /// <summary>
    /// Parses a comma-delimited list of calc-sum expressions, allowing 'none' keyword.
    /// Used for clamp() function.
    /// </summary>
    private static List<CssCalcNode?>? ParseCommaDelimitedCalcSumsWithNone(List<CssToken> arguments)
    {
        var results = new List<CssCalcNode?>();
        var currentGroup = new List<CssToken>();

        foreach (var token in arguments)
        {
            // Check for comma delimiter
            if (token.Type == ECssTokenType.Comma)
            {
                // Parse this argument group (may be 'none')
                var node = ParseCalcSumOrNoneFromComponents(currentGroup);
                if (currentGroup.Count > 0 && node is null && !IsNoneKeyword(currentGroup))
                {
                    return null; // Invalid argument
                }
                results.Add(node);
                currentGroup.Clear();
            }
            else
            {
                currentGroup.Add(token);
            }
        }

        // Parse the last argument group
        if (currentGroup.Count > 0)
        {
            var node = ParseCalcSumOrNoneFromComponents(currentGroup);
            if (node is null && !IsNoneKeyword(currentGroup))
            {
                return null;
            }
            results.Add(node);
        }
        else if (results.Count < 3)
        {
            // Handle trailing comma case - add null for 'none'
            results.Add(null);
        }

        return results;
    }

    /// <summary>
    /// Checks if a component list contains only the 'none' keyword.
    /// </summary>
    private static bool IsNoneKeyword(List<CssToken> components)
    {
        // Skip whitespace and look for a single 'none' ident
        foreach (var token in components)
        {
            if (token.Type == ECssTokenType.Whitespace)
                continue;

            if (token.Type == ECssTokenType.Ident)
            {
                var identToken = (IdentToken)token;
                return identToken.Value.Equals("none", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        return false;
    }

    /// <summary>
    /// Parses a calc-sum expression from component values, or returns null for 'none'.
    /// </summary>
    private static CssCalcNode? ParseCalcSumOrNoneFromComponents(List<CssToken> components)
    {
        if (IsNoneKeyword(components))
        {
            return null; // 'none' is represented as null
        }

        return ParseCalcSumFromComponents(components);
    }

    /// <summary>
    /// Parses a calc-sum expression from component values.
    /// </summary>
    private static CssCalcNode? ParseCalcSumFromComponents(List<CssToken> components)
    {
        if (components.Count == 0)
        {
            return null;
        }

        var stream = CssParsingHelpers.CreateTokenStream(components, preserveWhitespace: true);
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            return null;
        }

        var node = ParseCalcSum(stream);
        if (node is null)
        {
            return null;
        }

        // Skip trailing whitespace
        SkipWhitespace(stream);

        // Ensure we consumed all tokens
        if (!CssParsingHelpers.IsAtEnd(stream))
        {
            return null;
        }

        return node;
    }

    #endregion

    #region Parsing Productions

    /// <summary>
    /// Parses the calc-sum production: calc-product [ [ '+' | '-' ] calc-product ]*
    /// </summary>
    private static CssCalcNode? ParseCalcSum(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);

        // Parse first calc-product
        var left = ParseCalcProduct(stream);
        if (left is null)
        {
            return null;
        }

        // Collect sum terms
        var terms = new List<CssCalcNode> { left };

        while (!CssParsingHelpers.IsAtEnd(stream))
        {
            // Check for + or - operator (must be surrounded by whitespace per spec)
            SkipWhitespace(stream);

            var token = stream.Next;
            if (token.Type != ECssTokenType.Delim)
            {
                break;
            }

            var delimToken = (DelimToken)token;
            bool isAdd = delimToken.Value == '+';
            bool isSub = delimToken.Value == '-';

            if (!isAdd && !isSub)
            {
                break;
            }

            // Consume the operator
            stream.Consume();
            SkipWhitespace(stream);

            // Parse next calc-product
            var right = ParseCalcProduct(stream);
            if (right is null)
            {
                return null; // Invalid expression
            }

            // Subtraction becomes addition of negated value
            if (isSub)
            {
                terms.Add(new CssCalcNegateNode(right));
            }
            else
            {
                terms.Add(right);
            }
        }

        // If only one term, return it directly
        if (terms.Count == 1)
        {
            return terms[0];
        }

        return new CssCalcSumNode(terms);
    }

    /// <summary>
    /// Parses the calc-product production: calc-value [ [ '*' | '/' ] calc-value ]*
    /// </summary>
    private static CssCalcNode? ParseCalcProduct(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);

        // Parse first calc-value
        var left = ParseCalcValue(stream);
        if (left is null)
        {
            return null;
        }

        // Collect product terms
        var terms = new List<CssCalcNode> { left };

        while (!CssParsingHelpers.IsAtEnd(stream))
        {
            // Skip whitespace (optional around * and /)
            SkipWhitespace(stream);

            var token = stream.Next;
            if (token.Type != ECssTokenType.Delim)
            {
                break;
            }

            var delimToken = (DelimToken)token;
            bool isMul = delimToken.Value == '*';
            bool isDiv = delimToken.Value == '/';

            if (!isMul && !isDiv)
            {
                break;
            }

            // Consume the operator
            stream.Consume();
            SkipWhitespace(stream);

            // Parse next calc-value
            var right = ParseCalcValue(stream);
            if (right is null)
            {
                return null; // Invalid expression
            }

            // Division becomes multiplication by inverted value
            if (isDiv)
            {
                terms.Add(new CssCalcInvertNode(right));
            }
            else
            {
                terms.Add(right);
            }
        }

        // If only one term, return it directly
        if (terms.Count == 1)
        {
            return terms[0];
        }

        return new CssCalcProductNode(terms);
    }

    /// <summary>
    /// Parses the calc-value production: number | dimension | percentage | ( calc-sum ) | calc-keyword
    /// </summary>
    private static CssCalcNode? ParseCalcValue(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);

        if (CssParsingHelpers.IsAtEnd(stream))
        {
            return null;
        }

        var token = stream.Next;

        switch (token.Type)
        {
            case ECssTokenType.Number:
                {
                    var numToken = (NumberToken)stream.Consume();
                    return new CssCalcValueNode(numToken.AsNumber);
                }

            case ECssTokenType.Percentage:
                {
                    var pctToken = (PercentageToken)stream.Consume();
                    return new CssCalcValueNode(pctToken.Number, ECssUnit.None, isPercentage: true);
                }

            case ECssTokenType.Dimension:
                {
                    var dimToken = (DimensionToken)stream.Consume();
                    var unit = Lookup.Enum<ECssUnit>(dimToken.Unit);
                    return new CssCalcValueNode(dimToken.AsNumber, unit);
                }

            case ECssTokenType.Ident:
                {
                    // Handle calc keywords: e, pi, infinity, -infinity, NaN
                    var identToken = (IdentToken)token;
                    var value = identToken.Value;

                    if (value.Equals("e", StringComparison.OrdinalIgnoreCase))
                    {
                        stream.Consume();
                        return new CssCalcValueNode(Math.E);
                    }
                    if (value.Equals("pi", StringComparison.OrdinalIgnoreCase))
                    {
                        stream.Consume();
                        return new CssCalcValueNode(Math.PI);
                    }
                    if (value.Equals("infinity", StringComparison.OrdinalIgnoreCase))
                    {
                        stream.Consume();
                        return new CssCalcValueNode(double.PositiveInfinity);
                    }
                    // Note: -infinity is tokenized as a single ident token by the CSS tokenizer
                    // because `-` followed by a name-start character forms an identifier
                    if (value.Equals("-infinity", StringComparison.OrdinalIgnoreCase))
                    {
                        stream.Consume();
                        return new CssCalcValueNode(double.NegativeInfinity);
                    }
                    if (value.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                    {
                        stream.Consume();
                        return new CssCalcValueNode(double.NaN);
                    }

                    // Unknown identifier
                    return null;
                }

            case ECssTokenType.Delim:
                {
                    // Handle unary minus: -value or -infinity
                    var delimToken = (DelimToken)token;
                    if (delimToken.Value == '-')
                    {
                        stream.Consume();

                        // Check for -infinity keyword
                        if (!CssParsingHelpers.IsAtEnd(stream) &&
                            stream.Next.Type == ECssTokenType.Ident)
                        {
                            var identToken = (IdentToken)stream.Next;
                            if (identToken.Value.Equals("infinity", StringComparison.OrdinalIgnoreCase))
                            {
                                stream.Consume();
                                return new CssCalcValueNode(double.NegativeInfinity);
                            }
                        }

                        // Otherwise parse the value and negate it
                        var inner = ParseCalcValue(stream);
                        if (inner is null)
                            return null;
                        return new CssCalcNegateNode(inner);
                    }

                    // Handle unary plus (explicit positive)
                    if (delimToken.Value == '+')
                    {
                        stream.Consume();
                        return ParseCalcValue(stream);
                    }

                    return null;
                }

            case ECssTokenType.Parenth_Open:
                {
                    // Parenthesized expression
                    stream.Consume(); // Consume '('
                    SkipWhitespace(stream);

                    var inner = ParseCalcSum(stream);
                    if (inner is null)
                        return null;

                    SkipWhitespace(stream);

                    // Expect closing paren
                    if (CssParsingHelpers.IsAtEnd(stream) ||
                        stream.Next.Type != ECssTokenType.Parenth_Close)
                    {
                        return null; // Missing closing paren
                    }

                    stream.Consume(); // Consume ')'
                    return inner;
                }

            case ECssTokenType.SimpleBlock:
                {
                    // Handle SimpleBlock from pre-parsed tokens (like component values)
                    var block = (CssSimpleBlock)stream.Consume();

                    // Only handle parenthesized blocks
                    if (block.StartToken.Type != ECssTokenType.Parenth_Open)
                    {
                        return null;
                    }

                    // Parse the contents of the block
                    var innerStream = CssParsingHelpers.CreateTokenStream(block.Values, preserveWhitespace: true);
                    var inner = ParseCalcSum(innerStream);

                    // Ensure all tokens were consumed
                    SkipWhitespace(innerStream);
                    if (!CssParsingHelpers.IsAtEnd(innerStream))
                    {
                        return null;
                    }

                    return inner;
                }

            case ECssTokenType.FunctionName:
            case ECssTokenType.Function:
                {
                    // Handle nested calc() or other math functions
                    // @todo: Support nested calc(), min(), max(), clamp()
                    // For now, reject nested functions
                    return null;
                }

            default:
                return null;
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Skips whitespace tokens in the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SkipWhitespace(DataConsumer<CssToken> stream)
    {
        while (!CssParsingHelpers.IsAtEnd(stream) &&
               stream.Next.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }
    }

    #endregion
}
