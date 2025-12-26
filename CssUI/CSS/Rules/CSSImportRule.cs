using System;
using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @import rule.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSImportRule:
/// <code>@import url("href") media-list;</code>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/cssom-1/#the-cssimportrule-interface"/>
public sealed class CSSImportRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.IMPORT_RULE;

    /// <summary>
    /// The URL specified by the @import rule.
    /// </summary>
    public readonly string href;

    /// <summary>
    /// The imported stylesheet.
    /// </summary>
    public readonly CSSStyleSheet stylesheet;

    // @todo: Implement MediaList for import rules
    // public MediaList media => stylesheet.media;
    #endregion

    #region Constructors
    public CSSImportRule(string href, CSSStyleSheet stylesheet, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.IMPORT_RULE, parentRule, parentStyleSheet)
    {
        this.href = href;
        this.stylesheet = stylesheet;
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSImportRule serialization:
    /// <code>@import url("href");</code>
    /// or <code>@import url("href") media-list;</code> if media list is not empty.
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@import "
        if (!"@import ".TryCopyTo(destination))
            return false;
        charsWritten += 8;

        // 2) Serialize URL: url("href")
        string serializedUrl = Serializer.Serialize_URL(href);
        if (!serializedUrl.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += serializedUrl.Length;

        // @todo: 3) If media list is not empty, append " " + serialized media list

        // 4) ";"
        if (!";".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        return 16 + href.Length + 64; // "@import url("");" + href + media
    }
    #endregion
}

