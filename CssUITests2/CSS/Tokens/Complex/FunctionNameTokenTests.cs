using Xunit;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class FunctionNameTokenTests
    {
        [Theory()]
        [InlineData("not"), InlineData("contains"), InlineData("calc")]
        public void EqualsTest(string Value)
        {
            Assert.Equal(new FunctionNameToken(Value), new FunctionNameToken(Value));
        }

        [Theory()]
        [InlineData("not", "not("), InlineData("contains", "contains("), InlineData("calc", "calc(")]
        public void EncodeTest(string Value, string Expected)
        {
            string Actual = new FunctionNameToken(Value).Encode();
            Assert.Equal(Expected, Actual);
        }
    }
}