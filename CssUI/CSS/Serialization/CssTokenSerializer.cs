using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CssUI.CSS.Parser;

namespace CssUI.CSS.Serialization;

/// <summary>
/// Provides serialization of CSS tokens and component values per CSS Syntax Level 3 §10.
/// </summary>
/// <remarks>
/// <para>
/// The serializer ensures round-trip capability: parse → serialize → parse = same result.
/// </para>
/// <para>
/// Key requirements from the spec:
/// <list type="bullet">
///   <item>Consecutive whitespace tokens may be collapsed into a single token</item>
///   <item>Comments must be inserted between token pairs that would otherwise merge</item>
///   <item>DelimToken containing '\' must be serialized as '\\' followed by a newline</item>
///   <item>Hash tokens with "unrestricted" type flag may need different escaping</item>
///   <item>Dimension units may need escaping for scientific notation disambiguation</item>
/// </list>
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#serialization"/>
public static class CssTokenSerializer
{
    #region Constants
    /// <summary>
    /// Empty comment used to separate tokens that would otherwise merge.
    /// </summary>
    private const string EmptyComment = "/**/";
    #endregion

    #region Token Type Categories
    /// <summary>
    /// Gets whether the token is an ident-like token (ident, function, url, bad-url).
    /// </summary>
    private static bool IsIdentLike(ECssTokenType type) => type switch
    {
        ECssTokenType.Ident => true,
        ECssTokenType.FunctionName => true,
        ECssTokenType.Function => true,
        ECssTokenType.Url => true,
        ECssTokenType.Bad_Url => true,
        _ => false
    };

    /// <summary>
    /// Gets whether the token is a numeric token (number, percentage, dimension).
    /// </summary>
    private static bool IsNumeric(ECssTokenType type) => type switch
    {
        ECssTokenType.Number => true,
        ECssTokenType.Percentage => true,
        ECssTokenType.Dimension => true,
        _ => false
    };
    #endregion

    #region Token Adjacency Table
    /// <summary>
    /// Determines if a comment separator is required between two adjacent tokens
    /// to ensure they don't merge when re-parsed.
    /// </summary>
    /// <remarks>
    /// Per CSS Syntax Level 3 §10, certain token pairs require a comment between them
    /// to prevent them from being parsed as a single different token.
    /// </remarks>
    /// <param name="first">The first (preceding) token.</param>
    /// <param name="second">The second (following) token.</param>
    /// <returns>True if a comment separator is needed between the tokens.</returns>
    public static bool RequiresSeparator(CssToken? first, CssToken? second)
    {
        if (first is null || second is null) return false;

        return RequiresSeparator(first.Type, GetDelimValue(first), second.Type, GetDelimValue(second));
    }

    /// <summary>
    /// Determines if a comment separator is required between two adjacent token types.
    /// </summary>
    /// <param name="firstType">The first token type.</param>
    /// <param name="firstDelimValue">If first is a delim token, its character value; otherwise null.</param>
    /// <param name="secondType">The second token type.</param>
    /// <param name="secondDelimValue">If second is a delim token, its character value; otherwise null.</param>
    /// <returns>True if a comment separator is needed.</returns>
    public static bool RequiresSeparator(ECssTokenType firstType, char? firstDelimValue, ECssTokenType secondType, char? secondDelimValue)
    {
        // Handle delim tokens specially
        if (firstType == ECssTokenType.Delim && firstDelimValue.HasValue)
        {
            return RequiresSeparatorForFirstDelim(firstDelimValue.Value, secondType, secondDelimValue);
        }

        // Row: ident
        if (firstType == ECssTokenType.Ident)
        {
            return IsIdentLike(secondType) ||
                   IsNumeric(secondType) ||
                   secondType == ECssTokenType.CDC ||
                   secondType == ECssTokenType.Parenth_Open ||
                   (secondType == ECssTokenType.Delim && secondDelimValue == '-');
        }

        // Row: at-keyword
        if (firstType == ECssTokenType.At_Keyword)
        {
            return IsIdentLike(secondType) ||
                   IsNumeric(secondType) ||
                   secondType == ECssTokenType.CDC ||
                   (secondType == ECssTokenType.Delim && secondDelimValue == '-');
        }

        // Row: hash
        if (firstType == ECssTokenType.Hash)
        {
            return IsIdentLike(secondType) ||
                   IsNumeric(secondType) ||
                   secondType == ECssTokenType.CDC ||
                   (secondType == ECssTokenType.Delim && secondDelimValue == '-');
        }

        // Row: dimension
        if (firstType == ECssTokenType.Dimension)
        {
            return IsIdentLike(secondType) ||
                   IsNumeric(secondType) ||
                   secondType == ECssTokenType.CDC ||
                   (secondType == ECssTokenType.Delim && secondDelimValue == '-');
        }

        // Row: number
        if (firstType == ECssTokenType.Number)
        {
            return IsIdentLike(secondType) ||
                   IsNumeric(secondType) ||
                   secondType == ECssTokenType.CDC ||
                   (secondType == ECssTokenType.Delim && (secondDelimValue == '%'));
        }

        // Row: CDC
        if (firstType == ECssTokenType.CDC)
        {
            return false; // CDC doesn't require separator before anything in the table
        }

        // Row: CDO - not typically serialized but handle anyway
        if (firstType == ECssTokenType.CDO)
        {
            return false;
        }

        return false;
    }

