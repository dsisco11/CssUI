using CssUI.CSS.Media;
using CssUI.CSS.Parser;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Serialization
{
    /// <summary>
    /// Provides common functions related to CSS parsing
    /// </summary>
    public static class ParserCommon
    {

        #region Common
        public static string Get_Location(ReadOnlySpan<CssToken> Stream)
        {
            return string.Join("", Stream.Slice(0, 6).ToArray().Select(tok => tok.Encode()));
        }
        #endregion

        #region Media Querys

        /// <summary>
        /// Returns <c>True</c> if the token is a keyword for <see cref="EMediaCombinator"/> ('and', 'or', 'not')
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Combinator(CssToken A)
        {
            if (A == null)
                throw new ArgumentException();

            if (A.Type != ECssTokenType.Ident)
                return false;

            string tokStr = (A as IdentToken).Value;
            return CssLookup.Is_Declared(typeof(EMediaCombinator), tokStr);
        }

        /// <summary>
        /// Returns <c>True</c> if the next tokens makeup a media feature comparator ('=', '<', '>', '<=', '>=')
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Comparator(CssToken A)
        {
            /*if (A == null)
                throw new ArgumentException();

            if (A.Type != ECssTokenType.Ident)
                return false;

            string tokStr = (A as IdentToken).Value;
            return CssLookup.Is_Declared(typeof(EMediaOperator), tokStr);*/

            if (A == null)
                return false;

            if (A.Type == ECssTokenType.Delim)
            {
                var delimTok = A as DelimToken;
                return (delimTok.Value.Equals('=') || delimTok.Value.Equals('<') || delimTok.Value.Equals('>'));
            }

            if (A.Type == ECssTokenType.Ident)
            {
                var identTok = A as IdentToken;
                return (identTok.Value.Equals("<=") || identTok.Value.Equals(">="));
            }

            return false;
        }



        /// <summary>
        /// Returns if the next tokens in the stream define a ratio
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// <para>A ratio takes the form: "<number> /"</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Ratio_Value(ReadOnlySpan<CssToken> Stream)
        {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
            /* Only null check A and B here because thats the minimum that any ratio can consist of eg: "<number>/" is 2 tokens */
            if (Stream.Length < 2 || Stream[0] == null || Stream[1] == null)
            {
                return false;
            }

            /* Pattern: "<number> /" */
            if (Stream[0].Type == ECssTokenType.Number && Stream[1].Type == ECssTokenType.Delim && (Stream[1] as DelimToken).Value == UnicodeCommon.CHAR_SOLIDUS)
                return true;

            if (Stream[0].Type == ECssTokenType.Number && Stream[1].Type == ECssTokenType.Whitespace && Stream[2].Type == ECssTokenType.Delim && (Stream[2] as DelimToken).Value == UnicodeCommon.CHAR_SOLIDUS)
                return true;

            return false;
        }


        /// <summary>
        /// Returns if the next token in the stream defines a <see cref="MediaFeature"/> identifier or value
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// <para>A <see cref="MediaFeature"/> always starts with an opening parentheses followed by optional whitespace and an ident token which is NOT a combinator or a number</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_MF_Ident_Or_Value(ReadOnlySpan<CssToken> Stream)
        {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
            /* Only null check A and B here because thats the minimum that any media feature can consist of eg: "color)" is 2 tokens */
            if (Stream.Length < 1 || Stream[0] == null)
            {
                throw new ArgumentException();
            }

            /* Pattern: "<number> | <dimension> | <ident> | <ratio>" */

            if (Stream[0].Type == ECssTokenType.Ident)// <ident>
                return true;

            if (Stream[0].Type == ECssTokenType.Number)// <number>
                return true;

            if (Stream[0].Type == ECssTokenType.Dimension)// <dimension>
                return true;

            if (Starts_Ratio_Value(Stream))// <ratio>
                return true;

            return false;
        }


        /// <summary>
        /// Returns if the next tokens in the stream define a <see cref="MediaCondition"/>
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// <para>A <see cref="MediaCondition"/> always starts with an opening parentheses followed by either a combinator or optional whitespace and another opening parentheses</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Media_Condition(ReadOnlySpan<CssToken> Stream)
        {
            if ( Stream.Length < 2 || Stream[0] == null || Stream[1] == null)
            {
                return false;
            }
            /* Pattern: "(<combinator>" or "( (" */

            if (Stream[0].Type != ECssTokenType.Parenth_Open)
                return false;

            if (Is_Combinator(Stream[1]))// "(<combinator>"
                return true;

            if (Stream[1].Type == ECssTokenType.Parenth_Open)// "(("
                return true;

            if (Stream[1].Type == ECssTokenType.Whitespace && Stream[2].Type == ECssTokenType.Parenth_Open)// "( ("
                return true;

            return false;
        }



        /// <summary>
        /// Returns if the next tokens in the stream define a <see cref="MediaFeature"/>
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// <para>A <see cref="MediaFeature"/> always starts with an opening parentheses followed by optional whitespace and an ident token which is NOT a combinator</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Media_Feature(ReadOnlySpan<CssToken> Stream)
        {
            /* Only null check A and B here because thats the minimum that any media feature can consist of eg: "color)" is 2 tokens */
            if (Stream.Length < 2 || Stream[0] == null || Stream[1] == null)
            {
                return false;
            }

            /* Pattern: "(<?ws><ident> | <value>" */

            if (Stream[0].Type != ECssTokenType.Parenth_Open)
                return false;

            if (!Is_Combinator(Stream[1]) && Starts_MF_Ident_Or_Value(Stream.Slice(1)))// "(<ident>|<value>"
                return true;

            if (Stream[1].Type == ECssTokenType.Whitespace && !Is_Combinator(Stream[2]) && Starts_MF_Ident_Or_Value(Stream.Slice(2)))// "(<ws><ident>|<value>"
                return true;

            return false;
        }


        /// <summary>
        /// Returns if the next tokens in the stream define a discreet media feature
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Boolean_Feature(ReadOnlySpan<CssToken> Stream)
        {
            /* Only null check A and B here because thats the minimum that any media feature can consist of eg: "color)" is 2 tokens */
            if (Stream.Length < 2 || Stream[0] == null || Stream[1] == null)
                return false;

            /* Pattern: "<ident><?ws>)" */
            if (Stream[0].Type != ECssTokenType.Ident)
                return false;

            if (Stream[1].Type == ECssTokenType.Parenth_Close)// "<ident>)"
                return true;

            if (Stream.Length < 3 || Stream[2] == null)
                return false;

            if (Stream[1].Type == ECssTokenType.Whitespace && Stream[2].Type == ECssTokenType.Parenth_Close)// "<ident> )"
                return true;

            return false;
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a discreet media feature
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Discreet_Feature(ReadOnlySpan<CssToken> Stream)
        {
            /* Only null check A and B here because thats the minimum that any media feature can consist of eg: "color:" is 2 tokens */
            if (Stream.Length < 2 || Stream[0] == null || Stream[1] == null)
                return false;

            /* Pattern: "<ident><?ws>:" */
            if (Stream[0].Type != ECssTokenType.Ident)
                return false;

            if (Stream[1].Type == ECssTokenType.Colon)
                return true;

            if (Stream.Length < 3 || Stream[2] == null)
                return false;

            if (Stream[1].Type == ECssTokenType.Whitespace && Stream[2].Type == ECssTokenType.Colon)
                return true;

            return false;
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a range media feature
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Range_Feature(ReadOnlySpan<CssToken> Stream)
        {
            /* Only null check A and B here because thats the minimum that any media feature can consist of eg: "(color" is 2 tokens */
            if (Stream.Length < 2 || Stream[0] == null || Stream[1] == null)
                return false;

            /* Pattern: "<ident>|<value> <?ws> <comparator> " */
            if (!Starts_MF_Ident_Or_Value(Stream))
                return false;

            if (Is_Comparator(Stream[1]))
                return true;

            if (Stream.Length < 3 || Stream[2] == null)
                return false;

            if (Stream[1].Type == ECssTokenType.Whitespace && Is_Comparator(Stream[2]))
                return true;

            return false;
        }

        #endregion
    }
}
