using Xunit;
using CssUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CssUI.Filters;

namespace CssUI.Tests
{
    public class StringCommonTests
    {
        [Theory()]
        [InlineData("ABC", "ABC", true)]
        [InlineData("ABC", "aBC", false)]
        [InlineData("ABC 123", "ABC 123", true)]
        [InlineData("ABC 123", "ABC 1234", false)]
        public void StrEqTest(string Left, string Right, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.StrEq(Left, Right));
        }

        [Theory()]
        [InlineData("ABC", "ABC", true)]
        [InlineData("ABC", "abc", true)]
        [InlineData("ABC", "aBC", true)]
        [InlineData("ABC", " aBC ", false)]
        [InlineData("ABC 123", "abc 123", true)]
        [InlineData("ABC 123", "ABC 1234", false)]
        public void StriEqTest(string Left, string Right, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.StriEq(Left, Right));
        }

        [Theory()]
        [InlineData("ABC ", ' ', true)]
        [InlineData("ABC 123", '1', true)]
        [InlineData("ABC", ' ', false)]
        [InlineData("ABC", '1', false)]
        public void ContainsCharTest(string Input, char Match, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.Contains(Input, Match));
        }

        [Theory()]
        [InlineData("BC", "A", false)]
        [InlineData("ABC 23", "1", false)]
        [InlineData("ABC 123", "BCD", false)]
        [InlineData("ABC", "A", true)]
        [InlineData("ABC 123", "1", true)]
        [InlineData("ABC 123", "ABC", true)]
        public void ContainsStringTest(string Input, string Match, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.Contains(Input, Match));
        }

        [Theory()]
        [InlineData("", false)]
        [InlineData("ABC", false)]
        [InlineData(" ", true)]
        [InlineData("ABC ", true)]
        [InlineData(" ABC", true)]
        [InlineData(" ABC ", true)]
        public void ContainsDataFilterTest(string Input, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.Contains(Input, FilterWhitespace.Instance));
        }

        [Theory()]
        [InlineData("", false)]
        [InlineData("ABC", false)]
        [InlineData(" ", true)]
        [InlineData("ABC ", true)]
        [InlineData(" ABC", true)]
        [InlineData(" ABC ", true)]
        public void ContainsTest(string Input, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.Contains(Input, UnicodeCommon.Is_Ascii_Whitespace));
        }

        [Theory()]
        [InlineData("ABC", "ABC", true)]
        [InlineData("ABC", "abc", false)]
        [InlineData("ABC", "123", false)]
        [InlineData("123", "123", true)]
        public void ContainsOnlyTest(string Input, string Match, bool Expected)
        {
            Assert.Equal(Expected, StringCommon.ContainsOnly(Input, Match));
        }

        [Theory()]
        [InlineData("A B CDE FG HI ", ' ', 5)]
        [InlineData("Ab|CdE,f.|HIi", '|', 2)]
        [InlineData("Ab|C.dE,f.|HIi.", '.', 3)]
        public void CountCharTest(string Input, char Search, int Expected)
        {
            Assert.Equal(Expected, StringCommon.Count(Input, Search));
        }

        [Theory()]
        [InlineData("A B\rCDE\nF\rG HI\r\n", 11)]
        [InlineData("Ab|CdE\nf\r|HI", 10)]
        [InlineData("Ab|C\rdE\nf.|HIi\r", 12)]
        public void CountDataFilterTest(string Input, int Expected)
        {
            Assert.Equal(Expected, StringCommon.Count(Input, FilterCRLF.Instance));
        }

        [Theory()]
        [InlineData("A B CDE FG HI ", 5)]
        [InlineData("Ab |CdE,f.|HIi ", 2)]
        [InlineData(" Ab |CdE,f.|HIi", 2)]
        [InlineData("Ab| C.dE, f.| HI.", 3)]
        public void CountPredicateTest(string Input, int Expected)
        {
            Assert.Equal(Expected, StringCommon.Count(Input, UnicodeCommon.Is_Ascii_Whitespace));
        }

        [Theory()]
        [InlineData("A,  B,  C  ", "A, B, C")]
        [InlineData(" A,  B, C", "A, B, C")]
        [InlineData(" A      B   C  ", "A B C")]
        [InlineData(" A \t \n\n\r B \t  C \n ", "A B C")]
        public void Strip_And_Collapse_WhitespaceTest(string Input, string Expected)
        {
            var Actual = StringCommon.Strip_And_Collapse_Whitespace(Input);
            Assert.Equal(Expected, Actual);
        }

        [Theory()]
        [InlineData(',', "one,2,three", "one", "2", "three")]
        public void ConcatCharTest(char Delimiter, string Expected, params string[] Inputs)
        {
            Assert.Equal(Expected, StringCommon.Concat(Delimiter, Inputs.Select(o => o.AsMemory())));
        }

        [Theory()]
        [InlineData(", ", "one, 2, three", "one", "2", "three")]
        public void ConcatSpanTest(string Delimiter, string Expected, params string[] Inputs)
        {
            Assert.Equal(Expected, StringCommon.Concat(Delimiter, Inputs.Select(o => o.AsMemory())));
        }

        [Theory()]
        [InlineData("one2three", "one", "2", "three")]
        public void ConcatNoDelimiterTest(string Expected, params string[] Inputs)
        {
            Assert.Equal(Expected, StringCommon.Concat(Inputs.Select(o => o.AsMemory())));
        }

        [Fact()]
        public void StrtokCharTest()
        {
            Assert.Equal(new string[]{ "A", "B", "C"}, StringCommon.Strtok(" A B C ", UnicodeCommon.CHAR_SPACE).Select(x => x.ToString()).ToArray());
            Assert.Equal(new string[]{ "-1", "2", "3 "}, StringCommon.Strtok("-1,2,3 ", UnicodeCommon.CHAR_COMMA).Select(x => x.ToString()).ToArray());
        }

        [Fact()]
        public void StrtokCharArrayTest()
        {
            Assert.Equal(new string[]{ "A", "B", "C"}, StringCommon.Strtok(", A ,B C, ", UnicodeCommon.CHAR_SPACE, UnicodeCommon.CHAR_COMMA).Select(x => x.ToString()).ToArray());
        }

        [Fact()]
        public void StrtokDataFilterTest()
        {
            Assert.Equal(new string[] { "A", "B", "C" }, StringCommon.Strtok(" A B C ", FilterWhitespace.Instance).Select(x => x.ToString()).ToArray());
            Assert.Equal(new string[] { "A", "B", "C" }, StringCommon.Strtok(" A\r\n B \tC \n\r ", FilterWhitespace.Instance).Select(x => x.ToString()).ToArray());
        }


        [Fact()]
        public void ReplaceCharTest()
        {
            Assert.Equal("A,B,C", StringCommon.Replace("A,  B, C  ", false, true, (' ', "")));
            Assert.Equal("A, B, C ", StringCommon.Replace("A,  B, C  ", false, true, (' ', " ")));
            Assert.Equal("A, B, C", StringCommon.Replace("A,  B, C  ", true, true, (' ', " ")));

            Assert.Equal("A,B,C", StringCommon.Replace(" A,  B, C", false, true, (' ', "")));
            Assert.Equal(" A, B, C", StringCommon.Replace(" A,  B, C", false, true, (' ', " ")));
            Assert.Equal("A, B, C", StringCommon.Replace(" A,  B, C", true, true, (' ', " ")));

            Assert.Equal("ABC", StringCommon.Replace(" A      B   C  ", false, true, (' ', "")));
            Assert.Equal(" A B C ", StringCommon.Replace(" A      B   C  ", false, true, (' ', " ")));
            Assert.Equal("A B C", StringCommon.Replace(" A      B   C  ", true, true, (' ', " ")));

            Assert.Equal("A,B,C", StringCommon.Replace("A,0B,C0", true, true, ('0', "")));
            Assert.Equal("A,11B,C", StringCommon.Replace("A,0B,C0", true, true, ('0', "11")));
            Assert.Equal("A,11B11,C", StringCommon.Replace("0A,0B0,C0", true, true, ('0', "11")));
        }


        [Theory()]
        [InlineData("A,B,C", " A, \r B,\n C  ", false, true, "")]
        [InlineData("A, B, C ", "A,\r  B, C  ", false, true, " ")]
        [InlineData("A, B, C", "\nA,  B, C  ", true, true, " ")]
        [InlineData("A,B,C", " A,  B, C", false, true, "")]
        [InlineData(" A, B, C", " A,  B, C", false, true, " ")]
        [InlineData("A, B, C", " \nA,  B, C", true, true, " ")]
        [InlineData("ABC", " A     \r B \n  C  ", false, true, "")]
        [InlineData(" A B C ", " A      B \n  C  ", false, true, " ")]
        [InlineData("A B C", " A   \r   B   C  ", true, true, " ")]
        [InlineData("A000B000C", " A    \t\n  B   C  ", true, true, "000")]
        public void ReplacePredicateTest(string Expected, string Input, bool Trim, bool Collapse, string Replacement)
        {
            string Actual = StringCommon.Replace(Input, Trim, Collapse, (UnicodeCommon.Is_Ascii_Whitespace, Replacement));
            Assert.Equal(Expected, Actual);
        }


        [Theory()]
        [InlineData("A,B,C", "A,  B, C  ", false, true, "")]
        [InlineData("A, B, C ", "A,  B, C  ", false, true, " ")]
        [InlineData("A, B, C", "A,  B, C  ", true, true, " ")]
        [InlineData("A,B,C", "A,  B, C", false, true, "")]
        [InlineData(" A, B, C", " A,  B, C", false, true, " ")]
        [InlineData("A, B, C", " A,  B, C  ", true, true, " ")]
        [InlineData("ABC", "A      B   C  ", false, true, "")]
        [InlineData("A B C ", "A      B   C  ", false, true, " ")]
        [InlineData("A B C", "A      B   C  ", true, true, " ")]
        [InlineData("A000B000C", "A      B   C  ", true, true, "000")]
        public void ReplaceDataFilterTest(string Expected, string Input, bool Trim, bool Collapse, string Replacement)
        {
            string Actual = StringCommon.Replace(Input, Trim, Collapse, (FilterWhitespace.Instance, Replacement));
            Assert.Equal(Expected, Actual);
        }

        [Theory()]
        [InlineData("abc", "ABC")]
        [InlineData(" abc", " ABC")]
        [InlineData("123", "123")]
        [InlineData("123 ", "123 ")]
        [InlineData("1a23bc", "1A23bC")]
        [InlineData(" 1a23bc ", " 1A23bC ")]
        public void TransformTest(string Expected, string Input)
        {
            string Actual = StringCommon.Transform(Input, UnicodeCommon.To_ASCII_Lower_Alpha);
            Assert.Equal(Expected, Actual);
        }
    }
}