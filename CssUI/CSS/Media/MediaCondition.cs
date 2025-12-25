using System;
using System.Collections.Generic;
using CssUI.DOM;

namespace CssUI.CSS.Media;

/// <summary>
/// Represents a compound media condition combining multiple sub-conditions with a logical operator.
/// </summary>
/// <remarks>
/// Media conditions can combine sub-conditions using AND, OR, or NOT combinators.
/// </remarks>
/// <seealso href="https://www.w3.org/TR/mediaqueries-4/#media-condition"/>
public class MediaCondition : IMediaCondition
{
    #region Properties
    private readonly LinkedList<IMediaCondition> Conditions;
    private readonly EMediaCombinator Op;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new compound media condition.
    /// </summary>
    /// <param name="op">The logical combinator (AND, OR, NOT).</param>
    /// <param name="conditions">The sub-conditions to combine.</param>
    public MediaCondition(EMediaCombinator op, IEnumerable<IMediaCondition> conditions)
    {
        Conditions = new LinkedList<IMediaCondition>(conditions);
        Op = op;
    }
    #endregion

    /// <inheritdoc/>
    public bool Matches(Document document)
    {
        bool matches = true;
        if (Op == EMediaCombinator.OR) matches = false;

        foreach (IMediaCondition condition in Conditions)
        {
            if (condition.Matches(document))
            {
                if (Op == EMediaCombinator.NOT)
                {
                    return false;
                }
                else if (Op == EMediaCombinator.OR)
                {
                    matches = true;
                    return true;
                }
            }
            else
            {
                if (Op == EMediaCombinator.AND)
                {
                    matches = false;
                }
            }
        }

        return matches;
    }

    #region Formatting (ISpanFormattable)

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        Span<char> buffer = stackalloc char[1024];
        if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
        {
            return buffer[..charsWritten].ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// Tries to format the media condition into the provided span.
    /// </summary>
    /// <param name="destination">The span to write to.</param>
    /// <param name="charsWritten">The number of characters written.</param>
    /// <param name="format">The format string (ignored).</param>
    /// <param name="provider">The format provider (ignored).</param>
    /// <returns><c>true</c> if formatting succeeded; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Serialization follows CSS Media Queries Level 4 §3.
    /// Format: (condition1 and/or condition2 ...)
    /// </remarks>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        charsWritten = 0;

        if (Conditions.Count <= 0)
        {
            return true; // Empty string is valid for empty conditions
        }

        int pos = 0;

        // Opening parenthesis
        if (destination.Length < 1) return false;
        destination[pos++] = UnicodeCommon.CHAR_LEFT_PARENTHESES;

        bool first = true;
        string? keyword = Op.Keyword();

        foreach (IMediaCondition condition in Conditions)
        {
            if (!first)
            {
                // Space + keyword + space
                int separatorLen = 2 + (keyword?.Length ?? 0);
                if (destination.Length < pos + separatorLen) return false;

                destination[pos++] = UnicodeCommon.CHAR_SPACE;
                if (keyword != null)
                {
                    keyword.AsSpan().CopyTo(destination[pos..]);
                    pos += keyword.Length;
                }
                destination[pos++] = UnicodeCommon.CHAR_SPACE;
            }

            if (!condition.TryFormat(destination[pos..], out int conditionWritten, default, provider))
                return false;
            pos += conditionWritten;

            first = false;
        }

        // Closing parenthesis
        if (destination.Length <= pos) return false;
        destination[pos++] = UnicodeCommon.CHAR_RIGHT_PARENTHESES;

        charsWritten = pos;
        return true;
    }

    #endregion
}

