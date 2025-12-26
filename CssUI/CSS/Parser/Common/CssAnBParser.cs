using System;
using System.Diagnostics.CodeAnalysis;
using CssUI.CSS.Serialization;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parser for the An+B microsyntax as defined in CSS Syntax Level 3 §6.
/// </summary>
/// <remarks>
/// <para>
/// The An+B notation defines an integer step (A) and offset (B), and represents the An+Bth
/// elements in a list, for every positive integer or zero value of n.
/// </para>
/// <para>
/// Several things in CSS, such as the ':nth-child()' pseudo-class, need to indicate indexes
/// in a list. The An+B microsyntax is useful for this, allowing an author to easily indicate
/// single elements or all elements at regularly-spaced intervals in a list.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#anb-microsyntax"/>
public static class CssAnBParser
{
    /// <summary>
    /// Attempts to parse an An+B value from a stream of CSS tokens.
    /// </summary>
    /// <param name="stream">The token stream to parse from.</param>
    /// <param name="result">When this method returns, contains the parsed An+B value, if successful.</param>
    /// <returns>True if the An+B value was successfully parsed; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// The &lt;an+b&gt; type is defined as:
    /// </para>
    /// <code>
    /// &lt;an+b&gt; =
    ///   odd | even |
    ///   &lt;integer&gt; |
    ///   &lt;n-dimension&gt; |
    ///   '+'? n |
    ///   -n |
    ///   &lt;ndashdigit-dimension&gt; |
    ///   '+'? &lt;ndashdigit-ident&gt; |
    ///   &lt;dashndashdigit-ident&gt; |
    ///   &lt;n-dimension&gt; &lt;signed-integer&gt; |
    ///   '+'? n &lt;signed-integer&gt; |
    ///   -n &lt;signed-integer&gt; |
    ///   &lt;ndash-dimension&gt; &lt;signless-integer&gt; |
    ///   '+'? n- &lt;signless-integer&gt; |
    ///   -n- &lt;signless-integer&gt; |
    ///   &lt;n-dimension&gt; ['+' | '-'] &lt;signless-integer&gt; |
    ///   '+'? n ['+' | '-'] &lt;signless-integer&gt; |
    ///   -n ['+' | '-'] &lt;signless-integer&gt;
    /// </code>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#the-anb-type"/>
    public static bool TryParse(DataConsumer<CssToken> stream, [NotNullWhen(true)] out CssAnB? result)
    {
        result = null;

        if (stream.Next == null || stream.Next.Type == ECssTokenType.EOF)
        {
            return false;
        }

        // Skip leading whitespace
        while (stream.Next?.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }

        var startPosition = stream.Position;

        // Check for 'odd' or 'even' keywords
        if (stream.Next is IdentToken identToken)
        {
            if (identToken.Value.Equals("odd", StringComparison.OrdinalIgnoreCase))
            {
                stream.Consume();
                result = CssAnB.Odd;
                return true;
            }
            if (identToken.Value.Equals("even", StringComparison.OrdinalIgnoreCase))
            {
                stream.Consume();
                result = CssAnB.Even;
                return true;
            }

            // Check for 'n', '-n', or identifiers starting with 'n-' followed by digits
            return TryParseIdentBasedAnB(stream, out result);
        }

        // Check for <integer> (A=0, B=value)
        if (stream.Next is NumberToken numberToken && numberToken.DataType == ENumericTokenType.Integer)
        {
            stream.Consume();
            result = new CssAnB(0, (int)numberToken.AsInteger);
            return true;
        }

        // Check for <n-dimension>, <ndash-dimension>, <ndashdigit-dimension>
        if (stream.Next is DimensionToken dimensionToken && dimensionToken.DataType == ENumericTokenType.Integer)
        {
            return TryParseDimensionBasedAnB(stream, out result);
        }

        // Check for '+' or '-' followed by 'n'
        if (stream.Next is DelimToken delimToken && (delimToken.Value == '+' || delimToken.Value == '-'))
        {
            return TryParseSignBasedAnB(stream, out result);
        }

        return false;
    }

