using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using CssUI.CSS.Exceptions;
using CssUI.CSS.Media;
using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Media;

namespace CssUI.CSS.Serialization;
/* Docs: https://www.w3.org/TR/css-syntax-3/ */

/// <summary>
/// Parses a stream of <see cref="CssToken"/>s and returns CSS objects such as stylesheets, rules, and declarations.
/// </summary>
/// <remarks>
/// <para>
/// This parser implements CSS Syntax Level 3 with error recovery per §3 and §5.
/// Parse errors are well-defined and the parser attempts to recover gracefully,
/// throwing away only the minimum amount of content before resuming normal parsing.
/// </para>
/// <para>
/// Error handling modes:
/// <list type="bullet">
/// <item><description><see cref="EParseErrorMode.Recover"/> - Continue parsing after errors (default)</description></item>
/// <item><description><see cref="EParseErrorMode.Abort"/> - Throw exception on first error</description></item>
/// </list>
/// </para>
/// </remarks>
/// <seealso href="https://www.w3.org/TR/css-syntax-3/"/>
public class CssParser
{
    #region Properties
    private readonly DataConsumer<CssToken> Stream;
    private bool TopLevel = false;

    /// <summary>
    /// Gets the error reporter used to collect parse errors.
    /// </summary>
    public ICssParseErrorReporter ErrorReporter { get; }

