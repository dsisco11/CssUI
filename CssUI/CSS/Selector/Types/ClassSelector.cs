using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.Enums;

namespace CssUI.CSS.Selectors;


public class ClassSelector : SimpleSelector
{
    readonly string ClassName;

    public ClassSelector(string ClassName) : base(ESimpleSelectorType.ClassSelector)
    {
        // Class names are case-sensitive per HTML5/CSS spec
        this.ClassName = ClassName;
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index.
    /// Class matching is case-sensitive per CSS Selectors Level 4 spec.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        return E.classList.Contains(ClassName);
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: class selector serializes as "." followed by the class name as identifier
        charsWritten = 0;

        if (destination.Length < 1)
            return false;
        destination[0] = '.';
        charsWritten = 1;

        // Serialize the class name as an identifier
        // @todo: Proper identifier escaping per CSSOM §2.1
        if (!ClassName.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += ClassName.Length;

        return true;
    }
    #endregion
}

