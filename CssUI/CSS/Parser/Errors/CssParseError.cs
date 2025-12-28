namespace CssUI.CSS.Parser;

/// <summary>
/// Represents a CSS parse error with location and type information.
/// </summary>
/// <remarks>
/// <para>
/// Per CSS Syntax Level 3, parse errors are well-defined points in the parsing algorithm
/// where error handling behavior must occur. The parser attempts to recover gracefully,
/// throwing away only the minimum amount of content before returning to parsing as normal.
/// </para>
/// <para>
/// This struct captures information about each parse error to allow conformance checkers
/// to report them, and to assist in debugging CSS issues.
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/#error-handling"/>
public readonly struct CssParseError
{
    #region Properties
    /// <summary>
    /// Gets the type of parse error that occurred.
    /// </summary>
    public ECssParseErrorType ErrorType { get; }

    /// <summary>
    /// Gets a human-readable message describing the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the approximate line number where the error occurred (1-based).
    /// May be 0 if line information is not available.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets the approximate column number where the error occurred (1-based).
    /// May be 0 if column information is not available.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets the token index in the stream where the error was detected.
    /// </summary>
    public int TokenIndex { get; }

    /// <summary>
    /// Gets the relevant token or content that caused the error, if available.
    /// </summary>
    public string? Context { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new CSS parse error.
    /// </summary>
    /// <param name="errorType">The type of parse error.</param>
    /// <param name="message">A human-readable error message.</param>
    /// <param name="line">The line number (1-based), or 0 if unknown.</param>
    /// <param name="column">The column number (1-based), or 0 if unknown.</param>
    /// <param name="tokenIndex">The token index in the stream.</param>
    /// <param name="context">Optional context about what caused the error.</param>
    public CssParseError(
        ECssParseErrorType errorType,
        string message,
        int line = 0,
        int column = 0,
        int tokenIndex = 0,
        string? context = null)
    {
        ErrorType = errorType;
        Message = message;
        Line = line;
        Column = column;
        TokenIndex = tokenIndex;
        Context = context;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates a parse error for an unterminated string.
    /// </summary>
    public static CssParseError UnterminatedString(int tokenIndex, string? context = null) =>
        new(ECssParseErrorType.UnterminatedString, "String was not properly terminated before EOF", tokenIndex: tokenIndex, context: context);

    /// <summary>
    /// Creates a parse error for an unterminated URL.
    /// </summary>
    public static CssParseError UnterminatedUrl(int tokenIndex, string? context = null) =>
        new(ECssParseErrorType.UnterminatedUrl, "URL was not properly terminated before EOF", tokenIndex: tokenIndex, context: context);

    /// <summary>
    /// Creates a parse error for an unterminated comment.
    /// </summary>
    public static CssParseError UnterminatedComment(int tokenIndex) =>
        new(ECssParseErrorType.UnterminatedComment, "Comment was not properly terminated before EOF", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for a bad string token.
    /// </summary>
    public static CssParseError BadString(int tokenIndex, string? context = null) =>
        new(ECssParseErrorType.BadString, "Invalid string (newline in unescaped string)", tokenIndex: tokenIndex, context: context);

    /// <summary>
    /// Creates a parse error for a bad URL token.
    /// </summary>
    public static CssParseError BadUrl(int tokenIndex, string? context = null) =>
        new(ECssParseErrorType.BadUrl, "Invalid URL syntax", tokenIndex: tokenIndex, context: context);

    /// <summary>
    /// Creates a parse error for an unterminated at-rule.
    /// </summary>
    public static CssParseError UnterminatedAtRule(int tokenIndex, string? ruleName = null) =>
        new(ECssParseErrorType.UnterminatedAtRule, $"At-rule '@{ruleName ?? "unknown"}' was not properly terminated", tokenIndex: tokenIndex, context: ruleName);

    /// <summary>
    /// Creates a parse error for an unterminated qualified rule.
    /// </summary>
    public static CssParseError UnterminatedQualifiedRule(int tokenIndex) =>
        new(ECssParseErrorType.UnterminatedQualifiedRule, "Qualified rule reached EOF without a block", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for an unterminated block.
    /// </summary>
    public static CssParseError UnterminatedBlock(int tokenIndex, char openingBracket) =>
        new(ECssParseErrorType.UnterminatedBlock, $"Block opened with '{openingBracket}' was not properly closed", tokenIndex: tokenIndex, context: openingBracket.ToString());

    /// <summary>
    /// Creates a parse error for an unterminated function.
    /// </summary>
    public static CssParseError UnterminatedFunction(int tokenIndex, string? functionName = null) =>
        new(ECssParseErrorType.UnterminatedFunction, $"Function '{functionName ?? "unknown"}()' was not properly terminated", tokenIndex: tokenIndex, context: functionName);

    /// <summary>
    /// Creates a parse error for a missing colon in a declaration.
    /// </summary>
    public static CssParseError MissingColonInDeclaration(int tokenIndex, string? propertyName = null) =>
        new(ECssParseErrorType.MissingColonInDeclaration, $"Declaration for '{propertyName ?? "unknown"}' is missing a colon", tokenIndex: tokenIndex, context: propertyName);

    /// <summary>
    /// Creates a parse error for an unexpected token in a declaration list.
    /// </summary>
    public static CssParseError UnexpectedTokenInDeclarationList(int tokenIndex, string? tokenType = null) =>
        new(ECssParseErrorType.UnexpectedTokenInDeclarationList, $"Unexpected token '{tokenType ?? "unknown"}' in declaration list", tokenIndex: tokenIndex, context: tokenType);

    /// <summary>
    /// Creates a parse error for an unexpected token in a style block.
    /// </summary>
    public static CssParseError UnexpectedTokenInStyleBlock(int tokenIndex, string? tokenType = null) =>
        new(ECssParseErrorType.UnexpectedTokenInStyleBlock, $"Unexpected token '{tokenType ?? "unknown"}' in style block", tokenIndex: tokenIndex, context: tokenType);

    /// <summary>
    /// Creates a parse error for an unmatched closing bracket.
    /// </summary>
    public static CssParseError UnmatchedClosingBracket(int tokenIndex, char bracket) =>
        new(ECssParseErrorType.UnmatchedClosingBracket, $"Unmatched closing bracket '{bracket}'", tokenIndex: tokenIndex, context: bracket.ToString());

    /// <summary>
    /// Creates a parse error for a bad-string-token encountered during parsing.
    /// </summary>
    public static CssParseError BadStringToken(int tokenIndex) =>
        new(ECssParseErrorType.BadStringToken, "Bad string token encountered", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for a bad-url-token encountered during parsing.
    /// </summary>
    public static CssParseError BadUrlToken(int tokenIndex) =>
        new(ECssParseErrorType.BadUrlToken, "Bad URL token encountered", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for an invalid hex color.
    /// </summary>
    public static CssParseError InvalidHexColor(int tokenIndex, string? value = null) =>
        new(ECssParseErrorType.InvalidHexColor, $"Invalid hex color value: #{value ?? "unknown"}", tokenIndex: tokenIndex, context: value);

    /// <summary>
    /// Creates a parse error for an invalid URL syntax.
    /// </summary>
    public static CssParseError InvalidUrlSyntax(int tokenIndex) =>
        new(ECssParseErrorType.InvalidUrlSyntax, "Invalid URL syntax", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for an unhandled token type.
    /// </summary>
    public static CssParseError UnhandledTokenType(int tokenIndex, string? tokenType = null) =>
        new(ECssParseErrorType.UnhandledTokenType, $"Unhandled token type: {tokenType ?? "unknown"}", tokenIndex: tokenIndex, context: tokenType);

    /// <summary>
    /// Creates a parse error for unexpected EOF.
    /// </summary>
    public static CssParseError UnexpectedEof(int tokenIndex) =>
        new(ECssParseErrorType.UnexpectedEof, "Unexpected end of input", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for extra content after what should be the end of input.
    /// </summary>
    public static CssParseError ExtraContentAfterRule(int tokenIndex) =>
        new(ECssParseErrorType.ExtraContentAfterRule, "Extra content found after rule (expected EOF)", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for when an identifier was expected but not found.
    /// </summary>
    public static CssParseError ExpectedIdentifier(int tokenIndex, string? found = null) =>
        new(ECssParseErrorType.ExpectedIdentifier, $"Expected identifier{(found != null ? $", found '{found}'" : "")}", tokenIndex: tokenIndex, context: found);

    /// <summary>
    /// Creates a parse error for an invalid declaration.
    /// </summary>
    public static CssParseError InvalidDeclaration(int tokenIndex, string? propertyName = null) =>
        new(ECssParseErrorType.InvalidDeclaration, $"Failed to parse declaration{(propertyName != null ? $" for '{propertyName}'" : "")}", tokenIndex: tokenIndex, context: propertyName);

    /// <summary>
    /// Creates a parse error for an invalid component value.
    /// </summary>
    public static CssParseError InvalidComponentValue(int tokenIndex) =>
        new(ECssParseErrorType.InvalidComponentValue, "Failed to parse component value", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for an invalid media query.
    /// </summary>
    public static CssParseError InvalidMediaQuery(int tokenIndex, string? context = null) =>
        new(ECssParseErrorType.InvalidMediaQuery, $"Invalid media query{(context != null ? $": {context}" : "")}", tokenIndex: tokenIndex, context: context);

    /// <summary>
    /// Creates a parse error for an invalid media feature.
    /// </summary>
    public static CssParseError InvalidMediaFeature(int tokenIndex, string? featureName = null) =>
        new(ECssParseErrorType.InvalidMediaFeature, $"Invalid media feature{(featureName != null ? $": {featureName}" : "")}", tokenIndex: tokenIndex, context: featureName);

    /// <summary>
    /// Creates a parse error for when a comparator was expected.
    /// </summary>
    public static CssParseError ExpectedComparator(int tokenIndex) =>
        new(ECssParseErrorType.ExpectedComparator, "Expected comparator (=, <, >, <=, >=)", tokenIndex: tokenIndex);

    /// <summary>
    /// Creates a parse error for an unexpected token.
    /// </summary>
    public static CssParseError UnexpectedToken(int tokenIndex, string? tokenType = null) =>
        new(ECssParseErrorType.UnexpectedToken, $"Unexpected token{(tokenType != null ? $": {tokenType}" : "")}", tokenIndex: tokenIndex, context: tokenType);
    #endregion

    #region Object Overrides
    /// <inheritdoc />
    public override string ToString()
    {
        var location = Line > 0 ? $" at line {Line}" : string.Empty;
        if (Column > 0) location += $", column {Column}";
        return $"[{ErrorType}]{location}: {Message}";
    }
    #endregion
}
