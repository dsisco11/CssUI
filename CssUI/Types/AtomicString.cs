using System;

namespace CssUI.Internal
{
    /// <summary>
    /// Wrapper around the string class that caches the value from GetHashCode() to improve string lookup performance
    /// </summary>
    public sealed class AtomicString
    {
        #region Properties
        private int? Hash = null;
        private readonly string String = string.Empty;
        #endregion

        #region Constructors
        public AtomicString(string String)
        {
            this.String = String;
        }

        #endregion

        #region String casting
        public override string ToString() { return this.String; }
        public static implicit operator string(AtomicString atom) { return atom.String; }
        public static implicit operator AtomicString(string str) { return new AtomicString(str); }
        #endregion


        public override int GetHashCode()
        {
            if (!Hash.HasValue) Hash = String.GetHashCode();
            return Hash.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is AtomicString) return (((AtomicString)obj).Hash == this.Hash);
            return false;
        }

        public static bool operator ==(AtomicString A, AtomicString B)
        {
            // If both object are NULL they match
            if (object.ReferenceEquals(null, A) && object.ReferenceEquals(null, B)) return true;
            // If one object is null and not the other they do not match
            if (object.ReferenceEquals(null, A) ^ object.ReferenceEquals(null, B)) return false;
            // Check if hashes match
            return (A.GetHashCode() == B.GetHashCode());
        }

        public static bool operator !=(AtomicString A, AtomicString B)
        {
            // If both object are null they match
            if (object.ReferenceEquals(null, A) && object.ReferenceEquals(null, B)) return false;
            // If one object is null and not the other they do not match
            if (object.ReferenceEquals(null, A) ^ object.ReferenceEquals(null, B)) return true;
            // Check if hashes match
            return (A.GetHashCode() != B.GetHashCode());
        }
    }
}
