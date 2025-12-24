using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CssUI.CSS.Parser;

/// <summary>
/// Parses <c>@supports</c> conditions as defined in CSS Conditional Rules Level 3 §6.
/// </summary>
/// <remarks>
/// <para>
/// The grammar for <c>@supports</c> conditions is:
/// </para>
/// <code>
/// &lt;supports-condition&gt; = not &lt;supports-in-parens&gt;
///                       | &lt;supports-in-parens&gt; [ and &lt;supports-in-parens&gt; ]*
///                       | &lt;supports-in-parens&gt; [ or &lt;supports-in-parens&gt; ]*
///
/// &lt;supports-in-parens&gt; = ( &lt;supports-condition&gt; )
///                       | &lt;supports-feature&gt;
///                       | &lt;general-enclosed&gt;
///
/// &lt;supports-feature&gt;   = &lt;supports-decl&gt;
///
/// &lt;supports-decl&gt;      = ( &lt;declaration&gt; )
/// </code>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-conditional-3/#at-supports"/>
public static class CssSupportsConditionParser
{
    /// <summary>
    /// Parses a <c>@supports</c> condition from component values.
    /// </summary>
    /// <param name="prelude">The prelude tokens of the @supports rule.</param>
    /// <returns>
    /// A <see cref="CssSupportsConditionResult"/> indicating success or failure,
    /// with the parsed condition if successful.
    /// </returns>
    public static CssSupportsConditionResult TryParse(IReadOnlyList<CssComponent> prelude)
    {
        if (prelude is null || prelude.Count == 0)
        {
            return CssSupportsConditionResult.Failure("Empty @supports condition");
        }

        // Convert component values to a consumable stream
        var tokens = ExtractTokens(prelude);
        if (tokens.Length == 0)
        {
            return CssSupportsConditionResult.Failure("Empty @supports condition after filtering whitespace");
        }

        var stream = new DataConsumer<CssToken>(tokens);

        // Try to parse <supports-condition>
        var result = TryParseSupportsCondition(stream);

        // Verify we consumed all tokens
        if (result.Success && !stream.atEOF)
        {
            SkipWhitespace(stream);
            if (!stream.atEOF)
            {
                return CssSupportsConditionResult.Failure("Unexpected tokens after @supports condition");
            }
        }

        return result;
    }

    /// <summary>
    /// Parses a <c>@supports</c> condition from a token list (for testing).
    /// </summary>
    /// <param name="tokens">The tokens to parse.</param>
    /// <returns>
    /// A <see cref="CssSupportsConditionResult"/> indicating success or failure,
    /// with the parsed condition if successful.
    /// </returns>
    public static CssSupportsConditionResult TryParse(IReadOnlyList<CssToken> tokens)
    {
        if (tokens is null || tokens.Count == 0)
        {
            return CssSupportsConditionResult.Failure("Empty @supports condition");
        }

        // Copy to array for DataConsumer
        var tokenArray = new CssToken[tokens.Count];
        for (int i = 0; i < tokens.Count; i++)
        {
            tokenArray[i] = tokens[i];
        }

        var stream = new DataConsumer<CssToken>(tokenArray, CssToken.EOF);

        // Try to parse <supports-condition>
        var result = TryParseSupportsCondition(stream);

        // Verify we consumed all tokens
        if (result.Success && !stream.atEOF)
        {
            SkipWhitespace(stream);
            if (!stream.atEOF)
            {
                return CssSupportsConditionResult.Failure("Unexpected tokens after @supports condition");
            }
        }

        return result;
    }

    #region <supports-condition>
    /// <summary>
    /// Parses <c>&lt;supports-condition&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>Grammar:</para>
    /// <code>
    /// &lt;supports-condition&gt; = not &lt;supports-in-parens&gt;
    ///                       | &lt;supports-in-parens&gt; [ and &lt;supports-in-parens&gt; ]*
    ///                       | &lt;supports-in-parens&gt; [ or &lt;supports-in-parens&gt; ]*
    /// </code>
    /// </remarks>
    private static CssSupportsConditionResult TryParseSupportsCondition(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);

        if (stream.atEOF)
        {
            return CssSupportsConditionResult.Failure("Unexpected end of @supports condition");
        }

