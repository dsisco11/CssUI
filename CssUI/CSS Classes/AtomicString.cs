using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Wrapper around the string class that caches the value from GetHashCode() to improve string lookup performance a bit
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

        public override string ToString() { return this.String; }

        public static implicit operator string(AtomicString atom) { return atom.String; }


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
            if (object.ReferenceEquals(null, A) && object.ReferenceEquals(null, B)) return true;
            if (object.ReferenceEquals(null, A) ^ object.ReferenceEquals(null, B)) return false;
            return (A.GetHashCode() == B.GetHashCode());
        }

        public static bool operator !=(AtomicString A, AtomicString B)
        {
            if (object.ReferenceEquals(null, A) && object.ReferenceEquals(null, B)) return false;
            if (object.ReferenceEquals(null, A) ^ object.ReferenceEquals(null, B)) return true;
            return (A.GetHashCode() != B.GetHashCode());
        }
    }
}
