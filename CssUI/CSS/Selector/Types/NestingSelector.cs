using System;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;

/// <summary>
/// The nesting selector (&amp;) represents the elements matched by the parent rule's selector in CSS Nesting.
/// When used outside of nesting context, it represents the same elements as <c>:scope</c>.
/// </summary>
/// <remarks>
/// <para>
/// The nesting selector can be used anywhere in a compound selector, even before a type selector
/// (violating the normal restrictions on ordering within a compound selector).
/// For example: <c>&amp;div</c> is valid, meaning "whatever the parent rule matches, but only if it's also a div element".
/// </para>
/// <para>
/// The specificity of the nesting selector equals the largest specificity among the complex selectors
/// in the parent style rule's selector list (identical to the behavior of <c>:is()</c>).
/// </para>
/// <para>
/// The nesting selector cannot represent pseudo-elements (identical to <c>:is()</c> behavior).
/// </para>
/// SEE: https://www.w3.org/TR/css-nesting-1/#nest-selector
/// </remarks>
public sealed class NestingSelector : SimpleSelector
{
    #region Properties
    /// <summary>
    /// The parent selector that this nesting selector represents.
    /// If null, the nesting selector behaves like <c>:scope</c>.
    /// </summary>
    /// <remarks>
    /// This is set during stylesheet parsing when a nested style rule is encountered.
    /// The parent selector is resolved to an <c>:is()</c> wrapper containing the parent rule's selector list.
    /// </remarks>
    public CssSelector? ParentSelector { get; private set; }

    /// <summary>
    /// Cached specificity value computed from <see cref="ParentSelector"/>.
    /// This is the maximum specificity among all complex selectors in the parent selector list.
    /// </summary>
    private long _cachedSpecificity;
    private bool _specificityComputed;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a nesting selector without a parent context.
    /// In this state, it behaves like <c>:scope</c>.
    /// </summary>
    public NestingSelector() : base(ESimpleSelectorType.NestingSelector)
    {
        ParentSelector = null;
        _cachedSpecificity = 0;
        _specificityComputed = false;
    }

    /// <summary>
    /// Creates a nesting selector with an explicit parent selector.
    /// </summary>
    /// <param name="parentSelector">The selector from the parent style rule.</param>
    public NestingSelector(CssSelector parentSelector) : base(ESimpleSelectorType.NestingSelector)
    {
        ParentSelector = parentSelector;
        _specificityComputed = false;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets the parent selector for this nesting selector.
    /// This is typically called during stylesheet processing when resolving nested rules.
    /// </summary>
    /// <param name="parentSelector">The parent rule's selector to bind to this nesting selector.</param>
    public void SetParentSelector(CssSelector parentSelector)
    {
        ParentSelector = parentSelector;
        _specificityComputed = false; // Reset cached specificity
    }

    /// <summary>
    /// Gets the specificity of this nesting selector.
    /// Per CSS Nesting spec, this equals the largest specificity among the complex selectors
    /// in the parent style rule's selector list.
    /// </summary>
    /// <returns>The specificity value, or 0 if no parent selector is set.</returns>
    public long GetSpecificity()
    {
        if (_specificityComputed)
            return _cachedSpecificity;

        if (ParentSelector is null || ParentSelector.Count == 0)
        {
            // When no parent, behaves like :scope which has pseudo-class specificity (0,1,0)
            _cachedSpecificity = 1L << 16; // B component = 1
        }
        else
        {
            // Find the maximum specificity among all complex selectors in the parent
            long maxSpecificity = 0;
            foreach (var complexSelector in ParentSelector)
            {
                long spec = complexSelector.Get_Specificity();
                if (spec > maxSpecificity)
                    maxSpecificity = spec;
            }
            _cachedSpecificity = maxSpecificity;
        }

        _specificityComputed = true;
        return _cachedSpecificity;
    }
    #endregion

    #region Matching
    /// <summary>
    /// Returns whether the selector matches a specified element.
    /// </summary>
    /// <param name="E">The element to match against.</param>
    /// <param name="scopeElements">The scope elements for <c>:scope</c> pseudo-class matching.</param>
    /// <returns>True if the element matches this nesting selector.</returns>
    /// <remarks>
    /// If <see cref="ParentSelector"/> is set, matches if the element matches any of the parent's complex selectors.
    /// If <see cref="ParentSelector"/> is null (outside nesting context), matches elements in <paramref name="scopeElements"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Matches(Element E, params Node[] scopeElements)
    {
        if (ParentSelector is not null && ParentSelector.Count > 0)
        {
            // Match against the parent selector (behaves like :is(parent-selector))
            return ParentSelector.Match(E, scopeElements);
        }
        else
        {
            // No parent selector - behave like :scope
            // :scope matches the scoping root or the document element if no scope is set
            if (scopeElements.Length > 0)
            {
                foreach (var scope in scopeElements)
                {
                    if (ReferenceEquals(scope, E))
                        return true;
                }
                return false;
            }

            // If no scope elements provided, match the document element
            return E.ownerDocument?.documentElement == E;
        }
    }
    #endregion

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSS Nesting §3: nesting selector serializes as "&"
        if (destination.Length < 1)
        {
            charsWritten = 0;
            return false;
        }

        destination[0] = '&';
        charsWritten = 1;
        return true;
    }
    #endregion
}
