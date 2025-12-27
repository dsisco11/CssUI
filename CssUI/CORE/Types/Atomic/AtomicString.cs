using System;
using CssUI.Enums;

namespace CssUI;


/// <summary>
/// Wrapper around the string class that caches the value from GetHashCode() to improve string lookup performance
/// </summary>
public sealed class AtomicString
{
    #region Properties
    /// <summary>
    /// Stores the case sensitive hash
    /// </summary>
    private CachedValue<int>? Hash = null;
    /// <summary>
    /// Stores the case-insensitive hash
    /// </summary>
    private CachedValue<int>? Hash_Lower = null;

    private readonly string String = string.Empty;
    private readonly ReadOnlyMemory<char> Data = null;
    private readonly EAtomicStringFlags Flags = 0x0;
    #endregion

    #region Constructors
    public AtomicString(ReadOnlyMemory<char> Data, EAtomicStringFlags Flags)
    {
        // Detect uppercase BEFORE creating lambdas so they capture the correct flags
        if (StringCommon.Contains(Data.Span, c => char.IsUpper(c)))
        {
            Flags |= EAtomicStringFlags.HasUppercase;
        }

        this.Data = Data;
        this.Flags = Flags;

        Hash = new CachedValue<int>(() =>
        {
            if (0 != (Flags & (EAtomicStringFlags.CaseInsensitive | EAtomicStringFlags.HasUppercase)))
            {// This atomic-string wants to always be compared case-insensitively, or has uppercase characters
                return StringCommon.Transform(Data, UnicodeCommon.To_ASCII_Lower_Alpha).GetHashCode(StringComparison.Ordinal);
            }
            else
            {
                // Compute hash from the actual string content using ordinal comparison for consistency
                return Data.ToString().GetHashCode(StringComparison.Ordinal);
            }
        });

        Hash_Lower = new CachedValue<int>(() =>
        {
            /* Check if our string actually has uppercased characters, if it does then we need to lowercase it and get its hash */
            if (0 != (Flags & EAtomicStringFlags.HasUppercase))
            {// This atomic string has uppercase characters so we do infact need to create the caseless-hash
                return StringCommon.Transform(Data, UnicodeCommon.To_ASCII_Lower_Alpha).GetHashCode(StringComparison.Ordinal);
            }

            return GetHashCode();
        });
    }

    public AtomicString(ReadOnlyMemory<char> Data) : this(Data, 0x0) { }
    public AtomicString(string String) : this(String.AsMemory()) { }
    public AtomicString(string String, EAtomicStringFlags Flags) : this(String.AsMemory(), Flags) { }

    #endregion

    #region String casting

    public ReadOnlyMemory<char> AsMemory() => Data;
    public override string ToString() => Data.ToString();

    public static implicit operator String(AtomicString atom) => atom.Data.ToString();
    public static implicit operator ReadOnlyMemory<char>(AtomicString atom) => atom.Data;

    public static implicit operator AtomicString(String? str) => new AtomicString((str ?? string.Empty).AsMemory());
    public static implicit operator AtomicString(ReadOnlyMemory<char> memory) => new AtomicString(memory);
    #endregion

    #region Equality
    public override int GetHashCode()
    {
        return Hash.Get();
    }

    public bool Equals(AtomicString other)
    {
        // Determine if case-insensitive comparison is needed (if EITHER side has the flag)
        bool caseInsensitive = 0 != ((other.Flags | Flags) & EAtomicStringFlags.CaseInsensitive);

        if (caseInsensitive)
        {
            // Quick hash check using case-insensitive hash
            if (Hash_Lower!.Get() != other.Hash_Lower!.Get()) return false;
            // Hash match - verify with actual string comparison (case-insensitive)
            return Data.Span.Equals(other.Data.Span, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            // Case-sensitive comparison
            if (other.GetHashCode() != GetHashCode()) return false;
            // Hash match - verify with actual string comparison
            return Data.Span.SequenceEqual(other.Data.Span);
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is AtomicString atom)
        {
            return Equals(atom);
        }

        return false;
    }

    public static bool operator ==(AtomicString? A, AtomicString? B)
    {
        // If both object are NULL they match
        if (A is null && B is null) return true;
        // If one object is null and not the other they do not match
        if (A is null ^ B is null) return false;
        // Use the Equals method for proper comparison
        return A!.Equals(B!);
    }

    public static bool operator !=(AtomicString? A, AtomicString? B)
    {
        return !(A == B);
    }
    #endregion
}