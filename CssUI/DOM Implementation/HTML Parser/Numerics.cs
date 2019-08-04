﻿using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using System;
using static CssUI.UnicodeCommon;

namespace CssUI.HTML.Serialization
{
    public static partial class HTMLParserCommon
    {
        #region Integer
        public static bool Is_Valid_Integer(ReadOnlyMemory<char> input)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Integer(Stream);
        }
        public static bool Is_Valid_Integer(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#signed-integers */

            /* SKip ASCII whitespace */
            Stream.Consume_While(Is_Ascii_Whitespace);

            if (Stream.atEOF)
            {
                return false;
            }

            if (Stream.Next == CHAR_HYPHEN_MINUS || Stream.Next == CHAR_PLUS_SIGN)
            {
                Stream.Consume();
            }


            /* Collect sequence of ASCII digit codepoints */
            Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);

            if (!Stream.atEOF && Is_Ascii_Alpha(Stream.Next))
            {
                return false;
            }

            return true;
        }


        public static bool Parse_Integer(ReadOnlyMemory<char> input, out int outValue)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            bool result = Parse_Integer(Stream, out int outParsed);
            outValue = outParsed;
            return result;
        }
        public static bool Parse_Integer(DataStream<char> Stream, out int outValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#signed-integers */

            bool sign = true;//Sign

            /* SKip ASCII whitespace */
            Stream.Consume_While(Is_Ascii_Whitespace);

            if (Stream.Next == EOF)
            {
                outValue = int.MaxValue;
                return false;
            }

            if (Stream.Next == CHAR_HYPHEN_MINUS)
            {
                sign = false;
                Stream.Consume();
            }
            else if (Stream.Next == CHAR_PLUS_SIGN)
            {
                Stream.Consume();
            }


            /* Collect sequence of ASCII digit codepoints */
            Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);

            if (Stream.Next != EOF && Is_Ascii_Alpha(Stream.Next))
            {
                outValue = int.MaxValue;
                return false;
            }

            var parsed = (int)ParsingCommon.Digits_To_Base10(outDigits);

            outValue = sign ? parsed : 0 - parsed;
            return true;
        }


        public static bool Parse_Integer(ReadOnlyMemory<char> input, out long outValue)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            bool result = Parse_Integer(Stream, out long outParsed);
            outValue = outParsed;
            return result;
        }
        public static bool Parse_Integer(DataStream<char> Stream, out long outValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#signed-integers */
            bool sign = true;

            /* SKip ASCII whitespace */
            Stream.Consume_While(Is_Ascii_Whitespace);

            if (Stream.Next == EOF)
            {
                outValue = long.MaxValue;
                return false;
            }

            if (Stream.Next == CHAR_HYPHEN_MINUS)
            {
                sign = false;
                Stream.Consume();
            }
            else if (Stream.Next == CHAR_PLUS_SIGN)
            {
                Stream.Consume();
            }


            /* Collect sequence of ASCII digit codepoints */
            Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);

            if (Stream.Next != EOF && Is_Ascii_Alpha(Stream.Next))
            {
                outValue = long.MaxValue;
                return false;
            }

            var parsed = ParsingCommon.Digits_To_Base10(outDigits);

            outValue = sign ? parsed : 0 - parsed;
            return true;
        }
        #endregion

        #region Decimal

        public static bool Is_Valid_FloatingPoint(ReadOnlyMemory<char> input)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            return Is_Valid_FloatingPoint(Stream);
        }
        public static bool Is_Valid_FloatingPoint(DataStream<char> Stream)
        {
            return Parse_FloatingPoint(Stream, out double _);
        }


        public static bool Parse_FloatingPoint(ReadOnlyMemory<char> input, out float outValue)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            bool result = Parse_FloatingPoint(Stream, out double outParsed);
            outValue = (float)outParsed;
            return result;
        }
        public static bool Parse_FloatingPoint(DataStream<char> Stream, out float outValue)
        {
            bool result = Parse_FloatingPoint(Stream, out double outParsed);
            outValue = (float)outParsed;
            return result;
        }


        public static bool Parse_FloatingPoint(ReadOnlyMemory<char> input, out double outValue)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            bool result = Parse_FloatingPoint(Stream, out double outParsed);
            outValue = outParsed;
            return result;
        }
        public static bool Parse_FloatingPoint(DataStream<char> Stream, out double outValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-floating-point-number-values */

            double value = 1;
            double divisor = 1;
            double exponent = 1;

            /* Skip ASCII whitespace */
            Stream.Consume_While(Is_Ascii_Whitespace);

            if (Stream.atEOF)
                throw new DomSyntaxError();

            switch (Stream.Next)
            {
                case CHAR_HYPHEN_MINUS:
                    {
                        value = divisor = -1;
                        Stream.Consume();
                    }
                    break;

                case CHAR_PLUS_SIGN:
                    {
                        Stream.Consume();
                    }
                    break;
            }

            if (Stream.atEOF)
                throw new DomSyntaxError();

            /* 9) If the character indicated by position is a U+002E FULL STOP (.), 
             * and that is not the last character in input, 
             * and the character after the character indicated by position is an ASCII digit, 
             * then set value to zero and jump to the step labeled fraction. */
            if (Stream.Next == CHAR_FULL_STOP && Stream.NextNext != EOF && Is_Ascii_Digit(Stream.NextNext))
            {
                value = 0;
            }
            else if (!Is_Ascii_Digit(Stream.Next))
            {
                outValue = double.NaN;
                return false;
            }
            else
            {
                /* 11) Collect a sequence of code points that are ASCII digits from input given position, and interpret the resulting sequence as a base-ten integer. Multiply value by that integer. */
                Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);
                value *= ParsingCommon.Digits_To_Base10(outDigits);
            }

            /* 12) If position is past the end of input, jump to the step labeled conversion. */
            if (!Stream.atEOF)
            {
                /* 13) Fraction: If the character indicated by position is a U+002E FULL STOP (.), run these substeps: */
                if (Stream.Next == CHAR_FULL_STOP)
                {
                    Stream.Consume();
                    /* 2) If position is past the end of input, or if the character indicated by position is not an ASCII digit, U+0065 LATIN SMALL LETTER E (e), or U+0045 LATIN CAPITAL LETTER E (E), then jump to the step labeled conversion. */
                    if (!Stream.atEOF && (Is_Ascii_Digit(Stream.Next) || Stream.Next == CHAR_E_LOWER || Stream.Next == CHAR_E_UPPER))
                    {
                        /* 3) If the character indicated by position is a U+0065 LATIN SMALL LETTER E character (e) or a U+0045 LATIN CAPITAL LETTER E character (E), skip the remainder of these substeps. */
                        if (Is_Ascii_Digit(Stream.Next))
                        {
                            while (Is_Ascii_Digit(Stream.Next))
                            {
                                /* 4) Fraction loop: Multiply divisor by ten. */
                                divisor *= 10;
                                /* 5) Add the value of the character indicated by position, interpreted as a base-ten digit (0..9) and divided by divisor, to value. */
                                double n = Ascii_Digit_To_Value(Stream.Next) / divisor;
                                value += n;
                                /* 6) Advance position to the next character. */
                                Stream.Consume();
                                /* 7) If position is past the end of input, then jump to the step labeled conversion. */

                                if (Stream.Next == EOF)
                                    break;
                            }
                        }
                    }
                }

                /* 14) If the character indicated by position is U+0065 (e) or a U+0045 (E), then: */
                if (Stream.Next == CHAR_E_LOWER || Stream.Next == CHAR_E_UPPER)
                {
                    Stream.Consume();
                    /* 2) If position is past the end of input, then jump to the step labeled conversion. */
                    /* 3) If the character indicated by position is a U+002D HYPHEN-MINUS character (-): */
                    if (Stream.Next == CHAR_HYPHEN_MINUS)
                    {
                        exponent = -1;
                        Stream.Consume();
                    }
                    else if (Stream.Next == CHAR_PLUS_SIGN)
                    {
                        Stream.Consume();
                    }

                    /* 4) If the character indicated by position is not an ASCII digit, then jump to the step labeled conversion. */
                    if (Is_Ascii_Digit(Stream.Next))
                    {
                        /* 5) Collect a sequence of code points that are ASCII digits from input given position, and interpret the resulting sequence as a base-ten integer. Multiply exponent by that integer. */
                        Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);
                        exponent *= ParsingCommon.Digits_To_Base10(outDigits);
                        /* 6) Multiply value by ten raised to the exponentth power. */
                        value *= Math.Pow(10, exponent);
                    }
                }
            }

            /* 15) Conversion: Let S be the set of finite IEEE 754 double-precision floating-point values except −0, but with two special values added: 21024 and −21024. */
            /* 16) Let rounded-value be the number in S that is closest to value, 
             * selecting the number with an even significand if there are two equally close values. 
             * (The two special values 21024 and −21024 are considered to have even significands for this purpose.) */
            var roundedValue = Math.Round(value, MidpointRounding.ToEven);

            /* 17) If rounded-value is 21024 or −21024, return an error. */
            if (MathExt.Feq(roundedValue, double.MinValue) || MathExt.Feq(roundedValue, double.MaxValue))
            {
                outValue = double.NaN;
                return false;
            }

            outValue = roundedValue;
            return true;
        }
        #endregion

        #region Lengths
        public static bool Parse_Length(ReadOnlyMemory<char> input, out double outValue, out EAttributeType outType)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            /* 3) Skip ASCII whitespace within input given position. */
            Stream.Consume_While(Is_Ascii_Whitespace);
            if (Stream.Next == EOF)
            {
                outValue = double.NaN;
                outType = EAttributeType.Length;
                return false;
            }

            /* 5) Collect a sequence of code points that are ASCII digits from input given position, and interpret the resulting sequence as a base-ten integer. Let value be that number. */
            Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);
            double value = ParsingCommon.Digits_To_Base10(outDigits);

            /* 6) If position is past the end of input, then return value as a length. */
            if (Stream.Next == EOF)
            {
                outValue = value;
                outType = EAttributeType.Length;
                return true;
            }

            /* 7) If the code point at position within input is U+002E (.), then: */
            if (Stream.Next == CHAR_FULL_STOP)
            {
                Stream.Consume();
                /* 2) If position is past the end of input or the code point at position within input is not an ASCII digit, then return the current dimension value with value, input, and position. */
                if (Stream.Next == EOF)
                {
                    outValue = value;
                    outType = EAttributeType.Length;
                    return true;
                }
                else if (!Is_Ascii_Digit(Stream.Next))
                {
                    outType = Stream.Next == CHAR_PERCENT ? EAttributeType.Percentage : EAttributeType.Length;
                    outValue = value;
                    return true;
                }

                /* 3) Let divisor have the value 1. */
                double divisor = 1;

                while (Is_Ascii_Digit(Stream.Next))
                {
                    divisor *= 10;
                    /* 2) Add the value of the code point at position within input, interpreted as a base-ten digit (0..9) and divided by divisor, to value. */
                    double n = Ascii_Digit_To_Value(Stream.Next) / divisor;
                    value += n;
                    Stream.Consume();
                }
            }

            /* 8) Return the current dimension value with value, input, and position. */
            outType = Stream.Next == CHAR_PERCENT ? EAttributeType.Percentage : EAttributeType.Length;
            outValue = value;
            return true;
        }
        #endregion
    }
}