using System;
using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS margin at-rule (e.g., @top-left) within an @page rule.
/// </summary>
/// <remarks>
/// Serialization follows the expected format: <code>@margin-name { declarations }</code>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssmarginrule-interface"/>
public sealed class CSSMarginRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.MARGIN_RULE;

    /// <summary>
    /// The name of the margin at-rule (without the @ character).
    /// </summary>
    public readonly string name;

    /// <summary>
    /// The style declaration block.
    /// </summary>
    public readonly CSSStyleDeclaration style;
    #endregion

    #region Constructors
    public CSSMarginRule(string name, CSSStyleDeclaration style, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.MARGIN_RULE, parentRule, parentStyleSheet)
    {
        this.name = name;
        this.style = style;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Format: <code>@margin-name { declarations }</code>
    /// The @ character is not included in the name per spec.
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@" + name
        if (!"@".TryCopyTo(destination))
            return false;
        charsWritten += 1;

        string? serializedName = Serializer.Identifier(name);
        if (serializedName != null)
        {
            if (!serializedName.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += serializedName.Length;
        }

        // 2) " {"
        if (!" {".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        // 3) Declarations
        string? decls = style?.cssText?.Trim();
        if (!string.IsNullOrEmpty(decls))
        {
            if (!" ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            if (!decls.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += decls!.Length;

            if (!" }".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }
        else
        {
            if (!" }".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 8 + name.Length; // "@" + name + " { }"
        if (style != null)
            size += style.Count * 48;
        return size;
    }
    #endregion
}

