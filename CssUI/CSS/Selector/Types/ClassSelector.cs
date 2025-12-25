using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;
using CssUI.Enums;

namespace CssUI.CSS.Selectors;


public class ClassSelector : SimpleSelector
{
    readonly AtomicString ClassName;

    public ClassSelector(string ClassName) : base(ESimpleSelectorType.ClassSelector)
    {
        // Store as AtomicString with CaseInsensitive flag for proper comparison
        this.ClassName = new AtomicString(ClassName.AsMemory(), EAtomicStringFlags.CaseInsensitive);
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index.
    /// Class matching is case-insensitive for HTML documents.
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
        var classNameStr = ClassName.ToString();
        if (!classNameStr.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += classNameStr.Length;

        return true;
    }
    #endregion
}

