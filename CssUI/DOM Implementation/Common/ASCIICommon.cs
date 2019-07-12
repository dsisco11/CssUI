using System.Runtime.CompilerServices;

namespace CssUI.DOM
{
    /// <summary>
    /// Provides common ASCII manipulation functions
    /// </summary>
    public static class ASCIICommon
    {
        /// <summary>
        /// True id char is an ASCII whitespace character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Whitespace(char c)
        {
            switch (c)
            {
                case '\u0009':
                case '\u000A':
                case '\u000C':
                case '\u000D':
                case '\u0020':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// True if char is ASCII alpha lowercase character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_ASCII_Lower_Alpha(char c)
        {
            return (c >= 'a' && c <= 'z');
        }

        /// <summary>
        /// True if char is ASCII alpha uppercase character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_ASCII_Upper_Alpha(char c)
        {
            return (c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// True if string contains ASCII alpha lowercase characters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has_ASCII_Lower_Alpha(string str)
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
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Has_ASCII_Upper_Alpha(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Is_ASCII_Upper_Alpha(str[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts an ASCII uppercase character to its lowecase form
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char To_ASCII_Lower_Alpha(char c)
        {
            if (!Is_ASCII_Upper_Alpha(c))
                return c;

            return (char)('a' + (c - 'A'));
        }

        /// <summary>
        /// Converts an ASCII lowercase character to its uppercase form
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char To_ASCII_Upper_Alpha(char c)
        {
            if (!Is_ASCII_Lower_Alpha(c))
                return c;

            return (char)('A' + (c - 'a'));
        }
    }
}
