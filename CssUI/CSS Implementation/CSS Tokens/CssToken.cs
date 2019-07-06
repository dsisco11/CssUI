
namespace CssUI.CSS
{
    /// <summary>
    /// Represents a token within an <see cref="CssTokenizer"/> instance.
    /// </summary>
    public abstract class CssToken
    {// SEE:  https://www.w3.org/TR/css-syntax-3/#tokenizing-and-parsing
        #region Static
        public static EOFToken EOF = new EOFToken();
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
        public static bool operator ==(CssToken A, CssToken B)
        {
            bool Anull = object.ReferenceEquals(null, A);
            bool Bnull = object.ReferenceEquals(null, B);
            if (Anull ^ Bnull) return false;
            else if (Anull == Bnull) return true;

            return (A.Type == B.Type);
        }

        public static bool operator !=(CssToken A, CssToken B)
        {
            bool Anull = object.ReferenceEquals(null, A);
            bool Bnull = object.ReferenceEquals(null, B);
            if (Anull ^ Bnull) return true;
            else if (Anull == Bnull) return false;

            return (A.Type != B.Type);
        }

        public override bool Equals(object o)
        {

            if (o is CssToken)
            {
                return this == (CssToken)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Encode().GetHashCode();
        }
        #endregion

        /// <summary>
        /// Encodes the token back to it's CSS representation
        /// </summary>
        /// <returns>Css string</returns>
        public abstract string Encode();

        #region ToString
        public override string ToString()
        {
            return string.Concat("(", this.GetType().Name, ")", Encode());
        }
        #endregion
    }
}
