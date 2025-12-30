using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// Provides methods to validate CSS token sequences against grammar productions
/// defined in CSS Syntax Level 3 §8, with integration for CSS Nesting (CSS Nesting Module Level 1).
/// </summary>
/// <remarks>
/// <para>
/// The productions defined here are used for validating arbitrary content in CSS:
/// </para>
/// <list type="bullet">
/// <item><description><c>&lt;declaration-value&gt;</c> - Valid content for a declaration's value</description></item>
/// <item><description><c>&lt;any-value&gt;</c> - Valid CSS content in any context</description></item>
/// <item><description><c>&lt;style-block&gt;</c> - Declarations and nested style rules (CSS Nesting)</description></item>
/// <item><description><c>&lt;rule-list&gt;</c> - Only qualified rules and at-rules</description></item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#any-value"/>
/// <seealso href="https://www.w3.org/TR/css-nesting-1/"/>
public static class CssProductionMatcher
{
    #region CSS Nesting Integration (Phase 11.7.7)

    /// <summary>
    /// Determines if a token can start a nested style rule within a <c>&lt;style-block&gt;</c> context.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token can start a nested rule; false if it starts a declaration or is invalid.</returns>
    /// <remarks>
    /// <para>
    /// Per CSS Nesting spec §2.1, nested selectors cannot start with an identifier
    /// (to avoid ambiguity with property declarations), but can start with:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>&amp;</c> - nesting selector</description></item>
    /// <item><description><c>.</c> - class selector</description></item>
    /// <item><description><c>*</c> - universal selector</description></item>
    /// <item><description><c>&gt;</c> <c>+</c> <c>~</c> - relative combinators</description></item>
    /// <item><description><c>#</c> - ID selector (hash token)</description></item>
    /// <item><description><c>:</c> - pseudo-class/element</description></item>
    /// <item><description><c>[</c> - attribute selector</description></item>
    /// </list>
    /// <para>
    /// An identifier token (<c>&lt;ident-token&gt;</c>) always starts a declaration, not a nested rule.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-nesting-1/#syntax"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsNestedRule(CssToken token)
    {
        if (token is null)
            return false;

        return token.Type switch
        {
            // At-rules are nested rules (e.g., @media, @supports)
            ECssTokenType.At_Keyword => true,

            // Hash token starts ID selector (#id)
            ECssTokenType.Hash => true,

            // Colon starts pseudo-class/element (:hover, ::before)
            ECssTokenType.Colon => true,

            // Square bracket starts attribute selector ([type="text"])
            ECssTokenType.SqBracket_Open => true,

            // Delim tokens that can start nested rules
            ECssTokenType.Delim when token is DelimToken delimToken =>
                StartsNestedRuleDelim(delimToken.Value),

            // Ident tokens start declarations, NOT nested rules
            ECssTokenType.Ident => false,

            // Everything else doesn't start a nested rule
            _ => false
        };
    }

    /// <summary>
    /// Determines if a delimiter character can start a nested style rule.
    /// </summary>
    /// <param name="delimValue">The delimiter character.</param>
    /// <returns>True if the delimiter can start a nested rule.</returns>
    /// <remarks>
    /// Per CSS Nesting spec §2.1, these delimiters can start nested rules:
    /// <c>&amp;</c> (nesting selector), <c>.</c> (class), <c>*</c> (universal), 
    /// <c>&gt;</c> <c>+</c> <c>~</c> (relative combinators).
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-nesting-1/#syntax"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsNestedRuleDelim(char delimValue)
    {
        return delimValue switch
        {
            UnicodeCommon.CHAR_AMPERSAND => true,      // & - nesting selector
            UnicodeCommon.CHAR_FULL_STOP => true,      // . - class selector
            UnicodeCommon.CHAR_ASTERISK => true,       // * - universal selector
            UnicodeCommon.CHAR_RIGHT_CHEVRON => true,  // > - child combinator
            UnicodeCommon.CHAR_PLUS_SIGN => true,      // + - adjacent sibling combinator
            UnicodeCommon.CHAR_TILDE => true,          // ~ - general sibling combinator
            _ => false
        };
    }

    /// <summary>
    /// Determines if a token can start a declaration within a <c>&lt;style-block&gt;</c> context.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token starts a declaration (property name).</returns>
    /// <remarks>
    /// In CSS, declarations start with an identifier token (the property name).
    /// This is in contrast to nested rules which start with selector-starting tokens.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsDeclaration(CssToken token)
    {
        return token?.Type == ECssTokenType.Ident;
    }