    /// <summary>
    /// Parses An+B values that start with an identifier token.
    /// Handles: 'n', '-n', 'n-*', '-n-*' patterns
    /// </summary>
    private static bool TryParseIdentBasedAnB(DataConsumer<CssToken> stream, [NotNullWhen(true)] out CssAnB? result)
    {
        result = null;

        if (stream.Next is not IdentToken identToken)
        {
            return false;
        }

        string value = identToken.Value;

        // Handle 'n' (A=1, B=0) or '-n' (A=-1, B=0) followed by optional signed integer
        if (value.Equals("n", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteAnB(stream, 1, out result);
        }

        if (value.Equals("-n", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteAnB(stream, -1, out result);
        }

        // Handle <ndashdigit-ident>: 'n-*' where * is one or more digits (e.g., "n-6")
        if (value.StartsWith("n-", StringComparison.OrdinalIgnoreCase) && value.Length > 2)
        {
            if (TryParseTrailingDigits(value.AsSpan(2), out int b))
            {
                stream.Consume();
                result = new CssAnB(1, -b);
                return true;
            }
        }

        // Handle <dashndashdigit-ident>: '-n-*' where * is one or more digits (e.g., "-n-6")
        if (value.StartsWith("-n-", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
        {
            if (TryParseTrailingDigits(value.AsSpan(3), out int b))
            {
                stream.Consume();
                result = new CssAnB(-1, -b);
                return true;
            }
        }

        // Handle 'n-' followed by <signless-integer>
        if (value.Equals("n-", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteWithSignlessInteger(stream, 1, negate: true, out result);
        }

        // Handle '-n-' followed by <signless-integer>
        if (value.Equals("-n-", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteWithSignlessInteger(stream, -1, negate: true, out result);
        }

        return false;
    }

    /// <summary>
    /// Parses An+B values that start with a dimension token.
    /// Handles: &lt;n-dimension&gt;, &lt;ndash-dimension&gt;, &lt;ndashdigit-dimension&gt;
    /// </summary>
    private static bool TryParseDimensionBasedAnB(DataConsumer<CssToken> stream, [NotNullWhen(true)] out CssAnB? result)
    {
        result = null;

        if (stream.Next is not DimensionToken dimensionToken || dimensionToken.DataType != ENumericTokenType.Integer)
        {
            return false;
        }

        int a = (int)dimensionToken.AsInteger;
        string unit = dimensionToken.Unit;

        // Handle <n-dimension>: unit is exactly "n"
        if (unit.Equals("n", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteAnB(stream, a, out result);
        }

        // Handle <ndash-dimension>: unit is exactly "n-"
        if (unit.Equals("n-", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteWithSignlessInteger(stream, a, negate: true, out result);
        }

        // Handle <ndashdigit-dimension>: unit is "n-*" where * is digits (e.g., "2n-6" → unit is "n-6")
        if (unit.StartsWith("n-", StringComparison.OrdinalIgnoreCase) && unit.Length > 2)
        {
            if (TryParseTrailingDigits(unit.AsSpan(2), out int b))
            {
                stream.Consume();
                result = new CssAnB(a, -b);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Parses An+B values that start with a '+' or '-' delimiter token.
    /// Must be immediately followed by 'n' identifier (no whitespace allowed).
    /// </summary>
    private static bool TryParseSignBasedAnB(DataConsumer<CssToken> stream, [NotNullWhen(true)] out CssAnB? result)
    {
        result = null;

        if (stream.Next is not DelimToken delimToken)
        {
            return false;
        }

        bool isPositive = delimToken.Value == '+';
        int sign = isPositive ? 1 : -1;

        // Per spec: "When a plus sign (+) precedes an ident starting with 'n',
        // there must be no whitespace between the two tokens"
        // For '-', this forms the '-n' ident token during tokenization, so we only handle '+' here
        if (!isPositive)
        {
            return false; // '-n' would be tokenized as a single ident token
        }

        stream.Consume(); // Consume the '+' delimiter

        // The next token must be an ident starting with 'n' (no whitespace allowed)
        if (stream.Next is not IdentToken identToken)
        {
            return false;
        }

        string value = identToken.Value;

        // Handle '+n' → A=1
        if (value.Equals("n", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteAnB(stream, sign, out result);
        }

        // Handle '+n-' followed by <signless-integer>
        if (value.Equals("n-", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume();
            return TryCompleteWithSignlessInteger(stream, sign, negate: true, out result);
        }

        // Handle '+n-*' where * is digits
        if (value.StartsWith("n-", StringComparison.OrdinalIgnoreCase) && value.Length > 2)
        {
            if (TryParseTrailingDigits(value.AsSpan(2), out int b))
            {
                stream.Consume();
                result = new CssAnB(sign, -b);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Completes parsing of An+B after the 'n' part has been consumed.
    /// Handles the optional signed integer or sign + signless integer that follows.
    /// </summary>
    private static bool TryCompleteAnB(DataConsumer<CssToken> stream, int a, [NotNullWhen(true)] out CssAnB? result)
    {
        // Skip whitespace
        while (stream.Next?.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }

        // If at end, B=0
        if (stream.Next == null || stream.Next.Type == ECssTokenType.EOF)
        {
            result = new CssAnB(a, 0);
            return true;
        }

        // Check for <signed-integer> (starts with + or -)
        if (stream.Next is NumberToken signedNumber && signedNumber.DataType == ENumericTokenType.Integer)
        {
            // Check if the token's representation starts with + or -
            if (signedNumber.Value != null && signedNumber.Value.Length > 0)
            {
                char firstChar = signedNumber.Value[0];
                if (firstChar == '+' || firstChar == '-')
                {
                    stream.Consume();
                    result = new CssAnB(a, (int)signedNumber.AsInteger);
                    return true;
                }
            }
            // Not a signed integer - B=0
            result = new CssAnB(a, 0);
            return true;
        }

        // Check for '+' or '-' delimiter followed by <signless-integer>
        if (stream.Next is DelimToken delimToken && (delimToken.Value == '+' || delimToken.Value == '-'))
        {
            bool negate = delimToken.Value == '-';
            stream.Consume(); // Consume the sign delimiter

            // Skip whitespace between sign and number
            while (stream.Next?.Type == ECssTokenType.Whitespace)
            {
                stream.Consume();
            }

            // Must be followed by a signless integer
            if (stream.Next is NumberToken signlessNumber && signlessNumber.DataType == ENumericTokenType.Integer)
            {
                // Verify it's signless (doesn't start with + or -)
                if (signlessNumber.Value != null && signlessNumber.Value.Length > 0)
                {
                    char firstChar = signlessNumber.Value[0];
                    if (char.IsDigit(firstChar))
                    {
                        stream.Consume();
                        int b = (int)signlessNumber.AsInteger;
                        result = new CssAnB(a, negate ? -b : b);
                        return true;
                    }
                }
            }

            // Invalid - expected signless integer after sign
            result = null;
            return false;
        }

        // No B value specified
        result = new CssAnB(a, 0);
        return true;
    }

    /// <summary>
    /// Completes parsing after 'n-' by expecting a signless integer.
    /// </summary>
    private static bool TryCompleteWithSignlessInteger(DataConsumer<CssToken> stream, int a, bool negate, [NotNullWhen(true)] out CssAnB? result)
    {
        result = null;

        // Skip whitespace
        while (stream.Next?.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }

        // Must be followed by a signless integer
        if (stream.Next is NumberToken numberToken && numberToken.DataType == ENumericTokenType.Integer)
        {
            // Verify it's signless (representation doesn't start with + or -)
            if (numberToken.Value != null && numberToken.Value.Length > 0)
            {
                char firstChar = numberToken.Value[0];
                if (char.IsDigit(firstChar))
                {
                    stream.Consume();
                    int b = (int)numberToken.AsInteger;
                    result = new CssAnB(a, negate ? -b : b);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to parse a sequence of digits as a non-negative integer.
    /// </summary>
    private static bool TryParseTrailingDigits(ReadOnlySpan<char> span, out int value)
    {
        value = 0;

        if (span.IsEmpty)
        {
            return false;
        }

        // All characters must be digits
        foreach (char c in span)
        {
            if (!char.IsAsciiDigit(c))
            {
                return false;
            }
        }

#if NET7_0_OR_GREATER
        return int.TryParse(span, out value);
#else
        return int.TryParse(span.ToString(), out value);
#endif
    }

    /// <summary>
    /// Parses an An+B value from a string.
    /// </summary>
    /// <param name="input">The CSS string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed An+B value, if successful.</param>
    /// <returns>True if the An+B value was successfully parsed; otherwise, false.</returns>
    [Obsolete("Use CssAnB.TryParse(string, IFormatProvider?, out CssAnB) instead.")]
    public static bool TryParse(string input, [NotNullWhen(true)] out CssAnB? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var tokenizer = new CssTokenizer(input);
        var tokens = tokenizer.Tokens;
        var stream = new DataConsumer<CssToken>(tokens, CssToken.EOF);

        if (!TryParse(stream, out result))
        {
            return false;
        }

        // Skip trailing whitespace
        while (stream.Next?.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }

        // Ensure we consumed all tokens
        return stream.Next?.Type == ECssTokenType.EOF;
    }

    /// <summary>
    /// Parses an An+B value from a string, throwing an exception on failure.
    /// </summary>
    /// <param name="input">The CSS string to parse.</param>
    /// <returns>The parsed An+B value.</returns>
    /// <exception cref="CssSyntaxErrorException">Thrown when the input cannot be parsed as a valid An+B value.</exception>
    [Obsolete("Use CssAnB.Parse(string, IFormatProvider?) instead.")]
    public static CssAnB Parse(string input)
    {
        if (TryParse(input, out var result))
        {
            return result.Value;
        }

        throw new CssSyntaxErrorException($"Invalid An+B syntax: '{input}'");
    }
}
