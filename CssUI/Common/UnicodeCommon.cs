using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI
{
    public static class UnicodeCommon
    {
        #region Constants
        /* Specials */
        public const char CHAR_MAX = char.MaxValue;
        public const char EOF = '\u0000';
        public const char CHAR_NULL = '\u0000';
        public const char CHAR_REPLACEMENT = '\uFFFD';// U+FFFD



        /* Ascii whitespace */
        /* Docs: https://infra.spec.whatwg.org/#ascii-whitespace */
        public const char CHAR_TAB = '\u0009';
        /// <summary>
        /// Line Feed (Newline)
        /// </summary>
        public const char CHAR_LINE_FEED = '\u000A';
        /// <summary>
        /// Form Feed
        /// </summary>
        public const char CHAR_FORM_FEED = '\u000C';
        /// <summary>
        /// CR
        /// </summary>
        public const char CHAR_CARRIAGE_RETURN = '\u000D';
        /// <summary>
        /// " "
        /// </summary>
        public const char CHAR_SPACE = '\u0020';

        [Obsolete("Use CHAR_LINE_FEED")]
        public const char CHAR_NEWLINE = '\n';


        /* C0 Control codes */
        /* Docs: https://infra.spec.whatwg.org/#c0-control */
        /// <summary>
        /// C0 Control code: INFORMATION SEPARATOR ONE
        /// </summary>
        public const char CHAR_C0_INFO_SEPERATOR = '\u001F';
        public const char CHAR_C0_DELETE = '\u007F';
        public const char CHAR_C0_APPLICATION_PROGRAM_COMMAND = '\u009F';


        /* Ascii digits */
        /* Docs: https://infra.spec.whatwg.org/#ascii-digit */
        public const char CHAR_DIGIT_0 = '\u0030';
        public const char CHAR_DIGIT_9 = '\u0039';


        /* Ascii Upper Alpha */
        /* Docs: https://infra.spec.whatwg.org/#ascii-upper-alpha */
        public const char CHAR_A_UPPER = '\u0041';
        public const char CHAR_F_UPPER = '\u0046';
        public const char CHAR_Z_UPPER = '\u005A';


        /* Ascii Lower Alpha */
        /* Docs: https://infra.spec.whatwg.org/#ascii-lower-alpha */
        public const char CHAR_A_LOWER = '\u0061';
        public const char CHAR_F_LOWER = '\u0066';
        public const char CHAR_X_LOWER = '\u0078';
        public const char CHAR_Y_LOWER = '\u0079';
        public const char CHAR_Z_LOWER = '\u007A';


        /* Common */
        /// <summary>
        /// "
        /// </summary>
        public const char CHAR_QUOTATION_MARK = '\u0022';
        /// <summary>
        /// '
        /// </summary>
        public const char CHAR_APOSTRAPHE = '\u0027';
        /// <summary>
        /// +
        /// </summary>
        public const char CHAR_PLUS_SIGN = '\u002B';
        /// <summary>
        /// %
        /// </summary>
        public const char CHAR_PERCENT = '\u0025';
        /// <summary>
        /// -
        /// </summary>
        public const char CHAR_HYPHEN_MINUS = '\u002D';
        /// <summary>
        /// _
        /// </summary>
        public const char CHAR_UNDERSCORE = '\u005F';
        /// <summary>
        /// .
        /// </summary>
        public const char CHAR_FULL_STOP = '\u002E';
        /// <summary>
        /// /
        /// </summary>
        public const char CHAR_SOLIDUS = '/';
        /// <summary>
        /// \
        /// </summary>
        public const char CHAR_REVERSE_SOLIDUS = '\u005C';

        /// <summary>
        /// ,
        /// </summary>
        public const char CHAR_COMMA = '\u002C';
        /// <summary>
        /// :
        /// </summary>
        public const char CHAR_COLON = '\u003A';

        /// <summary>
        /// (
        /// </summary>
        public const char CHAR_PARENTHESES_OPEN = '\u0028';

        /// <summary>
        /// )
        /// </summary>
        public const char CHAR_PARENTHESES_CLOSE = '\u0029';

        /// <summary>
        /// Same as: <see cref="CHAR_SOLIDUS"/>
        /// </summary>
        [Obsolete("Use CHAR_SOLIDUS")]
        public const char CHAR_SLASH = '/';
        /// <summary>
        /// Same as: <see cref="CHAR_REVERSE_SOLIDUS"/>
        /// </summary>
        [Obsolete("Use CHAR_REVERSE_SOLIDUS")]
        public const char CHAR_BACKSLASH = '\u005C';

        #endregion


        #region Modifier Keys
        /* These are the character representations for common keys */
        public const char KEY_CTRL_MODIFIER = '⌃';
        public const char KEY_ALT_MODIFIER = '⌥';
        public const char KEY_SHIFT_MODIFIER = '⇧';
        public const char KEY_META_MODIFIER = '⌘';

        public const char KEY_ESCAPE = '⎋';
        public const char KEY_BACKSPACE = '⌫';
        public const char KEY_CAPSLOCK = '⇪';
        public const char KEY_ENTER = '↵';
        public const char KEY_TAB = '⇥';
        public const char KEY_SPACE = ' ';

        public const char KEY_DELETE = '⌦';
        public const char KEY_END = '↘';
        public const char KEY_INSERT = '↖';
        public const char KEY_HOME = '↖';
        public const char KEY_PGDOWN = '⇟';
        public const char KEY_PGUP = '⇞';

        public const char KEY_UP = '↑';
        public const char KEY_RIGHT = '→';
        public const char KEY_DOWN = '↓';
        public const char KEY_LEFT = '←';

        #endregion



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

        #region Unicode String Transformations

        /// <summary>
        /// Converts a given string into unicode scalar values
        /// </summary>
        /// <param name="stringMem"></param>
        /// <returns></returns>
        public static string Convert_To_Scalar_Values(ReadOnlyMemory<char> stringMem)
        {/* Docs: https://heycam.github.io/webidl/#dfn-obtain-unicode */
            /* XXX: Optimize this */

            var S = stringMem.Span;
            int n = stringMem.Length;
            int i = 0;
            StringBuilder U = new StringBuilder();

            for(;i<n; i++)
            {
                char c = S[i];
                if (c < 0xD800 || c > 0xDFFF)
                {/* Append to U the Unicode character with code point c. */
                    U.Append(c);
                }
                else if (0xDc00 <= c && c <= 0xDFFF)
                {/* Append to U a U+FFFD REPLACEMENT CHARACTER. */
                    U.Append(CHAR_REPLACEMENT);
                }
                else if (0xD800 <= c && c <= 0xDBFF)
                {
                    /* 1) If i = n−1, then append to U a U+FFFD REPLACEMENT CHARACTER. */
                    if (i == n-1)
                    {
                        U.Append(CHAR_REPLACEMENT);
                    }
                    else /* Otherwise, i < n−1: */
                    {
                        char d = S[i + 1];
                        if (0xDC00 <= d && d <= 0xDFFF)
                        {
                            /* 1) Let a be c & 0x3FF. */
                            int a = c & 0x3FF;
                            /* 2) Let b be d & 0x3FF. */
                            int b = d & 0x3FF;
                            /* 3) Append to U the Unicode character with code point 2^16+2^10a+b. */
                            const int x = 65536;// 2^16
                            const int y = 1024;// 2^10
                            char z = (char)(x + (y * a) + b);

                            U.Append(z);
                            /* 4) Set i to i+1. */
                            i = i + 1;
                        }
                        else if (d < 0xDC00 || d > 0xDFFF)
                        {
                            U.Append(CHAR_REPLACEMENT);
                        }
                    }
                }
            }


            return U.ToString();
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Selectable_Char(char c)
        {
            if (Is_Ascii_Control(c))
                return false;

            return true;
        }

    }
}
