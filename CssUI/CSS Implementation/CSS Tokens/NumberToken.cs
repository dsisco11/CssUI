
namespace CssUI.CSS
{
    public sealed class NumberToken : ValuedTokenBase
    {
        /// <summary>
        /// Holds the numeric representation of this token value
        /// </summary>
        public readonly dynamic Number = null;
        /// <summary>
        /// Specifies the type of value stored in the <see cref="Number"/> field. (int or float)
        /// </summary>
        public readonly ENumericTokenType DataType = ENumericTokenType.Number;

        public NumberToken(ENumericTokenType DataType, string Value, dynamic Number) : base (ECssTokenType.Number, Value)
        {
            this.DataType = DataType;
            this.Number = Number;
        }

        public override string Encode()
        {
            return this.Value;
        }

        #region Equality Operators
        public static bool operator ==(NumberToken A, NumberToken B)
        {
            return (A.Type == B.Type && A.Value == B.Value);
        }

        public static bool operator !=(NumberToken A, NumberToken B)
        {
            return (A.Type != B.Type && A.Value != B.Value);
        }

        public override bool Equals(object o)
        {

            if (o is NumberToken)
            {
                return this == (NumberToken)o;
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
