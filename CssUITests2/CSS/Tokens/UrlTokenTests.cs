using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class UrlTokenTests
    {
        [Theory()]
        [InlineData("https://google.com")]
        public void EqualsTest(string Value)
        {
            Assert.Equal(new UrlToken(Value), new UrlToken(Value));
        }

        [Theory()]
        [InlineData("https://google.com", "url(https://google.com)")]
        public void EncodeTest(string Value, string Expected)
        {
            string Actual = new UrlToken(Value).Encode();
            Assert.Equal(Expected, Actual);
        }
    }
}