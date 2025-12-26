using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @supports rule for feature queries.
/// </summary>
/// <remarks>
/// Serialization follows the pattern:
/// <code>@supports condition { rules }</code>
/// </remarks>
/// <seealso href="https://drafts.csswg.org/css-conditional-3/#csssupportsrule"/>
public sealed class CSSSupportsRule : CSSGroupingRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.SUPPORTS_RULE;

    /// <summary>
    /// The condition text of the @supports rule.
    /// </summary>
    public string conditionText;
    #endregion

    #region Constructors
    public CSSSupportsRule(string conditionText, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.SUPPORTS_RULE, parentRule, parentStyleSheet)
    {
        this.conditionText = conditionText;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Format: <code>@supports condition {
    ///   rule1
    ///   rule2
    /// }</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@supports "
        if (!"@supports ".TryCopyTo(destination))
            return false;
        charsWritten += 10;

        // 2) Condition text
        if (!conditionText.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += conditionText.Length;

        // 3) " {"
        if (!" {".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        // 4) Child rules
        if (!TryFormatChildRules(destination[charsWritten..], out int rulesWritten, format, provider))
            return false;
        charsWritten += rulesWritten;

        // 5) "\n}"
        if (!"\n}".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 16 + conditionText.Length; // "@supports " + condition + " { }"
        size += base.EstimateSerializationSize(); // Child rules
        return size;
    }
    #endregion
}
