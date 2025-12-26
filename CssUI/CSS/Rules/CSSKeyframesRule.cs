using System;
using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @keyframes rule containing animation keyframes.
/// </summary>
/// <remarks>
/// Serialization follows CSSOM §6.4 "Serialize a CSS rule" for CSSKeyframesRule:
/// <code>@keyframes name { keyframe1 keyframe2 ... }</code>
/// </remarks>
/// <seealso href="https://drafts.csswg.org/css-animations-1/#csskeyframesrule"/>
public sealed class CSSKeyframesRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.KEYFRAMES_RULE;

    /// <summary>
    /// The name of the keyframes rule.
    /// </summary>
    public string name;

    /// <summary>
    /// The list of keyframe rules.
    /// </summary>
    public readonly CSSRuleList cssRules = new CSSRuleList();
    #endregion

    #region Constructors
    public CSSKeyframesRule(string name, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.KEYFRAMES_RULE, parentRule, parentStyleSheet)
    {
        this.name = name;
    }
    #endregion

    #region Methods
    public void appendRule(string rule)
    {
        // @todo: Parse and append keyframe rule
        throw new NotImplementedException();
    }

    public void deleteRule(string select)
    {
        // @todo: Find and delete keyframe by selector
        throw new NotImplementedException();
    }

    public CSSKeyframeRule? findRule(string select)
    {
        // @todo: Find keyframe by selector
        throw new NotImplementedException();
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Per CSSOM §6.4, CSSKeyframesRule serialization:
    /// <code>@keyframes name { keyframe1\n  keyframe2\n}</code>
    /// If name is a CSS wide keyword, 'default', or 'none', serialize as string.
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@keyframes "
        if (!"@keyframes ".TryCopyTo(destination))
            return false;
        charsWritten += 11;

        // 2) Serialize name (as string if reserved, else as identifier)
        string serializedName = IsReservedKeyword(name)
            ? Serializer.Serialize_String(name)
            : Serializer.Identifier(name) ?? name;

        if (!serializedName.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += serializedName.Length;

        // 3) " { "
        if (!" { ".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 3;

        // 4) Serialize each keyframe rule
        bool first = true;
        foreach (CSSRule rule in cssRules)
        {
            if (!first)
            {
                if (!"\n  ".TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += 3;
            }
            else
            {
                first = false;
            }

            if (!rule.TryFormat(destination[charsWritten..], out int ruleWritten, format, provider))
                return false;
            charsWritten += ruleWritten;
        }

        // 5) "\n}" or " }" if empty
        if (cssRules.Count > 0)
        {
            if (!"\n}".TryCopyTo(destination[charsWritten..]))
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

    private static bool IsReservedKeyword(string name)
    {
        return name is "none" or "default" or "initial" or "inherit" or "unset" or "revert" or "revert-layer";
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 16 + name.Length; // "@keyframes " + name + " { }"
        size += cssRules.Count * 128; // Keyframe rules
        return size;
    }
    #endregion
}
