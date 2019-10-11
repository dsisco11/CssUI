using Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using CssUI.CSS.Parser;

namespace CssUI.CSS.Parser.Tests
{
    public class WhitespaceTokenTests
    {
        [Fact()]
        public void EqualsTest()
        {
            Assert.Equal(WhitespaceToken.Space, new WhitespaceToken(" "));
            Assert.Equal(WhitespaceToken.Tab, new WhitespaceToken("\t"));
            Assert.Equal(WhitespaceToken.LF, new WhitespaceToken("\n"));
            Assert.Equal(WhitespaceToken.LFLF, new WhitespaceToken("\n\n"));
        }

        [Theory()]
        [InlineData(" "), InlineData("\t"), InlineData("\n"), InlineData("\n\n")]
        public void EncodeTest(string Expected)
        {
            var Token = new WhitespaceToken(Expected);
            var Actual = Token.Encode();
            Assert.Equal(Expected, Actual);
        }
    }
}