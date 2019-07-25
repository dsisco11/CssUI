using System;
using System.Linq;
using System.Text;
using CssUI.CSS.Media;
using System.Collections.Generic;

using static CssUI.UnicodeCommon;

namespace CssUI.CSS.Serialization
{
    /// <summary>
    /// Provides serialization logic for CSS data structures
    /// </summary>
    public static class Serializer
    {
        #region Utility
        public static string Escape_Character(char c) => string.Concat(UnicodeCommon.CHAR_REVERSE_SOLIDUS, c);
        public static string Escape_Code_Point(char c) => string.Concat(UnicodeCommon.CHAR_REVERSE_SOLIDUS, string.Format("{0:x4}", (int)c), UnicodeCommon.CHAR_SPACE);
        #endregion

        #region Common
        public static string Serialize_URL(string url) => string.Concat("url(", Serialize_String(url), ")");
        public static string Serialize_LOCAL(string local) => string.Concat("local(", Serialize_String(local), ")");
        public static string Serialize_Comma_List(IEnumerable<string> list) => string.Join("\u002C\u0020", list);
        public static string Serialize_Whitespace_List(IEnumerable<string> list) => string.Join("\u0020", list);

        public static string Serialize_String(string str)
        {/* Docs: https://drafts.csswg.org/cssom/#serialize-a-string */
            StringBuilder sb = new StringBuilder(str.Length + 2);
            sb.Append(UnicodeCommon.CHAR_QUOTATION_MARK);
            foreach (char c in str)
            {
                if (c == UnicodeCommon.CHAR_NULL)
                {
                    sb.Append(UnicodeCommon.CHAR_REPLACEMENT);
                }
                else if ((c >= '\u0001' && c <= '\u001F') || c == '\u007F')
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (c == UnicodeCommon.CHAR_QUOTATION_MARK || c == UnicodeCommon.CHAR_BACKSLASH)
                {
                    sb.Append(Escape_Character(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            sb.Append(UnicodeCommon.CHAR_QUOTATION_MARK);
            return sb.ToString();
        }
        #endregion

        #region Identifiers
        public static string Identifier<Ty>(Ty ident) where Ty : struct
        {
            string keyword = CssLookup.Keyword<Ty>(ident);
            return Identifier(keyword);
        }

        public static string Identifier(string ident)
        {/* Docs: https://drafts.csswg.org/cssom/#serialize-an-identifier */
            if (ident == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(64);
            var characters = ident.ToCharArray();
            for (int i=0; i<characters.Length; i++)
            {
                char c = characters[i];
                if (c == CHAR_NULL)
                {
                    sb.Append(CHAR_REPLACEMENT);
                }
                else if ((c >= '\u0001' && c <= '\u001F') || c == '\u007F')
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (i == 0 && (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9))// First char in range [0-9]
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (i == 1 && (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9) && characters[0] == CHAR_HYPHEN_MINUS)// Second char in range [0-9] & First char is '-'
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (i == 0 && c == CHAR_HYPHEN_MINUS && characters.Length < 2)// First char is '-' & no second char
                {
                    sb.Append(Escape_Character(c));
                }
                else if (c >= '\u0080' || c == CHAR_HYPHEN_MINUS || c == CHAR_UNDERSCORE || (c >= CHAR_DIGIT_0 && c <= CHAR_DIGIT_9) || (c >= CHAR_A_LOWER && c <= CHAR_Z_LOWER) || (c >= CHAR_A_UPPER && c <= CHAR_Z_UPPER))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(Escape_Character(c));
                }
            }

            return sb.ToString();
        }
        #endregion


    }
}
