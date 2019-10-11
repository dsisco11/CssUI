using System;

namespace CssUI.CSS.Parser
{
    public sealed class NumberToken : ValuedTokenBase
    {
        /// <summary>
        /// Holds the numeric representation of this token value
        /// </summary>
        public readonly object Number = null;
        /// <summary>
        /// Specifies the type of value stored in the <see cref="Number"/> field. (int or float)
        /// </summary>
        public readonly ENumericTokenType DataType = ENumericTokenType.Number;

        public NumberToken(ENumericTokenType DataType, ReadOnlySpan<char> Value, object Number) : base(ECssTokenType.Number, Value)
        {
            this.DataType = DataType;
            this.Number = Number;
        }


        public override bool Equals(object o)
        {
            if (o is NumberToken Other)
            {
                return Type == Other.Type && DataType == Other.DataType && Value.Equals(Other.Value, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
