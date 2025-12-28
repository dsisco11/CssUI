using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS style rule (selector { declarations }).
/// </summary>
/// <remarks>
/// <para>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSStyleRule:
/// <list type="number">
/// <item>Serialize the group of selectors followed by " {".</item>
/// <item>Serialize the declaration block.</item>
/// <item>Serialize any nested rules.</item>
/// <item>Append newline + "}" if nested rules exist, else " }".</item>
/// </list>
/// </para>
/// <para>
/// Per CSS Nesting spec, style rules can contain nested style rules and at-rules.
/// The cssRules property provides access to these nested rules.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssstylerule-interface"/>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/#cssom-style"/>
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

    /// <summary>
    /// Gets the list of nested CSS rules within this style rule.
    /// Per CSS Nesting spec, style rules can contain nested style rules and at-rules.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/css-nesting-1/#cssom-style"/>
    public readonly CSSRuleList cssRules = new CSSRuleList();
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

    #region Nested Rule Methods
    /// <summary>
    /// Inserts a new rule into this style rule's nested rules at the specified index.
    /// </summary>
    /// <param name="rule">The CSS rule text to insert.</param>
    /// <param name="index">The index at which to insert the rule (default: 0).</param>
    /// <returns>The index at which the rule was inserted.</returns>
    /// <seealso href="https://www.w3.org/TR/css-nesting-1/#cssom-style"/>
    public int insertRule(string rule, int index = 0) => cssRules.InsertRule(index, rule);

    /// <summary>
    /// Deletes a rule from this style rule's nested rules at the specified index.
    /// </summary>
    /// <param name="index">The index of the rule to delete.</param>
    /// <seealso href="https://www.w3.org/TR/css-nesting-1/#cssom-style"/>
    public void deleteRule(int index) => cssRules.RemoveRule(index);
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSStyleRule serialization:
    /// <code>selector { declarations }</code>
    /// <para>With nested rules:</para>
    /// <code>selector { declarations\n  nestedRule\n}</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        bool hasNestedRules = cssRules.Count > 0;

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
        }

        // 4) Serialize nested rules (if any)
        if (hasNestedRules)
        {
            foreach (CSSRule nestedRule in cssRules)
            {
                // Newline + 2 spaces indentation per CSSOM
                if (!"\n  ".TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += 3;

                if (!nestedRule.TryFormat(destination[charsWritten..], out int nestedWritten, format, provider))
                    return false;
                charsWritten += nestedWritten;
            }

            // Close with newline + }
            if (!"\n}".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }
        else
        {
            // No nested rules: close with " }"
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
        // Add estimate for nested rules
        foreach (CSSRule rule in cssRules)
        {
            size += 256;
        }
        return size;
    }
    #endregion
}

