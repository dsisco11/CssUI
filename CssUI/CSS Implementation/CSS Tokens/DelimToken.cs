
using CssUI.CSS.Parser;

namespace CssUI.CSS
{
    public sealed class DelimToken : CssToken
    {
        public readonly char Value = (char)0;
     
        public DelimToken(char Value) : base(ECssTokenType.Delim)
        {
            this.Value = Value;
        }

        public override string Encode()
        {
            return string.Concat(Value);
        }

        #region Equality Operators
        public static bool operator ==(DelimToken A, DelimToken B)
        {
            return (A.Type == B.Type && A.Value == B.Value);
        }

        public static bool operator !=(DelimToken A, DelimToken B)
        {
            return (A.Type != B.Type && A.Value != B.Value);
        }

        public override bool Equals(object o)
        {

            if (o is DelimToken)
            {
                return this == (DelimToken)o;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        #endregion
    }
}
