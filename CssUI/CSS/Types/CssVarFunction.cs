using System;
using CssUI.CSS.Parser;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS var() function reference to a custom property.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-variables-1/#using-variables
/// 
/// Grammar:
/// <code>
/// var() = var( &lt;custom-property-name&gt; [, &lt;declaration-value&gt;]? )
/// </code>
/// 
/// The var() function allows referencing custom properties (CSS variables)
/// defined elsewhere in the cascade. Custom property names must start with
/// two dashes (--).
/// 
/// Examples:
/// - var(--main-color)
/// - var(--fallback-font, sans-serif)
/// - var(--spacing, 10px)
/// - var(--nested, var(--other))
/// </remarks>
public readonly struct CssVarFunction : IEquatable<CssVarFunction>
{
    #region Static

    /// <summary>
    /// Empty var() function with no property name.
    /// </summary>
    public static readonly CssVarFunction Empty = new CssVarFunction();

    #endregion

    #region Properties

    /// <summary>
    /// The custom property name being referenced (including the -- prefix).
    /// </summary>
    /// <remarks>
    /// Per CSS Variables Level 1, custom property names:
    /// - Must start with two dashes (--)
    /// - Are case-sensitive
    /// - Cannot be just "--" (reserved for future use)
    /// </remarks>
    public string PropertyName { get; }

    /// <summary>
    /// The fallback value to use if the custom property is not defined
    /// or has the guaranteed-invalid value.
    /// </summary>
    /// <remarks>
    /// The fallback is optional. When present, it can contain any valid
    /// CSS value including commas (e.g., var(--font, Arial, sans-serif)).
    /// The fallback may also contain nested var() references.
    /// </remarks>
    public CssValue? Fallback { get; }

    /// <summary>
    /// The raw tokens that make up the fallback value.
    /// </summary>
    /// <remarks>
    /// Preserved for cases where the fallback needs to be re-parsed
    /// in context or contains nested var() references.
    /// </remarks>
    public ReadOnlyMemory<CssToken>? FallbackTokens { get; }

    /// <summary>
    /// Indicates whether this var() reference has a fallback value.
    /// </summary>
    public bool HasFallback => Fallback is not null || FallbackTokens is not null;

    /// <summary>
    /// Indicates whether the property name is a valid custom property name.
    /// </summary>
    public bool IsValid => !string.IsNullOrEmpty(PropertyName) && PropertyName.StartsWith("--", StringComparison.Ordinal);

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new var() function reference without a fallback.
    /// </summary>
    /// <param name="propertyName">The custom property name (must include -- prefix).</param>
    public CssVarFunction(string propertyName)
    {
        PropertyName = propertyName ?? string.Empty;
        Fallback = null;
        FallbackTokens = null;
    }

    /// <summary>
    /// Creates a new var() function reference with a fallback value.
    /// </summary>
    /// <param name="propertyName">The custom property name (must include -- prefix).</param>
    /// <param name="fallback">The fallback value to use if the property is invalid.</param>
    public CssVarFunction(string propertyName, CssValue fallback)
    {
        PropertyName = propertyName ?? string.Empty;
        Fallback = fallback;
        FallbackTokens = null;
    }

    /// <summary>
    /// Creates a new var() function reference with raw fallback tokens.
    /// </summary>
    /// <param name="propertyName">The custom property name (must include -- prefix).</param>
    /// <param name="fallbackTokens">The raw tokens making up the fallback value.</param>
    public CssVarFunction(string propertyName, ReadOnlyMemory<CssToken> fallbackTokens)
    {
        PropertyName = propertyName ?? string.Empty;
        Fallback = null;
        FallbackTokens = fallbackTokens;
    }

    /// <summary>
    /// Creates a new var() function reference with both parsed fallback and raw tokens.
    /// </summary>
    /// <param name="propertyName">The custom property name (must include -- prefix).</param>
    /// <param name="fallback">The parsed fallback value.</param>
    /// <param name="fallbackTokens">The raw tokens making up the fallback value.</param>
    public CssVarFunction(string propertyName, CssValue fallback, ReadOnlyMemory<CssToken> fallbackTokens)
    {
        PropertyName = propertyName ?? string.Empty;
        Fallback = fallback;
        FallbackTokens = fallbackTokens;
    }

    #endregion

    #region IEquatable

    public bool Equals(CssVarFunction other)
    {
        // Property names are case-sensitive per spec
        if (!string.Equals(PropertyName, other.PropertyName, StringComparison.Ordinal))
            return false;

        // Compare fallback presence
        if (HasFallback != other.HasFallback)
            return false;

        // If both have fallbacks, compare them
        if (Fallback is not null && other.Fallback is not null)
        {
            return Fallback.Equals(other.Fallback);
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is CssVarFunction other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (PropertyName?.GetHashCode() ?? 0);
            hash = hash * 31 + (Fallback?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public static bool operator ==(CssVarFunction left, CssVarFunction right) => left.Equals(right);
    public static bool operator !=(CssVarFunction left, CssVarFunction right) => !left.Equals(right);

    #endregion

    #region ToString

    /// <summary>
    /// Returns the CSS serialization of this var() function.
    /// </summary>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(PropertyName))
            return "var()";

        if (HasFallback)
        {
            // Serialize fallback - use CssValue if available, otherwise tokens
            string fallbackStr = Fallback?.ToString() ?? SerializeFallbackTokens();
            return $"var({PropertyName}, {fallbackStr})";
        }

        return $"var({PropertyName})";
    }

    private string SerializeFallbackTokens()
    {
        if (FallbackTokens is null || FallbackTokens.Value.Length == 0)
            return string.Empty;

        var sb = new System.Text.StringBuilder();
        foreach (var token in FallbackTokens.Value.Span)
        {
            sb.Append(token.Encode());
        }
        return sb.ToString();
    }

    #endregion
}