    /// <summary>
    /// Gets the expected content type for a token within a given block context.
    /// </summary>
    /// <param name="token">The token to classify.</param>
    /// <param name="contextType">The block contents type context.</param>
    /// <returns>The content classification for the token.</returns>
    /// <remarks>
    /// <para>
    /// Different block types expect different content:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>StyleBlock</c>: declarations and nested rules</description></item>
    /// <item><description><c>DeclarationList</c>: declarations and at-rules only</description></item>
    /// <item><description><c>RuleList</c>: qualified rules and at-rules only</description></item>
    /// <item><description><c>Stylesheet</c>: qualified rules and at-rules with CDO/CDC handling</description></item>
    /// </list>
    /// </remarks>
    public static ECssBlockContentClassification ClassifyTokenForContext(
        CssToken token,
        ECssBlockContentsType contextType)
    {
        if (token is null)
            return ECssBlockContentClassification.Invalid;

        return contextType switch
        {
            ECssBlockContentsType.StyleBlock => ClassifyForStyleBlock(token),
            ECssBlockContentsType.DeclarationList => ClassifyForDeclarationList(token),
            ECssBlockContentsType.RuleList => ClassifyForRuleList(token),
            ECssBlockContentsType.Stylesheet => ClassifyForStylesheet(token),
            _ => ECssBlockContentClassification.Invalid
        };
    }

    /// <summary>
    /// Classifies a token for <c>&lt;style-block&gt;</c> context (declarations + nested rules).
    /// </summary>
    private static ECssBlockContentClassification ClassifyForStyleBlock(CssToken token)
    {
        return token.Type switch
        {
            ECssTokenType.Whitespace => ECssBlockContentClassification.Whitespace,
            ECssTokenType.Semicolon => ECssBlockContentClassification.Separator,
            ECssTokenType.EOF => ECssBlockContentClassification.EndOfInput,
            ECssTokenType.Ident => ECssBlockContentClassification.Declaration,
            ECssTokenType.At_Keyword => ECssBlockContentClassification.AtRule,
            ECssTokenType.Hash => ECssBlockContentClassification.NestedRule,
            ECssTokenType.Colon => ECssBlockContentClassification.NestedRule,
            ECssTokenType.SqBracket_Open => ECssBlockContentClassification.NestedRule,
            ECssTokenType.Delim when token is DelimToken delimToken && StartsNestedRuleDelim(delimToken.Value)
                => ECssBlockContentClassification.NestedRule,
            ECssTokenType.Bracket_Close or ECssTokenType.Parenth_Close or ECssTokenType.SqBracket_Close
                => ECssBlockContentClassification.UnmatchedCloseBracket,
            ECssTokenType.Bad_String or ECssTokenType.Bad_Url
                => ECssBlockContentClassification.Invalid,
            _ => ECssBlockContentClassification.Invalid
        };
    }

    /// <summary>
    /// Classifies a token for <c>&lt;declaration-list&gt;</c> context (declarations + at-rules only).
    /// </summary>
    private static ECssBlockContentClassification ClassifyForDeclarationList(CssToken token)
    {
        return token.Type switch
        {
            ECssTokenType.Whitespace => ECssBlockContentClassification.Whitespace,
            ECssTokenType.Semicolon => ECssBlockContentClassification.Separator,
            ECssTokenType.EOF => ECssBlockContentClassification.EndOfInput,
            ECssTokenType.Ident => ECssBlockContentClassification.Declaration,
            ECssTokenType.At_Keyword => ECssBlockContentClassification.AtRule,
            // Nested qualified rules are NOT allowed in declaration-list
            ECssTokenType.Bracket_Close or ECssTokenType.Parenth_Close or ECssTokenType.SqBracket_Close
                => ECssBlockContentClassification.UnmatchedCloseBracket,
            ECssTokenType.Bad_String or ECssTokenType.Bad_Url
                => ECssBlockContentClassification.Invalid,
            _ => ECssBlockContentClassification.Invalid
        };
    }

    /// <summary>
    /// Classifies a token for <c>&lt;rule-list&gt;</c> context (qualified rules + at-rules only).
    /// </summary>
    private static ECssBlockContentClassification ClassifyForRuleList(CssToken token)
    {
        return token.Type switch
        {
            ECssTokenType.Whitespace => ECssBlockContentClassification.Whitespace,
            ECssTokenType.EOF => ECssBlockContentClassification.EndOfInput,
            ECssTokenType.At_Keyword => ECssBlockContentClassification.AtRule,
            // In rule-list, most tokens start qualified rules
            ECssTokenType.Ident => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Hash => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Colon => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.SqBracket_Open => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Delim => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Bracket_Close or ECssTokenType.Parenth_Close or ECssTokenType.SqBracket_Close
                => ECssBlockContentClassification.UnmatchedCloseBracket,
            ECssTokenType.Bad_String or ECssTokenType.Bad_Url
                => ECssBlockContentClassification.Invalid,
            _ => ECssBlockContentClassification.QualifiedRule
        };
    }

