using System;

namespace CssUI.CSS.Parser
{
    /// <summary>
    /// Implements a token type that holds a value comprised on zero or more characters
    /// </summary>
    public abstract class ValuedTokenBase : CssToken
    {
        #region Properties
        /// <summary>
        /// Holds the string representation of this tokens value
        /// </summary>
        public readonly string Value = null;
        #endregion

        #region Constructors
        public ValuedTokenBase(ECssTokenType Type, ReadOnlySpan<char> Value, bool AutoLowercase = true) : base(Type)
        {
            if (AutoLowercase)
            {// CSS is case-insensitive
                this.Value = StringCommon.Transform(Value, UnicodeCommon.To_ASCII_Lower_Alpha);
            }
            else this.Value = Value.ToString();
        }
        #endregion

        public override bool Equals(object o)
        {
            if (o is ValuedTokenBase other)
            {
                return Type == other.Type && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override String Encode()
        {
            return Value;
        }

    }
}
