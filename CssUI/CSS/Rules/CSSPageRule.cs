using System;

namespace CssUI.CSS.Internal;

/// <summary>
/// Represents a CSS @page rule.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4. Note: CSSOM spec says "@page serialization is not yet defined" (ISSUE 6).
/// This implementation follows the expected format: <code>@page selector { declarations }</code>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-csspagerule-interface"/>
public sealed class CSSPageRule : CSSGroupingRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.PAGE_RULE;

    /// <summary>
    /// The page selector for this @page rule.
    /// </summary>
    public CssSelector selector;

    /// <summary>
    /// The style declaration block.
    /// </summary>
    public readonly CSSStyleDeclaration style;
    #endregion

    #region Constructors
    public CSSPageRule(CssSelector selector, CSSStyleDeclaration style, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.PAGE_RULE, parentRule, parentStyleSheet)
    {
        this.selector = selector;
        this.style = style;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Format: <code>@page selector { declarations }</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@page"
        if (!"@page".TryCopyTo(destination))
            return false;
        charsWritten += 5;

        // 2) Selector (if any)
        if (selector != null && selector.Count > 0)
        {
            if (!" ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            if (!selector.TryFormat(destination[charsWritten..], out int selectorWritten, format, provider))
                return false;
            charsWritten += selectorWritten;
        }

        // 3) " {"
        if (!" {".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        // 4) Declarations
        string? decls = style?.cssText?.Trim();
        if (!string.IsNullOrEmpty(decls))
        {
            if (!" ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            if (!decls.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += decls!.Length;
        }

        // 5) Child rules (margin rules)
        if (cssRules.Count > 0)
        {
            if (!TryFormatChildRules(destination[charsWritten..], out int rulesWritten, format, provider))
                return false;
            charsWritten += rulesWritten;

            if (!"\n}".TryCopyTo(destination[charsWritten..]))
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
        int size = 32; // "@page " + braces
        if (selector != null)
            size += selector.Count * 64;
        if (style != null)
            size += style.Count * 48;
        size += base.EstimateSerializationSize(); // Margin rules
        return size;
    }
    #endregion
}

