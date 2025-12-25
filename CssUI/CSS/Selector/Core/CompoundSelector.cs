using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CssUI.CSS.Internal;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;


/// <summary>
/// A Compound selector is one that consists of multiple simple selectors
/// In addition a Compound selector can contain at most ONE type-selector and if it does, that must be the first selector within it.
/// </summary>
public class CompoundSelector : List<SimpleSelector>, ISpanFormattable
{/* Docs: https://drafts.csswg.org/selectors-4/#typedef-compound-selector */

    #region Constructors
    public CompoundSelector()
    {
    }

    public CompoundSelector(IEnumerable<SimpleSelector> Collection) : base(Collection)
    {
        SimpleSelector? ts = this.FirstOrDefault(o => o is TypeSelector);
        if (ts != null && ts != this[0] || this.Count(o => o is TypeSelector) > 1)
            throw new CssSyntaxErrorException("Compound selectors can only contain a single type-selector and it MUST be the first selector in the list!");
    }

    public CompoundSelector(CompoundSelector Compound) : this(Compound.ToArray())
    {
    }
    #endregion

    #region Matching
    /// <summary>
    /// Performs simple selector matching against all elements in <paramref name="MatchList"/>.
    /// Modifies the list, removing any non-matching elements
    /// </summary>
    /// <param name="MatchList">List of elements to match against</param>
    /// <param name="Order">The matching direction</param>
    /// <returns>True if any elements were a match</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool Match(IEnumerable<Element> MatchList, out LinkedList<Element> outList, ESelectorMatchingOrder Order, params Node[] scopeElements)
    {
        if (!MatchList.Any())
        {
            outList = (LinkedList<Element>)MatchList;
            return false;
        }

        var newList = new LinkedList<Element>();
        foreach (Element element in MatchList)
        {
            bool fullMatch = true;
            switch (Order)
            {
                case ESelectorMatchingOrder.LTR:
                    {
                        for (int i = 0; i < Count; i++)// progressing forwards
                        {
                            SimpleSelector Selector = this[i];
                            if (!Selector.Matches(element, scopeElements))
                            {
                                fullMatch = false;
                                break;
                            }
                        }
                    }
                    break;
                case ESelectorMatchingOrder.RTL:
                    {
                        for (int i = Count - 1; i >= 0; i--)// progressing backwards
                        {
                            SimpleSelector Selector = this[i];
                            if (!Selector.Matches(element, scopeElements))
                            {
                                fullMatch = false;
                                break;
                            }
                        }
                    }
                    break;
            }

            if (fullMatch)
            {
                newList.AddLast(element);
            }
        }

        outList = newList;
        return (newList.Count > 0);
    }

    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: compound selector serializes as concatenation of its simple selectors
        // If there is only one simple selector and it's a universal selector, serialize it.
        // Otherwise, omit universal selector if its namespace maps to default namespace.
        charsWritten = 0;

        if (Count == 0)
            return true;

        // Special case: if only one selector and it's universal, serialize it
        if (Count == 1 && this[0] is UniversalSelector)
        {
            return this[0].TryFormat(destination, out charsWritten, format, provider);
        }

        // Serialize each simple selector (skipping universal selector if not the only one)
        for (int i = 0; i < Count; i++)
        {
            var selector = this[i];

            // Skip universal selector when it's not the only selector
            // (universal selector with default namespace is implied)
            if (selector is UniversalSelector && Count > 1)
                continue;

            if (!selector.TryFormat(destination[charsWritten..], out int written, format, provider))
                return false;
            charsWritten += written;
        }

        return true;
    }

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // Estimate buffer size
        int estimatedSize = Count * 32;
        char[]? rented = null;
        Span<char> buffer = estimatedSize <= 256
            ? stackalloc char[256]
            : (rented = ArrayPool<char>.Shared.Rent(estimatedSize));

        try
        {
            if (TryFormat(buffer, out int charsWritten, format.AsSpan(), formatProvider))
                return new string(buffer[..charsWritten]);

            // Fallback for unexpectedly large output
            rented = ArrayPool<char>.Shared.Rent(estimatedSize * 4);
            buffer = rented;
            if (TryFormat(buffer, out charsWritten, format.AsSpan(), formatProvider))
                return new string(buffer[..charsWritten]);

            throw new InvalidOperationException("Buffer too small for CompoundSelector serialization");
        }
        finally
        {
            if (rented != null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }

    /// <inheritdoc/>
    public override string ToString() => ToString(null, null);
    #endregion
}

