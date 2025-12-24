using System;
using System.Diagnostics.CodeAnalysis;
using CssUI.CSS.Serialization;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parser for the Unicode-Range microsyntax as defined in CSS Syntax Level 3 §7.
/// </summary>
/// <remarks>
/// <para>
/// The &lt;urange&gt; notation was originally defined as a primitive token in CSS, but it is used
/// very rarely, and collides with legitimate &lt;ident-token&gt;s in confusing ways. This parser
/// describes how to recognize the &lt;urange&gt; notation in terms of existing CSS tokens.
/// </para>
/// <para>
/// The &lt;urange&gt; production has three informal forms:
/// <list type="bullet">
/// <item><description>U+0001 - Defines a range consisting of a single code point.</description></item>
/// <item><description>U+0001-00ff - Defines a range of codepoints between the first and second value inclusive.</description></item>
/// <item><description>U+00?? - Defines a range using wildcards.</description></item>
/// </list>
/// </para>
/// <para>
/// Note: The CSS tokenizer (CssTokenizer) already produces UnicodeRangeToken instances when it
/// encounters unicode-range syntax. This parser provides a higher-level API for working with
/// these tokens and creating CssUnicodeRange values from them.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#urange"/>
public static class CssUnicodeRangeParser
{
    #region Constants

    /// <summary>
    /// Maximum allowed code point (U+10FFFF).
    /// </summary>
    private const int MaxCodePoint = CssUnicodeRange.MaxCodePoint;

    #endregion

    #region Public API

    /// <summary>
    /// Attempts to parse a unicode-range value from a stream of CSS tokens.
    /// </summary>
    /// <param name="stream">The token stream to parse from.</param>
    /// <param name="result">When this method returns, contains the parsed unicode range, if successful.</param>
    /// <returns>True if the unicode range was successfully parsed; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// This method looks for a <see cref="UnicodeRangeToken"/> in the token stream.
    /// The CSS tokenizer already handles the complex parsing of unicode-range syntax
    /// (U+XXXX, U+XXXX-XXXX, U+XX??) and produces UnicodeRangeToken instances.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#urange-syntax"/>
    public static bool TryParse(DataConsumer<CssToken> stream, [NotNullWhen(true)] out CssUnicodeRange? result)
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

        // The CSS tokenizer produces UnicodeRangeToken for valid unicode-range syntax
        if (stream.Next is UnicodeRangeToken unicodeRangeToken)
        {
            stream.Consume();

            // Validate the range values
            if (!TryCreateRange(unicodeRangeToken.Start, unicodeRangeToken.End, out result))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Parses a unicode-range value from a string.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <returns>The parsed unicode range.</returns>
    /// <exception cref="CssParserException">Thrown when the input cannot be parsed as a unicode range.</exception>
    public static CssUnicodeRange Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new CssParserException("Input cannot be null or empty.");
        }

        var tokenizer = new CssTokenizer(input);
        var tokens = tokenizer.Tokens;
        var stream = new DataConsumer<CssToken>(tokens);

        if (TryParse(stream, out var result))
        {
            // Ensure all tokens were consumed (except EOF and trailing whitespace)
            while (stream.Next?.Type == ECssTokenType.Whitespace)
            {
                stream.Consume();
            }

            if (stream.Next?.Type == ECssTokenType.EOF || stream.Next == null)
            {
                return result.Value;
            }
            throw new CssParserException($"Unexpected tokens after unicode-range: '{stream.Next}'");
        }

        throw new CssParserException($"Failed to parse unicode-range from input: '{input}'");
    }

    /// <summary>
    /// Attempts to parse a unicode-range value from a string.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <param name="result">When this method returns, contains the parsed unicode range, if successful.</param>
    /// <returns>True if the unicode range was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(string input, [NotNullWhen(true)] out CssUnicodeRange? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        try
        {
            var tokenizer = new CssTokenizer(input);
            var tokens = tokenizer.Tokens;
            var stream = new DataConsumer<CssToken>(tokens);

            if (TryParse(stream, out result))
            {
                // Ensure all tokens were consumed (except EOF and trailing whitespace)
                while (stream.Next?.Type == ECssTokenType.Whitespace)
                {
                    stream.Consume();
                }

                return stream.Next?.Type == ECssTokenType.EOF || stream.Next == null;
            }
        }
        catch
        {
            // Parse failed
        }

        return false;
    }

    /// <summary>
    /// Creates a CssUnicodeRange from a UnicodeRangeToken.
    /// </summary>
    /// <param name="token">The unicode range token.</param>
    /// <returns>The CssUnicodeRange value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when token is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the token values are invalid.</exception>
    public static CssUnicodeRange FromToken(UnicodeRangeToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        if (token.End > MaxCodePoint)
        {
            throw new ArgumentOutOfRangeException(nameof(token), $"End value {token.End} exceeds maximum code point U+{MaxCodePoint:X}");
        }

        if (token.Start > token.End)
        {
            throw new ArgumentOutOfRangeException(nameof(token), $"Start value {token.Start} exceeds end value {token.End}");
        }

        if (token.Start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(token), $"Start value {token.Start} cannot be negative");
        }

        return new CssUnicodeRange(token.Start, token.End);
    }

    /// <summary>
    /// Attempts to create a CssUnicodeRange from a UnicodeRangeToken.
    /// </summary>
    /// <param name="token">The unicode range token.</param>
    /// <param name="result">When this method returns, contains the CssUnicodeRange value, if successful.</param>
    /// <returns>True if the CssUnicodeRange was successfully created; otherwise, false.</returns>
    public static bool TryFromToken(UnicodeRangeToken? token, [NotNullWhen(true)] out CssUnicodeRange? result)
    {
        result = null;

        if (token == null)
        {
            return false;
        }

        return TryCreateRange(token.Start, token.End, out result);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tries to create a CssUnicodeRange, validating the values.
    /// </summary>
    private static bool TryCreateRange(int start, int end, [NotNullWhen(true)] out CssUnicodeRange? result)
    {
        result = null;

        // Validation per spec:
        // 1. If end value is greater than maximum allowed code point, invalid
        if (end > MaxCodePoint)
        {
            return false;
        }

        // 2. If start value is greater than end value, invalid
        if (start > end)
        {
            return false;
        }

        // 3. Negative values are invalid
        if (start < 0)
        {
            return false;
        }

        result = new CssUnicodeRange(start, end);
        return true;
    }

    #endregion
}
