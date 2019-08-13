using CssUI.CSS.Media;
using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Serialization
{/* Docs: https://www.w3.org/TR/css-syntax-3/ */

    /// <summary>
    /// Parses a stream of <see cref="CssToken"/>s and returns 
    /// </summary>
    public class CssParser
    {
        #region Properties
        private readonly TokenStream Stream;
        private bool TopLevel = false;
        #endregion

        #region Constructors
        public CssParser(string Text)
        {
            CssTokenizer Tokenizer = new CssTokenizer(Text);
            Stream = new TokenStream(Tokenizer.Tokens);
        }

        public CssParser(CssToken[] Tokens)
        {
            Stream = new TokenStream(Tokens);
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

            if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException("Unexpected EOF");
            else if (Stream.Next.Type == ECssTokenType.At_Keyword)
            {
                Rule = Consume_AtRule(Stream);
            }
            else
            {
                Rule = Consume_QualifiedRule(Stream);
                if (Rule == null) throw new CssSyntaxErrorException("Unable to consume qualified rule!");
            }

            Consume_All_Whitespace(Stream);// Consume all whitespace
            if (Stream.Next.Type == ECssTokenType.EOF)
                return Rule;
            else
                throw new CssSyntaxErrorException("Expected EOF");
        }

        public CssDecleration Parse_Decleration()
        {
            Consume_All_Whitespace(Stream);
            if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected Ident token");

            CssDecleration Dec = Consume_Decleration(Stream);
            if (Dec != null) throw new CssSyntaxErrorException("Unable to consume a decleration!");

            return Dec;
        }

        public IEnumerable<CssComponent> Parse_Decleration_List()
        {
            return Consume_Decleration_List(Stream);
        }

        public CssToken Parse_ComponentValue()
        {
            Consume_All_Whitespace(Stream);
            if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException("Unexpected EOF!");

            CssToken Res;
            Res = Consume_ComponentValue(Stream);
            if (Res == null) throw new CssSyntaxErrorException("Unable to consume component value!");

            Consume_All_Whitespace(Stream);
            if (Stream.Next.Type == ECssTokenType.EOF)
                return Res;
            else
                throw new CssSyntaxErrorException("EOF expected");
        }

        public IEnumerable<CssToken> Parse_ComponentValue_List()
        {
            List<CssToken> List = new List<CssToken>();
            CssToken Value;
            do
            {
                Value = Consume_ComponentValue(Stream);
                //if (Value.Type == ECssComponent.PreservedToken && (Value as CssPreservedToken).Value.Type == ECssTokenType.EOF)
                if (Value.Type == ECssTokenType.EOF)
                    return List;

                List.Add(Value);
            }
            while (Value != null);

            return List;
        }

        public CssValue Parse_CssValue()
        {
            return Consume_CssValue(Stream);
        }
        #endregion

        #region Consuming
        /// <summary>
        /// Continually consumes tokens until the current token is not a whitespace one
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static void Consume_All_Whitespace(TokenStream Stream)
        {
            if (Stream == null)
            {
                throw new CssParserException("Stream is NULL!");
            }
            while (Stream.Next.Type == ECssTokenType.Whitespace) { Stream.Consume(); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static IEnumerable<CssComponent> Consume_Rule_List(TokenStream Stream, bool TopLevel = false)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
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
                            if (rule != null) Rules.AddLast(rule);
                        }
                        break;
                    case ECssTokenType.At_Keyword:
                        {
                            Stream.Reconsume();
                            var rule = Consume_AtRule(Stream);
                            if (rule != null) Rules.AddLast(rule);
                        }
                        break;
                    default:
                        {
                            Stream.Reconsume();
                            var rule = Consume_QualifiedRule(Stream);
                            if (rule != null) Rules.AddLast(rule);
                        }
                        break;
                }
            }
            while (Token != CssToken.EOF);

            return Rules;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static CssAtRule Consume_AtRule(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            string name = (Stream.Next as ValuedTokenBase).Value;
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
                        Rule.Block = Consume_SimpleBlock(Stream);
                        break;
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
        static CssQualifiedRule Consume_QualifiedRule(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
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
                            Rule.Block = Consume_SimpleBlock(Stream);
                            return Rule;
                        }

                }
            }
            while (Token.Type != ECssTokenType.EOF);
            return Rule;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static IEnumerable<CssComponent> Consume_Decleration_List(TokenStream Stream)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-list-of-declarations0
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            LinkedList<CssComponent> List = new LinkedList<CssComponent>();
            CssToken Token;
            do
            {
                Token = Stream.Consume();
                switch (Token.Type)
                {
                    case ECssTokenType.Whitespace:
                    case ECssTokenType.Semicolon:
                        continue;
                    case ECssTokenType.EOF:
                        return List;
                    case ECssTokenType.At_Keyword:
                        List.AddLast(Consume_AtRule(Stream));
                        break;
                    case ECssTokenType.Ident:
                        {
                            List<CssToken> tmp = new List<CssToken>();
                            tmp.Add(Token);
                            Stream.Consume();
                            do
                            {
                                if (Stream.Next.Type == ECssTokenType.EOF) break;
                                else if (Stream.Next.Type == ECssTokenType.Semicolon) break;

                                tmp.Add(Stream.Next);
                                Stream.Consume();
                            }
                            while (Stream.Next.Type != ECssTokenType.EOF);

                            List.AddLast(Consume_Decleration(new TokenStream(tmp.ToArray())));
                        }
                        break;
                    default:
                        {// parse error, consume tokens until reaching an EOF or semicolon
                            do
                            {
                                if (Stream.Next.Type == ECssTokenType.EOF) break;
                                else if (Stream.Next.Type == ECssTokenType.Semicolon) break;
                                Stream.Consume();
                            }
                            while (Stream.Next.Type != ECssTokenType.EOF);
                        }
                        break;
                }
            }
            while (Token.Type != ECssTokenType.EOF);

            return List;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static CssDecleration Consume_Decleration(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            string name = (Stream.Consume() as ValuedTokenBase).Value;

            CssDecleration Decleration = new CssDecleration(name);
            // Consume all whitespace
            Consume_All_Whitespace(Stream);
            //while (Stream.Next.Type == ECssTokenType.Whitespace) { Stream.Consume(); }

            if (Stream.Next.Type != ECssTokenType.Colon) return null;// Parser error
            Stream.Consume();// Consume the colon

            CssToken Token;
            do
            {// Find all of the decleration values
                Token = Stream.Consume();
                if (Token.Type == ECssTokenType.EOF) break;
                //Decleration.Value.Add(new CssPreservedToken(Token));
                Decleration.Values.Add(Token as CssComponent);// upcast to Component (Preserve the token)
            }
            while (Token.Type != ECssTokenType.EOF);

            CssToken A = null, B = null;
            // Find the last two non-whitespace tokens out of the declerations values
            for (int i = Decleration.Values.Count - 1; i >= 0; i--)
            {
                CssToken t = Decleration.Values[i];// (Decleration.Value[i] as CssPreservedToken).Value;
                if (t.Type != ECssTokenType.Whitespace)
                {
                    if (B == null)
                    {
                        B = t;
                    }
                    else
                    {
                        A = t;
                        break;
                    }
                }
            }
            // Check if those last two values indicate this declerations 'important' flag is set
            if (A.Type == ECssTokenType.Delim && (A as DelimToken).Value == '!')
            {
                if (B.Type == ECssTokenType.Ident && string.Compare((B as IdentToken).Value, "important", true) == 0)
                {
                    Decleration.Important = true;
                }
            }

            return Decleration;
        }

        /// <summary>
        /// Consumes a block of tokens encased inbetween one of: [], {}, or ()
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static CssSimpleBlock Consume_SimpleBlock(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            CssToken StartToken = Stream.Consume();
            CssToken EndToken;
            switch (StartToken.Type)
            {
                case ECssTokenType.Bracket_Open:
                    EndToken = new BracketCloseToken();
                    break;
                case ECssTokenType.Parenth_Open:
                    EndToken = new ParenthesisCloseToken();
                    break;
                case ECssTokenType.SqBracket_Open:
                    EndToken = new SqBracketCloseToken();
                    break;
                default:
                    throw new CssSyntaxErrorException("Current input token is not the start of a simple block");
            }

            CssSimpleBlock Block = new CssSimpleBlock(StartToken);
            CssToken Token;
            do
            {
                Token = Stream.Consume();
                if (Token.Type == EndToken.Type) return Block;
                if (Token.Type == ECssTokenType.EOF) return Block;

                Block.Values.Add(Consume_ComponentValue(Stream));
            }
            while (Token.Type != ECssTokenType.EOF);

            return Block;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static CssFunction Consume_Function(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            if (Stream.Next.Type != ECssTokenType.FunctionName) throw new CssParserException("Expected function name token!");

            string name = (Stream.Next as FunctionNameToken).Value;
            CssFunction Func = new CssFunction(name);

            CssToken Token;
            do
            {
                Token = Stream.Consume();
                switch (Token.Type)
                {
                    case ECssTokenType.EOF:
                    case ECssTokenType.Parenth_Close:
                        return Func;
                    default:
                        Stream.Reconsume();
                        Func.Arguments.Add(Consume_ComponentValue(Stream));
                        break;
                }
            }
            while (Token.Type != ECssTokenType.EOF);

            return Func;
        }

        /// <summary>
        /// Attempts to consume all tokens within a matching pair of () or {} brackets and otherwise just returns the next token.
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static CssToken Consume_ComponentValue(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            switch (Stream.Next.Type)
            {
                case ECssTokenType.Bracket_Open:
                case ECssTokenType.SqBracket_Open:
                case ECssTokenType.Parenth_Open:
                    {
                        return Consume_SimpleBlock(Stream);
                    }
                case ECssTokenType.FunctionName:
                    {
                        return Consume_Function(Stream);
                    }
            }

            //return new CssPreservedToken(Stream.Consume());
            return Stream.Consume();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]// Private static function called in loops, inline it
        static IEnumerable<IEnumerable<CssToken>> Consume_Comma_Seperated_Component_Value_List(TokenStream Stream)
        {/* Docs: https://drafts.csswg.org/css-syntax-3/#parse-a-comma-separated-list-of-component-values */
            if (Stream == null) throw new CssParserException("Stream is NULL!");

            var cvls = new LinkedList<LinkedList<CssToken>>();
            var node = cvls.AddLast(new LinkedList<CssToken>());

            do
            {
                switch (Stream.Next.Type)
                {
                    case ECssTokenType.Comma:
                    case ECssTokenType.EOF:
                        {
                            cvls.AddLast(node);
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
        public static CssValue Consume_CssValue(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            CssToken Token = Stream.Next;

            switch (Token.Type)
            {
                case ECssTokenType.Dimension:
                    {
                        var tok = Stream.Consume() as DimensionToken;
                        EUnit unit = EUnit.PX;

                        if (!string.IsNullOrEmpty(tok.Unit))
                        {
                            EUnit unitLookup = Lookup.Enum<EUnit>(tok.Unit);
                            unit = unitLookup;
                        }

                        return new CssValue(ECssValueType.DIMENSION, tok.Number, unit);
                    }
                    break;
                case ECssTokenType.Number:
                    {
                        var tok = Stream.Consume() as NumberToken;
                        return new CssValue(ECssValueType.NUMBER, tok.Number);
                    }
                    break;
                case ECssTokenType.Percentage:
                    {
                        var tok = Stream.Consume() as PercentageToken;
                        return new CssValue(ECssValueType.PERCENT, tok.Number);
                    }
                    break;
                case ECssTokenType.String:
                    {
                        var tok = Stream.Consume() as StringToken;
                        return new CssValue(ECssValueType.STRING, tok.Value);
                    }
                    break;
                case ECssTokenType.Ident:// Keyword
                    {
                        var tok = Stream.Consume() as IdentToken;
                        return new CssValue(ECssValueType.KEYWORD, tok.Value);
                    }
                    break;
                case ECssTokenType.FunctionName:
                    {
                        CssFunction func = Consume_Function(Stream);
                        return new CssValue(func);
                    }
                    break;
                case ECssTokenType.Function:
                    {
                        var func = Stream.Consume() as CssFunction;
                        return new CssValue(func);
                    }
                    break;
                case ECssTokenType.Url:
                    throw new NotSupportedException("URL values are not supported as of yet!");
                    break;
            }

            throw new CssParserException($"Unhandled token type cannot be interpreted as css-value: \"{Lookup.Keyword(Token.Type)}\"");
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
                TokenStream tokenStream = new TokenStream(tokenList.ToArray());
                var query = Consume_MediaQuery(tokenStream);
                queryList.AddLast(query);
            }

            return new MediaQueryList(document, queryList);
        }

        static MediaQuery Consume_MediaQuery(TokenStream Stream = null)
        {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
            if (Stream == null) throw new CssParserException("Stream is NULL!");

            if (Stream.Next.Type != ECssTokenType.Ident)
            {
                throw new CssSyntaxErrorException("Expected ident token", Stream);
            }
            EMediaQueryModifier modifier = 0x0;
            EMediaType mediaType = 0x0;
            LinkedList<IMediaCondition> conditionList = new LinkedList<IMediaCondition>();

            /* First check for media modifier */
            if (Stream.Next.Type != ECssTokenType.Ident)
            {
                throw new CssSyntaxErrorException("Expected ident token", Stream);
            }
            if (Lookup.TryEnum((Stream.Next as IdentToken).Value, out EMediaQueryModifier mod))
            {
                Stream.Consume();// consume this token
                modifier = mod;
            }

            /* Skip 'and' keyword if present */
            if (ParserCommon.Is_Combinator(Stream.Next))
            {
                Stream.Consume();
            }

            /* Set the media type */
            if (Stream.Next.Type != ECssTokenType.Ident)
            {
                throw new CssSyntaxErrorException("Expected media type", Stream);
            }

            if (!Lookup.TryEnum((Stream.Next as IdentToken).Value, out EMediaType type))
            {
                throw new CssParserException($"Unrecognized media type: \"{(Stream.Next as IdentToken).Value}\" @\"{ParserCommon.Get_Location(Stream)}\"");
            }
            else
            {
                Stream.Consume();
            }

            /* Skip thew first combinator keyword if present */
            Consume_All_Whitespace(Stream);
            if (ParserCommon.Is_Combinator(Stream.Next))
            {
                Stream.Consume();
            }


            /* Now consume media conditions until we cant anymore */
            do
            {
                Consume_All_Whitespace(Stream);
                if (Stream.Next.Type != ECssTokenType.Parenth_Open)
                {/* This isn't invalid, it just signals that we have no more features to consume */
                    break;
                }

                var condition = Consume_Media_Condition(Stream);
                conditionList.AddLast(condition);
            }
            while (Stream.Next.Type != ECssTokenType.EOF);

            return new MediaQuery(modifier, mediaType, conditionList);
        }

        /// <summary>
        /// Consumes a new <see cref="MediaCondition"/> or <see cref="MediaFeature"/>
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        static IMediaCondition Consume_Media_Condition(TokenStream Stream)
        {/* Docs: https://www.w3.org/TR/mediaqueries-4/#media-condition */
            if (Stream == null) throw new CssParserException("Stream is NULL!");

            Consume_All_Whitespace(Stream);
            if (ParserCommon.Starts_Media_Feature(Stream.AsSpan()))
            {
                /* Consume opening parentheses */
                Stream.Consume();
                var feature = Consume_Media_Feature(Stream);

                /* Consume closing parentheses */
                if (Stream.Next.Type == ECssTokenType.Parenth_Close)
                {
                    Stream.Consume();
                }

                return feature;
            }
            else if (ParserCommon.Starts_Media_Condition(Stream.AsSpan()))
            {
                EMediaCombinator Combinator = EMediaCombinator.None;
                var conditionList = new LinkedList<IMediaCondition>();


                if (Stream.Next.Type == ECssTokenType.Parenth_Close)
                {
                    /* Empty media condition block */
                    Stream.Consume();
                    return new MediaCondition(EMediaCombinator.None, new MediaFeature[0]);
                }
                else if (Stream.Next.Type == ECssTokenType.Parenth_Open)
                {
                    Stream.Consume();
                }

                Consume_All_Whitespace(Stream);
                /* Repeatedly consume sub-media-conditions until we hit a closing parentheses */
                do
                {
                    Consume_All_Whitespace(Stream);

                    if (Stream.Next.Type == ECssTokenType.Parenth_Close)
                    {
                        /* End of this media condition block */
                        break;
                    }

                    /* Anything other than the first condition *MUST* specify a combinator */
                    if (conditionList.Count > 0)
                    {
                        if (!ParserCommon.Is_Combinator(Stream.Next))
                        {
                            throw new CssSyntaxErrorException($"Expected combinator @: {ParserCommon.Get_Location(Stream.AsSpan())}");
                        }
                    }

                    /* Otherwise we just COULD have a combinator */
                    if (ParserCommon.Is_Combinator(Stream.Next))
                    {
                        /* Consume combinator */
                        IdentToken combinatorToken = Stream.Consume() as IdentToken;
                        if (!Lookup.TryEnum(combinatorToken.Value, out EMediaCombinator combLookup))
                        {
                            throw new CssSyntaxErrorException($"Unrecognized combinator keyword \"{combinatorToken.Value}\" in media query @\"{ParserCommon.Get_Location(Stream)}\"");
                        }
                        else if (Combinator == EMediaCombinator.None)
                        {/* This is the first combinator specified */
                            Combinator = combLookup;
                        }
                        else if (Combinator != EMediaCombinator.None && combLookup != Combinator)
                        {/* Ensure this new combinator matches the combinator for this method group */
                            throw new CssSyntaxErrorException($"It is invalid to mix 'and' and 'or' and 'not' at the same level of a media query. please enclose in parentheses the condition @\"{ParserCommon.Get_Location(Stream)}\"");
                        }
                    }

                    Consume_All_Whitespace(Stream);
                    if (Stream.Next.Type != ECssTokenType.Parenth_Open)
                    {
                        throw new CssSyntaxErrorException($"Expected opening parentheses to continue media-condition @\"{ParserCommon.Get_Location(Stream)}\"");
                    }

                    /* Oh look a yummy little sub-condition for us to gobble up! */
                    var feature = Consume_Media_Condition(Stream);
                    conditionList.AddLast(feature);
                }
                while (Stream.Next.Type != ECssTokenType.EOF);

                /* Consume closing parentheses */
                if (Stream.Next.Type == ECssTokenType.Parenth_Close)
                {
                    Stream.Consume();
                }

                return new MediaCondition(Combinator, conditionList);
            }

            throw new CssSyntaxErrorException($"Expected start of media condition @: \"{ParserCommon.Get_Location(Stream)}\"");
        }

        static IMediaCondition Consume_Media_Feature(TokenStream Stream)
        {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
            if (Stream == null) throw new CssParserException("Stream is NULL!");

            
            /* Consume feature name */
            if (ParserCommon.Starts_Boolean_Feature(Stream.AsSpan()))
            {
                /* Consume feature name */
                IdentToken nameTok = Stream.Consume() as IdentToken;

                /* Resolve the name */
                if (!Lookup.TryEnum(nameTok.Value, out EMediaFeatureName Name))
                {
                    throw new CssParserException($"Unrecognized media type: \"{nameTok.Value}\"");
                }

                return new MediaFeature(Name);
            }
            else if (ParserCommon.Starts_Discreet_Feature(Stream.AsSpan()))
            {
                /* Consume feature name */
                IdentToken nameTok = Stream.Consume() as IdentToken;

                /* Resolve the name */
                if (!Lookup.TryEnum(nameTok.Value, out EMediaFeatureName Name))
                {
                    throw new CssParserException($"Unrecognized media type: \"{nameTok.Value}\"");
                }

                /* Consume the value to match */
                Consume_All_Whitespace(Stream);
                var value = Consume_MediaFeature_Value(Stream);

                return new MediaFeature(new CssValue[] { CssValue.From_Enum(Name), value }, new EMediaOperator[] { EMediaOperator.EqualTo });
            }
            else if (ParserCommon.Starts_Range_Feature(Stream.AsSpan()))
            {
                /* This is a range feature of some sort, it could be a short one or a long one */
                /* Repeatedly consume CssValues, operator, and a single ident */
                LinkedList<CssValue> Values = new LinkedList<CssValue>();
                LinkedList<EMediaOperator> Ops = new LinkedList<EMediaOperator>();
                bool firstToken = true;
                bool lastWasComparator = false;

                while (Stream.Next != CssToken.EOF)
                {
                    Consume_All_Whitespace(Stream);

                    if (Stream.Next.Type == ECssTokenType.Parenth_Close)
                    {
                        break;
                    }
                    else if (Stream.Next.Type == ECssTokenType.Ident)
                    {
                        if (!firstToken && !lastWasComparator)
                        {
                            throw new CssSyntaxErrorException($"Expected comparator @: \"{ParserCommon.Get_Location(Stream.AsSpan())}\"");
                        }

                        var nameTok = (IdentToken)Stream.Consume();
                        /* Resolve the name */
                        if (!Lookup.TryEnum(nameTok.Value, out EMediaFeatureName Name))
                        {
                            throw new CssParserException($"Unrecognized media type: \"{nameTok.Value}\"");
                        }

                        var value = CssValue.From_Enum(Name);
                        Values.AddLast(value);
                        lastWasComparator = false;
                    }
                    else if (ParserCommon.Starts_MF_Ident_Or_Value(Stream.AsSpan()))
                    {
                        if (!firstToken && !lastWasComparator)
                        {
                            throw new CssSyntaxErrorException($"Expected comparator @: \"{ParserCommon.Get_Location(Stream.AsSpan())}\"");
                        }

                        CssValue value = Consume_MediaFeature_Value(Stream);
                        Values.AddLast(value);
                        lastWasComparator = false;
                    }
                    else if (ParserCommon.Is_Comparator(Stream.Next))
                    {
                        if (lastWasComparator || firstToken)
                        {
                            throw new CssSyntaxErrorException($"Unexpected comparator @: \"{ParserCommon.Get_Location(Stream.AsSpan())}\"");
                        }

                        var comparatorTok = (ValuedTokenBase)Stream.Consume();
                        if (!Lookup.TryEnum(comparatorTok.Value, out EMediaOperator outComparator))
                        {
                            throw new CssParserException($"Unable to interpret keyword \"{comparatorTok.Value}\" as enum value");
                        }

                        Ops.AddLast(outComparator);
                        lastWasComparator = true;
                    }

                    firstToken = false;
                }

                return new MediaFeature(Values.ToArray(), Ops.ToArray());
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static CssValue Consume_MediaFeature_Value(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
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
                            DelimToken dtok = Stream.Consume() as DelimToken;
                            Consume_All_Whitespace(Stream);
                            NumberToken numTok2 = Stream.Consume() as NumberToken;

                            double ratioValue = (double)(numTok.Number / numTok2.Number);
                            return new CssValue(ECssValueType.RATIO, ratioValue);
                        }

                        /* Nope, its just a number */
                        return new CssValue(ECssValueType.NUMBER, numTok.Number);
                    }
                    break;
                case ECssTokenType.Dimension:
                case ECssTokenType.Ident:
                    {
                        return Consume_CssValue(Stream);
                    }
                    break;
            }

            throw new CssSyntaxErrorException($"Expected Number/Dimension/Keyword token but got: \"{Enum.GetName(typeof(ECssTokenType), Stream.Next.Type)}\"");
        }


        #endregion
    }
}
