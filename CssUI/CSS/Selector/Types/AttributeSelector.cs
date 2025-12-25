using System;
using System.Linq;
using System.Runtime.CompilerServices;
using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Nodes;

namespace CssUI.CSS.Selectors;

/// <summary>
/// Represents a CSS attribute selector that matches elements based on their attributes.
/// Supports presence checks [attr], exact match [attr=value], and various substring matches.
/// Per W3C Selectors Level 4 §6.3, also supports case-sensitivity modifiers (i/s).
/// </summary>
public class AttributeSelector : SimpleSelector
{
    readonly NamespacePrefixToken? Namespace;
    readonly AtomicName<EAttributeName> AttributeName;
    readonly ECssAttributeOperator Operator = ECssAttributeOperator.None;
    readonly string? Value = null;
    readonly EAttributeCaseSensitivity CaseSensitivity = EAttributeCaseSensitivity.Default;

    #region Constructor
    /// <summary>
    /// Creates an attribute presence selector [attr].
    /// </summary>
    /// <param name="Namespace">Optional namespace prefix</param>
    /// <param name="Attrib">The attribute name for this selector</param>
    public AttributeSelector(NamespacePrefixToken? Namespace, string Attrib) : base(ESimpleSelectorType.AttributeSelector)
    {
        this.Namespace = Namespace;
        this.AttributeName = Attrib;
        this.Operator = ECssAttributeOperator.Isset;
    }

    /// <summary>
    /// Creates an attribute value selector [attr=value] with default case-sensitivity.
    /// </summary>
    /// <param name="Namespace">Optional namespace prefix</param>
    /// <param name="Attrib">The attribute name for this selector</param>
    /// <param name="OperatorToken">Token that defines the method of comparison (=, ~=, |=, ^=, $=, *=)</param>
    /// <param name="Value">The value to match against</param>
    public AttributeSelector(NamespacePrefixToken? Namespace, string Attrib, CssToken OperatorToken, string Value)
        : this(Namespace, Attrib, OperatorToken, Value, EAttributeCaseSensitivity.Default)
    {
    }

