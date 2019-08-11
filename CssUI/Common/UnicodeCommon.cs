using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CssUI
{
    public static class UnicodeCommon
    {/* https://en.wikipedia.org/wiki/List_of_Unicode_characters */
        #region Constants

        #region /* Specials */
        public const char CHAR_MAX = char.MaxValue;
        public const char EOF = '\u0000';
        public const char CHAR_NULL = '\u0000';
        public const char CHAR_REPLACEMENT = '\uFFFD';// U+FFFD
        #endregion


        #region /* Ascii whitespace */
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
        #endregion


        #region /* C0 Control codes */
        /* Docs: https://infra.spec.whatwg.org/#c0-control */
        /// <summary>
        /// C0 Control code: INFORMATION SEPARATOR ONE
        /// </summary>
        public const char CHAR_C0_INFO_SEPERATOR = '\u001F';
        public const char CHAR_C0_DELETE = '\u007F';
        public const char CHAR_C0_APPLICATION_PROGRAM_COMMAND = '\u009F';
        #endregion


        #region /* Ascii Symbols */

        /// <summary>
        /// !
        /// </summary>
        public const char CHAR_EXCLAMATION_POINT = '\u0021';
        /// <summary>
        /// ?
        /// </summary>
        public const char CHAR_QUESTION_MARK = '\u003F';
        /// <summary>
        /// @
        /// </summary>
        public const char CHAR_AT_SIGN = '\u0040';
        /// <summary>
        /// $
        /// </summary>
        public const char CHAR_DOLLAR_SIGN = '\u0024';
        /// <summary>
        /// &
        /// </summary>
        public const char CHAR_AMPERSAND = '\u0026';
        /// <summary>
        /// *
        /// </summary>
        public const char CHAR_ASTERISK = '\u002A';
        /// <summary>
        /// ^
        /// </summary>
        public const char CHAR_CARET = '\u005E';
        /// <summary>
        /// `
        /// </summary>
        public const char CHAR_BACKTICK = '\u0060';
        /// <summary>
        /// ~
        /// </summary>
        public const char CHAR_TILDE = '\u007E';
        /// <summary>
        /// |
        /// </summary>
        public const char CHAR_PIPE = '\u007C';
        /// <summary>
        /// =
        /// </summary>
        public const char CHAR_EQUALS = '\u003D';

        /// <summary>
        /// <
        /// </summary>
        public const char CHAR_LEFT_CHEVRON = '\u003C';
        /// <summary>
        /// >
        /// </summary>
        public const char CHAR_RIGHT_CHEVRON = '\u003E';

        /// <summary>
        /// {
        /// </summary>
        public const char CHAR_LEFT_CURLY_BRACKET = '\u007B';
        /// <summary>
        /// }
        /// </summary>
        public const char CHAR_RIGHT_CURLY_BRACKET = '\u007D';

        /// <summary>
        /// [
        /// </summary>
        public const char CHAR_LEFT_SQUARE_BRACKET = '\u005B';
        /// <summary>
        /// ]
        /// </summary>
        public const char CHAR_RIGHT_SQUARE_BRACKET = '\u005D';

        /// <summary>
        /// (
        /// </summary>
        public const char CHAR_LEFT_PARENTHESES = '\u0028';
        /// <summary>
        /// )
        /// </summary>
        public const char CHAR_RIGHT_PARENTHESES = '\u0029';
        #endregion


        #region /* Ascii digits */
        /* Docs: https://infra.spec.whatwg.org/#ascii-digit */
        public const char CHAR_DIGIT_0 = '\u0030';
        public const char CHAR_DIGIT_9 = '\u0039';
        #endregion


        #region /* Ascii Upper Alpha */
        /* Docs: https://infra.spec.whatwg.org/#ascii-upper-alpha */
        public const char CHAR_A_UPPER = '\u0041';
        public const char CHAR_B_UPPER = '\u0042';
        public const char CHAR_C_UPPER = '\u0043';
        public const char CHAR_D_UPPER = '\u0044';
        public const char CHAR_E_UPPER = '\u0045';
        public const char CHAR_F_UPPER = '\u0046';
        public const char CHAR_H_UPPER = '\u0048';
        public const char CHAR_M_UPPER = '\u004D';
        public const char CHAR_P_UPPER = '\u0050';
        public const char CHAR_S_UPPER = '\u0053';
        public const char CHAR_T_UPPER = '\u0054';
        public const char CHAR_W_UPPER = '\u0057';
        public const char CHAR_X_UPPER = '\u0058';
        public const char CHAR_Y_UPPER = '\u0059';
        public const char CHAR_Z_UPPER = '\u005A';
        #endregion


        #region /* Ascii Lower Alpha */
        /* Docs: https://infra.spec.whatwg.org/#ascii-lower-alpha */
        public const char CHAR_A_LOWER = '\u0061';
        public const char CHAR_B_LOWER = '\u0062';
        public const char CHAR_C_LOWER = '\u0063';
        public const char CHAR_D_LOWER = '\u0064';
        public const char CHAR_E_LOWER = '\u0065';
        public const char CHAR_F_LOWER = '\u0066';
        public const char CHAR_H_LOWER = '\u0068';
        public const char CHAR_M_LOWER = '\u006D';
        public const char CHAR_S_LOWER = '\u0073';
        public const char CHAR_W_LOWER = '\u0077';
        public const char CHAR_X_LOWER = '\u0078';
        public const char CHAR_Y_LOWER = '\u0079';
        public const char CHAR_Z_LOWER = '\u007A';
        #endregion


        #region /* Common */
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
        /// #
        /// </summary>
        public const char CHAR_HASH = '\u0023';

        /// <summary>
        /// ,
        /// </summary>
        public const char CHAR_COMMA = '\u002C';
        /// <summary>
        /// :
        /// </summary>
        public const char CHAR_COLON = '\u003A';
        /// <summary>
        /// ;
        /// </summary>
        public const char CHAR_SEMICOLON = '\u003B';

        /// <summary>
        /// &nbsp
        /// </summary>
        public const char CHAR_NBSP = '\u00A0';

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
        /// True if char is an ASCII non-character
        /// </summary>
        /// <param name="c">Code point to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_NonCharacter(char c)
        {/* Docs: https://infra.spec.whatwg.org/#noncharacter */

            if (c >= '\uFDD0' && c <= '\uFDEF') return true;

            switch ((int)c)
            {
                case 0xFFFE:
                case 0xFFFF:
                case 0x1FFFE:
                case 0x1FFFF:
                case 0x2FFFE:
                case 0x2FFFF:
                case 0x3FFFE:
                case 0x3FFFF:
                case 0x4FFFE:
                case 0x4FFFF:
                case 0x5FFFE:
                case 0x5FFFF:
                case 0x6FFFE:
                case 0x6FFFF:
                case 0x7FFFE:
                case 0x7FFFF:
                case 0x8FFFE:
                case 0x8FFFF:
                case 0x9FFFE:
                case 0x9FFFF:
                case 0xAFFFE:
                case 0xAFFFF:
                case 0xBFFFE:
                case 0xBFFFF:
                case 0xCFFFE:
                case 0xCFFFF:
                case 0xDFFFE:
                case 0xDFFFF:
                case 0xEFFFE:
                case 0xEFFFF:
                case 0xFFFFE:
                case 0xFFFFF:
                case 0x10FFFE:
                case 0x10FFFF:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// True if char is an ASCII whitespace character
        /// </summary>
        /// <param name="c">Code point to check</param>
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
        /// True if code point is an ASCII tab or newline character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Tab_Or_Newline(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-tab-or-newline */
            switch (c)
            {
                case CHAR_TAB:
                case CHAR_LINE_FEED:
                case CHAR_CARRIAGE_RETURN:
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
        {/* Docs: https://infra.spec.whatwg.org/#c0-control */
            return (c >= CHAR_C0_DELETE && c <= CHAR_C0_APPLICATION_PROGRAM_COMMAND);
        }

        /// <summary>
        /// True if code point is an ASCII control or space character
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Control_Or_Space(char c)
        {/* Docs: https://infra.spec.whatwg.org/#c0-control-or-space */
            return (c >= CHAR_C0_DELETE && c <= CHAR_C0_APPLICATION_PROGRAM_COMMAND) || c == CHAR_SPACE;
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


        /// <summary>
        /// True if code point is an ASCII hex-digit character (0-9 | a-f | A-F)
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Hex_Digit(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-digit */
            return (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9) || (c >= CHAR_A_LOWER && c <= CHAR_F_LOWER) || (c >= CHAR_A_UPPER && c <= CHAR_F_UPPER);
        }

        /// <summary>
        /// True if code point is an ASCII hex-digit character excluding uppercase alpha characters
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Hex_Digit_Lower(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-digit */
            return (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9) || (c >= CHAR_A_LOWER && c <= CHAR_F_LOWER);
        }

        /// <summary>
        /// True if code point is an ASCII hex-digit character excluding lowercase alpha characters
        /// </summary>
        /// <param name="c">Code point to check</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Ascii_Hex_Digit_Upper(char c)
        {/* Docs: https://infra.spec.whatwg.org/#ascii-digit */
            return (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9) || (c >= CHAR_A_UPPER && c <= CHAR_F_UPPER);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Selectable_Char(char c)
        {
            if (Is_Ascii_Control(c))
                return false;

            return true;
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

            return (int)c - CHAR_DIGIT_0;
        }

        #endregion


        #region Hexadecimal
        /// <summary>
        /// Converts an ASCII hexadecimal character to its numeric value
        /// </summary>
        /// <param name="c">Code point to convert</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ascii_Hex_To_Value(char c)
        {
            if (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9)
                return (int)c - CHAR_DIGIT_0;

            switch (c)
            {
                case CHAR_A_LOWER:
                case CHAR_A_UPPER:
                    return 10;
                case CHAR_B_LOWER:
                case CHAR_B_UPPER:
                    return 11;
                case CHAR_C_LOWER:
                case CHAR_C_UPPER:
                    return 12;
                case CHAR_D_LOWER:
                case CHAR_D_UPPER:
                    return 13;
                case CHAR_E_LOWER:
                case CHAR_E_UPPER:
                    return 14;
                case CHAR_F_LOWER:
                case CHAR_F_UPPER:
                    return 15;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Converts a numeric value to its ASCII hexadecimal character(s)
        /// </summary>
        /// <param name="codePoint">Code point to convert</param>
        /// <param name="Digits">Minimum number of digits to include</param>
        /// <returns>Characters representing the hexadecimal form of the given value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char[] Ascii_Value_To_Hex(char codePoint, UInt32 Digits = 0)
        {
            return Ascii_Value_To_Hex((UInt64)codePoint, Digits);
        }

        /// <summary>
        /// Converts a numeric value to its ASCII hexadecimal character(s)
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="Digits">Minimum number of digits to include</param>
        /// <returns>Characters representing the hexadecimal form of the given value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char[] Ascii_Value_To_Hex(Int32 value, UInt32 Digits = 0)
        {
            return Ascii_Value_To_Hex((UInt64)value, Digits);
        }

        /// <summary>
        /// Converts a numeric value to its ASCII hexadecimal character(s)
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="Digits">Minimum number of digits to include</param>
        /// <returns>Characters representing the hexadecimal form of the given value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char[] Ascii_Value_To_Hex(UInt32 value, UInt32 Digits = 0)
        {
            return Ascii_Value_To_Hex((UInt64)value, Digits);
        }

        /// <summary>
        /// Converts a numeric value to its ASCII hexadecimal character(s)
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="Digits">Minimum number of digits to include</param>
        /// <returns>Characters representing the hexadecimal form of the given value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char[] Ascii_Value_To_Hex(Int64 value, UInt32 Digits = 0)
        {
            return Ascii_Value_To_Hex((UInt64)value, Digits);
        }

        /// <summary>
        /// Converts a numeric value to its ASCII hexadecimal character(s)
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="Digits">Minimum number of digits to include</param>
        /// <returns>Characters representing the hexadecimal form of the given value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char[] Ascii_Value_To_Hex(UInt64 value, UInt32 Digits = 0)
        {
            /* To determine how many hexidecimal chars we will need to represent this value:
             * Find the index of the values most significant bit and divide it by 4and then round it up */
            int msb = (int)Math.Ceiling(BitScanner.BitScanForward(value) / 4.0d);
            int charCount = MathExt.Max(1, msb);
            char[] result = new char[MathExt.Max(charCount, Digits)];
            int padding = (int)((charCount > Digits) ? 0 : (Digits - charCount));

            for (int i=0; i<charCount; i++)
            {
                var halfByte = (value & 0xF);
                value = value >> 4;
                result[padding+i] = (char)((halfByte < 10) ? (halfByte + CHAR_DIGIT_0) : (halfByte + CHAR_A_UPPER));
            }

            for (int i = 0; i < padding; i++)
            {
                result[i] = CHAR_DIGIT_0;
            }

            return result;
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

        #region Encoding Sets
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Percent_Encode_Set_C0_Control(char c)
        {/* Docs: https://url.spec.whatwg.org/#c0-control-percent-encode-set */
            return (c >= CHAR_C0_DELETE && c <= CHAR_C0_APPLICATION_PROGRAM_COMMAND) || (c > CHAR_TILDE);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Percent_Encode_Set_Fragment(char c)
        {/* Docs: https://url.spec.whatwg.org/#fragment-percent-encode-set */
            if (Percent_Encode_Set_C0_Control(c)) return true;
            switch (c)
            {
                case CHAR_SPACE:
                case CHAR_QUOTATION_MARK:
                case CHAR_LEFT_CHEVRON:
                case CHAR_RIGHT_CHEVRON:
                case CHAR_BACKTICK:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Percent_Encode_Set_Path(char c)
        {/* Docs: https://url.spec.whatwg.org/#path-percent-encode-set */
            if (Percent_Encode_Set_Fragment(c)) return true;
            switch (c)
            {
                case CHAR_HASH:
                case CHAR_QUESTION_MARK:
                case CHAR_LEFT_CURLY_BRACKET:
                case CHAR_RIGHT_CURLY_BRACKET:
                    return true;
                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Percent_Encode_Set_Userinfo(char c)
        {/* Docs: https://url.spec.whatwg.org/#userinfo-percent-encode-set */
            if (Percent_Encode_Set_Path(c)) return true;
            switch (c)
            {
                case CHAR_SOLIDUS:
                case CHAR_COLON:
                case CHAR_SEMICOLON:
                case CHAR_EQUALS:
                case CHAR_AT_SIGN:
                case CHAR_LEFT_SQUARE_BRACKET:
                case CHAR_REVERSE_SOLIDUS:
                case CHAR_RIGHT_SQUARE_BRACKET:
                case CHAR_CARET:
                case CHAR_PIPE:
                    return true;
                default:
                    return false;
            }
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

        #region Percent Encoding
        public static char[] Percent_Encode(byte b)
        {/* Docs: https://url.spec.whatwg.org/#percent-encode */
            char[] hex = Ascii_Value_To_Hex((ulong)b, 2);
            return new char[3] { CHAR_PERCENT, hex[0], hex[1] };
        }

        public static byte[] Percent_Decode(ReadOnlyMemory<byte> input)
        {/* Docs: https://url.spec.whatwg.org/#percent-decode */
            DataStream<byte> Stream = new DataStream<byte>(input, byte.MinValue);
            /* Create a list of memory chunks that make up the final string */
            ulong newLength = 0;
            ulong? chunkStart = null;
            ulong chunkCount = 0;
            var chunks = new LinkedList<Tuple<ReadOnlyMemory<byte>, byte?>>();

            while (!Stream.atEnd)
            {
                EFilterResult filterResult = EFilterResult.FILTER_ACCEPT;
                byte? bytePoint = null;

                if (Stream.Next == CHAR_PERCENT && Is_Ascii_Hex_Digit((char)Stream.NextNext) && Is_Ascii_Hex_Digit((char)Stream.NextNextNext))
                {
                    filterResult = EFilterResult.FILTER_SKIP;
                    uint low = (uint)Ascii_Hex_To_Value((char)Stream.NextNext);
                    uint high = (uint)Ascii_Hex_To_Value((char)Stream.NextNextNext);
                    bytePoint = (byte)(low | (high >> 4));
                    Stream.Consume(2);
                    break;
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

                if (end_chunk || Stream.Remaining <= 1)
                {
                    if (!chunkStart.HasValue) chunkStart = Stream.Position;

                    /* Push new chunk to our list */
                    var chunkSize = Stream.Position - chunkStart.Value;
                    var Mem = Stream.AsMemory().Slice((int)chunkStart.Value, (int)chunkSize);
                    var chunk = new Tuple<ReadOnlyMemory<byte>, byte?>(Mem, bytePoint);
                    chunks.AddLast(chunk);

                    chunkCount++;
                    chunkStart = null;
                    newLength += chunkSize;
                    /* If we actually decoded a byte then account for it in the newLength */
                    if (filterResult != EFilterResult.FILTER_ACCEPT) newLength++;
                }

                Stream.Consume();
            }

            /* Compile the string */
            var dataPtr = new byte[newLength];
            Memory<byte> data = new Memory<byte>(dataPtr);

            ulong index = 0;
            foreach (var tpl in chunks)
            {
                var chunk = tpl.Item1;
                /* Copy chunk data */
                chunk.CopyTo(data.Slice((int)index));
                index += (ulong)chunk.Length;

                if (tpl.Item2.HasValue)
                {
                    data.Span[(int)index] = tpl.Item2.Value;
                    index++;
                }
            }

            return dataPtr;
        }

        public static string UTF8_Percent_Encode(char codePoint, Predicate<char> percentEncodePredicate)
        {/* Docs: https://url.spec.whatwg.org/#utf-8-percent-encode */
            if (!percentEncodePredicate(codePoint)) return codePoint.ToString();

            var bytes = Encoding.UTF8.GetBytes(new char[1] { codePoint });
            char[] data = new char[bytes.Length * 3];

            for (int i=0; i<bytes.Length; i++)
            {
                var enc = Percent_Encode(bytes[i]);
                int j = i * 3;
                data[j + 0] = enc[0];
                data[j + 1] = enc[1];
                data[j + 2] = enc[2];
            }

            return new string(data);
        }
        #endregion

    }
}