    /// <summary>
    /// Handles separator requirements when the first token is a delim token.
    /// </summary>
    private static bool RequiresSeparatorForFirstDelim(char delimValue, ECssTokenType secondType, char? secondDelimValue)
    {
        switch (delimValue)
        {
            // Row: # (number sign delim)
            case '#':
                return IsIdentLike(secondType) ||
                       IsNumeric(secondType) ||
                       secondType == ECssTokenType.CDC ||
                       (secondType == ECssTokenType.Delim && secondDelimValue == '-');

            // Row: - (hyphen-minus delim)
            case '-':
                return IsIdentLike(secondType) ||
                       IsNumeric(secondType) ||
                       secondType == ECssTokenType.CDC ||
                       (secondType == ECssTokenType.Delim && secondDelimValue == '-');

            // Row: @ (at-sign delim)
            case '@':
                return IsIdentLike(secondType) ||
                       secondType == ECssTokenType.CDC ||
                       (secondType == ECssTokenType.Delim && secondDelimValue == '-');

            // Row: . (full stop delim)
            case '.':
                return IsNumeric(secondType);

            // Row: + (plus sign delim)
            case '+':
                return IsNumeric(secondType);

            // Row: / (solidus delim)
            case '/':
                return secondType == ECssTokenType.Delim && secondDelimValue == '*';

            default:
                return false;
        }
    }

    /// <summary>
    /// Gets the delim character value from a token, if it's a delim token.
    /// </summary>
    private static char? GetDelimValue(CssToken token)
    {
        if (token is DelimToken delim)
        {
            return delim.Value;
        }
        return null;
    }
    #endregion

    #region Single Token Serialization
    /// <summary>
    /// Serializes a single CSS token to its canonical string representation.
    /// </summary>
    /// <param name="token">The token to serialize.</param>
    /// <returns>The serialized token string.</returns>
    public static string Serialize(CssToken token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return token.Type switch
        {
            ECssTokenType.Delim => SerializeDelimToken((DelimToken)token),
            ECssTokenType.Hash => SerializeHashToken((HashToken)token),
            ECssTokenType.String => SerializeStringToken((StringToken)token),
            ECssTokenType.Dimension => SerializeDimensionToken((DimensionToken)token),
            ECssTokenType.Number => SerializeNumberToken((NumberToken)token),
            ECssTokenType.Percentage => SerializePercentageToken((PercentageToken)token),
            ECssTokenType.At_Keyword => SerializeAtKeywordToken((AtToken)token),
            ECssTokenType.Ident => SerializeIdentToken((IdentToken)token),
            ECssTokenType.Url => SerializeUrlToken((UrlToken)token),
            _ => token.Encode() ?? string.Empty
        };
    }

    /// <summary>
    /// Serializes a delim token, handling the special case of reverse solidus.
    /// </summary>
    private static string SerializeDelimToken(DelimToken token)
    {
        // Per spec: A <delim-token> containing U+005C REVERSE SOLIDUS (\) must be
        // serialized as U+005C followed by a newline.
        if (token.Value == '\\')
        {
            return "\\\n";
        }

        return token.Encode();
    }

    /// <summary>
    /// Serializes a hash token with proper escaping based on its type flag.
    /// </summary>
    private static string SerializeHashToken(HashToken token)
    {
        // Hash tokens with "id" type need identifier-safe escaping
        // Hash tokens with "unrestricted" type may need less escaping
        string value = token.Value ?? string.Empty;

        if (token.HashType == EHashTokenType.ID)
        {
            // Serialize as an identifier (with proper escaping)
            string? escaped = Serializer.Identifier(value);
            return "#" + (escaped ?? value);
        }

        // Unrestricted type - just prefix with #
        return "#" + value;
    }

    /// <summary>
    /// Serializes a string token with proper escaping.
    /// </summary>
    private static string SerializeStringToken(StringToken token)
    {
        return Serializer.Serialize_String(token.Value ?? string.Empty);
    }

