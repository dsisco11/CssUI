using System;
using System.Runtime.CompilerServices;
using static CssUI.UnicodeCommon;

namespace CssUI
{
    public static class ParsingCommon
    {

        #region Utility
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Digits_To_Base10(ReadOnlyMemory<char> digits) => Digits_To_Base10(digits.Span);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Digits_To_Base10(ReadOnlySpan<char> digits)
        {
            if (digits == null)
            {
                return 0;
            }

            long Integer = 0;
            var span = digits;
            long power = 1;
            for (int i = 0; i < digits.Length; i++)
            {
                Integer += power * Ascii_Digit_To_Value(span[i]);
                power *= 10;
            }

            return Integer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToInteger(int sign, ReadOnlySpan<char> integerDigits, int exponent_sign, ReadOnlySpan<char> exponentDigits)
        {
            long I = Digits_To_Base10(integerDigits);
            long E = Digits_To_Base10(exponentDigits);

            /* s·(i + f·10-d)·10te. */
            return (long)(sign * I * Math.Pow(10, exponent_sign * E));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDecimal(double sign, ReadOnlySpan<char> integerDigits, ReadOnlySpan<char> fractionDigits, double exponent_sign, ReadOnlySpan<char> exponentDigits)
        {
            long I = Digits_To_Base10(integerDigits);
            long F = Digits_To_Base10(fractionDigits);
            long E = Digits_To_Base10(exponentDigits);

            /* s·(i + f·10-d)·10te. */
            return (sign * (I + (F * Math.Pow(10, -fractionDigits.Length))) * Math.Pow(10, exponent_sign * E));
        }
        #endregion

        #region Integer
        public static bool Parse_Integer(ReadOnlyMemory<char> input, out long outValue)
        {
            DataStream<char> Stream = new DataStream<char>(input, EOF);
            bool result = Parse_Integer(Stream, out long outParsed);
            outValue = outParsed;
            return result;
        }
        public static bool Parse_Integer(DataStream<char> Stream, out long outValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#signed-integers */
            bool sign = true;//Sign

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
                sign = true;
                Stream.Consume();
            }


            /* Collect sequence of ASCII digit codepoints */
            Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> outDigits);

            if (Stream.Next != EOF && Is_Ascii_Alpha(Stream.Next))
            {
                outValue = long.MaxValue;
                return false;
            }

            var parsed = Digits_To_Base10(outDigits);

            outValue = sign ? parsed :  -parsed;
            return true;
        }
        #endregion

        #region Decimal
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

            if (Stream.Next == EOF)
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

            if (Stream.Next == EOF)
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
                value *= double.Parse(outDigits.ToString());
            }

            /* 12) If position is past the end of input, jump to the step labeled conversion. */
            if (Stream.Next != EOF)
            {
                /* 13) Fraction: If the character indicated by position is a U+002E FULL STOP (.), run these substeps: */
                if (Stream.Next == CHAR_FULL_STOP)
                {
                    Stream.Consume();
                    /* 2) If position is past the end of input, or if the character indicated by position is not an ASCII digit, U+0065 LATIN SMALL LETTER E (e), or U+0045 LATIN CAPITAL LETTER E (E), then jump to the step labeled conversion. */
                    if (Stream.Next != EOF && (Is_Ascii_Digit(Stream.Next) || Stream.Next == CHAR_E_LOWER || Stream.Next == CHAR_E_UPPER))
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
                        exponent *= double.Parse(outDigits.ToString());
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
    }
}
