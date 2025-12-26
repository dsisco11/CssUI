using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @font-face rule.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSFontFaceRule:
/// <code>@font-face { font-family: "name"; src: url("..."); ... }</code>
/// </remarks>
/// <seealso href="https://drafts.csswg.org/css-fonts-5/#cssfontfacerule"/>
public sealed class CSSFontFaceRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.FONT_FACE_RULE;

    /// <summary>
    /// The style declaration block containing font descriptors.
    /// </summary>
    public readonly CSSStyleDeclaration style;
    #endregion

    #region Constructors
    public CSSFontFaceRule(CSSStyleDeclaration style, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.FONT_FACE_RULE, parentRule, parentStyleSheet)
    {
        this.style = style;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSFontFaceRule serialization:
    /// <code>@font-face { font-family: "..."; src: ...; ... }</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@font-face {"
        if (!"@font-face {".TryCopyTo(destination))
            return false;
        charsWritten += 12;

        // 2) Serialize descriptors
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
        int size = 16; // "@font-face { }"
        if (style != null)
            size += style.Count * 64;
        return size;
    }
    #endregion
}
