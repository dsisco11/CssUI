using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parses CSS var() function syntax per CSS Custom Properties (Variables) Level 1.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-variables-1/#using-variables
/// 
/// Grammar:
/// <code>
/// var() = var( &lt;custom-property-name&gt; [, &lt;declaration-value&gt;]? )
/// </code>
/// 
/// The &lt;custom-property-name&gt; is a &lt;dashed-ident&gt; (identifier starting with --).
/// The optional fallback &lt;declaration-value&gt; can contain any sequence of tokens,
/// including nested var() references.
/// 
/// Per spec:
/// - Property names are case-sensitive
/// - var(--a,) is valid (empty fallback)
/// - The fallback can contain commas: var(--font, Arial, sans-serif)
/// - The fallback must match the &lt;declaration-value&gt; production
/// </remarks>
internal static class CssVarFunctionParser
{
    #region Public API

    /// <summary>
    /// Attempts to parse a var() function and return a <see cref="CssVarFunction"/>.
    /// </summary>
    /// <param name="function">The CSS function to parse.</param>
    /// <param name="result">The resulting var function reference if successful.</param>
    /// <returns>True if the function was a valid var() and parsing succeeded.</returns>
    /// <remarks>
    /// The fallback value (if present) is validated against the &lt;declaration-value&gt; production
    /// per CSS Syntax Level 3 §8.2. The validation result is stored in <see cref="CssVarFunction.FallbackValidation"/>.
    /// </remarks>
    public static bool TryParseVarFunction(CssFunction function, out CssVarFunction result)
    {
        ArgumentNullException.ThrowIfNull(function);

        result = CssVarFunction.Empty;

        // Check function name (case-insensitive)
        if (!function.Name.Equals("var", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = function.Arguments;
        if (arguments is null || arguments.Count == 0)
        {
            // var() with no arguments is invalid
            return false;
        }

        // Create token stream, preserving whitespace for accurate parsing
        var stream = CssParsingHelpers.CreateTokenStream(arguments, preserveWhitespace: false);
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            return false;
        }

        // First token must be an ident starting with --
        var firstToken = stream.Next;
        if (firstToken is not IdentToken identToken)
        {
            return false;
        }

        string propertyName = identToken.Value;
        if (!IsValidCustomPropertyName(propertyName))
        {
            return false;
        }

        stream.Consume(); // Consume the property name

        // Check for optional fallback after comma
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            // No fallback - valid var(--prop)
            result = new CssVarFunction(propertyName);
            return true;
        }

        // Next token should be a comma if there's a fallback
        var nextToken = stream.Next;
        if (nextToken is not CommaToken)
        {
            // Extra tokens but no comma - invalid
            return false;
        }

        stream.Consume(); // Consume the comma

        // Everything after the comma is the fallback value
        // Per spec, var(--a,) is valid (empty fallback)
        if (CssParsingHelpers.IsAtEnd(stream))
        {
            // Empty fallback - valid per spec
            result = new CssVarFunction(propertyName, CssValue.Null);
            return true;
        }

        // Collect remaining tokens as fallback
        var fallbackTokens = CollectFallbackTokens(stream);

        // Validate fallback against <declaration-value> production per CSS Variables spec
        var validationResult = CssProductionMatcher.MatchDeclarationValue(fallbackTokens);

        // Try to parse the fallback as a CssValue
        // The fallback can be complex (multiple values, nested var(), etc.)
        CssValue? fallbackValue = TryParseFallbackValue(fallbackTokens);

        if (fallbackValue is not null)
        {
            result = new CssVarFunction(propertyName, fallbackValue, new ReadOnlyMemory<CssToken>(fallbackTokens), validationResult);
        }
        else
        {
            // Store just the raw tokens if we couldn't parse a simple value
            result = new CssVarFunction(propertyName, new ReadOnlyMemory<CssToken>(fallbackTokens), validationResult);
        }

        return true;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Validates that a property name is a valid custom property name.
    /// </summary>
    /// <remarks>
    /// Per CSS Variables Level 1:
    /// - Custom property names must start with two dashes (--)
    /// - Cannot be just "--" (reserved for future use)
    /// - Are case-sensitive
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidCustomPropertyName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        // Must start with --
        if (!name.StartsWith("--", StringComparison.Ordinal))
            return false;

        // Cannot be just "--"
        if (name.Length <= 2)
            return false;

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
    /// For complex fallbacks (multiple values, nested var()), we return null
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

        return Lookup.Enum<ECssUnit>(unitStr);
    }

    #endregion
}
