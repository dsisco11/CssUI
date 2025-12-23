using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using CssUI.Filters;
using static CssUI.UnicodeCommon;

namespace CssUI;

/// <summary>
/// Provides string manipulation utilities compliant with WHATWG Infra and HTML specifications.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why custom implementations instead of .NET string methods?</b>
/// </para>
/// <para>
/// The WHATWG and HTML specifications define specific string operations that differ from .NET:
/// </para>
/// <list type="bullet">
/// <item><description><b>ASCII Whitespace:</b> Specs define exactly 5 characters (TAB U+0009, LF U+000A,
/// FF U+000C, CR U+000D, SPACE U+0020). .NET's <c>char.IsWhiteSpace()</c> includes many more
/// Unicode whitespace characters.</description></item>
/// <item><description><b>Memory efficiency:</b> Operations work directly on <see cref="ReadOnlySpan{T}"/>
/// and <see cref="ReadOnlyMemory{T}"/> without allocating intermediate strings.</description></item>
/// <item><description><b>Filter/Predicate patterns:</b> Custom filter system for character matching
/// used throughout the DOM/CSS parsing infrastructure.</description></item>
/// </list>
/// <para>
/// See <see href="https://infra.spec.whatwg.org/#strings">WHATWG Infra §4</see> for specification details.
/// </para>
/// </remarks>
public static class StringCommon
{
    /// <summary>
    /// WHATWG-defined ASCII whitespace characters for spec-compliant operations.
    /// </summary>
    private static readonly SearchValues<char> AsciiWhitespace = SearchValues.Create("\t\n\f\r ");

    #region Checks

    /// <summary>
    /// Returns whether <paramref name="p"/> and <paramref name="q"/> contain the same values
    /// </summary>
    /// <returns>True if both strings are an exact match</returns>
    // public static bool StrEq(StringPtr p, StringPtr q) => p.Data.Span.Equals(q.Data.Span, StringComparison.Ordinal);

    /// <summary>
    /// Returns whether <paramref name="p"/> and <paramref name="q"/> contain the same (case-insensitive) values
    /// </summary>
    /// <returns>True if both strings are an case-insensitive match</returns>
    // public static bool StriEq(StringPtr p, StringPtr q) => p.Data.Span.Equals(q.Data.Span, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns whether <paramref name="p"/> and <paramref name="q"/> contain the same values
    /// </summary>
    /// <returns>True if both strings are an exact match</returns>
    public static bool StrEq(ReadOnlySpan<char> p, ReadOnlySpan<char> q) => p.Equals(q, StringComparison.Ordinal);

    /// <summary>
    /// Returns whether <paramref name="p"/> and <paramref name="q"/> contain the same (case-insensitive) values
    /// </summary>
    /// <returns>True if both strings are an case-insensitive match</returns>
    public static bool StriEq(ReadOnlySpan<char> p, ReadOnlySpan<char> q) => p.Equals(q, StringComparison.OrdinalIgnoreCase);
    #endregion

