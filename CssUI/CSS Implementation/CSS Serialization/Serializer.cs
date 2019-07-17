using System;
using System.Linq;
using System.Text;
using CssUI.CSS.Media;
using System.Collections.Generic;

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
            string keyword = CssLookup.Keyword_From_Enum<Ty>(ident);
            return Identifier(keyword);
        }

        public static string Identifier(string ident)
        {/* Docs: https://drafts.csswg.org/cssom/#serialize-an-identifier */
            if (ident == null)
                return null;

            StringBuilder sb = new StringBuilder(64);
            var characters = ident.ToCharArray();
            for (int i=0; i<characters.Length; i++)
            {
                char c = characters[i];
                if (c == UnicodeCommon.CHAR_NULL)
                {
                    sb.Append(UnicodeCommon.CHAR_REPLACEMENT);
                }
                else if ((c >= '\u0001' && c <= '\u001F') || c == '\u007F')
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (i == 0 && (c >= '\u0030' && c<= '\u0039'))// First char in range [0-9]
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (i == 1 && (c >= '\u0030' && c<= '\u0039') && characters[0] == '-')// Second char in range [0-9] & First char is '-'
                {
                    sb.Append(Escape_Code_Point(c));
                }
                else if (i == 0 && c == UnicodeCommon.CHAR_DASH && characters.Length < 2)// First char is '-' & no second char
                {
                    sb.Append(Escape_Character(c));
                }
                else if (c >= '\u0080' || c == '-' || c == '_' || (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
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


        #region Media
        public static string MediaQuery(MediaQuery query)
        {/* Docs: https://drafts.csswg.org/cssom/#serialize-a-media-query */
            StringBuilder sb = new StringBuilder();
            if (query.Modifier == EMediaQueryModifier.Not)
            {
                sb.Append("not\u0020");
            }

            string type = Identifier<EMediaType>(query.MediaType);
            /* 3) If the media query does not contain media features append type, to s, then return s. */
            if (query.Conditions.Count() <= 0)
            {
                sb.Append(type);
                return sb.ToString();
            }

            /* 4) If type is not "all" or if the media query is negated append type, followed by a single SPACE (U+0020), followed by "and", followed by a single SPACE (U+0020), to s. */
            if (query.MediaType != EMediaType.All || query.Modifier == EMediaQueryModifier.Not)
            {
                sb.Append(type);
                sb.Append("\u0020and\u0020");
            }

            /* 5) Sort the media features in lexicographical order. */
            List<MediaFeature> features = query.Conditions.OrderBy(o => CssLookup.Keyword_From_Enum(o.Name)).ToList();
            /* 6) Then, for each media feature: */
            for (int i=0; i<features.Count; i++)
            {
                MediaFeature feature = features[i];
                /* 1) Append a "(" (U+0028), followed by the media feature name, converted to ASCII lowercase, to s. */
                sb.Append('\u0028');
                sb.Append(CssLookup.Keyword_From_Enum(feature.Name));
                /* 2) If a value is given append a ":" (U+003A), followed by a single SPACE (U+0020), followed by the serialized media feature value, to s. */
                if (!ReferenceEquals(null, feature.Value))
                {
                    sb.Append("\u003A\u0020");
                    sb.Append(feature.Value.ToString());
                }
                /* 3) Append a ")" (U+0029) to s. */
                sb.Append('\u0029');
                /* 4) If this is not the last media feature append a single SPACE (U+0020), followed by "and", followed by a single SPACE (U+0020), to s. */
                if (i < (features.Count-1))
                {
                    sb.Append("\u0020and\u0020");
                }
            }

            return sb.ToString();
        }

        public static string MediaQueryList(IEnumerable<MediaQuery> queries)
        {
            IEnumerable<string> queryList = queries.Select(q => Serializer.MediaQuery(q));
            return Serialize_Comma_List(queryList);
        }
        #endregion

    }
}
