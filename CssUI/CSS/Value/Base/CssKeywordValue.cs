using System;

namespace CssUI.CSS;

/// <summary>
/// A CSS value representing an untyped keyword (identifier) string.
/// </summary>
/// <remarks>
/// <para>
/// This class handles CSS identifiers (keywords) that are parsed from CSS input but
/// are not associated with a strongly-typed enum. Examples include:
/// </para>
/// <list type="bullet">
/// <item><description>Unknown identifiers from CSS input</description></item>
/// <item><description>The <c>currentColor</c> keyword</description></item>
/// <item><description>Fallback values in <c>var()</c> and <c>env()</c> functions</description></item>
/// <item><description>Custom identifiers in properties like <c>font-family</c></description></item>
/// </list>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#keywords
/// </para>
/// <para>
/// For strongly-typed enum keywords (e.g., <c>flex-direction: row</c>), use
/// <see cref="CssEnumValue{T}"/> instead, which provides type-safe access.
/// </para>
/// </remarks>
public sealed record class CssKeywordValue : CssValue
{
    private readonly string _keyword;

    /// <summary>
    /// Gets the keyword string.
    /// </summary>
    public string Keyword => _keyword;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssKeywordValue"/> with the specified keyword string.
    /// </summary>
    /// <param name="keyword">The keyword identifier.</param>
    internal CssKeywordValue(string keyword) : base(ECssValueTypes.KEYWORD)
    {
        _keyword = keyword ?? string.Empty;
    }

    /// <inheritdoc/>
    public override string ToString() => _keyword;

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => _keyword;

    /// <inheritdoc/>
    public override string Serialize() => _keyword;

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
    /// <remarks>
    /// Returns the keyword string directly. For strongly-typed enum access,
    /// use <see cref="CssEnumValue{T}"/> instead.
    /// </remarks>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override string AsKeyword() => _keyword;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">
    /// Always thrown. <see cref="CssKeywordValue"/> stores untyped string keywords.
    /// Use <see cref="CssEnumValue{T}"/> for strongly-typed enum keywords.
    /// </exception>
    public override TEnum AsEnum<TEnum>()
    {
        throw new InvalidOperationException(
            $"Cannot convert untyped keyword '{_keyword}' to enum {typeof(TEnum).Name}. " +
            $"Use {nameof(CssEnumValue<TEnum>)} for strongly-typed keyword values, or check the keyword string via {nameof(AsKeyword)}().");
    }

    /// <inheritdoc/>
    public bool Equals(CssKeywordValue? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(_keyword, other._keyword, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Type, _keyword);
}
