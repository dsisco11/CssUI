using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS style rule (selector { declarations }).
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSStyleRule:
/// <list type="number">
/// <item>Serialize the group of selectors followed by " {".</item>
/// <item>Serialize the declaration block.</item>
/// <item>Append " }" or newline + "}" depending on nested rules.</item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssstylerule-interface"/>
public sealed class CSSStyleRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.STYLE_RULE;

    /// <summary>
    /// The selector(s) for this style rule.
    /// </summary>
    public CssSelector? Selector;

    /// <summary>
    /// The style declaration block containing property-value pairs.
    /// </summary>
    public readonly CSSStyleDeclaration? style;
    #endregion

    #region Constructors
    public CSSStyleRule() : base(ECssRuleType.STYLE_RULE)
    {
    }

    public CSSStyleRule(CssSelector? selector, CSSStyleDeclaration? style, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.STYLE_RULE, parentRule, parentStyleSheet)
    {
        Selector = selector;
        this.style = style;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSStyleRule serialization:
    /// <code>selector { declarations }</code>
    /// or <code>selector { }</code> if no declarations.
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) Serialize selector
        if (Selector != null)
        {
            if (!Selector.TryFormat(destination, out int selectorWritten, format, provider))
                return false;
            charsWritten += selectorWritten;
        }

        // 2) Append " {"
        if (!" {".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        // 3) Serialize declarations
        string? decls = style?.cssText?.Trim();
        bool hasDecls = !string.IsNullOrEmpty(decls);

        if (hasDecls)
        {
            // Append " " + declarations
            if (!" ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            if (!decls.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += decls!.Length;

            // Append " }"
            if (!" }".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }
        else
        {
            // Empty rule: " }"
            if (!" }".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 64; // Base overhead for braces and spaces
        if (Selector != null)
            size += Selector.Count * 64;
        if (style != null)
            size += style.Count * 48;
        return size;
    }
    #endregion
}

