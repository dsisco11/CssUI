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
        public static string Get_Location(TokenStream Stream)
        {
            return string.Join("", Stream.Peek(0, 6).Select(tok => tok.Encode()));
        }
        #endregion

        #region Media Querys

        /// <summary>
        /// Returns <c>True</c> if the token is the 'not' keyword
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Negator(CssToken A)
        {
            if (A == null)
                throw new ArgumentException();

            if (A.Type != ECssTokenType.Ident)
                return false;

            string tokStr = (A as IdentToken).Value;
            return tokStr.Equals("not");
        }

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
        /// Returns <c>True</c> if the token is a keyword for <see cref="EMediaOperator"/> ('=', '<', '>', '<=', '>=')
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_Comparator(CssToken A)
        {
            if (A == null)
                throw new ArgumentException();

            if (A.Type != ECssTokenType.Ident)
                return false;

            string tokStr = (A as IdentToken).Value;
            return CssLookup.Is_Declared(typeof(EMediaOperator), tokStr);
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a <see cref="MediaCondition"/>
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// <para>A <see cref="MediaCondition"/> always starts with an opening parentheses followed by either a combinator or optional whitespace and another opening parentheses</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Media_Condition(CssToken A, CssToken B, CssToken C)
        {
            if (A == null || B == null || C == null)
                throw new ArgumentException();
            /* Pattern: "(<combinator>" or "( (" */

            if (A.Type != ECssTokenType.Parenth_Open)
                return false;

            if (Is_Combinator(B))// "(<combinator>"
                return true;

            if (B.Type == ECssTokenType.Parenth_Open)// "(("
                return true;

            if (B.Type == ECssTokenType.Whitespace && C.Type == ECssTokenType.Parenth_Open)// "( ("
                return true;

            return false;
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a <see cref="MediaFeature"/>
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// <para>A <see cref="MediaFeature"/> always starts with an opening parentheses followed by optional whitespace and an ident token which is NOT a combinator</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Media_Feature(CssToken A, CssToken B, CssToken C)
        {
            /* Only null check A and B here because thats the minimum that any media feature can consist of eg: "color)" is 2 tokens */
            if (A == null || B == null)
                throw new ArgumentException();

            /* Pattern: "(<ident>" */

            if (A.Type != ECssTokenType.Parenth_Open)
                return false;

            if (B.Type == ECssTokenType.Ident && !Is_Combinator(B))// "(<ident>"
                return true;

            if (B.Type == ECssTokenType.Whitespace && C.Type == ECssTokenType.Ident && !Is_Combinator(C))// "( <ident>"
                return true;

            return false;
        }


        /// <summary>
        /// Returns if the next tokens in the stream define a discreet media feature
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Boolean_Feature(CssToken A, CssToken B, CssToken C)
        {
            if (A == null || B == null || C == null)
                throw new ArgumentException();
            /* Pattern: "(<ident>" */

            if (A.Type != ECssTokenType.Parenth_Open)
                return false;

            if (B.Type == ECssTokenType.Ident)// "(<ident>"
                return true;

            if (B.Type == ECssTokenType.Whitespace && C.Type == ECssTokenType.Ident)// "( <ident>"
                return true;

            return false;
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a discreet media feature
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Discreet_Feature(CssToken A, CssToken B, CssToken C)
        {
            if (A == null || B == null)
                throw new ArgumentException();

            if (A.Type != ECssTokenType.Ident)
                return false;

            if (B.Type == ECssTokenType.Colon)
                return true;

            if (B.Type == ECssTokenType.Whitespace && C.Type == ECssTokenType.Colon)
                return true;

            return false;
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a discreet media feature
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Range_Feature(CssToken A, CssToken B, CssToken C)
        {
            if (A == null || B == null)
                throw new ArgumentException();

            if (A.Type != ECssTokenType.Ident)
                return false;

            if (Is_Comparator(B))
                return true;

            if (B.Type == ECssTokenType.Whitespace && Is_Comparator(C))
                return true;

            return false;
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a ratio value
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Ratio_Value(CssToken A, CssToken B)
        {
            /* A ratio is a <number> <?whitespace> / <?whitespace> <number> */
            if (A == null || B == null)
                throw new ArgumentException();

            if (A.Type == ECssTokenType.Delim && (A as ValuedTokenBase).Value.Equals("/"))
                return true;

            if (A.Type == ECssTokenType.Whitespace && B.Type == ECssTokenType.Delim && (B as ValuedTokenBase).Value.Equals("/"))
                return true;

            return false;
        }

        /// <summary>
        /// Returns <c>True</c> if the next tokens makeup a media feature comparator ('=', '<', '>', '<=', '>=')
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Media_Comparator(CssToken A)
        {
            if (A == null)
                throw new ArgumentException();

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

        #endregion
    }
}
