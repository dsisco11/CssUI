using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS
{
    /// <summary>
    /// Handles An+B syntax matching
    /// <para>Only used by certain CSS functions</para>
    /// </summary>
    public class CssAnBMatcher
    {/* Docs: https://www.w3.org/TR/css-syntax-3/#anb-syntax */

        readonly int A;
        readonly int B;

        #region Constructors
        public CssAnBMatcher(int A, int B)
        {
            this.A = A;
            this.B = B;
        }

        /// <summary>
        /// Consumes and returns an An+B token from a <see cref="TokenStream"/>
        /// </summary>
        /// <param name="Tokens"></param>
        /// <returns></returns>
        public static CssAnBMatcher Consume(DataConsumer<CssToken> Stream)
        {
            // TODO: Test An+B syntax parsing
            int A = 0;
            int B = 0;
            if (Stream.Next.Type == ECssTokenType.Ident)// this token is a word, but we only accept 'even' or 'odd'
            {
                IdentToken word = Stream.Consume<IdentToken>();
                if (word.Value.Equals("even", StringComparison.OrdinalIgnoreCase)) return new CssAnBMatcher(2, 0);//{ A = 2; B = 0; }
                else if (word.Value.Equals("odd", StringComparison.OrdinalIgnoreCase)) return new CssAnBMatcher(2, 1);//{ A = 2; B = 1; }
                else throw new CssSyntaxErrorException($"Invalid identity token ({word.Value}) within An+B syntax");

                // if (Stream.Next != Stream.EOF_ITEM) throw new CssSyntaxError("Expected EOF!");
                // return null;
            }
            else if (Stream.Next.Type == ECssTokenType.Dimension)
            {
                DimensionToken dim = Stream.Consume() as DimensionToken;
                if (dim.DataType == ENumericTokenType.Integer && dim.Unit.Equals("n", StringComparison.OrdinalIgnoreCase))
                {// <n-dimension>
                    throw new NotImplementedException($"{nameof(CssAnBMatcher)} Has not implemented parsing support for {nameof(ECssTokenType.Dimension)}");

                }
                else if (dim.DataType == ENumericTokenType.Integer && dim.Unit.Equals("n-", StringComparison.OrdinalIgnoreCase))
                {// <ndash-dimension>
                    throw new NotImplementedException($"{nameof(CssAnBMatcher)} Has not implemented parsing support for {nameof(ECssTokenType.Dimension)}");
                }
                else if (dim.DataType == ENumericTokenType.Integer && dim.Unit.StartsWith("n-", StringComparison.OrdinalIgnoreCase))
                {// <ndashdigit-dimension>
                    throw new NotImplementedException($"{nameof(CssAnBMatcher)} Has not implemented parsing support for {nameof(ECssTokenType.Dimension)}");
                }
            }
            else if (Stream.Next.Type == ECssTokenType.Ident)
            {
                throw new NotImplementedException($"{nameof(CssAnBMatcher)} Has not implemented parsing support for {nameof(ECssTokenType.Ident)}");
            }
            else if (Stream.Next.Type == ECssTokenType.Number)
            {
                throw new NotImplementedException($"{nameof(CssAnBMatcher)} Has not implemented parsing support for {nameof(ECssTokenType.Number)}");
            }

            return new CssAnBMatcher(A, B);
        }
        #endregion

        /// <summary>
        /// Checks if a given index matches
        /// </summary>
        /// <param name="index">The index to check</param>
        public bool Match(int index)
        {
            return (A % (index+B)) == 0;
        }
    }
}
