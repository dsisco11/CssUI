using Xunit;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class PercentageTokenTests
    {
        [Theory()]
        [InlineData("-1.", -1D), InlineData("0.0", 0D), InlineData(".1", 0.1D)]
        public void EqualsTest(string Value, double Number)
        {
            Assert.Equal(new PercentageToken(Value, Number), new PercentageToken(Value, Number));
        }

        [Theory()]
        [InlineData("-1.", -1D, "-1.%"), InlineData("0.0", 0D, "0.0%"), InlineData(".1", 0.1D, ".1%")]
        public void EncodeTest(string Value, double Number, string Expected)
        {
            string Actual = new PercentageToken(Value, Number).Encode();
            Assert.Equal(Expected, Actual);
        }
    }
}