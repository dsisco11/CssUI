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
}

