using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;


/// <summary>
/// A type-selector matches an elements <see cref="Element.tagName"/>
/// </summary>
public class TypeSelector : SimpleSelector
{
    #region Properties
    /// <summary>
    /// The namespace to restrict this type matcher too.
    /// <para>'*' if it matches ANY namespace</para>
    /// <para></para>
    /// </summary>
    readonly string? Namespace;
    readonly string TypeName;
    #endregion

    #region Constructors
    public TypeSelector(string TypeName) : base(ESimpleSelectorType.TypeSelector)
    {
        if (string.Compare("*", TypeName) == 0) throw new CssParserException("Caught attempt to create a TypeSelector with the UniversalSelector symbol(*)!");
        this.Namespace = "*";// Match ANY namespace
        this.TypeName = TypeName;
    }

    public TypeSelector(string Namespace, string TypeName) : base(ESimpleSelectorType.TypeSelector)
    {
        this.Namespace = Namespace;
        this.TypeName = TypeName;
    }

    public TypeSelector(NamespacePrefixToken? Namespace, string TypeName) : base(ESimpleSelectorType.TypeSelector)
    {
        this.Namespace = Namespace?.Value;
        this.TypeName = TypeName;
    }
    #endregion

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        if (Namespace is not null)
        {
            if (!Namespace.AsSpan().Equals("*".AsSpan(), StringComparison.OrdinalIgnoreCase))
            {// Perform namespace matching
            }
        }
        else
        {// ONLY match null namespaces (what?!?)
        }

        return TypeName.AsSpan().Equals(E.tagName.AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: type selector serializes as the element name (lowercase)
        // If namespace prefix maps to non-default namespace, prefix with "ns|"
        charsWritten = 0;

        // Handle namespace prefix (if not "*" which means any namespace / default)
        if (Namespace is not null && !Namespace.Equals("*", StringComparison.Ordinal))
        {
            if (!Namespace.AsSpan().TryCopyTo(destination))
                return false;
            charsWritten += Namespace.Length;

            if (destination.Length <= charsWritten)
                return false;
            destination[charsWritten] = '|';
            charsWritten++;
        }

        // Serialize the element name (as identifier, lowercase)
        var typeName = TypeName.ToLowerInvariant();
        if (!typeName.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += typeName.Length;

        return true;
    }
    #endregion
}

