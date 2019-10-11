using Xunit;
using System;

namespace CssUI.CSS.Parser.Tests
{
    public class CssTokenizerTests
    {

        [Fact()]
        public void ParseDeclerationBlockTest()
        {
            const string CssTestStr = @"@charset ""UTF-8"";
/*! Hello World this is a CSS comment */
html {
  font-family: sans-serif;
        }

";
            // Tokenize a CSS string and make sure it spits out the correct token sequence
            CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
            CssToken[] Expected = new CssToken[] { new AtToken("charset"), WhitespaceToken.Space, new StringToken("UTF-8"), SemicolonToken.Instance, WhitespaceToken.LFLF,
                WhitespaceToken.LFLF,
                new IdentToken("html"), WhitespaceToken.Space, BracketOpenToken.Instance, WhitespaceToken.LFLF,
                new IdentToken("font-family"), ColonToken.Instance, WhitespaceToken.Space, new IdentToken("sans-serif"), SemicolonToken.Instance, WhitespaceToken.LFLF,
                BracketCloseToken.Instance, WhitespaceToken.LFLF, EOFToken.Instance
            };

            var Engine = new Difference.DiffEngine<CssToken>();
            var diff = Engine.Compile(Expected, Actual);
            if (diff.Count > 1)
            {
                Engine.DisplayHTML(diff);
            }

            Assert.Equal(Expected, Actual);
        }

        [Fact()]
        public void ParseMutliSpecifierTest()
        {
            const string CssTestStr = @"a:active,
a:hover {
  outline: 0; }";
            // Tokenize a CSS string and make sure it spits out the correct token sequence
            CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
            CssToken[] Expected = new CssToken[] { new IdentToken("a"), ColonToken.Instance, new IdentToken("active"), CommaToken.Instance, WhitespaceToken.LFLF,
                new IdentToken("a"), ColonToken.Instance, new IdentToken("hover"), WhitespaceToken.Space, BracketOpenToken.Instance, WhitespaceToken.LFLF,
                new IdentToken("outline"), ColonToken.Instance, WhitespaceToken.Space, new NumberToken(ENumericTokenType.Integer, "0", 0), SemicolonToken.Instance, WhitespaceToken.Space, BracketCloseToken.Instance, EOFToken.Instance
            };

            var Engine = new Difference.DiffEngine<CssToken>();
            var diff = Engine.Compile(Expected, Actual);
            if (diff.Count > 1)
            {
                Engine.DisplayHTML(diff);
            }

            Assert.Equal(Expected, Actual);
        }

        [Fact()]
        public void ParseComplexTest()
        {
            const string CssTestStr = @"input[type=""checkbox""].filter-class-cb:checked + label.filter-class-lb, input[type=""checkbox""].filter-class-cb:active + label.filter-class-lb {}";
            // Tokenize a CSS string and make sure it spits out the correct token sequence
            CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
            CssToken[] Expected = new CssToken[] { new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
                new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new IdentToken("checked"), 
                WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
                new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), CommaToken.Instance, WhitespaceToken.Space,
                new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
                new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new IdentToken("active"), 
                WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
                new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), WhitespaceToken.Space, 
                BracketOpenToken.Instance, BracketCloseToken.Instance, 
                EOFToken.Instance
            };

            var Engine = new Difference.DiffEngine<CssToken>();
            var diff = Engine.Compile(Expected, Actual);
            if (diff.Count > 1)
            {
                Engine.DisplayHTML(diff);
            }

            Assert.Equal(Expected, Actual);
        }

        [Fact()]
        public void ParsePseudoTest()
        {
            const string CssTestStr = @"input[type=""checkbox""].filter-class-cb:checked + label.filter-class-lb[data-class-idx=""0""]::after {}";
            // Tokenize a CSS string and make sure it spits out the correct token sequence
            CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
            CssToken[] Expected = new CssToken[] { new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
                new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new IdentToken("checked"), WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
                new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), SqBracketOpenToken.Instance, new IdentToken("data-class-idx"), new DelimToken('='), new StringToken("0"), SqBracketCloseToken.Instance, 
                ColonToken.Instance, ColonToken.Instance, new IdentToken("after"), WhitespaceToken.Space, 
                BracketOpenToken.Instance, BracketCloseToken.Instance,
                EOFToken.Instance
            };

            var Engine = new Difference.DiffEngine<CssToken>();
            var diff = Engine.Compile(Expected, Actual);
            if (diff.Count > 1)
            {
                Engine.DisplayHTML(diff);
            }

            Assert.Equal(Expected, Actual);
        }

        [Fact()]
        public void ParsePseudoTest2()
        {
            const string CssTestStr = @"input[type=""checkbox""].filter-class-cb:not(:checked) + label.filter-class-lb[data-class-idx=""0""]::after {}";
            // Tokenize a CSS string and make sure it spits out the correct token sequence
            CssToken[] Actual = CssTokenizer.Parse(CssTestStr);
            CssToken[] Expected = new CssToken[] { new IdentToken("input"), SqBracketOpenToken.Instance, new IdentToken("type"), new DelimToken('='), new StringToken("checkbox"), SqBracketCloseToken.Instance,
                new DelimToken('.'), new IdentToken("filter-class-cb"), ColonToken.Instance, new FunctionNameToken("not"), ColonToken.Instance, new IdentToken("checked"), ParenthesisCloseToken.Instance,
                WhitespaceToken.Space, new DelimToken('+'), WhitespaceToken.Space,
                new IdentToken("label"), new DelimToken('.'), new IdentToken("filter-class-lb"), SqBracketOpenToken.Instance, new IdentToken("data-class-idx"), new DelimToken('='), new StringToken("0"), SqBracketCloseToken.Instance, 
                ColonToken.Instance, ColonToken.Instance, new IdentToken("after"), WhitespaceToken.Space, 
                BracketOpenToken.Instance, BracketCloseToken.Instance,
                EOFToken.Instance
            };

            var Engine = new Difference.DiffEngine<CssToken>();
            var diff = Engine.Compile(Expected, Actual);
            if (diff.Count > 1)
            {
                Engine.DisplayHTML(diff);
            }

            Assert.Equal(Expected, Actual);
        }

        [Theory()]
        [InlineData(-1L, "-1", ENumericTokenType.Integer), InlineData(0L, "0", ENumericTokenType.Integer), InlineData(1L, "1", ENumericTokenType.Integer)]
        [InlineData(-20000.0D, "-2.0E4", ENumericTokenType.Number), InlineData(-1.5D, "-1.5", ENumericTokenType.Number), InlineData(0.0D, "0.0", ENumericTokenType.Number), InlineData(1.0D, "1.0", ENumericTokenType.Number), InlineData(20000.0D, "2.0E4", ENumericTokenType.Number)]
        public void Consume_NumberTest(object expected, string input, ENumericTokenType tokenType)
        {
            var Stream = new DataConsumer<char>(input.AsMemory());
            CssTokenizer.Consume_Number(Stream, out ReadOnlyMemory<char> outResult, out object outNumber, out ENumericTokenType outTokenType);

            Assert.Equal(tokenType, outTokenType);
            Assert.Equal(expected, outNumber);
        }
    }
}