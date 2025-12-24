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
/// Parses a stream of <see cref="CssToken"/>s and returns
/// </summary>
public class CssParser
{
    #region Properties
    private readonly DataConsumer<CssToken> Stream;
    private bool TopLevel = false;
    #endregion

    #region Constructors
    public CssParser(ReadOnlySpan<char> Text)
    {
        CssTokenizer Tokenizer = new CssTokenizer(Text);
        Stream = new DataConsumer<CssToken>(Tokenizer.Tokens, CssToken.EOF);
    }

    public CssParser(CssToken[] Tokens)
    {
        Stream = new DataConsumer<CssToken>(Tokens, CssToken.EOF);
    }
    #endregion

    #region Parsing
    /// <summary>
    /// Parses and returns a list of rules
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CssComponent> Parse_Rule_List()
    {
        TopLevel = false;
        return Consume_Rule_List(Stream, TopLevel);
    }

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
    /// This algorithm assumes that the next input token has already been checked to be an &lt;ident-token&gt;.
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/css-syntax-3/#consume-a-declaration"/>
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

        // Step 7: Return the declaration.
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
                        ECssUnit unitLookup = Lookup.Enum<ECssUnit>(tok.Unit);
                        unit = unitLookup;
                    }

                    return new CssValue(ECssValueTypes.DIMENSION, tok.Number, unit);
                }
            case ECssTokenType.Number:
                {
                    var tok = Stream.Consume() as NumberToken;
                    return new CssValue(ECssValueTypes.NUMBER, tok.Number);
                }
            case ECssTokenType.Percentage:
                {
                    var tok = Stream.Consume() as PercentageToken;
                    return new CssValue(ECssValueTypes.PERCENT, tok.Number);
                }
            case ECssTokenType.String:
                {
                    var tok = Stream.Consume() as StringToken;
                    return new CssValue(ECssValueTypes.STRING, tok!.Value);
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
                            return new CssValue(ECssValueTypes.KEYWORD, tok.Value);
                        }

                        return CssValue.From(namedColor);
                    }

                    return new CssValue(ECssValueTypes.KEYWORD, tok!.Value);
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

                    return new CssValue(func);
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

                    return new CssValue(func!);
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
                    if (CssColor.TryFromHex(tok.Value, out CssColor color))
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
            if (Lookup.TryEnum(((IdentToken)Stream.Next).Value, out EMediaQueryModifier mod))
            {
                Stream.Consume();
                modifier = mod;
                Consume_All_Whitespace(Stream);
            }
        }

        // Consume the media type (required in this branch after optional modifier)
        if (Stream.Next.Type == ECssTokenType.Ident)
        {
            var typeIdent = (IdentToken)Stream.Consume();
            if (!Lookup.TryEnum(typeIdent.Value, out EMediaType type))
            {
                // Unknown media type - per spec, unknown media types don't match
                // but we still parse them. Use NONE to indicate unknown.
                mediaType = EMediaType.NONE;
            }
            else
            {
                mediaType = type;
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

                if (!Lookup.TryEnum(comparatorStr, out EMediaOperator outComparator))
                {
                    throw new CssParserException(CssErrors.EXPECTING_COMPARATOR, Stream);
                }
                ops.AddLast(outComparator);
                expectValue = true;
                continue;
            }

            // Check for ident (could be feature name)
            if (Stream.Next.Type == ECssTokenType.Ident)
            {
                var identTok = (IdentToken)Stream.Consume();

                // Try to resolve as media feature name
                if (Lookup.TryEnum(identTok.Value, out EMediaFeatureName name))
                {
                    featureName = name;
                    values.AddLast(CssValue.From(name));
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

                        double ratioValue = ((double)numTok.Number / (double)numTok2.Number);
                        return new CssValue(ECssValueTypes.RATIO, ratioValue);
                    }

                    /* Nope, its just a number */
                    return new CssValue(ECssValueTypes.NUMBER, numTok.Number);
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