    /// <summary>
    /// Creates an attribute value selector with explicit case-sensitivity control.
    /// Per W3C Selectors Level 4 §6.3, the 'i' and 's' modifiers control case-sensitivity.
    /// </summary>
    /// <param name="Namespace">Optional namespace prefix</param>
    /// <param name="Attrib">The attribute name for this selector</param>
    /// <param name="OperatorToken">Token that defines the method of comparison (=, ~=, |=, ^=, $=, *=)</param>
    /// <param name="Value">The value to match against</param>
    /// <param name="caseSensitivity">Case-sensitivity behavior for value matching</param>
    public AttributeSelector(NamespacePrefixToken? Namespace, string Attrib, CssToken OperatorToken, string Value, EAttributeCaseSensitivity caseSensitivity) : base(ESimpleSelectorType.AttributeSelector)
    {
        this.Namespace = Namespace;
        this.AttributeName = Attrib;
        this.Value = Value ?? string.Empty;
        this.CaseSensitivity = caseSensitivity;

        if (OperatorToken == null || OperatorToken.Type == ECssTokenType.Delim && (OperatorToken as DelimToken)!.Value == '>')
        {
            this.Operator = ECssAttributeOperator.Isset;
        }
        else
        {
            this.Operator = OperatorToken.Type switch
            {
                ECssTokenType.Delim when (OperatorToken as DelimToken)!.Value == '=' => ECssAttributeOperator.Equals,
                ECssTokenType.Dash_Match => ECssAttributeOperator.PrefixedWith,
                ECssTokenType.Include_Match => ECssAttributeOperator.Includes,
                ECssTokenType.Prefix_Match => ECssAttributeOperator.StartsWith,
                ECssTokenType.Suffix_Match => ECssAttributeOperator.EndsWith,
                ECssTokenType.Substring_Match => ECssAttributeOperator.Contains,
                _ => throw new CssSelectorException("Attribute selector: operator token-to-enum translation not implemented for (", OperatorToken, ")!")
            };
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Gets the StringComparison to use based on case-sensitivity setting.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private StringComparison GetStringComparison()
    {
        return CaseSensitivity switch
        {
            EAttributeCaseSensitivity.CaseInsensitive => StringComparison.OrdinalIgnoreCase,
            EAttributeCaseSensitivity.CaseSensitive => StringComparison.Ordinal,
            _ => StringComparison.Ordinal // Default to case-sensitive
        };
    }

    /// <summary>
    /// Compares two strings using the selector's case-sensitivity setting.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool StringEquals(string a, string b)
    {
        return string.Equals(a, b, GetStringComparison());
    }
    #endregion

    /// <summary>
    /// Returns whether the selector matches a specified element or index
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    override public bool Matches(Element E, params Node[] scopeElements)
    {
        var comparison = GetStringComparison();

        switch (Operator)
        {
            // CSS 2.0 operators
            case ECssAttributeOperator.Isset:// isset
                {
                    return E.hasAttribute(AttributeName);
                }
            case ECssAttributeOperator.Equals:// equals
                {
                    if (string.IsNullOrEmpty(Value)) return false;
                    return StringEquals(Value, E.getAttribute(AttributeName).AsString());
                }
            case ECssAttributeOperator.PrefixedWith:// equals or prefixed-with
                {
                    if (!E.hasAttribute(AttributeName)) return false;
                    string val = E.getAttribute(AttributeName).AsString();
                    if (StringEquals(Value!, val)) return true;
                    if (val.StartsWith(string.Concat(Value, '-'), comparison)) return true;
                    return false;
                }
            case ECssAttributeOperator.Includes:// list-contains
                {
                    if (string.IsNullOrEmpty(Value)) return false;
                    /* First lets check the elements token-list map */
                    if (E.tokenListMap.TryGetValue(AttributeName, out IAttributeTokenList listMap))
                    {
                        var TokenList = (AttributeTokenList<string>)listMap;
                        // Need to check with case-sensitivity
                        foreach (var item in TokenList)
                        {
                            if (StringEquals(Value, item)) return true;
                        }
                        return false;
                    }

                    if (!E.hasAttribute(AttributeName)) return false;
                    var attr = E.getAttribute(AttributeName);
                    var set = DOMCommon.Parse_Ordered_Set(attr.AsAtomic().AsMemory());
                    foreach (var item in set)
                    {
                        if (Value.AsMemory().Span.Equals(item.Span, comparison)) return true;
                    }
                    return false;
                }
            // Sub-string operators
            case ECssAttributeOperator.StartsWith:// starts-with
                {
                    if (string.IsNullOrEmpty(Value)) return false;
                    if (!E.hasAttribute(AttributeName)) return false;
                    var attr = E.getAttribute(AttributeName);
                    return attr.AsString().StartsWith(Value, comparison);
                }
            case ECssAttributeOperator.EndsWith:// ends-with
                {
                    if (string.IsNullOrEmpty(Value)) return false;
                    if (!E.hasAttribute(AttributeName)) return false;
                    var attr = E.getAttribute(AttributeName);
                    return attr.AsString().EndsWith(Value, comparison);
                }
            case ECssAttributeOperator.Contains:// contains
                {
                    if (string.IsNullOrEmpty(Value)) return false;
                    if (!E.hasAttribute(AttributeName)) return false;
                    var attr = E.getAttribute(AttributeName);
                    return attr.AsString().Contains(Value, comparison);
                }
            default:
                throw new CssSelectorException($"Attribute selector operator ({Enum.GetName(typeof(ECssAttributeOperator), Operator)}) logic not implemented!");
        }
    }

    #region Formatting
    /// <inheritdoc/>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Per CSSOM §5.2: attribute selector serializes as "[attr]" or "[attr=value]" etc.
        charsWritten = 0;

        // 1. Append "[" (U+005B)
        if (destination.Length < 1)
            return false;
        destination[0] = '[';
        charsWritten = 1;

        // 2. If namespace prefix maps to non-null namespace, append "ns|"
        if (Namespace is not null && !string.IsNullOrEmpty(Namespace.Value))
        {
            if (!Namespace.Value.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += Namespace.Value.Length;

            if (destination.Length <= charsWritten)
                return false;
            destination[charsWritten] = '|';
            charsWritten++;
        }

        // 3. Append the serialization of the attribute name as an identifier
        var attrName = AttributeName.ToString();
        if (!attrName.AsSpan().TryCopyTo(destination[charsWritten..]))
            return false;
        charsWritten += attrName.Length;

        // 4. If there is an attribute value specified, append operator and value
        if (Operator != ECssAttributeOperator.None && Operator != ECssAttributeOperator.Isset && Value is not null)
        {
            // Append operator
            var operatorStr = Operator switch
            {
                ECssAttributeOperator.Equals => "=",
                ECssAttributeOperator.Includes => "~=",
                ECssAttributeOperator.PrefixedWith => "|=",
                ECssAttributeOperator.StartsWith => "^=",
                ECssAttributeOperator.EndsWith => "$=",
                ECssAttributeOperator.Contains => "*=",
                _ => "="
            };

            if (!operatorStr.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += operatorStr.Length;

            // Serialize value as a string (with quotes)
            if (destination.Length <= charsWritten)
                return false;
            destination[charsWritten] = '"';
            charsWritten++;

            // @todo: Proper string escaping per CSSOM §2.1
            if (!Value.AsSpan().TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += Value.Length;

            if (destination.Length <= charsWritten)
                return false;
            destination[charsWritten] = '"';
            charsWritten++;
        }

        // 5. If case-sensitivity flag present, append " i" or " s"
        if (CaseSensitivity == EAttributeCaseSensitivity.CaseInsensitive)
        {
            if (!" i".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }
        else if (CaseSensitivity == EAttributeCaseSensitivity.CaseSensitive)
        {
            // Note: "s" is the default for most attributes, but we serialize it if explicitly set
            if (!" s".TryCopyTo(destination[charsWritten..]))
                return false;
            charsWritten += 2;
        }

        // 6. Append "]" (U+005D)
        if (destination.Length <= charsWritten)
            return false;
        destination[charsWritten] = ']';
        charsWritten++;

        return true;
    }
    #endregion
}

