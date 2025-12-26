using System;
using System.Collections.Generic;
using CssUI.CSS.Internal;
using CssUI.CSS.Serialization;

namespace CssUI.CSS;

/// <summary>
/// Represents a CSS @layer statement rule for declaring layer order.
/// </summary>
/// <remarks>
/// Format: <code>@layer name1, name2, name3;</code>
/// </remarks>
/// <seealso href="https://drafts.csswg.org/css-cascade-5/#csslayerstatementrule"/>
public sealed class CSSLayerStatementRule : CSSRule
{
    #region Properties
    public new readonly ECssRuleType type = ECssRuleType.LAYER_STATEMENT_RULE;

    /// <summary>
    /// The list of layer names declared by this statement.
    /// </summary>
    public readonly List<string> nameList;
    #endregion

    #region Constructors
    public CSSLayerStatementRule(IEnumerable<string> names, CSSRule? parentRule = null, CSSStyleSheet? parentStyleSheet = null)
        : base(ECssRuleType.LAYER_STATEMENT_RULE, parentRule, parentStyleSheet)
    {
        nameList = new List<string>(names);
    }
    #endregion

    #region Formatting (ISpanFormattable)
    /// <inheritdoc/>
    /// <remarks>
    /// Format: <code>@layer name1, name2, name3;</code>
    /// </remarks>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;

        // 1) "@layer "
        if (!"@layer ".TryCopyTo(destination))
            return false;
        charsWritten += 7;

        // 2) Comma-separated layer names
        bool first = true;
        foreach (string name in nameList)
        {
            if (!first)
            {
                if (!", ".TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += 2;
            }

            string? serializedName = Serializer.Identifier(name);
            if (serializedName != null)
            {
                if (!serializedName.AsSpan().TryCopyTo(destination[charsWritten..]))
                    return false;
                charsWritten += serializedName.Length;
            }
            first = false;
        }

        // 3) ";"
        if (!";".TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += 1;

        return true;
    }

    /// <inheritdoc/>
    protected override int EstimateSerializationSize()
    {
        int size = 10; // "@layer " + ";"
        foreach (string name in nameList)
            size += name.Length + 2; // name + ", "
        return size;
    }
    #endregion
}
