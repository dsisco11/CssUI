﻿using CssUI.CSS.Parser;
using CssUI.CSS.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static CssUI.UnicodeCommon;
using CssUI.CSS.Internal;
using System;

namespace CssUI.CSS.Selectors
{
    /// <summary>
    /// Parses low-level <see cref="CssComponent"/>s into higher-level <see cref="CSSSelectorComponent"/> objects
    /// </summary>
    public class SelectorParser
    {
        readonly DataConsumer<CssToken> Stream;

        public SelectorParser(ReadOnlySpan<char> SelectorString)
        {
            CssParser parser = new CssParser(SelectorString);
            LinkedList<CssToken> items = parser.Parse_ComponentValue_List();

            Stream = new DataConsumer<CssToken>(items.ToArray(), CssToken.EOF);
        }

        #region Pre-Parsing
        static DataConsumer<CssToken> Parse_Tokens(DataConsumer<CssToken> Stream)
        {
            List<CssToken> List = new List<CssToken>();
            CssToken Token;
            do
            {
                Token = Consume_Token(Stream);
                List.Add(Token);
            }
            while (Token.Type != ECssTokenType.EOF);
            return new DataConsumer<CssToken>(List.ToArray(), CssToken.EOF);
        }
        #endregion

        #region Pre-parse Consumption

        static CssToken Consume_Token(DataConsumer<CssToken> Stream)
        {
            if (Starts_Combinator(Stream.Peek(0)))
            {
                var o = Consume_Combinator(Stream);
                if (o == null) throw new CssParserException("Unable to consume combinator!");
                return o;
            }
            else if (Starts_QualifiedName(Stream.Peek(0), Stream.Peek(1), Stream.Peek(2)))
            {
                var o = Consume_QualifiedName(Stream);
                if (o == null) throw new CssParserException("Unable to consume qualified name!");
                return o;
            }
            else if (Starts_NamespacePrefix(Stream.Peek(0), Stream.Peek(1)))
            {
                var o = Consume_NamespacePrefix(Stream);
                if (o == null) throw new CssParserException("Unable to consume namespace prefix!");
                return o;
            }

            return Stream.Consume();
        }

        static NamespacePrefixToken Consume_NamespacePrefix(DataConsumer<CssToken> Stream)
        {
            string Name = null;

            CssToken Tok;
            Tok = Stream.Consume();

            if (Tok.Type == ECssTokenType.Ident)
            {
                Name = (Tok as IdentToken).Value;
                if (Stream.Next.Type != ECssTokenType.Column) throw new CssParserException("Namespace prefixes must be followed by a '|' token!");

                Stream.Consume();// Consume the next '|' token
            }
            else if (Tok.Type == ECssTokenType.Delim && (Tok as DelimToken).Value == '*')
            {
                Name = "*";// Any namespace
                if (Stream.Next.Type != ECssTokenType.Column) throw new CssParserException("Namespace prefixes must be followed by a '|' token!");

                Stream.Consume();// Consume the next '|' token
            }
            else if (Tok.Type == ECssTokenType.Column)
            {
                Name = null;// no namespace
            }

            return new NamespacePrefixToken(Name);
        }

        static QualifiedNameToken Consume_QualifiedName(DataConsumer<CssToken> Stream)
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

