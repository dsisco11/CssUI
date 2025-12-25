using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parses CSS env() function syntax per CSS Environment Variables Level 1.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-env-1/#env-function
/// 
/// Grammar:
/// <code>
/// env() = env( &lt;custom-ident&gt; &lt;integer [0,∞]&gt;* [, &lt;declaration-value&gt;]? )
/// </code>
/// 
/// The &lt;custom-ident&gt; is the environment variable name.
/// The optional integers are dimension indices for indexed variables (e.g., viewport-segment-*).
/// The optional fallback &lt;declaration-value&gt; can contain any sequence of tokens,
/// including nested env() or var() references.
/// 
/// Per spec:
/// - UA-defined variable names are case-insensitive (safe-area-inset-top)
/// - Author-defined variable names (--*) are case-sensitive
/// - The fallback can contain commas: env(foo, red, blue)
/// - Indices must be non-negative integers
/// </remarks>
internal static class CssEnvFunctionParser
{
    #region Public API

    /// <summary>
    /// Attempts to parse an env() function and return a <see cref="CssEnvFunction"/>.
    /// </summary>
    /// <param name="function">The CSS function to parse.</param>
    /// <param name="result">The resulting env function reference if successful.</param>
    /// <returns>True if the function was a valid env() and parsing succeeded.</returns>
    public static bool TryParseEnvFunction(CssFunction function, out CssEnvFunction result)
    {
        ArgumentNullException.ThrowIfNull(function);

        result = CssEnvFunction.Empty;

        // Check function name (case-insensitive)
        if (!function.Name.Equals("env", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = function.Arguments;
        if (arguments is null || arguments.Count == 0)
        {
            // env() with no arguments is invalid
            return false;
        }

        // Create token stream, filtering whitespace for cleaner parsing
        var stream = CssParsingHelpers.CreateTokenStream(arguments, preserveWhitespace: false);
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // First token must be an ident (variable name)
        var firstToken = stream.Next;
        if (firstToken is not IdentToken identToken)
        {
            return false;
        }

        string variableName = identToken.Value;
        if (string.IsNullOrEmpty(variableName))
        {
            return false;
        }

        stream.Consume(); // Consume the variable name

        // Check for optional indices and fallback
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            // No indices, no fallback - valid env(name)
            result = new CssEnvFunction(variableName);
            return true;
        }

        // Parse optional indices (non-negative integers before the comma)
        var indices = new List<int>();
        while (!CssParsingHelpers.IsAtEnd(stream))
        {
            var nextToken = stream.Next;
            
            // If we hit a comma, we're done with indices
            if (nextToken is CommaToken)
            {
                break;
            }

            // Check for integer token
            if (nextToken is NumberToken numToken)
            {
                // Must be a non-negative integer
                if (!IsNonNegativeInteger(numToken, out int index))
                {
                    // Not a valid index, treat remaining as invalid
                    return false;
                }
                
                indices.Add(index);
                stream.Consume();
            }
            else
            {
                // Not a number and not a comma - invalid
                return false;
            }
        }

        ReadOnlyMemory<int> indicesMemory = indices.Count > 0 
            ? new ReadOnlyMemory<int>(indices.ToArray()) 
            : ReadOnlyMemory<int>.Empty;

        // Check for optional fallback after comma
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            // No fallback - valid env(name) or env(name i1 i2 ...)
            result = new CssEnvFunction(variableName, indicesMemory);
            return true;
        }

        // Next token should be a comma if there's a fallback
        var commaToken = stream.Next;
        if (commaToken is not CommaToken)
        {
            // Expected comma but got something else
            return false;
        }

        stream.Consume(); // Consume the comma

        // Everything after the comma is the fallback value
        // Per spec, env(foo,) is valid (empty fallback)
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            // Empty fallback - valid per spec
            result = new CssEnvFunction(variableName, indicesMemory, CssValue.Null);
            return true;
        }

        // Collect remaining tokens as fallback
        var fallbackTokens = CollectFallbackTokens(stream);

        // Try to parse the fallback as a CssValue
        CssValue? fallbackValue = TryParseFallbackValue(fallbackTokens);

        if (fallbackValue is not null)
        {
            result = new CssEnvFunction(variableName, indicesMemory, fallbackValue, new ReadOnlyMemory<CssToken>(fallbackTokens));
        }
        else
        {
            // Store just the raw tokens if we couldn't parse a simple value
            result = new CssEnvFunction(variableName, indicesMemory, new ReadOnlyMemory<CssToken>(fallbackTokens));
        }

        return true;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Checks if a number token represents a non-negative integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNonNegativeInteger(NumberToken token, out int value)
    {
        value = 0;
        
        // Check if it's an integer type (not a decimal/number)
        if (token.DataType != ENumericTokenType.Integer)
        {
            return false;
        }

        double num = token.AsNumber;
        
        // Must be non-negative
        if (num < 0)
        {
            return false;
        }

        // Must be within integer range
        if (num > int.MaxValue)
        {
            return false;
        }

        value = (int)num;
        return true;
    }

    /// <summary>
    /// Collects all remaining tokens in the stream as fallback tokens.
    /// </summary>
    private static CssToken[] CollectFallbackTokens(DataConsumer<CssToken> stream)
    {
        var tokens = new List<CssToken>();

        while (!CssParsingHelpers.IsAtEnd(stream))
        {
            var token = stream.Consume();
            if (token.Type != ECssTokenType.EOF)
            {
                tokens.Add(token);
            }
        }

        return tokens.ToArray();
    }

    /// <summary>
    /// Attempts to parse fallback tokens as a simple CssValue.
    /// </summary>
    /// <remarks>
    /// For simple fallbacks (single value), we return a parsed CssValue.
    /// For complex fallbacks (multiple values, nested env()/var()), we return null
    /// and let the caller use the raw tokens.
    /// </remarks>
    private static CssValue? TryParseFallbackValue(CssToken[] tokens)
    {
        if (tokens.Length == 0)
            return CssValue.Null;

        // Filter out whitespace for counting meaningful tokens
        int meaningfulCount = 0;
        foreach (var token in tokens)
        {
            if (token is not WhitespaceToken)
                meaningfulCount++;
        }

        // For single simple values, parse directly
        if (meaningfulCount == 1)
        {
            foreach (var token in tokens)
            {
                if (token is WhitespaceToken)
                    continue;

                // Try to create a CssValue from a single token
                return token switch
                {
                    NumberToken num => CssValue.From(num.AsNumber),
                    DimensionToken dim => CssValue.From(dim.AsNumber, GetUnit(dim.Unit)),
                    PercentageToken pct => CssValue.From_Percent(pct.Number),
                    StringToken str => CssValue.From_String(str.Value ?? string.Empty),
                    IdentToken ident => new CssValue(ECssValueTypes.KEYWORD, ident.Value),
                    HashToken hash => TryParseHashColor(hash),
                    _ => null
                };
            }
        }

        // Complex fallback - return null to use raw tokens
        return null;
    }

    /// <summary>
    /// Attempts to parse a hash token as a color value.
    /// </summary>
    private static CssValue? TryParseHashColor(HashToken token)
    {
        if (CssHexColorParser.TryParse(token.Value, out CssColor color))
        {
            return CssValue.From(color);
        }
        return null;
    }

    /// <summary>
    /// Gets the ECssUnit from a unit string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ECssUnit GetUnit(string? unitStr)
    {
        if (string.IsNullOrEmpty(unitStr))
            return ECssUnit.None;

        return ECssUnitExtensions.FromKeyword(unitStr);
    }

    #endregion
}