        // Check for 'not' keyword
        var token = stream.Next;
        if (token is IdentToken identToken &&
            identToken.Value.Equals("not", StringComparison.OrdinalIgnoreCase))
        {
            stream.Consume(); // consume 'not'

            // 'not' must be followed by whitespace
            if (!ConsumeRequiredWhitespace(stream))
            {
                return CssSupportsConditionResult.Failure("'not' must be followed by whitespace in @supports condition");
            }

            // Parse <supports-in-parens>
            var childResult = TryParseSupportsInParens(stream);
            if (!childResult.Success)
                return childResult;

            return CssSupportsConditionResult.Ok(new CssSupportsNot(childResult.Condition!));
        }

        // Parse first <supports-in-parens>
        var firstResult = TryParseSupportsInParens(stream);
        if (!firstResult.Success)
            return firstResult;

        SkipWhitespace(stream);

        // Check for 'and' or 'or' continuations
        if (!stream.atEOF)
        {
            token = stream.Next;
            if (token is IdentToken opToken)
            {
                bool isAnd = opToken.Value.Equals("and", StringComparison.OrdinalIgnoreCase);
                bool isOr = opToken.Value.Equals("or", StringComparison.OrdinalIgnoreCase);

                if (isAnd || isOr)
                {
                    // Parse conjunction or disjunction
                    return TryParseAndOr(stream, firstResult.Condition!, isAnd);
                }
            }
        }

