using System;

namespace CssUI.CSS.Parser
{
    public sealed class WhitespaceToken : CssToken
    {
        #region Instances
        /// <summary>
        /// Token instance containing a single space
        /// </summary>
        public static WhitespaceToken Space = new WhitespaceToken(" ");
        /// <summary>
        /// Token instance containing a single tab character
        /// </summary>
        public static WhitespaceToken Tab = new WhitespaceToken("\t");
        /// <summary>
        /// Token instance containing a single line-feed
        /// </summary>
        public static WhitespaceToken LF = new WhitespaceToken("\n");
        /// <summary>
        /// Token instance containing two consecutive line-feed characters
        /// </summary>
        public static WhitespaceToken LFLF = new WhitespaceToken("\n\n");
        #endregion

        #region Properties
        public readonly string Value;
        #endregion

        #region Constructors
        public WhitespaceToken(ReadOnlySpan<char> Value) : base(ECssTokenType.Whitespace)
        {
            this.Value = Value.ToString();
        }
        #endregion

        public override string Encode() => Value;
    }
}