    #region Contains
    /// <summary>
    /// Returns whether <paramref name="Str"/> contains any characters matching the given filter
    /// </summary>
    /// <returns>True if string contains a character which the given filter matches</returns>
    public static bool Contains(ReadOnlySpan<char> Str, Filter<char> Filter)
    {
        if (Filter is null)
            return false;

        if (Str.Length == 0)
            return false;

        var Span = Str;
        for (int i = 0; i < Span.Length; i++)
        {
            var result = Filter.acceptData(Span[i]);
            if (result != EFilterResult.FILTER_ACCEPT)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns whether <paramref name="Str"/> contains any characters matching the given filter
    /// </summary>
    /// <returns>True if string contains a character which the given filter matches</returns>
    public static bool Contains(ReadOnlySpan<char> Str, Predicate<char> Predicate)
    {
        if (Predicate is null)
            return false;

        if (Str.Length == 0)
            return false;

        var Span = Str;//.Data.Span;
        for (int i = 0; i < Span.Length; i++)
        {
            if (Predicate(Span[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns whether <paramref name="Str"/> contains any characters matching the given filter
    /// </summary>
    /// <returns>True if string contains a character which the given filter matches</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(ReadOnlySpan<char> Str, char Search)
    {
        return Str.Contains(Search);
    }

    /// <summary>
    /// Returns whether <paramref name="Str"/> contains the specified substring
    /// </summary>
    /// <returns>True if string contains the search substring</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contains(ReadOnlySpan<char> Str, ReadOnlySpan<char> Search)
    {
        if (Search.IsEmpty)
            return false;

        return Str.IndexOf(Search) >= 0;
    }
    #endregion

    #region ContainsOnly
    /// <summary>
    /// Returns true if all characters in <paramref name="Str"/> are present in <paramref name="Search"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsOnly(ReadOnlySpan<char> Str, ReadOnlySpan<char> Search)
    {
        if (Str.IsEmpty || Search.IsEmpty)
            return false;

        // IndexOfAnyExcept returns -1 if all characters match the search set
        return Str.IndexOfAnyExcept(Search) < 0;
    }

    /// <summary>
    /// Returns true if all characters in <paramref name="Str"/> are present in <paramref name="searchValues"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsOnly(ReadOnlySpan<char> Str, SearchValues<char> searchValues)
    {
        if (Str.IsEmpty)
            return false;

        return Str.IndexOfAnyExcept(searchValues) < 0;
    }
    #endregion


    #region Count
    /// <summary>
    /// Returns the number of characters within <paramref name="Str"/> matching the given <paramref name="Filter"/>
    /// </summary>
    /// <returns>Number of matching characters</returns>
    public static int Count(ReadOnlySpan<char> Str, Filter<char> Filter)
    {
        if (Filter == null)
            return 0;

        if (Str.Length == 0)
            return 0;

        int RetVal = 0;
        var Span = Str;//.Data.Span;
        for (int i = 0; i < Span.Length; i++)
        {
            var result = Filter.acceptData(Span[i]);
            if (result == EFilterResult.FILTER_ACCEPT)
            {
                RetVal++;
            }
        }

        return RetVal;
    }

    /// <summary>
    /// Returns the number of characters within <paramref name="Str"/> matching the given <paramref name="Predicate"/>
    /// </summary>
    /// <returns>Number of matching characters</returns>
    public static int Count(ReadOnlySpan<char> Str, Predicate<char> Predicate)
    {
        if (Predicate == null)
            return 0;

        if (Str.Length == 0)
            return 0;


        int RetVal = 0;
        var Span = Str;//.Data.Span;
        for (int i = 0; i < Span.Length; i++)
        {
            if (Predicate(Span[i]))
            {
                RetVal++;
            }
        }

        return RetVal;
    }

    /// <summary>
    /// Returns the number of characters within <paramref name="Str"/> matching the given <paramref name="Search"/> character
    /// </summary>
    /// <returns>Number of matching characters</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Count(ReadOnlySpan<char> Str, char Search)
    {
        return Str.Count(Search);
    }
    #endregion


    #region Transformations
    static ValueTuple<char, ReadOnlyMemory<char>>[] WhitespaceReplacements = new ValueTuple<char, ReadOnlyMemory<char>>[]
    {
        (CHAR_TAB, " ".AsMemory()),
        (CHAR_LINE_FEED, " ".AsMemory()),
        (CHAR_FORM_FEED, " ".AsMemory()),
        (CHAR_CARRIAGE_RETURN, " ".AsMemory()),
        (CHAR_SPACE, " ".AsMemory()),
    };

    /// <summary>
    /// Strips leading/trailing ASCII whitespace and collapses runs of whitespace to single spaces.
    /// </summary>
    /// <param name="buffMem">The string memory to process.</param>
    /// <returns>A new string with normalized whitespace.</returns>
    /// <remarks>
    /// <para>
    /// Implements <see href="https://infra.spec.whatwg.org/#strip-and-collapse-ascii-whitespace">WHATWG Infra §4.6</see>.
    /// </para>
    /// <para>
    /// <b>Why not use .NET string methods?</b>
    /// </para>
    /// <para>
    /// .NET's <c>string.Trim()</c> and regex-based whitespace collapsing use Unicode whitespace
    /// definitions. The WHATWG spec explicitly requires only these 5 ASCII characters:
    /// </para>
    /// <list type="bullet">
    /// <item><description>U+0009 TAB</description></item>
    /// <item><description>U+000A LINE FEED (LF)</description></item>
    /// <item><description>U+000C FORM FEED (FF)</description></item>
    /// <item><description>U+000D CARRIAGE RETURN (CR)</description></item>
    /// <item><description>U+0020 SPACE</description></item>
    /// </list>
    /// </remarks>
    public static String Strip_And_Collapse_Whitespace(ReadOnlySpan<char> buffMem)
    {/* Docs: https://infra.spec.whatwg.org/#strip-and-collapse-ascii-whitespace */
        return Replace(buffMem, true, true, WhitespaceReplacements);
    }


    /// <summary>
    /// Transforms a <c>string</c> into a byte-array
    /// </summary>
    /// <returns>Byte-array containing the string data</returns>
    public static IReadOnlyList<byte> ToByteArray(this string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }
    #endregion


    #region Concatenation
    /// <summary>
    /// Concatenates an array of strings into a single string with each original string separated from the next by a given delimiter
    /// </summary>
    /// <param name="Delim">The delimiter(s) that should separate each token</param>
    /// <param name="Args">The strings to join</param>
    /// <returns></returns>
    public static String Concat(char Delim, IEnumerable<ReadOnlyMemory<char>> Args)
    {
        ArgumentNullException.ThrowIfNull(Args);
        Contract.EndContractBlock();

        // Materialize to list to avoid double enumeration
        var chunks = Args as IList<ReadOnlyMemory<char>> ?? Args.ToList();
        if (chunks.Count == 0) return string.Empty;

        int newLength = 0;
        for (int i = 0; i < chunks.Count; i++)
        {
            newLength += chunks[i].Length;
        }

        // Add delimiter space
        if (Delim != '\0')
        {
            newLength += chunks.Count - 1;
        }

        return string.Create(newLength, (chunks, Delim), static (span, state) =>
        {
            int pos = 0;
            for (int i = 0; i < state.chunks.Count; i++)
            {
                if (i > 0 && state.Delim != '\0')
                {
                    span[pos++] = state.Delim;
                }
                state.chunks[i].Span.CopyTo(span.Slice(pos));
                pos += state.chunks[i].Length;
            }
        });
    }

    /// <summary>
    /// Concatenates an array of strings into a single string with each original string separated from the next by a given delimiter
    /// </summary>
    /// <param name="Delim">The delimiter(s) that should separate each token</param>
    /// <param name="Args">The strings to join</param>
    /// <returns></returns>
    public static String Concat(char Delim, params ReadOnlyMemory<char>[] Args)
    {
        ArgumentNullException.ThrowIfNull(Args);
        Contract.EndContractBlock();
        if (Args.Length == 0) return string.Empty;

        int newLength = 0;
        for (int i = 0; i < Args.Length; i++)
        {
            newLength += Args[i].Length;
        }

        // Add delimiter space
        if (Delim != '\0')
        {
            newLength += Args.Length - 1;
        }

        return string.Create(newLength, (Args, Delim), static (span, state) =>
        {
            int pos = 0;
            for (int i = 0; i < state.Args.Length; i++)
            {
                if (i > 0 && state.Delim != '\0')
                {
                    span[pos++] = state.Delim;
                }
                state.Args[i].Span.CopyTo(span.Slice(pos));
                pos += state.Args[i].Length;
            }
        });
    }

    /// <summary>
    /// Concatenates an array of strings into a single string with each original string separated from the next by a given delimiter
    /// </summary>
    /// <param name="Delim">The delimiter(s) that should separate each token</param>
    /// <param name="Args">The strings to join</param>
    /// <returns></returns>
    public static String Concat(ReadOnlySpan<char> Delim, IEnumerable<ReadOnlyMemory<char>> Args)
    {
        ArgumentNullException.ThrowIfNull(Args);
        Contract.EndContractBlock();

        // Materialize to list to avoid double enumeration
        var chunks = Args as IList<ReadOnlyMemory<char>> ?? Args.ToList();
        if (chunks.Count == 0) return string.Empty;

        int newLength = 0;
        for (int i = 0; i < chunks.Count; i++)
        {
            newLength += chunks[i].Length;
        }

        // Add delimiter space
        var delimiterLength = Delim.Length;
        newLength += (chunks.Count - 1) * delimiterLength;

        // Must copy delimiter since ReadOnlySpan can't be captured in lambda
        var delimStr = Delim.ToString();

        return string.Create(newLength, (chunks, delimStr), static (span, state) =>
        {
            int pos = 0;
            for (int i = 0; i < state.chunks.Count; i++)
            {
                if (i > 0 && state.delimStr.Length > 0)
                {
                    state.delimStr.AsSpan().CopyTo(span.Slice(pos));
                    pos += state.delimStr.Length;
                }
                state.chunks[i].Span.CopyTo(span.Slice(pos));
                pos += state.chunks[i].Length;
            }
        });
    }

    /// <summary>
    /// Concatenates an array of strings into a single string with each original string separated from the next by a given delimiter
    /// </summary>
    /// <param name="Delim">The delimiter(s) that should separate each token</param>
    /// <param name="Args">The strings to join</param>
    /// <returns></returns>
    public static String Concat(ReadOnlySpan<char> Delim, params ReadOnlyMemory<char>[] Args)
    {
        ArgumentNullException.ThrowIfNull(Args);
        Contract.EndContractBlock();
        if (Args.Length == 0) return string.Empty;

        int newLength = 0;
        for (int i = 0; i < Args.Length; i++)
        {
            newLength += Args[i].Length;
        }

        // Add delimiter space
        var delimiterLength = Delim.Length;
        newLength += (Args.Length - 1) * delimiterLength;

        // Must copy delimiter since ReadOnlySpan can't be captured in lambda
        var delimStr = Delim.ToString();

        return string.Create(newLength, (Args, delimStr), static (span, state) =>
        {
            int pos = 0;
            for (int i = 0; i < state.Args.Length; i++)
            {
                if (i > 0 && state.delimStr.Length > 0)
                {
                    state.delimStr.AsSpan().CopyTo(span.Slice(pos));
                    pos += state.delimStr.Length;
                }
                state.Args[i].Span.CopyTo(span.Slice(pos));
                pos += state.Args[i].Length;
            }
        });
    }

    /* Delimitless concats */

    /// <summary>
    /// Concatenates an array of strings into a single string
    /// </summary>
    /// <param name="Args">The strings to join</param>
    /// <returns></returns>
    public static String Concat(IEnumerable<ReadOnlyMemory<char>> Args)
    {
        ArgumentNullException.ThrowIfNull(Args);
        Contract.EndContractBlock();

        // Materialize to list to avoid double enumeration
        var chunks = Args as IList<ReadOnlyMemory<char>> ?? Args.ToList();
        if (chunks.Count == 0) return string.Empty;

        int newLength = 0;
        for (int i = 0; i < chunks.Count; i++)
        {
            newLength += chunks[i].Length;
        }

        return string.Create(newLength, chunks, static (span, state) =>
        {
            int pos = 0;
            for (int i = 0; i < state.Count; i++)
            {
                state[i].Span.CopyTo(span.Slice(pos));
                pos += state[i].Length;
            }
        });
    }

    /// <summary>
    /// Concatenates an array of strings into a single string
    /// </summary>
    /// <param name="Args">The strings to join</param>
    /// <returns></returns>
    public static String Concat(params ReadOnlyMemory<char>[] Args)
    {
        ArgumentNullException.ThrowIfNull(Args);
        Contract.EndContractBlock();
        if (Args.Length == 0) return string.Empty;

        int newLength = 0;
        for (int i = 0; i < Args.Length; i++)
        {
            newLength += Args[i].Length;
        }

        return string.Create(newLength, Args, static (span, state) =>
        {
            int pos = 0;
            for (int i = 0; i < state.Length; i++)
            {
                state[i].Span.CopyTo(span.Slice(pos));
                pos += state[i].Length;
            }
        });
    }
    #endregion

    #region Trimming

    #region Trim
    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading or trailing instances of <paramref name="Delim"/> by offsetting its start and end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Delim">The character to trim out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, char Delim)
    {
        if (Input.Length == 0) return Input;

        int start = 0;
        int end = Input.Length;
        var span = Input.Span;

        /* Trim start */
        while (start < end && span[start] == Delim)
        {
            start++;
        }

        /* Trim end */
        while (end > start && span[end - 1] == Delim)
        {
            end--;
        }

        return Input.Slice(start, end - start);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading or trailing instances of <paramref name="Delim"/> by offsetting its start and end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Delims">The characters to trim out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, params char[] Delims)
    {
        if (Delims.Length == 0 || Input.Length == 0) return Input;

        int start = 0;
        int end = Input.Length;
        var span = Input.Span;

        /* Trim start */
        while (start < end && Delims.AsSpan().Contains(span[start]))
        {
            start++;
        }

        /* Trim end */
        while (end > start && Delims.AsSpan().Contains(span[end - 1]))
        {
            end--;
        }

        return Input.Slice(start, end - start);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading or trailing code points which do not match the given <paramref name="Filter"/> by offsetting its start and end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Filter">The filter used to trim characters out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, Filter<char> Filter)
    {
        if (Input.Length == 0) return Input;

        int start = 0;
        int end = Input.Length;
        var span = Input.Span;

        /* Trim start - skip chars that the filter SKIPS (they are delimiters) */
        while (start < end && Filter.acceptData(span[start]) == EFilterResult.FILTER_SKIP)
        {
            start++;
        }

        /* Trim end - skip chars that the filter SKIPS (they are delimiters) */
        while (end > start && Filter.acceptData(span[end - 1]) == EFilterResult.FILTER_SKIP)
        {
            end--;
        }

        return Input.Slice(start, end - start);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading or trailing code points which match the given <paramref name="Predicate"/> by offsetting its start and end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Predicate">The filter used to trim characters out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, Predicate<char> Predicate)
    {
        if (Input.Length == 0) return Input;

        int start = 0;
        int end = Input.Length;
        var span = Input.Span;

        /* Trim start - skip chars that match the predicate (they are delimiters) */
        while (start < end && Predicate(span[start]))
        {
            start++;
        }

        /* Trim end - skip chars that match the predicate (they are delimiters) */
        while (end > start && Predicate(span[end - 1]))
        {
            end--;
        }

        return Input.Slice(start, end - start);
    }
    #endregion

    #region Trim Start
    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading instances of <paramref name="Delim"/> by offsetting its start position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Delim">The character to trim out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, char Delim)
    {
        if (Input.Length == 0) return Input;

        int start = 0;
        var span = Input.Span;

        while (start < Input.Length && span[start] == Delim)
        {
            start++;
        }

        return Input.Slice(start);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading instances of <paramref name="Delims"/> by offsetting its start position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Delims">The characters to trim out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, params char[] Delims)
    {
        if (Delims.Length == 0 || Input.Length == 0) return Input;

        int start = 0;
        var span = Input.Span;

        while (start < Input.Length && Delims.AsSpan().Contains(span[start]))
        {
            start++;
        }

        return Input.Slice(start);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading code points which do not match the given <paramref name="Filter"/> by offsetting its start position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Filter">The filter used to trim characters out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, Filter<char> Filter)
    {
        if (Input.Length == 0) return Input;

        int start = 0;
        var span = Input.Span;

        while (start < Input.Length && Filter.acceptData(span[start]) == EFilterResult.FILTER_SKIP)
        {
            start++;
        }

        return Input.Slice(start);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any leading code points which match the given <paramref name="Predicate"/> by offsetting its start position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Predicate">The filter used to trim characters out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, Predicate<char> Predicate)
    {
        if (Input.Length == 0) return Input;

        int start = 0;
        var span = Input.Span;

        while (start < Input.Length && Predicate(span[start]))
        {
            start++;
        }

        return Input.Slice(start);
    }
    #endregion

    #region Trim End
    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any trailing instances of <paramref name="Delim"/> by offsetting its end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Delim">The character to trim out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, char Delim)
    {
        if (Input.Length == 0) return Input;

        int end = Input.Length;
        var span = Input.Span;

        while (end > 0 && span[end - 1] == Delim)
        {
            end--;
        }

        return Input.Slice(0, end);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any trailing instances of <paramref name="Delims"/> by offsetting its end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Delims">The characters to trim out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, params char[] Delims)
    {
        if (Delims.Length == 0 || Input.Length == 0) return Input;

        int end = Input.Length;
        var span = Input.Span;

        while (end > 0 && Delims.AsSpan().Contains(span[end - 1]))
        {
            end--;
        }

        return Input.Slice(0, end);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any trailing code points which do not match the given <paramref name="Filter"/> by offsetting its end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Filter">The filter used to trim characters out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, Filter<char> Filter)
    {
        if (Input.Length == 0) return Input;

        int end = Input.Length;
        var span = Input.Span;

        while (end > 0 && Filter.acceptData(span[end - 1]) == EFilterResult.FILTER_SKIP)
        {
            end--;
        }

        return Input.Slice(0, end);
    }

    /// <summary>
    /// Modifies the given <paramref name="Input"/>, removing any trailing code points which match the given <paramref name="Predicate"/> by offsetting its end position without modifying its data or creating a new string instance
    /// </summary>
    /// <param name="Input">The string memory to trim</param>
    /// <param name="Predicate">The filter used to trim characters out of the input</param>
    /// <returns></returns>
    public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, Predicate<char> Predicate)
    {
        if (Input.Length == 0) return Input;

        int end = Input.Length;
        var span = Input.Span;

        while (end > 0 && Predicate(span[end - 1]))
        {
            end--;
        }

        return Input.Slice(0, end);
    }
    #endregion
    #endregion

    #region Tokenization
    /// <summary>
    /// Splits a string <paramref name="Source"/> into tokens based on a given delimiter(s)
    /// </summary>
    /// <param name="Source">The string to tokenize</param>
    /// <param name="Delim">The delimiter(s) that should separate each token</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<char>[] Strtok(ReadOnlyMemory<char> Source, char Delim)
    {
        return Strtok(Source, new char[1] { Delim });
    }

    /// <summary>
    /// Splits a string <paramref name="Source"/> into tokens based on a given delimiter(s)
    /// </summary>
    /// <param name="Source">The string to tokenize</param>
    /// <param name="Delims">The delimiter(s) that should separate each token</param>
    /// <returns></returns>
    /// DO NOT INLINE THIS FUNCTION
    public static ReadOnlyMemory<char>[] Strtok(ReadOnlyMemory<char> Source, params char[] Delims)
    {
        if (Delims.Length == 0) throw new ArgumentException("Delimiters must be non-null and contain one or more characters");
        Contract.EndContractBlock();

        // Split the source string into chunks using the given delimiters
        var AllChunks = _chunkify(Source.Span, true, Delims);

        // Count non-delimiter chunks first
        int count = 0;
        for (int i = 0; i < AllChunks.Count; i++)
        {
            if (!AllChunks[i].IsDelimiter) count++;
        }

        // Compile the return list of memory segments
        var RetVal = new ReadOnlyMemory<char>[count];
        int idx = 0;
        for (int i = 0; i < AllChunks.Count; i++)
        {
            var chunk = AllChunks[i];
            if (!chunk.IsDelimiter)
            {
                RetVal[idx++] = Source.Slice(chunk.Start, chunk.Size);
            }
        }

        return RetVal;
    }

    /// <summary>
    /// Splits a string <paramref name="Source"/> into tokens based on a given delimiter(s)
    /// </summary>
    /// <param name="Source">The string to tokenize</param>
    /// <param name="Filter">The delimiter(s) that should separate each token</param>
    /// <returns></returns>
    /// /// DO NOT INLINE THIS FUNCTION
    public static ReadOnlyMemory<char>[] Strtok(ReadOnlyMemory<char> Source, Filter<char>? Filter = null)
    {
        ArgumentNullException.ThrowIfNull(Filter);
        Contract.EndContractBlock();

        // Split the source string into chunks using the given delimiters
        var AllChunks = _chunkify(Source.Span, true, Filter);

        // Count non-delimiter chunks first
        int count = 0;
        for (int i = 0; i < AllChunks.Count; i++)
        {
            if (!AllChunks[i].IsDelimiter) count++;
        }

        // Compile the return list of memory segments
        var RetVal = new ReadOnlyMemory<char>[count];
        int idx = 0;
        for (int i = 0; i < AllChunks.Count; i++)
        {
            var chunk = AllChunks[i];
            if (!chunk.IsDelimiter)
            {
                RetVal[idx++] = Source.Slice(chunk.Start, chunk.Size);
            }
        }

        return RetVal;
    }
    #endregion


    #region Mutation

    /// <summary>
    /// Replaces all characters matching the filter with the replacement string.
    /// This is a convenience overload for the more complex Replace methods.
    /// </summary>
    /// <param name="Source">Target string as ReadOnlyMemory</param>
    /// <param name="Filter">Filter to match characters that should be replaced</param>
    /// <param name="Replacement">String to replace matched characters with</param>
    /// <returns>Altered string</returns>
    public static string Replace(ReadOnlyMemory<char> Source, Filter<char> Filter, string Replacement)
    {
        return Replace(Source.Span, false, false, (Filter, Replacement.AsMemory()));
    }

    /// <summary>
    /// Replaces all characters matching the filter with the replacement string.
    /// </summary>
    /// <param name="Source">Target string as ReadOnlySpan</param>
    /// <param name="Filter">Filter to match characters that should be replaced</param>
    /// <param name="Replacement">String to replace matched characters with</param>
    /// <returns>Altered string</returns>
    public static string Replace(ReadOnlySpan<char> Source, Filter<char> Filter, string Replacement)
    {
        return Replace(Source, false, false, (Filter, Replacement.AsMemory()));
    }

    /// <summary>
    /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
    /// </summary>
    /// <param name="Source">Target string</param>
    /// <param name="Trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
    /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
    /// <returns>Altered string</returns>
    public static string Replace(ReadOnlySpan<char> Source, bool Trim = false, bool Collapse = false, params ValueTuple<char, ReadOnlyMemory<char>>[] Replacements)
    {
        if (Source.IsEmpty) return string.Empty;
        if (Replacements.Length <= 0) return Source.ToString();
        Contract.EndContractBlock();

        // Prepare the arrays needed for the generic chunking functions
        char[] Delimiters = Replacements.Select(o => o.Item1).ToArray();
        ReadOnlyMemory<char>[] Substitutions = Replacements.Select(o => o.Item2).ToArray();

        // Separate the source memory into chunks using the given predicates
        var Chunks = _chunkify(Source, Collapse, Delimiters);
        // Calculate size of the new string
        int newLength = _tally_chunks(Chunks, Trim, Substitutions);

        if (newLength == 0) return string.Empty;

        // Use ArrayPool for the buffer
        char[] rentedBuffer = ArrayPool<char>.Shared.Rent(newLength);
        try
        {
            _compile_chunks(Source, Chunks, rentedBuffer.AsSpan(0, newLength), Substitutions);
            return new string(rentedBuffer, 0, newLength);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }
    }

    /// <summary>
    /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
    /// </summary>
    /// <param name="Source">Target string</param>
    /// <param name="Trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
    /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
    /// <returns>Altered string</returns>
    public static string Replace(ReadOnlySpan<char> Source, bool Trim = false, bool Collapse = false, params ValueTuple<Predicate<char>, ReadOnlyMemory<char>>[] Replacements)
    {
        if (Source.IsEmpty) return string.Empty;
        if (Replacements.Length <= 0) return Source.ToString();
        Contract.EndContractBlock();

        // Prepare the arrays needed for the generic chunking functions
        Predicate<char>[] Predicates = Replacements.Select(o => o.Item1).ToArray();
        ReadOnlyMemory<char>[] Substitutions = Replacements.Select(o => o.Item2).ToArray();

        // Separate the source memory into chunks using the given predicates
        var Chunks = _chunkify(Source, Collapse, Predicates);
        // Calculate size of the new string
        int newLength = _tally_chunks(Chunks, Trim, Substitutions);

        if (newLength == 0) return string.Empty;

        // Use ArrayPool for the buffer
        char[] rentedBuffer = ArrayPool<char>.Shared.Rent(newLength);
        try
        {
            _compile_chunks(Source, Chunks, rentedBuffer.AsSpan(0, newLength), Substitutions);
            return new string(rentedBuffer, 0, newLength);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }
    }

    /// <summary>
    /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
    /// </summary>
    /// <param name="Source">Target string</param>
    /// <param name="Trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
    /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
    /// <returns>Altered string</returns>
    public static string Replace(ReadOnlySpan<char> Source, bool Trim = false, bool Collapse = false, params ValueTuple<Filter<char>, ReadOnlyMemory<char>>[] Replacements)
    {
        if (Source.IsEmpty) return string.Empty;
        if (Replacements.Length <= 0) return Source.ToString();
        Contract.EndContractBlock();

        // Prepare the arrays needed for the generic chunking functions
        Filter<char>[] Filters = Replacements.Select(o => o.Item1).ToArray();
        ReadOnlyMemory<char>[] Substitutions = Replacements.Select(o => o.Item2).ToArray();

        // Separate the source memory into chunks using the given predicates
        var Chunks = _chunkify(Source, Collapse, Filters);
        // Calculate size of the new string
        int newLength = _tally_chunks(Chunks, Trim, Substitutions);

        if (newLength == 0) return string.Empty;

        // Use ArrayPool for the buffer
        char[] rentedBuffer = ArrayPool<char>.Shared.Rent(newLength);
        try
        {
            _compile_chunks(Source, Chunks, rentedBuffer.AsSpan(0, newLength), Substitutions);
            return new string(rentedBuffer, 0, newLength);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }
    }


    /// <summary>
    /// Runs a transform function on every character in a string and returns a new string containing the transformed characters
    /// </summary>
    /// <param name="buffMem">String memory</param>
    /// <param name="Transform">Function that takes in a char and outputs one that should go into the new string. Returning a null char will cause that char to be omitted from the returned string</param>
    /// <returns>Altered string</returns>
    public static string Transform(ReadOnlyMemory<char> buffMem, Func<char, char> Transform)
    {
        return StringCommon.Transform(buffMem.Span, Transform);
    }

    /// <summary>
    /// Runs a transform function on every character in a string and returns a new string containing the transformed characters
    /// </summary>
    /// <param name="buffMem">String memory</param>
    /// <param name="Transform">Function that takes in a char and outputs one that should go into the new string. Returning a null char will cause that char to be omitted from the returned string</param>
    /// <returns>Altered string</returns>
    public static string Transform(ReadOnlySpan<char> buffMem, Func<char, char> Transform)
    {
        if (buffMem.IsEmpty) return string.Empty;

        int Length = buffMem.Length;
        char[] rentedBuffer = ArrayPool<char>.Shared.Rent(Length);
        try
        {
            int idx = 0;
            for (int i = 0; i < Length; i++)
            {
                char ch = Transform(buffMem[i]);
                if (ch != CHAR_NULL)
                {
                    rentedBuffer[idx++] = ch;
                }
            }

            return new string(rentedBuffer, 0, idx);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedBuffer);
        }
    }
    #endregion


    #region Scan
    /// <summary>
    /// Searches the given string and returns the next location where its value matches the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Match(ReadOnlySpan<char> Source, char[] Match, int Offset, out int MatchedIndex)
    {
        if (Offset > 0) Source = Source.Slice(Offset);
        var idx = Source.IndexOfAny(Match);
        if (idx > -1)
        {
            if (Match.Length <= 1)
                MatchedIndex = 0;
            else
                MatchedIndex = new Span<char>(Match).IndexOf(Source[idx]);

            idx += Offset;
        }
        else
        {
            MatchedIndex = -1;
        }

        return idx;
    }

    /// <summary>
    /// Searches the given string and returns the next location where its value doesn't match the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Mismatch(ReadOnlySpan<char> Source, char[] Match, int Offset)
    {
        if (Offset > 0) Source = Source.Slice(Offset);
        if (Source.IndexOfAny(Match) == 0)
        {// Scan forwards to the end of the block of matching chars at the start
            ReadOnlySpan<char> ptr = Source.Slice(1);
            // XXX: Optimize
            // This method is probably pretty slow comparatively
            int i = 1;
            for (; ptr.Length > 0 && ptr.IndexOfAny(Match) == 0; i++)
            {
                ptr = ptr.Slice(1);
            }
            // This is the end of the matching block at the start of this string, and therefore is the index of the next mismatch
            if (i > -1) i += Offset;
            return i;
        }

        var idx = Source.IndexOfAny(Match);
        if (idx > -1) idx += Offset;
        return idx;
    }


    /// <summary>
    /// Searches the given string and returns the next location where its value matches the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Match(ReadOnlySpan<char> Source, Predicate<char> Predicate, int Offset)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            if (true == Predicate(Source[Pos]))
            {
                return Pos;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches the given string and returns the next location where its value doesn't match the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Mismatch(ReadOnlySpan<char> Source, Predicate<char> Predicate, int Offset)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            if (false == Predicate(Source[Pos]))
            {
                return Pos;
            }
        }

        return -1;
    }


    /// <summary>
    /// Searches the given string and returns the next location where its value matches the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Match(ReadOnlySpan<char> Source, Predicate<char>[] Predicates, int Offset, out int MatchedIndex)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            for (int i = 0; i < Predicates.Length; i++)
            {
                if (true == Predicates[i](Source[Pos]))
                {
                    MatchedIndex = i;
                    return Pos;
                }
            }
        }

        MatchedIndex = -1;
        return -1;
    }

    /// <summary>
    /// Searches the given string and returns the next location where its value doesn't match the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Mismatch(ReadOnlySpan<char> Source, Predicate<char>[] Predicates, int Offset)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            bool Matched = false;
            for (int i = 0; i < Predicates.Length; i++)
            {
                if (false == Predicates[i](Source[Pos]))
                {
                    Matched = true;
                    break;
                }
            }

            if (!Matched)
                continue;

            return Pos;
        }

        return -1;
    }



    /// <summary>
    /// Searches the given string and returns the next location where its value matches the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Match(ReadOnlySpan<char> Source, Filter<char> Filter, int Offset)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            if (Filter.acceptData(Source[Pos]) != EFilterResult.FILTER_ACCEPT)
            {
                return Pos;
            }
        }

        return -1;
    }

    /// <summary>
    /// Searches the given string and returns the next location where its value doesn't match the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Mismatch(ReadOnlySpan<char> Source, Filter<char> Filter, int Offset)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            if (Filter.acceptData(Source[Pos]) == EFilterResult.FILTER_ACCEPT)
            {
                return Pos;
            }
        }

        return -1;
    }


    /// <summary>
    /// Searches the given string and returns the next location where its value matches the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Match(ReadOnlySpan<char> Source, Filter<char>[] Filters, int Offset, out int MatchedIndex)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            for (int i = 0; i < Filters.Length; i++)
            {
                if (Filters[i].acceptData(Source[Pos]) != EFilterResult.FILTER_ACCEPT)// DataFilters accept anything that ISNT their subject
                {
                    MatchedIndex = i;
                    return Pos;
                }
            }
        }

        MatchedIndex = -1;
        return -1;
    }

    /// <summary>
    /// Searches the given string and returns the next location where its value doesn't match the search value
    /// </summary>
    /// <param name="Offset">Offset to begin searching from</param>
    /// <returns>-1 on failure</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Scan_Mismatch(ReadOnlySpan<char> Source, Filter<char>[] Filters, int Offset)
    {
        for (int Pos = Offset; Pos < Source.Length; Pos++)
        {
            bool Matched = false;
            for (int i = 0; i < Filters.Length; i++)
            {
                if (Filters[i].acceptData(Source[Pos]) == EFilterResult.FILTER_ACCEPT)// DataFilters accept anything that ISNT their subject
                {
                    Matched = true;
                    break;
                }
            }

            if (!Matched)
                continue;

            return Pos;
        }

        return -1;
    }
    #endregion

    #region Internal Chunking
    private struct StringChunk
    {
        public readonly int Start;
        public readonly int Size;
        public readonly int DelimiterIndex;
        public bool IsDelimiter => (DelimiterIndex > -1);

        public StringChunk(int start_offset, int size)
        {
            Start = start_offset;
            Size = size;
            DelimiterIndex = -1;
        }

        public StringChunk(int start_offset, int size, int delimiter_index)
        {
            Start = start_offset;
            Size = size;
            DelimiterIndex = delimiter_index;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, params char[] Delimiters)
    {
        var Chunks = new List<StringChunk>();

        // Scan for replacement characters, when encountered create a new chunk(non inclusive).
        for (int chunkStart = 0; chunkStart < Source.Length;)
        {
            var lastChunkEnd = Math.Max(0, chunkStart);
            // Find the chunk starting pos by finding the next occurance of any of the replacement targets
            chunkStart = Scan_Match(Source, Delimiters, chunkStart, out int DelimiterIndex);

            // If no more replacements can be found then abort
            if (chunkStart < 0)
            {
                int tailLen = Source.Length - lastChunkEnd;
                if (tailLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, tailLen));
                }
                break;
            }
            else
            {
                // Add room for the non-replaced contents inbetween this chunk and the last
                var interChunkLen = chunkStart - lastChunkEnd;
                if (interChunkLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, interChunkLen));
                }
            }

            Debug.Assert(DelimiterIndex > -1);
            // Find the length of the chunk by locating the next spot which doesn't match any replacements
            int chunkEnd = chunkStart + 1;
            if (Collapse)
            {// In order to collapse we have to consume all consecutive replacements
                chunkEnd = Scan_Mismatch(Source, Delimiters, chunkStart);
                if (chunkEnd < 0)
                {// If no character mismatch could be found then the rest of the string consists of matches
                    chunkEnd = Source.Length;
                }
            }

            var chunkSize = chunkEnd - chunkStart;
            // Push new chunk
            Chunks.Add(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
            // Move our read position forward for the next pass
            chunkStart = chunkEnd;
        }

        return Chunks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, params Predicate<char>[] Predicates)
    {
        var Chunks = new List<StringChunk>();

        // Scan for replacement characters, when encountered create a new chunk(non inclusive).
        for (int chunkStart = 0; chunkStart < Source.Length;)
        {
            var lastChunkEnd = Math.Max(0, chunkStart);
            // Find the chunk starting pos by finding the next occurance of any of the replacement targets
            chunkStart = Scan_Match(Source, Predicates, chunkStart, out int DelimiterIndex);

            // If no more replacements can be found then abort
            if (chunkStart < 0)
            {
                int tailLen = Source.Length - lastChunkEnd;
                if (tailLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, tailLen));
                }
                break;
            }
            else
            {
                // Add room for the non-replaced contents inbetween this chunk and the last
                var interChunkLen = chunkStart - lastChunkEnd;
                if (interChunkLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, interChunkLen));
                }
            }

            Debug.Assert(DelimiterIndex > -1);
            // Find the length of the chunk by locating the next spot which doesn't match any replacements
            int chunkEnd = chunkStart + 1;
            if (Collapse)
            {// In order to collapse we have to consume all consecutive replacements
                chunkEnd = Scan_Mismatch(Source, Predicates, chunkStart);
                if (chunkEnd < 0)
                {// If no character mismatch could be found then the rest of the string consists of matches
                    chunkEnd = Source.Length;
                }
            }

            var chunkSize = chunkEnd - chunkStart;
            // Push new chunk
            Chunks.Add(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
            // Move our read position forward for the next pass
            chunkStart = chunkEnd;
        }

        return Chunks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, Filter<char> Filter)
    {
        var Chunks = new List<StringChunk>();
        const int DelimiterIndex = 0;// We just use this here to keep this generic code the same

        // Scan for replacement characters, when encountered create a new chunk(non inclusive).
        for (int chunkStart = 0; chunkStart < Source.Length;)
        {
            var lastChunkEnd = Math.Max(0, chunkStart);
            // Find the chunk starting pos by finding the next occurance of any of the replacement targets
            chunkStart = Scan_Match(Source, Filter, chunkStart);

            // If no more replacements can be found then abort
            if (chunkStart < 0)
            {
                int tailLen = Source.Length - lastChunkEnd;
                if (tailLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, tailLen));
                }
                break;
            }
            else
            {
                // Add room for the non-replaced contents inbetween this chunk and the last
                var interChunkLen = chunkStart - lastChunkEnd;
                if (interChunkLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, interChunkLen));
                }
            }

            Debug.Assert(DelimiterIndex > -1);
            // Find the length of the chunk by locating the next spot which doesn't match any replacements
            int chunkEnd = chunkStart + 1;
            if (Collapse)
            {// In order to collapse we have to consume all consecutive replacements
                chunkEnd = Scan_Mismatch(Source, Filter, chunkStart);
                if (chunkEnd < 0)
                {// If no character mismatch could be found then the rest of the string consists of matches
                    chunkEnd = Source.Length;
                }
            }

            var chunkSize = chunkEnd - chunkStart;
            // Push new chunk
            Chunks.Add(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
            // Move our read position forward for the next pass
            chunkStart = chunkEnd;
        }

        return Chunks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static List<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, params Filter<char>[] Delimiters)
    {
        var Chunks = new List<StringChunk>();

        // Scan for replacement characters, when encountered create a new chunk(non inclusive).
        for (int chunkStart = 0; chunkStart < Source.Length;)
        {
            var lastChunkEnd = Math.Max(0, chunkStart);
            // Find the chunk starting pos by finding the next occurance of any of the replacement targets
            chunkStart = Scan_Match(Source, Delimiters, chunkStart, out int DelimiterIndex);

            // If no more replacements can be found then abort
            if (chunkStart < 0)
            {
                int tailLen = Source.Length - lastChunkEnd;
                if (tailLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, tailLen));
                }
                break;
            }
            else
            {
                // Add room for the non-replaced contents inbetween this chunk and the last
                var interChunkLen = chunkStart - lastChunkEnd;
                if (interChunkLen > 0)
                {
                    Chunks.Add(new StringChunk(lastChunkEnd, interChunkLen));
                }
            }

            Debug.Assert(DelimiterIndex > -1);
            // Find the length of the chunk by locating the next spot which doesn't match any replacements
            int chunkEnd = chunkStart + 1;
            if (Collapse)
            {// In order to collapse we have to consume all consecutive replacements
                chunkEnd = Scan_Mismatch(Source, Delimiters, chunkStart);
                if (chunkEnd < 0)
                {// If no character mismatch could be found then the rest of the string consists of matches
                    chunkEnd = Source.Length;
                }
            }

            var chunkSize = chunkEnd - chunkStart;
            // Push new chunk
            Chunks.Add(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
            // Move our read position forward for the next pass
            chunkStart = chunkEnd;
        }

        return Chunks;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int _tally_chunks(List<StringChunk> Chunks, bool Trim, ReadOnlyMemory<char>[] Substitutions)
    {
        int Length = 0;

        // Trim leading delimiter chunks
        while (Trim && Chunks.Count > 0 && Chunks[0].IsDelimiter)
        {
            Chunks.RemoveAt(0);
        }

        // Trim trailing delimiter chunks
        while (Trim && Chunks.Count > 0 && Chunks[^1].IsDelimiter)
        {
            Chunks.RemoveAt(Chunks.Count - 1);
        }

        // Calculate total length
        for (int i = 0; i < Chunks.Count; i++)
        {
            StringChunk chunk = Chunks[i];
            if (chunk.IsDelimiter)
            {
                Length += Substitutions[chunk.DelimiterIndex].Length;
            }
            else
            {
                Length += chunk.Size;
            }
        }

        return Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void _compile_chunks(ReadOnlySpan<char> Source, List<StringChunk> Chunks, Span<char> Buffer, ReadOnlyMemory<char>[] Substitutions)
    {
        int writePos = 0;

        for (int i = 0; i < Chunks.Count; i++)
        {
            StringChunk Chunk = Chunks[i];
            int chunkEnd = Chunk.Start + Chunk.Size;

            if (Chunk.IsDelimiter)
            {
                if (Chunk.DelimiterIndex >= 0)
                {
                    var Substitution = Substitutions[Chunk.DelimiterIndex];
                    if (Substitution.Length > 0)
                    {
                        // Insert substitute
                        Substitution.Span.CopyTo(Buffer.Slice(writePos));
                        writePos += Substitution.Length;
                    }
                }
            }
            else
            {
                // Insert this chunks data directly into new string unmodified
                Debug.Assert(chunkEnd <= Source.Length);

                if (Chunk.Start < Source.Length && Chunk.Size > 0)
                {
                    Source.Slice(Chunk.Start, Chunk.Size).CopyTo(Buffer.Slice(writePos));
                    writePos += Chunk.Size;
                }
            }
        }
    }
    #endregion
}

