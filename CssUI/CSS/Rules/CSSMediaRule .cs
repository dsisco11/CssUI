using System;
using System.Collections.Generic;
using CssUI.CSS.Internal;
using CssUI.CSS.Media;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @media rule containing conditional rules based on media queries.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSMediaRule:
/// <code>@media media-query-list {
///   /* child rules indented by 2 spaces */
/// }</code>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssmediarule-interface"/>
public sealed class CSSMediaRule : CSSGroupingRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.MEDIA_RULE;

    /// <summary>
    /// The media query list for this @media rule.
    /// </summary>
    public readonly LinkedList<MediaQuery> media = new LinkedList<MediaQuery>();
    #endregion

    #region Constructors
    public CSSMediaRule() : base(ECssRuleType.MEDIA_RULE)
    {
    }

    public CSSMediaRule(IEnumerable<MediaQuery> mediaQueries, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.MEDIA_RULE, parentRule, parentStyleSheet)
    {
        foreach (var query in mediaQueries)
            media.AddLast(query);
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSMediaRule serialization:
    /// <code>@media media-query-list {
    ///   rule1
    ///   rule2
    /// }</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@media "
        if (!"@media ".TryCopyTo(destination))
            return false;
        charsWritten += 7;

        // 2) Serialize media query list (comma-separated)
        bool first = true;
        foreach (MediaQuery query in media)
        {
            if (!first)
            {
                if (!", ".TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += 2;
            }

            if (!query.TryFormat(destination[charsWritten..], out int queryWritten, format, provider))
                return false;
            charsWritten += queryWritten;
            first = false;
        }

        // 3) " {"
        if (!" {".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        // 4) Serialize child rules with indentation
        if (!TryFormatChildRules(destination[charsWritten..], out int rulesWritten, format, provider))
            return false;
        charsWritten += rulesWritten;

        // 5) Newline + "}"
        if (!"\n}".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 2;

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 32; // "@media " + braces
        size += media.Count * 64; // Media queries
        size += base.EstimateSerializationSize(); // Child rules
        return size;
    }
    #endregion
}

