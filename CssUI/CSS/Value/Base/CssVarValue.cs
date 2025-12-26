using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for var() function references, avoiding boxing of the <see cref="CssVarFunction"/> struct.
/// </summary>
/// <remarks>
/// <para>
/// CSS var() functions reference custom properties (CSS variables) defined elsewhere in the cascade.
/// They are commonly used in modern CSS for theming and dynamic styling.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-variables-1/#using-variables
/// </para>
/// <para>
/// This subclass stores the <see cref="CssVarFunction"/> directly, eliminating the boxing
/// overhead of <see cref="CssValueData.ObjectValue"/>.
/// </para>
/// </remarks>
public sealed class CssVarValue : CssValue
{
    private readonly CssVarFunction _value;

    /// <summary>
    /// Gets the var() function reference.
    /// </summary>
    public CssVarFunction Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => _value.IsValid;

    /// <summary>
    /// Creates a new <see cref="CssVarValue"/> with the specified var() function.
    /// </summary>
    /// <param name="varFunction">The var() function reference.</param>
    internal CssVarValue(CssVarFunction varFunction) : base(ECssValueTypes.VAR)
    {
        _value = varFunction;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the CSS serialization: var(--name) or var(--name, fallback)
    /// </remarks>
    public override string ToString() => _value.ToString();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS var() function per CSS Variables Level 1.
    /// </remarks>
    public override string Serialize() => _value.ToString();

    /// <summary>
    /// Returns the var() function directly (without boxing).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssVarFunction AsVarFunction() => _value;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return _value.TryFormat(destination, out charsWritten, format, provider);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is CssVarValue other)
            return _value.Equals(other._value);

        if (obj is CssValue cssVal && cssVal.Type == ECssValueTypes.VAR)
            return _value.Equals(cssVal.AsVarFunction());

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();
}