    /// <summary>
    /// Gets the error handling mode for this parser instance.
    /// </summary>
    public EParseErrorMode ErrorMode { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new CssParser from CSS text.
    /// </summary>
    /// <param name="Text">The CSS text to parse.</param>
    /// <param name="errorReporter">Optional error reporter. If null, errors are discarded.</param>
    /// <param name="errorMode">The error handling mode.</param>
    public CssParser(ReadOnlySpan<char> Text, ICssParseErrorReporter? errorReporter = null, EParseErrorMode errorMode = EParseErrorMode.Recover)
    {
        CssTokenizer Tokenizer = new CssTokenizer(Text);
        Stream = new DataConsumer<CssToken>(Tokenizer.Tokens, CssToken.EOF);
        ErrorReporter = errorReporter ?? NullParseErrorReporter.Instance;
        ErrorMode = errorMode;
    }

    /// <summary>
    /// Creates a new CssParser from pre-tokenized tokens.
    /// </summary>
    /// <param name="Tokens">The tokens to parse.</param>
    /// <param name="errorReporter">Optional error reporter. If null, errors are discarded.</param>
    /// <param name="errorMode">The error handling mode.</param>
    public CssParser(CssToken[] Tokens, ICssParseErrorReporter? errorReporter = null, EParseErrorMode errorMode = EParseErrorMode.Recover)
    {
        Stream = new DataConsumer<CssToken>(Tokens, CssToken.EOF);
        ErrorReporter = errorReporter ?? NullParseErrorReporter.Instance;
        ErrorMode = errorMode;
    }

    /// <summary>
    /// Creates a CssParser from a list of CSS component values.
    /// </summary>
    /// <param name="componentValues">The component values to parse.</param>
    /// <param name="errorReporter">Optional error reporter. If null, errors are discarded.</param>
    /// <param name="errorMode">The error handling mode.</param>
    public CssParser(IEnumerable<CssToken> componentValues, ICssParseErrorReporter? errorReporter = null, EParseErrorMode errorMode = EParseErrorMode.Recover)
    {
        Stream = new DataConsumer<CssToken>(componentValues.ToArray(), CssToken.EOF);
        ErrorReporter = errorReporter ?? NullParseErrorReporter.Instance;
        ErrorMode = errorMode;
    }
    #endregion

    #region Error Reporting Helpers
    /// <summary>
    /// Reports a parse error and throws if abort mode is enabled.
    /// </summary>
    /// <param name="error">The parse error to report.</param>
    private void ReportError(CssParseError error)
    {
        ErrorReporter.ReportError(error);
        if (ErrorMode == EParseErrorMode.Abort)
        {
            throw new CssSyntaxErrorException($"Parse error: {error}");
        }
    }

    /// <summary>
    /// Reports a parse error for an unterminated at-rule.
    /// </summary>
    private void ReportUnterminatedAtRule(string? name) =>
        ReportError(CssParseError.UnterminatedAtRule(Stream.Position, name));

    /// <summary>
    /// Reports a parse error for an unterminated qualified rule.
    /// </summary>
    private void ReportUnterminatedQualifiedRule() =>
        ReportError(CssParseError.UnterminatedQualifiedRule(Stream.Position));

    /// <summary>
    /// Reports a parse error for an unterminated block.
    /// </summary>
    private void ReportUnterminatedBlock(char bracket) =>
        ReportError(CssParseError.UnterminatedBlock(Stream.Position, bracket));

    /// <summary>
    /// Reports a parse error for an unterminated function.
    /// </summary>
    private void ReportUnterminatedFunction(string? name) =>
        ReportError(CssParseError.UnterminatedFunction(Stream.Position, name));

    /// <summary>
    /// Reports a parse error for a missing colon in declaration.
    /// </summary>
    private void ReportMissingColonInDeclaration(string? propertyName) =>
        ReportError(CssParseError.MissingColonInDeclaration(Stream.Position, propertyName));

    /// <summary>
    /// Reports a parse error for an unexpected token in a declaration list.
    /// </summary>
    private void ReportUnexpectedTokenInDeclarationList(ECssTokenType tokenType) =>
        ReportError(CssParseError.UnexpectedTokenInDeclarationList(Stream.Position, tokenType.ToString()));

    /// <summary>
    /// Reports a parse error for an unexpected token in a style block.
    /// </summary>
    private void ReportUnexpectedTokenInStyleBlock(ECssTokenType tokenType) =>
        ReportError(CssParseError.UnexpectedTokenInStyleBlock(Stream.Position, tokenType.ToString()));

    /// <summary>
    /// Reports a parse error for a bad string token.
    /// </summary>
    private void ReportBadStringToken() =>
        ReportError(CssParseError.BadStringToken(Stream.Position));

    /// <summary>
    /// Reports a parse error for a bad URL token.
    /// </summary>
    private void ReportBadUrlToken() =>
        ReportError(CssParseError.BadUrlToken(Stream.Position));

    /// <summary>
    /// Reports a parse error for an unmatched closing bracket.
    /// </summary>
    private void ReportUnmatchedClosingBracket(char bracket) =>
        ReportError(CssParseError.UnmatchedClosingBracket(Stream.Position, bracket));

    /// <summary>
    /// Checks if a token is a "bad" token type that is always a parse error per CSS Syntax Level 3.
    /// If so, reports the error.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns>True if the token is a bad token that was reported; false otherwise.</returns>
    /// <remarks>
    /// Per CSS Syntax Level 3: "The tokens &lt;}-token&gt;, &lt;)-token&gt;, &lt;]-token&gt;,
    /// &lt;bad-string-token&gt;, and &lt;bad-url-token&gt; are always parse errors, but they
    /// are preserved in the token stream by this specification to allow other specs, such
    /// as Media Queries, to define more fine-grained error-handling than just dropping an
    /// entire declaration or block."
    /// </remarks>
    private bool CheckAndReportBadToken(CssToken token)
    {
        switch (token.Type)
        {
            case ECssTokenType.Bad_String:
                ReportBadStringToken();
                return true;
            case ECssTokenType.Bad_Url:
                ReportBadUrlToken();
                return true;
            // Note: Unmatched closing brackets are contextual - only report when they're
            // truly unmatched, not when they could be part of valid parsing
            default:
                return false;
        }
    }
    #endregion

    #region Static Normalization (§5.3)
    /// <summary>
    /// Normalizes input into a token stream per CSS Syntax Level 3 §5.3.
    /// </summary>
    /// <param name="input">A string containing CSS text.</param>
    /// <returns>An array of CSS tokens.</returns>
    /// <remarks>
    /// <para>
    /// To normalize into a token stream a given input:
    /// </para>
    /// <list type="number">
    /// <item>If input is a list of CSS tokens, return input.</item>
    /// <item>If input is a list of CSS component values, return input.</item>
    /// <item>If input is a string, then filter code points from input, tokenize the result, and return the final result.</item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#normalize-into-a-token-stream"/>
    public static CssToken[] Normalize(string input)
    {
        // If input is a string, then filter code points from input, tokenize the result, and return the final result.
        // Note: CssTokenizer already handles code point filtering per §3.3
        var tokenizer = new CssTokenizer(input.AsSpan());
        return tokenizer.Tokens.ToArray();
    }

    /// <summary>
    /// Normalizes input into a token stream per CSS Syntax Level 3 §5.3.
    /// </summary>
    /// <param name="tokens">A list of CSS tokens.</param>
    /// <returns>The same array of CSS tokens (identity operation).</returns>
    /// <remarks>
    /// When input is already a list of CSS tokens, it is returned as-is.
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#normalize-into-a-token-stream"/>
    public static CssToken[] Normalize(CssToken[] tokens)
    {
        // If input is a list of CSS tokens, return input.
        return tokens;
    }

    /// <summary>
    /// Normalizes input into a token stream per CSS Syntax Level 3 §5.3.
    /// </summary>
    /// <param name="componentValues">A list of CSS component values.</param>
    /// <returns>An array of CSS tokens.</returns>
    /// <remarks>
    /// <para>
    /// When input is a list of CSS component values, it is returned as-is.
    /// </para>
    /// <para>
    /// Note: The only difference between a list of tokens and a list of component values is
    /// that some objects that "contain" things, like functions or blocks, are a single
    /// entity in the component-value list, but are multiple entities in a token list.
    /// This makes no difference to any of the algorithms in this specification.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#normalize-into-a-token-stream"/>
    public static CssToken[] Normalize(IEnumerable<CssToken> componentValues)
    {
        // If input is a list of CSS component values, return input.
        return componentValues.ToArray();
    }
    #endregion

    #region Parsing

    #region Parser Entry Points (§5.3)
    /// <summary>
    /// Parses a stylesheet per CSS Syntax Level 3 §5.3.3.
    /// </summary>
    /// <param name="location">Optional URL location of the stylesheet.</param>
    /// <returns>A stylesheet result containing the parsed rules and location.</returns>
    /// <remarks>
    /// <para>
    /// To parse a stylesheet from an input given an optional url location:
    /// </para>
    /// <list type="number">
    /// <item>If input is a byte stream for stylesheet, decode bytes from input, and set input to the result.</item>
    /// <item>Normalize input, and set input to the result.</item>
    /// <item>Create a new stylesheet, with its location set to location (or null, if location was not passed).</item>
    /// <item>Consume a list of rules from input, with the top-level flag set, and set the stylesheet's value to the result.</item>
    /// <item>Return the stylesheet.</item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#parse-stylesheet"/>
    public CssParsedStylesheet Parse_Stylesheet(Uri? location = null)
    {
        // Note: Byte stream decoding is handled externally before CssParser is constructed
        // Input normalization is handled by the tokenizer (CssTokenizer filters code points)

        // Consume a list of rules with the top-level flag set
        TopLevel = true;
        var rules = Consume_Rule_List(Stream, TopLevel);

        // Return the stylesheet with its location
        return new CssParsedStylesheet(rules, location);
    }

    /// <summary>
    /// Parses a style block's contents per CSS Syntax Level 3 §5.3.7.
    /// </summary>
    /// <returns>
    /// A list containing both declarations and nested rules from the style block.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This algorithm parses the contents of style rules, which need to allow nested style rules
    /// and other at-rules. If you don't need nested style rules, such as in @page or in @keyframes
    /// child rules, use Parse_Decleration_List instead.
    /// </para>
    /// <para>
    /// To parse a style block's contents from input:
    /// </para>
    /// <list type="number">
    /// <item>Normalize input, and set input to the result.</item>
    /// <item>Consume a style block's contents from input, and return the result.</item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#parse-style-blocks-contents"/>
    public IEnumerable<CssComponent> Parse_Style_Block_Contents()
    {
        // Input normalization is handled by the tokenizer
        return Consume_Style_Block_Contents(Stream);
    }

    /// <summary>
    /// Parses block contents according to the specified content type.
    /// </summary>
    /// <param name="contentsType">The type of block contents to parse.</param>
    /// <returns>A list of parsed components (declarations, rules, or both).</returns>
    /// <remarks>
    /// <para>
    /// This method dispatches to the appropriate parser algorithm based on the content type:
    /// </para>
    /// <list type="bullet">
    /// <item><description><see cref="ECssBlockContentsType.StyleBlock"/> - Uses <c>Consume_Style_Block_Contents</c></description></item>
    /// <item><description><see cref="ECssBlockContentsType.DeclarationList"/> - Uses <c>Consume_Decleration_List</c></description></item>
    /// <item><description><see cref="ECssBlockContentsType.RuleList"/> - Uses <c>Consume_Rule_List</c> (top-level = false)</description></item>
    /// <item><description><see cref="ECssBlockContentsType.Stylesheet"/> - Uses <c>Consume_Rule_List</c> (top-level = true)</description></item>
    /// </list>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#declaration-rule-list"/>
    public IEnumerable<CssComponent> Parse_Block_Contents(ECssBlockContentsType contentsType)
    {
        return contentsType switch
        {
            ECssBlockContentsType.StyleBlock => Consume_Style_Block_Contents(Stream),
            ECssBlockContentsType.DeclarationList => Consume_Decleration_List(Stream),
            ECssBlockContentsType.RuleList => Consume_Rule_List(Stream, TopLevel: false),
            ECssBlockContentsType.Stylesheet => Consume_Rule_List(Stream, TopLevel: true),
            _ => throw new ArgumentOutOfRangeException(nameof(contentsType), contentsType, "Unknown block contents type")
        };
    }

    /// <summary>
    /// Parses and returns a list of rules
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CssComponent> Parse_Rule_List()
    {
        TopLevel = false;
        return Consume_Rule_List(Stream, TopLevel);
    }
    #endregion

    /// <summary>
    /// Parses and returns a single rule
    /// </summary>
    /// <returns></returns>
    public CssComponent Parse_Rule()
    {
        CssComponent Rule;
        Consume_All_Whitespace(Stream);// Consume all whitespace

        if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException(CssErrors.UNEXPECTED_EOF, Stream);
        else if (Stream.Next.Type == ECssTokenType.At_Keyword)
        {
            Rule = Consume_AtRule(Stream);
        }
        else
        {
            Rule = Consume_QualifiedRule(Stream);
            if (Rule is null) throw new CssSyntaxErrorException(CssErrors.CANT_CONSUME_QUALIFIED_RULE, Stream);
        }

        Consume_All_Whitespace(Stream);// Consume all whitespace
        if (Stream.Next.Type == ECssTokenType.EOF)
            return Rule;
        else
            throw new CssSyntaxErrorException(CssErrors.EOF_EXPECTED, Stream);
    }

    public CssDecleration? Parse_Decleration()
    {
        Consume_All_Whitespace(Stream);
        if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException(CssErrors.EXPECTING_IDENT, Stream);

        CssDecleration? Dec = Consume_Decleration(Stream);
        if (Dec is null) throw new CssSyntaxErrorException(CssErrors.CANT_CONSUME_DECLERATION, Stream);

        return Dec;
    }

    public IEnumerable<CssComponent> Parse_Decleration_List()
    {
        if (Stream is null) throw new System.InvalidOperationException("Stream is null - parser not properly initialized");
        return Consume_Decleration_List(Stream);
    }

    public CssToken Parse_ComponentValue()
    {
        Consume_All_Whitespace(Stream);
        if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException(CssErrors.UNEXPECTED_EOF);

        CssToken Res;
        Res = Consume_ComponentValue(Stream);
        if (Res is null) throw new CssSyntaxErrorException(CssErrors.CANT_CONSUME_COMPONENT_VALUE);

        Consume_All_Whitespace(Stream);
        if (Stream.Next.Type == ECssTokenType.EOF)
            return Res;
        else
            throw new CssSyntaxErrorException(CssErrors.EOF_EXPECTED);
    }

    public LinkedList<CssToken> Parse_ComponentValue_List()
    {
        LinkedList<CssToken> List = new LinkedList<CssToken>();
        CssToken Value;
        do
        {
            Value = Consume_ComponentValue(Stream);
            //if (Value.Type == ECssComponent.PreservedToken && (Value as CssPreservedToken).Value.Type == ECssTokenType.EOF)
            if (Value.Type == ECssTokenType.EOF)
                return List;

            List.AddLast(Value);
        }
        while (Value is not null);

        return List;
    }

    public CssValue Parse_CssValue()
    {
        Consume_All_Whitespace(Stream);
        return Consume_CssValue(Stream);
    }
    #endregion

    #region Consuming
    /// <summary>
    /// Continually consumes tokens until the current token is not a whitespace one
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
    static void Consume_All_Whitespace(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        Contract.EndContractBlock();

        while (Stream.Next != null && Stream.Next != CssToken.EOF && Stream.Next.Type == ECssTokenType.Whitespace) { Stream.Consume(); }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IEnumerable<CssComponent> Consume_Rule_List(DataConsumer<CssToken> Stream, bool TopLevel = false)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        Contract.EndContractBlock();

        LinkedList<CssComponent> Rules = new LinkedList<CssComponent>();

        CssToken Token;
        do
        {
            Token = Stream.Consume();
            switch (Token.Type)
            {
                case ECssTokenType.Whitespace:
                    continue;
                case ECssTokenType.EOF:
                    return Rules;
                case ECssTokenType.CDO:
                case ECssTokenType.CDC:
                    {
                        if (TopLevel) continue;
                        Stream.Reconsume();
                        var rule = Consume_QualifiedRule(Stream);
                        if (rule is not null) Rules.AddLast(rule);
                    }
                    break;
                case ECssTokenType.At_Keyword:
                    {
                        Stream.Reconsume();
                        var rule = Consume_AtRule(Stream);
                        if (rule is not null) Rules.AddLast(rule);
                    }
                    break;
                default:
                    {
                        Stream.Reconsume();
                        var rule = Consume_QualifiedRule(Stream);
                        if (rule is not null) Rules.AddLast(rule);
                    }
                    break;
            }
        }
        while (Token != CssToken.EOF);

        return Rules;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
    static CssAtRule Consume_AtRule(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        // Consume the at-keyword token and get its name
        var atToken = Stream.Consume();
        var name = (atToken as ValuedTokenBase)?.Value ?? string.Empty;
        CssAtRule Rule = new CssAtRule(name);
        CssToken Token;
        do
        {
            Token = Stream.Consume();
            switch (Token.Type)
            {
                case ECssTokenType.Semicolon:
                case ECssTokenType.EOF:
                    return Rule;
                case ECssTokenType.Bracket_Open:
                    Rule.Block = Consume_SimpleBlock(Stream, Token);
                    return Rule;
                default:
                    {
                        Stream.Reconsume();
                        Rule.Prelude.Add(Consume_ComponentValue(Stream));
                    }
                    break;
            }
        }
        while (Token.Type != ECssTokenType.EOF);
        return Rule;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
    static CssQualifiedRule? Consume_QualifiedRule(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        CssQualifiedRule Rule = new CssQualifiedRule();
        CssToken Token;
        do
        {
            Token = Stream.Consume();
            switch (Token.Type)
            {
                case ECssTokenType.EOF:
                    return null;
                case ECssTokenType.Bracket_Open:
                    {
                        Rule.Block = Consume_SimpleBlock(Stream, Token);
                        return Rule;
                    }
                default:
                    {
                        Stream.Reconsume();
                        Rule.Prelude.Add(Consume_ComponentValue(Stream));
                    }
                    break;
            }
        }
        while (Token.Type != ECssTokenType.EOF);
        return Rule;
    }

    /// <summary>
    /// Consumes a style block's contents per CSS Syntax Level 3 §5.4.4.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This algorithm is used for parsing the contents of style rules, which support nested rules.
    /// It differs from Consume_Decleration_List by handling:
    /// </para>
    /// <list type="bullet">
    /// <item>&lt;ident-token&gt; starts a declaration</item>
    /// <item>&lt;at-keyword-token&gt; starts a nested at-rule (added to rules list)</item>
    /// <item>&lt;delim-token&gt; with value "&amp;" starts a nested qualified rule (CSS Nesting)</item>
    /// </list>
    /// <para>
    /// Returns declarations and rules in a single list, with rules appended after declarations.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-style-block"/>
    static IEnumerable<CssComponent> Consume_Style_Block_Contents(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        // Create an initially empty list of declarations decls, and an initially empty list of rules rules.
        LinkedList<CssComponent> decls = new LinkedList<CssComponent>();
        LinkedList<CssComponent> rules = new LinkedList<CssComponent>();

        CssToken? Token;
        do
        {
            Token = Stream.Consume();
            if (Token is null) break;

            switch (Token.Type)
            {
                // <whitespace-token> or <semicolon-token>: Do nothing.
                case ECssTokenType.Whitespace:
                case ECssTokenType.Semicolon:
                    continue;

                // <EOF-token>: Extend decls with rules, then return decls.
                case ECssTokenType.EOF:
                    foreach (var rule in rules)
                    {
                        decls.AddLast(rule);
                    }
                    return decls;

                // <at-keyword-token>: Reconsume the current input token.
                // Consume an at-rule, and append the result to rules.
                case ECssTokenType.At_Keyword:
                    Stream.Reconsume();
                    rules.AddLast(Consume_AtRule(Stream));
                    break;

                // <ident-token>: Initialize a temporary list initially filled with the current input token.
                // As long as the next input token is anything other than a <semicolon-token> or <EOF-token>,
                // consume a component value and append it to the temporary list.
                // Consume a declaration from the temporary list. If anything was returned, append it to decls.
                case ECssTokenType.Ident:
                    {
                        List<CssToken> tmp = new List<CssToken> { Token };
                        while (Stream.Next is CssToken next &&
                               next.Type != ECssTokenType.EOF &&
                               next.Type != ECssTokenType.Semicolon)
                        {
                            tmp.Add(Consume_ComponentValue(Stream));
                        }
                        tmp.Add(EOFToken.Instance);

                        var decl = Consume_Decleration(new DataConsumer<CssToken>(tmp.ToArray(), CssToken.EOF));
                        if (decl is not null)
                        {
                            decls.AddLast(decl);
                        }
                    }
                    break;

                // <delim-token> with a value of "&" (U+0026 AMPERSAND):
                // Reconsume the current input token. Consume a qualified rule.
                // If anything was returned, append it to rules.
                case ECssTokenType.Delim when Token is DelimToken delimToken && delimToken.Value == UnicodeCommon.CHAR_AMPERSAND:
                    Stream.Reconsume();
                    var qualifiedRule = Consume_QualifiedRule(Stream);
                    if (qualifiedRule is not null)
                    {
                        rules.AddLast(qualifiedRule);
                    }
                    break;

                // anything else: This is a parse error. Reconsume the current input token.
                // As long as the next input token is anything other than a <semicolon-token> or <EOF-token>,
                // consume a component value and throw away the returned value.
                default:
                    Stream.Reconsume();
                    while (Stream.Next is CssToken nextToken &&
                           nextToken.Type != ECssTokenType.EOF &&
                           nextToken.Type != ECssTokenType.Semicolon)
                    {
                        Consume_ComponentValue(Stream);
                    }
                    break;
            }
        }
        while (Token is not null && Token.Type != ECssTokenType.EOF);

        // Extend decls with rules, then return decls.
        foreach (var rule in rules)
        {
            decls.AddLast(rule);
        }
        return decls;
    }

    static IEnumerable<CssComponent> Consume_Decleration_List(DataConsumer<CssToken> Stream)
    {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-list-of-declarations0
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        LinkedList<CssComponent> List = new LinkedList<CssComponent>();
        CssToken? Token;
        do
        {
            Token = Stream.Consume();
            if (Token is null) return List; // Safety check for null token

            switch (Token.Type)
            {
                case ECssTokenType.Whitespace:
                case ECssTokenType.Semicolon:
                    continue;
                case ECssTokenType.EOF:
                    return List;
                case ECssTokenType.At_Keyword:
                    Stream.Reconsume();
                    List.AddLast(Consume_AtRule(Stream));
                    break;
                case ECssTokenType.Ident:
                    {
                        List<CssToken> tmp = new List<CssToken>();
                        tmp.Add(Token);
                        // Collect all tokens until EOF or semicolon for this declaration
                        while (Stream.Next is CssToken next && next.Type != ECssTokenType.EOF && next.Type != ECssTokenType.Semicolon)
                        {
                            tmp.Add(Stream.Consume());
                        }
                        // Add EOF token so sub-stream has proper termination
                        tmp.Add(EOFToken.Instance);

                        var decl = Consume_Decleration(new DataConsumer<CssToken>(tmp.ToArray(), CssToken.EOF));
                        if (decl is not null)
                        {
                            List.AddLast(decl);
                        }
                    }
                    break;
                default:
                    {// parse error, consume tokens until reaching an EOF or semicolon
                        while (Stream.Next is CssToken nextToken && nextToken.Type != ECssTokenType.EOF && nextToken.Type != ECssTokenType.Semicolon)
                        {
                            Stream.Consume();
                        }
                    }
                    break;
            }
        }
        while (Token is not null && Token.Type != ECssTokenType.EOF);

        return List;
    }

    /// <summary>
    /// Consumes a declaration per CSS Syntax Level 3 §5.4.6.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This algorithm assumes that the next input token has already been checked to be an &lt;ident-token&gt;.
    /// </para>
    /// <para>
    /// For custom properties (names starting with "--"), the value is validated against
    /// the <c>&lt;declaration-value&gt;</c> production per CSS Syntax Level 3 §8.2.
    /// </para>
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-a-declaration"/>
    /// <seealso href="https://www.w3.org/TR/css-variables-1/#defining-variables"/>
    static CssDecleration? Consume_Decleration(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        // Consume the next input token. Create a new declaration with its name set to
        // the value of the current input token and its value initially set to an empty list.
        var nameToken = Stream.Consume();
        var name = (nameToken as ValuedTokenBase)?.Value;
        CssDecleration Decleration = new CssDecleration(name);

        // Step 1: While the next input token is a <whitespace-token>, consume the next input token.
        Consume_All_Whitespace(Stream);

        // Step 2: If the next input token is anything other than a <colon-token>, this is a parse error. Return nothing.
        // Otherwise, consume the next input token.
        if (Stream.Next.Type != ECssTokenType.Colon) return null;
        Stream.Consume(); // Consume the colon

        // Step 3: While the next input token is a <whitespace-token>, consume the next input token.
        Consume_All_Whitespace(Stream);

        // Step 4: As long as the next input token is anything other than an <EOF-token>,
        // consume a component value and append it to the declaration's value.
        while (Stream.Next.Type != ECssTokenType.EOF)
        {
            var componentValue = Consume_ComponentValue(Stream);
            Decleration.Values.Add(componentValue);
        }

        // Step 5: If the last two non-<whitespace-token>s in the declaration's value are a <delim-token>
        // with the value "!" followed by an <ident-token> with a value that is an ASCII case-insensitive
        // match for "important", remove them from the declaration's value and set the declaration's important flag to true.
        int indexA = -1, indexB = -1;
        CssToken? A = null, B = null;

        // Find the last two non-whitespace tokens and their indices
        for (int i = Decleration.Values.Count - 1; i >= 0; i--)
        {
            CssToken t = Decleration.Values[i];
            if (t.Type != ECssTokenType.Whitespace)
            {
                if (B is null)
                {
                    B = t;
                    indexB = i;
                }
                else
                {
                    A = t;
                    indexA = i;
                    break;
                }
            }
        }

        // Check if those last two values indicate this declaration's 'important' flag is set
        if (A?.Type == ECssTokenType.Delim && (A as DelimToken)?.Value == UnicodeCommon.CHAR_EXCLAMATION_POINT)
        {
            if (B?.Type == ECssTokenType.Ident && (B as IdentToken)?.Value.Equals("important", StringComparison.OrdinalIgnoreCase) == true)
            {
                Decleration.Important = true;
                // Remove the "!" and "important" tokens (remove higher index first to preserve lower index)
                if (indexB > indexA)
                {
                    Decleration.Values.RemoveAt(indexB);
                    Decleration.Values.RemoveAt(indexA);
                }
                else
                {
                    Decleration.Values.RemoveAt(indexA);
                    Decleration.Values.RemoveAt(indexB);
                }
            }
        }

        // Step 6: While the last token in the declaration's value is a <whitespace-token>, remove that token.
        while (Decleration.Values.Count > 0 && Decleration.Values[^1].Type == ECssTokenType.Whitespace)
        {
            Decleration.Values.RemoveAt(Decleration.Values.Count - 1);
        }

        // Step 7: For custom properties, validate the value against <declaration-value> production.
        // Per CSS Custom Properties Level 1: "The value of a custom property is everything after
        // the property name, up to the end of the declaration."
        // Per CSS Syntax Level 3 §8.2: custom property values must match <declaration-value>.
        if (Decleration.IsCustomProperty && Decleration.Values.Count > 0)
        {
            Decleration.ValueValidation = CssProductionMatcher.MatchDeclarationValue(Decleration.Values);
        }

        // Step 8: Return the declaration.
        return Decleration;
    }

    /// <summary>
    /// Consumes a simple block per CSS Syntax Level 3 §5.4.8.
    /// </summary>
    /// <remarks>
    /// This algorithm assumes that the current input token has already been
    /// checked to be a <c>&lt;{-token&gt;</c>, <c>&lt;[-token&gt;</c>, or <c>&lt;(-token&gt;</c>.
    /// The opening token should be passed via <paramref name="startToken"/>.
    /// </remarks>
    /// <param name="Stream">The token stream.</param>
    /// <param name="startToken">The already-consumed opening bracket token.</param>
    /// <returns>A simple block containing the consumed tokens.</returns>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-simple-block"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static CssSimpleBlock Consume_SimpleBlock(DataConsumer<CssToken> Stream, CssToken startToken)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        // Determine the ending token (mirror variant of start token)
        ECssTokenType endTokenType = startToken.Type switch
        {
            ECssTokenType.Bracket_Open => ECssTokenType.Bracket_Close,
            ECssTokenType.Parenth_Open => ECssTokenType.Parenth_Close,
            ECssTokenType.SqBracket_Open => ECssTokenType.SqBracket_Close,
            _ => throw new CssSyntaxErrorException(CssErrors.EXPECTING_SIMPLE_BLOCK_START, Stream)
        };

        CssSimpleBlock block = new CssSimpleBlock(startToken);

        // Repeatedly consume the next input token
        while (true)
        {
            CssToken token = Stream.Consume();

            if (token.Type == endTokenType)
            {
                // ending token: Return the block
                return block;
            }

            if (token.Type == ECssTokenType.EOF)
            {
                // EOF: This is a parse error. Return the block.
                return block;
            }

            // anything else: Reconsume the current input token.
            // Consume a component value and append it to the value of the block.
            Stream.Reconsume();
            block.Values.Add(Consume_ComponentValue(Stream));
        }
    }

    /// <summary>
    /// Consumes a function per CSS Syntax Level 3 §5.4.9.
    /// </summary>
    /// <remarks>
    /// This algorithm assumes that the current input token has already been
    /// checked to be a <c>&lt;function-token&gt;</c>.
    /// The function token should be passed via <paramref name="functionToken"/>.
    /// </remarks>
    /// <param name="Stream">The token stream.</param>
    /// <param name="functionToken">The already-consumed function-name token.</param>
    /// <returns>A function containing the consumed tokens.</returns>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-function"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static CssFunction Consume_Function(DataConsumer<CssToken> Stream, FunctionNameToken functionToken)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        // Create a function with its name equal to the value of the current input token
        CssFunction func = new CssFunction(functionToken.Value);

        // Repeatedly consume the next input token
        while (true)
        {
            CssToken token = Stream.Consume();

            switch (token.Type)
            {
                case ECssTokenType.Parenth_Close:
                    // <)-token>: Return the function
                    return func;

                case ECssTokenType.EOF:
                    // EOF: This is a parse error. Return the function.
                    return func;

                default:
                    // anything else: Reconsume the current input token.
                    // Consume a component value and append the returned value to the function's value.
                    Stream.Reconsume();
                    func.Arguments.Add(Consume_ComponentValue(Stream));
                    break;
            }
        }
    }

    /// <summary>
    /// Consumes a component value per CSS Syntax Level 3 §5.4.7.
    /// </summary>
    /// <remarks>
    /// Attempts to consume all tokens within a matching pair of <c>()</c>, <c>{}</c>, or <c>[]</c> brackets,
    /// or a function, and otherwise just returns the next token.
    /// </remarks>
    /// <param name="Stream">The token stream.</param>
    /// <returns>The consumed component value (token, simple block, or function).</returns>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-component-value"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static CssToken Consume_ComponentValue(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        // Consume the next input token
        CssToken token = Stream.Consume();

        switch (token.Type)
        {
            // If the current input token is a <{-token>, <[-token>, or <(-token>,
            // consume a simple block and return it.
            case ECssTokenType.Bracket_Open:
            case ECssTokenType.SqBracket_Open:
            case ECssTokenType.Parenth_Open:
                return Consume_SimpleBlock(Stream, token);

            // Otherwise, if the current input token is a <function-token>,
            // consume a function and return it.
            case ECssTokenType.FunctionName:
                return Consume_Function(Stream, (FunctionNameToken)token);

            // Otherwise, return the current input token.
            default:
                return token;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
    static IEnumerable<IEnumerable<CssToken>> Consume_Comma_Seperated_Component_Value_List(DataConsumer<CssToken> Stream)
    {/* Docs: https://drafts.csswg.org/css-syntax-3/#parse-a-comma-separated-list-of-component-values */
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        var cvls = new LinkedList<LinkedList<CssToken>>();
        var node = cvls.AddLast(new LinkedList<CssToken>());

        do
        {
            switch (Stream.Next.Type)
            {
                case ECssTokenType.EOF:
                    {
                        // Don't consume EOF, just add final node and exit
                        cvls.AddLast(node);
                        return cvls;
                    }
                case ECssTokenType.Comma:
                    {
                        Stream.Consume(); // Consume the comma
                        cvls.AddLast(node);
                        node = cvls.AddLast(new LinkedList<CssToken>());
                    }
                    break;
                default:
                    {
                        node.Value.AddLast(Consume_ComponentValue(Stream));
                    }
                    break;
            }
        }
        while (Stream.Next != CssToken.EOF);

        return cvls;
    }
    #endregion

    #region CSS Values
    public static CssValue Consume_CssValue(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        Contract.EndContractBlock();


        CssToken Token = Stream.Next;
        switch (Token.Type)
        {
            case ECssTokenType.Dimension:
                {
                    var tok = Stream.Consume() as DimensionToken;
                    ECssUnit unit = ECssUnit.PX;

                    if (!string.IsNullOrEmpty(tok.Unit))
                    {
                        ECssUnit unitLookup = ECssUnitExtensions.FromKeyword(tok.Unit);
                        unit = unitLookup;
                    }

                    // Use resolution subclass for resolution units, dimension for others
                    return unit switch
                    {
                        ECssUnit.DPI or ECssUnit.DPCM or ECssUnit.DPPX => new CssResolutionValue(tok.AsNumber, unit),
                        _ => new CssDimensionValue(tok.AsNumber, unit)
                    };
                }
            case ECssTokenType.Number:
                {
                    var tok = Stream.Consume() as NumberToken;
                    return new CssNumberValue(tok.AsNumber);
                }
            case ECssTokenType.Percentage:
                {
                    var tok = Stream.Consume() as PercentageToken;
                    return new CssPercentValue(tok.Number);
                }
            case ECssTokenType.String:
                {
                    var tok = Stream.Consume() as StringToken;
                    return new CssStringValue(tok!.Value);
                }
            case ECssTokenType.Ident:// Keyword
                {
                    var tok = Stream.Consume() as IdentToken;

                    // Check if this is a named color keyword (e.g., "red", "blue", "transparent")
                    // Per CSS Color Level 4 spec: https://www.w3.org/TR/css-color-4/#named-colors
                    if (CssColor.TryFromNamedColor(tok!.Value.AsSpan(), out CssColor namedColor, out bool isCurrentColor))
                    {
                        // Handle 'currentColor' keyword specially - it's a valid color but needs context resolution
                        if (isCurrentColor)
                        {
                            // Return as a keyword for now; it will be resolved during cascade/inheritance
                            return new CssKeywordValue(tok.Value);
                        }

                        return new CssColorValue(namedColor);
                    }

                    return new CssKeywordValue(tok!.Value);
                }
            case ECssTokenType.FunctionName:
                {
                    var funcToken = Stream.Consume() as FunctionNameToken;
                    CssFunction func = Consume_Function(Stream, funcToken!);

                    // Check if this is a color function (rgb, rgba, hsl, hsla, etc.)
                    if (CssColorFunctionParser.TryParseColorFunction(func, out CssColor color))
                    {
                        return CssValue.From(color);
                    }

                    // Check if this is a url() function with quoted string
                    if (CssUrlFunctionParser.TryParseUrlFunction(func, out CssUrl url))
                    {
                        return CssValue.From(url);
                    }

                    // Check if this is a calc() function
                    if (CssCalcFunctionParser.TryParseCalcFunction(func, out CssCalcExpression? calcExpr))
                    {
                        return CssValue.From(calcExpr!);
                    }

                    // Check if this is a min(), max(), or clamp() function
                    if (CssCalcFunctionParser.TryParseComparisonFunction(func, out CssCalcExpression? compExpr))
                    {
                        return CssValue.From(compExpr!);
                    }

                    // Check if this is a var() function (CSS custom property reference)
                    if (CssVarFunctionParser.TryParseVarFunction(func, out CssVarFunction varFunc))
                    {
                        return CssValue.From(varFunc);
                    }

                    // Check if this is an env() function (CSS environment variable reference)
                    if (CssEnvFunctionParser.TryParseEnvFunction(func, out CssEnvFunction envFunc))
                    {
                        return CssValue.From(envFunc);
                    }

                    return CssValue.From(func);
                }
            case ECssTokenType.Function:
                {
                    var func = Stream.Consume() as CssFunction;

                    // Check if this is a color function (rgb, rgba, hsl, hsla, etc.)
                    if (CssColorFunctionParser.TryParseColorFunction(func!, out CssColor color))
                    {
                        return CssValue.From(color);
                    }

                    // Check if this is a url() function with quoted string
                    if (CssUrlFunctionParser.TryParseUrlFunction(func!, out CssUrl url))
                    {
                        return CssValue.From(url);
                    }

                    // Check if this is a calc() function
                    if (CssCalcFunctionParser.TryParseCalcFunction(func!, out CssCalcExpression? calcExpr2))
                    {
                        return CssValue.From(calcExpr2!);
                    }

                    // Check if this is a min(), max(), or clamp() function
                    if (CssCalcFunctionParser.TryParseComparisonFunction(func!, out CssCalcExpression? compExpr2))
                    {
                        return CssValue.From(compExpr2!);
                    }

                    // Check if this is a var() function (CSS custom property reference)
                    if (CssVarFunctionParser.TryParseVarFunction(func!, out CssVarFunction varFunc2))
                    {
                        return CssValue.From(varFunc2);
                    }

                    // Check if this is an env() function (CSS environment variable reference)
                    if (CssEnvFunctionParser.TryParseEnvFunction(func!, out CssEnvFunction envFunc2))
                    {
                        return CssValue.From(envFunc2);
                    }

                    return CssValue.From(func!);
                }
            case ECssTokenType.Url:
                {
                    // Handle unquoted URL token: url(path)
                    // Per CSS Syntax Level 3, <url-token> contains the URL value directly
                    if (Stream.Consume() is not UrlToken tok)
                    {
                        throw new CssParserException("Expected UrlToken but received null", Stream);
                    }
                    return CssValue.From(new CssUrl(tok.Value ?? string.Empty));
                }
            case ECssTokenType.Bad_Url:
                {
                    // Bad URL tokens are parse errors per CSS Syntax Level 3
                    Stream.Consume(); // Consume the bad token
                    throw new CssParserException("Invalid URL syntax (bad-url-token)", Stream);
                }
            case ECssTokenType.Hash:
                {
                    // Parse hex color values: #RGB, #RGBA, #RRGGBB, #RRGGBBAA
                    if (Stream.Consume() is not HashToken tok)
                    {
                        throw new CssParserException("Expected HashToken but received null", Stream);
                    }

                    // Attempt to parse the hash value as a color
                    // The HashToken.Value does NOT include the leading '#'
                    if (CssHexColorParser.TryParse(tok.Value, out CssColor color))
                    {
                        return CssValue.From(color);
                    }

                    // If not a valid hex color, treat as a parse error
                    throw new CssParserException($"Invalid hex color value: #{tok.Value}", Stream);
                }
            case ECssTokenType.EOF:
                {
                    return CssValue.Null;
                }
            default:
                {
                    throw new CssParserException(String.Format(CultureInfo.InvariantCulture, CssErrors.UNHANDLED_TOKEN_FOR_CSS_VALUE, Token.Type), Stream);
                }
        }

    }
    #endregion

    #region Media
    public MediaQueryList Parse_Media_Query_List(Document document)
    {/* Docs: https://www.w3.org/TR/mediaqueries-4/#mq-syntax */
        Consume_All_Whitespace(Stream);

        /*
         * To parse a <media-query-list> production,
         * parse a comma-separated list of component values,
         * then parse each entry in the returned list as a <media-query>.
         * Its value is the list of <media-query>s so produced.
         */
        var cvls = Consume_Comma_Seperated_Component_Value_List(Stream);
        LinkedList<MediaQuery> queryList = new LinkedList<MediaQuery>();
        foreach (LinkedList<CssToken> tokenList in cvls)
        {
            DataConsumer<CssToken> tokenStream = new DataConsumer<CssToken>(tokenList.ToArray());
            var query = Consume_MediaQuery(tokenStream);
            queryList.AddLast(query);
        }

        return new MediaQueryList(document, queryList);
    }

    /// <summary>
    /// Consumes a media query per Media Queries Level 4 §3 Syntax.
    /// </summary>
    /// <remarks>
    /// Grammar:
    /// <code>
    /// &lt;media-query&gt; = &lt;media-condition&gt;
    ///              | [ not | only ]? &lt;media-type&gt; [ and &lt;media-condition-without-or&gt; ]?
    /// </code>
    /// </remarks>
    /// <seealso href="https://drafts.csswg.org/mediaqueries-4/#mq-syntax"/>
    static MediaQuery Consume_MediaQuery(DataConsumer<CssToken>? Stream = null)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        Consume_All_Whitespace(Stream);

        // Handle empty media query (evaluates to true per spec)
        if (Stream.Next.Type == ECssTokenType.EOF)
        {
            return new MediaQuery(EMediaQueryModifier.None, EMediaType.All, new LinkedList<IMediaCondition>());
        }

        // Determine which branch of the grammar we're in:
        // Branch 1: <media-condition> - starts with '(' or 'not ('
        // Branch 2: [ not | only ]? <media-type> [ and <media-condition-without-or> ]?

        bool isConditionOnlyBranch = false;

        // Check if this is a standalone <media-condition>
        // A <media-condition> starts with:
        // - '(' for <media-in-parens> (appears as SimpleBlock after component value parsing)
        // - 'not' followed by '(' for <media-not>
        // Note: After Consume_Comma_Seperated_Component_Value_List, parenthesized content
        // becomes SimpleBlock tokens, not raw Parenth_Open tokens.
        if (Stream.Next.Type == ECssTokenType.SimpleBlock)
        {
            // Starts with '(...)' block - this is the <media-condition> branch
            var block = (CssSimpleBlock)Stream.Next;
            if (block.StartToken.Type == ECssTokenType.Parenth_Open)
            {
                isConditionOnlyBranch = true;
            }
        }
        else if (Stream.Next.Type == ECssTokenType.Ident)
        {
            var identValue = ((IdentToken)Stream.Next).Value;

            // Check if it's 'not' followed by '(...)' (media-not branch of media-condition)
            // vs 'not' followed by media-type (modifier branch)
            if (identValue.Equals("not", StringComparison.OrdinalIgnoreCase))
            {
                // Look ahead: 'not' + '(...)' means <media-not> (condition branch)
                // 'not' + <ident> means modifier + media-type
                int peekIdx = 1;

                // Skip whitespace in lookahead
                while (Stream.Peek(peekIdx).Type == ECssTokenType.Whitespace)
                    peekIdx++;

                var nextToken = Stream.Peek(peekIdx);
                if (nextToken.Type == ECssTokenType.SimpleBlock)
                {
                    var block = (CssSimpleBlock)nextToken;
                    if (block.StartToken.Type == ECssTokenType.Parenth_Open)
                    {
                        // 'not' followed by '(...)' - this is <media-not> in <media-condition>
                        isConditionOnlyBranch = true;
                    }
                }
                // Otherwise it's 'not <media-type>' (modifier branch)
            }
            // 'only' is always a modifier, never starts a condition
        }

        if (isConditionOnlyBranch)
        {
            // Branch 1: <media-condition>
            // Parse as a standalone condition with implicit 'all' media type
            var condition = Consume_Media_Condition(Stream, allowOr: true);
            var conditionList = new LinkedList<IMediaCondition>();
            if (condition is not null)
            {
                conditionList.AddLast(condition);
            }

            return new MediaQuery(EMediaQueryModifier.None, EMediaType.All, conditionList);
        }

        // Branch 2: [ not | only ]? <media-type> [ and <media-condition-without-or> ]?
        EMediaQueryModifier modifier = EMediaQueryModifier.None;
        EMediaType mediaType = EMediaType.All;
        var conditions = new LinkedList<IMediaCondition>();

        // Check for optional modifier (not | only)
        if (Stream.Next.Type == ECssTokenType.Ident)
        {
            if (EMediaQueryModifierExtensions.TryFromKeyword(((IdentToken)Stream.Next).Value, out EMediaQueryModifier? mod))
            {
                Stream.Consume();
                modifier = mod.Value;
                Consume_All_Whitespace(Stream);
            }
        }

        // Consume the media type (required in this branch after optional modifier)
        if (Stream.Next.Type == ECssTokenType.Ident)
        {
            var typeIdent = (IdentToken)Stream.Consume();
            if (!EMediaTypeExtensions.TryFromKeyword(typeIdent.Value, out EMediaType? type))
            {
                // Unknown media type - per spec, unknown media types don't match
                // but we still parse them. Use NONE to indicate unknown.
                mediaType = EMediaType.NONE;
            }
            else
            {
                mediaType = type.Value;
            }
        }
        else if (modifier != EMediaQueryModifier.None)
        {
            // Had a modifier but no media type following - syntax error
            throw new CssSyntaxErrorException(CssErrors.EXPECTING_IDENT, Stream);
        }

        Consume_All_Whitespace(Stream);

        // Check for optional 'and <media-condition-without-or>'
        if (Stream.Next.Type == ECssTokenType.Ident)
        {
            var nextIdent = (IdentToken)Stream.Next;
            if (nextIdent.Value.Equals("and", StringComparison.OrdinalIgnoreCase))
            {
                Stream.Consume(); // consume 'and'
                Consume_All_Whitespace(Stream);

                // Now consume <media-condition-without-or>
                // This is: <media-not> | <media-in-parens> <media-and>*
                // (no 'or' allowed at the top level)
                while (Stream.Next.Type != ECssTokenType.EOF)
                {
                    Consume_All_Whitespace(Stream);

                    if (Stream.Next.Type != ECssTokenType.Parenth_Open &&
                        !(Stream.Next.Type == ECssTokenType.Ident &&
                          ((IdentToken)Stream.Next).Value.Equals("not", StringComparison.OrdinalIgnoreCase)))
                    {
                        break;
                    }

                    var condition = Consume_Media_Condition(Stream, allowOr: false);
                    if (condition is not null)
                    {
                        conditions.AddLast(condition);
                    }

                    Consume_All_Whitespace(Stream);

                    // Check for 'and' to continue
                    if (Stream.Next.Type == ECssTokenType.Ident)
                    {
                        var combIdent = (IdentToken)Stream.Next;
                        if (combIdent.Value.Equals("and", StringComparison.OrdinalIgnoreCase))
                        {
                            Stream.Consume();
                            Consume_All_Whitespace(Stream);
                        }
                        else if (combIdent.Value.Equals("or", StringComparison.OrdinalIgnoreCase))
                        {
                            // 'or' not allowed in <media-condition-without-or>
                            throw new CssSyntaxErrorException(CssErrors.INVALID_MULTIPLE_COMBINATORS_ON_MEDIARULE, Stream);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MediaQuery(modifier, mediaType, conditions);
    }

    /// <summary>
    /// Consumes a media condition per Media Queries Level 4 §3 Syntax.
    /// </summary>
    /// <remarks>
    /// Grammar:
    /// <code>
    /// &lt;media-condition&gt; = &lt;media-not&gt; | &lt;media-in-parens&gt; [ &lt;media-and&gt;* | &lt;media-or&gt;* ]
    /// &lt;media-condition-without-or&gt; = &lt;media-not&gt; | &lt;media-in-parens&gt; &lt;media-and&gt;*
    /// &lt;media-not&gt; = not &lt;media-in-parens&gt;
    /// &lt;media-and&gt; = and &lt;media-in-parens&gt;
    /// &lt;media-or&gt; = or &lt;media-in-parens&gt;
    /// &lt;media-in-parens&gt; = ( &lt;media-condition&gt; ) | ( &lt;media-feature&gt; ) | &lt;general-enclosed&gt;
    /// </code>
    /// </remarks>
    /// <param name="Stream">The token stream.</param>
    /// <param name="allowOr">If false, enforces &lt;media-condition-without-or&gt; (no 'or' allowed).</param>
    /// <returns>The parsed media condition, or null if parsing fails.</returns>
    /// <seealso href="https://www.w3.org/TR/mediaqueries-4/#media-condition"/>
    static IMediaCondition? Consume_Media_Condition(DataConsumer<CssToken> Stream, bool allowOr = true)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        Consume_All_Whitespace(Stream);

        // Handle EOF/null token
        if (Stream.Next == null || Stream.Next == CssToken.EOF || Stream.Next.Type == ECssTokenType.EOF)
        {
            return null;
        }

        // Check for <media-not>: 'not' <media-in-parens>
        if (Stream.Next.Type == ECssTokenType.Ident)
        {
            var identValue = ((IdentToken)Stream.Next).Value;
            if (identValue.Equals("not", StringComparison.OrdinalIgnoreCase))
            {
                Stream.Consume(); // consume 'not'
                Consume_All_Whitespace(Stream);

                // Must be followed by <media-in-parens>
                var innerCondition = Consume_Media_In_Parens(Stream, allowOr);
                if (innerCondition is null)
                {
                    throw new CssSyntaxErrorException(CssErrors.EXPECTING_OPENING_PARENTHESES, Stream);
                }

                // Wrap in a MediaCondition with NOT combinator
                var notConditionList = new LinkedList<IMediaCondition>();
                notConditionList.AddLast(innerCondition);
                return new MediaCondition(EMediaCombinator.NOT, notConditionList);
            }
        }

        // Otherwise: <media-in-parens> [ <media-and>* | <media-or>* ]
        var firstCondition = Consume_Media_In_Parens(Stream, allowOr);
        if (firstCondition is null)
        {
            return null;
        }

        Consume_All_Whitespace(Stream);

        // Check for chained conditions (and/or)
        EMediaCombinator combinator = EMediaCombinator.None;
        var conditionList = new LinkedList<IMediaCondition>();
        conditionList.AddLast(firstCondition);

        while (Stream.Next != null && Stream.Next != CssToken.EOF && Stream.Next.Type == ECssTokenType.Ident)
        {
            var combIdent = (IdentToken)Stream.Next;
            var combValue = combIdent.Value;

            EMediaCombinator currentCombinator;
            if (combValue.Equals("and", StringComparison.OrdinalIgnoreCase))
            {
                currentCombinator = EMediaCombinator.AND;
            }
            else if (combValue.Equals("or", StringComparison.OrdinalIgnoreCase))
            {
                if (!allowOr)
                {
                    // 'or' not allowed in <media-condition-without-or>
                    throw new CssSyntaxErrorException(CssErrors.INVALID_MULTIPLE_COMBINATORS_ON_MEDIARULE, Stream);
                }
                currentCombinator = EMediaCombinator.OR;
            }
            else
            {
                // Not a combinator, stop here
                break;
            }

            // Validate combinator consistency (can't mix 'and' and 'or' at same level)
            if (combinator == EMediaCombinator.None)
            {
                combinator = currentCombinator;
            }
            else if (combinator != currentCombinator)
            {
                throw new CssSyntaxErrorException(CssErrors.INVALID_MULTIPLE_COMBINATORS_ON_MEDIARULE, Stream);
            }

            Stream.Consume(); // consume combinator
            Consume_All_Whitespace(Stream);

            var nextCondition = Consume_Media_In_Parens(Stream, allowOr);
            if (nextCondition is null)
            {
                throw new CssSyntaxErrorException(CssErrors.EXPECTING_OPENING_PARENTHESES, Stream);
            }

            conditionList.AddLast(nextCondition);
            Consume_All_Whitespace(Stream);
        }

        // If only one condition and no combinator, return it directly
        if (conditionList.Count == 1 && combinator == EMediaCombinator.None)
        {
            return firstCondition;
        }

        return new MediaCondition(combinator, conditionList);
    }

    /// <summary>
    /// Consumes a &lt;media-in-parens&gt; production.
    /// </summary>
    /// <remarks>
    /// Grammar: &lt;media-in-parens&gt; = ( &lt;media-condition&gt; ) | ( &lt;media-feature&gt; ) | &lt;general-enclosed&gt;
    /// Note: After component value parsing, parenthesized content appears as SimpleBlock tokens.
    /// </remarks>
    static IMediaCondition? Consume_Media_In_Parens(DataConsumer<CssToken> Stream, bool allowOr)
    {
        Consume_All_Whitespace(Stream);

        // After Consume_Comma_Seperated_Component_Value_List, parenthesized content
        // becomes SimpleBlock tokens, not raw Parenth_Open tokens.
        if (Stream.Next.Type != ECssTokenType.SimpleBlock)
        {
            return null;
        }

        var block = (CssSimpleBlock)Stream.Next;
        if (block.StartToken.Type != ECssTokenType.Parenth_Open)
        {
            return null;
        }

        // Consume the SimpleBlock
        Stream.Consume();

        // Create a stream from the block's contents
        var innerStream = new DataConsumer<CssToken>(block.Values.ToArray(), CssToken.EOF);

        Consume_All_Whitespace(innerStream);

        // Check if it's a <media-feature> by looking at the contents
        // Media features are: <mf-plain> | <mf-boolean> | <mf-range>
        // They have an ident (feature name) possibly followed by ':' and value, or comparison operators
        if (Starts_Media_Feature_In_Block(innerStream))
        {
            var feature = Consume_Media_Feature(innerStream);
            return feature;
        }

        // Otherwise, it's a nested <media-condition>
        var nestedCondition = Consume_Media_Condition(innerStream, allowOr);
        return nestedCondition;
    }

    /// <summary>
    /// Checks if the stream appears to start a media feature (vs a nested condition).
    /// Media features start with an ident that's a known feature name,
    /// or contain comparison operators like '&lt;', '&gt;', '&lt;=', '&gt;=', '='.
    /// </summary>
    static bool Starts_Media_Feature_In_Block(DataConsumer<CssToken> Stream)
    {
        // Skip whitespace
        int idx = 0;
        while (Stream.Peek(idx) != null && Stream.Peek(idx) != CssToken.EOF && Stream.Peek(idx).Type == ECssTokenType.Whitespace)
            idx++;

        var firstToken = Stream.Peek(idx);

        // Handle empty block or EOF
        if (firstToken == null || firstToken == CssToken.EOF || firstToken.Type == ECssTokenType.EOF)
        {
            return false;
        }

        // If it starts with 'not' followed by '(' or SimpleBlock, it's a condition
        if (firstToken.Type == ECssTokenType.Ident)
        {
            var identValue = ((IdentToken)firstToken).Value;
            if (identValue.Equals("not", StringComparison.OrdinalIgnoreCase))
            {
                // Check next non-whitespace token
                idx++;
                while (Stream.Peek(idx).Type == ECssTokenType.Whitespace)
                    idx++;

                var nextToken = Stream.Peek(idx);
                if (nextToken.Type == ECssTokenType.SimpleBlock || nextToken.Type == ECssTokenType.Parenth_Open)
                {
                    return false; // It's a nested condition starting with 'not'
                }
            }

            // If it's an ident, check what follows
            idx++;
            while (Stream.Peek(idx).Type == ECssTokenType.Whitespace)
                idx++;

            var afterIdent = Stream.Peek(idx);

            // If followed by ':' it's a plain feature (name: value)
            if (afterIdent.Type == ECssTokenType.Colon)
            {
                return true;
            }

            // If followed by comparison delims it's a range feature
            if (afterIdent.Type == ECssTokenType.Delim)
            {
                var delimChar = ((DelimToken)afterIdent).Value;
                if (delimChar == '<' || delimChar == '>' || delimChar == '=')
                {
                    return true;
                }
            }

            // If followed by EOF or ')' it's a boolean feature (just the name)
            if (afterIdent.Type == ECssTokenType.EOF || afterIdent.Type == ECssTokenType.Parenth_Close)
            {
                return true;
            }

            // If followed by 'and' or 'or' it might be a boolean feature followed by more conditions
            // but at the block level, we only have one feature, so check if it's a known feature name
            // For simplicity, treat lone idents as boolean features
            if (afterIdent.Type == ECssTokenType.Ident)
            {
                var nextIdent = ((IdentToken)afterIdent).Value;
                if (nextIdent.Equals("and", StringComparison.OrdinalIgnoreCase) ||
                    nextIdent.Equals("or", StringComparison.OrdinalIgnoreCase))
                {
                    // Boolean feature followed by combinator - this is a feature
                    return true;
                }
            }

            // Default: treat as feature if starts with ident (could be boolean feature)
            return true;
        }

        // If it starts with '(' or SimpleBlock, it's a nested condition
        if (firstToken.Type == ECssTokenType.Parenth_Open || firstToken.Type == ECssTokenType.SimpleBlock)
        {
            return false;
        }

        // If it starts with a number/dimension (for range syntax like "320px < width")
        if (firstToken.Type == ECssTokenType.Number || firstToken.Type == ECssTokenType.Dimension)
        {
            return true; // Range feature with value first
        }

        return false;
    }

    static IMediaCondition? Consume_Media_Feature(DataConsumer<CssToken> Stream)
    {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);

        Consume_All_Whitespace(Stream);

        // Media feature syntax:
        // <mf-boolean> = <mf-name>
        // <mf-plain> = <mf-name> : <mf-value>
        // <mf-range> = <mf-name> <mf-comparison> <mf-value>
        //            | <mf-value> <mf-comparison> <mf-name>
        //            | <mf-value> <mf-comparison> <mf-name> <mf-comparison> <mf-value>

        // Collect all tokens that are part of the feature
        var values = new LinkedList<CssValue>();
        var ops = new LinkedList<EMediaOperator>();
        bool expectValue = true;
        EMediaFeatureName? featureName = null;

        while (Stream.Next.Type != ECssTokenType.EOF && Stream.Next.Type != ECssTokenType.Parenth_Close)
        {
            Consume_All_Whitespace(Stream);

            if (Stream.Next.Type == ECssTokenType.EOF)
                break;

            // Check for colon (plain feature syntax: name: value)
            if (Stream.Next.Type == ECssTokenType.Colon)
            {
                Stream.Consume(); // consume ':'
                Consume_All_Whitespace(Stream);

                // What follows is the value
                var value = Consume_MediaFeature_Value(Stream);

                // This is a plain feature: (name: value)
                if (featureName.HasValue)
                {
                    return new MediaFeature(
                        new CssValue[] { CssValue.From(featureName.Value), value },
                        new EMediaOperator[] { EMediaOperator.EqualTo });
                }
                continue;
            }

            // Check for comparator
            if (ParserCommon.Is_Comparator(Stream.Next))
            {
                string comparatorStr;
                var token = Stream.Consume();

                if (token is DelimToken delimTok)
                {
                    // Single-char operator: <, >, =
                    // Check for compound operators like <=, >=
                    Consume_All_Whitespace(Stream);
                    if (Stream.Next is DelimToken nextDelim && nextDelim.Value == '=')
                    {
                        Stream.Consume();
                        comparatorStr = delimTok.Value.ToString() + "=";
                    }
                    else
                    {
                        comparatorStr = delimTok.Value.ToString();
                    }
                }
                else if (token is IdentToken identTok)
                {
                    // Multi-char operator like <=, >= stored as ident (shouldn't happen with correct tokenizer)
                    comparatorStr = identTok.Value;
                }
                else
                {
                    throw new CssParserException(CssErrors.EXPECTING_COMPARATOR, Stream);
                }

                if (!EMediaOperatorExtensions.TryFromKeyword(comparatorStr, out EMediaOperator? outComparator))
                {
                    throw new CssParserException(CssErrors.EXPECTING_COMPARATOR, Stream);
                }
                ops.AddLast(outComparator.Value);
                expectValue = true;
                continue;
            }

            // Check for ident (could be feature name)
            if (Stream.Next.Type == ECssTokenType.Ident)
            {
                var identTok = (IdentToken)Stream.Consume();

                // Try to resolve as media feature name
                if (EMediaFeatureNameExtensions.TryFromKeyword(identTok.Value, out EMediaFeatureName? name))
                {
                    featureName = name.Value;
                    values.AddLast(CssValue.From(name.Value));
                    expectValue = false;
                }
                else
                {
                    // Unknown feature name - per spec, unknown features should be
                    // parsed but evaluate to "unknown" state. We use Unknown enum value.
                    featureName = EMediaFeatureName.Unknown;
                    // Store the Unknown enum value (the raw string is lost)
                    values.AddLast(CssValue.From(EMediaFeatureName.Unknown));
                    expectValue = false;
                }
                continue;
            }

            // Otherwise it's a value (number, dimension, ratio)
            if (Stream.Next.Type == ECssTokenType.Number ||
                Stream.Next.Type == ECssTokenType.Dimension)
            {
                var value = Consume_MediaFeature_Value(Stream);
                values.AddLast(value);
                expectValue = false;
                continue;
            }

            // Unknown token - stop parsing
            break;
        }

        // Determine the type of feature
        if (values.Count == 0)
        {
            return null;
        }

        if (values.Count == 1 && ops.Count == 0 && featureName.HasValue)
        {
            // Boolean feature: just (name)
            return new MediaFeature(featureName.Value);
        }

        // Range or plain feature
        return new MediaFeature(values.ToArray(), ops.ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static CssValue Consume_MediaFeature_Value(DataConsumer<CssToken> Stream)
    {
        if (Stream is null) throw new CssParserException(CssErrors.STREAM_IS_NULL);
        /* Consume: <number> | <dimension> | <ident> | <ratio> */
        Consume_All_Whitespace(Stream);

        switch (Stream.Next.Type)
        {
            case ECssTokenType.Number:
                {
                    var numTok = Stream.Consume() as NumberToken;
                    /* This could be a ratio - so check if it is */
                    /* A ratio is a <number> <?whitespace> / <?whitespace> <number> */
                    if (ParserCommon.Starts_Ratio_Value(Stream.AsSpan()))
                    {
                        Consume_All_Whitespace(Stream);
                        DelimToken? dtok = Stream.Consume() as DelimToken;
                        Consume_All_Whitespace(Stream);
                        NumberToken? numTok2 = Stream.Consume() as NumberToken;

                        double ratioValue = (numTok.AsNumber / numTok2.AsNumber);
                        return new CssNumberValue(ratioValue); // Ratio stored as number
                    }

                    /* Nope, its just a number */
                    return new CssNumberValue(numTok.AsNumber);
                }
            case ECssTokenType.Dimension:
            case ECssTokenType.Ident:
                {
                    return Consume_CssValue(Stream);
                }
        }
        // throw new CssSyntaxErrorException($"Expected Number/Dimension/Keyword token but got: \"{Enum.GetName(typeof(ECssTokenType), Stream.Next.Type)}\"");
        throw new CssSyntaxErrorException(CssErrors.UNEXPECTED_TOKEN, Stream);
    }


    #endregion
}

