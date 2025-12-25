using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;

/// <summary>
/// A simple selector is either a type selector, universal selector, attribute selector, class selector, ID selector, or pseudo-class.
/// </summary>
public abstract class SimpleSelector : ISpanFormattable
{
    public ESimpleSelectorType Type { get; protected set; }

    public SimpleSelector(ESimpleSelectorType Type)
    {
        this.Type = Type;
    }

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    abstract public bool Matches(Element E, params Node[] scopeElements);

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    public abstract bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);

    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        // Estimate buffer size for a simple selector
        int estimatedSize = 64;
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

            throw new InvalidOperationException("Buffer too small for SimpleSelector serialization");
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

