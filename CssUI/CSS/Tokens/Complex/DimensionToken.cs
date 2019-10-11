using System;

namespace CssUI.CSS.Parser
{
    public sealed class DimensionToken : ValuedTokenBase
    {
        /// <summary>
        /// Holds the numeric representation of this token value
        /// </summary>
        public readonly object Number = null;
        /// <summary>
        /// Specifies the type of value stored in the <see cref="Number"/> field. (int or float)
        /// </summary>
        public readonly ENumericTokenType DataType = ENumericTokenType.Number;
        /// <summary>
        /// Holds the dimension's unit type string
        /// </summary>
        public readonly string Unit = null;

        public DimensionToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, object Number, ReadOnlySpan<char> Unit) : base(ECssTokenType.Dimension, Value)
        {
            this.Number = Number;
            this.DataType = DataType;
            this.Unit = Unit.ToString();
        }

        public override string Encode()
        {
            return string.Concat(Value, Unit);
        }


        #region Equality Operators
        public override bool Equals(object o)
        {
            if (o is DimensionToken Other)
            {
                return Type == Other.Type && Unit.Equals(Other.Unit, StringComparison.OrdinalIgnoreCase) && Value.Equals(Other.Value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        #endregion
    }
}
