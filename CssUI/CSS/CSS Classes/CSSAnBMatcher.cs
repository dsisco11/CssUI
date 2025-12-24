using System;
using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Handles An+B syntax matching for CSS pseudo-classes like :nth-child().
/// </summary>
/// <remarks>
/// <para>
/// This class wraps the <see cref="CssAnB"/> struct and <see cref="CssAnBParser"/> 
/// for backward compatibility with existing code.
/// </para>
/// <para>
/// For new code, prefer using <see cref="CssAnB"/> and <see cref="CssAnBParser"/> directly.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#anb-microsyntax"/>
public class CssAnBMatcher
{
    /// <summary>
    /// The underlying An+B value.
    /// </summary>
    private readonly CssAnB _anb;

    /// <summary>
    /// Gets the step value (coefficient of n).
    /// </summary>
    public int A => _anb.A;

    /// <summary>
    /// Gets the offset value.
    /// </summary>
    public int B => _anb.B;

    #region Constructors
    /// <summary>
    /// Creates a new An+B matcher with the specified values.
    /// </summary>
    /// <param name="A">The step value (coefficient of n).</param>
    /// <param name="B">The offset value.</param>
    public CssAnBMatcher(int A, int B)
    {
        _anb = new CssAnB(A, B);
    }

    /// <summary>
    /// Creates a new An+B matcher from an existing <see cref="CssAnB"/> value.
    /// </summary>
    /// <param name="anb">The An+B value.</param>
    public CssAnBMatcher(CssAnB anb)
    {
        _anb = anb;
    }

    /// <summary>
    /// Consumes and returns an An+B token from a <see cref="DataConsumer{T}"/> token stream.
    /// </summary>
    /// <param name="Stream">The token stream to consume from.</param>
    /// <returns>A new <see cref="CssAnBMatcher"/> representing the parsed An+B value.</returns>
    /// <exception cref="CssSyntaxErrorException">Thrown when the input is not a valid An+B syntax.</exception>
    public static CssAnBMatcher Consume(DataConsumer<CssToken> Stream)
    {
        if (CssAnBParser.TryParse(Stream, out var result))
        {
            return new CssAnBMatcher(result.Value);
        }

        throw new CssSyntaxErrorException("Invalid An+B syntax");
    }

    /// <summary>
    /// Attempts to consume an An+B value from a token stream.
    /// </summary>
    /// <param name="Stream">The token stream to consume from.</param>
    /// <param name="matcher">When this method returns, contains the parsed matcher, if successful.</param>
    /// <returns>True if the An+B value was successfully parsed; otherwise, false.</returns>
    public static bool TryConsume(DataConsumer<CssToken> Stream, out CssAnBMatcher? matcher)
    {
        if (CssAnBParser.TryParse(Stream, out var result))
        {
            matcher = new CssAnBMatcher(result.Value);
            return true;
        }

        matcher = null;
        return false;
    }

    /// <summary>
    /// Parses an An+B value from a string.
    /// </summary>
    /// <param name="input">The CSS string to parse.</param>
    /// <returns>A new <see cref="CssAnBMatcher"/> representing the parsed An+B value.</returns>
    /// <exception cref="CssSyntaxErrorException">Thrown when the input is not a valid An+B syntax.</exception>
    public static CssAnBMatcher Parse(string input)
    {
        return new CssAnBMatcher(CssAnBParser.Parse(input));
    }

    /// <summary>
    /// Attempts to parse an An+B value from a string.
    /// </summary>
    /// <param name="input">The CSS string to parse.</param>
    /// <param name="matcher">When this method returns, contains the parsed matcher, if successful.</param>
    /// <returns>True if the An+B value was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(string input, out CssAnBMatcher? matcher)
    {
        if (CssAnBParser.TryParse(input, out var result))
        {
            matcher = new CssAnBMatcher(result.Value);
            return true;
        }

        matcher = null;
        return false;
    }
    #endregion

    /// <summary>
    /// Checks if a given 1-based index matches this An+B pattern.
    /// </summary>
    /// <param name="index">The 1-based index to check.</param>
    /// <returns>True if the index matches the An+B pattern; otherwise, false.</returns>
    /// <remarks>
    /// The first element in a list has index 1 (not 0).
    /// </remarks>
    public bool Match(int index)
    {
        return _anb.Matches(index);
    }

    /// <summary>
    /// Gets the canonical CSS string representation of this An+B value.
    /// </summary>
    /// <returns>The serialized An+B string.</returns>
    public override string ToString()
    {
        return _anb.ToString();
    }
}

