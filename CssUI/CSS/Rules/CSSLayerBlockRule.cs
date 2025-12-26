using System;
using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @layer block rule for cascade layers.
/// </summary>
/// <remarks>
/// Format: <code>@layer name { rules }</code>
/// </remarks>
/// <seealso href="https://drafts.csswg.org/css-cascade-5/#csslayerblockrule"/>
public sealed class CSSLayerBlockRule : CSSGroupingRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.LAYER_BLOCK_RULE;

    /// <summary>
    /// The name of the layer, or empty string for anonymous layers.
    /// </summary>
    public readonly string name;
    #endregion

    #region Constructors
    public CSSLayerBlockRule(string name, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.LAYER_BLOCK_RULE, parentRule, parentStyleSheet)
    {
        this.name = name;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Format: <code>@layer name {
    ///   rule1
    ///   rule2
    /// }</code>
    /// or <code>@layer { rules }</code> for anonymous layers.
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@layer"
        if (!"@layer".TryCopyTo(destination))
            return false;
        charsWritten += 6;

        // 2) Name (if not anonymous)
        if (!string.IsNullOrEmpty(name))
        {
            if (!" ".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;

            string? serializedName = Serializer.Identifier(name);
            if (serializedName != null)
            {
                if (!serializedName.AsSpan().TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += serializedName.Length;
            }
        }

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
        int size = 12 + name.Length; // "@layer " + name + " { }"
        size += base.EstimateSerializationSize(); // Child rules
        return size;
    }
    #endregion
}
