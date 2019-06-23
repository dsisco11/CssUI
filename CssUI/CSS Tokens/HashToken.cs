
namespace CssUI.CSS
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
        
        public HashToken(EHashTokenType HashType, string Value) : base(ECssTokenType.Hash, Value)
        {
            this.HashType = HashType;
        }
        
        public override string Encode()
        {
            return this.Value;
        }


        #region Equality Operators
        public static bool operator ==(HashToken A, HashToken B)
        {
            return (A.Type == B.Type && A.Value == B.Value && A.HashType == B.HashType);
        }

        public static bool operator !=(HashToken A, HashToken B)
        {
            return (A.Type != B.Type && A.Value != B.Value && A.HashType != B.HashType);
        }

        public override bool Equals(object o)
        {

            if (o is HashToken)
            {
                return this == (HashToken)o;
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
