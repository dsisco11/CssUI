namespace CssUI.CSS.Parser
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
            return new string(Value, 1);
        }

        #region Equality Operators
        public override bool Equals(object o)
        {
            if (o is DelimToken Other)
            {
                return Type == Other.Type && Value == Other.Value;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion
    }
}
