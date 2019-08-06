using CssUI.DOM.Exceptions;
using CssUI.Filters;
using CssUI.HTML;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using static CssUI.UnicodeCommon;

namespace CssUI.DOM
{
    public static class MimeType
    {/* Docs: https://mimesniff.spec.whatwg.org/#understanding-mime-types */

        #region Constructors

        public static MimeTypeRecord CreateRecord(EMimeType mime)
        {
            string mimeString = Lookup.Keyword(mime);
            return CreateRecord(mimeString);
        }

        public static MimeTypeRecord CreateRecord(string mimeStr)
        {
            if (!Parse(mimeStr.AsMemory(), out MimeTypeRecord record))
            {
                throw new DOMException($"Invalid MIME type: \"{mimeStr}\" is not a recognized mime type!");
            }

            return record;
        }

        public static MimeTypeRecord CreateRecord(string Type, string SubType, Dictionary<string, string> Parameters = null)
        {
            return new MimeTypeRecord(Type, SubType, Parameters);
        }
        #endregion

        #region Mime Subtype definitions
        static string[] FONT_APPLICATION_MIME_SUBTYPES = new string[] 
        {
            "font-cff",
            "font-off",
            "font-sfnt",
            "font-ttf",
            "font-woff",
            "vnd.ms-fontobject",
            "vnd.ms-opentype",
        };
        static string[] ARCHIVE_APPLICATION_MIME_SUBTYPES = new string[] 
        {
            "x-rar-compressed",
            "zip",
            "x-gzip",
        };
        static string[] JAVASCRIPT_APPLICATION_MIME_SUBTYPES = new string[]
        {
            "ecmascript",
            "javascript",
            "x-ecmascript",
            "x-javascript",
        };
        static string[] JAVASCRIPT_TEXT_MIME_SUBTYPES = new string[] 
        {
            "ecmascript",
            "javascript",
            "javascript1.0",
            "javascript1.1",
            "javascript1.2",
            "javascript1.3",
            "javascript1.4",
            "javascript1.5",
            "jscript",
            "livescript",
            "x-ecmascript",
            "x-javascript",
        };

