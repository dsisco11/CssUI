using System;

namespace CssUI.CSS.Parser
{
    public enum EHashTokenType
    {
        Unrestricted,
        ID,
    }

    public sealed class HashToken : ValuedTokenBase
    {
        #region Properties
        public readonly EHashTokenType HashType = EHashTokenType.Unrestricted;
        #endregion

        public HashToken(EHashTokenType HashType, ReadOnlySpan<char> Value) : base(ECssTokenType.Hash, Value)
        {
            this.HashType = HashType;
        }


        #region Equality Operators
        public override bool Equals(object o)
        {
            if (o is HashToken Other)
            {
                return Type == Other.Type && HashType == Other.HashType && Value.Equals(Other.Value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        #endregion
    }
}
