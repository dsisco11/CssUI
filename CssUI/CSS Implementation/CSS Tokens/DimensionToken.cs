
namespace CssUI.CSS
{
    public sealed class DimensionToken : ValuedTokenBase
    {
        /// <summary>
        /// Holds the numeric representation of this token value
        /// </summary>
        public readonly dynamic Number = null;
        /// <summary>
        /// Specifies the type of value stored in the <see cref="Number"/> field. (int or float)
        /// </summary>
        public readonly ENumericTokenType DataType = ENumericTokenType.Number;
        /// <summary>
        /// Holds the dimension's unit type string
        /// </summary>
        public readonly string Unit = null;

        public DimensionToken(ENumericTokenType DataType, string Value, dynamic Number, string Unit) : base(ECssTokenType.Dimension, Value)
        {
            this.Number = Number;
            this.DataType = DataType;
            this.Unit = Unit;
        }

        public override string Encode()
        {
            return string.Concat(Value, Unit);
        }


        #region Equality Operators
        public static bool operator ==(DimensionToken A, DimensionToken B)
        {
            return (A.Type == B.Type && A.Value == B.Value);
        }

        public static bool operator !=(DimensionToken A, DimensionToken B)
        {
            return (A.Type != B.Type && A.Value != B.Value);
        }

        public override bool Equals(object o)
        {

            if (o is DimensionToken)
            {
                return this == (DimensionToken)o;
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
