using CssUI.CSS.Media;
using CssUI.CSS.Parser;
using CssUI.DOM;
using CssUI.DOM.Media;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Serialization
{// DOCS: https://www.w3.org/TR/css-syntax-3/#consume-a-simple-block

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
        public List<CssComponent> Parse_Rule_List()
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

        public List<CssComponent> Parse_Decleration_List()
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

        public List<CssToken> Parse_ComponentValue_List()
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Consume_All_Whitespace(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            while (Stream.Next.Type == ECssTokenType.Whitespace) { Stream.Consume(); }
        }

        static LinkedList<CssComponent> Consume_Rule_List(TokenStream Stream, bool TopLevel = false)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static LinkedList<CssComponent> Consume_Decleration_List(TokenStream Stream)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static CssDecleration Consume_Decleration(TokenStream Stream)
        {
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            string name = (Stream.Consume() as ValuedTokenBase).Value;

            CssDecleration Decleration = new CssDecleration(name);
            // Consume all whitespace
            while (Stream.Next.Type == ECssTokenType.Whitespace) { Stream.Consume(); }
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
        /// Attempts to consume all tokens within a matching pair of () or {} brackets and otherwise just returns the next token.
        /// </summary>
        /// <param name="Stream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                    throw new CssSyntaxErrorException("Current input token is not a bracket type!");
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                        ECssUnit unit = ECssUnit.PX;

                        if (!string.IsNullOrEmpty(tok.Unit))
                        {
                            ECssUnit? unitLookup = CssLookup.Enum_From_Keyword<ECssUnit>(tok.Unit);
                            if (!unitLookup.HasValue) throw new CssParserException($"Failed to find keyword in enum lookup table: \"{tok.Unit}\"");
                            unit = unitLookup.Value;
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

            throw new CssParserException($"Unhandled token type cannot be interpreted as css-value: \"{CssLookup.Keyword_From_Enum(Token.Type)}\"");
        }
        #endregion

        #region Media
        public MediaQueryList Parse_Media_Query_List(Document document)
        {
            Consume_All_Whitespace();// Consume all whitespace

            if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException("Unexpected EOF");
            if (Stream.Next.Type != ECssTokenType.At_Keyword) throw new CssSyntaxErrorException("Expected at-rule");

            CssAtRule rule = Consume_AtRule(Stream);
            if (ReferenceEquals(null, rule)) throw new CssSyntaxErrorException("Failed to parse Media rule, cannot parse at-rule");

            LinkedList<MediaQuery> queryList = new LinkedList<MediaQuery>();
            TokenStream tokenStream = new TokenStream(rule.Prelude);
            CssToken Token = null;
            do
            {
                var query = Consume_MediaQuery(tokenStream);
                queryList.AddLast(query);
            }
            while (Token != CssToken.EOF);

            return new MediaQueryList(document, queryList);
        }

        static MediaQuery Consume_MediaQuery(TokenStream Stream = null)
        {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
            if (Stream == null) throw new CssParserException("Stream is NULL!");

            if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected ident token");
            EMediaQueryModifier modifier = 0x0;
            EMediaType mediaType = 0x0;
            LinkedList<MediaFeature> featureList = new LinkedList<MediaFeature>();

            IdentToken Token = Stream.Next as IdentToken;

            /* First check for media modifier */
            EMediaQueryModifier? mod = CssLookup.Enum_From_Keyword<EMediaQueryModifier>(Token.Value);
            if (mod.HasValue)
            {
                Stream.Consume();// consume this token
                modifier = mod.Value;

                if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected ident token");
                Token = Stream.Next as IdentToken;
            }

            /* Skip 'and' keyword if present */
            if (Token.Value == "and")
            {
                Stream.Consume();

                if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected ident token");
                Token = Stream.Next as IdentToken;
            }

            /* Set the media type */
            EMediaType? type = CssLookup.Enum_From_Keyword<EMediaType>(Token.Value);
            if (!type.HasValue) throw new CssParserException($"Unrecognized media type: \"{Token.Value}\"");
            else
            {
                Stream.Consume();
            }

            /* Skip 'and' keyword if present */
            if (Token.Value == "and")
            {
                Stream.Consume();

                if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected ident token");
                Token = Stream.Next as IdentToken;
            }

            /* Consume features */
            if (Stream.Next.Type == ECssTokenType.Parenth_Open)
            {
                do
                {
                    Consume_All_Whitespace(Stream);
                    if (Stream.Next.Type == ECssTokenType.Parenth_Close)
                    {
                        Stream.Consume();
                        break;
                    }

                    var feature = Consume_MediaFeature(Stream);
                    featureList.AddLast(feature);
                }
                while (Stream.Next.Type != ECssTokenType.EOF);
            }

            return new MediaQuery(modifier, mediaType, featureList);
        }

        static MediaFeature Consume_MediaFeature(TokenStream Stream = null)
        {/* Docs: https://drafts.csswg.org/mediaqueries-4/#mq-syntax */
            if (Stream == null) throw new CssParserException("Stream is NULL!");
            /*
             * <mf-plain> = <mf-name> : <mf-value>
             * <mf-boolean> = <mf-name>
             * <mf-range> = <mf-name> <mf-comparison> <mf-value>
             * <mf-value> <mf-comparison> <mf-name>
             * <mf-value> <mf-lt> <mf-name> <mf-lt> <mf-value>
             * <mf-value> <mf-gt> <mf-name> <mf-gt> <mf-value>
             */
            Consume_All_Whitespace(Stream);
            /* Consume feature name */
            if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException($"Expected Ident-token, but got \"{Enum.GetName(typeof(ECssTokenType), Stream.Next.Type)}\"");
            IdentToken nameTok = Stream.Consume() as IdentToken;
            /* Resolve the name */
            EMediaFeature? Name = CssLookup.Enum_From_Keyword<EMediaFeature>(nameTok.Value);
            if (!Name.HasValue) throw new CssParserException($"Unrecognized media type: \"{nameTok.Value}\"");
            /* Determine feature type: plain / boolean / range */
            if (Stream.Next.Type == ECssTokenType.Colon)
            {/* Plain */
                Stream.Consume();// Consume the ':'
                var value = Consume_MediaFeature_Value(Stream);
                return new MediaFeature(Name.Value, value);
            }

            Consume_All_Whitespace(Stream);
            /* If the next token is 'and' then it signals the end of this feature */
            if (!ParserCommon.Starts_Media_Comparator(Stream.Next))
            {/* Boolean */
                return new MediaFeature(Name.Value, null);
            }

            /* This must be a range */
            var comparatorTok = (Stream.Consume() as ValuedTokenBase);
            string comparatorString = comparatorTok.Value.ToString();
            char[] compBuf = comparatorString.ToCharArray();
            EMediaFeatureComparator comparator = 0x0;
            for(int i=0;i<comparatorString.Length; i++)
            {
                EMediaFeatureComparator? lookup = CssLookup.Enum_From_Keyword<EMediaFeatureComparator>(compBuf[i].ToString());
                if (!lookup.HasValue)
                {
                    throw new CssParserException($"Unable to interpret keyword \"{comparatorString}\" as enum value");
                }

                comparator |= lookup.Value;
            }

            Consume_All_Whitespace(Stream);
            /* Safety check if the next toke is the 'and' keyword */
            if (Stream.Next.Type == ECssTokenType.Ident && (Stream.Next as IdentToken).Value.Equals("and"))
                throw new CssSyntaxErrorException($"A value is expected after the comparator \'{comparatorTok.Value}\' within a Media rule");

            CssValue rangeValue = Consume_CssValue(Stream);


            return new MediaFeature(Name.Value, comparator, rangeValue);
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
                        if (ParserCommon.Starts_Ratio_Value(Stream.Next, Stream.NextNext))
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
