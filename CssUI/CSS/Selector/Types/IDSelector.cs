using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;

public class IDSelector : SimpleSelector
{
    readonly string MatchID;

    public IDSelector(string MatchID) : base(ESimpleSelectorType.IDSelector)
    {
        this.MatchID = MatchID;
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index.
    /// ID matching is case-insensitive for HTML documents but case-sensitive for XML.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        // Use case-insensitive comparison for HTML compatibility
        return MatchID.AsSpan().Equals(E.id.AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: ID selector serializes as "#" followed by the ID as identifier
        charsWritten = 0;

        if (destination.Length < 1)
            return false;
        destination[0] = '#';
        charsWritten = 1;

        // Serialize the ID as an identifier
        // @todo: Proper identifier escaping per CSSOM §2.1
        if (!MatchID.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += MatchID.Length;

        return true;
    }
    #endregion
}

