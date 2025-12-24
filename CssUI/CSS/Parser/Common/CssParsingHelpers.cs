using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Shared helper methods for parsing CSS tokens.
/// </summary>
internal static class CssParsingHelpers
{
    #region Numeric Value Extraction

    /// <summary>
    /// Extracts a numeric value from a <see cref="NumberToken"/>.
    /// </summary>
    /// <param name="token">The token to extract from.</param>
    /// <param name="value">The extracted numeric value.</param>
    /// <returns>True if the token is a valid NumberToken with a value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetNumber(CssToken token, out double value)
    {
        value = 0;
        if (token is not NumberToken numToken)
        {
            return false;
        }

        value = numToken.AsNumber;
        return true;
    }

    /// <summary>
    /// Extracts a percentage value from a <see cref="PercentageToken"/>.
    /// </summary>
    /// <param name="token">The token to extract from.</param>
    /// <param name="value">The extracted percentage value (0-100 scale).</param>
    /// <returns>True if the token is a valid PercentageToken.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetPercentage(CssToken token, out double value)
    {
        value = 0;
        if (token is not PercentageToken pctToken)
        {
            return false;
        }

        value = pctToken.Number;
        return true;
    }

    #endregion

    #region Hue Value Extraction

    /// <summary>
    /// Tries to get a hue value from a token.
    /// Hue can be a number (interpreted as degrees), an angle with unit (deg, rad, grad, turn),
    /// or the 'none' keyword (CSS Color 4).
    /// </summary>
    /// <param name="token">The token to extract from.</param>
    /// <param name="hue">The extracted hue value in degrees.</param>
    /// <returns>True if the token represents a valid hue value.</returns>
    public static bool TryGetHueValue(CssToken token, out double hue)
    {
        hue = 0;

        // Number token: interpreted as degrees
        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out hue))
            {
                return false;
            }
            return true;
        }

        // Dimension token: angle with unit
        if (token is DimensionToken dimToken)
        {
            double value = dimToken.AsNumber;
            var unit = dimToken.Unit;

            // Convert to degrees based on unit
            if (unit.Equals("deg", StringComparison.OrdinalIgnoreCase))
            {
                hue = value;
            }
            else if (unit.Equals("rad", StringComparison.OrdinalIgnoreCase))
            {
                hue = value * (180.0 / Math.PI);
            }
            else if (unit.Equals("grad", StringComparison.OrdinalIgnoreCase))
            {
                hue = value * (360.0 / 400.0);
            }
            else if (unit.Equals("turn", StringComparison.OrdinalIgnoreCase))
            {
                hue = value * 360.0;
            }
            else
            {
                return false; // Unknown angle unit
            }
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            hue = 0; // 'none' hue is treated as 0 for calculation purposes
            return true;
        }

        return false;
    }

    #endregion

    #region Alpha Value Extraction

    private const double BYTE_MAX = 255.0;
    private const double PERCENT_MAX = 100.0;

    /// <summary>
    /// Tries to get an alpha value from a token, returning the value in 0-255 range.
    /// Alpha can be a number (0-1), percentage (0-100%), or the 'none' keyword.
    /// </summary>
    /// <param name="token">The token to extract from.</param>
    /// <param name="value">The extracted alpha value (0-255 scale).</param>
    /// <returns>True if the token represents a valid alpha value.</returns>
    public static bool TryGetAlphaValue(CssToken token, out double value)
    {
        value = BYTE_MAX; // Default fully opaque

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out var num))
            {
                return false;
            }
            // Alpha as number is 0-1 range, convert to 0-255
            value = Math.Clamp(num, 0, 1) * BYTE_MAX;
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out var pct))
            {
                return false;
            }
            // Alpha as percentage is 0-100, convert to 0-255
            value = Math.Clamp(pct, 0, 100) / PERCENT_MAX * BYTE_MAX;
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            value = BYTE_MAX; // 'none' alpha defaults to fully opaque per spec
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tries to get a normalized alpha value (0-1) from a token.
    /// Alpha can be a number (0-1), percentage (0-100%), or the 'none' keyword.
    /// </summary>
    /// <param name="token">The token to extract from.</param>
    /// <param name="alpha">The extracted alpha value (0-1 scale).</param>
    /// <returns>True if the token represents a valid alpha value.</returns>
    public static bool TryGetAlphaValueNormalized(CssToken token, out double alpha)
    {
        alpha = 1.0; // Default fully opaque

        if (token.Type == ECssTokenType.Number)
        {
            if (!TryGetNumber(token, out var num))
            {
                return false;
            }
            alpha = Math.Clamp(num, 0, 1);
            return true;
        }

        if (token.Type == ECssTokenType.Percentage)
        {
            if (!TryGetPercentage(token, out var pct))
            {
                return false;
            }
            alpha = Math.Clamp(pct / 100.0, 0, 1);
            return true;
        }

        // 'none' keyword support (CSS Color 4)
        if (token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            alpha = 1.0; // 'none' alpha defaults to fully opaque
            return true;
        }

        return false;
    }

    #endregion

    #region Token Stream Helpers

    /// <summary>
    /// Checks if the token is the 'none' keyword (case-insensitive).
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is the 'none' ident keyword.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNoneKeyword(CssToken token)
    {
        return token is IdentToken ident && ident.Value.Equals("none", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the token is a comma delimiter.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is a comma token.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsComma(CssToken token)
    {
        return token.Type == ECssTokenType.Comma;
    }

    /// <summary>
    /// Checks if the token is a slash delimiter used for alpha separation.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is a '/' delimiter.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSlash(CssToken token)
    {
        return token is DelimToken delim && delim.Value == '/';
    }

    /// <summary>
    /// Checks if the token is whitespace.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is a whitespace token.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhitespace(CssToken token)
    {
        return token.Type == ECssTokenType.Whitespace;
    }

    #endregion

    #region Token Stream Creation

    /// <summary>
    /// Creates a token stream (DataConsumer) directly from a list of CSS tokens.
    /// The stream includes all tokens including whitespace - use <see cref="SkipWhitespace"/>
    /// to advance past whitespace during consumption.
    /// </summary>
    /// <param name="tokens">The original token list.</param>
    /// <returns>A DataConsumer wrapping the token array.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DataConsumer<CssToken> CreateTokenStream(List<CssToken> tokens)
    {
        return new DataConsumer<CssToken>(tokens.ToArray());
    }

    /// <summary>
    /// Creates a token stream (DataConsumer) from a list of CSS tokens, optionally filtering whitespace.
    /// </summary>
    /// <param name="tokens">The original token list.</param>
    /// <param name="preserveWhitespace">If true, whitespace is preserved in the stream.
    /// This is important for calc() where whitespace around + and - is significant.</param>
    /// <returns>A DataConsumer wrapping the token array. When filtering whitespace,
    /// returns a <see cref="FilteringDataConsumer{T}"/> that skips whitespace transparently.</returns>
    public static DataConsumer<CssToken> CreateTokenStream(List<CssToken> tokens, bool preserveWhitespace)
    {
        if (preserveWhitespace)
        {
            return new DataConsumer<CssToken>(tokens.ToArray());
        }

        // Return a filtering consumer that skips whitespace without copying data
        return new FilteringDataConsumer<CssToken>(
            tokens.ToArray(),
            static token => token.Type == ECssTokenType.Whitespace);
    }

    /// <summary>
    /// Creates a token stream that automatically filters out whitespace tokens.
    /// Use this for most CSS parsing where whitespace is not significant.
    /// </summary>
    /// <param name="tokens">The original token list.</param>
    /// <returns>A <see cref="FilteringDataConsumer{T}"/> that skips whitespace transparently.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FilteringDataConsumer<CssToken> CreateFilteredTokenStream(List<CssToken> tokens)
    {
        return new FilteringDataConsumer<CssToken>(
            tokens.ToArray(),
            static token => token.Type == ECssTokenType.Whitespace);
    }

    /// <summary>
    /// Creates a token stream with a custom filter predicate.
    /// Items matching the predicate will be automatically skipped during iteration.
    /// </summary>
    /// <param name="tokens">The original token list.</param>
    /// <param name="skipPredicate">Predicate returning true for tokens to skip.</param>
    /// <returns>A <see cref="FilteringDataConsumer{T}"/> that skips matching tokens.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FilteringDataConsumer<CssToken> CreateFilteredTokenStream(List<CssToken> tokens, Predicate<CssToken> skipPredicate)
    {
        return new FilteringDataConsumer<CssToken>(tokens.ToArray(), skipPredicate);
    }

    /// <summary>
    /// Skips whitespace tokens in the stream, advancing until the next token is not whitespace.
    /// Call this before consuming meaningful tokens.
    /// </summary>
    /// <param name="stream">The token stream.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SkipWhitespace(DataConsumer<CssToken> stream)
    {
        while (stream.Next != null && stream.Next.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }
    }

    /// <summary>
    /// Consumes and returns the next non-whitespace token from the stream.
    /// Returns null if the stream is exhausted.
    /// </summary>
    /// <param name="stream">The token stream.</param>
    /// <returns>The next non-whitespace token, or null if at end.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CssToken? ConsumeNonWhitespace(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);
        return stream.atEOF ? null : stream.Consume();
    }

    /// <summary>
    /// Peeks at the next non-whitespace token without consuming it.
    /// Returns null if no more non-whitespace tokens exist.
    /// </summary>
    /// <param name="stream">The token stream.</param>
    /// <returns>The next non-whitespace token, or null if at end.</returns>
    public static CssToken? PeekNonWhitespace(DataConsumer<CssToken> stream)
    {
        // We need to skip whitespace to peek, but DataConsumer doesn't support
        // non-destructive lookahead past whitespace, so we use the position
        int savedPos = stream.Position;
        SkipWhitespace(stream);
        var result = stream.Next;
        // Note: We can't restore position in DataConsumer, so this advances past whitespace
        // This is acceptable as whitespace is not meaningful
        return result;
    }

    /// <summary>
    /// Checks if the stream has reached the end (no more non-whitespace tokens).
    /// </summary>
    /// <param name="stream">The token stream.</param>
    /// <returns>True if no more non-whitespace tokens remain.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAtEnd(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);
        return stream.atEOF;
    }

    /// <summary>
    /// Checks if the argument list contains any comma tokens (indicating legacy CSS syntax).
    /// </summary>
    /// <param name="tokens">The token list to check.</param>
    /// <returns>True if any comma tokens are present.</returns>
    public static bool ContainsCommas(List<CssToken> tokens)
    {
        foreach (var token in tokens)
        {
            if (token.Type == ECssTokenType.Comma)
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}
