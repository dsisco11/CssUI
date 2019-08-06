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
        public static bool StrEq(ReadOnlySpan<char> p, ReadOnlySpan<char> q) => p.Equals(q, StringComparison.Ordinal);

        /// <summary>
        /// Returns whether <paramref name="p"/> and <paramref name="q"/> contain the same (case-insensitive) values
        /// </summary>
        /// <returns>True if both strings are an case-insensitive match</returns>
        public static bool StriEq(ReadOnlySpan<char> p, ReadOnlySpan<char> q) => p.Equals(q, StringComparison.OrdinalIgnoreCase);
        #endregion

        #region Contains
        /// <summary>
        /// Returns whether <paramref name="str"/> contains any characters matching the given filter
        /// </summary>
        /// <returns>True if string contains a character which the given filter matches</returns>
        public static bool Contains(ReadOnlySpan<char> str, DataFilter<char> Filter)
        {
            if (Filter == null)
                return false;

            if (str.Length == 0)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                var result = Filter.acceptData(str[i]);
                if (result == EFilterResult.FILTER_ACCEPT)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether <paramref name="str"/> contains any characters matching the given filter
        /// </summary>
        /// <returns>True if string contains a character which the given filter matches</returns>
        public static bool Contains(ReadOnlySpan<char> str, Func<char, bool> Predicate)
        {
            if (Predicate == null)
                return false;

            if (str.Length == 0)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (Predicate(str[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether <paramref name="str"/> contains any characters matching the given filter
        /// </summary>
        /// <returns>True if string contains a character which the given filter matches</returns>
        public static bool Contains(ReadOnlySpan<char> str, char Search)
        {
            if (str.Length == 0)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == Search)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion


        #region Transformations
        const string SPACE = " ";
        /// <summary>
        /// Strips leading and trailing whitespace from a string and also collapses groups of whitespace characters with a single space
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <returns>Altered string</returns>
        public static string Strip_And_Collapse_Whitespace(ReadOnlyMemory<char> buffMem)
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
        public static string Concat(char Delim, IEnumerable<ReadOnlyMemory<char>> Inputs)
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
        public static string Concat(char Delim, params ReadOnlyMemory<char>[] Inputs)
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
        public static string Concat(ReadOnlyMemory<char> Delim, IEnumerable<ReadOnlyMemory<char>> Inputs)
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
                Delim.CopyTo(data.Slice((int)index));
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
        public static string Concat(ReadOnlyMemory<char> Delim, params ReadOnlyMemory<char>[] Inputs)
        {
            ulong chunkCount = (ulong)Inputs.Length;
            ulong newLength = 0;
            for (ulong i=0; i<chunkCount; i++)
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
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

                /* Insert delimiter */
                Delim.CopyTo(data.Slice((int)index));
                index += substituteLength;
            }

            return newStr;
        }

        /* Delimitless concats */

        /// <summary>
        /// Concatenates an array of strings into a single string
        /// </summary>
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static string Concat(IEnumerable<ReadOnlyMemory<char>> Inputs)
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
        /// <param name="Delim">The delimiter(s) that should seperate each token</param>
        /// <param name="Inputs">The strings to join</param>
        /// <returns></returns>
        public static string Concat(params ReadOnlyMemory<char>[] Inputs)
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
        public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, char Delim)
        {
            /* Trim start */
            for(int i=0; i<Input.Length; i++)
            {
                if (Input.Span[i] != Delim)
                {
                    Input = Input.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i=Input.Length-1; i > -1; i--)
            {
                if (Input.Span[i] != Delim)
                {
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading or trailing instances of <paramref name="Delim"/> by offsetting its start and end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delims">The characters to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, params char[] Delims)
        {
            if (Delims.Length <= 0) return Input;

            var span = Input.Span;
            /* Trim start */
            for (int i=0; i<Input.Length; i++)
            {
                bool found = false;
                for (int x=0; x<Delims.Length; x++)
                {
                    if (Delims[x] == span[i])
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Input = Input.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i=Input.Length-1; i > -1; i--)
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
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading or trailing code points which do not match the given <paramref name="Filter"/> by offsetting its start and end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Filter">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, DataFilter<char> Filter)
        {
            /* Trim start */
            for (int i = 0; i < Input.Length; i++)
            {
                if (Filter.acceptData(Input.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Input = Input.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i = Input.Length - 1; i > -1; i--)
            {
                if (Filter.acceptData(Input.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading or trailing code points which match the given <paramref name="Predicate"/> by offsetting its start and end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Predicate">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> Trim(ReadOnlyMemory<char> Input, Func<char, bool> Predicate)
        {
            /* Trim start */
            for (int i = 0; i < Input.Length; i++)
            {
                if (!Predicate(Input.Span[i]))
                {
                    Input = Input.Slice(i);
                    break;
                }
            }

            /* Trim end */
            for (int i = Input.Length - 1; i > -1; i--)
            {
                if (!Predicate(Input.Span[i]))
                {
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
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
            /* Trim start */
            for (int i = 0; i < Input.Length; i++)
            {
                if (Input.Span[i] != Delim)
                {
                    Input = Input.Slice(i);
                    break;
                }
            }
            
            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading instances of <paramref name="Delims"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delims">The characters to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, params char[] Delims)
        {
            if (Delims.Length <= 0) return Input;

            var span = Input.Span;
            /* Trim start */
            for (int i = 0; i < Input.Length; i++)
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
                    Input = Input.Slice(i);
                    break;
                }
            }
            
            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading code points which do not match the given <paramref name="Filter"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Filter">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, DataFilter<char> Filter)
        {
            /* Trim start */
            for (int i = 0; i < Input.Length; i++)
            {
                if (Filter.acceptData(Input.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Input = Input.Slice(i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any leading code points which match the given <paramref name="Predicate"/> by offsetting its start position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Predicate">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimStart(ReadOnlyMemory<char> Input, Func<char, bool> Predicate)
        {
            /* Trim start */
            for (int i = 0; i < Input.Length; i++)
            {
                if (!Predicate(Input.Span[i]))
                {
                    Input = Input.Slice(i);
                    break;
                }
            }

            return Input;
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
            /* Trim end */
            for (int i = Input.Length - 1; i > -1; i--)
            {
                if (Input.Span[i] != Delim)
                {
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing instances of <paramref name="Delims"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Delims">The characters to trim out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, params char[] Delims)
        {
            if (Delims.Length <= 0) return Input;
            
            /* Trim end */
            for (int i = Input.Length - 1; i > -1; i--)
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
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing code points which do not match the given <paramref name="Filter"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Filter">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, DataFilter<char> Filter)
        {
            /* Trim end */
            for (int i = Input.Length - 1; i > -1; i--)
            {
                if (Filter.acceptData(Input.Span[i]) == EFilterResult.FILTER_ACCEPT)
                {
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
        }

        /// <summary>
        /// Modifies the given <paramref name="Input"/>, removing any trailing code points which match the given <paramref name="Predicate"/> by offsetting its end position without modifying its data or creating a new string instance
        /// </summary>
        /// <param name="Input">The string memory to trim</param>
        /// <param name="Predicate">The filter used to trim characters out of the input</param>
        /// <returns></returns>
        public static ReadOnlyMemory<char> TrimEnd(ReadOnlyMemory<char> Input, Func<char, bool> Predicate)
        {
            /* Trim end */
            for (int i = Input.Length - 1; i > -1; i--)
            {
                if (!Predicate(Input.Span[i]))
                {
                    Input = Input.Slice(0, Input.Length - i);
                    break;
                }
            }

            return Input;
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
        public static IReadOnlyList<ReadOnlyMemory<char>> Strtok(ReadOnlyMemory<char> Input, char Delim)
        {
            return Strtok(Input, new char[1] { Delim });
        }

        /// <summary>
        /// Splits a string <paramref name="Input"/> into tokens based on a given delimiter(s)
        /// </summary>
        /// <param name="Input">The string to tokenize</param>
        /// <param name="Delims">The delimiter(s) that should seperate each token</param>
        /// <returns></returns>
        public static IReadOnlyList<ReadOnlyMemory<char>> Strtok(ReadOnlyMemory<char> Input, params char[] Delims)
        {
            if (Delims == null || Delims.Length == 0)
            {
                throw new ArgumentException("Delimeters must be non-null and contain one or more characters");
            }

            DataStream<char> Stream = new DataStream<char>(Input, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            ulong? chunkStart = null;
            ulong chunkCount = 0;
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
                            if (!chunkStart.HasValue) chunkStart = Stream.Position;/* Start new chunk (if one isnt started yet) */
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
                                chunkStart = Stream.Position + 1;/* At chunk-start */
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
                    if (!chunkStart.HasValue) chunkStart = Stream.Position - 1;

                    /* Push new chunk to our list */
                    var chunkSize = (Stream.Position - 1) - chunkStart.Value;
                    chunks.Add(Input.Slice((int)chunkStart.Value, (int)chunkSize));

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
        public static IReadOnlyList<ReadOnlyMemory<char>> Strtok(ReadOnlyMemory<char> Input, DataFilter<char> Filter)
        {
            if (Filter == null)
            {
                throw new ArgumentNullException("Filter cannot be null");
            }

            DataStream<char> Stream = new DataStream<char>(Input, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            ulong? chunkStart = null;
            ulong chunkCount = 0;
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
                            if (!chunkStart.HasValue) chunkStart = Stream.Position;/* Start new chunk (if one isnt started yet) */
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
                                chunkStart = Stream.Position + 1;/* At chunk-start */
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
                    if (!chunkStart.HasValue) chunkStart = Stream.Position - 1;

                    /* Push new chunk to our list */
                    var chunkSize = (Stream.Position -1) - chunkStart.Value;
                    chunks.Add(Input.Slice((int)chunkStart.Value, (int)chunkSize));

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
        public static string Replace(ReadOnlyMemory<char> buffMem, bool trim = false, params Tuple<char, ReadOnlyMemory<char>>[] Replacements)
        {
            DataStream<char> Stream = new DataStream<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            ulong newLength = 0;
            ulong? chunkStart = null;
            ulong chunkCount = 0;
            var chunks = new LinkedList<Tuple<ReadOnlyMemory<char>, int>>();
            ulong substituteLength = 0;

            /* Scan for CR characters, when encountered create a new chunk(non inclusive for CR).
             * Scan for LF characters, when encountered if not preceeded by a CR create a new chunk (non inclusive). */
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
                        substituteLength += (ulong)replace.Item2.Length;
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
                            if (!chunkStart.HasValue) chunkStart = Stream.Position;/* Start new chunk (if one isnt started yet) */
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
                                chunkStart = Stream.Position + 1;/* At chunk-start */
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
                    if (!chunkStart.HasValue) chunkStart = Stream.Position;

                    /* Push new chunk to our list */
                    var chunkSize = Stream.Position - chunkStart.Value;
                    var Mem = buffMem.Slice((int)chunkStart.Value, (int)chunkSize);
                    var chunk = new Tuple<ReadOnlyMemory<char>, int>(Mem, replacementIndex);
                    chunks.AddLast(chunk);

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

            ulong insertCount = (chunkCount - 1);/* Number of substitutes we will insert into new string */
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

                /* Insert substitute */
                var replaceIndex = tpl.Item2;
                var replaceTarget = data.Slice((int)index);
                var replaceSource = Replacements[replaceIndex].Item2;
                replaceSource.CopyTo(replaceTarget);
                index += (ulong)replaceSource.Length;
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
            DataStream<char> Stream = new DataStream<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            ulong newLength = 0;
            ulong? chunkStart = null;
            ulong chunkCount = 0;
            var chunks = new LinkedList<ReadOnlyMemory<char>>();

            /* Scan for CR characters, when encountered create a new chunk(non inclusive for CR).
             * Scan for LF characters, when encountered if not preceeded by a CR create a new chunk (non inclusive). */
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
                            if (!chunkStart.HasValue) chunkStart = Stream.Position;/* Start new chunk (if one isnt started yet) */
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
                                chunkStart = Stream.Position + 1;/* At chunk-start */
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
                    if (!chunkStart.HasValue) chunkStart = Stream.Position - 1;

                    /* Push new chunk to our list */
                    var chunkSize = (Stream.Position - 1) - chunkStart.Value;
                    chunks.AddLast(buffMem.Slice((int)chunkStart.Value, (int)chunkSize));

                    chunkCount++;
                    chunkStart = null;
                    newLength += chunkSize;
                }

            }

            /* Compile the new string */

            if (trim)
            {/* To trim we just need to skip any empty chunks at the beginning and end */
                while (chunks.First.Value.Length <= 0)
                {
                    chunks.RemoveFirst();
                    chunkCount--;
                }

                while (chunks.Last.Value.Length <= 0)
                {
                    chunks.RemoveLast();
                    chunkCount--;
                }
            }

            ulong substituteLength = (ulong)substituteData.Length;
            ulong insertCount = (chunkCount - 1);/* Number of substitutes we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);

            ulong index = 0;
            foreach (var chunk in chunks)
            {
                /* Copy substring */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

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
        public static string Replace(ReadOnlyMemory<char> buffMem, bool trim = false, params Tuple<DataFilter<char>, ReadOnlyMemory<char>>[] Replacements)
        {
            DataStream<char> Stream = new DataStream<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            ulong newLength = 0;
            ulong? chunkStart = null;
            ulong chunkCount = 0;
            var chunks = new LinkedList<Tuple<ReadOnlyMemory<char>, int>>();
            ulong substituteLength = 0;

            /* Scan for CR characters, when encountered create a new chunk(non inclusive for CR).
             * Scan for LF characters, when encountered if not preceeded by a CR create a new chunk (non inclusive). */
            while (!Stream.atEnd)
            {
                EFilterResult filterResult = EFilterResult.FILTER_ACCEPT;
                int replacementIndex = -1;

                for (int i=0; i<Replacements.Length; i++)
                {
                    var replace = Replacements[i];
                    filterResult = replace.Item1.acceptData(Stream.Next);
                    if (filterResult != EFilterResult.FILTER_ACCEPT)
                    {
                        replacementIndex = i;
                        substituteLength += (ulong)replace.Item2.Length;
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
                            if (!chunkStart.HasValue) chunkStart = Stream.Position;/* Start new chunk (if one isnt started yet) */
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
                                chunkStart = Stream.Position + 1;/* At chunk-start */
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
                    if (!chunkStart.HasValue) chunkStart = Stream.Position;

                    /* Push new chunk to our list */
                    var chunkSize = Stream.Position - chunkStart.Value;
                    var Mem = buffMem.Slice((int)chunkStart.Value, (int)chunkSize);
                    var chunk = new Tuple<ReadOnlyMemory<char>, int>(Mem, replacementIndex);
                    chunks.AddLast(chunk);

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

            ulong insertCount = (chunkCount - 1);/* Number of substitutes we will insert into new string */
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

                /* Insert substitute */
                var replaceIndex = tpl.Item2;
                var replaceTarget = data.Slice((int)index);
                var replaceSource = Replacements[replaceIndex].Item2;
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
            DataStream<char> Stream = new DataStream<char>(buffMem, UnicodeCommon.EOF);
            int Length = buffMem.Length;

            char[] dataPtr = new char[Length];
            Memory<char> data = new Memory<char>(dataPtr);
            var buff = data.Span;
            int idx = 0;

            while (!Stream.atEnd)
            {
                char ch = Transform(Stream.Next);
                if (ch != CHAR_NULL)
                {
                    buff[idx++] = ch;
                }

                Stream.Consume();
            }

            if (idx < Length)
            {/* The new string isnt quite as long as the old one */
                char[] finalDataPtr = new char[idx];
                Span<char> finalBuff = new Span<char>(finalDataPtr);

                buff.CopyTo(finalBuff);
                return new string(finalDataPtr);
            }

            return new string(dataPtr);
        }
        #endregion
    }
}
