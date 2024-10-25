using System;
using System.IO;

namespace CssUI.CSS.Parser
{
    /// <summary>
    /// Represents a token within an <see cref="CssTokenizer"/> instance.
    /// </summary>
    public abstract class CssToken
    {/* Docs: https://www.w3.org/TR/css-syntax-3/#tokenizing-and-parsing */
        #region Static
        public static EOFToken EOF = EOFToken.Instance;
        #endregion

        #region Properties
        public readonly ECssTokenType Type = ECssTokenType.None;
        #endregion

        #region Constructors
        public CssToken(ECssTokenType Type)
        {
            this.Type = Type;
        }
        #endregion

        #region Equality Operators
        public static bool operator ==(CssToken left, CssToken right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CssToken left, CssToken right)
        {
            return !(left == right);
        }

        public override bool Equals(object o)
        {
            if (o is CssToken other)
            {
                return Type == other.Type;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Encode().GetHashCode(System.StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        /// <summary>
        /// Encodes the token back to it's representation as a text string
        /// </summary>
        /// <returns>Text form of the token</returns>
        public abstract string Encode();

        #region ToString
        public override string ToString()
        {
            return Encode();
        }
        #endregion
    }
}
