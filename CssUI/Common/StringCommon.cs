using CssUI.Filters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI
{
    public static class StringCommon
    {
        /// <summary>
        /// Transforms a <c>string</c> into a byte-array
        /// </summary>
        /// <returns>Byte-array containing the string data</returns>
        public static byte[] ToByteArray(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        /// <summary>
        /// Returns whether <paramref name="str1"/> and <paramref name="str2"/> contain the same values
        /// </summary>
        /// <returns>True if both strings are an exact match</returns>
        public static bool streq(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
        {
            if (str1.Length != str2.Length)
                return false;

            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[i])
                {
                    return false;
                }
            }

            return true;
        }



        /// <summary>
        /// Replaces every occurrence of U+000D (CR) not followed by U+000A (LF), and every occurrence of U+000A (LF) not preceded by U+000D (CR), by a string consisting of a U+000D (CR) and U+000A (LF).
        /// </summary>
        /// <param name="buffMem">String memory</param>
        /// <returns>Altered string</returns>
        public static string Replace(ReadOnlyMemory<char> buffMem, DataFilter<DataStream<char>> dataFilter, ReadOnlySpan<char> substituteData)
        {
            DataStream<char> Stream = new DataStream<char>(buffMem, UnicodeCommon.EOF);
            /* Create a list of memory chunks that make up the final string */
            int newLength = 0;
            int? chunkStart = null;
            int chunkCount = 0;
            var chunks = new LinkedList<ReadOnlyMemory<char>>();

            /* Scan for CR characters, when encountered create a new chunk(non inclusive for CR).
             * Scan for LF characters, when encountered if not preceeded by a CR create a new chunk (non inclusive). */
            while (!Stream.atEOF)
            {
                EFilterResult filterResult = dataFilter.acceptData(Stream);
                /* When filter result:
                 * ACCEPT: Char should be included in chunk
                 * SKIP: Char should not be included in chunk, if at chunk-start shift chunk-start past char, otherwise end chunk
                 * REJECT: Char should not be included in chunk, current chunk ends
                 */
                bool end_chunk = false;
                switch (filterResult)
                {
                    case EFilterResult.FILTER_ACCEPT:// Char should be included in the chunk
                        {// Start new chunk (if one isnt started yet)
                            if (!chunkStart.HasValue)
                            {
                                chunkStart = Stream.Position;
                            }
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
                            {// At chunk-start
                                chunkStart = Stream.Position + 1;
                            }
                            else
                            {
                                end_chunk = true;
                            }
                        }
                        break;
                }

                if (end_chunk)
                {
                    if (!chunkStart.HasValue)
                    {
                        chunkStart = Stream.Position;
                    }

                    /* Push new chunk to our list */

                    int chunkSize = Stream.Position - chunkStart.Value;
                    chunks.AddLast(buffMem.Slice(chunkStart.Value, chunkSize));

                    chunkCount++;
                    chunkStart = null;
                    newLength += chunkSize;
                }

                Stream.Consume();
            }

            /* Compile the new string */
            int substituteLength = substituteData.Length;
            int insertCount = (chunkCount - 1);/* Number of substitutes we will insert into new string */
            newLength += insertCount * substituteLength;

            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            int index = 0;
            foreach (var chunk in chunks)
            {
                /* Copy substring */
                chunk.CopyTo(data.Slice(index));
                index += chunk.Length;

                /* Insert substitute */
                substituteData.CopyTo(data.Slice(index).Span);
                index += substituteLength;
            }

            return newStr;
        }
    }
}