    /// <summary>
    /// Serializes a dimension token with proper unit escaping.
    /// </summary>
    /// <remarks>
    /// Per spec: The unit of a dimension-token may need escaping to disambiguate
    /// with scientific notation (e.g., "1e" followed by a digit-starting unit).
    /// </remarks>
    private static string SerializeDimensionToken(DimensionToken token)
    {
        string numericPart = SerializeNumericValue(token.AsNumber, token.DataType);
        string unit = token.Unit;

        // Check if unit needs escaping for scientific notation disambiguation
        // If the numeric representation ends with 'e' or 'E' (scientific notation)
        // and the unit starts with a digit, we need to escape the first char of the unit
        if (NeedsUnitEscaping(numericPart, unit))
        {
            unit = EscapeUnitFirstChar(unit);
        }

        return numericPart + unit;
    }

    /// <summary>
    /// Determines if a dimension unit needs escaping to avoid scientific notation ambiguity.
    /// </summary>
    private static bool NeedsUnitEscaping(string numericPart, string unit)
    {
        if (string.IsNullOrEmpty(unit)) return false;

        // Check if numeric part uses scientific notation
        bool hasScientific = numericPart.IndexOfAny(['e', 'E']) >= 0;
        if (!hasScientific) return false;

        // Check if unit starts with something that could be confused
        char firstUnitChar = unit[0];

        // If unit starts with +, -, or digit after scientific notation, escape needed
        return char.IsDigit(firstUnitChar) || firstUnitChar == '+' || firstUnitChar == '-';
    }

    /// <summary>
    /// Escapes the first character of a unit string.
    /// </summary>
    private static string EscapeUnitFirstChar(string unit)
    {
        if (string.IsNullOrEmpty(unit)) return unit;
        return Serializer.Escape_Code_Point(unit[0]) + unit[1..];
    }

    /// <summary>
    /// Serializes a number token.
    /// </summary>
    private static string SerializeNumberToken(NumberToken token)
    {
        return SerializeNumericValue(token.AsNumber, token.DataType);
    }

    /// <summary>
    /// Serializes a percentage token.
    /// </summary>
    private static string SerializePercentageToken(PercentageToken token)
    {
        return SerializeNumericValue(token.Number, ENumericTokenType.Number) + "%";
    }

    /// <summary>
    /// Serializes an at-keyword token.
    /// </summary>
    private static string SerializeAtKeywordToken(AtToken token)
    {
        string? identifier = Serializer.Identifier(token.Value ?? string.Empty);
        return "@" + identifier;
    }

    /// <summary>
    /// Serializes an ident token with proper escaping.
    /// </summary>
    private static string SerializeIdentToken(IdentToken token)
    {
        // Custom properties (--*) are case-sensitive and need special handling
        if (token.IsDashedIdent)
        {
            // For custom properties, preserve case exactly
            return token.Value ?? string.Empty;
        }

        return Serializer.Identifier(token.Value ?? string.Empty) ?? token.Value ?? string.Empty;
    }

    /// <summary>
    /// Serializes a URL token.
    /// </summary>
    private static string SerializeUrlToken(UrlToken token)
    {
        return Serializer.Serialize_URL(token.Value ?? string.Empty);
    }

    /// <summary>
    /// Serializes a numeric value to its canonical string representation.
    /// </summary>
    private static string SerializeNumericValue(double value, ENumericTokenType dataType)
    {
        if (dataType == ENumericTokenType.Integer)
        {
            return ((long)value).ToString(CultureInfo.InvariantCulture);
        }

        // For floats, use a format that preserves precision without trailing zeros
        string result = value.ToString("G", CultureInfo.InvariantCulture);

        // Ensure we always use '.' as decimal separator
        return result;
    }
    #endregion

