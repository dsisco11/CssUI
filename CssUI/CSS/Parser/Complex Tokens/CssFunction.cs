using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CssUI.CSS.Parser;

namespace CssUI.CSS;

internal class CssFunction : CssComponent, IEquatable<CssFunction>
{
    public readonly string Name;
    //public List<CssComponent> Value = new List<CssComponent>();
    public List<CssToken> Arguments = new List<CssToken>();

    #region Constructors
    //public CssFunction(string Name) : base(ECssComponent.Function)
    public CssFunction(ReadOnlySpan<char> Name) : base(ECssTokenType.Function)
    {
        this.Name = Name.ToString();
    }
    #endregion

    public override string Encode()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Name);
        sb.Append("(");
        foreach (CssToken t in Arguments) { sb.Append(t.Encode()); }
        sb.Append(")");

        return sb.ToString();
    }

    #region Equality
    /// <summary>
    /// Determines value equality between this CssFunction and another.
    /// </summary>
    /// <remarks>
    /// Functions are equal if they have the same name and equivalent arguments.
    /// </remarks>
    public bool Equals(CssFunction? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        // Compare names (case-insensitive per CSS spec)
        if (!string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase))
            return false;

        // Compare argument counts
        if (Arguments.Count != other.Arguments.Count)
            return false;

        // Compare arguments by their encoded representation
        for (int i = 0; i < Arguments.Count; i++)
        {
            if (Arguments[i].Encode() != other.Arguments[i].Encode())
                return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as CssFunction);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Name, StringComparer.OrdinalIgnoreCase);
        hash.Add(Arguments.Count);
        foreach (var arg in Arguments)
        {
            hash.Add(arg.Encode());
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(CssFunction? left, CssFunction? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(CssFunction? left, CssFunction? right)
        => !(left == right);
    #endregion
}

