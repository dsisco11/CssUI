using System;

namespace CssUI.CSS;

/// <summary>
/// A specialized CSS value for CSS string types, avoiding boxing of the string reference.
/// </summary>
/// <remarks>
/// <para>
/// CSS strings are delimited by quotation marks and are used in properties like
/// <c>content</c>, <c>font-family</c> (quoted names), custom properties, etc.
/// </para>
/// <para>
/// Spec: https://www.w3.org/TR/css-values-4/#strings
/// </para>
/// <para>
/// This subclass stores the string directly, eliminating the boxing overhead
/// of <see cref="CssValueData.ObjectValue"/> and providing optimized serialization.
/// </para>
/// </remarks>
public sealed class CssStringValue : CssValue
{
    private readonly string _value;

    /// <summary>
    /// Gets the raw string value (without quotes).
    /// </summary>
    public string Value => _value;

    /// <inheritdoc/>
    public override bool HasValue => true;

    /// <summary>
    /// Creates a new <see cref="CssStringValue"/> with the specified string.
    /// </summary>
    /// <param name="value">The string value (without surrounding quotes).</param>
    internal CssStringValue(string value) : base(ECssValueTypes.STRING)
    {
        _value = value ?? string.Empty;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the string without quotes for display purposes.
    /// Use <see cref="Serialize"/> for CSS-compliant quoted output.
    /// </remarks>
    public override string ToString() => SerializeQuoted();

    /// <inheritdoc/>
    public override string ToString(string? format, IFormatProvider? formatProvider) => SerializeQuoted();

    /// <inheritdoc/>
    /// <remarks>
    /// Serializes as a CSS quoted string per CSS Values Level 4 specification.
    /// Uses double quotes and escapes special characters as needed.
    /// </remarks>
    public override string Serialize() => SerializeQuoted();

    /// <summary>
    /// Returns the string value directly (without quotes).
    /// </summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public override string AsString() => _value;

    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        // CSS strings are serialized with surrounding quotes
        // Need space for: quote + string + quote (minimum, without escaping)
        int minRequired = 2 + _value.Length;

        if (destination.Length < minRequired)
        {
            charsWritten = 0;
            return false;
        }

        // For efficiency, check if escaping is needed
        if (!NeedsEscaping(_value))
        {
            // Fast path: no escaping needed
            destination[0] = UnicodeCommon.CHAR_QUOTATION_MARK;
            _value.AsSpan().CopyTo(destination[1..]);
            destination[1 + _value.Length] = UnicodeCommon.CHAR_QUOTATION_MARK;
            charsWritten = minRequired;
            return true;
        }

        // Slow path: needs escaping, fall back to string allocation
        var serialized = SerializeQuoted();
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
        if (obj is CssStringValue other)
            return string.Equals(_value, other._value, StringComparison.Ordinal);

        if (obj is CssValue cssVal && cssVal.Type == ECssValueTypes.STRING)
            return string.Equals(_value, cssVal.AsString(), StringComparison.Ordinal);

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Serializes the string with CSS-compliant quoting and escaping.
    /// </summary>
    private string SerializeQuoted()
    {
        // Per CSS spec, strings are delimited by double or single quotes
        // We use double quotes and escape: " \ newline
        // Spec: https://www.w3.org/TR/css-syntax-3/#consume-string-token

        if (!NeedsEscaping(_value))
        {
            return string.Concat("\"", _value, "\"");
        }

        // Escape special characters
        var escaped = EscapeString(_value);
        return string.Concat("\"", escaped, "\"");
    }

    /// <summary>
    /// Checks if the string contains characters that need CSS escaping.
    /// </summary>
    private static bool NeedsEscaping(string value)
    {
        foreach (char c in value)
        {
            if (c == '"' || c == '\\' || c == '\n' || c == '\r' || c == '\f')
                return true;
        }
        return false;
    }

    /// <summary>
    /// Escapes special characters per CSS string escaping rules.
    /// </summary>
    /// <remarks>
    /// Per CSS Syntax Level 3:
    /// - Backslash is escaped as \\
    /// - Double quote is escaped as \"
    /// - Newline (\n), carriage return (\r), form feed (\f) use hex escapes
    /// </remarks>
    private static string EscapeString(string value)
    {
        var builder = new System.Text.StringBuilder(value.Length * 2);

        foreach (char c in value)
        {
            switch (c)
            {
                case '"':
                    builder.Append('\\');
                    builder.Append('"');
                    break;
                case '\\':
                    builder.Append('\\');
                    builder.Append('\\');
                    break;
                case '\n':
                    builder.Append('\\');
                    builder.Append('a'); // \a in CSS = line feed
                    builder.Append(' '); // Space terminates hex escape
                    break;
                case '\r':
                    builder.Append('\\');
                    builder.Append('d'); // \d in CSS = carriage return
                    builder.Append(' ');
                    break;
                case '\f':
                    builder.Append('\\');
                    builder.Append('c'); // \c in CSS = form feed
                    builder.Append(' ');
                    break;
                default:
                    builder.Append(c);
                    break;
            }
        }

        return builder.ToString();
    }
}
