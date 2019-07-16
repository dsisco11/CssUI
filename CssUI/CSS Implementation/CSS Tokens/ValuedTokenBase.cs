
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    /// <summary>
    /// Implements a token type that holds a value comprised on zero or more characters
    /// </summary>
    public abstract class ValuedTokenBase: CssToken
    {
        #region Properties
        /// <summary>
        /// Holds the string representation of this tokens value
        /// </summary>
        public readonly string Value = null;
        #endregion

        #region Constructors
        public ValuedTokenBase(ECssTokenType Type, string Value, bool AutoLowercase = true) : base(Type)
        {
            if (AutoLowercase) this.Value = Value.ToLowerInvariant();// CSS is case-insensitive
            else this.Value = Value;
        }
        #endregion


        #region Equality Operators
        public static bool operator ==(ValuedTokenBase A, ValuedTokenBase B)
        {
            return (A.Type == B.Type && string.Compare(A.Value, B.Value) == 0);
        }

        public static bool operator !=(ValuedTokenBase A, ValuedTokenBase B)
        {
            return (A.Type != B.Type && string.Compare(A.Value, B.Value) != 0);
        }

        public override bool Equals(object o)
        {

            if (o is ValuedTokenBase)
            {
                return this == (ValuedTokenBase)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Encode().GetHashCode();
        }
        #endregion

    }
}
