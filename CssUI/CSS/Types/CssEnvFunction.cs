using System;
using CssUI.CSS.Parser;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS env() function reference to an environment variable.
/// </summary>
/// <remarks>
/// Spec Reference: https://www.w3.org/TR/css-env-1/#env-function
///
/// Grammar:
/// <code>
/// env() = env( &lt;custom-ident&gt; &lt;integer [0,∞]&gt;* [, &lt;declaration-value&gt;]? )
/// </code>
///
/// The env() function allows referencing environment variables
/// defined by the user agent or author. Environment variables are
/// global to a document (unlike var() which uses cascading custom properties).
///
/// UA-defined environment variables include:
/// - safe-area-inset-* (top, right, bottom, left)
/// - safe-area-max-inset-* (top, right, bottom, left)
/// - viewport-segment-* (width, height, top, left, bottom, right) [indexed]
/// - preferred-text-scale
///
/// Author-defined environment variables use custom property names (--*).
///
/// Examples:
/// - env(safe-area-inset-top)
/// - env(safe-area-inset-top, 0px)
/// - env(viewport-segment-width 0 0, 300px)
/// - env(--custom-env, fallback)
/// </remarks>
public readonly struct CssEnvFunction : IEquatable<CssEnvFunction>
{
    #region Static

    /// <summary>
    /// Empty env() function with no variable name.
    /// </summary>
    public static readonly CssEnvFunction Empty = new CssEnvFunction();

    #endregion

    #region Properties

    /// <summary>
    /// The environment variable name being referenced.
    /// </summary>
    /// <remarks>
    /// Per CSS Environment Variables Level 1:
    /// - UA-defined names are predefined identifiers (e.g., safe-area-inset-top)
    /// - Author-defined names start with two dashes (--)
    /// </remarks>
    public string VariableName { get; }

    /// <summary>
    /// The indices for indexed environment variables (e.g., viewport-segment-*).
    /// </summary>
    /// <remarks>
    /// Indexed environment variables represent multiple values and require
    /// one or more integer indices to select a specific value.
    /// For example: env(viewport-segment-width 0 0, 300px) uses indices [0, 0].
    /// Non-indexed variables have an empty array.
    /// </remarks>
    public ReadOnlyMemory<int> Indices { get; }

    /// <summary>
    /// The fallback value to use if the environment variable is not defined.
    /// </summary>
    /// <remarks>
    /// The fallback is optional. When present, it can contain any valid
    /// CSS value including commas (e.g., env(foo, red, blue)).
    /// The fallback may also contain nested env() or var() references.
    /// </remarks>
    public CssValue? Fallback { get; }

    /// <summary>
    /// The raw tokens that make up the fallback value.
    /// </summary>
    /// <remarks>
    /// Preserved for cases where the fallback needs to be re-parsed
    /// in context or contains nested references.
    /// </remarks>
    public ReadOnlyMemory<CssToken>? FallbackTokens { get; }

    /// <summary>
    /// Indicates whether this env() reference has a fallback value.
    /// </summary>
    public bool HasFallback => Fallback is not null || FallbackTokens is not null;

    /// <summary>
    /// Indicates whether the variable name is valid.
    /// </summary>
    public bool IsValid => !string.IsNullOrEmpty(VariableName);

    /// <summary>
    /// Indicates whether the variable name is an author-defined custom identifier.
    /// </summary>
    public bool IsCustomVariable => !string.IsNullOrEmpty(VariableName) &&
                                    VariableName.StartsWith("--", StringComparison.Ordinal);

    /// <summary>
    /// Indicates whether the variable is indexed (has dimension indices).
    /// </summary>
    public bool IsIndexed => Indices.Length > 0;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new env() function reference without indices or fallback.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    public CssEnvFunction(string variableName)
    {
        VariableName = variableName ?? string.Empty;
        Indices = ReadOnlyMemory<int>.Empty;
        Fallback = null;
        FallbackTokens = null;
    }

    /// <summary>
    /// Creates a new env() function reference with indices but no fallback.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    /// <param name="indices">The dimension indices for indexed variables.</param>
    public CssEnvFunction(string variableName, ReadOnlyMemory<int> indices)
    {
        VariableName = variableName ?? string.Empty;
        Indices = indices;
        Fallback = null;
        FallbackTokens = null;
    }

    /// <summary>
    /// Creates a new env() function reference with a fallback value.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    /// <param name="fallback">The fallback value to use if the variable is undefined.</param>
    public CssEnvFunction(string variableName, CssValue fallback)
    {
        VariableName = variableName ?? string.Empty;
        Indices = ReadOnlyMemory<int>.Empty;
        Fallback = fallback;
        FallbackTokens = null;
    }

    /// <summary>
    /// Creates a new env() function reference with indices and a fallback value.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    /// <param name="indices">The dimension indices for indexed variables.</param>
    /// <param name="fallback">The fallback value to use if the variable is undefined.</param>
    public CssEnvFunction(string variableName, ReadOnlyMemory<int> indices, CssValue fallback)
    {
        VariableName = variableName ?? string.Empty;
        Indices = indices;
        Fallback = fallback;
        FallbackTokens = null;
    }

    /// <summary>
    /// Creates a new env() function reference with raw fallback tokens.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    /// <param name="fallbackTokens">The raw tokens making up the fallback value.</param>
    public CssEnvFunction(string variableName, ReadOnlyMemory<CssToken> fallbackTokens)
    {
        VariableName = variableName ?? string.Empty;
        Indices = ReadOnlyMemory<int>.Empty;
        Fallback = null;
        FallbackTokens = fallbackTokens;
    }

    /// <summary>
    /// Creates a new env() function reference with indices and raw fallback tokens.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    /// <param name="indices">The dimension indices for indexed variables.</param>
    /// <param name="fallbackTokens">The raw tokens making up the fallback value.</param>
    public CssEnvFunction(string variableName, ReadOnlyMemory<int> indices, ReadOnlyMemory<CssToken> fallbackTokens)
    {
        VariableName = variableName ?? string.Empty;
        Indices = indices;
        Fallback = null;
        FallbackTokens = fallbackTokens;
    }

    /// <summary>
    /// Creates a new env() function reference with both parsed fallback and raw tokens.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    /// <param name="indices">The dimension indices for indexed variables.</param>
    /// <param name="fallback">The parsed fallback value.</param>
    /// <param name="fallbackTokens">The raw tokens making up the fallback value.</param>
    public CssEnvFunction(string variableName, ReadOnlyMemory<int> indices, CssValue fallback, ReadOnlyMemory<CssToken> fallbackTokens)
    {
        VariableName = variableName ?? string.Empty;
        Indices = indices;
        Fallback = fallback;
        FallbackTokens = fallbackTokens;
    }

    #endregion

    #region IEquatable

    public bool Equals(CssEnvFunction other)
    {
        // Variable names are case-insensitive for UA-defined variables
        // but case-sensitive for author-defined (--*) variables per spec
        bool namesEqual;
        if (IsCustomVariable && other.IsCustomVariable)
        {
            // Author-defined: case-sensitive
            namesEqual = string.Equals(VariableName, other.VariableName, StringComparison.Ordinal);
        }
        else
        {
            // UA-defined: case-insensitive
            namesEqual = string.Equals(VariableName, other.VariableName, StringComparison.OrdinalIgnoreCase);
        }

        if (!namesEqual)
            return false;

        // Compare indices
        if (Indices.Length != other.Indices.Length)
            return false;

        var thisSpan = Indices.Span;
        var otherSpan = other.Indices.Span;
        for (int i = 0; i < thisSpan.Length; i++)
        {
            if (thisSpan[i] != otherSpan[i])
                return false;
        }

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
        return obj is CssEnvFunction other && Equals(other);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        // Use ordinal case for custom vars, ignore case for UA vars
        if (IsCustomVariable)
        {
            hash.Add(VariableName, StringComparer.Ordinal);
        }
        else
        {
            hash.Add(VariableName, StringComparer.OrdinalIgnoreCase);
        }

        // Add indices
        var span = Indices.Span;
        for (int i = 0; i < span.Length; i++)
        {
            hash.Add(span[i]);
        }

        hash.Add(HasFallback);
        if (Fallback is not null)
        {
            hash.Add(Fallback);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(CssEnvFunction left, CssEnvFunction right) => left.Equals(right);
    public static bool operator !=(CssEnvFunction left, CssEnvFunction right) => !left.Equals(right);

    #endregion

    #region Object Overrides

    public override string ToString()
    {
        if (string.IsNullOrEmpty(VariableName))
            return "env()";

        var result = $"env({VariableName}";

        // Add indices if present
        if (Indices.Length > 0)
        {
            var span = Indices.Span;
            for (int i = 0; i < span.Length; i++)
            {
                result += $" {span[i]}";
            }
        }

        // Add fallback if present
        if (Fallback is not null)
        {
            result += $", {Fallback}";
        }
        else if (FallbackTokens is not null)
        {
            result += ", <tokens>";
        }

        result += ")";
        return result;
    }

    #endregion
}
