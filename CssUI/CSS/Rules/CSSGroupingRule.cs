using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// A CSS rule that holds other rules within it, things like @media and @page rules inherit from this.
/// </summary>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssgroupingrule-interface"/>
public abstract class CSSGroupingRule : CSSRule
{
    #region Properties
    public readonly CSSRuleList cssRules = new CSSRuleList();
    #endregion

    #region Constructors
    protected CSSGroupingRule(ECssRuleType type, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(type, parentRule, parentStyleSheet)
    {
    }
    #endregion

    #region Methods
    public int insertRule(string rule, int index) => cssRules.InsertRule(index, rule);
    public void deleteRule(int index) => cssRules.RemoveRule(index);
    #endregion

    #region Formatting Helpers
    /// <summary>
    /// Serializes child rules with proper indentation (2 spaces per spec).
    /// </summary>
    protected bool TryFormatChildRules(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        foreach (CSSRule rule in cssRules)
        {
            // Newline + 2 spaces indentation
            if (!"\n  ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 3;

            // Serialize child rule
            if (!rule.TryFormat(destination[charsWritten..], out int ruleWritten, format, provider))
                return false;
            charsWritten += ruleWritten;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 64; // Base overhead
        foreach (CSSRule rule in cssRules)
        {
            size += 256; // Estimate per child rule
        }
        return size;
    }
    #endregion
}

