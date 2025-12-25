using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;

public class PseudoElementSelector : SimpleSelector
{
    protected readonly string Name;

    public PseudoElementSelector(string Name) : base(ESimpleSelectorType.PseudoElementSelector)
    {
        this.Name = Name;
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        switch (Name)
        {
            default:
                throw new NotImplementedException($"Selector pseudo-element ({Name}) logic not implemented!");
        }
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: pseudo-element serializes as "::" followed by the name
        charsWritten = 0;

        if (destination.Length < 2)
            return false;
        destination[0] = ':';
        destination[1] = ':';
        charsWritten = 2;

        if (!Name.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += Name.Length;

        return true;
    }
    #endregion
}

