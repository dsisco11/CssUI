using CssUI.DOM;
using CssUI.Filters;
using System;
using System.Collections.Generic;
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

            var Span = Str;//.Data.Span;
            for (int i = 0; i < Span.Length; i++)
            {
                var result = Filter.acceptData(Span[i]);
                if (result == EFilterResult.FILTER_ACCEPT)
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
                if (Span[i] == Search)
                {
                    RetVal++;
                }
            }

            return RetVal;
        }
        #endregion


        #region Transformations
        const string SPACE = " ";
        /// <summary>
        /// Strips leading and trailing whitespace from a string and also collapses groups of whitespace characters with a single space
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <returns>Altered string</returns>
        public static String Strip_And_Collapse_Whitespace(ReadOnlyMemory<char> buffMem)
        {/* Docs: https://infra.spec.whatwg.org/#strip-and-collapse-ascii-whitespace */
            return Replace(buffMem, FilterWhitespace.Instance, SPACE.AsSpan(), true);
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
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static String Concat(char Delim, IEnumerable<ReadOnlyMemory<char>> Inputs)
        {
            ulong chunkCount = 0;
            ulong newLength = 0;
            foreach (var chunk in Inputs)
            {
                chunkCount++;
                newLength += (ulong)chunk.Length;
            }

            /* Compile the new string */
            ulong insertCount = (chunkCount - 1);/* Number of delimiters we will insert into new string */
            newLength += insertCount;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            ulong index = 0;
            foreach (var chunk in Inputs)
            {
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

                /* Insert delimiter */
                data.Span[(int)index] = Delim;
                index += 1;
            }

            return newStr;
        }

        /// <summary>
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static String Concat(char Delim, params StringPtr[] Inputs)
        {
            ulong chunkCount = (ulong)Inputs.Length;
            ulong newLength = 0;
            for (ulong i = 0; i < chunkCount; i++)
            {
                newLength += (ulong)Inputs[i].Length;
            }

            /* Compile the new string */
            ulong insertCount = (chunkCount - 1);/* Number of delimiters we will insert into new string */
            newLength += insertCount;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            ulong index = 0;
            foreach (var chunk in Inputs)
            {
                /* Copy substring */
                chunk.Data.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

                /* Insert delimiter */
                data.Span[(int)index] = Delim;
                index += 1;
            }

            return newStr;
        }

        /// <summary>
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static String Concat(ReadOnlySpan<char> Delim, IEnumerable<ReadOnlyMemory<char>> Inputs)
        {
            ulong chunkCount = 0;
            ulong newLength = 0;
            foreach (var chunk in Inputs)
            {
                chunkCount++;
                newLength += (ulong)chunk.Length;
            }

            /* Compile the new string */
            ulong substituteLength = (ulong)Delim.Length;
            ulong insertCount = (chunkCount - 1);/* Number of delimiters we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            ulong index = 0;
            foreach (var chunk in Inputs)
            {
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

                /* Insert delimiter */
                Delim.CopyTo(data.Span.Slice((int)index));
                index += substituteLength;
            }

            return newStr;
        }

        /// <summary>
        /// Concatenates an array of strings into a single string with each original string seperated from the next by a given delimiter
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static String Concat(ReadOnlySpan<char> Delim, params StringPtr[] Inputs)
        {
            ulong chunkCount = (ulong)Inputs.Length;
            ulong newLength = 0;
            for (ulong i = 0; i < chunkCount; i++)
            {
                newLength += (ulong)Inputs[i].Length;
            }

            /* Compile the new string */
            ulong substituteLength = (ulong)Delim.Length;
            ulong insertCount = (chunkCount - 1);/* Number of delimiters we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            ulong index = 0;
            foreach (var chunk in Inputs)
            {
                /* Copy substring */
                chunk.Data.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

                /* Insert delimiter */
                Delim.CopyTo(data.Span.Slice((int)index));
                index += substituteLength;
            }

            return newStr;
        }

        /* Delimitless concats */

        /// <summary>
        /// Concatenates an array of strings into a single string
        /// </summary>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static String Concat(IEnumerable<ReadOnlyMemory<char>> Inputs)
        {
            ulong chunkCount = 0;
            ulong newLength = 0;
            foreach (var chunk in Inputs)
            {
                chunkCount++;
                newLength += (ulong)chunk.Length;
            }

            /* Compile the new string */
            ulong insertCount = (chunkCount - 1);/* Number of delimiters we will insert into new string */
            newLength += insertCount;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            ulong index = 0;
            foreach (var chunk in Inputs)
            {
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;
            }

            return newStr;
        }

        /// <summary>
        /// Concatenates an array of strings into a single string
        /// </summary>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static String Concat(params ReadOnlyMemory<char>[] Inputs)
        {
            ulong chunkCount = (ulong)Inputs.Length;
            ulong newLength = 0;
            for (ulong i = 0; i < chunkCount; i++)
            {
                newLength += (ulong)Inputs[i].Length;
            }

            /* Compile the new string */
            ulong insertCount = (chunkCount - 1);/* Number of delimiters we will insert into new string */
            newLength += insertCount;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            ulong index = 0;
            foreach (var chunk in Inputs)
            {
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;
            }

            return newStr;
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
        /// Splits a string <paramref name="Input"/> into tokens based on a given delimiter(s)
        /// </summary>
        /// <param name="Input">The string to tokenize</param>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        public static IReadOnlyList<ReadOnlyMemory<char>> Strtok(StringPtr Input, char Delim)
        {
            return Strtok(Input, new char[1] { Delim });
        }

        /// <summary>
        /// Splits a string <paramref name="Input"/> into tokens based on a given delimiter(s)
        /// </summary>
        /// <param name="Input">The string to tokenize</param>
        /// <param name="Delims">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        public static IReadOnlyList<ReadOnlyMemory<char>> Strtok(StringPtr Input, params char[] Delims)
        {
            if (Delims == null || Delims.Length == 0)
            {
                throw new ArgumentException("Delimeters must be non-null and contain one or more characters");
            }

            DataConsumer<char> Stream = new DataConsumer<char>(Input, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            int? chunkStart = null;
            int chunkCount = 0;
            var chunks = new List<ReadOnlyMemory<char>>(16);

            while (!Stream.atEnd)
            {
                EFilterResult filterResult = EFilterResult.FILTER_ACCEPT;
                if (Delims.Length == 1)
                {
                    if (Stream.Next == Delims[0])
                        filterResult = EFilterResult.FILTER_SKIP;
                }
                else
                {
                    foreach (char delim in Delims)
                    {
                        if (Stream.Next == delim)
                        {
                            filterResult = EFilterResult.FILTER_SKIP;
                        }
                    }
                }


                /* When filter result:
                 * ACCEPT: Char should be included in chunk
                 * SKIP: Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                 * REJECT: Char should not be included in chunk, current chunk ends
                 */
                bool end_chunk = false;
                switch (filterResult)
                {
                    case EFilterResult.FILTER_ACCEPT:// Char should be included in the chunk
                        {
                            if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;/* Start new chunk (if one isnt started yet) */
                        }
                        break;
                    case EFilterResult.FILTER_REJECT:// Char should not be included in chunk, current chunk ends
                        {
                            end_chunk = true;
                        }
                        break;
                    case EFilterResult.FILTER_SKIP:// Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                        {
                            if (!chunkStart.HasValue)
                            {
                                chunkStart = (int)Stream.LongPosition + 1;/* At chunk-start */
                            }
                            else
                            {
                                end_chunk = true;
                            }
                        }
                        break;
                }


                Stream.Consume();
                if (end_chunk || Stream.atEnd)
                {
                    if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition - 1;

                    /* Push new chunk to our list */
                    var chunkSize = (int)(Stream.LongPosition - 1) - chunkStart.Value;
                    chunks.Add(Input.Data.Slice((int)chunkStart.Value, (int)chunkSize));

                    chunkCount++;
                    chunkStart = null;
                }
            }

            return chunks;
        }

        /// <summary>
        /// Splits a string <paramref name="Input"/> into tokens based on a given delimeter(s)
        /// </summary>
        /// <param name="Input">The string to tokenize</param>
        /// <param name="Filter">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        public static IReadOnlyList<ReadOnlyMemory<char>> Strtok(StringPtr Input, DataFilter<char> Filter)
        {
            if (Filter == null)
            {
                throw new ArgumentNullException("Filter cannot be null");
            }

            DataConsumer<char> Stream = new DataConsumer<char>(Input, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            int? chunkStart = null;
            int chunkCount = 0;
            var chunks = new List<ReadOnlyMemory<char>>(16);

            while (!Stream.atEnd)
            {
                EFilterResult filterResult = Filter.acceptData(Stream.Next);

                /* When filter result:
                 * ACCEPT: Char should be included in chunk
                 * SKIP: Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                 * REJECT: Char should not be included in chunk, current chunk ends
                 */
                bool end_chunk = false;
                switch (filterResult)
                {
                    case EFilterResult.FILTER_ACCEPT:// Char should be included in the chunk
                        {
                            if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;/* Start new chunk (if one isnt started yet) */
                        }
                        break;
                    case EFilterResult.FILTER_REJECT:// Char should not be included in chunk, current chunk ends
                        {
                            end_chunk = true;
                        }
                        break;
                    case EFilterResult.FILTER_SKIP:// Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                        {
                            if (!chunkStart.HasValue)
                            {
                                chunkStart = (int)Stream.LongPosition + 1;/* At chunk-start */
                            }
                            else
                            {
                                end_chunk = true;
                            }
                        }
                        break;
                }

                Stream.Consume();
                if (end_chunk || Stream.atEnd)
                {
                    if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition - 1;

                    /* Push new chunk to our list */
                    var chunkSize = (int)(Stream.LongPosition - 1) - chunkStart.Value;
                    chunks.Add(Input.Data.Slice((int)chunkStart.Value, (int)chunkSize));

                    chunkCount++;
                    chunkStart = null;
                }

            }

            return chunks;
        }
        #endregion


        #region Mutation

        /// <summary>
        /// Replaces all characters indicated by the first value for each of the <paramref name="Replacements"/>, with the characters provided by their second value
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <param name="trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
        /// <param name="Replacements">A series of tuples containing characters to be replaced and the characters which will replace each of them</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlyMemory<char> buffMem, bool trim = false, params ValueTuple<char, StringPtr>[] Replacements)
        {
            DataConsumer<char> Stream = new DataConsumer<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            int newLength = 0;
            int? chunkStart = null;
            int chunkCount = 0;
            var chunks = new LinkedList<ValueTuple<ReadOnlyMemory<char>, int>>();
            int substituteLength = 0;

            /* Scan for replacement characters, when encountered create a new chunk (non inclusive). */
            while (!Stream.atEnd)
            {
                EFilterResult filterResult = EFilterResult.FILTER_ACCEPT;
                int replacementIndex = -1;

                for (int i = 0; i < Replacements.Length; i++)
                {
                    var replace = Replacements[i];
                    if (Stream.Next == replace.Item1)
                    {
                        filterResult = EFilterResult.FILTER_SKIP;
                        replacementIndex = i;
                        substituteLength += replace.Item2.Length;
                        break;
                    }
                }
                /* When filter result:
                 * ACCEPT: Char should be included in chunk
                 * SKIP: Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                 * REJECT: Char should not be included in chunk, current chunk ends
                 */
                bool end_chunk = false;
                switch (filterResult)
                {
                    case EFilterResult.FILTER_ACCEPT:// Char should be included in the chunk
                        {
                            if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;/* Start new chunk (if one isnt started yet) */
                        }
                        break;
                    case EFilterResult.FILTER_REJECT:// Char should not be included in chunk, current chunk ends
                        {
                            end_chunk = true;
                        }
                        break;
                    case EFilterResult.FILTER_SKIP:// Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                        {
                            if (!chunkStart.HasValue)
                            {
                                chunkStart = (int)Stream.LongPosition + 1;/* At chunk-start */
                            }
                            else
                            {
                                end_chunk = true;
                            }
                        }
                        break;
                }

                Stream.Consume();
                if (end_chunk || Stream.Next == Stream.EOF_ITEM)
                {
                    if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;
                    if (replacementIndex < 0) Stream.Consume();// if we arent actually trying to replace something in this chunk then we go ahead and include the last char also

                    /* Push new chunk to our list */
                    var chunkSize = (int)Stream.LongPosition - chunkStart.Value;
                    var Mem = buffMem.Slice((int)chunkStart.Value, (int)chunkSize);
                    chunks.AddLast((Mem, replacementIndex));

                    if (replacementIndex >= 0) chunkCount++;
                    chunkStart = null;
                    newLength += chunkSize;
                }
            }

            /* Compile the new string */

            if (trim)
            {/* To trim we just need to skip any empty chunks at the beginning and end */
                while (chunks.First.Value.Item1.Length <= 0)
                {
                    chunks.RemoveFirst();
                    chunkCount--;
                }

                while (chunks.Last.Value.Item1.Length <= 0)
                {
                    chunks.RemoveLast();
                    chunkCount--;
                }
            }

            int insertCount = chunkCount;/* Number of substitutes we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var tpl in chunks)
            {
                var chunk = tpl.Item1;
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += chunk.Length;
                if (tpl.Item2 < 0) continue;

                /* Insert substitute */
                var replaceIndex = tpl.Item2;
                var replaceTarget = data.Slice((int)index);
                var replaceSource = Replacements[replaceIndex].Item2.AsMemory();
                replaceSource.CopyTo(replaceTarget);
                index += replaceSource.Length;
            }

            return new string(dataPtr);
        }

        /// <summary>
        /// Replaces all characters, for which <paramref name="dataFilter"/> does not return <see cref="EFilterResult.FILTER_ACCEPT"/>, with the characters provided by <paramref name="substituteData"/>
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <param name="dataFilter"></param>
        /// <param name="substituteData"></param>
        /// <param name="trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlyMemory<char> buffMem, DataFilter<char> dataFilter, ReadOnlySpan<char> substituteData, bool trim = false)
        {
            DataConsumer<char> Stream = new DataConsumer<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            int newLength = 0;
            int? chunkStart = null;
            int chunkCount = 0;
            var chunks = new LinkedList<ValueTuple<ReadOnlyMemory<char>, bool>>();

            /* Scan for replacement characters, when encountered create a new chunk (non inclusive). */
            while (!Stream.atEnd)
            {
                EFilterResult filterResult = dataFilter.acceptData(Stream.Next);
                /* When filter result:
                 * ACCEPT: Char should be included in chunk
                 * SKIP: Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                 * REJECT: Char should not be included in chunk, current chunk ends
                 */
                bool end_chunk = false;
                switch (filterResult)
                {
                    case EFilterResult.FILTER_ACCEPT:// Char should be included in the chunk
                        {
                            if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;/* Start new chunk (if one isnt started yet) */
                        }
                        break;
                    case EFilterResult.FILTER_REJECT:// Char should not be included in chunk, current chunk ends
                        {
                            end_chunk = true;
                        }
                        break;
                    case EFilterResult.FILTER_SKIP:// Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                        {
                            if (!chunkStart.HasValue)
                            {
                                chunkStart = (int)Stream.LongPosition + 1;/* At chunk-start */
                            }
                            else
                            {
                                end_chunk = true;
                            }
                        }
                        break;
                }

                Stream.Consume();
                if (end_chunk || Stream.atEnd)
                {
                    if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition - 1;
                    if (Stream.atEnd) chunkStart++;// if we arent actually trying to replace something in this chunk then we go ahead and include the last char also

                    /* Push new chunk to our list */
                    var chunkSize = (int)(Stream.LongPosition - 1) - chunkStart.Value;
                    chunks.AddLast((buffMem.Slice((int)chunkStart.Value, (int)chunkSize), !Stream.atEnd));

                    chunkCount++;
                    chunkStart = null;
                    newLength += chunkSize;
                }

            }

            /* Compile the new string */

            if (trim)
            {/* To trim we just need to skip any empty chunks at the beginning and end */
                while (chunks.First.Value.Item1.Length <= 0)
                {
                    chunks.RemoveFirst();
                    chunkCount--;
                }

                while (chunks.Last.Value.Item1.Length <= 0)
                {
                    chunks.RemoveLast();
                    chunkCount--;
                }
            }

            int substituteLength = substituteData.Length;
            int insertCount = (chunkCount - 1);/* Number of substitutes we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            int index = 0;
            foreach (var chunk in chunks)
            {
                /* Copy substring */
                chunk.Item1.CopyTo(data.Slice((int)index));
                index += chunk.Item1.Length;
                if (!chunk.Item2) continue;
                /* Insert substitute */
                substituteData.CopyTo(data.Slice((int)index).Span);
                index += substituteLength;
            }

            return new string(dataPtr);
        }

        /// <summary>
        /// Replaces all characters indicated by each of the data-filters in <paramref name="Replacements"/> does not return <see cref="EFilterResult.FILTER_ACCEPT"/>, with the characters provided by their second value
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <param name="trim">If <c>True</c> then leading and trailing ends of the returned string will have the <paramref name="substituteData"/> stripped from them</param>
        /// <param name="Replacements">A series of tuples containing data filters and the characters to replace them.</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlyMemory<char> buffMem, bool trim = false, params ValueTuple<DataFilter<char>, StringPtr>[] Replacements)
        {
            DataConsumer<char> Stream = new DataConsumer<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            int newLength = 0;
            int? chunkStart = null;
            int chunkCount = 0;
            var chunks = new LinkedList<ValueTuple<ReadOnlyMemory<char>, int>>();
            int substituteLength = 0;

            /* Scan for replacement characters, when encountered create a new chunk (non inclusive). */
            while (!Stream.atEnd)
            {
                EFilterResult filterResult = EFilterResult.FILTER_ACCEPT;
                int replacementIndex = -1;

                for (int i = 0; i < Replacements.Length; i++)
                {
                    var replace = Replacements[i];
                    filterResult = replace.Item1.acceptData(Stream.Next);
                    if (filterResult != EFilterResult.FILTER_ACCEPT)
                    {
                        replacementIndex = i;
                        substituteLength += replace.Item2.Length;
                        break;
                    }
                }
                /* When filter result:
                 * ACCEPT: Char should be included in chunk
                 * SKIP: Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                 * REJECT: Char should not be included in chunk, current chunk ends
                 */
                bool end_chunk = false;
                switch (filterResult)
                {
                    case EFilterResult.FILTER_ACCEPT:// Char should be included in the chunk
                        {
                            if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;/* Start new chunk (if one isnt started yet) */
                        }
                        break;
                    case EFilterResult.FILTER_REJECT:// Char should not be included in chunk, current chunk ends
                        {
                            end_chunk = true;
                        }
                        break;
                    case EFilterResult.FILTER_SKIP:// Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                        {
                            if (!chunkStart.HasValue)
                            {
                                chunkStart = (int)Stream.LongPosition + 1;/* At chunk-start */
                            }
                            else
                            {
                                end_chunk = true;
                            }
                        }
                        break;
                }

                if (end_chunk || Stream.NextNext == Stream.EOF_ITEM)
                {
                    if (!chunkStart.HasValue) chunkStart = (int)Stream.LongPosition;
                    if (replacementIndex < 0) Stream.Consume();// if we arent actually trying to replace something in this chunk then we go ahead and include the last char also

                    /* Push new chunk to our list */
                    var chunkSize = (int)Stream.LongPosition - chunkStart.Value;
                    var Mem = buffMem.Slice((int)chunkStart.Value, (int)chunkSize);
                    chunks.AddLast((Mem, replacementIndex));

                    chunkCount++;
                    chunkStart = null;
                    newLength += chunkSize;
                }

                Stream.Consume();
            }

            /* Compile the new string */

            if (trim)
            {/* To trim we just need to skip any empty chunks at the beginning and end */
                while (chunks.First.Value.Item1.Length <= 0)
                {
                    chunks.RemoveFirst();
                    chunkCount--;
                }

                while (chunks.Last.Value.Item1.Length <= 0)
                {
                    chunks.RemoveLast();
                    chunkCount--;
                }
            }

            int insertCount = (chunkCount - 1);/* Number of substitutes we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            ulong index = 0;
            foreach (var tpl in chunks)
            {
                var chunk = tpl.Item1;
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;
                if (tpl.Item2 < 0) continue;

                /* Insert substitute */
                var replaceIndex = tpl.Item2;
                var replaceTarget = data.Slice((int)index);
                var replaceSource = Replacements[replaceIndex].Item2.AsMemory();
                replaceSource.CopyTo(replaceTarget);
                index += (ulong)replaceSource.Length;
            }

            return new string(dataPtr);
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

            for (int i=0; i<Length; i++)
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
    }
}
