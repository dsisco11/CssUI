using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static CssUI.UnicodeCommon;

namespace CssUI
{
    /// <summary>
    /// Provides common ASCII manipulation functions
    /// </summary>
    public static class ASCIICommon
    {

        #region Character Checks
        /// <summary>
        /// True if char is an ASCII whitespace character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Whitespace(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-whitespace */
            switch (c)
            {
                case CHAR_TAB:
                case CHAR_LINE_FEED:
                case CHAR_FORM_FEED:
                case CHAR_CARRIAGE_RETURN:
                case CHAR_SPACE:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// True if code point is an ASCII control character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Control(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-digit */
            return c >= CHAR_C0_DELETE && c <= CHAR_C0_APPLICATION_PROGRAM_COMMAND;
        }

        /// <summary>
        /// True if code point is an ASCII alphabet or digit character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Alphanumeric(char c)
        {
            return Is_Ascii_Digit(c) || Is_ASCII_Lower_Alpha(c) || Is_ASCII_Upper_Alpha(c);
        }

        /// <summary>
        /// True if code point is an ASCII digit character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Digit(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-digit */
            return c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9;
        }

        /// <summary>
        /// True if code point is an ASCII alphabet character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Alpha(char c)
        {
            return Is_ASCII_Lower_Alpha(c) || Is_ASCII_Upper_Alpha(c);
        }

        /// <summary>
        /// True if code point is ASCII alpha lowercase character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_ASCII_Lower_Alpha(char c)
        {
            return c >= CHAR_A_LOWER && c <= CHAR_Z_LOWER;
        }

        /// <summary>
        /// True if code point is ASCII alpha uppercase character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_ASCII_Upper_Alpha(char c)
        {
            return c >= CHAR_A_UPPER && c <= CHAR_Z_UPPER;
        }
        #endregion

        #region Character Transformation

        /// <summary>
        /// Converts an ASCII uppercase character to its lowecase form
        /// </summary>
        /// <param name="c">Code point to transform</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char To_ASCII_Lower_Alpha(char c)
        {
            if (!Is_ASCII_Upper_Alpha(c))
                return c;

            // Add to lowercase 'a' the distance that c is from uppercase 'A'
            return (char)(CHAR_A_LOWER + (c - CHAR_A_UPPER));
        }

        /// <summary>
        /// Converts an ASCII lowercase character to its uppercase form
        /// </summary>
        /// <param name="c">Code point to transform</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char To_ASCII_Upper_Alpha(char c)
        {
            if (!Is_ASCII_Lower_Alpha(c))
                return c;
            // Add to uppercase 'A' the distance that c is from lowercase 'a'
            return (char)(CHAR_A_UPPER + (c - CHAR_A_LOWER));
        }

        /// <summary>
        /// Converts an ASCII digit character to its numeric value
        /// </summary>
        /// <param name="c">Code point to convert</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ascii_Digit_To_Value(char c)
        {
            if (c < CHAR_DIGIT_0 || c > CHAR_DIGIT_9)
                throw new IndexOutOfRangeException();

            return c - CHAR_DIGIT_0;
        }
        #endregion

        #region String Checks

        /// <summary>
        /// True if string contains ASCII alpha lowercase characters
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has_ASCII_Lower_Alpha(ReadOnlySpan<char> str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Is_ASCII_Lower_Alpha(str[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True if string contains ASCII alpha uppercase characters
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has_ASCII_Upper_Alpha(ReadOnlySpan<char> str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Is_ASCII_Upper_Alpha(str[i]))
                    return true;
            }

            return false;
        }

        #endregion

        #region String Transformation

        public static string Strip_And_Collapse_Whitespace(ReadOnlyMemory<char> buffMem)
        {/* Docs: https://infra.spec.whatwg.org/#strip-and-collapse-ascii-whitespace */
            ReadOnlySpan<char> buff = buffMem.Span;
            /* Create a list of memory chunks that make up the final string */
            int newLength = 0;
            int chunkStart = -1;
            int chunkCount = 0;
            var chunks = new LinkedList<ReadOnlyMemory<char>>();

            for (int i=0; i<buff.Length; i++)
            {
                if (Is_Ascii_Whitespace(buff[i]))
                {
                    if (chunkStart > -1)
                    {/* This whitespace marks the end of a chunk, push it to our list */
                        int chunkLen = i - chunkStart;
                        chunks.AddLast(buffMem.Slice(chunkStart, chunkLen));

                        chunkCount++;
                        chunkStart = -1;
                        newLength += chunkLen;
                    }
                }
                else if (chunkStart == -1)
                {// this is the first char of a new segment
                    chunkStart = i;
                }
            }

            /* Compile the new string */
            int whiteSpaceCount = (chunkCount - 1);/* Number of whitespace chars we will insert into new string */
            newLength += whiteSpaceCount;
            char[] dataPtr = new char[newLength];
            Memory<char> data = new Memory<char>(dataPtr);
            string newStr = new string(dataPtr);

            int index = 0;
            foreach (var chunk in chunks)
            {
                /* Copy substring */
                chunk.CopyTo( data.Slice(index) );
                index += chunk.Length;
                /* Insert whitespace */
                data.Span[index] = CHAR_SPACE;
                index++;
            }

            return newStr;
        }
        #endregion
    }
}