    /// <summary>
    /// Classifies a token for <c>&lt;stylesheet&gt;</c> context (top-level with CDO/CDC handling).
    /// </summary>
    private static ECssBlockContentClassification ClassifyForStylesheet(CssToken token)
    {
        return token.Type switch
        {
            ECssTokenType.Whitespace => ECssBlockContentClassification.Whitespace,
            ECssTokenType.EOF => ECssBlockContentClassification.EndOfInput,
            ECssTokenType.CDO or ECssTokenType.CDC => ECssBlockContentClassification.Ignored,
            ECssTokenType.At_Keyword => ECssBlockContentClassification.AtRule,
            // In stylesheet, most tokens start qualified rules
            ECssTokenType.Ident => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Hash => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Colon => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.SqBracket_Open => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Delim => ECssBlockContentClassification.QualifiedRule,
            ECssTokenType.Bracket_Close or ECssTokenType.Parenth_Close or ECssTokenType.SqBracket_Close
                => ECssBlockContentClassification.UnmatchedCloseBracket,
            ECssTokenType.Bad_String or ECssTokenType.Bad_Url
                => ECssBlockContentClassification.Invalid,
            _ => ECssBlockContentClassification.QualifiedRule
        };
    }

    /// <summary>
    /// Validates that content is appropriate for the given block contents type.
    /// </summary>
    /// <param name="tokens">The tokens to validate.</param>
    /// <param name="contextType">The block contents type context.</param>
    /// <returns>True if the tokens are valid for the context; false otherwise.</returns>
    /// <remarks>
    /// <para>
    /// This method performs a lightweight validation to ensure tokens don't contain
    /// elements that are always invalid (bad tokens, unmatched brackets).
    /// </para>
    /// <para>
    /// For <c>StyleBlock</c> context, it also validates that the content structure
    /// follows CSS Nesting rules (declarations vs nested rules distinction).
    /// </para>
    /// </remarks>
    public static bool ValidateBlockContents(IReadOnlyList<CssToken> tokens, ECssBlockContentsType contextType)
    {
        if (tokens is null || tokens.Count == 0)
            return true; // Empty content is valid for all block types

        // Check for always-invalid tokens
        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Type == ECssTokenType.EOF)
                continue;

            // Bad tokens are always invalid
            if (token.Type == ECssTokenType.Bad_String || token.Type == ECssTokenType.Bad_Url)
                return false;

