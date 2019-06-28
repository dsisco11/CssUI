using System;
using System.Collections.Generic;

namespace CssUI.CSS.Parser
{// DOCS: https://www.w3.org/TR/css-syntax-3/#consume-a-simple-block

    /// <summary>
    /// Parses a stream of <see cref="CssToken"/>s and returns 
    /// </summary>
    public class CssParser
    {
        #region Properties
        readonly CssTokenStream Stream;
        bool TopLevel = false;
        #endregion

        #region Constructors
        public CssParser(string Text)
        {
            CssTokenizer Tokenizer = new CssTokenizer(Text);
            Stream = new CssTokenStream(Tokenizer.Tokens);
        }

        public CssParser(CssToken[] Tokens)
        {
            Stream = new CssTokenStream(Tokens);
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
            return Consume_Rule_List();
        }

        /// <summary>
        /// Parses and returns a single rule
        /// </summary>
        /// <returns></returns>
        public CssComponent Parse_Rule()
        {
            CssComponent Rule;
            Consume_All_Whitespace();// Consume all whitespace

            if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException("Unexpected EOF");
            else if (Stream.Next.Type == ECssTokenType.At_Keyword)
            {
                Rule = Consume_AtRule();
            }
            else
            {
                Rule = Consume_QualifiedRule();
                if (Rule == null) throw new CssSyntaxErrorException("Unable to consume qualified rule!");
            }

            Consume_All_Whitespace();// Consume all whitespace
            if (Stream.Next.Type == ECssTokenType.EOF)
                return Rule;
            else
                throw new CssSyntaxErrorException("Expected EOF");
        }

        public CssDecleration Parse_Decleration()
        {
            Consume_All_Whitespace();
            if (Stream.Next.Type != ECssTokenType.Ident) throw new CssSyntaxErrorException("Expected Ident token");

            CssDecleration Dec = Consume_Decleration();
            if (Dec != null) throw new CssSyntaxErrorException("Unable to consume a decleration!");

            return Dec;
        }

        public List<CssComponent> Parse_Decleration_List()
        {
            return Consume_Decleration_List();
        }

        public CssToken Parse_ComponentValue()
        {
            Consume_All_Whitespace();
            if (Stream.Next.Type == ECssTokenType.EOF) throw new CssSyntaxErrorException("Unexpected EOF!");

            CssToken Res;
            Res = Consume_ComponentValue();
            if (Res == null) throw new CssSyntaxErrorException("Unable to consume component value!");

            Consume_All_Whitespace();
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
                Value = Consume_ComponentValue();
                //if (Value.Type == ECssComponent.PreservedToken && (Value as CssPreservedToken).Value.Type == ECssTokenType.EOF)
                if (Value.Type == ECssTokenType.EOF)
                    return List;

                List.Add(Value);
            }
            while (Value != null);

            return List;
        }

        #endregion

        #region Consuming
        /// <summary>
        /// Continually consumes tokens until the current token is not a whitespace one
        /// </summary>
        void Consume_All_Whitespace(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
            while (Stream.Next.Type == ECssTokenType.Whitespace) { Stream.Consume(); }
        }

        List<CssComponent> Consume_Rule_List(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
            List<CssComponent> Rules = new List<CssComponent>();

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
                            var rule = Consume_QualifiedRule();
                            if (rule != null) Rules.Add(rule);
                        }
                        break;
                    case ECssTokenType.At_Keyword:
                        {
                            Stream.Reconsume();
                            var rule = Consume_AtRule();
                            if (rule != null) Rules.Add(rule);
                        }
                        break;
                    default:
                        {
                            Stream.Reconsume();
                            var rule = Consume_QualifiedRule();
                            if (rule != null) Rules.Add(rule);
                        }
                        break;
                }
            }
            while (Token != CssToken.EOF);

            return Rules;
        }

        CssAtRule Consume_AtRule(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
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
                        Rule.Block = Consume_SimpleBlock();
                        break;
                    default:
                        {
                            Stream.Reconsume();
                            Rule.Prelude.Add(Consume_ComponentValue());
                        }
                        break;
                }
            }
            while (Token.Type != ECssTokenType.EOF);
            return Rule;
        }

        CssQualifiedRule Consume_QualifiedRule(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
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
                            Rule.Block = Consume_SimpleBlock();
                            return Rule;
                        }

                }
            }
            while (Token.Type != ECssTokenType.EOF);
            return Rule;
        }

        List<CssComponent> Consume_Decleration_List(CssTokenStream Stream = null)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-list-of-declarations0
            if (Stream == null) Stream = this.Stream;
            List<CssComponent> List = new List<CssComponent>();
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
                        List.Add(Consume_AtRule());
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

                            List.Add(Consume_Decleration(new CssTokenStream(tmp.ToArray())));
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

        CssDecleration Consume_Decleration(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
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

            CssToken A=null, B=null;
            // Find the last two non-whitespace tokens out of the declerations values
            for(int i=Decleration.Values.Count-1; i >= 0; i--)
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
                if (B.Type == ECssTokenType.Ident && string.Compare((B as IdentToken).Value, "important", true)==0)
                {
                    Decleration.Important = true;
                }
            }

            return Decleration;
        }

        CssToken Consume_ComponentValue(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
            switch (Stream.Next.Type)
            {
                case ECssTokenType.Bracket_Open:
                case ECssTokenType.SqBracket_Open:
                case ECssTokenType.Parenth_Open:
                    return Consume_SimpleBlock();
                case ECssTokenType.FunctionName:
                    return Consume_Function();
            }

            //return new CssPreservedToken(Stream.Consume());
            return Stream.Consume();
        }

        CssSimpleBlock Consume_SimpleBlock(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
            CssToken StartToken = Stream.Consume();
            CssToken EndToken;
            switch(StartToken.Type)
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

                Block.Values.Add(Consume_ComponentValue());
            }
            while (Token.Type != ECssTokenType.EOF);

            return Block;
        }

        CssFunction Consume_Function(CssTokenStream Stream = null)
        {
            if (Stream == null) Stream = this.Stream;
            string name = (Stream.Next as ValuedTokenBase).Value;
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
                        Func.Arguments.Add(Consume_ComponentValue());
                        break;
                }
            }
            while (Token.Type != ECssTokenType.EOF);

            return Func;
        }
        #endregion
    }
}
