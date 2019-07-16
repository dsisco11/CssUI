using CssUI.CSS.Parser;
using System;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Serialization
{
    /// <summary>
    /// Provides common functions related to CSS parsing
    /// </summary>
    public static class ParserCommon
    {

        #region Media Querys

        /// <summary>
        /// Returns <c>True</c> if the next tokens makeup a media feature comparator ('=', '<', '>', '<=', '>=')
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Is_And_Token(CssToken A)
        {
            if (ReferenceEquals(null, A))
                throw new ArgumentException();

            return (A.Type == ECssTokenType.Delim && (A as IdentToken).Value.Equals("and"));
        }

        /// <summary>
        /// Returns if the next tokens in the stream define a ratio value
        /// <para>Used by <see cref="Media.MediaFeature"/></para>
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Ratio_Value(CssToken A, CssToken B)
        {
            /* A ratio is a <number> <?whitespace> / <?whitespace> <number> */
            if (ReferenceEquals(null, A) || ReferenceEquals(null, B))
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
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Starts_Media_Comparator(CssToken A)
        {
            if (ReferenceEquals(null, A))
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
