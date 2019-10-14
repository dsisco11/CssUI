using CssUI.DOM;
using CssUI.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using static CssUI.UnicodeCommon;

namespace CssUI
{
    public static class StringCommon
    {

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
        public static bool Contains(ReadOnlySpan<char> Str, DataFilter<char> Filter)
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
        public static bool Contains(ReadOnlySpan<char> Str, char Search)
        {
            if (Str.Length == 0)
                return false;

            var Span = Str;//.Data.Span;
            for (int i = 0; i < Span.Length; i++)
            {
                if (Span[i] == Search)
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
        public static bool Contains(ReadOnlySpan<char> Str, ReadOnlySpan<char> Search)
        {
            if (Str.Length == 0)
                return false;

            if (Search.Length == 0)
                return false;

            var Span = Str;//.Data.Span;
            var SearchSpan = Search;//.Data.Span;
            for (int i = 0; i < Span.Length; i++)
            {
                if (Span[i] == SearchSpan[0])
                {
                    bool bMismatch = false;
                    i++;
                    for (int j = 1; j < SearchSpan.Length; j++, i++)
                    {
                        if (Span[i] != SearchSpan[j])
                        {
                            bMismatch = true;
                            break;
                        }
                    }

                    if (bMismatch)
                    {
                        continue;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region ContainsOnly
        /// <summary>
        /// Returns the index of the first character which matches none of the <paramref name="Search"/> characters
        /// </summary>
        public static bool ContainsOnly(ReadOnlySpan<char> Str, ReadOnlySpan<char> Search)
        {
            if (Str.Length == 0)
                return false;

            if (Search.Length == 0)
                return false;

            var Span = Str;
            for (int i = 0; i < Span.Length; i++)
            {
                if (Search.IndexOf(Span[i]) < 0)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion


        #region Count
        /// <summary>
        /// Returns the number of characters within <paramref name="Str"/> matching the given <paramref name="Filter"/>
        /// </summary>
        /// <returns>Number of matching characters</returns>
        public static int Count(ReadOnlySpan<char> Str, DataFilter<char> Filter)
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
        public static int Count(ReadOnlySpan<char> Str, char Search)
        {
            if (Str.Length == 0)
                return 0;


            int RetVal = 0;
            var Span = Str;//.Data.Span;
            for (int i = 0; i < Span.Length; i++)
            {
                char c = Span[i];
                if (c.Equals(Search))
                {
                    RetVal++;
                }
            }

            return RetVal;
        }
        #endregion


        #region Transformations
        static ValueTuple<char, StringPtr>[] WhitespaceReplacements = new ValueTuple<char, StringPtr>[]
        {
            (CHAR_TAB, " "),
            (CHAR_LINE_FEED, " "),
            (CHAR_FORM_FEED, " "),
            (CHAR_CARRIAGE_RETURN, " "),
            (CHAR_SPACE, " "),
        };

        /// <summary>
        /// Strips leading and trailing whitespace from a string and also collapses groups of whitespace characters with a single space
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <returns>Altered string</returns>
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
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Args">The strings to join</param>
        /// <returns></returns>
        public static String Concat(char Delim, IEnumerable<ReadOnlyMemory<char>> Args)
        {
            if (Args is null) throw new ArgumentNullException(nameof(Args));
            Contract.EndContractBlock();

            int chunkCount = 0;
            int newLength = 0;
            foreach (var chunk in Args)
            {
                chunkCount++;
                newLength += chunk.Length;
            }

            // Compile the new string
            if (Delim != '\0')
            {
                var insertCount = (chunkCount - 1);// Number of delimiters we will insert into new string
                newLength += insertCount;
            }

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in Args)
            {
                // Insert delimiter
                if (Delim != '\0' && index > 0)
                {
                    data.Span[index] = Delim;
                    index += 1;
                }

                // Copy substring 
                chunk.CopyTo(data.Slice(index));
                index += chunk.Length;
            }

            return new string(dataPtr);
        }

        /// <summary>
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Args">The strings to join</param>
        /// <returns></returns>
        public static String Concat(char Delim, params StringPtr[] Args)
        {
            if (Args is null) throw new ArgumentNullException(nameof(Args));
            Contract.EndContractBlock();

            var chunkCount = Args.Length;
            int newLength = 0;
            for (int i = 0; i < chunkCount; i++)
            {
                newLength += Args[i].Length;
            }

            // Compile the new string
            if (Delim != '\0')
            {
                var insertCount = (chunkCount - 1);// Number of delimiters we will insert into new string
                newLength += insertCount;
            }

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in Args)
            {
                // Insert delimiter
                if (Delim != '\0' && index > 0)
                {
                    data.Span[index] = Delim;
                    index += 1;
                }

                // Copy substring 
                chunk.Data.CopyTo(data.Slice(index));
                index += chunk.Length;
            }

            return new string(dataPtr);
        }

        /// <summary>
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Args">The strings to join</param>
        /// <returns></returns>
        public static String Concat(ReadOnlySpan<char> Delim, IEnumerable<ReadOnlyMemory<char>> Args)
        {
            if (Args is null) throw new ArgumentNullException(nameof(Args));
            Contract.EndContractBlock();

            int chunkCount = 0;
            int newLength = 0;
            foreach (var chunk in Args)
            {
                chunkCount++;
                newLength += chunk.Length;
            }

            // Compile the new string
            var substituteLength = Delim.Length;
            var insertCount = (chunkCount - 1);// Number of delimiters we will insert into new string
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in Args)
            {
                // Insert delimiter
                if (index > 0)
                {
                    Delim.CopyTo(data.Span.Slice(index));
                    index += substituteLength;
                }

                // Copy substring 
                chunk.CopyTo(data.Slice(index));
                index += chunk.Length;
            }

            return new string(dataPtr);
        }

        /// <summary>
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Args">The strings to join</param>
        /// <returns></returns>
        public static String Concat(ReadOnlySpan<char> Delim, params StringPtr[] Args)
        {
            if (Args is null) throw new ArgumentNullException(nameof(Args));
            Contract.EndContractBlock();

            int chunkCount = Args.Length;
            int newLength = 0;
            for (int i = 0; i < chunkCount; i++)
            {
                newLength += Args[i].Length;
            }

            // Compile the new string
            var substituteLength = Delim.Length;
            var insertCount = (chunkCount - 1);// Number of delimiters we will insert into new string
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in Args)
            {
                // Insert delimiter
                if (index > 0)
                {
                    Delim.CopyTo(data.Span.Slice(index));
                    index += substituteLength;
                }

                // Copy substring 
                chunk.Data.CopyTo(data.Slice(index));
                index += chunk.Length;
            }

            return new string(dataPtr);
        }

        /* Delimitless concats */

        /// <summary>
        /// Concatenates an array of strings into a single string
        /// </summary>
        /// <param name="Args">The strings to join</param>
        /// <returns></returns>
        public static String Concat(IEnumerable<ReadOnlyMemory<char>> Args)
        {
            if (Args is null) throw new ArgumentNullException(nameof(Args));
            Contract.EndContractBlock();

            int chunkCount = 0;
            int newLength = 0;
            foreach (var chunk in Args)
            {
                chunkCount++;
                newLength += chunk.Length;
            }

            // Compile the new string
            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in Args)
            {
                // Copy substring
                chunk.CopyTo(data.Slice(index));
                index += chunk.Length;
            }

            return new string(dataPtr);
        }

        /// <summary>
        /// Concatenates an array of strings into a single string
        /// </summary>
        /// <param name="Args">The strings to join</param>
        /// <returns></returns>
        public static String Concat(params StringPtr[] Args)
        {
            if (Args is null) throw new ArgumentNullException(nameof(Args));
            Contract.EndContractBlock();
            if (Args.Length <= 0) return String.Empty;

            int chunkCount = Args.Length;
            int newLength = 0;
            for (int i = 0; i < chunkCount; i++)
            {
                newLength += Args[i].Length;
            }

            // Compile the new string
            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in Args)
            {
                // Copy substring 
                chunk.AsMemory().CopyTo(data.Slice(index));
                index += chunk.Length;
            }

            return new string(dataPtr);
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
        public static ReadOnlyMemory<char> Trim(StringPtr Input, char Delim)
        {
            StringPtr Ptr = Input;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                if (Ptr.Data.Span[i] != Delim)
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                if (Ptr.Data.Span[i] != Delim)
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading or trailing instances of <paramref name="Delim"/> by offsetting its start and end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delims">The characters to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> Trim(StringPtr Input, params char[] Delims)
        {
            if (Delims.Length <= 0) return Input;

            StringPtr Ptr = Input;
            var span = Ptr.Data.Span;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                bool found = false;
                for (int x = 0; x < Delims.Length; x++)
                {
                    if (Delims[x] == span[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                bool found = false;
                for (int x = 0; x < Delims.Length; x++)
                {
                    if (Delims[x] == span[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading or trailing code points which do not match the given <paramref name="Filter"/> by offsetting its start and end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Filter">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> Trim(StringPtr Input, DataFilter<char> Filter)
        {
            var Ptr = Input;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                if (Filter.acceptData(Ptr.Data.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                if (Filter.acceptData(Ptr.Data.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading or trailing code points which match the given <paramref name="Predicate"/> by offsetting its start and end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Predicate">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> Trim(StringPtr Input, Predicate<char> Predicate)
        {
            var Ptr = Input;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                if (!Predicate(Ptr.Data.Span[i]))
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                if (!Predicate(Ptr.Data.Span[i]))
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }
        #endregion

        #region Trim Start
        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading instances of <paramref name="Delim"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delim">The character to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(StringPtr Input, char Delim)
        {
            var Ptr = Input;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                if (Ptr.Data.Span[i] != Delim)
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading instances of <paramref name="Delims"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delims">The characters to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(StringPtr Input, params char[] Delims)
        {
            if (Delims.Length <= 0) return Input;

            var Ptr = Input;
            var span = Ptr.Data.Span;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                bool found = false;
                for (int x = 0; x < Delims.Length; x++)
                {
                    if (Delims[x] == span[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading code points which do not match the given <paramref name="Filter"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Filter">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(StringPtr Input, DataFilter<char> Filter)
        {
            var Ptr = Input;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                if (Filter.acceptData(Ptr.Data.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading code points which match the given <paramref name="Predicate"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Predicate">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(StringPtr Input, Predicate<char> Predicate)
        {
            var Ptr = Input;
            /* Trim start */
            for (int i = 0; i < Ptr.Length; i++)
            {
                if (!Predicate(Ptr.Data.Span[i]))
                {
                    Ptr = Ptr.Data.Slice(i);
                    break;
                }
            }

            return Ptr;
        }
        #endregion

        #region Trim End
        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing instances of <paramref name="Delim"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delim">The character to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(StringPtr Input, char Delim)
        {
            var Ptr = Input;
            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                if (Ptr.Data.Span[i] != Delim)
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing instances of <paramref name="Delims"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delims">The characters to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(StringPtr Input, params char[] Delims)
        {
            if (Delims.Length <= 0) return Input;

            var Ptr = Input;
            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                bool found = false;
                for (int x = 0; x < Delims.Length; x++)
                {
                    if (Delims[x] == Ptr.Data.Span[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing code points which do not match the given <paramref name="Filter"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Filter">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(StringPtr Input, DataFilter<char> Filter)
        {
            var Ptr = Input;
            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                if (Filter.acceptData(Ptr.Data.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing code points which match the given <paramref name="Predicate"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Predicate">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(StringPtr Input, Predicate<char> Predicate)
        {
            var Ptr = Input;
            /* Trim end */
            for (int i = Ptr.Length - 1; i > -1; i--)
            {
                if (!Predicate(Ptr.Data.Span[i]))
                {
                    Ptr = Ptr.Data.Slice(0, Ptr.Length - i);
                    break;
                }
            }

            return Ptr;
        }
        #endregion
        #endregion

        #region Tokenization
        /// <summary>
        /// Splits a string <paramref name="Source"/> into tokens based on a given delimiter(s)
        /// </summary>
        /// <param name="Source">The string to tokenize</param>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<char>[] Strtok(StringPtr Source, char Delim)
        {
            return Strtok(Source, new char[1] { Delim });
        }

        /// <summary>
        /// Splits a string <paramref name="Source"/> into tokens based on a given delimiter(s)
        /// </summary>
        /// <param name="Source">The string to tokenize</param>
        /// <param name="Delims">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        /// DO NOT INLINE THIS FUNCTION
        public static ReadOnlyMemory<char>[] Strtok(StringPtr Source, params char[] Delims)
        {
            if (Source is null) throw new ArgumentNullException(nameof(Source));
            if (Delims is null) throw new ArgumentNullException(nameof(Delims));
            if (Delims.Length == 0) throw new ArgumentException("Delimeters must be non-null and contain one or more characters");
            Contract.EndContractBlock();

            // Split the source string into chunks using the given delimiters
            IEnumerable<StringChunk> AllChunks = _chunkify(Source.AsSpan(), true, Delims);
            // We only want to return chunks that arent delimiters
            IEnumerable<StringChunk> Chunks = AllChunks.Where(x => !x.IsDelimiter);
            // Compile the return list of memory segments
            var Src = Source.AsMemory();
            ReadOnlyMemory<char>[] RetVal = Chunks.Select(Chunk => Src.Slice(Chunk.Start, Chunk.Size)).ToArray();

            return RetVal;
        }

        /// <summary>
        /// Splits a string <paramref name="Source"/> into tokens based on a given delimeter(s)
        /// </summary>
        /// <param name="Source">The string to tokenize</param>
        /// <param name="Filter">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        /// /// DO NOT INLINE THIS FUNCTION
        public static ReadOnlyMemory<char>[] Strtok(StringPtr Source, DataFilter<char> Filter = null)
        {
            if (Source is null) throw new ArgumentNullException(nameof(Source));
            if (Filter is null) throw new ArgumentNullException(nameof(Filter));
            Contract.EndContractBlock();

            // Split the source string into chunks using the given delimiters
            IEnumerable<StringChunk> AllChunks = _chunkify(Source.AsSpan(), true, Filter);
            // We only want to return chunks that arent delimiters
            IEnumerable<StringChunk> Chunks = AllChunks.Where(x => !x.IsDelimiter);
            // Compile the return list of memory segments
            var Src = Source.AsMemory();
            ReadOnlyMemory<char>[] RetVal = Chunks.Select(Chunk => Src.Slice(Chunk.Start, Chunk.Size)).ToArray();

            return RetVal;
        }
        #endregion


        #region Mutation


        /// <summary>
        /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
        /// </summary>
        /// <param name="Source">Target string</param>
        /// <param name="Trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
        /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlySpan<char> Source, bool Trim = false, bool Collapse = false, params ValueTuple<char, StringPtr>[] Replacements)
        {
            if (Source.IsEmpty) return string.Empty;
            if (Replacements.Length <= 0) return Source.ToString();
            Contract.EndContractBlock();

            // Prepare the arrays needed for the generic chunking functions
            char[] Delimiters = Replacements.Select(o => o.Item1).ToArray();
            StringPtr[] Substitutions = Replacements.Select(o => o.Item2).ToArray();

            // Seperate the source memory into chunks using the given predicates
            var Chunks = _chunkify(Source, Collapse, Delimiters);
            // Calculate size of the new string
            int newLength = _tally_chunks(ref Chunks, Trim, Substitutions);
            // Allocate final string
            char[] pBuffer = _compile_chunks(Source, Chunks, newLength, Substitutions);
            return new string(pBuffer);
        }

        /// <summary>
        /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
        /// </summary>
        /// <param name="Source">Target string</param>
        /// <param name="Trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
        /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlySpan<char> Source, bool Trim = false, bool Collapse = false, params ValueTuple<Predicate<char>, StringPtr>[] Replacements)
        {
            if (Source.IsEmpty) return string.Empty;
            if (Replacements.Length <= 0) return Source.ToString();
            Contract.EndContractBlock();

            // Prepare the arrays needed for the generic chunking functions
            Predicate<char>[] Predicates = Replacements.Select(o => o.Item1).ToArray();
            StringPtr[] Substitutions = Replacements.Select(o => o.Item2).ToArray();

            // Seperate the source memory into chunks using the given predicates
            var Chunks = _chunkify(Source, Collapse, Predicates);
            // Calculate size of the new string
            int newLength = _tally_chunks(ref Chunks, Trim, Substitutions);
            // Allocate final string
            char[] pBuffer = _compile_chunks(Source, Chunks, newLength, Substitutions);
            return new string(pBuffer);
        }

        /// <summary>
        /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
        /// </summary>
        /// <param name="Source">Target string</param>
        /// <param name="Trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
        /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlySpan<char> Source, bool Trim = false, bool Collapse = false, params ValueTuple<DataFilter<char>, StringPtr>[] Replacements)
        {
            if (Source.IsEmpty) return string.Empty;
            if (Replacements.Length <= 0) return Source.ToString();
            Contract.EndContractBlock();
            // Prepare the arrays needed for the generic chunking functions
            DataFilter<char>[] Filters = Replacements.Select(o => o.Item1).ToArray();
            StringPtr[] Substitutions = Replacements.Select(o => o.Item2).ToArray();

            // Seperate the source memory into chunks using the given predicates
            var Chunks = _chunkify(Source, Collapse, Filters);
            // Calculate size of the new string
            int newLength = _tally_chunks(ref Chunks, Trim, Substitutions);
            // Allocate final string
            char[] pBuffer = _compile_chunks(Source, Chunks, newLength, Substitutions);
            return new string(pBuffer);
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
            int Length = buffMem.Length;
            char[] data = new char[Length];
            int idx = 0;

            for (int i = 0; i < Length; i++)
            {
                char ch = Transform(buffMem[i]);
                if (ch != CHAR_NULL)
                {
                    data[idx++] = ch;
                }
            }

            return new string(data, 0, idx);
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
        private static int Scan_Match(ReadOnlySpan<char> Source, ReadOnlySpan<char> Match, int Offset)
        {
            if (Offset > 0) Source = Source.Slice(Offset);
            var idx = Source.IndexOf(Match, StringComparison.Ordinal);
            if (idx > -1) idx += Offset;

            return idx;
        }

        /// <summary>
        /// Searches the given string and returns the next location where its value doesn't match the search value
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        /// <returns>-1 on failure</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Scan_Mismatch(ReadOnlySpan<char> Source, ReadOnlySpan<char> Match, int Offset)
        {
            if (Offset > 0) Source = Source.Slice(Offset);
            if (Source.StartsWith(Match))
            {// Scan forwards to the end of the block of matching chars at the start
                ReadOnlySpan<char> ptr = Source.Slice(Match.Length);
                int i = 1;
                for (; ptr.Length > Match.Length && ptr.StartsWith(Match); i += Match.Length)
                {
                    ptr = ptr.Slice(Match.Length);
                }
                // This is the end of the matching block at the start of this string, and therefore is the index of the next mismatch
                if (i > -1) i += Offset;
                return i;
            }

            var idx = Source.IndexOf(Match, StringComparison.Ordinal);
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
        private static int Scan_Match(ReadOnlySpan<char> Source, DataFilter<char> Filter, int Offset)
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
        private static int Scan_Mismatch(ReadOnlySpan<char> Source, DataFilter<char> Filter, int Offset)
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
        private static int Scan_Match(ReadOnlySpan<char> Source, DataFilter<char>[] Filters, int Offset, out int MatchedIndex)
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
        private static int Scan_Mismatch(ReadOnlySpan<char> Source, DataFilter<char>[] Filters, int Offset)
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
        private static LinkedList<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, params char[] Delimiters)
        {
            var Chunks = new LinkedList<StringChunk>();

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
                        Chunks.AddLast(new StringChunk(lastChunkEnd, tailLen));
                    }
                    break;
                }
                else
                {
                    // Add room for the non-replaced contents inbetween this chunk and the last
                    var interChunkLen = chunkStart - lastChunkEnd;
                    if (interChunkLen > 0)
                    {
                        Chunks.AddLast(new StringChunk(lastChunkEnd, interChunkLen));
                    }
                }

                Debug.Assert(DelimiterIndex > -1);
                // Find the length of the chunk by locating the next spot which doesn't match any replacements
                int chunkEnd = chunkStart + 1;
                if (Collapse)
                {// In order to collapse we have to consume all consecutive replacements
                    chunkEnd = Scan_Mismatch(Source, Delimiters, chunkStart);
                    if (chunkEnd < 0)
                    {// If no character mismatch could be found then the rest of the string consists of amtches
                        chunkEnd = Source.Length;
                    }
                }

                var chunkSize = chunkEnd - chunkStart;
                // Push new chunk
                Chunks.AddLast(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
                // Move our read position forward for the next pass
                chunkStart = chunkEnd;
            }

            return Chunks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LinkedList<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, Predicate<char> Predicate)
        {
            var Chunks = new LinkedList<StringChunk>();
            const int DelimiterIndex = 0;// We just use this here to keep this generic code the same

            // Scan for replacement characters, when encountered create a new chunk(non inclusive).
            for (int chunkStart = 0; chunkStart < Source.Length;)
            {
                var lastChunkEnd = Math.Max(0, chunkStart);
                // Find the chunk starting pos by finding the next occurance of any of the replacement targets
                chunkStart = Scan_Match(Source, Predicate, chunkStart);

                // If no more replacements can be found then abort
                if (chunkStart < 0)
                {
                    int tailLen = Source.Length - lastChunkEnd;
                    if (tailLen > 0)
                    {
                        Chunks.AddLast(new StringChunk(lastChunkEnd, tailLen));
                    }
                    break;
                }
                else
                {
                    // Add room for the non-replaced contents inbetween this chunk and the last
                    var interChunkLen = chunkStart - lastChunkEnd;
                    if (interChunkLen > 0)
                    {
                        Chunks.AddLast(new StringChunk(lastChunkEnd, interChunkLen));
                    }
                }

                Debug.Assert(DelimiterIndex > -1);
                // Find the length of the chunk by locating the next spot which doesn't match any replacements
                int chunkEnd = chunkStart + 1;
                if (Collapse)
                {// In order to collapse we have to consume all consecutive replacements
                    chunkEnd = Scan_Mismatch(Source, Predicate, chunkStart);
                    if (chunkEnd < 0)
                    {// If no character mismatch could be found then the rest of the string consists of amtches
                        chunkEnd = Source.Length;
                    }
                }

                var chunkSize = chunkEnd - chunkStart;
                // Push new chunk
                Chunks.AddLast(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
                // Move our read position forward for the next pass
                chunkStart = chunkEnd;
            }

            return Chunks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LinkedList<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, params Predicate<char>[] Predicates)
        {
            var Chunks = new LinkedList<StringChunk>();

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
                        Chunks.AddLast(new StringChunk(lastChunkEnd, tailLen));
                    }
                    break;
                }
                else
                {
                    // Add room for the non-replaced contents inbetween this chunk and the last
                    var interChunkLen = chunkStart - lastChunkEnd;
                    if (interChunkLen > 0)
                    {
                        Chunks.AddLast(new StringChunk(lastChunkEnd, interChunkLen));
                    }
                }

                Debug.Assert(DelimiterIndex > -1);
                // Find the length of the chunk by locating the next spot which doesn't match any replacements
                int chunkEnd = chunkStart + 1;
                if (Collapse)
                {// In order to collapse we have to consume all consecutive replacements
                    chunkEnd = Scan_Mismatch(Source, Predicates, chunkStart);
                    if (chunkEnd < 0)
                    {// If no character mismatch could be found then the rest of the string consists of amtches
                        chunkEnd = Source.Length;
                    }
                }

                var chunkSize = chunkEnd - chunkStart;
                // Push new chunk
                Chunks.AddLast(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
                // Move our read position forward for the next pass
                chunkStart = chunkEnd;
            }

            return Chunks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LinkedList<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, DataFilter<char> Filter)
        {
            var Chunks = new LinkedList<StringChunk>();
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
                        Chunks.AddLast(new StringChunk(lastChunkEnd, tailLen));
                    }
                    break;
                }
                else
                {
                    // Add room for the non-replaced contents inbetween this chunk and the last
                    var interChunkLen = chunkStart - lastChunkEnd;
                    if (interChunkLen > 0)
                    {
                        Chunks.AddLast(new StringChunk(lastChunkEnd, interChunkLen));
                    }
                }

                Debug.Assert(DelimiterIndex > -1);
                // Find the length of the chunk by locating the next spot which doesn't match any replacements
                int chunkEnd = chunkStart + 1;
                if (Collapse)
                {// In order to collapse we have to consume all consecutive replacements
                    chunkEnd = Scan_Mismatch(Source, Filter, chunkStart);
                    if (chunkEnd < 0)
                    {// If no character mismatch could be found then the rest of the string consists of amtches
                        chunkEnd = Source.Length;
                    }
                }

                var chunkSize = chunkEnd - chunkStart;
                // Push new chunk
                Chunks.AddLast(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
                // Move our read position forward for the next pass
                chunkStart = chunkEnd;
            }

            return Chunks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static LinkedList<StringChunk> _chunkify(ReadOnlySpan<char> Source, bool Collapse, params DataFilter<char>[] Delimiters)
        {
            var Chunks = new LinkedList<StringChunk>();

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
                        Chunks.AddLast(new StringChunk(lastChunkEnd, tailLen));
                    }
                    break;
                }
                else
                {
                    // Add room for the non-replaced contents inbetween this chunk and the last
                    var interChunkLen = chunkStart - lastChunkEnd;
                    if (interChunkLen > 0)
                    {
                        Chunks.AddLast(new StringChunk(lastChunkEnd, interChunkLen));
                    }
                }

                Debug.Assert(DelimiterIndex > -1);
                // Find the length of the chunk by locating the next spot which doesn't match any replacements
                int chunkEnd = chunkStart + 1;
                if (Collapse)
                {// In order to collapse we have to consume all consecutive replacements
                    chunkEnd = Scan_Mismatch(Source, Delimiters, chunkStart);
                    if (chunkEnd < 0)
                    {// If no character mismatch could be found then the rest of the string consists of amtches
                        chunkEnd = Source.Length;
                    }
                }

                var chunkSize = chunkEnd - chunkStart;
                // Push new chunk
                Chunks.AddLast(new StringChunk(chunkStart, chunkSize, DelimiterIndex));
                // Move our read position forward for the next pass
                chunkStart = chunkEnd;
            }

            return Chunks;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _tally_chunks(ref LinkedList<StringChunk> Chunks, bool Trim, int[] SubstitutionLengths)
        {
            int Length = 0;
            LinkedListNode<StringChunk> node = Chunks.First;
            while (node is object)
            {
                StringChunk chunk = node.Value;
                if (chunk.IsDelimiter)
                {
                    // Check if this chunk qualifies to be trimmed
                    if (Trim && node.Previous is null)
                    {
                        node = node.Next;
                        Chunks.RemoveFirst();
                        continue;
                    }
                    else if (Trim && node.Next is null)
                    {
                        node = node.Next;
                        Chunks.RemoveLast();
                        continue;
                    }

                    Length += SubstitutionLengths[chunk.DelimiterIndex];
                }
                else
                {
                    Length += chunk.Size;
                }

                node = node.Next;
            }

            if (Trim && Chunks.Last.Value.IsDelimiter)
            {
                // Make sure theres no trailing delimiter chunks
                while (Chunks.Last.Value.IsDelimiter)
                {
                    Length -= SubstitutionLengths[Chunks.Last.Value.DelimiterIndex];
                    Chunks.RemoveLast();
                }
            }

            return Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _tally_chunks(ref LinkedList<StringChunk> Chunks, bool Trim, StringPtr[] Substitutions)
        {
            int Length = 0;
            LinkedListNode<StringChunk> node = Chunks.First;
            while (node is object)
            {
                StringChunk chunk = node.Value;
                if (chunk.IsDelimiter)
                {
                    // Check if this chunk qualifies to be trimmed
                    if (Trim && node.Previous is null)
                    {
                        node = node.Next;
                        Chunks.RemoveFirst();
                        continue;
                    }
                    else if (Trim && node.Next is null)
                    {
                        node = node.Next;
                        Chunks.RemoveLast();
                        continue;
                    }

                    Length += Substitutions[chunk.DelimiterIndex].Length;
                }
                else
                {
                    Length += chunk.Size;
                }

                node = node.Next;
            }

            if (Trim && Chunks.Last.Value.IsDelimiter)
            {
                // Make sure theres no trailing delimiter chunks
                while (Chunks.Last.Value.IsDelimiter)
                {
                    Length -= Substitutions[Chunks.Last.Value.DelimiterIndex].Length;
                    Chunks.RemoveLast();
                }
            }

            return Length;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char[] _compile_chunks(ReadOnlySpan<char> Source, LinkedList<StringChunk> Chunks, int Length, char[] Substitutions)
        {
            // Allocate final string
            char[] pBuffer = new char[Length];
            Memory<char> Buffer = new Memory<char>(pBuffer);
            int writePos = 0;

            var node = Chunks.First;
            while (node is object)
            {
                StringChunk Chunk = node.Value;
                node = node.Next;
                int chunkEnd = Chunk.Start + Chunk.Size;

                if (Chunk.IsDelimiter)
                {
                    if (Chunk.DelimiterIndex >= 0)
                    {
                        var Substitution = Substitutions[Chunk.DelimiterIndex];
                        if (Substitution != '\0')
                        {
                            // Insert substitute
                            pBuffer[writePos] = Substitution;
                            // Increase writePos
                            writePos += 1;
                        }
                    }
                }
                else
                {
                    // Insert this chunks data directly into new string unmodified
                    Debug.Assert(chunkEnd <= Source.Length);

                    if (Chunk.Start < Source.Length && Chunk.Size > 0)
                    {
                        Source.Slice(Chunk.Start, Chunk.Size).CopyTo(Buffer.Span.Slice(writePos));
                        // Increase writePos
                        writePos += Chunk.Size;
                    }
                }
            }

            return pBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char[] _compile_chunks(ReadOnlySpan<char> Source, LinkedList<StringChunk> Chunks, int Length, StringPtr[] Substitutions)
        {
            // Allocate final string
            char[] pBuffer = new char[Length];
            Memory<char> Buffer = new Memory<char>(pBuffer);
            int writePos = 0;

            var node = Chunks.First;
            while (node is object)
            {
                StringChunk Chunk = node.Value;
                node = node.Next;
                int chunkEnd = Chunk.Start + Chunk.Size;

                if (Chunk.IsDelimiter)
                {
                    if (Chunk.DelimiterIndex >= 0)
                    {
                        var Substitution = Substitutions[Chunk.DelimiterIndex];
                        if (Substitution.Length > 0)
                        {
                            // Insert substitute
                            var replaceTarget = Buffer.Slice(writePos);
                            var replaceSource = Substitution.AsMemory();
                            replaceSource.CopyTo(replaceTarget);
                            // Increase writePos
                            writePos += replaceSource.Length;
                        }
                    }
                }
                else
                {
                    // Insert this chunks data directly into new string unmodified
                    Debug.Assert(chunkEnd <= Source.Length);

                    if (Chunk.Start < Source.Length && Chunk.Size > 0)
                    {
                        Source.Slice(Chunk.Start, Chunk.Size).CopyTo(Buffer.Span.Slice(writePos));
                        // Increase writePos
                        writePos += Chunk.Size;
                    }
                }
            }

            return pBuffer;
        }
        #endregion
    }
}
