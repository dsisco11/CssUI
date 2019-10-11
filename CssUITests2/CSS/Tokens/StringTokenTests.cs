using Xunit;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class StringTokenTests
    {
        [Theory()]
        [InlineData("abc"), InlineData("123"), InlineData("Hello World")]
        public void EqualsTest(string Value)
        {
            Assert.Equal(new StringToken(Value), new StringToken(Value));
        }

        [Theory()]
        [InlineData("abc"), InlineData("123"), InlineData("Hello World")]
        public void EncodeTest(string Value)
        {
            string Actual = new StringToken(Value).Encode();
            Assert.Equal(Value, Actual);
        }
    }
}