    #region Token Stream Serialization
    /// <summary>
    /// Serializes a sequence of CSS tokens to a string, inserting separators as needed.
    /// </summary>
    /// <param name="tokens">The tokens to serialize.</param>
    /// <returns>The serialized CSS string.</returns>
    public static string Serialize(IEnumerable<CssToken> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        var sb = new StringBuilder();
        CssToken? previousToken = null;

        foreach (var token in tokens)
        {
            // Check if we need a separator between this token and the previous one
            if (previousToken is not null && RequiresSeparator(previousToken, token))
            {
                sb.Append(EmptyComment);
            }

            sb.Append(Serialize(token));
            previousToken = token;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Serializes a sequence of CSS tokens to a string, collapsing consecutive whitespace.
    /// </summary>
    /// <param name="tokens">The tokens to serialize.</param>
    /// <returns>The serialized CSS string with collapsed whitespace.</returns>
    public static string SerializeWithCollapsedWhitespace(IEnumerable<CssToken> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        var sb = new StringBuilder();
        CssToken? previousToken = null;
        bool previousWasWhitespace = false;

        foreach (var token in tokens)
        {
            // Collapse consecutive whitespace tokens
            if (token.Type == ECssTokenType.Whitespace)
            {
                if (previousWasWhitespace)
                {
                    continue; // Skip consecutive whitespace
                }
                previousWasWhitespace = true;
            }
            else
            {
                previousWasWhitespace = false;
            }

            // Check if we need a separator
            if (previousToken is not null && RequiresSeparator(previousToken, token))
            {
                sb.Append(EmptyComment);
            }

            sb.Append(Serialize(token));
            previousToken = token;
        }

        return sb.ToString();
    }
    #endregion

    #region Component Value Serialization
    /// <summary>
    /// Serializes a CSS function to its canonical string representation.
    /// </summary>
    /// <param name="function">The function to serialize.</param>
    /// <returns>The serialized function string.</returns>
    internal static string Serialize(CssFunction function)
    {
        ArgumentNullException.ThrowIfNull(function);

        var sb = new StringBuilder();
        sb.Append(Serializer.Identifier(function.Name) ?? function.Name);
        sb.Append('(');
        sb.Append(Serialize(function.Arguments));
        sb.Append(')');

        return sb.ToString();
    }

    /// <summary>
    /// Serializes a CSS simple block to its canonical string representation.
    /// </summary>
    /// <param name="block">The block to serialize.</param>
    /// <returns>The serialized block string.</returns>
    public static string Serialize(CssSimpleBlock block)
    {
        ArgumentNullException.ThrowIfNull(block);

        var sb = new StringBuilder();

        // Start token
        sb.Append(block.StartToken.Encode());

        // Values
        sb.Append(Serialize(block.Values));

        // End token (mirror of start)
        sb.Append(GetClosingBracket(block.StartToken.Type));

        return sb.ToString();
    }

    /// <summary>
    /// Serializes a CSS at-rule to its canonical string representation.
    /// </summary>
    /// <param name="atRule">The at-rule to serialize.</param>
    /// <returns>The serialized at-rule string.</returns>
    public static string Serialize(CssAtRule atRule)
    {
        ArgumentNullException.ThrowIfNull(atRule);

        var sb = new StringBuilder();
        sb.Append('@');
        sb.Append(Serializer.Identifier(atRule.Name) ?? atRule.Name);

        // Prelude
        if (atRule.Prelude.Count > 0)
        {
            sb.Append(' ');
            sb.Append(Serialize(atRule.Prelude));
        }

        // Block or semicolon
        if (atRule.Block is not null)
        {
            sb.Append(Serialize(atRule.Block));
        }
        else
        {
            sb.Append(';');
        }

        return sb.ToString();
    }

    /// <summary>
    /// Serializes a CSS qualified rule to its canonical string representation.
    /// </summary>
    /// <param name="rule">The qualified rule to serialize.</param>
    /// <returns>The serialized rule string.</returns>
    public static string Serialize(CssQualifiedRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        var sb = new StringBuilder();

        // Prelude (selector)
        sb.Append(Serialize(rule.Prelude));

        // Block
        sb.Append(Serialize(rule.Block));

        return sb.ToString();
    }

    /// <summary>
    /// Serializes a CSS declaration to its canonical string representation.
    /// </summary>
    /// <param name="declaration">The declaration to serialize.</param>
    /// <returns>The serialized declaration string.</returns>
    public static string Serialize(CssDecleration declaration)
    {
        ArgumentNullException.ThrowIfNull(declaration);

        var sb = new StringBuilder();

        // Property name
        if (declaration.IsCustomProperty)
        {
            // Custom properties preserve case
            sb.Append(declaration.Name);
        }
        else
        {
            sb.Append(Serializer.Identifier(declaration.Name) ?? declaration.Name);
        }

        sb.Append(':');

        // Value (with leading space if there are values)
        if (declaration.Values.Count > 0)
        {
            // Add space after colon for readability
            sb.Append(' ');
            sb.Append(Serialize(declaration.Values));
        }

        // Important flag
        if (declaration.Important)
        {
            sb.Append(" !important");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Gets the closing bracket character for a given opening bracket token type.
    /// </summary>
    private static char GetClosingBracket(ECssTokenType openingType) => openingType switch
    {
        ECssTokenType.Parenth_Open => ')',
        ECssTokenType.Bracket_Open => '}',
        ECssTokenType.SqBracket_Open => ']',
        _ => '}' // Default to curly brace
    };
    #endregion
}
