using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;


/// <summary>
/// A universal selector matches any element in any namespace
/// <para>Universal-selectors MUST be seperate from the Type-selector class because they are ignores when calculating the selectors specificity!</para>
/// </summary>
public class UniversalSelector : SimpleSelector
{
    public UniversalSelector() : base(ESimpleSelectorType.UniversalSelector)
    {
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        return true;
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: universal selector serializes as "*"
        if (destination.Length < 1)
        {
            charsWritten = 0;
            return false;
        }

        destination[0] = '*';
        charsWritten = 1;
        return true;
    }
    #endregion
}

