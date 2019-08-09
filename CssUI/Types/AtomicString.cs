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
        private CacheableValue<int> Hash = null;
        /// <summary>
        /// Stores the case-insensitive hash
        /// </summary>
        private CacheableValue<int> Hash_Lower = null;

        private readonly string String = string.Empty;
        private readonly ReadOnlyMemory<char> Data = null;
        private readonly EAtomicStringFlags Flags = 0x0;
        #endregion

        #region Constructors
        public AtomicString(ReadOnlyMemory<char> Data, EAtomicStringFlags Flags)
        {
            Hash = new CacheableValue<int>(() => {
                if (0 != (Flags & EAtomicStringFlags.CaseInsensitive | EAtomicStringFlags.HasUppercase))
                {// This atomic-string wants to always be compared case insensitevly, but has uppercase character in it's string
                    return StringCommon.Transform(Data, UnicodeCommon.To_ASCII_Lower_Alpha).GetHashCode();
                }
                else
                {
                    return new string(Data.ToArray()).GetHashCode();
                }
            });

            Hash_Lower = new CacheableValue<int>(() =>
            {
                /* Check if our string actually has uppercased characters, if it does then we need to lowercase it and get its hash */
                if (0 != (Flags & EAtomicStringFlags.HasUppercase))
                {// This atomic string has uppercase characters so we do infact need to create the caseless-hash
                    return StringCommon.Transform(Data, UnicodeCommon.To_ASCII_Lower_Alpha).GetHashCode();
                }

                return GetHashCode();
            });

            this.Data = Data;
            /*this.Data = new Memory<char>(Data.Length);
            Data.CopyTo(this.Data);*/

            this.Flags = Flags;

            if (StringCommon.Contains(Data.Span, c => char.IsUpper(c)))
            {
                Flags |= EAtomicStringFlags.HasUppercase;
            }
        }

        public AtomicString(ReadOnlyMemory<char> Data) : this(Data, 0x0) { }
        public AtomicString(string String) : this(String.AsMemory()) { }
        public AtomicString(string String, EAtomicStringFlags Flags) : this(String.AsMemory(), Flags) { }

        #endregion

        #region String casting
        public override string ToString() { return Data.ToString(); }

        public static implicit operator string(AtomicString atom) { return atom.Data.ToString(); }
        public static implicit operator ReadOnlyMemory<char>(AtomicString atom) { return atom.Data; }

        public static implicit operator AtomicString(string str) { return new AtomicString(str.AsMemory()); }
        public static implicit operator AtomicString(ReadOnlyMemory<char> memory) { return new AtomicString(memory); }
        #endregion

        #region Equality
        public override int GetHashCode()
        {
            return Hash.Get();
        }

        public bool Equals(AtomicString other)
        {
            // Check if hashes match
            if (0 != ((other.Flags ^ Flags) & EAtomicStringFlags.CaseInsensitive))
            {/* Flag mismatch: the XOR of both flags still returned CaseInsensitive, meaning only one of these atomic-strings is trying to be case-insensitive */
                return Hash_Lower == other.Hash_Lower;
            }
            else
            {
                return other.GetHashCode() == GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is AtomicString atom)
            {
                // Check if hashes match
                if (0 != ((atom.Flags ^ Flags) & EAtomicStringFlags.CaseInsensitive))
                {/* Flag mismatch: the XOR of both flags still returned CaseInsensitive, meaning only one of these atomic-strings is trying to be case-insensitive */
                    return Hash_Lower == atom.Hash_Lower;
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
            // If both object are null they do not match
            if (ReferenceEquals(null, A) && ReferenceEquals(null, B)) return false;
            // If one object is null and not the other they do match
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
        #endregion
    }
}
