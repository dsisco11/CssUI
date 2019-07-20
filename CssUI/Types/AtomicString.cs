using CssUI.Enums;
using System;
using System.Linq;

namespace CssUI
{
    /// <summary>
    /// Wrapper around the string class that caches the value from GetHashCode() to improve string lookup performance
    /// </summary>
    public sealed class AtomicString
    {
        #region Properties
        /// <summary>
        /// Stores the case sensitive hash
        /// </summary>
        private int? Hash = null;
        private int? _caseless_hash = null;
        /// <summary>
        /// Stores the case-insensitive hash
        /// </summary>
        private int Hash_Lower
        {
            get
            {
                if (_caseless_hash.HasValue)
                    return _caseless_hash.Value;

                if (0 != (Flags & EAtomicStringFlags.HasUppercase))
                {// This atomic string has uppercase characters so we do infact need to create the caseless-hash
                    _caseless_hash = String.ToLowerInvariant().GetHashCode();
                }

                return GetHashCode();
            }
        }
        private readonly string String = string.Empty;
        private readonly EAtomicStringFlags Flags = 0x0;
        #endregion

        #region Constructors
        public AtomicString(string String)
        {
            this.String = String;
        }
        public AtomicString(string String, EAtomicStringFlags Flags)
        {
            this.String = String;
            this.Flags = Flags;

            if (String.ToCharArray().Where(c => char.IsUpper(c)).Any())
                Flags |= EAtomicStringFlags.HasUppercase;
        }

        #endregion

        #region String casting
        public override string ToString() { return String; }
        public static implicit operator string(AtomicString atom) { return atom.String; }
        public static implicit operator AtomicString(string str) { return new AtomicString(str); }
        #endregion


        public override int GetHashCode()
        {
            if (!Hash.HasValue)
            {
                if (0 != (Flags & EAtomicStringFlags.CaseInsensitive | EAtomicStringFlags.HasUppercase))
                {// This atomic-string wants to always be compared case insensitevly, but has uppercase character in it's string
                    Hash = String.ToLowerInvariant().GetHashCode();
                }
                else
                {
                    Hash = String.GetHashCode();
                }
            }
            return Hash.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is AtomicString atom)
            {
                // Check if hashes match
                if (0 != ((atom.Flags ^ Flags) & EAtomicStringFlags.CaseInsensitive))
                {/* Flag mismatch: the XOR of both flags still returned CaseInsensitive, meaning only one of these atomic-strings is trying to be case-insensitive */
                    return atom.Hash_Lower == Hash_Lower;
                }
                else
                {
                    return atom.GetHashCode() == GetHashCode();
                }
            }

            return false;
        }

        public static bool operator ==(AtomicString A, AtomicString B)
        {
            // If both object are NULL they match
            if (ReferenceEquals(null, A) && ReferenceEquals(null, B)) return true;
            // If one object is null and not the other they do not match
            if (ReferenceEquals(null, A) ^ ReferenceEquals(null, B)) return false;
            // Check if hashes match
            if (0 != ((A.Flags ^ B.Flags) & EAtomicStringFlags.CaseInsensitive))
            {/* Flag mismatch: the XOR of both flags still returned CaseInsensitive, meaning only one of these atomic-strings is trying to be case-insensitive */
                return A.Hash_Lower == B.Hash_Lower;
            }
            else
            {
                return A.GetHashCode() == B.GetHashCode();
            }
        }

        public static bool operator !=(AtomicString A, AtomicString B)
        {
            // If both object are null they match
            if (ReferenceEquals(null, A) && ReferenceEquals(null, B)) return false;
            // If one object is null and not the other they do not match
            if (ReferenceEquals(null, A) ^ ReferenceEquals(null, B)) return true;
            // Check if hashes match
            if (0 != ((A.Flags ^ B.Flags) & EAtomicStringFlags.CaseInsensitive))
            {/* Flag mismatch: the XOR of both flags still returned CaseInsensitive, meaning only one of these atomic-strings is trying to be case-insensitive */
                return A.Hash_Lower != B.Hash_Lower;
            }
            else
            {
                return A.GetHashCode() != B.GetHashCode();
            }
        }
    }
}
