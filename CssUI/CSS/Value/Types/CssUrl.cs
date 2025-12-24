using System;

namespace CssUI.CSS;

/// <summary>
/// Represents an immutable CSS URL value.
/// </summary>
/// <remarks>
/// Per CSS Syntax Level 3:
/// - url() with unquoted value is tokenized as <url-token>
/// - url("...") with quoted string is tokenized as <function-token>
/// Both forms are supported and normalized to this type.
/// Docs: https://www.w3.org/TR/css-syntax-3/#url-token-diagram
/// </remarks>
public readonly record struct CssUrl : IEquatable<CssUrl>
{
    #region Fields
    /// <summary>
    /// The URL string value (without the url() wrapper and any quotes).
    /// </summary>
    private readonly string _value;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the URL value string.
    /// </summary>
    public string Value => _value ?? string.Empty;

    /// <summary>
    /// Gets whether this URL is empty or unset.
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(_value);
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="CssUrl"/> with the specified URL value.
    /// </summary>
    /// <param name="url">The URL string (without quotes or url() wrapper).</param>
    public CssUrl(string url)
    {
        _value = url ?? string.Empty;
    }

    /// <summary>
    /// Creates a new <see cref="CssUrl"/> from a ReadOnlySpan of characters.
    /// </summary>
    /// <param name="url">The URL characters.</param>
    public CssUrl(ReadOnlySpan<char> url)
    {
        _value = url.ToString();
    }
    #endregion

    #region Static Factory Methods
    /// <summary>
    /// Creates a <see cref="CssUrl"/> from a URL string value.
    /// </summary>
    /// <param name="url">The URL string.</param>
    /// <returns>A new <see cref="CssUrl"/> instance.</returns>
    public static CssUrl FromString(string url) => new(url);

    /// <summary>
    /// Creates an empty <see cref="CssUrl"/>.
    /// </summary>
    public static CssUrl Empty => new(string.Empty);
    #endregion

    #region Parsing
    /// <summary>
    /// Attempts to parse a URL from a CSS function with quoted string argument.
    /// Handles: url("path"), url('path')
    /// </summary>
    /// <param name="func">The CSS function token.</param>
    /// <param name="result">The parsed URL if successful.</param>
    /// <returns>True if the function was a valid url() with a quoted string.</returns>
    internal static bool TryFromFunction(CssFunction func, out CssUrl result)
    {
        result = Empty;

        if (func is null)
            return false;

        // Check for url() function name (case-insensitive per CSS spec)
        if (!func.Name.Equals("url", StringComparison.OrdinalIgnoreCase))
            return false;

        // The url() function should have a single string argument
        var args = func.Arguments;
        if (args is null || args.Count == 0)
        {
            // url() with no args is valid - empty URL
            result = Empty;
            return true;
        }

        // Skip whitespace to find the string argument
        CssUI.CSS.Parser.CssToken? stringToken = null;
        foreach (var token in args)
        {
            if (token is Parser.WhitespaceToken)
                continue;

            if (token is Parser.StringToken str)
            {
                stringToken = str;
                break;
            }

            // url() function form only accepts quoted strings
            // Raw URLs come as <url-token>, not <function-token>
            return false;
        }

        if (stringToken is Parser.StringToken strTok)
        {
            result = new CssUrl(strTok.Value);
            return true;
        }

        // No string found - this is valid, empty URL
        result = Empty;
        return true;
    }
    #endregion

    #region Serialization
    /// <summary>
    /// Returns the CSS serialization of this URL value.
    /// </summary>
    /// <remarks>
    /// Per CSS Syntax Level 3 serialization rules, URLs should be serialized
    /// using the quoted form: url("...")
    /// </remarks>
    public string ToCssString()
    {
        // Escape any special characters in the URL for CSS serialization
        // For now, use simple double-quote wrapping
        // @todo: Implement proper CSS escape handling per spec
        return $"url(\"{EscapeForCss(Value)}\")";
    }

    /// <summary>
    /// Returns just the URL string value.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Escapes special characters in a URL string for CSS serialization.
    /// </summary>
    private static string EscapeForCss(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Per CSS Syntax Level 3, we need to escape:
        // - Backslash (\)
        // - Quotes (") when using double quotes
        // - Newlines
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\A ")
            .Replace("\r", "\\D ");
    }
    #endregion

    #region Equality
    /// <summary>
    /// Checks equality with another <see cref="CssUrl"/>.
    /// </summary>
    public bool Equals(CssUrl other) =>
        string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <summary>
    /// Gets the hash code for this URL.
    /// </summary>
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    #endregion

    #region Operators
    /// <summary>
    /// Implicit conversion from string to <see cref="CssUrl"/>.
    /// </summary>
    public static implicit operator CssUrl(string url) => new(url);

    /// <summary>
    /// Implicit conversion from <see cref="CssUrl"/> to string.
    /// </summary>
    public static implicit operator string(CssUrl url) => url.Value;
    #endregion
}
