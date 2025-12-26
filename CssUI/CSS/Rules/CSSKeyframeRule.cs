using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// Represents a single keyframe within a CSS @keyframes rule.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSKeyframeRule:
/// <code>keyText { declarations }</code>
/// </remarks>
/// <seealso href="https://drafts.csswg.org/css-animations-1/#csskeyframerule"/>
public sealed class CSSKeyframeRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.KEYFRAME_RULE;

    /// <summary>
    /// The keyframe selector (e.g., "0%", "50%", "100%", "from", "to").
    /// </summary>
    public string keyText;

    /// <summary>
    /// The style declaration block for this keyframe.
    /// </summary>
    public readonly CSSStyleDeclaration style;
    #endregion

    #region Constructors
    public CSSKeyframeRule(string keyText, CSSStyleDeclaration style, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.KEYFRAME_RULE, parentRule, parentStyleSheet)
    {
        this.keyText = keyText;
        this.style = style;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSKeyframeRule serialization:
    /// <code>keyText { declarations }</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) keyText
        if (!keyText.AsSpan().TryCopyTo(destination))
            return false;
        charsWritten += keyText.Length;

        // 2) " { "
        if (!" { ".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 3;

        // 3) Serialize declarations
        string? decls = style?.cssText?.Trim();
        if (!string.IsNullOrEmpty(decls))
        {
            if (!decls.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += decls!.Length;

            if (!" }".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }
        else
        {
            if (!"}".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 1;
        }

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = keyText.Length + 8; // keyText + " { }"
        if (style != null)
            size += style.Count * 48;
        return size;
    }
    #endregion
}
