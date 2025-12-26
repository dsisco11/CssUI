using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for env() function references, avoiding boxing of the <see cref="CssEnvFunction"/> struct.
/// </summary>
/// <remarks>
/// <para>
/// CSS env() functions reference environment variables defined by the user agent or author.
/// Unlike var(), env() variables are global to a document.
/// </para>
/// <para>
/// UA-defined environment variables include:
/// - safe-area-inset-* (top, right, bottom, left)
/// - viewport-segment-* (width, height, top, left, bottom, right)
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-env-1/
/// </para>
/// <para>
/// This subclass stores the <see cref="CssEnvFunction"/> directly, eliminating the boxing
/// overhead of <see cref="CssValueData.ObjectValue"/>.
/// </para>
/// </remarks>
public sealed class CssEnvValue : CssValue
{
    private readonly CssEnvFunction _value;

    /// <summary>
    /// Gets the env() function reference.
    /// </summary>
    public CssEnvFunction Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => _value.IsValid;

    /// <summary>
    /// Creates a new <see cref="CssEnvValue"/> with the specified env() function.
    /// </summary>
    /// <param name="envFunction">The env() function reference.</param>
    internal CssEnvValue(CssEnvFunction envFunction) : base(ECssValueTypes.ENV)
    {
        _value = envFunction;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the CSS serialization: env(name) or env(name index0 index1, fallback)
    /// </remarks>
    public override string ToString() => _value.ToString();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS env() function per CSS Environment Variables Level 1.
    /// </remarks>
    public override string Serialize() => _value.ToString();

    /// <summary>
    /// Returns the env() function directly (without boxing).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssEnvFunction AsEnvFunction() => _value;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return _value.TryFormat(destination, out charsWritten, format, provider);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is CssEnvValue other)
            return _value.Equals(other._value);

        if (obj is CssValue cssVal && cssVal.Type == ECssValueTypes.ENV)
            return _value.Equals(cssVal.AsEnvFunction());

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();
}
