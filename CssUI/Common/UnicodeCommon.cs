using System;

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


    }
}
