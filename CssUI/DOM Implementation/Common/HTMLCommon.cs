using System;
using System.Runtime.CompilerServices;
using static CssUI.UnicodeCommon;

namespace CssUI.DOM.Internal
{
    /// <summary>
    /// Provides common functions outlined in the HTML specifications
    /// </summary>
    public static class HTMLCommon
    {
        #region Metadata
        /// <summary>
        /// Returns the official HTML namespace string
        /// </summary>
        public const string Namespace = "http://www.w3.org/1999/xhtml";

        private static readonly string[] FORBIDDEN_CUSTOM_ELEMENT_NAMES = 
        {
            "annotation-xml",
            "color-profile",
            "font-face",
            "font-face-src",
            "font-face-uri",
            "font-face-format",
            "font-face-name",
            "missing-glyph",
        };
        #endregion

        #region String Transformation
        public static string Uppercased_Name(string Name)
        {/* Docs: https://dom.spec.whatwg.org/#element-html-uppercased-qualified-name */

            char[] buf = new char[Name.Length];
            for (int c=0; c<buf.Length; c++)
            {
                buf[c] = ASCIICommon.To_ASCII_Upper_Alpha(Name[c]);
            }

            return new string(buf);
        }
        #endregion

        #region Internal Utilities
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_PCEN_Char(char c)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#prod-pcenchar */
            switch (c)
            {
                case CHAR_HYPHEN_MINUS:
                case CHAR_UNDERSCORE:
                case CHAR_FULL_STOP:
                    {
                        return true;
                    }
                default:
                    {
                        return (c >= CHAR_A_LOWER && c <= CHAR_Z_LOWER) || (c == 0xB7) || (c >= 0xC0 && c <= 0xD6) || (c >= 0xD8 && c <= 0xF6) || (c >= 0xF8 && c <= 0x37D) || (c >= 0x37F && c <= 0x1FFF) || (c >= 0x200C && c <= 0x200D) || (c >= 0x203F && c <= 0x2040) || (c >= 0x2070 && c <= 0x218F) || (c >= 0x2C00 && c <= 0x2FEF) || (c >= 0x3001 && c <= 0xD7FF) || (c >= 0xF900 && c <= 0xFDCF) || (c >= 0xFDF0 && c <= 0xFFFD) || (c >= 0x10000 && c <= 0xEFFFF);
                    }
            }
        }
        #endregion

        #region Validation

        public static bool Is_Valid_Custom_Element_Name(ReadOnlyMemory<char> data)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#valid-custom-element-name */
            DataStream<char> Stream = new DataStream<char>(data, EOF);
            /* FORMAT:  [a-z] (PCENChar)* '-' (PCENChar)*  */
            if (data.IsEmpty || data.Length < 2)
            {
                return false;
            }

            /* Must start with a lowercased alpha character */
            if (!ASCIICommon.Is_ASCII_Lower_Alpha(Stream.Next))
            {
                return false;
            }

            /* Consume the first char */
            Stream.Consume();

            /* Consume any chars that are valid PCEN but NOT the first hypen */
            Stream.Consume_While(c => Is_PCEN_Char(c) && c != CHAR_HYPHEN_MINUS);

            /* The name must contain atleast one hypen */
            if (Stream.Next != CHAR_HYPHEN_MINUS)
            {
                return false;
            }

            /* Consume all remaining PCEN chars */
            Stream.Consume_While(c => Is_PCEN_Char(c));

            /* All PCEN chars have been consumed, if this isnt EOF then its an invalid char */
            if (Stream.Next != EOF)
            {
                return false;
            }

            /* name must not be any of the following:
             *      annotation-xml
             *      color-profile
             *      font-face
             *      font-face-src
             *      font-face-uri
             *      font-face-format
             *      font-face-name
             *      missing-glyph 
             */
            for (int i=0; i< FORBIDDEN_CUSTOM_ELEMENT_NAMES.Length; i++)
            {
                if (StringCommon.streq(data.Span, FORBIDDEN_CUSTOM_ELEMENT_NAMES[i].AsSpan()))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
