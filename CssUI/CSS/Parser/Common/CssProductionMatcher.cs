using System.Collections.Generic;

namespace CssUI.CSS.Parser;

/// <summary>
/// Provides methods to validate CSS token sequences against grammar productions
/// defined in CSS Syntax Level 3 §8.
/// </summary>
/// <remarks>
/// <para>
/// The productions defined here are used for validating arbitrary content in CSS:
/// </para>
/// <list type="bullet">
/// <item><description><c>&lt;declaration-value&gt;</c> - Valid content for a declaration's value</description></item>
/// <item><description><c>&lt;any-value&gt;</c> - Valid CSS content in any context</description></item>
/// </list>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#any-value"/>
public static class CssProductionMatcher
{
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
            switch (token.Type)
            {
                case ECssTokenType.Parenth_Open:
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
            switch (token.Type)
            {
                case ECssTokenType.Parenth_Open:
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
