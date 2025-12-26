using System;
using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @namespace rule.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSNamespaceRule:
/// <code>@namespace prefix url("namespaceURI");</code>
/// or <code>@namespace url("namespaceURI");</code> if no prefix.
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssnamespacerule-interface"/>
public sealed class CSSNamespaceRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.NAMESPACE_RULE;

    /// <summary>
    /// The namespace URI.
    /// </summary>
    public readonly string namespaceURI;

    /// <summary>
    /// The namespace prefix, or empty string if no prefix.
    /// </summary>
    public readonly string prefix;
    #endregion

    #region Constructors
    public CSSNamespaceRule(string namespaceURI, string prefix, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.NAMESPACE_RULE, parentRule, parentStyleSheet)
    {
        this.namespaceURI = namespaceURI;
        this.prefix = prefix;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSNamespaceRule serialization:
    /// <code>@namespace prefix url("namespaceURI");</code>
    /// or <code>@namespace url("namespaceURI");</code> if prefix is empty.
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@namespace"
        if (!"@namespace".TryCopyTo(destination))
            return false;
        charsWritten += 10;

        // 2) If prefix is not empty, serialize it as identifier with leading space
        if (!string.IsNullOrEmpty(prefix))
        {
            if (!" ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            string? serializedPrefix = Serializer.Identifier(prefix);
            if (serializedPrefix != null)
            {
                if (!serializedPrefix.AsSpan().TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += serializedPrefix.Length;
            }
        }

        // 3) " " + url("namespaceURI")
        if (!" ".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        string serializedUrl = Serializer.Serialize_URL(namespaceURI);
        if (!serializedUrl.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += serializedUrl.Length;

        // 4) ";"
        if (!";".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        return 16 + prefix.Length + namespaceURI.Length + 16; // "@namespace " + prefix + url("") + ";"
    }
    #endregion
}

