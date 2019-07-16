using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS.Parser
{
    /// <summary>
    /// Handles parsing CSS text
    /// </summary>
    public class CssTokenizer
    {
        #region Defines
        public const char TOKEN_EOF = '\0';
        public const char TOKEN_MAX = char.MaxValue;
        public const char TOKEN_TAB = (char)'\t';
        public const char TOKEN_SPACE = (char)' ';
        public const char TOKEN_NEWLINE = (char)'\n';
        public const char TOKEN_QUOTATION_MARK = (char)'\u0022';// "
        public const char TOKEN_REPLACEMENT_CHAR = (char)'�';// U+FFFD
        public const char TOKEN_APOSTRAPHE = (char)'\u0027';// '
        public const char TOKEN_FULL_STOP = (char)'.';// .
        public const char TOKEN_SOLIDUS = (char)'/';
        public const char TOKEN_REVERSE_SOLIDUS = (char)'\\';
        #endregion

        #region Properties
        /// <summary>
        /// Current reading offset within the text
        /// </summary>
        int ReadPos = 0;
        /// <summary>
        /// The text this tokenizer is interpreting
        /// </summary>
        readonly string Text = null;
        public readonly CssToken[] Tokens = null;
        #endregion

        #region Operators
        public CssToken this[int index]
        {
            get
            {
                return Tokens[index];
            }
        }
        #endregion

        #region Constructors
        public CssTokenizer(string Text)
        {
            this.Text = CssInput.PreProcess(Text);
            List<CssToken> Tokens = new List<CssToken>();

            CssToken last;
            do
            {
                last = Consume_Token();
                Tokens.Add(last);
            }
            while (last.Type != ECssTokenType.EOF);

            this.Tokens = Tokens.ToArray();
        }
        #endregion

        #region Stream Management
        /// <summary>
        /// Returns the character at +Offset from the current read position within the text
        /// </summary>
        /// <param name="Offset">Distance from the current read position at which to peek</param>
        /// <returns></returns>
        private char Peek(int Offset = 0)
        {
            int i = (ReadPos + Offset);
            if (i >= Text.Length || i < 0) return TOKEN_EOF;
            return Text[i];
        }

        /// <summary>
        /// Reads the current character from the text and progresses the current reading position
        /// </summary>
        private char Consume()
        {
            int EndPos = (ReadPos + 1);
            if (ReadPos >= Text.Length) return TOKEN_EOF;

            char retVal = Text[ReadPos];
            ReadPos += 1;

            return retVal;
        }

        /// <summary>
        /// Reads the specified number of characters from the text and progresses the current reading position
        /// </summary>
        /// <param name="Count">Number of characters to consume</param>
        private string Consume(int Count = 1)
        {
            int NextPos = (ReadPos + Count);
            if (NextPos >= Text.Length) NextPos = Text.Length;// No this isnt a mistake, we DO want it to be the indice of the text length.

            string retVal = Text.Substring(ReadPos, (NextPos - ReadPos));
            ReadPos = NextPos;

            return retVal;
        }

        /// <summary>
        /// Pushes the given number of characters back onto the front of the "stream"
        /// </summary>
        /// <param name="Count"></param>
        private void Reconsume(int Count = 1)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#reconsume-the-current-input-code-point
            ReadPos -= Count;
            if (ReadPos < 0) ReadPos = 0;
        }

        /// <summary>
        /// Seeks forward until a character doesn't match the given predicate, then stops and returns the number of matching characters.
        /// </summary>
        /// <returns>Count matching characters</returns>
        private int Seek(Func<char, bool> Predicate)
        {
            int count = 0;
            while ( Predicate( Peek( count ) ) )
            {
                //count++;
                if ((ReadPos + count + 1) > Text.Length) break;
                count++;
            }

            return count;
        }

        /// <summary>
        /// Seeks forward until a character doesn't match the given predicate, then stops and returns the number of matching characters.
        /// </summary>
        /// <returns>Count matching characters</returns>
        private int Seek(Func<char, bool> Predicate, int Limit)
        {
            int count = 0;
            while ( Predicate( Peek( count ) ) )
            {
                //count++;
                //if (count == Limit) break;
                if ((ReadPos + count + 1) > Text.Length) break;
                if (count == Limit) break;

                count++;
            }

            return count;
        }
        #endregion

        #region Parsing
        /// <summary>
        /// Consumes an escaped character from the current reading position
        /// </summary>
        /// <returns></returns>
        private char Consume_Escaped()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-an-escaped-code-point0
            char tok = Consume();
            if (Is_Hex_Digit(tok))
            {// Consume as many hex digits as possible but no more then 5 (for a total of 6)
                string buf = string.Concat(tok);
                int cnt = Seek(Is_Hex_Digit, 5);
                buf += Consume(cnt);
                if (Is_Whitespace(Peek())) Consume();

                int hex = Convert.ToInt32(buf, 16);
                char hexC = (char)hex;

                if (hex <= 0 || hex > TOKEN_MAX)
                    return TOKEN_REPLACEMENT_CHAR;
                else if (Is_Surrogate(hexC))
                    return TOKEN_REPLACEMENT_CHAR;

                return hexC;
            }
            else if (tok == TOKEN_EOF)
                return TOKEN_REPLACEMENT_CHAR;
            else
                return tok;
        }

        /// <summary>
        /// Consumes and returns a string token
        /// </summary>
        /// <param name="EndChar">Character which indicates the end of a string</param>
        /// <returns>StringToken or BadStringToken</returns>
        private CssToken Consume_String_Token(char EndChar)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-string-token0
            string Buf = "";

            char tok;
            do
            {
                tok = Consume();
                if (tok == EndChar)
                    return new StringToken(Buf);

                switch (tok)
                {
                    case TOKEN_EOF:
                        return new StringToken(Buf);
                    case TOKEN_NEWLINE:
                        {
                            Reconsume();
                            return new BadStringToken(Buf);
                        }
                    case TOKEN_REVERSE_SOLIDUS:
                        {
                            char next = Peek();
                            switch (next)
                            {
                                case TOKEN_EOF:
                                    {
                                        continue;
                                    }
                                case TOKEN_NEWLINE:
                                    {
                                        Consume();
                                        break;
                                    }
                                default:
                                    {
                                        if (Is_Valid_Escape(tok, next))
                                        {
                                            Buf += Consume_Escaped();
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    default:
                        {
                            Buf += tok;
                            break;
                        }
                }

            }
            while (tok != TOKEN_EOF);

            return new StringToken(Buf);
        }

        double Convert_String_To_Number(string Str)
        {
            double S = 1;//Sign
            double T = 1;// Exponent Sign
            double I = 0;// Integer
            double F = 0;// Fraction
            double D = 0;// Number of fraction digits
            double E = 0;// Exponent

            string Sign = "";
            string Integer = "";
            string Decimal = "";
            string Fraction = "";
            string Exponent_Indicator = "";
            string Exponent_Sign = "";
            string Exponent = "";

            StringTokenizer tok = new StringTokenizer(Str);
            
            if (Is_Plus_Or_Minus(tok.Peek()))
            {
                Sign += tok.Consume();
                if (Sign[0] == '-') S = -1;
            }

            while (Is_Digit(tok.Peek()))
            {
                Integer += tok.Consume();
            }
            if (Integer.Length > 0)
            {
                I = Convert.ToInt32(Integer, 10);
            }

            if (tok.Peek() == TOKEN_FULL_STOP) Decimal += tok.Consume();

            while (Is_Digit(tok.Peek()))
            {
                Fraction += tok.Consume();
            }
            if (Fraction.Length > 0)
            {
                F = Convert.ToInt32(Fraction, 10);
                D = Fraction.Length;
            }

            char c = tok.Peek();
            if (c == 'E' || c == 'e')
            {
                Exponent_Indicator += tok.Consume();
            }
            
            if (Is_Plus_Or_Minus(tok.Peek()))
            {
                Exponent_Sign += tok.Consume();
                if (Exponent_Sign[0] == '-') T = -1;
            }

            while (Is_Digit(tok.Peek()))
            {
                Exponent += tok.Consume();
            }
            if (Exponent.Length > 0)
            {
                E = Convert.ToInt32(Exponent, 10);
            }

            return S * (I + (F * Math.Pow(10, -D))) * Math.Pow(10, T*E);

        }

        void Consume_Number(out string Result, out dynamic Number, out ENumericTokenType Type)
        {/* Docs:  https://www.w3.org/TR/css-syntax-3/#consume-a-number0 */

            Result = "";
            Type = ENumericTokenType.Integer;

            char tok = Peek();
            if (tok == '+' || tok == '-')
            {
                Result += Consume();
            }

            while (Is_Digit(Peek()))
            {
                Result += Consume();
            }

            if (Peek(0) == TOKEN_FULL_STOP && Is_Digit(Peek(1)))
            {
                Type = ENumericTokenType.Number;
                Result += Consume(2);
                while (Is_Digit(Peek()))
                {
                    Result += Consume();
                }
            }

            tok = Peek();
            if ((tok == 'E' || tok == 'e'))
            {
                Type = ENumericTokenType.Number;
                Result += Consume();
                if (Is_Plus_Or_Minus(Peek()) && Is_Digit(Peek(1)))
                {
                    Result += Consume(2);
                }

                while (Is_Digit(Peek()))
                {
                    Result += Consume();
                }
            }

            double num = Convert_String_To_Number(Result);

            if (Type == ENumericTokenType.Integer) Number = (int)num;
            else Number = num;
        }

        /// <summary>
        /// Consumes and returns a string token
        /// </summary>
        /// <param name="EndChar">Character which indicates the end of a string</param>
        /// <returns>StringToken or BadStringToken</returns>
        private CssToken Consume_Numeric_Token()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-numeric-token0
            string nStr = "";
            ENumericTokenType nType;
            dynamic N;
            Consume_Number(out nStr, out N, out nType);
            
            if (Is_Identifier_Start(Peek(0), Peek(1), Peek(2)))
            {
                string nUnit = Consume_Name();
                return new DimensionToken(nType, nStr, N, nUnit);
            }

            if (Peek() == '%')
            {
                Consume();
                return new PercentageToken(nStr, (double)N);
            }

            return new NumberToken(nType, nStr, N);
        }

        void Consume_Bad_Url_Remnants()
        {
            char tok;
            do
            {
                tok = Consume();
                if (tok == ')' || tok == TOKEN_EOF)
                {
                    return;
                }
                else if (Is_Valid_Escape(tok, Peek()))
                {
                    Consume_Escaped();
                }
            }
            while (tok != TOKEN_EOF);
        }

        UrlToken Consume_Url_Token()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-url-token0
            string Result = "";
            Consume_Whitespace();
            if (Peek() == TOKEN_EOF)
            {
                return new UrlToken(Result);
            }
            else if (Peek() == TOKEN_QUOTATION_MARK || Peek() == TOKEN_APOSTRAPHE)
            {
                char c = Consume();
                CssToken stok = Consume_String_Token(c);
                if (stok.Type == ECssTokenType.Bad_String)
                {
                    Consume_Bad_Url_Remnants();
                    return new BadUrlToken();
                }

                Result = ((StringToken)stok).Value;
                Consume_Whitespace();
                if (Peek() == ')' || Peek() == TOKEN_EOF)
                {
                    Consume();
                    return new UrlToken(Result);
                }

                Consume_Bad_Url_Remnants();
                return new BadUrlToken();
            }

            char tok;
            do
            {
                tok = Consume();
                if (tok == ')' || tok == TOKEN_EOF)
                    return new UrlToken(Result);
                else if (Is_Whitespace(tok))
                    Consume_Whitespace();
                else if (tok == TOKEN_QUOTATION_MARK || tok == TOKEN_APOSTRAPHE || tok == '(' || Is_NonPrintable(tok))
                {
                    Consume_Bad_Url_Remnants();
                    return new BadUrlToken();
                }
                else if (tok == TOKEN_REVERSE_SOLIDUS)
                {
                    if (Is_Valid_Escape(tok, Peek()))
                    {
                        Result += Consume_Escaped();
                    }
                    else
                    {
                        Consume_Bad_Url_Remnants();
                        return new BadUrlToken();
                    }
                }
                else
                {
                    Result += tok;
                }

            }
            while (tok != TOKEN_EOF);

            return new BadUrlToken();
        }

        CssToken Consume_Ident_Like_Token()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-an-ident-like-token0
            string Name = Consume_Name();

            if (string.Compare(Name.ToLower(), "url") == 0 && Peek() == '(')
            {
                Consume();
                return Consume_Url_Token();
            }
            else if (Peek() == '(')
            {
                Consume();
                return new FunctionNameToken(Name);
            }

            return new IdentToken(Name);
        }

        UnicodeRangeToken Consume_Unicode_Range_Token()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-unicode-range-token0
            string Digits = "";
            
            Digits += Consume(Seek(ch => (Is_Hex_Digit(ch) || ch == '?'), 6));// Consume hex and '?' digits up to a maximum of 6
            if (Digits.Contains('?'))
            {
                int Start = Convert.ToInt32(Digits.Replace('?', '0'), 16);
                int End = Convert.ToInt32(Digits.Replace('?', 'F'), 16);
                return new UnicodeRangeToken(Start, End);
            }
            
            int StartRange = Convert.ToInt32(Digits, 16);
            int EndRange = StartRange;
            if (Peek(0) == '-' && Is_Hex_Digit(Peek(1)))
            {
                Consume();

                string endDigits = "";
                endDigits += Consume(Seek(Is_Hex_Digit, 6));// Consume hex digits up to a maximum of 6
                EndRange = Convert.ToInt32(endDigits, 16);
            }

            return new UnicodeRangeToken(StartRange, EndRange);
        }

        void Consume_Whitespace()
        {
            while (Is_Whitespace(Peek()))
            {
                Consume();
            }
        }

        string Consume_Name()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-name0
            string Result = "";
            char tok = TOKEN_EOF;
            do
            {
                tok = Consume();
                if (Is_Name_Char(tok))
                {
                    Result += tok;
                }
                else if (Is_Valid_Escape(tok, Peek()))
                {
                    Result += Consume_Escaped();
                }
                else
                {
                    break;
                }

            }
            while (tok != TOKEN_EOF);

            return Result;
        }
        #endregion

        #region Character Identification
        bool Is_Identifier_Start(char A, char B, char C)
        {
            if (A == '-')
            {
                return (Is_Name_Start_Char(B) || Is_Valid_Escape(B, C));
            }
            else if (Is_Name_Start_Char(A))
            {
                return true;
            }
            else if (A == TOKEN_REVERSE_SOLIDUS)
            {
                return Is_Valid_Escape(A, B);
            }

            return false;
        }

        bool Is_Number_Start(char A, char B, char C)
        {
            if (A == '+' || A == '-')
            {
                if (Is_Digit(B)) return true;
                if (B == TOKEN_FULL_STOP && Is_Digit(C)) return true;
                return false;
            }
            else if (A == TOKEN_FULL_STOP)
            {
                return Is_Digit(B);
            }
            else if (Is_Digit(A)) return true;

            return false;
        }

        bool Is_Name_Char(char token)
        {
            if ((token >= 'A' && token <= 'Z') || (token >= 'a' && token <= 'z')) return true;
            if (token >= '\u0080') return true;
            if (token == '_') return true;
            if (token >= '0' && token <= '9') return true;
            if (token == '-') return true;

            return false;
        }

        bool Is_Name_Start_Char(char token)
        {
            if ((token >= 'A' && token <= 'Z') || (token >= 'a' && token <= 'z')) return true;
            if (token >= '\u0080') return true;
            if (token == '_') return true;

            return false;
        }

        bool Is_NonPrintable(char token)
        {
            if (token >= '\0' && token <= '\u0008') return true;
            if (token == '\t') return true;
            if (token >= '\u000E' && token <= '\u001F') return true;
            if (token == '\u007F') return true;

            return false;
        }

        bool Is_NonAscii(char token)
        {
            return (token >= '\u0080');
        }

        bool Is_Plus_Or_Minus(char token)
        {
            return (token == '+' || token == '-');
        }

        bool Is_Digit(char token)
        {
            return (token >= '0' && token <= '9');
        }

        bool Is_Letter(char token)
        {
            return ((token >= 'A' && token <= 'F') || (token >= 'a' && token <= 'f'));
        }

        bool Is_Valid_Escape(char A, char B)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#check-if-two-code-points-are-a-valid-escape
            if (A != TOKEN_REVERSE_SOLIDUS) return false;
            else if (B == TOKEN_NEWLINE) return false;
            
            return true;
        }

        bool Is_Surrogate(char token)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#surrogate-code-point
            return (token >= '\uD800' && token <= '\uDFFF');
        }

        bool Is_Hex_Digit(char token)
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#hex-digit
            return ((token >= '0' && token <= '9') || (token >= 'A' && token <= 'F') || (token >= 'a' && token <= 'f'));
        }

        bool Is_Whitespace(char token)
        {
            switch (token)
            {
                case TOKEN_SPACE:
                case TOKEN_TAB:
                case TOKEN_NEWLINE:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        /// <summary>
        /// Consumes and returns the next Token
        /// </summary>
        /// <returns></returns>
        private CssToken Consume_Token()
        {// SEE:  https://www.w3.org/TR/css-syntax-3/#consume-a-token0
            char token = Consume();

            if (Is_Whitespace(token))
            {// Consume as much whitepace as possible
                int cnt = Seek(Is_Whitespace);
                string Value = Consume(cnt);
                return new WhitespaceToken(Value);
            }
            else if (token == TOKEN_QUOTATION_MARK)
            {
                return Consume_String_Token(TOKEN_QUOTATION_MARK);
            }
            else if (token == '#')
            {
                char n1 = Peek(0);
                char n2 = Peek(1);
                if (Is_Name_Char(Peek()) || Is_Valid_Escape(Peek(), Peek(1)))
                {
                    char n3 = Peek(2);
                    if (Is_Identifier_Start(n1, n2, n3))
                    {
                        return new HashToken(EHashTokenType.ID, Consume_Name());
                    }
                    else
                    {
                        return new HashToken(EHashTokenType.Unrestricted, Consume_Name());
                    }
                }
                else
                {
                    return new DelimToken(token);
                }
            }
            else if (token == '$')
            {
                if (Peek() == '=')
                {
                    Consume();
                    return new SuffixMatchToken();
                }

                return new DelimToken(token);
            }
            else if (token == TOKEN_APOSTRAPHE)
            {
                return Consume_String_Token(TOKEN_APOSTRAPHE);
            }
            else if (token == '(')
            {
                return new ParenthesisOpenToken();
            }
            else if (token == ')')
            {
                return new ParenthesisCloseToken();
            }
            else if (token == '*')
            {
                if (Peek() == '=')
                {
                    Consume();
                    return new SubstringMatchToken();
                }
                return new DelimToken(token);
            }
            else if (token == '+')
            {
                if (Is_Number_Start(token, Peek(0), Peek(1)))
                {
                    Reconsume();
                    return Consume_Numeric_Token();
                }

                return new DelimToken(token);
            }
            else if (token == ',')
            {
                return new CommaToken();
            }
            else if (token == '-')
            {
                if (Is_Number_Start(token, Peek(0), Peek(1)))
                {
                    Reconsume();
                    return Consume_Numeric_Token();
                }
                else if (Is_Identifier_Start(token, Peek(0), Peek(1)))
                {
                    Reconsume();
                    return Consume_Ident_Like_Token();
                }
                else if (Peek(0) == '-' && Peek(1) == '>')
                {
                    Consume(2);
                    return new CdcToken();
                }

                return new DelimToken(token);
            }
            else if (token == '.')
            {
                if (Is_Number_Start(token, Peek(0), Peek(1)))
                {
                    Reconsume();
                    return Consume_Numeric_Token();
                }

                return new DelimToken(token);
            }
            else if (token == TOKEN_SOLIDUS)
            {
                if (Peek() == '*')
                {
                    Consume();
                    char c;
                    do
                    {
                        c = Consume();
                        if (c == TOKEN_EOF) break;
                        else if (c == '*' && Peek() == TOKEN_SOLIDUS)
                        {
                            return Consume_Token();
                        }
                    }
                    while (Peek() != TOKEN_EOF);

                    return new DelimToken(token);
                }
            }
            else if (token == ':')
            {
                return new ColonToken();
            }
            else if (token == ';')
            {
                return new SemicolonToken();
            }
            else if (token == '<')
            {
                if (Peek(0) == '!' && Peek(1) == '-' && Peek(2) == '-')
                {
                    Consume(3);
                    return new CdoToken();
                }
                return new DelimToken(token);
            }
            else if (token == '@')
            {
                if (Is_Identifier_Start(Peek(0), Peek(1), Peek(2)))
                {
                    string Name = Consume_Name();
                    return new AtToken(Name);
                }
                return new DelimToken(token);
            }
            else if (token == '[')
            {
                return new SqBracketOpenToken();
            }
            else if (token == TOKEN_REVERSE_SOLIDUS)
            {
                if (Is_Valid_Escape(token, Peek()))
                {
                    Reconsume();
                    return Consume_Ident_Like_Token();
                }

                return new DelimToken(token);
            }
            else if (token == ']')
            {
                return new SqBracketCloseToken();
            }
            else if (token == '^')
            {
                if (Peek() == '=')
                {
                    Consume();
                    return new PrefixMatchToken();
                }
                return new DelimToken(token);
            }
            else if (token == '{')
            {
                return new BracketOpenToken();
            }
            else if (token == '}')
            {
                return new BracketCloseToken();
            }
            else if (Is_Digit(token))
            {
                Reconsume();
                return Consume_Numeric_Token();
            }
            else if (token == 'U' || token == 'u')
            {
                if (Peek(0) == '+' && (Is_Hex_Digit(Peek(1)) || Peek(1) == '?'))
                {
                    Consume(1);
                    return Consume_Unicode_Range_Token();
                }

                Reconsume();
                return Consume_Ident_Like_Token();
            }
            else if (Is_Name_Start_Char(token))
            {
                Reconsume();
                return Consume_Ident_Like_Token();
            }
            else if (token == '|')
            {
                if (Peek() == '=')
                {
                    Consume();
                    return new DashMatchToken();
                }
                else if (Peek() == '|')
                {
                    Consume();
                    return new ColumnToken();
                }
                return new DelimToken(token);
            }
            else if (token == '~')
            {
                if (Peek() == '=')
                {
                    Consume();
                    return new IncludeMatchToken();
                }
                return new DelimToken(token);
            }
            else if (token == TOKEN_EOF)
            {
                return new EOFToken();
            }


            return new DelimToken(token);
        }


    }
}
