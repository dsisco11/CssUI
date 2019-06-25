using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS
{
    /// <summary>
    /// Parses low-level <see cref="CssComponent"/>s into higher-level <see cref="CSSSelectorComponent"/> objects
    /// </summary>
    public class CssSelectorParser
    {
        readonly CssTokenStream Stream;

        public CssSelectorParser(string SelectorString)
        {
            CssParser parser = new CssParser(SelectorString);
            List<CssToken> items = parser.Parse_ComponentValue_List();
            /*
            var strm = new ObjectStream<CssComponent>(items.ToArray(), null);
            IEnumerable<CssToken> Tokens = strm.Consume_While(c => c.Type == ECssComponent.PreservedToken).Select(c => (c as CssPreservedToken).Value);
            var tokens = new ObjectStream<CssToken>(Tokens, CssToken.EOF);
            */
            /*
            // Pre-parse the tokens into appropriate Selector token types
            var tokens = new ObjectStream<CssToken>(items, CssToken.EOF);
            this.Stream = Parse_Tokens(tokens);
            */

            this.Stream = new CssTokenStream(items);
        }

        #region Pre-Parsing
        static ObjectStream<CssToken> Parse_Tokens(ObjectStream<CssToken> Stream)
        {
            List<CssToken> List = new List<CssToken>();
            CssToken Token;
            do
            {
                Token = Consume_Token(Stream);
                List.Add(Token);
            }
            while (Token.Type != ECssTokenType.EOF);
            return new ObjectStream<CssToken>(List, CssToken.EOF);
        }
        #endregion

        #region Pre-parse Consumption

        static CssToken Consume_Token(ObjectStream<CssToken> Stream)
        {
            if (Starts_Combinator(Stream.Peek(0)))
            {
                var o = Consume_Combinator(Stream);
                if (o == null) throw new CssParserError("Unable to consume combinator!");
                return o;
            }
            else if (Starts_QualifiedName(Stream.Peek(0), Stream.Peek(1), Stream.Peek(2)))
            {
                var o = Consume_QualifiedName(Stream);
                if (o == null) throw new CssParserError("Unable to consume qualified name!");
                return o;
            }
            else if (Starts_NamespacePrefix(Stream.Peek(0), Stream.Peek(1)))
            {
                var o = Consume_NamespacePrefix(Stream);
                if (o == null) throw new CssParserError("Unable to consume namespace prefix!");
                return o;
            }

            return Stream.Consume();
        }

        static NamespacePrefixToken Consume_NamespacePrefix(ObjectStream<CssToken> Stream)
        {
            string Name = null;

            CssToken Tok;
            Tok = Stream.Consume();

            if (Tok.Type == ECssTokenType.Ident)
            {
                Name = (Tok as IdentToken).Value;
                if (Stream.Next.Type != ECssTokenType.Column) throw new CssParserError("Namespace prefixes must be followed by a '|' token!");

                Stream.Consume();// Consume the next '|' token
            }
            else if (Tok.Type == ECssTokenType.Delim && (Tok as DelimToken).Value == '*')
            {
                Name = "*";// Any namespace
                if (Stream.Next.Type != ECssTokenType.Column) throw new CssParserError("Namespace prefixes must be followed by a '|' token!");

                Stream.Consume();// Consume the next '|' token
            }
            else if (Tok.Type == ECssTokenType.Column)
            {
                Name = null;// no namespace
            }

            return new NamespacePrefixToken(Name);
        }

        static QualifiedNameToken Consume_QualifiedName(ObjectStream<CssToken> Stream)
        {
            NamespacePrefixToken NS = null;
            if (Starts_NamespacePrefix(Stream.Next, Stream.NextNext))
            {
                NS = Consume_NamespacePrefix(Stream);
            }

            CssToken Tok = Stream.Consume();
            if (Tok.Type != ECssTokenType.Ident) return null;
            string Name = (Tok as IdentToken).Value;

            return new QualifiedNameToken(Name, NS);
        }

        static CombinatorToken Consume_Combinator(ObjectStream<CssToken> Stream)
        {
            var ws = Stream.Consume_While(o => o.Type == ECssTokenType.Whitespace);
            string Value = string.Empty;
            if (Stream.Next.Type == ECssTokenType.Delim)
            {
                Value = string.Concat((Stream.Consume() as DelimToken).Value);
                if (Stream.Next.Type == ECssTokenType.Delim)
                {
                    char dv = (Stream.Next as DelimToken).Value;
                    if (dv == '>')
                    {
                        Value = string.Concat(Value, (Stream.Consume() as DelimToken).Value);
                    }
                }

                Stream.Consume_While(o => o.Type == ECssTokenType.Whitespace);// Consume all whitespace after the combinator
                return new CombinatorToken(Value);
            }
            else if (Stream.Next.Type == ECssTokenType.Column)
            {
                Stream.Consume();
                Stream.Consume_While(o => o.Type == ECssTokenType.Whitespace);// Consume all whitespace after the combinator
                return new CombinatorToken("||");
            }

            if (ws.Count > 0)// We consumed whitespace so at this point the only option is that our combinator was intended to be that whitespace
            {
                return new CombinatorToken(">>");
            }

            return null;
        }
        #endregion


        #region Parsing
        public CssSelectorList Parse_Selector_List()
        {
            return Consume_Selector_List(Stream);
        }
        #endregion

        #region Testing
        static bool Is_Char(CssToken A, char CH)
        {
            return (A.Type == ECssTokenType.Delim && (A as DelimToken).Value == CH);
        }

        static bool Starts_Simple_Selector(CssToken A, CssToken B, CssToken C)
        {
            if (Starts_ID_Selector(A)) return true;
            if (Starts_Type_Selector(A, B, C)) return true;
            if (Starts_Class_Selector(A, B)) return true;
            if (Starts_Universal_Selector(A, B)) return true;
            if (Starts_Attribute_Selector(A, B)) return true;
            if (Starts_Pseudo_Class_Selector(A, B)) return true;
            
            return false;
        }

        static bool Starts_ID_Selector(CssToken A)
        {
            return (A.Type == ECssTokenType.Hash && (A as HashToken).HashType == EHashTokenType.ID);
        }

        static bool Starts_Class_Selector(CssToken A, CssToken B)
        {
            return (Is_Char(A, '.') && B.Type == ECssTokenType.Ident);
        }

        static bool Starts_Attribute_Selector(CssToken A, CssToken B)
        {
            return (A.Type == ECssTokenType.SqBracket_Open && B.Type == ECssTokenType.QualifiedName);
        }

        static bool Starts_Pseudo_Class_Selector(CssToken A, CssToken B)
        {
            return (A.Type == ECssTokenType.Colon && (B.Type == ECssTokenType.Ident || B.Type == ECssTokenType.FunctionName));
        }

        static bool Starts_Pseudo_Element_Selector(CssToken A, CssToken B, CssToken C)
        {
            return (A.Type == ECssTokenType.Colon && B.Type == ECssTokenType.Colon && (C.Type == ECssTokenType.Ident || C.Type == ECssTokenType.FunctionName));
        }

        static bool Starts_Universal_Selector(CssToken A, CssToken B)
        {
            return (Is_Char(A, '*') || A.Type == ECssTokenType.NamespacePrefix && Is_Char(B, '*'));
        }

        static bool Starts_Type_Selector(CssToken A, CssToken B, CssToken C)
        {
            if (Starts_NamespacePrefix(A, B))
            {
                return C.Type == ECssTokenType.Ident;
            }
            return (A.Type == ECssTokenType.Ident);
            //return (A.Type == ECssTokenType.QualifiedName || A.Type == ECssTokenType.NamespacePrefix && B.Type == ECssTokenType.QualifiedName);
        }

        static bool Starts_NamespacePrefix(CssToken A, CssToken B)
        {
            if (B.Type == ECssTokenType.Column)
            {
                if ((A.Type == ECssTokenType.QualifiedName || Is_Char(A, '*'))) return true;
                if ((A.Type == ECssTokenType.Ident || Is_Char(A, '*'))) return true;
            }

            return (A.Type == ECssTokenType.Column);
        }

        static bool Starts_QualifiedName(CssToken A, CssToken B, CssToken C)
        {
            if (Starts_NamespacePrefix(A, B) && C.Type == ECssTokenType.Ident) return true;
            return (A.Type == ECssTokenType.Ident);
        }

        static bool Starts_Combinator(CssToken A)
        {
            if (A.Type == ECssTokenType.Whitespace) return true;// ' '
            if (A.Type == ECssTokenType.Column) return true;// '||'
            if (A.Type != ECssTokenType.Delim) return false;

            char delim = (A as DelimToken).Value;
            return (delim == '>' || delim == '+' || delim == '~');
        }
        #endregion

        #region Selector Consuming
        public static CssSelectorList Consume_Selector_List(CssTokenStream Stream)
        {// SEE:  https://www.w3.org/TR/2011/REC-css3-selectors-20110929/#selectors
            CssSelectorList List = null;

            CssSelectorFilterSet Selector = null;
            do
            {
                Selector = Consume_Selector_FilterSet(Stream);
                if (Selector != null)
                {
                    if (List == null) List = new CssSelectorList();
                    List.Add(Selector);
                }
            }
            while (Selector != null);
            return List;
        }

        /// <summary>
        /// Consumes a list of Compound/Complex selectors
        /// </summary>
        public static CssSelectorFilterSet Consume_Selector_FilterSet(CssTokenStream Stream)
        {
            CssSelectorFilterSet List = null;
            ICssSelectorFilter Filter;
            do
            {
                Filter = Consume_Filter(Stream);
                if (Filter != null)
                {
                    if (List == null) List = new CssSelectorFilterSet();
                    List.Add(Filter);
                }

                switch (Stream.Next.Type)
                {
                    case ECssTokenType.Comma:
                        Stream.Consume();
                        break;
                    case ECssTokenType.EOF:
                    default:
                        return List;
                }
            }
            while (Filter != null);

            return null;
        }

        /// <summary>
        /// Consumes a list of Compound/Complex selectors
        /// </summary>
        public static CssSelectorFilterSet Consume_Complex_Selector_List(CssTokenStream Stream)
        {
            CssSelectorFilterSet List = null;
            CssComplexSelector Complex;
            do
            {
                Complex = Consume_Complex_Selector(Stream);
                if (Complex != null)
                {
                    if (List == null) List = new CssSelectorFilterSet();
                    List.Add(Complex);
                }

                switch (Stream.Next.Type)
                {
                    case ECssTokenType.Comma:
                        Stream.Consume();
                        break;
                    case ECssTokenType.EOF:
                    default:
                        return List;
                }
            }
            while (Complex != null);

            return null;
        }

        /// <summary>
        /// Consumes a new selector-filter item
        /// </summary>
        /// <returns></returns>
        static ICssSelectorFilter Consume_Filter(CssTokenStream Stream)
        {
            Stream.Consume_While(tok => tok.Type == ECssTokenType.Whitespace);// Consume all of the prefixing whitespace
            CssCompoundSelector Compound = Consume_Compound_Selector(Stream);
            if (Compound == null) return null;
            if (!Starts_Combinator(Stream.Next)) return Compound;

            CombinatorToken Comb = Consume_Combinator(Stream);
            ESelectorCombinator Combinator = ESelectorCombinator.None;
            if (Stream.Next.Type == ECssTokenType.Combinator)
            {
                switch (Comb.Value)
                {
                    case " ":
                    case ">>":
                        Combinator = ESelectorCombinator.Descendant;
                        break;
                    case ">":
                        Combinator = ESelectorCombinator.Child;
                        break;
                    case "+":
                        Combinator = ESelectorCombinator.Sibling_Adjacent;
                        break;
                    case "~":
                        Combinator = ESelectorCombinator.Sibling_General;
                        break;
                    default:
                        throw new CssParserError(string.Concat("Unrecognized Combinator(", Comb.Value, ")"));
                }
            }

            return new CssComplexSelector(Combinator, Compound);
        }

        /// <summary>
        /// Consumes a compound selector and it's combinator (if available)
        /// </summary>
        /// <returns></returns>
        static CssComplexSelector Consume_Complex_Selector(CssTokenStream Stream)
        {
            Stream.Consume_While(tok => tok.Type == ECssTokenType.Whitespace);// Consume all of the prefixing whitespace
            CssCompoundSelector Compound = Consume_Compound_Selector(Stream);
            if (Compound == null) return null;
            if (!Starts_Combinator(Stream.Next)) return new CssComplexSelector(ESelectorCombinator.None, Compound);

            CombinatorToken Comb = Consume_Combinator(Stream);
            ESelectorCombinator Combinator = ESelectorCombinator.None;
            if (Stream.Next.Type == ECssTokenType.Combinator)
            {
                switch(Comb.Value)
                {
                    case " ":
                    case ">>":
                        Combinator = ESelectorCombinator.Descendant;
                        break;
                    case ">":
                        Combinator = ESelectorCombinator.Child;
                        break;
                    case "+":
                        Combinator = ESelectorCombinator.Sibling_Adjacent;
                        break;
                    case "~":
                        Combinator = ESelectorCombinator.Sibling_General;
                        break;
                    default:
                        throw new CssParserError(string.Concat("Unrecognized Combinator(", Comb.Value, ")"));
                }
            }

            return new CssComplexSelector(Combinator, Compound);
        }

        /// <summary>
        /// Consumes a compound selector, which is a comprised of multiple simple selectors
        /// </summary>
        /// <returns></returns>
        static CssCompoundSelector Consume_Compound_Selector(CssTokenStream Stream)
        {
            CssCompoundSelector Compound = null;
            CssSimpleSelector Simple;
            do
            {
                if (!Starts_Simple_Selector(Stream.Next, Stream.NextNext, Stream.NextNextNext)) return Compound;

                Simple = Consume_Simple_Selector(Stream);
                if (Simple != null)
                {
                    if (Compound == null) Compound = new CssCompoundSelector();
                    Compound.Selectors.Add(Simple);
                }
            }
            while (Simple != null);

            return Compound;
        }

        static CssSimpleSelector Consume_Simple_Selector(CssTokenStream Stream)
        {
            if (Starts_ID_Selector(Stream.Next))
            {
                return Consume_ID_Selector(Stream);
            }
            else if (Starts_Universal_Selector(Stream.Next, Stream.NextNext))
            {
                return Consume_Universal_Selector(Stream);
            }
            else if (Starts_Type_Selector(Stream.Next, Stream.NextNext, Stream.NextNextNext))
            {
                return Consume_Type_Selector(Stream);
            }
            else if (Starts_Class_Selector(Stream.Next, Stream.NextNext))
            {
                return Consume_Class_Selector(Stream);
            }
            else if (Starts_Attribute_Selector(Stream.Next, Stream.NextNext))
            {
                return Consume_Attribute_Selector(Stream);
            }
            else if (Starts_Pseudo_Class_Selector(Stream.Next, Stream.NextNext))
            {
                return Consume_Pseudo_Class_Selector(Stream);
            }
            else if (Starts_Pseudo_Element_Selector(Stream.Next, Stream.NextNext, Stream.NextNextNext))
            {
                return Consume_Pseudo_Element_Selector(Stream);
            }

            return null;
        }

        #endregion

        #region Selector Consumption

        static CssIDSelector Consume_ID_Selector(CssTokenStream Stream)
        {
            HashToken Hash = (Stream.Consume() as HashToken);
            if (Hash.HashType != EHashTokenType.ID) throw new CssParserError("Invalid Hash token, hash-type is not ID!");
            return new CssIDSelector(Hash.Value);
        }

        static CssUniversalSelector Consume_Universal_Selector(CssTokenStream Stream)
        {
            if (Starts_NamespacePrefix(Stream.Next, Stream.NextNext))
            {
                Consume_NamespacePrefix(Stream);
            }
            CssToken Token = Stream.Consume();// Consume the '*' character

            if (Token.Type == ECssTokenType.NamespacePrefix)
            {
                Stream.Consume();// Consume the '*' character now, because the first token we consumed was actually just the namespace prefix
            }

            return new CssUniversalSelector();
        }

        static CssTypeSelector Consume_Type_Selector(CssTokenStream Stream)
        {
            if (Starts_NamespacePrefix(Stream.Next, Stream.NextNext))
            {
                NamespacePrefixToken Namespace = Consume_NamespacePrefix(Stream);
                return new CssTypeSelector(Namespace, Stream.Consume<IdentToken>().Value);
            }
            
            return new CssTypeSelector(Stream.Consume<IdentToken>().Value);
        }

        static CssClassSelector Consume_Class_Selector(CssTokenStream Stream)
        {
            Stream.Consume();// Consume the '.' prefixing the classname
            IdentToken Ident = Stream.Consume<IdentToken>();

            return new CssClassSelector(Ident.Value);
        }

        /// <summary>
        /// Consumes an attribute selector item from the stream
        /// <para>Attribute-Selector formats:</para>
        /// <para>'[' <qualified-name> ']'</para>
        /// <para>'[' <qualified-name> <attr-matcher> [ <string-token> | <ident-token> ] <attr-modifier>? ']'</para>
        /// </summary>
        /// <returns></returns>
        static CssAttributeSelector Consume_Attribute_Selector(CssTokenStream Stream)
        {
            Stream.Consume();// Consume the '[' prefix

            NamespacePrefixToken NS = null;
            if (Starts_NamespacePrefix(Stream.Next, Stream.NextNext)) NS = Consume_NamespacePrefix(Stream);

            //QualifiedNameToken attrName = Stream.Consume<QualifiedNameToken>();
            IdentToken attrName = Stream.Consume<IdentToken>();
            CssToken Tok = Stream.Consume();
            if (Tok.Type == ECssTokenType.SqBracket_Close)
            {
                return new CssAttributeSelector(NS, attrName.Value);
            }

            CssToken OperatorToken = Tok;
            CssToken value = Stream.Consume();
            if (value.Type == ECssTokenType.SqBracket_Close) return null;// Parse error
            if (Stream.Next.Type != ECssTokenType.SqBracket_Close) return null;// Parse error
            Stream.Consume();// Consume the closing bracket

            if (value.Type == ECssTokenType.String)
            {
                return new CssAttributeSelector(NS, attrName.Value, OperatorToken, (value as StringToken).Value);
            }
            else if (value.Type == ECssTokenType.Ident)
            {
                return new CssAttributeSelector(NS, attrName.Value, OperatorToken, (value as IdentToken).Value);
            }

            return null;// Parse error
        }

        /// <summary>
        /// Consumes a pseudo-class item from the stream
        /// <para>Pseudo-Class formats:</para>
        /// <para>':' <ident-token></para>
        /// <para>':' <function-token> <any-value> ')'</para>
        /// </summary>
        static CssPseudoClassSelector Consume_Pseudo_Class_Selector(CssTokenStream Stream)
        {
            Stream.Consume();// Consume the ':' prefix
            switch (Stream.Next.Type)
            {
                case ECssTokenType.Ident:
                    {
                        return new CssPseudoClassSelector(Stream.Consume<IdentToken>().Value);
                    }
                case ECssTokenType.FunctionName:
                    {
                        throw new CssParserError("Encountered unexpected 'FunctionNameToken' in parsed list (Should have been turned into a 'CssFunction' token already!)");
                    }
                case ECssTokenType.Function:
                    {
                        CssFunction func = Stream.Consume<CssFunction>();
                        return CssPseudoClassSelector.Create_Function(func.Name, func.Arguments);
                    }
            }

            return null;
        }

        /// <summary>
        /// Consumes a Pseudo-Element item from the stream
        /// <para>Pseudo-Element formats:</para>
        /// <para>'::' <ident-token></para>
        /// <para>'::' <function-token> <any-value> ')'</para>
        /// </summary>
        static CssPseudoElementSelector Consume_Pseudo_Element_Selector(CssTokenStream Stream)
        {
            Stream.Consume(2);// Consume the '::' prefix
            switch (Stream.Next.Type)
            {
                case ECssTokenType.Ident:
                    {
                        return new CssPseudoElementSelector(Stream.Consume<IdentToken>().Value);
                    }
                case ECssTokenType.FunctionName:
                    {
                        throw new CssParserError("Encountered unexpected 'FunctionNameToken' in parsed list (Should have been turned into a 'CssFunction' token already!)");
                    }
                case ECssTokenType.Function:
                    {
                        CssFunction func = Stream.Consume<CssFunction>();
                        return new CssPseudoElementSelectorFunction(func.Name, func.Arguments);
                    }
            }

            return null;
        }
        #endregion
    }
}