        // Just a single <supports-in-parens>
        return firstResult;
    }

    /// <summary>
    /// Parses an 'and' or 'or' chain.
    /// </summary>
    private static CssSupportsConditionResult TryParseAndOr(
        DataConsumer<CssToken> stream,
        CssSupportsCondition first,
        bool isAnd)
    {
        var children = ImmutableArray.CreateBuilder<CssSupportsCondition>();
        children.Add(first);

        string expectedOp = isAnd ? "and" : "or";

        while (!stream.atEOF)
        {
            SkipWhitespace(stream);

            var token = stream.Next;
            if (token is not IdentToken identToken)
                break;

            bool tokenIsAnd = identToken.Value.Equals("and", StringComparison.OrdinalIgnoreCase);
            bool tokenIsOr = identToken.Value.Equals("or", StringComparison.OrdinalIgnoreCase);

            if (!tokenIsAnd && !tokenIsOr)
                break;

            // Cannot mix 'and' and 'or' without parentheses
            if ((isAnd && tokenIsOr) || (!isAnd && tokenIsAnd))
            {
                return CssSupportsConditionResult.Failure(
                    $"Cannot mix 'and' and 'or' without parentheses in @supports condition. " +
                    $"Expected '{expectedOp}' but found '{identToken.Value}'");
            }

            stream.Consume(); // consume 'and'/'or'

            // Must be followed by whitespace
            if (!ConsumeRequiredWhitespace(stream))
            {
                return CssSupportsConditionResult.Failure($"'{expectedOp}' must be followed by whitespace in @supports condition");
            }

            // Parse next <supports-in-parens>
            var childResult = TryParseSupportsInParens(stream);
            if (!childResult.Success)
                return childResult;

            children.Add(childResult.Condition!);
        }

        if (children.Count < 2)
        {
            return CssSupportsConditionResult.Failure($"'{expectedOp}' requires at least two conditions");
        }

        CssSupportsCondition result = isAnd
            ? new CssSupportsAnd(children.ToImmutable())
            : new CssSupportsOr(children.ToImmutable());

        return CssSupportsConditionResult.Ok(result);
    }
    #endregion

    #region <supports-in-parens>
    /// <summary>
    /// Parses <c>&lt;supports-in-parens&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>Grammar:</para>
    /// <code>
    /// &lt;supports-in-parens&gt; = ( &lt;supports-condition&gt; )
    ///                       | &lt;supports-feature&gt;
    ///                       | &lt;general-enclosed&gt;
    ///
    /// &lt;supports-feature&gt;   = &lt;supports-decl&gt;
    /// &lt;supports-decl&gt;      = ( &lt;declaration&gt; )
    /// </code>
    /// </remarks>
    private static CssSupportsConditionResult TryParseSupportsInParens(DataConsumer<CssToken> stream)
    {
        SkipWhitespace(stream);

        if (stream.atEOF)
        {
            return CssSupportsConditionResult.Failure("Expected '(' in @supports condition");
        }

        var token = stream.Next;

        // Must start with '(' or a function
        if (token.Type == ECssTokenType.Parenth_Open)
        {
            return TryParseParenthesizedCondition(stream);
        }
        else if (token.Type == ECssTokenType.FunctionName)
        {
            // This is <general-enclosed> with a function
            return TryParseGeneralEnclosedFunction(stream);
        }

        return CssSupportsConditionResult.Failure(
            $"Expected '(' or function in @supports condition, found {token.Type}");
    }

    /// <summary>
    /// Parses a parenthesized condition: either ( &lt;declaration&gt; ), ( &lt;supports-condition&gt; ),
    /// or &lt;general-enclosed&gt;.
    /// </summary>
    private static CssSupportsConditionResult TryParseParenthesizedCondition(DataConsumer<CssToken> stream)
    {
        // Consume '('
        stream.Consume();
        SkipWhitespace(stream);

        if (stream.atEOF)
        {
            return CssSupportsConditionResult.Failure("Unclosed '(' in @supports condition");
        }

        // Peek to determine what we have:
        // - <ident-token> followed by ':' -> declaration
        // - 'not' keyword -> nested condition
        // - '(' -> nested condition (could be nested parens or declaration)
        // - anything else -> <general-enclosed>

        var firstToken = stream.Next;

        // Check for 'not' keyword - indicates nested condition
        if (firstToken is IdentToken notToken &&
            notToken.Value.Equals("not", StringComparison.OrdinalIgnoreCase))
        {
            return TryParseNestedCondition(stream);
        }

        // Check for nested '(' - indicates nested condition
        if (firstToken.Type == ECssTokenType.Parenth_Open)
        {
            return TryParseNestedCondition(stream);
        }

        // Check for ident followed by ':' - indicates declaration
        if (firstToken.Type == ECssTokenType.Ident)
        {
            // Need to look ahead to see if there's a ':'
            var savedPosition = stream.Position;
            stream.Consume(); // consume ident
            SkipWhitespace(stream);

            if (!stream.atEOF && stream.Next.Type == ECssTokenType.Colon)
            {
                // This is a declaration - backtrack and parse
                stream.Seek(savedPosition);
                return TryParseSupportsDeclaration(stream);
            }

            // Not a declaration - backtrack and try as nested condition or general-enclosed
            stream.Seek(savedPosition);

            // Check if this could be 'and' or 'or' (part of nested condition)
            if (firstToken is IdentToken keywordToken &&
                (keywordToken.Value.Equals("and", StringComparison.OrdinalIgnoreCase) ||
                 keywordToken.Value.Equals("or", StringComparison.OrdinalIgnoreCase)))
            {
                return CssSupportsConditionResult.Failure(
                    $"Unexpected '{keywordToken.Value}' in @supports condition - did you forget parentheses?");
            }

            // Parse as general-enclosed (already inside parens)
            return TryParseGeneralEnclosedContents(stream);
        }

        // Parse as general-enclosed (already inside parens)
        return TryParseGeneralEnclosedContents(stream);
    }

    /// <summary>
    /// Parses a nested &lt;supports-condition&gt; inside parentheses.
    /// </summary>
    private static CssSupportsConditionResult TryParseNestedCondition(DataConsumer<CssToken> stream)
    {
        // Parse the inner condition
        var innerResult = TryParseSupportsCondition(stream);
        if (!innerResult.Success)
            return innerResult;

        SkipWhitespace(stream);

        // Expect closing ')'
        if (stream.atEOF || stream.Next.Type != ECssTokenType.Parenth_Close)
        {
            return CssSupportsConditionResult.Failure("Expected ')' to close @supports condition");
        }

        stream.Consume(); // consume ')'

        return CssSupportsConditionResult.Ok(new CssSupportsNested(innerResult.Condition!));
    }
    #endregion

    #region <supports-decl>
    /// <summary>
    /// Parses <c>&lt;supports-decl&gt;</c>: a declaration inside parentheses.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The declaration value is validated against the <c>&lt;declaration-value&gt;</c> production.
    /// </para>
    /// </remarks>
    private static CssSupportsConditionResult TryParseSupportsDeclaration(DataConsumer<CssToken> stream)
    {
        // We're already past the '(' - now parse the declaration

        // Get property name
        var propToken = stream.Consume();
        if (propToken is not IdentToken identToken)
        {
            return CssSupportsConditionResult.Failure("Expected property name in @supports declaration");
        }

        string propertyName = identToken.Value;

        SkipWhitespace(stream);

        // Expect ':'
        if (stream.atEOF || stream.Next.Type != ECssTokenType.Colon)
        {
            return CssSupportsConditionResult.Failure("Expected ':' after property name in @supports declaration");
        }

        stream.Consume(); // consume ':'
        SkipWhitespace(stream);

        // Collect value tokens until ')' (or EOF)
        var valueTokens = ImmutableArray.CreateBuilder<CssToken>();
        int parenDepth = 0;
        bool foundClose = false;

        while (!stream.atEOF)
        {
            var token = stream.Next;

            if (token.Type == ECssTokenType.Parenth_Close && parenDepth == 0)
            {
                // Check for !important before closing
                // (handled after collecting all tokens)
                foundClose = true;
                break;
            }

            // Track nested parentheses
            if (token.Type == ECssTokenType.Parenth_Open ||
                token.Type == ECssTokenType.FunctionName)
            {
                parenDepth++;
            }
            else if (token.Type == ECssTokenType.Parenth_Close)
            {
                parenDepth--;
            }

            valueTokens.Add(token);
            stream.Consume();
        }

        if (!foundClose)
        {
            return CssSupportsConditionResult.Failure("Expected ')' to close @supports declaration");
        }

        stream.Consume(); // consume ')'

        // Check for !important at the end
        var value = valueTokens.ToImmutable();
        bool isImportant;
        (value, isImportant) = ExtractImportant(value);

        // Validate value against <declaration-value> production
        bool isValid = CssProductionMatcher.IsDeclarationValue(value);

        return CssSupportsConditionResult.Ok(
            new CssSupportsDeclaration(propertyName, value, isImportant, isValid));
    }

    /// <summary>
    /// Extracts and removes !important from the end of a value token list.
    /// </summary>
    private static (ImmutableArray<CssToken> value, bool isImportant) ExtractImportant(ImmutableArray<CssToken> tokens)
    {
        // Look for '!' followed by 'important' at the end, ignoring trailing whitespace
        if (tokens.Length < 2)
            return (tokens, false);

        // Work backwards, skipping whitespace
        int endIndex = tokens.Length - 1;
        while (endIndex >= 0 && tokens[endIndex].Type == ECssTokenType.Whitespace)
        {
            endIndex--;
        }

        if (endIndex < 1)
            return (tokens, false);

        // Check for 'important' ident
        if (tokens[endIndex] is IdentToken importantToken &&
            importantToken.Value.Equals("important", StringComparison.OrdinalIgnoreCase))
        {
            int exclamationIndex = endIndex - 1;
            while (exclamationIndex >= 0 && tokens[exclamationIndex].Type == ECssTokenType.Whitespace)
            {
                exclamationIndex--;
            }

            if (exclamationIndex >= 0 &&
                tokens[exclamationIndex] is DelimToken delimToken &&
                delimToken.Value == UnicodeCommon.CHAR_EXCLAMATION_POINT)
            {
                // Found !important - remove it from value
                var newValue = tokens.RemoveRange(exclamationIndex, tokens.Length - exclamationIndex);

                // Remove trailing whitespace
                while (newValue.Length > 0 && newValue[^1].Type == ECssTokenType.Whitespace)
                {
                    newValue = newValue.RemoveAt(newValue.Length - 1);
                }

                return (newValue, true);
            }
        }

        return (tokens, false);
    }
    #endregion

    #region <general-enclosed>
    /// <summary>
    /// Parses a function as &lt;general-enclosed&gt;.
    /// </summary>
    private static CssSupportsConditionResult TryParseGeneralEnclosedFunction(DataConsumer<CssToken> stream)
    {
        var funcToken = stream.Consume();
        if (funcToken is not FunctionNameToken)
        {
            return CssSupportsConditionResult.Failure("Expected function in @supports general-enclosed");
        }

        // Collect tokens until matching ')'
        var tokens = ImmutableArray.CreateBuilder<CssToken>();
        tokens.Add(funcToken);

        int parenDepth = 1;

        while (!stream.atEOF && parenDepth > 0)
        {
            var token = stream.Consume();
            tokens.Add(token);

            if (token.Type == ECssTokenType.Parenth_Open ||
                token.Type == ECssTokenType.FunctionName)
            {
                parenDepth++;
            }
            else if (token.Type == ECssTokenType.Parenth_Close)
            {
                parenDepth--;
            }
        }

        if (parenDepth > 0)
        {
            return CssSupportsConditionResult.Failure("Unclosed function in @supports general-enclosed");
        }

        // Validate content against <any-value> production
        bool isValid = CssProductionMatcher.IsAnyValue(tokens.ToImmutable());

        return CssSupportsConditionResult.Ok(
            new CssSupportsGeneralEnclosed(tokens.ToImmutable(), isValid));
    }

    /// <summary>
    /// Parses content inside parens as &lt;general-enclosed&gt; (we're already inside the parens).
    /// </summary>
    private static CssSupportsConditionResult TryParseGeneralEnclosedContents(DataConsumer<CssToken> stream)
    {
        // Collect tokens until matching ')'
        var tokens = ImmutableArray.CreateBuilder<CssToken>();
        int parenDepth = 0;

        while (!stream.atEOF)
        {
            var token = stream.Next;

            if (token.Type == ECssTokenType.Parenth_Close && parenDepth == 0)
            {
                break;
            }

            tokens.Add(token);
            stream.Consume();

            if (token.Type == ECssTokenType.Parenth_Open ||
                token.Type == ECssTokenType.FunctionName)
            {
                parenDepth++;
            }
            else if (token.Type == ECssTokenType.Parenth_Close)
            {
                parenDepth--;
            }
        }

        if (stream.atEOF)
        {
            return CssSupportsConditionResult.Failure("Unclosed '(' in @supports general-enclosed");
        }

        stream.Consume(); // consume ')'

        // Validate content against <any-value> production
        bool isValid = tokens.Count == 0 || CssProductionMatcher.IsAnyValue(tokens.ToImmutable());

        return CssSupportsConditionResult.Ok(
            new CssSupportsGeneralEnclosed(tokens.ToImmutable(), isValid));
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Extracts tokens from component values, flattening nested structures.
    /// </summary>
    private static CssToken[] ExtractTokens(IReadOnlyList<CssComponent> components)
    {
        var result = new List<CssToken>();

        foreach (var component in components)
        {
            if (component is CssToken token)
            {
                result.Add(token);
            }
            else if (component is CssFunction func)
            {
                // Add function opening
                result.Add(new FunctionNameToken(func.Name));

                // Add function arguments
                foreach (var arg in func.Arguments)
                {
                    if (arg is CssToken argToken)
                    {
                        result.Add(argToken);
                    }
                }

                // Add closing paren
                result.Add(ParenthesisCloseToken.Instance);
            }
            else if (component is CssSimpleBlock block)
            {
                // Add block tokens
                result.Add(block.StartToken);
                foreach (var val in block.Values)
                {
                    result.Add(val);
                }

                // Add closing token based on block type
                switch (block.StartToken.Type)
                {
                    case ECssTokenType.Parenth_Open:
                        result.Add(ParenthesisCloseToken.Instance);
                        break;
                    case ECssTokenType.SqBracket_Open:
                        result.Add(SqBracketCloseToken.Instance);
                        break;
                    case ECssTokenType.Bracket_Open:
                        result.Add(BracketCloseToken.Instance);
                        break;
                }
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Skips whitespace tokens in the stream.
    /// </summary>
    private static void SkipWhitespace(DataConsumer<CssToken> stream)
    {
        while (!stream.atEOF && stream.Next.Type == ECssTokenType.Whitespace)
        {
            stream.Consume();
        }
    }

    /// <summary>
    /// Consumes required whitespace (returns false if not present).
    /// </summary>
    private static bool ConsumeRequiredWhitespace(DataConsumer<CssToken> stream)
    {
        if (stream.atEOF || stream.Next.Type != ECssTokenType.Whitespace)
        {
            return false;
        }

        SkipWhitespace(stream);
        return true;
    }
    #endregion
}

/// <summary>
/// Represents the result of parsing a <c>@supports</c> condition.
/// </summary>
public sealed class CssSupportsConditionResult
{
    /// <summary>
    /// Gets whether the parsing was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the parsed condition if successful; otherwise, null.
    /// </summary>
    public CssSupportsCondition? Condition { get; }

    /// <summary>
    /// Gets the error message if parsing failed; otherwise, null.
    /// </summary>
    public string? ErrorMessage { get; }

    private CssSupportsConditionResult(bool success, CssSupportsCondition? condition, string? errorMessage)
    {
        Success = success;
        Condition = condition;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result with the parsed condition.
    /// </summary>
    public static CssSupportsConditionResult Ok(CssSupportsCondition condition) =>
        new(true, condition, null);

    /// <summary>
    /// Creates a failure result with an error message.
    /// </summary>
    public static CssSupportsConditionResult Failure(string errorMessage) =>
        new(false, null, errorMessage);
}
