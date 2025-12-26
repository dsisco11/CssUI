using System;
using System.Collections.Generic;

namespace CssUI.CSS;

/// <summary>
/// A strongly-typed CSS value representing an enum keyword.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
/// <remarks>
/// This subclass stores the enum value unboxed and pre-computes the keyword string
/// at construction time, eliminating reflection during serialization.
/// </remarks>
public sealed class CssEnumValue<T> : CssValue where T : struct, Enum
{
    private readonly T _value;
    private readonly string _keyword;

    /// <summary>
    /// Gets the strongly-typed enum value.
    /// </summary>
    public T Value => _value;

    /// <summary>
    /// Gets the pre-computed keyword string for this enum value.
    /// </summary>
    public string Keyword => _keyword;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssEnumValue{T}"/> with the specified enum value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    internal CssEnumValue(T value) : base(ECssValueTypes.KEYWORD)
    {
        _value = value;
        // Use Lookup.Keyword which handles generic enum→keyword conversion
        // This is called once at construction, not during every serialization
        _keyword = Lookup.Keyword<T>(value);
    }

    /// <inheritdoc/>
    public override string ToString() => _keyword;

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _keyword;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        if (destination.Length < _keyword.Length)
        {
            charsWritten = 0;
            return false;
        }

        _keyword.AsSpan().CopyTo(destination);
        charsWritten = _keyword.Length;
        return true;
    }

    /// <inheritdoc/>
    public override string Serialize() => _keyword;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is CssEnumValue<T> other)
            return EqualityComparer<T>.Default.Equals(_value, other._value);

        if (obj is CssValue cssVal && cssVal.Type == ECssValueTypes.KEYWORD)
        {
            // Fallback: compare with boxed enum via AsEnum<T>()
            try
            {
                return EqualityComparer<T>.Default.Equals(_value, cssVal.AsEnum<T>());
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the enum value directly without boxing when <typeparamref name="TEnum"/> matches <typeparamref name="T"/>.
    /// Falls back to conversion for mismatched types.
    /// </remarks>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override TEnum AsEnum<TEnum>()
    {
        // Fast path: if types match exactly, return unboxed value
        if (typeof(TEnum) == typeof(T))
            return System.Runtime.CompilerServices.Unsafe.As<T, TEnum>(ref System.Runtime.CompilerServices.Unsafe.AsRef(in _value));

        // Mismatched type: box and convert
        return (TEnum)(object)_value;
    }
}