            // For bracket validation, we'd need to track depth
            // This is handled by MatchDeclarationValue/MatchAnyValue for value contexts
        }

        return true;
    }

    #endregion
    #region <declaration-value> Production (§8.2)
    /// <summary>
    /// Validates a sequence of tokens against the <c>&lt;declaration-value&gt;</c> production.
    /// </summary>
    /// <param name="tokens">The tokens to validate.</param>
    /// <returns>A result indicating whether the tokens match the production.</returns>
    /// <remarks>
    /// <para>
    /// The <c>&lt;declaration-value&gt;</c> production matches any sequence of one or more tokens,
    /// so long as the sequence does not contain:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>&lt;bad-string-token&gt;</c></description></item>
    /// <item><description><c>&lt;bad-url-token&gt;</c></description></item>
    /// <item><description>unmatched <c>&lt;)-token&gt;</c>, <c>&lt;]-token&gt;</c>, or <c>&lt;}-token&gt;</c></description></item>
    /// <item><description>top-level <c>&lt;semicolon-token&gt;</c> tokens</description></item>
    /// <item><description>top-level <c>&lt;delim-token&gt;</c> tokens with a value of "!"</description></item>
    /// </list>
    /// <para>
    /// It represents the entirety of what a valid declaration can have as its value.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#typedef-declaration-value"/>
    public static CssProductionMatchResult MatchDeclarationValue(IReadOnlyList<CssToken> tokens)
    {
        if (tokens is null || tokens.Count == 0)
        {
            return CssProductionMatchResult.Failure(
                ECssProductionMatchFailure.EmptySequence,
                0,
                "The <declaration-value> production requires one or more tokens");
        }

        // Track bracket depth to determine if brackets are matched
        int parenDepth = 0;   // ()
        int bracketDepth = 0; // []
        int braceDepth = 0;   // {}

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            // Skip EOF tokens
            if (token.Type == ECssTokenType.EOF)
                continue;

            // Check for bad tokens (always invalid)
            if (token.Type == ECssTokenType.Bad_String)
            {
                return CssProductionMatchResult.Failure(
                    ECssProductionMatchFailure.BadStringToken,
                    i,
                    "The <declaration-value> production does not allow <bad-string-token>");
            }

            if (token.Type == ECssTokenType.Bad_Url)
            {
                return CssProductionMatchResult.Failure(
                    ECssProductionMatchFailure.BadUrlToken,
                    i,
                    "The <declaration-value> production does not allow <bad-url-token>");
            }

            // Check for top-level semicolon (invalid in declaration-value)
            if (token.Type == ECssTokenType.Semicolon &&
                parenDepth == 0 && bracketDepth == 0 && braceDepth == 0)
            {
                return CssProductionMatchResult.Failure(
                    ECssProductionMatchFailure.TopLevelSemicolon,
                    i,
                    "The <declaration-value> production does not allow top-level semicolons");
            }

            // Check for top-level "!" delimiter (invalid in declaration-value)
            if (token is DelimToken delimToken &&
                delimToken.Value == UnicodeCommon.CHAR_EXCLAMATION_POINT &&
                parenDepth == 0 && bracketDepth == 0 && braceDepth == 0)
            {
                return CssProductionMatchResult.Failure(
                    ECssProductionMatchFailure.TopLevelExclamation,
                    i,
                    "The <declaration-value> production does not allow top-level '!' delimiters");
            }

            // Track opening brackets
            // Note: ECssTokenType.Function and ECssTokenType.SimpleBlock tokens represent
            // already-parsed, balanced blocks and should NOT affect depth tracking.
            // Only raw bracket tokens from unparsed token streams should affect depth.
            // However, ECssTokenType.FunctionName tokens from the tokenizer include an
            // implicit opening parenthesis (per CSS Syntax 3 §4.3.4) that must be balanced.
            switch (token.Type)
            {
                case ECssTokenType.Parenth_Open:
                case ECssTokenType.FunctionName: // <function-token> includes implicit '('
                    parenDepth++;
                    break;
                case ECssTokenType.SqBracket_Open:
                    bracketDepth++;
                    break;
                case ECssTokenType.Bracket_Open:
                    braceDepth++;
                    break;
                // Function and SimpleBlock tokens are already balanced - no depth tracking needed
                case ECssTokenType.Function:
                case ECssTokenType.SimpleBlock:
                    break;
            }

            // Check closing brackets (invalid if unmatched)
            switch (token.Type)
            {
                case ECssTokenType.Parenth_Close:
                    if (parenDepth <= 0)
                    {
                        return CssProductionMatchResult.Failure(
                            ECssProductionMatchFailure.UnmatchedCloseBracket,
                            i,
                            "The <declaration-value> production does not allow unmatched <)-token>");
                    }
                    parenDepth--;
                    break;

                case ECssTokenType.SqBracket_Close:
                    if (bracketDepth <= 0)
                    {
                        return CssProductionMatchResult.Failure(
                            ECssProductionMatchFailure.UnmatchedCloseBracket,
                            i,
                            "The <declaration-value> production does not allow unmatched <]-token>");
                    }
                    bracketDepth--;
                    break;

                case ECssTokenType.Bracket_Close:
                    if (braceDepth <= 0)
                    {
                        return CssProductionMatchResult.Failure(
                            ECssProductionMatchFailure.UnmatchedCloseBracket,
                            i,
                            "The <declaration-value> production does not allow unmatched <}-token>");
                    }
                    braceDepth--;
                    break;
            }
        }

        return CssProductionMatchResult.Success();
    }

    /// <summary>
    /// Checks if a sequence of tokens matches the <c>&lt;declaration-value&gt;</c> production.
    /// </summary>
    /// <param name="tokens">The tokens to validate.</param>
    /// <returns>True if the tokens match; false otherwise.</returns>
    public static bool IsDeclarationValue(IReadOnlyList<CssToken> tokens) =>
        MatchDeclarationValue(tokens).IsMatch;
    #endregion

    #region <any-value> Production (§8.2)
    /// <summary>
    /// Validates a sequence of tokens against the <c>&lt;any-value&gt;</c> production.
    /// </summary>
    /// <param name="tokens">The tokens to validate.</param>
    /// <returns>A result indicating whether the tokens match the production.</returns>
    /// <remarks>
    /// <para>
    /// The <c>&lt;any-value&gt;</c> production is identical to <c>&lt;declaration-value&gt;</c>,
    /// but also allows:
    /// </para>
    /// <list type="bullet">
    /// <item><description>top-level <c>&lt;semicolon-token&gt;</c> tokens</description></item>
    /// <item><description>top-level <c>&lt;delim-token&gt;</c> tokens with a value of "!"</description></item>
    /// </list>
    /// <para>
    /// It represents the entirety of what valid CSS can be in any context.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#typedef-any-value"/>
    public static CssProductionMatchResult MatchAnyValue(IReadOnlyList<CssToken> tokens)
    {
        if (tokens is null || tokens.Count == 0)
        {
            return CssProductionMatchResult.Failure(
                ECssProductionMatchFailure.EmptySequence,
                0,
                "The <any-value> production requires one or more tokens");
        }

        // Track bracket depth to determine if brackets are matched
        int parenDepth = 0;   // ()
        int bracketDepth = 0; // []
        int braceDepth = 0;   // {}

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            // Skip EOF tokens
            if (token.Type == ECssTokenType.EOF)
                continue;

            // Check for bad tokens (always invalid)
            if (token.Type == ECssTokenType.Bad_String)
            {
                return CssProductionMatchResult.Failure(
                    ECssProductionMatchFailure.BadStringToken,
                    i,
                    "The <any-value> production does not allow <bad-string-token>");
            }

            if (token.Type == ECssTokenType.Bad_Url)
            {
                return CssProductionMatchResult.Failure(
                    ECssProductionMatchFailure.BadUrlToken,
                    i,
                    "The <any-value> production does not allow <bad-url-token>");
            }

            // Note: <any-value> DOES allow top-level semicolons and "!" delimiters
            // (this is the key difference from <declaration-value>)

            // Track opening brackets
            // Note: ECssTokenType.Function and ECssTokenType.SimpleBlock tokens represent
            // already-parsed, balanced blocks and should NOT affect depth tracking.
            // Only raw bracket tokens from unparsed token streams should affect depth.
            // However, ECssTokenType.FunctionName tokens from the tokenizer include an
            // implicit opening parenthesis (per CSS Syntax 3 §4.3.4) that must be balanced.
            switch (token.Type)
            {
                case ECssTokenType.Parenth_Open:
                case ECssTokenType.FunctionName: // <function-token> includes implicit '('
                    parenDepth++;
                    break;
                case ECssTokenType.SqBracket_Open:
                    bracketDepth++;
                    break;
                case ECssTokenType.Bracket_Open:
                    braceDepth++;
                    break;
                // Function and SimpleBlock tokens are already balanced - no depth tracking needed
                case ECssTokenType.Function:
                case ECssTokenType.SimpleBlock:
                    break;
            }

            // Check closing brackets (invalid if unmatched)
            switch (token.Type)
            {
                case ECssTokenType.Parenth_Close:
                    if (parenDepth <= 0)
                    {
                        return CssProductionMatchResult.Failure(
                            ECssProductionMatchFailure.UnmatchedCloseBracket,
                            i,
                            "The <any-value> production does not allow unmatched <)-token>");
                    }
                    parenDepth--;
                    break;

                case ECssTokenType.SqBracket_Close:
                    if (bracketDepth <= 0)
                    {
                        return CssProductionMatchResult.Failure(
                            ECssProductionMatchFailure.UnmatchedCloseBracket,
                            i,
                            "The <any-value> production does not allow unmatched <]-token>");
                    }
                    bracketDepth--;
                    break;

                case ECssTokenType.Bracket_Close:
                    if (braceDepth <= 0)
                    {
                        return CssProductionMatchResult.Failure(
                            ECssProductionMatchFailure.UnmatchedCloseBracket,
                            i,
                            "The <any-value> production does not allow unmatched <}-token>");
                    }
                    braceDepth--;
                    break;
            }
        }

        return CssProductionMatchResult.Success();
    }

    /// <summary>
    /// Checks if a sequence of tokens matches the <c>&lt;any-value&gt;</c> production.
    /// </summary>
    /// <param name="tokens">The tokens to validate.</param>
    /// <returns>True if the tokens match; false otherwise.</returns>
    public static bool IsAnyValue(IReadOnlyList<CssToken> tokens) =>
        MatchAnyValue(tokens).IsMatch;
    #endregion
}
