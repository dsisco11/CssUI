using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class DelimTokenTests
    {
        [Theory()]
        [InlineData('='), InlineData('>')]
        public void EqualsTest(char Value)
        {
            Assert.Equal(new DelimToken(Value), new DelimToken(Value));
        }

        [Theory()]
        [InlineData('='), InlineData('>')]
        public void EncodeTest(char Value)
        {
            string Actual = new DelimToken(Value).Encode();
            Assert.Equal(Value.ToString(), Actual);
        }
    }
}