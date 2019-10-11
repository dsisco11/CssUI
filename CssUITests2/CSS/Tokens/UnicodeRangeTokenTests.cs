using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class UnicodeRangeTokenTests
    {

        [Theory()]
        [InlineData(0x7F, 0xFF), InlineData(0xFFFD, 0x6FFFD)]
        public void EqualsTest(int Start, int End)
        {
            Assert.Equal(new UnicodeRangeToken(Start, End), new UnicodeRangeToken(Start, End));
        }

        [Theory()]
        [InlineData(0x7F, 0xFF, "U+7F-U+FF"), InlineData(0xFFFD, 0x6FFFD, "U+FFFD-U+6FFFD")]
        public void EncodeTest(int Start, int End, string Expected)
        {
            var Actual = new UnicodeRangeToken(Start, End).Encode();
            Assert.Equal(Expected, Actual);
        }

    }
}