        public static EMimeGroup Determine_Groups(string Type, ReadOnlyMemory<char> SubType)
        {
            EMimeGroup Groups = 0x0;

            switch (Type)
            {
                case "image":
                    Groups |= EMimeGroup.Image;
                    break;
                case "audio":
                    Groups |= EMimeGroup.Audio;
                    break;
                case "video":
                    Groups |= EMimeGroup.Video;
                    break;
                case "font":
                    Groups |= EMimeGroup.Font;
                    break;
                case "text":
                    {
                        if (SubType.Span.Equals("xml".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.XML | EMimeGroup.Scriptable;
                        else if (SubType.Span.Equals("html".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.HTML | EMimeGroup.Scriptable;
                        else if (SubType.Span.Equals("json".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.JSON;
                        else
                        {
                            for(int i=0; i < JAVASCRIPT_TEXT_MIME_SUBTYPES.Length; i++)
                            {
                                if (SubType.Span.Equals(JAVASCRIPT_TEXT_MIME_SUBTYPES[i].AsSpan(), StringComparison.OrdinalIgnoreCase))
                                {
                                    Groups |= EMimeGroup.Javascript;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case "application":
                    {
                        if (SubType.Span.Equals("zip".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.Zip | EMimeGroup.Archive;
                        else if (SubType.Span.Equals("ogg".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.Audio;
                        else if (SubType.Span.Equals("xml".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.XML | EMimeGroup.Scriptable;
                        else if (SubType.Span.Equals("pdf".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.Scriptable;
                        else if (SubType.Span.Equals("json".AsSpan(), StringComparison.OrdinalIgnoreCase))
                            Groups |= EMimeGroup.JSON;
                        else
                        {
                            bool found = false;
                            for (int i = 0; i < FONT_APPLICATION_MIME_SUBTYPES.Length; i++)
                            {
                                if (SubType.Span.Equals(JAVASCRIPT_APPLICATION_MIME_SUBTYPES[i].AsSpan(), StringComparison.OrdinalIgnoreCase))
                                {
                                    Groups |= EMimeGroup.Font;
                                    found = true;
                                    break;
                                }
                            }
                            if (found) break;

                            for (int i = 0; i < ARCHIVE_APPLICATION_MIME_SUBTYPES.Length; i++)
                            {
                                if (SubType.Span.Equals(JAVASCRIPT_APPLICATION_MIME_SUBTYPES[i].AsSpan(), StringComparison.OrdinalIgnoreCase))
                                {
                                    Groups |= EMimeGroup.Archive;
                                    found = true;
                                    break;
                                }
                            }
                            for (int i = 0; i < JAVASCRIPT_APPLICATION_MIME_SUBTYPES.Length; i++)
                            {
                                if (SubType.Span.Equals(JAVASCRIPT_APPLICATION_MIME_SUBTYPES[i].AsSpan(), StringComparison.OrdinalIgnoreCase))
                                {
                                    Groups |= EMimeGroup.Javascript;
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }

            if (SubType.Span.EndsWith("+xml".AsSpan()))
                Groups |= EMimeGroup.XML | EMimeGroup.Scriptable;
            else if (SubType.Span.EndsWith("+json".AsSpan()))
                Groups |= EMimeGroup.JSON;


            return Groups;
        }
        #endregion

        #region Byte Checks

        /// <summary>
        /// Returns <c>True</c> if the given code point is one of the HTML binary data bytes
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Binary_Data_Byte(byte b)
        {/* Docs: https://mimesniff.spec.whatwg.org/#binary-data-byte */
            return (b >= 0x0 && b <= 0x08) || (b == 0x0B) || (b >= 0x0E && b <= 0x1A) || (b >= 0x1C && b <= 0x1F);
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is an HTML whitespace byte
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Whitepace_Byte(byte b)
        {/* Docs: https://mimesniff.spec.whatwg.org/#whitespace-byte */
            return (b == 0x09) || (b == 0x0A) || (b == 0x0C) || (b == 0x0D) || (b == 0x20);
        }

        /// <summary>
        /// Returns <c>True</c> if the given code point is one of the HTML tag-terminating bytes
        /// </summary>
        /// <param name="b">The character to check</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Tag_Terminating_Byte(byte b)
        {/* Docs: https://mimesniff.spec.whatwg.org/#tag-terminating-byte */
            return (b == 0x20 || b == 0x3E);
        }
        #endregion

        #region Parsing
        /// <summary>
        /// Extracts a MIME type/subtype from parsing a MIME string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outType"></param>
        /// <param name="outSubType"></param>
        /// <returns></returns>
        public static bool Parse(ReadOnlyMemory<char> input, out MimeTypeRecord outMimeType)
        {/* Docs:  */
            input = StringCommon.Trim(input, FilterHTMLWhitespace.Instance);
            var Stream = new DataStream<char>(input, EOF);

            if (!Stream.Consume_While(c => c != CHAR_SOLIDUS, out ReadOnlyMemory<char> Type) || !StringCommon.Contains(Type.Span, c => !HTTPCommon.Is_HTTP_Token(c)))
            {
                outMimeType = default;
                return false;
            }

            if (Stream.atEnd)
            {
                outMimeType = default;
                return false;
            }

            Stream.Consume();/* Consume the solidus(/) */

            if (!Stream.Consume_While(c => c != CHAR_SEMICOLON, out ReadOnlyMemory<char> Subtype))
            {
                outMimeType = default;
                return false;
            }

            /* 8) Remove any trailing HTTP whitespace from subtype. */
            Subtype = StringCommon.TrimEnd(Subtype, FilterHTMLWhitespace.Instance);
            /* 9) If subtype is the empty string or does not solely contain HTTP token code points, then return failure. */
            if (StringCommon.Contains(Subtype.Span, c => !HTTPCommon.Is_HTTP_Token(c)))
            {
                outMimeType = default;
                return false;
            }
            var Parameters = new Dictionary<string, string>();

            /* 11) While position is not past the end of input: */
            while (!Stream.atEnd)
            {
                /* 1) Advance position by 1. (This skips past U+003B (;).) */
                Stream.Consume();
                /* 2) Collect a sequence of code points that are HTTP whitespace from input given position. */
                Stream.Consume_While(HTTPCommon.Is_HTTP_Whitespace);

                Stream.Consume_While(c => c != CHAR_SEMICOLON && c != CHAR_EQUALS, out ReadOnlyMemory<char> paramName);

                string parameterName = StringCommon.Transform(paramName, To_ASCII_Lower_Alpha);
                string parameterValue = null;

                if (!Stream.atEnd)
                {
                    if (Stream.Next == CHAR_SEMICOLON)
                        continue;

                    Stream.Consume();/* Skip '=' sign */
                }

                if (Stream.atEnd)
                    break;

                if (Stream.Next == CHAR_QUOTATION_MARK)
                {
                    parameterValue = HTTPCommon.Consume_HTTP_Quoted_String(Stream, true);

                    Stream.Consume_While(c => c != CHAR_SEMICOLON, out ReadOnlyMemory<char> _);
                }
                else
                {
                    if (Stream.Consume_While(c => c != CHAR_SEMICOLON, out ReadOnlyMemory<char> paramVal))
                    {
                        parameterValue = StringCommon.TrimEnd(paramVal, HTTPCommon.Is_HTTP_Whitespace).ToString();

                        if (parameterValue.Length <= 0)
                            continue;
                    }
                }

                /* 10) If all of the following are true */
                if (!ReferenceEquals(null, parameterName) && parameterName.Length > 0 && !StringCommon.Contains(parameterName.AsSpan(), c => !HTTPCommon.Is_HTTP_Token(c)))
                {
                    if (!StringCommon.Contains(parameterValue.AsSpan(), c => !HTTPCommon.Is_HTTP_Quoted_String_Token(c)))
                    {
                        if (!Parameters.ContainsKey(parameterName))
                        {
                            Parameters.Add(parameterName, parameterValue);
                        }
                    }
                }
            }

            outMimeType = new MimeTypeRecord(Type.ToString(), Subtype.ToString(), Parameters);
            return true;
        }
        #endregion

        #region Serializing
        public static bool Serialize(MimeTypeRecord mimeType, out string outSerialized)
        {/* Docs: https://mimesniff.spec.whatwg.org/#serialize-a-mime-type */

            var sb = new StringBuilder();
            sb.Append(mimeType.Type);
            sb.Append(CHAR_SOLIDUS);
            sb.Append(mimeType.SubType);

            foreach (KeyValuePair<string, string> kv in mimeType.Parameters)
            {
                sb.Append(CHAR_SEMICOLON);
                sb.Append(kv.Key);
                sb.Append(CHAR_EQUALS);

                string valueStr = kv.Value;
                /* 4) If value does not solely contain HTTP token code points or value is the empty string, then: */
                if (StringCommon.Contains(kv.Value.AsSpan(), c => !HTTPCommon.Is_HTTP_Token(c)))
                {
                    var escapedQuote = new ReadOnlyMemory<char>(new char[2] { CHAR_REVERSE_SOLIDUS, CHAR_QUOTATION_MARK });
                    var escapedSlash = new ReadOnlyMemory<char>(new char[2] { CHAR_REVERSE_SOLIDUS, CHAR_REVERSE_SOLIDUS });
                    string escapedStr = StringCommon.Replace(kv.Value.AsMemory(), false, 
                        new Tuple<char, ReadOnlyMemory<char>>(CHAR_QUOTATION_MARK, escapedQuote), 
                        new Tuple<char, ReadOnlyMemory<char>>(CHAR_REVERSE_SOLIDUS, escapedSlash));

                    valueStr = string.Concat(CHAR_QUOTATION_MARK, escapedStr, CHAR_QUOTATION_MARK);
                }

                sb.Append(valueStr);
            }

            outSerialized = sb.ToString();
            return true;
        }
        #endregion

        #region Header Patterns
        static ReadOnlyMemory<byte> WHITESPACE_BYTES = new ReadOnlyMemory<byte>(new byte[] { 0x09, 0x0A, 0x0C, 0x0D, 0x20 });


        #region /* HTML */

        static ReadOnlyMemory<byte> PATTERN_HTMLDOC = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x21, 0x44, 0x4F, 0x43, 0x54, 0x59, 0x50, 0x45, 0x20, 0x48, 0x54, 0x4D, 0x4C });
        static ReadOnlyMemory<byte> MASK_HTMLDOC =    new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF, 0xFF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_HTML = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x48, 0x54, 0x4D, 0x4C });/* The case-insensitive string "<HTML" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_HTML = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_HEAD = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x48, 0x45, 0x41, 0x44 });/* The case-insensitive string "<HEAD" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_HEAD = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_SCRIPT = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x53, 0x43, 0x52, 0x49, 0x50, 0x54 });/* The case-insensitive string "<SCRIPT" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_SCRIPT = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_IFRAME = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x49, 0x46, 0x52, 0x41, 0x4D, 0x45 });/* The case-insensitive string "<IFRAME" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_IFRAME = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_H1 = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x48, 0x31 });/* The case-insensitive string "<H1" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_H1 = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_TAG_DIV = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x44, 0x49, 0x56 });/* The case-insensitive string "<DIV" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_DIV = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_FONT = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x46, 0x4F, 0x4E, 0x54 });/* The case-insensitive string "<FONT" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_FONT = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_TABLE = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x54, 0x41, 0x42, 0x4C, 0x45 });/* 	The case-insensitive string "<TABLE" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_TABLE = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_A = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x41 });/* The case-insensitive string "<A" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_A = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_STYLE = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x53, 0x54, 0x59, 0x4C, 0x45 });/* The case-insensitive string "<STYLE" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_STYLE = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_TITLE = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x54, 0x49, 0x54, 0x4C, 0x45 });/* The case-insensitive string "<TITLE" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_TITLE = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_B = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x42 });/* The case-insensitive string "<B" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_B = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_BODY = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x42, 0x4F, 0x44, 0x59 });/* The case-insensitive string "<BODY" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_BODY = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_BR = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x42, 0x52 });/* The case-insensitive string "<BR" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_BR = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_P = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x50 });/* The case-insensitive string "<P" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_P = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xDF });

        static ReadOnlyMemory<byte> PATTERN_TAG_HTML_COMMENT = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x21, 0x2D, 0x2D });/* The string "<!--" followed by a tag-terminating byte. */
        static ReadOnlyMemory<byte> MASK_TAG_HTML_COMMENT = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_TAG_XML = new ReadOnlyMemory<byte>(new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C });/* The string "<?xml". */
        static ReadOnlyMemory<byte> MASK_TAG_XML = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_PDF = new ReadOnlyMemory<byte>(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D });/* The string "%PDF-", the PDF signature. */
        static ReadOnlyMemory<byte> MASK_PDF    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        #endregion


        #region /* MISC */

        static ReadOnlyMemory<byte> PATTERN_ADOBE_POSTSCRIPT = new ReadOnlyMemory<byte>(new byte[] { 0x25, 0x21, 0x50, 0x53, 0x2D, 0x41, 0x64, 0x6F, 0x62, 0x65, 0x2D });/* The string "%!PS-Adobe-", the PostScript signature. */
        static ReadOnlyMemory<byte> MASK_ADOBE_POSTSCRIPT    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_UTF16BE_BOM = new ReadOnlyMemory<byte>(new byte[] { 0xFE, 0xFF, 0x00, 0x00 });/* UTF-16BE BOM */
        static ReadOnlyMemory<byte> MASK_UTF16BE_BOM    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0x00, 0x00 });

        static ReadOnlyMemory<byte> PATTERN_UTF16LE_BOM = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFE, 0x00, 0x00 });/* UTF-16LE BOM */
        static ReadOnlyMemory<byte> MASK_UTF16LE_BOM    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0x00, 0x00 });

        static ReadOnlyMemory<byte> PATTERN_UTF8_BOM = new ReadOnlyMemory<byte>(new byte[] { 0xEF, 0xBB, 0xBF, 0x00 });/* UTF-8 BOM */
        static ReadOnlyMemory<byte> MASK_UTF8_BOM    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0x00 });
        #endregion


        #region /* ARCHIVE */

        static ReadOnlyMemory<byte> PATTERN_GZIP = new ReadOnlyMemory<byte>(new byte[] { 0x1F, 0x8B, 0x08 });/* The GZIP archive signature. */
        static ReadOnlyMemory<byte> MASK_GZIP    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_ZIP = new ReadOnlyMemory<byte>(new byte[] { 0x50, 0x4B, 0x03, 0x04 });/* The string "PK" followed by ETX EOT, the ZIP archive signature. */
        static ReadOnlyMemory<byte> MASK_ZIP    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_RAR = new ReadOnlyMemory<byte>(new byte[] { 0x52, 0x61, 0x72, 0x20, 0x1A, 0x07, 0x00 });/* The string "Rar " followed by SUB BEL NUL, the RAR archive signature. */
        static ReadOnlyMemory<byte> MASK_RAR    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
        #endregion


        #region /* FONT */

        static ReadOnlyMemory<byte> PATTERN_OPENTYPE_EMBEDDED = new ReadOnlyMemory<byte>(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x50 });/* 34 bytes followed by the string "LP", the Embedded OpenType signature. */
        static ReadOnlyMemory<byte> MASK_OPENTYPE_EMBEDDED    = new ReadOnlyMemory<byte>(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_TRUETYPE = new ReadOnlyMemory<byte>(new byte[] { 0x00, 0x01, 0x00, 0x00 });/* 4 bytes representing the version number 1.0, a TrueType signature. */
        static ReadOnlyMemory<byte> MASK_TRUETYPE    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_OPENTYPE = new ReadOnlyMemory<byte>(new byte[] { 0x4F, 0x54, 0x54, 0x4F });/* The string "OTTO", the OpenType signature. */
        static ReadOnlyMemory<byte> MASK_OPENTYPE    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_TRUETYPE_COLLECTION = new ReadOnlyMemory<byte>(new byte[] { 0x74, 0x74, 0x63, 0x66 });/* The string "ttcf", the TrueType Collection signature. */
        static ReadOnlyMemory<byte> MASK_TRUETYPE_COLLECTION    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_WEB_OPEN_FONT_FORMAT_V1 = new ReadOnlyMemory<byte>(new byte[] { 0x77, 0x4F, 0x46, 0x46 });/* The string "wOFF", the Web Open Font Format 1.0 signature. */
        static ReadOnlyMemory<byte> MASK_WEB_OPEN_FONT_FORMAT_V1    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_WEB_OPEN_FONT_FORMAT_V2 = new ReadOnlyMemory<byte>(new byte[] { 0x77, 0x4F, 0x46, 0x32 });/* The string "wOF2", the Web Open Font Format 2.0 signature. */
        static ReadOnlyMemory<byte> MASK_WEB_OPEN_FONT_FORMAT_V2    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
        #endregion


        #region /* IMAGES */

        static ReadOnlyMemory<byte> PATTERN_WINDOWS_ICON = new ReadOnlyMemory<byte>(new byte[] { 0x00, 0x00, 0x01, 0x00 });/* A Windows Icon signature. */
        static ReadOnlyMemory<byte> MASK_WINDOWS_ICON    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_WINDOWS_CURSOR = new ReadOnlyMemory<byte>(new byte[] { 0x00, 0x00, 0x02, 0x00 });/* A Windows Cursor signature. */
        static ReadOnlyMemory<byte> MASK_WINDOWS_CURSOR    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_IMAGE_BMP = new ReadOnlyMemory<byte>(new byte[] { 0x42, 0x4D });/* The string "BM", a BMP signature. */
        static ReadOnlyMemory<byte> MASK_IMAGE_BMP    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_IMAGE_GIF87a = new ReadOnlyMemory<byte>(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 });/* The string "GIF87a", a GIF signature. */
        static ReadOnlyMemory<byte> MASK_IMAGE_GIF87a    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_IMAGE_GIF89a = new ReadOnlyMemory<byte>(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 });/* The string "GIF89a", a GIF signature. */
        static ReadOnlyMemory<byte> MASK_IMAGE_GIF89a    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_IMAGE_WEBP = new ReadOnlyMemory<byte>(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50, 0x56, 0x50 });/* The string "RIFF" followed by four bytes followed by the string "WEBPVP". */
        static ReadOnlyMemory<byte> MASK_IMAGE_WEBP    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_IMAGE_PNG = new ReadOnlyMemory<byte>(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });/* An error-checking byte followed by the string "PNG" followed by CR LF SUB LF, the PNG signature. */
        static ReadOnlyMemory<byte> MASK_IMAGE_PNG    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_IMAGE_JPEG = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xD8, 0xFF });/* The JPEG Start of Image marker followed by the indicator byte of another marker. */
        static ReadOnlyMemory<byte> MASK_IMAGE_JPEG    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF });
        #endregion


        #region /* AUDIO / VIDEO */

        static ReadOnlyMemory<byte> PATTERN_SND = new ReadOnlyMemory<byte>(new byte[] { 0x2E, 0x73, 0x6E, 0x64 });/* The string ".snd", the basic audio signature. */
        static ReadOnlyMemory<byte> MASK_SND    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_AIFF = new ReadOnlyMemory<byte>(new byte[] { 0x46, 0x4F, 0x52, 0x4D, 0x00, 0x00, 0x00, 0x00, 0x41, 0x49, 0x46, 0x46 });/* The string "FORM" followed by four bytes followed by the string "AIFF", the AIFF signature. */
        static ReadOnlyMemory<byte> MASK_AIFF    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_MP3 = new ReadOnlyMemory<byte>(new byte[] { 0x49, 0x44, 0x33 });/* The string "ID3", the ID3v2-tagged MP3 signature. */
        static ReadOnlyMemory<byte> MASK_MP3    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_OGG = new ReadOnlyMemory<byte>(new byte[] { 0x4F, 0x67, 0x67, 0x53, 0x00 });/* The string "OggS" followed by NUL, the Ogg container signature. */
        static ReadOnlyMemory<byte> MASK_OGG    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_MIDI = new ReadOnlyMemory<byte>(new byte[] { 0x4D, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06 });/* The string "MThd" followed by four bytes representing the number 6 in 32 bits (big-endian), the MIDI signature. */
        static ReadOnlyMemory<byte> MASK_MIDI    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_AVI = new ReadOnlyMemory<byte>(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x41, 0x56, 0x49, 0x20 });/* The string "RIFF" followed by four bytes followed by the string "AVI ", the AVI signature. */
        static ReadOnlyMemory<byte> MASK_AVI    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF });

        static ReadOnlyMemory<byte> PATTERN_WAVE = new ReadOnlyMemory<byte>(new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x41, 0x56, 0x45 });/* The string "RIFF" followed by four bytes followed by the string "WAVE", the WAVE signature. */
        static ReadOnlyMemory<byte> MASK_WAVE    = new ReadOnlyMemory<byte>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF });
        #endregion


        //static ReadOnlyMemory<byte> PATTERN_ = new ReadOnlyMemory<byte>(new byte[] {  });/*  */
        //static ReadOnlyMemory<byte> MASK_    = new ReadOnlyMemory<byte>(new byte[] {  });
        #endregion

        #region Sniffing
        /// <summary>
        /// Extracts a MIME type/subtype by reading header data from binary file data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="outType"></param>
        /// <param name="outSubType"></param>
        /// <returns></returns>
        public static bool Sniff(ReadOnlyMemory<byte> Header, string suppliedMimeType, out string outMIME, bool bNoSniff, bool bCheckApacheBug)
        {/* Docs: https://mimesniff.spec.whatwg.org/#mime-type-sniffing-algorithm */

            if (suppliedMimeType.Equals("unknown/unknown") || suppliedMimeType.Equals("application/unknown") || suppliedMimeType.Equals("*/*"))
            {
                var mimeType = Identify_Unknown_MIME_Type(Header, !bNoSniff);
                outMIME = Lookup.Keyword(mimeType);
                return true;
            }

            if (bNoSniff)
            {
                outMIME = suppliedMimeType;
                return true;
            }

            if (bCheckApacheBug)
            {
                if (Header.Length >= 2)
                {
                    if (PatternMatch(Header, PATTERN_UTF16BE_BOM, MASK_UTF16BE_BOM, null, true)) { outMIME = Lookup.Keyword(EMimeType.Plain); return true; }
                    if (PatternMatch(Header, PATTERN_UTF16LE_BOM, MASK_UTF16LE_BOM, null, true)) { outMIME = Lookup.Keyword(EMimeType.Plain); return true; }
                }

                if (Header.Length >= 3)
                {
                    if (PatternMatch(Header, PATTERN_UTF8_BOM, MASK_UTF8_BOM, null, true)) { outMIME = Lookup.Keyword(EMimeType.Plain); return true; }
                }

                bool foundBinaryDataByte = false;
                for (int i = 0; i < Header.Length; i++)
                {
                    byte b = Header.Span[i];
                    if (Is_Binary_Data_Byte(b))
                    {
                        foundBinaryDataByte = true;
                        break;
                    }
                }

                if (!foundBinaryDataByte) { outMIME = Lookup.Keyword(EMimeType.Plain); return true; }

                outMIME = Lookup.Keyword(EMimeType.OctetStream);
                return true;
            }

            string[] spl = suppliedMimeType.Split(CHAR_REVERSE_SOLIDUS);
            var groups = Determine_Groups(spl[0], spl[1].AsMemory());
            if (0 != (groups & EMimeGroup.XML))
            {
                outMIME = suppliedMimeType;
                return true;
            }

            if (suppliedMimeType.Equals("text/html", StringComparison.OrdinalIgnoreCase))
            {
                /* XXX: Implement feed sniffing */
                /* https://mimesniff.spec.whatwg.org/#sniffing-a-mislabeled-feed */
                outMIME = Identify_Mislabeled_Feed(Header, suppliedMimeType);
                return true;
            }

            if (0 != (groups & EMimeGroup.Image))
            {
                if (Identify_Image_MIME_Type(Header, out EMimeType outImageMIME))
                {
                    outMIME = Lookup.Keyword(outImageMIME);
                    return true;
                }
            }

            if (0 != (groups & (EMimeGroup.Video | EMimeGroup.Audio)))
            {
                if (Identify_Audio_Or_Video_MIME_Type(Header, out EMimeType outVideoMIME))
                {
                    outMIME = Lookup.Keyword(outVideoMIME);
                    return true;
                }
            }

            outMIME = suppliedMimeType;
            return false;
        }
        #endregion

        #region Mislabeled Feed Identification
        static byte[] seq_comment_begin = new byte[] { 0x21, 0x2D, 0x2D };/* !-- */
        static byte[] seq_comment_end = new byte[] { 0x2D, 0x2D, 0x3E };/* --> */
        static byte[] seq_code_tag_end = new byte[] { 0x3F, 0x3E };/* ?> */
        static byte[] seq_rss = new byte[] { 0x72, 0x73, 0x73 };/* rss */
        static byte[] seq_feed = new byte[] { 0x66, 0x65, 0x65, 0x64 };/* feed */
        static byte[] seq_rdf = new byte[] { 0x72, 0x64, 0x66, 0x3A, 0x52, 0x44, 0x46 };/* rdf:RDF */
        static byte[] seq_purl_namespace = new byte[] { 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x70, 0x75, 0x72, 0x6C, 0x2E, 0x6F, 0x72, 0x67, 0x2F, 0x72, 0x73, 0x73, 0x2F, 0x31, 0x2E, 0x30, 0x2F };/* http://purl.org/rss/1.0/ */
        static byte[] seq_rdf_syntax = new byte[] { 0x68, 0x74, 0x74, 0x70, 0x3A, 0x2F, 0x2F, 0x77, 0x77, 0x77, 0x2E, 0x77, 0x33, 0x2E, 0x6F, 0x72, 0x67, 0x2F, 0x31, 0x39, 0x39, 0x39, 0x2F, 0x30, 0x32, 0x2F, 0x32, 0x32, 0x2D, 0x72, 0x64, 0x66, 0x2D, 0x73, 0x79, 0x6E, 0x74, 0x61, 0x78, 0x2D, 0x6E, 0x73, 0x23 };/* http://www.w3.org/1999/02/22-rdf-syntax-ns# */

        private static string Identify_Mislabeled_Feed(ReadOnlyMemory<byte> Header, string suppliedMIMEType)
        {/* Docs: https://mimesniff.spec.whatwg.org/#sniffing-a-mislabeled-feed */

            int s = 0;
            var Stream = new DataStream<byte>(Header, byte.MinValue);
            if (Stream.Remaining >= 3 && Stream.Slice(0, 3).Span.SequenceEqual(PATTERN_UTF8_BOM.Span))
            {
                Stream.Consume(3);
            }


            while (!Stream.atEnd)
            {
                while (!Stream.atEnd)/* LOOP: L */
                {
                    /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type.*/
                    /* What would undefined even mean here? I'm going to guess its 0x0 */
                    if (Stream.Next == 0x0)
                        return suppliedMIMEType;

                    if (Stream.Next == 0x3C)
                    {
                        Stream.Consume();
                        break;
                    }

                    if (!Is_Whitepace_Byte(Stream.Next))
                        return suppliedMIMEType;

                    Stream.Consume();
                }

                while (!Stream.atEnd)/* LOOP: L */
                {
                    bool EXIT_L = false;
                    /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                    if (Stream.Next == 0x0) return suppliedMIMEType;

                    /* 2)  */
                    if (Stream.Remaining >= 3 && Stream.Slice(0, 3).Span.SequenceEqual(seq_comment_begin))
                    {
                        Stream.Consume(3);
                        while (!Stream.atEnd)/* LOOP: M */
                        {
                            /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                            if (Stream.Next == 0x0) return suppliedMIMEType;

                            if (Stream.Remaining >= 3 && Stream.Slice(0,3).Span.SequenceEqual(seq_comment_end))
                            {
                                Stream.Consume(3);
                                EXIT_L = true;
                                break;
                            }

                            Stream.Consume();
                        }
                    }
                    if (EXIT_L) break;

                    /* 3)  */
                    if (Stream.Remaining >= 1 && Stream.Next == 0x21)// == '!'
                    {
                        Stream.Consume();
                        while (!Stream.atEnd)/* LOOP: M */
                        {
                            /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                            if (Stream.Next == 0x0) return suppliedMIMEType;

                            if (Stream.Remaining >= 1 && Stream.Next == 0x3E)// == '>'
                            {
                                Stream.Consume();
                                EXIT_L = true;
                                break;
                            }

                            Stream.Consume();
                        }
                    }
                    if (EXIT_L) break;

                    /* 4)  */
                    if (Stream.Remaining >= 1 && Stream.Next == 0x3F)// == '?'
                    {
                        Stream.Consume();
                        while (!Stream.atEnd)/* LOOP: M */
                        {
                            /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                            if (Stream.Next == 0x0) return suppliedMIMEType;

                            if (Stream.Remaining >= 2 && Stream.Slice(0, 2).Span.SequenceEqual(seq_code_tag_end))
                            {
                                Stream.Consume(2);
                                EXIT_L = true;
                                break;
                            }

                            Stream.Consume();
                        }
                    }
                    if (EXIT_L) break;

                    /* 5)  */
                    if (Stream.Remaining >= 3 && Stream.Slice(0, 3).Span.SequenceEqual(seq_rss))
                        return "application/rss+xml";

                    /* 6)  */
                    if (Stream.Remaining >= 3 && Stream.Slice(0, 3).Span.SequenceEqual(seq_feed))
                        return "application/atom+xml";

                    /* 7)  */
                    if (Stream.Remaining >= 7 && Stream.Slice(0, 6).Span.SequenceEqual(seq_rdf))
                    {
                        Stream.Consume(7);
                        while (!Stream.atEnd)
                        {
                            /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                            if (Stream.Next == 0x0) return suppliedMIMEType;

                            if (Stream.Remaining >= 24 && Stream.Slice(0, 24).Span.SequenceEqual(seq_purl_namespace))
                            {
                                Stream.Consume(24);
                                while (!Stream.atEnd)/* LOOP: N */
                                {
                                    /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                                    if (Stream.Next == 0x0) return suppliedMIMEType;

                                    if (Stream.Remaining >= 43 && Stream.Slice(0, 43).Span.SequenceEqual(seq_rdf_syntax))
                                        return "application/rss+xml";

                                    Stream.Consume();
                                }
                            }

                            if (Stream.Remaining >= 24 && Stream.Slice(0, 24).Span.SequenceEqual(seq_rdf_syntax))
                            {
                                Stream.Consume(24);
                                while (!Stream.atEnd)
                                {
                                    /* 1) If sequence[s] is undefined, the computed MIME type is the supplied MIME type. */
                                    if (Stream.Next == 0x0) return suppliedMIMEType;
                                    if (Stream.Remaining >= 43 && Stream.Slice(0, 43).Span.SequenceEqual(seq_purl_namespace))
                                        return "application/rss+xml";

                                    Stream.Consume();
                                }
                            }

                            Stream.Consume();
                        }
                    }

                    /* 8)  */
                    return suppliedMIMEType;
                }
            }

            return suppliedMIMEType;
        }
        #endregion

        #region MIMEType Pattern Detection
        private static EMimeType Identify_Unknown_MIME_Type(ReadOnlyMemory<byte> Header, bool bSniffScriptableFlag = false)
        {/* Docs: https://mimesniff.spec.whatwg.org/#rules-for-identifying-an-unknown-mime-type */

            if (bSniffScriptableFlag)
            {
                if (PatternMatch(Header, PATTERN_HTMLDOC, MASK_HTMLDOC, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_HTML, MASK_TAG_HTML, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_HEAD, MASK_TAG_HEAD, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_SCRIPT, MASK_TAG_SCRIPT, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_IFRAME, MASK_TAG_IFRAME, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_H1, MASK_TAG_H1, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_DIV, MASK_TAG_DIV, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_FONT, MASK_TAG_FONT, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_TABLE, MASK_TAG_TABLE, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_A, MASK_TAG_A, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_STYLE, MASK_TAG_STYLE, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_TITLE, MASK_TAG_TITLE, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_B, MASK_TAG_B, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_BODY, MASK_TAG_BODY, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_BR, MASK_TAG_BR, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_P, MASK_TAG_P, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_HTML_COMMENT, MASK_TAG_HTML_COMMENT, WHITESPACE_BYTES, true)) return EMimeType.HTML;
                if (PatternMatch(Header, PATTERN_TAG_XML, MASK_TAG_XML, WHITESPACE_BYTES, true)) return EMimeType.XML;
                if (PatternMatch(Header, PATTERN_PDF, MASK_PDF, null, true)) return EMimeType.PDF;
            }


            if (PatternMatch(Header, PATTERN_ADOBE_POSTSCRIPT, MASK_ADOBE_POSTSCRIPT, null, false)) return EMimeType.AdobePostscript;
            if (PatternMatch(Header, PATTERN_UTF16BE_BOM, MASK_UTF16BE_BOM, null, false)) return EMimeType.Plain;
            if (PatternMatch(Header, PATTERN_UTF16LE_BOM, MASK_UTF16LE_BOM, null, false)) return EMimeType.Plain;
            if (PatternMatch(Header, PATTERN_UTF8_BOM, MASK_UTF8_BOM, null, false)) return EMimeType.Plain;

            if (Identify_Image_MIME_Type(Header, out EMimeType outImageMIME))
                return outImageMIME;

            if (Identify_Audio_Or_Video_MIME_Type(Header, out EMimeType outVideoMIME))
                return outVideoMIME;

            if (Identify_Archive_MIME_Type(Header, out EMimeType outArchiveMIME))
                return outArchiveMIME;


            bool foundBinaryDataByte = false;
            for (int i = 0; i < Header.Length; i++)
            {
                byte b = Header.Span[i];
                if (Is_Binary_Data_Byte(b))
                {
                    foundBinaryDataByte = true;
                    break;
                }
            }

            if (!foundBinaryDataByte) return EMimeType.Plain;

            return EMimeType.OctetStream;
        }


        private static bool Identify_Image_MIME_Type(ReadOnlyMemory<byte> Header, out EMimeType outMIME)
        {/* Docs: https://mimesniff.spec.whatwg.org/#matching-an-image-type-pattern */
            if (PatternMatch(Header, PATTERN_WINDOWS_ICON, MASK_WINDOWS_ICON, null, false)) { outMIME = EMimeType.XIcon; return true; }
            if (PatternMatch(Header, PATTERN_WINDOWS_CURSOR, MASK_WINDOWS_CURSOR, null, false)) { outMIME = EMimeType.XIcon; return true; }
            if (PatternMatch(Header, PATTERN_IMAGE_BMP, MASK_IMAGE_BMP, null, false)) { outMIME = EMimeType.BMP; return true; }
            if (PatternMatch(Header, PATTERN_IMAGE_GIF87a, MASK_IMAGE_GIF87a, null, false)) { outMIME = EMimeType.GIF; return true; }
            if (PatternMatch(Header, PATTERN_IMAGE_GIF89a, MASK_IMAGE_GIF89a, null, false)) { outMIME = EMimeType.GIF; return true; }
            if (PatternMatch(Header, PATTERN_IMAGE_WEBP, MASK_IMAGE_WEBP, null, false)) { outMIME = EMimeType.WebP; return true; }
            if (PatternMatch(Header, PATTERN_IMAGE_PNG, MASK_IMAGE_PNG, null, false)) { outMIME = EMimeType.PNG; return true; }
            if (PatternMatch(Header, PATTERN_IMAGE_JPEG, MASK_IMAGE_JPEG, null, false)) { outMIME = EMimeType.JPEG; return true; }

            outMIME = 0x0;
            return false;
        }


        private static bool Identify_Audio_Or_Video_MIME_Type(ReadOnlyMemory<byte> Header, out EMimeType outMIME)
        {/* Docs: https://mimesniff.spec.whatwg.org/#matching-an-audio-or-video-type-pattern */
            if (PatternMatch(Header, PATTERN_SND, MASK_SND, null, false)) { outMIME = EMimeType.AudioBasic; return true; }
            if (PatternMatch(Header, PATTERN_AIFF, MASK_AIFF, null, false)) { outMIME = EMimeType.AIFF; return true; }
            if (PatternMatch(Header, PATTERN_MP3, MASK_MP3, null, false)) { outMIME = EMimeType.MP3; return true; }
            if (PatternMatch(Header, PATTERN_OGG, MASK_OGG, null, false)) { outMIME = EMimeType.OGG; return true; }
            if (PatternMatch(Header, PATTERN_MIDI, MASK_MIDI, null, false)) { outMIME = EMimeType.MIDI; return true; }
            if (PatternMatch(Header, PATTERN_AVI, MASK_AVI, null, false)) { outMIME = EMimeType.AVI; return true; }
            if (PatternMatch(Header, PATTERN_WAVE, MASK_WAVE, null, false)) { outMIME = EMimeType.WAVE; return true; }

            /* 2) If input matches the signature for MP4, return "video/mp4". */
            if (Is_MP4_Header(Header)) { outMIME = EMimeType.MPEG4; return true; }
            /* 3) If input matches the signature for WebM, return "video/webm". */
            if (Is_WEBM_Header(Header)) { outMIME = EMimeType.WEBM; return true; }
            /* 4) If input matches the signature for MP3 without ID3, return "audio/mpeg". */
            if (Is_MP3_Header(Header)) { outMIME = EMimeType.MP3; return true; }

            outMIME = 0x0;
            return false;
        }

        static byte[] PATTERN_MP4_FTYP = (new byte[] { 0x66, 0x74, 0x79, 0x70 });
        static byte[] PATTERN_MP4_MP4 = (new byte[] { 0x6D, 0x70, 0x34 });

        private static bool Is_MP4_Header(ReadOnlyMemory<byte> Header)
        {/* Docs: https://mimesniff.spec.whatwg.org/#signature-for-mp4 */
            if (Header.Length < 12) return false;
            byte[] data = Header.ToArray();

            UInt32 boxSize = (UInt32)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 0));
            if (Header.Length < boxSize || (boxSize % 4) != 0) return false;

            if (Header.Slice(4, 4).Span.SequenceEqual(PATTERN_MP4_FTYP)) return true;
            if (Header.Slice(8, 3).Span.SequenceEqual(PATTERN_MP4_MP4)) return true;

            var bytesRead = 16;
            while(bytesRead < boxSize)
            {
                if (Header.Slice(bytesRead, 3).Span.SequenceEqual(PATTERN_MP4_MP4)) return true;
                bytesRead += 4;
            }

            return false;
        }


        private static bool Is_WEBM_Header(ReadOnlyMemory<byte> Header)
        {/* Docs: https://mimesniff.spec.whatwg.org/#signature-for-webm */
            if (Header.Length < 4) return false;
            if (!Header.Slice(0, 4).Span.SequenceEqual(new byte[] { 0x1A, 0x45, 0xDF, 0xA3 })) return false;

            var iter = 4;
            while (iter < Header.Length && iter < 38)
            {
                if (Header.Slice(iter, 2).Span.SequenceEqual(new byte[] { 0x42, 0x82 }))
                {
                    iter += 2;
                    if (iter >= Header.Length) break;
                    parse_webm_vint(Header.Slice(iter), out int _, out int number_size);
                    iter += number_size;
                    if (iter < (Header.Length - 4)) break;
                    var matched = match_padded_sequence(new byte[] { }, Header.Slice(iter));
                    if (matched) return true;
                }
                iter++;
            }

            return false;
        }

        static bool match_padded_sequence(ReadOnlyMemory<byte> pattern, ReadOnlyMemory<byte> sequence)
        {/* Docs: https://mimesniff.spec.whatwg.org/#matching-a-padded-sequence */
            /* Matching a padded sequence pattern on a sequence sequence at starting at byte offset and ending at by end means returning true if sequence has a length greater than end, 
             * and contains exactly, in the range [offset, end], the bytes in pattern, in the same order, eventually preceded by bytes with a value of 0x00, false otherwise. */
            if (!sequence.Span.SequenceEqual(pattern.Span)) return false;
            /*for (int i=end; i<sequence.Length; i++)
            {
                if (sequence.Span[i] != 0x0) return false;
            }*/

            return true;
        }

        static void parse_webm_vint(ReadOnlyMemory<byte> data, out int outParsedNumber, out int outNumberSize)
        {/* Docs: https://mimesniff.spec.whatwg.org/#parse-a-vint */
            byte mask = 128;
            var max_vint_length = 8;
            var numberSize = 1;

            while (numberSize < max_vint_length && numberSize < data.Length)
            {
                if (0 != (data.Span[0] & mask)) break;
                //mask = (mask >> 1) & 0xFF;
                mask = unchecked((byte)(mask >> 1));
                numberSize++;
            }
            var index = 0;
            var parsedNumber = data.Span[index] & ~mask;
            index++;
            var bytesRemaining = numberSize;
            while (bytesRemaining != 0)
            {
                parsedNumber = parsedNumber << 8;
                parsedNumber = parsedNumber | data.Span[index];
                index++;
                if (index >= data.Length) break;
                bytesRemaining--;
            }

            outParsedNumber = parsedNumber;
            outNumberSize = numberSize;
        }


        private static bool Is_MP3_Header(ReadOnlyMemory<byte> Header)
        {/* Docs: https://mimesniff.spec.whatwg.org/#signature-for-mp3-without-id3 */
            int s = 0;
            if (!match_mp3_header(Header)) return false;
            if (!parse_mp3_frame(Header, out int version, out int bitRate, out int sampleRate, out int pad)) return false;
            var skippedBytes = compute_mp3_frame_size(Header, version, bitRate, sampleRate, pad);
            if (skippedBytes < 4 || skippedBytes > (0 - Header.Length)) return false;
            s += skippedBytes;
            return Is_MP3_Header(Header.Slice(s));
        }


        static int[] mp2_rate_table = new int[] { 0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000, 160000 };
        static int[] mp3_samplerate_table = new int[] { 44100, 48000, 32000 };
        static int[] mp3_rate_table = new int[] { 0, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000, 192000, 224000, 256000, 320000 };
        private static bool match_mp3_header(ReadOnlyMemory<byte> data)
        {/* Docs: https://mimesniff.spec.whatwg.org/#match-an-mp3-header */
            if (data.Length < 4) return false;
            if (data.Span[0] != 0xFF && (data.Span[1] & 0xE0) != 0xE0) return false;
            var layer = data.Span[1] & 0x06 >> 1;
            if (layer == 0) return false;
            var bitRate = data.Span[2] & 0xF0 >> 4;
            if (bitRate == 15) return false;
            var sampleRate = data.Span[2] & 0x0C >> 2;
            if (sampleRate == 3) return false;
            var freq = mp3_samplerate_table[sampleRate];
            var finalLayer = 4 - (data.Span[1]);
            if ((finalLayer & 0x06 >> 1) != 3) return false;

            return true;
        }

        private static bool parse_mp3_frame(ReadOnlyMemory<byte> data, out int outVersion, out int outBitRate, out int outSampleRate, out int outPad)
        {/* Docs: https://mimesniff.spec.whatwg.org/#parse-an-mp3-frame */
            var version = data.Span[1] & 0x18 >> 3;
            var bitRateIndex = data.Span[2] & 0xF0 >> 4;
            int bitRate = 0;
            if (0 != (version & 0x01))
                bitRate = mp2_rate_table[bitRateIndex];
            else if (0 == (version & 0x01))
                bitRate = mp3_rate_table[bitRateIndex];

            var sampleRateIndex = data.Span[2] & 0x0C >> 2;
            var sampleRate = mp3_samplerate_table[sampleRateIndex];
            var pad = data.Span[2] & 0x02 >> 1;

            outVersion = version;
            outBitRate = bitRate;
            outSampleRate = sampleRate;
            outPad = pad;
            return true;
        }

        private static int compute_mp3_frame_size(ReadOnlyMemory<byte> data, int version, int bitRate, int sampleRate, int pad)
        {/* Docs: https://mimesniff.spec.whatwg.org/#compute-an-mp3-frame-size */
            int scale = (version == 1) ? 72 : 144;
            var size = bitRate * scale / sampleRate;
            if (pad != 0) size++;

            return size;
        }


        private static bool Identify_Archive_MIME_Type(ReadOnlyMemory<byte> Header, out EMimeType outMIME)
        {/* Docs: https://mimesniff.spec.whatwg.org/#archive-type-pattern-matching-algorithm */

            if (PatternMatch(Header.Slice(4, 4), PATTERN_GZIP, MASK_GZIP, null, false)) { outMIME = EMimeType.GZIP; return true; }
            if (PatternMatch(Header.Slice(4, 4), PATTERN_ZIP, MASK_ZIP, null, false)) { outMIME = EMimeType.ZIP; return true; }
            if (PatternMatch(Header.Slice(4, 4), PATTERN_RAR, MASK_RAR, null, false)) { outMIME = EMimeType.RAR; return true; }

            outMIME = 0x0;
            return false;
        }




        private static bool PatternMatch(ReadOnlyMemory<byte> input, ReadOnlyMemory<byte> pattern, ReadOnlyMemory<byte> mask, ReadOnlyMemory<byte> ignore, bool tagTerminated)
        {/* Docs: https://mimesniff.spec.whatwg.org/#pattern-matching-algorithm */
            if (pattern.Length != mask.Length)
            {
                throw new DOMException($"Cannot pattern match Pattern with Mask of different length!");
            }

            if (input.Length < pattern.Length)
                return false;

            var inputSpan = input.Span;
            var patternSpan = pattern.Span;
            var maskSpan = mask.Span;

            var s = 0;
            if (!ReferenceEquals(null, ignore))
            {
                var ignoreSpan = ignore.Span;
                while (s < input.Length)
                {
                    if (-1 == ignoreSpan.IndexOf(inputSpan[s]))
                        break;

                    s++;
                }
            }

            var p = 0;
            while (p < pattern.Length)
            {
                var maskedData = inputSpan[s] & maskSpan[p];

                if (maskedData != patternSpan[p])
                    return false;

                s++;
                p++;
            }

            if (tagTerminated)
            {
                if (!Is_Tag_Terminating_Byte(inputSpan[s]))
                    return false;
            }

            return true;
        }
        #endregion
    }
}
