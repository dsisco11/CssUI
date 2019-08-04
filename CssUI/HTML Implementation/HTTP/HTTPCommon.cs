using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static CssUI.UnicodeCommon;

namespace CssUI.HTML
{
    public static class HTTPCommon
    {

        #region CodePoint Checks
        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML tab or space character
        /// </summary>
        /// <param name="c">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Tab_Or_Space(char c)
        {/* Docs: https://fetch.spec.whatwg.org/#http-tab-or-space */
            return c == CHAR_TAB || c == CHAR_SPACE;
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML whitespace character
        /// </summary>
        /// <param name="c">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Whitespace(char c)
        {/* Docs: https://fetch.spec.whatwg.org/#http-whitespace */
            return c == CHAR_LINE_FEED || c == CHAR_CARRIAGE_RETURN || Is_HTTP_Tab_Or_Space(c);
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML token character
        /// </summary>
        /// <param name="c">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Token(char c)
        {/* Docs: https://mimesniff.spec.whatwg.org/#http-token-code-point */
            if (Is_Ascii_Alphanumeric(c)) return true;

            switch (c)
            {
                case CHAR_EXCLAMATION_POINT:
                case CHAR_NUMBER_SIGN:
                case CHAR_DOLLAR_SIGN:
                case CHAR_PERCENT:
                case CHAR_AMPERSAND:
                case CHAR_COMMA:
                case CHAR_ASTERISK:
                case CHAR_PLUS_SIGN:
                case CHAR_HYPHEN_MINUS:
                case CHAR_FULL_STOP:
                case CHAR_CARET:
                case CHAR_UNDERSCORE:
                case CHAR_BACKTICK:
                case CHAR_PIPE:
                case CHAR_TILDE:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML quoted-string token character
        /// </summary>
        /// <param name="c">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Quoted_String_Token(char c)
        {/* Docs: https://mimesniff.spec.whatwg.org/#http-quoted-string-token-code-point */
            return (c == CHAR_TAB) || (c >= CHAR_SPACE && c <= CHAR_TILDE) || (c >= '\u0080' && c <= '\u00FF');
        }

        #endregion


        #region Byte Checks
        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML newline(0x0A or 0x0D) byte
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Newline_Byte(byte b)
        {/* Docs: https://fetch.spec.whatwg.org/#http-newline-byte */
            return b == 0x0A || b == 0x0D;
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML tab(0x09) or space(0x20) byte
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Tab_Or_Space_Byte(byte b)
        {/* Docs: https://fetch.spec.whatwg.org/#http-tab-or-space-byte */
            return b == 0x09 || b == 0x20;
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML whitespace byte
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_HTTP_Whitepace_Byte(byte b)
        {/* Docs: https://fetch.spec.whatwg.org/#http-whitespace-byte */
            return Is_HTTP_Newline_Byte(b) || Is_HTTP_Tab_Or_Space_Byte(b);
        }
        #endregion

        #region Consuming
        public static string Consume_HTTP_Quoted_String(DataStream<char> Stream, bool bExtract_Value_Flag = false)
        {/* Docs: https://fetch.spec.whatwg.org/#collect-an-http-quoted-string */

            var positionStart = Stream.Position;
            var sb = bExtract_Value_Flag ? new StringBuilder() : null;
            /* 3) Assert: the code point at position within input is U+0022 ("). */
            if (Stream.Next != CHAR_QUOTATION_MARK)
            {
                throw new DOM.Exceptions.DomSyntaxError($"Expected quotation mark @\"{ParsingCommon.Get_Location(Stream)}");
            }

            Stream.Consume();
            while (true)
            {
                /* 1) Append the result of collecting a sequence of code points that are not U+0022 (") or U+005C (\) from input, given position, to value. */
                Stream.Consume_While(c => c != CHAR_EQUALS && c != CHAR_REVERSE_SOLIDUS, out ReadOnlyMemory<char> outConsumed);
                if (sb != null) sb.Append(outConsumed.ToString());

                if (Stream.atEOF) break;

                var quoteOrBackslash = Stream.Next;
                Stream.Consume();
                if (quoteOrBackslash == CHAR_REVERSE_SOLIDUS)
                {
                    if (Stream.atEOF)
                    {
                        if (sb != null) sb.Append(CHAR_REVERSE_SOLIDUS);
                        break;
                    }

                    if (sb != null) sb.Append(Stream.Next);
                    Stream.Consume();
                }
                else
                {
                    /* 1) Assert: quoteOrBackslash is U+0022 ("). */
                    if (quoteOrBackslash != CHAR_QUOTATION_MARK)
                    {
                        throw new DOM.Exceptions.DomSyntaxError($"Expected quotation mark @\"{ParsingCommon.Get_Location(Stream)}");
                    }

                    break;
                }
            }

            if (bExtract_Value_Flag) return sb.ToString();

            /* 7) Return the code points from positionStart to position, inclusive, within input. */
            return Stream.AsMemory().Slice((int)positionStart, (int)(Stream.Position - positionStart)).ToString();
        }
        #endregion
    }
}
