using CssUI.Common.Exceptions;
using CssUI.DOM.Exceptions;
using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime.CompilerServices;
using static CssUI.UnicodeCommon;

namespace CssUI
{
    public static class ParsingCommon
    {

        #region Utility
        public static string Get_Location(DataConsumer<char> Stream)
        {
            return Stream.AsMemory().Slice((int)Stream.LongPosition, 32).ToString();
        }

        /// <summary>
        /// Converts a series of digits into a base10 number
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Digits_To_Base10(ReadOnlyMemory<char> digits) => Digits_To_Base10(digits.Span);
        /// <summary>
        /// Converts a series of digits into a base10 number
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Digits_To_Base10(ReadOnlySpan<char> digits)
        {
            if (digits == null || digits.IsEmpty)
                return 0;
            if (!StringCommon.ContainsOnly(digits, ASCII_DIGITS))
                throw new ArgumentOutOfRangeException(ParserErrors.INVALID_CONTAINS_NON_DIGIT_CHARS);
            Contract.EndContractBlock();

            long Integer = 0;
            var span = digits;
            long power = 1;
            for (int i = digits.Length-1; i >= 0 ; i--)
            {
                Integer += power * Ascii_Digit_To_Value(span[i]);
                power *= 10;
            }

            return Integer;
        }

        /// <summary>
        /// Converts a series of digits into an unsigned base10 number
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Digits_To_Base10_Unsigned(ReadOnlyMemory<char> digits) => Digits_To_Base10_Unsigned(digits.Span);
        /// <summary>
        /// Converts a series of digits into an unsigned base10 number
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Digits_To_Base10_Unsigned(ReadOnlySpan<char> digits)
        {
            if (digits == null || digits.IsEmpty)
                return 0;
            if (!StringCommon.ContainsOnly(digits, ASCII_DIGITS))
                throw new ArgumentOutOfRangeException(ParserErrors.INVALID_CONTAINS_NON_DIGIT_CHARS);
            Contract.EndContractBlock();

            ulong Integer = 0;
            var span = digits;
            ulong power = 1;
            for (int i = digits.Length-1; i >= 0; i--)
            {
                Integer += power * (ulong)Ascii_Digit_To_Value(span[i]);
                power *= 10;
            }

            return Integer;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToInteger(int sign, ReadOnlySpan<char> integerDigits, int exponent_sign, ReadOnlySpan<char> exponentDigits)
        {/* s·(i + f·10-d)·10te. */
            var I = Digits_To_Base10(integerDigits);
            if (sign != 1) I = -I;

            if (exponentDigits.Length > 0)
            {
                var E = Digits_To_Base10_Unsigned(exponentDigits);
                long Exp = MathExt.Pow(10L, E);
                if (exponent_sign != 1) Exp = -Exp;

                I *= Exp;
            }

            return I; 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDecimal(int sign, ReadOnlySpan<char> integerDigits, ReadOnlySpan<char> fractionDigits, int exponent_sign, ReadOnlySpan<char> exponentDigits)
        {/* s·(i + f·10-d)·10te. */
            long I = Digits_To_Base10(integerDigits);
            double RetVal = I;

            if (fractionDigits.Length > 0)
            {
                var F = Digits_To_Base10_Unsigned(fractionDigits);
                var Frac = MathExt.NPow(10, (uint)fractionDigits.Length);
                Frac *= F;
                RetVal += Frac;
            }

            if (exponentDigits.Length > 0)
            {
                var E = Digits_To_Base10_Unsigned(exponentDigits);
                var Exp = MathExt.Pow(10L, E);
                if (exponent_sign != 1) Exp = -Exp;

                RetVal *= Exp;
            }

            return (sign == 1) ? RetVal : -RetVal;
            //return (sign * (I + (F * Math.Pow(10, -fractionDigits.Length))) * Math.Pow(10, exponent_sign * E));
        }
        #endregion

        #region Hexadecimal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Parse_Hex(ReadOnlyMemory<char> input)
        {
            if (!Parse_Hex(input, out ulong outValue))
            {
                throw new Exception(ParserErrors.PARSING_FAILED);
            }

            return outValue;
        }
        public static bool Parse_Hex(ReadOnlyMemory<char> input, out ulong outValue)
        {
            ulong result = 0;
            var span = input.Span;

            for(int i=0; i<span.Length; i++)
            {
                if (!Is_Ascii_Hex_Digit(span[i]))
                {
                    if (span[i] == CHAR_HASH && i == 0)
                    {
                        /* it's fine just ignore it */
                        continue;
                    }

                    outValue = 0;
                    return false;
                }

                var v = (ulong)Ascii_Hex_To_Value(span[i]);
                result = (16 * result) + v;
            }

            outValue = result;
            return true;
        }
        #endregion

        #region Integer
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse_Integer(ReadOnlyMemory<char> input, out long outValue)
        {
            DataConsumer<char> Stream = new DataConsumer<char>(input, EOF);
            bool result = Parse_Integer(Stream, out long outParsed);
            outValue = outParsed;
            return result;
        }
        public static bool Parse_Integer(DataConsumer<char> Stream, out long outValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#signed-integers */
            if (Stream is null) throw new ArgumentNullException(nameof(Stream));
            Contract.EndContractBlock();

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse_FloatingPoint(ReadOnlyMemory<char> input, out float outValue)
        {
            DataConsumer<char> Stream = new DataConsumer<char>(input, EOF);
            bool result = Parse_FloatingPoint(Stream, out double outParsed);
            outValue = (float)outParsed;
            return result;
        }
        public static bool Parse_FloatingPoint(DataConsumer<char> Stream, out float outValue)
        {
            bool result = Parse_FloatingPoint(Stream, out double outParsed);
            outValue = (float)outParsed;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Parse_FloatingPoint(ReadOnlyMemory<char> input, out double outValue)
        {
            DataConsumer<char> Stream = new DataConsumer<char>(input, EOF);
            bool result = Parse_FloatingPoint(Stream, out double outParsed);
            outValue = outParsed;
            return result;
        }
        public static bool Parse_FloatingPoint(DataConsumer<char> Stream, out double outValue)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-floating-point-number-values */
            if (Stream is null) throw new ArgumentNullException(nameof(Stream));
            Contract.EndContractBlock();

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
                value *= Digits_To_Base10(outDigits);//double.Parse(outDigits.ToString(), CultureInfo.InvariantCulture);
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
                        exponent *= Digits_To_Base10(outDigits);//double.Parse(outDigits.ToString(), CultureInfo.InvariantCulture);
                        /* 6) Multiply value by ten raised to the exponentth power. */
                        value *= Math.Pow(10, exponent);
                    }
                }
            }

            /* 15) Conversion: Let S be the set of finite IEEE 754 double-precision floating-point values except −0, but with two special values added: 2^1024 and −2^1024. */
            /* 16) Let rounded-value be the number in S that is closest to value, 
             * selecting the number with an even significand if there are two equally close values. 
             * (The two special values 2^1024 and −2^1024 are considered to have even significands for this purpose.) */
            var roundedValue = value;
            if (roundedValue == -0D) roundedValue = -roundedValue;

            /* 17) If rounded-value is 2^1024 or −2^1024, return an error. */
            if (roundedValue == double.MinValue || roundedValue == double.MaxValue)
            {
                outValue = double.NaN;
                return false;
            }
            /* 18) Return rounded-value. */
            outValue = roundedValue;
            return true;
        }
        #endregion
    }
}
