using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for CSS function types, avoiding boxing of the function reference.
/// </summary>
/// <remarks>
/// <para>
/// CSS functions are used throughout CSS for various purposes like colors (rgb(), hsl()),
/// gradients (linear-gradient()), transforms (rotate(), scale()), filters (blur()), etc.
/// </para>
/// <para>
/// This subclass stores the <see cref="CssFunction"/> directly, eliminating the boxing
/// overhead of <see cref="CssValueData.ObjectValue"/>.
/// </para>
/// </remarks>
internal sealed class CssFunctionValue : CssValue
{
    private readonly CssFunction _function;

    /// <summary>
    /// Gets the CSS function.
    /// </summary>
    public CssFunction Function => _function;

    /// <inheritdoc/>
    public override bool HasValue => _function is not null;

    /// <summary>
    /// Creates a new <see cref="CssFunctionValue"/> with the specified function.
    /// </summary>
    /// <param name="function">The CSS function.</param>
    internal CssFunctionValue(CssFunction function) : base(ECssValueTypes.FUNCTION)
    {
        _function = function ?? throw new ArgumentNullException(nameof(function));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the CSS serialization: name(args)
    /// </remarks>
    public override string ToString() => _function.Encode();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _function.Encode();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS function: name(args)
    /// </remarks>
    public override string Serialize() => _function.Encode();

    /// <summary>
    /// Returns the function directly (without boxing).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    internal new CssFunction? AsFunction() => _function;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        // Fall back to string allocation for function serialization
        var serialized = _function.Encode();
        if (destination.Length < serialized.Length)
        {
            charsWritten = 0;
            return false;
        }

        serialized.AsSpan().CopyTo(destination);
        charsWritten = serialized.Length;
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is CssFunctionValue other)
            return ReferenceEquals(_function, other._function) ||
                   string.Equals(_function.Encode(), other._function.Encode(), StringComparison.Ordinal);

        if (obj is CssValue cssVal && cssVal.Type == ECssValueTypes.FUNCTION)
        {
            var otherFunc = cssVal.AsFunction();
            if (otherFunc is null) return false;
            return ReferenceEquals(_function, otherFunc) ||
                   string.Equals(_function.Encode(), otherFunc.Encode(), StringComparison.Ordinal);
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => _function.Encode().GetHashCode();
}
