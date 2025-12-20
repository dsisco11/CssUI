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
}

