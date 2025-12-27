using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for URL types, avoiding boxing of the <see cref="CssUrl"/> struct.
/// </summary>
/// <remarks>
/// <para>
/// CSS URLs are used in properties like <c>background-image</c>, <c>list-style-image</c>,
/// <c>cursor</c>, <c>@import</c>, etc. They use the <c>url()</c> function syntax.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-syntax-3/#url-token-diagram
/// </para>
/// <para>
/// This subclass stores the <see cref="CssUrl"/> directly, eliminating the boxing
/// overhead of <see cref="CssValueData.ObjectValue"/>.
/// </para>
/// </remarks>
public sealed record class CssUrlValue : CssValue
{
    private readonly CssUrl _value;

    /// <summary>
    /// Gets the URL value.
    /// </summary>
    public CssUrl Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => !_value.IsEmpty;

    /// <summary>
    /// Creates a new <see cref="CssUrlValue"/> with the specified URL.
    /// </summary>
    /// <param name="url">The CSS URL value.</param>
    internal CssUrlValue(CssUrl url) : base(ECssValueTypes.URL)
    {
        _value = url;
    }

    /// <summary>
    /// Creates a new <see cref="CssUrlValue"/> from a URL string.
    /// </summary>
    /// <param name="url">The URL string.</param>
    internal CssUrlValue(string url) : base(ECssValueTypes.URL)
    {
        _value = new CssUrl(url);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the CSS serialization: url("...")
    /// </remarks>
    public override string ToString() => _value.ToCssString();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _value.ToCssString();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS url() function per CSS Syntax Level 3.
    /// </remarks>
    public override string Serialize() => _value.ToCssString();

    /// <summary>
    /// Returns the URL value directly (without boxing).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override CssUrl AsUrl() => _value;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        return _value.TryFormat(destination, out charsWritten, format, provider);
    }
}