        static CombinatorToken Consume_Combinator(DataConsumer<CssToken> Stream)
        {
            var bHasWhitespace = Stream.Consume_While(o => o.Type == ECssTokenType.Whitespace);
            string Value = string.Empty;
            if (Stream.Next.Type == ECssTokenType.Delim)
            {
                Value = string.Concat((Stream.Consume() as DelimToken).Value);
                if (Stream.Next.Type == ECssTokenType.Delim)
                {
                    char dv = (Stream.Next as DelimToken).Value;
                    if (dv == CHAR_RIGHT_CHEVRON)
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

            if (bHasWhitespace)// We consumed whitespace so at this point the only option is that our combinator was intended to be that whitespace
            {
                return new CombinatorToken(">>");
            }

            return null;
        }
        #endregion


        #region Parsing
        public IEnumerable<ComplexSelector> Parse_Selector_List()
        {
            return Consume_Selector_List(Stream);
        }

        public ComplexSelector Parse_Single_Selector()
        {
            return Consume_Single_Selector(Stream);
        }
        #endregion

        #region Token Checks
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Is_Char(CssToken A, char CH)
        {
            return (A.Type == ECssTokenType.Delim && (A as DelimToken).Value == CH);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_ID_Selector(CssToken A)
        {
            return (A.Type == ECssTokenType.Hash && (A as HashToken).HashType == EHashTokenType.ID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_Class_Selector(CssToken A, CssToken B)
        {
            return (Is_Char(A, '.') && B.Type == ECssTokenType.Ident);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_Attribute_Selector(CssToken A, CssToken B)
        {
            return (A.Type == ECssTokenType.SqBracket_Open && B.Type == ECssTokenType.QualifiedName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_Pseudo_Class_Selector(CssToken A, CssToken B)
        {
            return (A.Type == ECssTokenType.Colon && (B.Type == ECssTokenType.Ident || B.Type == ECssTokenType.FunctionName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_Pseudo_Element_Selector(CssToken A, CssToken B, CssToken C)
        {
            return (A.Type == ECssTokenType.Colon && B.Type == ECssTokenType.Colon && (C.Type == ECssTokenType.Ident || C.Type == ECssTokenType.FunctionName));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_Universal_Selector(CssToken A, CssToken B)
        {
            return (Is_Char(A, '*') || A.Type == ECssTokenType.NamespacePrefix && Is_Char(B, '*'));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_Type_Selector(CssToken A, CssToken B, CssToken C)
        {
            if (Starts_NamespacePrefix(A, B))
            {
                return C.Type == ECssTokenType.Ident;
            }
            return (A.Type == ECssTokenType.Ident);
            //return (A.Type == ECssTokenType.QualifiedName || A.Type == ECssTokenType.NamespacePrefix && B.Type == ECssTokenType.QualifiedName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_NamespacePrefix(CssToken A, CssToken B)
        {
            if (B.Type == ECssTokenType.Column)
            {
                if ((A.Type == ECssTokenType.QualifiedName || Is_Char(A, '*'))) return true;
                if ((A.Type == ECssTokenType.Ident || Is_Char(A, '*'))) return true;
            }

            return (A.Type == ECssTokenType.Column);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Starts_QualifiedName(CssToken A, CssToken B, CssToken C)
        {
            if (Starts_NamespacePrefix(A, B) && C.Type == ECssTokenType.Ident) return true;
            return (A.Type == ECssTokenType.Ident);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Consumes a list of complex selectors
        /// </summary>
        public static ComplexSelector Consume_Single_Selector(DataConsumer<CssToken> Stream)
        {
            return Consume_Complex_Selector(Stream);
        }

        /// <summary>
        /// Consumes a list of complex selectors
        /// </summary>
        public static IEnumerable<ComplexSelector> Consume_Selector_List(DataConsumer<CssToken> Stream)
        {
            ComplexSelector Selector;
            LinkedList<ComplexSelector> List = new LinkedList<ComplexSelector>();
            do
            {
                Selector = Consume_Complex_Selector(Stream);
                if (Selector != null)
                {
                    List.AddLast(Selector);
                }

                switch (Stream.Next.Type)
                {
                    case ECssTokenType.Comma:
                        Stream.Consume();
                        break;
                    case ECssTokenType.EOF:
                    default:
                        Selector = null;
                        break;
                }
            }
            while (Selector != null);

            return List;
        }

        /// <summary>
        /// Consumes a sequence of <see cref="RelativeSelector"/> consisting of compound selectors and their combinators (if available)
        /// </summary>
        /// <returns></returns>
        private static ComplexSelector Consume_Complex_Selector(DataConsumer<CssToken> Stream)
        {
            RelativeSelector Selector;
            LinkedList<RelativeSelector> selectorList = new LinkedList<RelativeSelector>();
            do
            {
                Selector = Consume_Relative_Selector(Stream);
                if (Selector != null)
                {
                    selectorList.AddLast(Selector);
                }
            }
            while (Selector != null);

            return new ComplexSelector(selectorList);
        }

        /// <summary>
        /// Consumes a single <see cref="CompoundSelector"/> and it's <see cref="ESelectorCombinator"/> (if available)
        /// </summary>
        /// <returns></returns>
        private static RelativeSelector Consume_Relative_Selector(DataConsumer<CssToken> Stream)
        {
            Stream.Consume_While(tok => tok.Type == ECssTokenType.Whitespace);// Consume all of the prefixing whitespace
            CompoundSelector Compound = Consume_Compound_Selector(Stream);

            if (Compound == null)
                return null;

            if (!Starts_Combinator(Stream.Next))
            {
                return new RelativeSelector(ESelectorCombinator.None, Compound);
            }
            else
            {
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
                            Combinator = ESelectorCombinator.Sibling_Subsequent;
                            break;
                        default:
                            throw new CssParserException(string.Concat("Unrecognized Combinator(", Comb.Value, ")"));
                    }
                }

                return new RelativeSelector(Combinator, Compound);
            }
        }

        /// <summary>
        /// Consumes a single <see cref="CompoundSelector"/>, which is a comprised of multiple <see cref="SimpleSelector"/>s
        /// </summary>
        /// <returns></returns>
        private static CompoundSelector Consume_Compound_Selector(DataConsumer<CssToken> Stream)
        {
            SimpleSelector Simple;
            var selectorList = new LinkedList<SimpleSelector>();
            do
            {
                if (!Starts_Simple_Selector(Stream.Next, Stream.NextNext, Stream.NextNextNext))
                {
                    break;
                }

                Simple = Consume_Simple_Selector(Stream);
                if (Simple != null)
                {
                    selectorList.AddLast(Simple);
                }
            }
            while (Simple != null);

            return new CompoundSelector(selectorList);
        }

        private static SimpleSelector Consume_Simple_Selector(DataConsumer<CssToken> Stream)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IDSelector Consume_ID_Selector(DataConsumer<CssToken> Stream)
        {
            HashToken Hash = (Stream.Consume() as HashToken);
            if (Hash.HashType != EHashTokenType.ID) throw new CssParserException("Invalid Hash token, hash-type is not ID!");
            return new IDSelector(Hash.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static UniversalSelector Consume_Universal_Selector(DataConsumer<CssToken> Stream)
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

            return new UniversalSelector();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static TypeSelector Consume_Type_Selector(DataConsumer<CssToken> Stream)
        {
            if (Starts_NamespacePrefix(Stream.Next, Stream.NextNext))
            {
                NamespacePrefixToken Namespace = Consume_NamespacePrefix(Stream);
                return new TypeSelector(Namespace, Stream.Consume<IdentToken>().Value);
            }
            
            return new TypeSelector(Stream.Consume<IdentToken>().Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ClassSelector Consume_Class_Selector(DataConsumer<CssToken> Stream)
        {
            Stream.Consume();// Consume the '.' prefixing the classname
            IdentToken Ident = Stream.Consume<IdentToken>();

            return new ClassSelector(Ident.Value);
        }

        /// <summary>
        /// Consumes an attribute selector item from the stream
        /// <para>Attribute-Selector formats:</para>
        /// <para>'[' <qualified-name> ']'</para>
        /// <para>'[' <qualified-name> <attr-matcher> [ <string-token> | <ident-token> ] <attr-modifier>? ']'</para>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static AttributeSelector Consume_Attribute_Selector(DataConsumer<CssToken> Stream)
        {
            Stream.Consume();// Consume the '[' prefix

            NamespacePrefixToken NS = null;
            if (Starts_NamespacePrefix(Stream.Next, Stream.NextNext)) NS = Consume_NamespacePrefix(Stream);

            //QualifiedNameToken attrName = Stream.Consume<QualifiedNameToken>();
            IdentToken attrName = Stream.Consume<IdentToken>();
            CssToken Tok = Stream.Consume();
            if (Tok.Type == ECssTokenType.SqBracket_Close)
            {
                return new AttributeSelector(NS, attrName.Value);
            }

            CssToken OperatorToken = Tok;
            CssToken value = Stream.Consume();
            if (value.Type == ECssTokenType.SqBracket_Close) return null;// Parse error
            if (Stream.Next.Type != ECssTokenType.SqBracket_Close) return null;// Parse error
            Stream.Consume();// Consume the closing bracket

            if (value.Type == ECssTokenType.String)
            {
                return new AttributeSelector(NS, attrName.Value, OperatorToken, (value as StringToken).Value);
            }
            else if (value.Type == ECssTokenType.Ident)
            {
                return new AttributeSelector(NS, attrName.Value, OperatorToken, (value as IdentToken).Value);
            }

            return null;// Parse error
        }

        /// <summary>
        /// Consumes a pseudo-class item from the stream
        /// <para>Pseudo-Class formats:</para>
        /// <para>':' <ident-token></para>
        /// <para>':' <function-token> <any-value> ')'</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static PseudoClassSelector Consume_Pseudo_Class_Selector(DataConsumer<CssToken> Stream)
        {
            Stream.Consume();// Consume the ':' prefix
            switch (Stream.Next.Type)
            {
                case ECssTokenType.Ident:
                    {
                        return new PseudoClassSelector(Stream.Consume<IdentToken>().Value);
                    }
                case ECssTokenType.FunctionName:
                    {
                        throw new CssParserException("Encountered unexpected 'FunctionNameToken' in parsed list (Should have been turned into a 'CssFunction' token already!)");
                    }
                case ECssTokenType.Function:
                    {
                        CssFunction func = Stream.Consume<CssFunction>();
                        return PseudoClassSelector.Create_Function(func.Name, func.Arguments.ToArray());
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static PseudoElementSelector Consume_Pseudo_Element_Selector(DataConsumer<CssToken> Stream)
        {
            Stream.Consume(2);// Consume the '::' prefix
            switch (Stream.Next.Type)
            {
                case ECssTokenType.Ident:
                    {
                        return new PseudoElementSelector(Stream.Consume<IdentToken>().Value);
                    }
                case ECssTokenType.FunctionName:
                    {
                        throw new CssParserException("Encountered unexpected 'FunctionNameToken' in parsed list (Should have been turned into a 'CssFunction' token already!)");
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
