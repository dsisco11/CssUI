using CssUI.DOM;
using CssUI.DOM.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static CssUI.UnicodeCommon;

namespace CssUI.HTML
{
    /// <summary>
    /// Provides common functions outlined in the HTML specifications
    /// </summary>
    public static partial class HTMLCommon
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
        static IdnMapping IDN = new IdnMapping();
        public static bool Is_Valid_Punycode(string unicode)
        {
            if (ReferenceEquals(null, unicode))
                throw new ArgumentNullException(nameof(unicode));

            try
            {
                To_Punycode(unicode);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
        public static string To_Punycode(string unicode) => IDN.GetAscii(unicode);
        public static string From_Punycode(string code) => IDN.GetUnicode(code);
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
                        return c >= CHAR_A_LOWER && c <= CHAR_Z_LOWER || c == 0xB7 || c >= 0xC0 && c <= 0xD6 || c >= 0xD8 && c <= 0xF6 || c >= 0xF8 && c <= 0x37D || c >= 0x37F && c <= 0x1FFF || c >= 0x200C && c <= 0x200D || c >= 0x203F && c <= 0x2040 || c >= 0x2070 && c <= 0x218F || c >= 0x2C00 && c <= 0x2FEF || c >= 0x3001 && c <= 0xD7FF || c >= 0xF900 && c <= 0xFDCF || c >= 0xFDF0 && c <= 0xFFFD || c >= 0x10000 && c <= 0xEFFFF;
                    }
            }
        }
        #endregion


        #region Byte Checks
        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML newline(0x0A or 0x0D) byte
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTML_Newline_Byte(byte b)
        {
            return b == 0x0A || b == 0x0D;
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML tab(0x09) or space(0x20) byte
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTML_Tab_Or_Space_Byte(byte b)
        {
            return b == 0x09 || b == 0x20;
        }

        #endregion


        #region Validation
        static Regex email_validation_regex = new Regex(@"/^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/", RegexOptions.ECMAScript | RegexOptions.Compiled);
        public static bool Is_Valid_Email(string email)
        {
            return email_validation_regex.IsMatch(email);
        }

        public static bool Is_Valid_Browsing_Context_Name(ReadOnlyMemory<char> data)
        {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#valid-browsing-context-name */
            if (data.Span[0] == CHAR_UNDERSCORE)
            {
                return false;
            }

            return true;
        }

        public static bool Is_Valid_Browsing_Context_Keyword(ReadOnlyMemory<char> data)
        {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#valid-browsing-context-name */
            return Lookup.Is_Declared<EBrowsingTarget>(data.ToString());
        }

        public static bool Is_Valid_Browsing_Context_Name_Or_Keyword(ReadOnlyMemory<char> data)
        {/* Docs: https://html.spec.whatwg.org/multipage/browsers.html#valid-browsing-context-name */
            return Is_Valid_Browsing_Context_Name(data) || Is_Valid_Browsing_Context_Keyword(data);
        }

        public static bool Is_Valid_Custom_Element_Name(ReadOnlyMemory<char> data)
        {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#valid-custom-element-name */
            DataStream<char> Stream = new DataStream<char>(data, EOF);
            /* FORMAT:  [a-z] (PCENChar)* '-' (PCENChar)*  */
            if (data.IsEmpty || data.Length < 2)
            {
                return false;
            }

            /* Must start with a lowercased alpha character */
            if (!Is_ASCII_Lower_Alpha(Stream.Next))
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
            for (int i = 0; i < FORBIDDEN_CUSTOM_ELEMENT_NAMES.Length; i++)
            {
                if (StringCommon.StrEq(data.Span, FORBIDDEN_CUSTOM_ELEMENT_NAMES[i].AsSpan()))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion


        #region Attribute Resolution
        public static EAutofillMantle Get_Autofill_Mantle(Element element)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#autofill-anchor-mantle */
            /* On an input element whose type attribute is in the Hidden state, the autocomplete attribute wears the autofill anchor mantle. In all other cases, it wears the autofill expectation mantle. */
            if (element is HTMLInputElement inputElement && inputElement.type == EInputType.Hidden)
            {
                return EAutofillMantle.Anchor;
            }

            return EAutofillMantle.Expectation;
        }

        /// <summary>
        /// Resolves an elements IDL-exposed autofill value
        /// </summary>
        /// <param name="element"></param>
        /// <param name="outFieldName">The autofill field name specifies the specific kind of data expected in the field, e.g. "street-address" or "cc-exp".</param>
        /// <param name="outHint">The autofill hint set identifies what address or contact information type the user agent is to look at, e.g. "shipping fax" or "billing".</param>
        /// <param name="outScope">The autofill scope identifies the group of fields whose information concerns the same subject, and consists of the autofill hint set with, if applicable, the "section-*" prefix, e.g. "billing", "section-parent shipping", or "section-child shipping home".</param>
        public static void Resolve_Autofill(FormAssociatedElement element, out EAutofill outFieldName, out IReadOnlyList<string> outHint, out IReadOnlyList<string> outScope, out string outidlValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/form-control-infrastructure.html#autofill-processing-model */

            AttributeValue autocomplete = element.getAttribute(EAttributeName.Autocomplete);
            EAutofillMantle afMantle = Get_Autofill_Mantle(element);
            if (autocomplete != null)
            {
                /* 2) Let tokens be the result of splitting the attribute's value on ASCII whitespace. */
                //var tokens = DOMCommon.Parse_Ordered_Set(autocomplete.Get_String().AsMemory());
                string acString = StringCommon.Transform(autocomplete.Get_String().AsMemory(), To_ASCII_Lower_Alpha);
                var tokenList = new List<string>(DOMCommon.Parse_Ordered_Set(acString.AsMemory()).Select(o => o.ToString()));
                tokenList.Reverse();
                DataStream<string> Stream = new DataStream<string>(tokenList.ToArray(), null);

                /* 3) If tokens is empty, then jump to the step labeled default. */
                if (Stream.Length > 0)
                {
                    /* 4) Let index be the index of the last token in tokens. */
                    //int index = tokens.Count - 1;

                    /* 5) If the indexth token in tokens is not an ASCII case-insensitive match for one of the tokens given in the first column of the following table, 
                     * or if the number of tokens in tokens is greater than the maximum number given in the cell in the second column of that token's row, then jump to the step labeled default. 
                     * Otherwise, let field be the string given in the cell of the first column of the matching row, and let category be the value of the cell in the third column of that same row. */

                    //var key = tokens[index];
                    string key = Stream.Next;
                    EAutofill afValue = Lookup.Enum<EAutofill>(key);
                    EnumData afData = Lookup.Data(afValue);
                    int maxTokens = afData[0];
                    EAutofillCategory afCategory = afData[1];

                    /* ...if the number of tokens in tokens is greater than the maximum number given in the cell in the second column of that token's row, then jump to the step labeled default. */
                    if (Stream.Length <= maxTokens)
                    {
                        var field = afValue;
                        var category = afCategory;

                        /* 6) If category is Off or Automatic but the element's autocomplete attribute is wearing the autofill anchor mantle, then jump to the step labeled default. */
                        if ((category == EAutofillCategory.Off || category == EAutofillCategory.Automatic) && afMantle == EAutofillMantle.Anchor)
                        {/* "Jump" to default */
                        }
                        else
                        {
                            /* 7) If category is Off, let the element's autofill field name be the string "off", let its autofill hint set be empty, and let its IDL-exposed autofill value be the string "off". Then, return. */
                            if (category == EAutofillCategory.Off)
                            {
                                outFieldName = EAutofill.Off;
                                outHint = new string[0];
                                outScope = new string[0];
                                outidlValue = "off";
                                return;
                            }

                            /* 8) If category is Automatic, let the element's autofill field name be the string "on", let its autofill hint set be empty, and let its IDL-exposed autofill value be the string "on". Then, return. */
                            if (category == EAutofillCategory.Automatic)
                            {
                                outFieldName = EAutofill.On;
                                outHint = new string[0];
                                outScope = new string[0];
                                outidlValue = "on";
                                return;
                            }

                            /* 9) Let scope tokens be an empty list. */
                            LinkedList<string> scopeTokens = new LinkedList<string>();
                            /* 10) Let hint tokens be an empty set. */
                            LinkedList<string> hintTokens = new LinkedList<string>();
                            /* 11) Let IDL value have the same value as field. */
                            string idlValue = Lookup.Keyword(field);

                            /* 12) If the indexth token in tokens is the first entry, then skip to the step labeled done. */
                            //if (index == 0)
                            if (Stream.Remaining > 1)
                            {
                                /* 13) Decrement index by one. */
                                //index--;
                                Stream.Consume();
                                /* 14) If category is Contact and the indexth token in tokens is an ASCII case-insensitive match for one of the strings in the following list, then run the substeps that follow: */
                                if (category == EAutofillCategory.Contact)
                                {
                                    if (Lookup.TryEnum(Stream.Next, out EAutofillContact hint))
                                    {
                                        /* 1) Let contact be the matching string from the list above. */
                                        string contact = Stream.Next;
                                        /* 2) Insert contact at the start of scope tokens. */
                                        scopeTokens.AddFirst(contact);
                                        /* 3) Add contact to hint tokens. */
                                        hintTokens.AddLast(contact);
                                        /* 4) Let IDL value be the concatenation of contact, a U+0020 SPACE character, and the previous value of IDL value (which at this point will always be field). */
                                        idlValue = string.Concat(Stream.Next, CHAR_SPACE, idlValue);
                                        /* 5) If the indexth entry in tokens is the first entry, then skip to the step labeled done. */
                                        if (Stream.Remaining > 1)
                                        {
                                            /* 6) Decrement index by one. */
                                            //index--;
                                            Stream.Consume();
                                        }
                                    }
                                }
                                /* 15) If the indexth token in tokens is an ASCII case-insensitive match for one of the strings in the following list, then run the substeps that follow: */
                                if (Lookup.Is_Declared<EAutofillMode>(Stream.Next))
                                {
                                    /* 1) Let mode be the matching string from the list above. */
                                    var mode = Stream.Next;
                                    /* 2) Insert mode at the start of scope tokens. */
                                    scopeTokens.AddFirst(mode);
                                    /* 3) Add mode to hint tokens. */
                                    hintTokens.AddLast(mode);
                                    /* 4) Let IDL value be the concatenation of mode, a U+0020 SPACE character, and the previous value of IDL value (which at this point will either be field or the concatenation of contact, a space, and field). */
                                    idlValue = string.Concat(Stream.Next, CHAR_SPACE, idlValue);
                                    /* 5) If the indexth entry in tokens is the first entry, then skip to the step labeled done. */
                                    if (Stream.Remaining > 1)
                                    {
                                        /* 6) Decrement index by one. */
                                        //index--;
                                        Stream.Consume();
                                    }
                                }
                            }

                            /* 16) If the indexth entry in tokens is not the first entry, then jump to the step labeled default. */
                            if (Stream.Remaining == 1)
                            {
                                /* 17) If the first eight characters of the indexth token in tokens are not an ASCII case-insensitive match for the string "section-", then jump to the step labeled default. */
                                if (Stream.Next.StartsWith("section-"))
                                {
                                    /* 18) Let section be the indexth token in tokens, converted to ASCII lowercase. */
                                    string section = Stream.Next;/* Already in ascii lowercase */
                                    /* 19) Insert section at the start of scope tokens. */
                                    scopeTokens.AddFirst(section);
                                    /* 20) Let IDL value be the concatenation of section, a U+0020 SPACE character, and the previous value of IDL value. */
                                    idlValue = string.Concat(Stream.Next, CHAR_SPACE, idlValue);
                                }


                                /* 21) Done: Let the element's autofill hint set be hint tokens. */
                                outHint = hintTokens.ToArray();

                                /* 22) Let the element's autofill scope be scope tokens. */
                                outScope = scopeTokens.ToArray();

                                /* 23) Let the element's autofill field name be field. */
                                outFieldName = field;

                                /* 24) Let the element's IDL-exposed autofill value be IDL value. */
                                outidlValue = idlValue;

                                /* 25) Return. */
                                return;
                            }
                        }
                    }
                }
            }

            /* 26) Default: Let the element's IDL-exposed autofill value be the empty string, and its autofill hint set and autofill scope be empty. */
            outidlValue = string.Empty;
            outHint = new string[0];
            outScope = new string[0];

            /* 27) If the element's autocomplete attribute is wearing the autofill anchor mantle, then let the element's autofill field name be the empty string and return. */
            if (afMantle == EAutofillMantle.Anchor)
            {
                outFieldName = EAutofill.EMPTY;
                return;
            }

            /* 28) Let form be the element's form owner, if any, or null otherwise. */
            var form = element.form;

            /* 29) If form is not null and form's autocomplete attribute is in the off state, then let the element's autofill field name be "off".
             * Otherwise, let the element's autofill field name be "on". */

            if (form != null && autocomplete != null && autocomplete.Get_String().Equals("off"))
            {
                outFieldName = EAutofill.Off;
            }
            else
            {
                outFieldName = EAutofill.On;
            }
        }
        #endregion
    }
}
