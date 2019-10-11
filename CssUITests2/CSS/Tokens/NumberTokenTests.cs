using Xunit;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class NumberTokenTests
    {
        [Theory()]
        [InlineData(ENumericTokenType.Integer, "-1", -1), InlineData(ENumericTokenType.Integer, "0", 0), InlineData(ENumericTokenType.Integer, "+1", 1), InlineData(ENumericTokenType.Integer, "2", 2)]
        [InlineData(ENumericTokenType.Number, "-1.", -1D), InlineData(ENumericTokenType.Number, "0.0", 0D), InlineData(ENumericTokenType.Number, ".1", 0.1D), InlineData(ENumericTokenType.Number, "+.5", 0.5D)]
        public void EqualsTest(ENumericTokenType Type, string Value, double Number)
        {
            Assert.Equal(new NumberToken(Type, Value, Number), new NumberToken(Type, Value, Number));
        }

        [Theory()]
        [InlineData(ENumericTokenType.Integer, "-1", -1), InlineData(ENumericTokenType.Integer, "0", 0), InlineData(ENumericTokenType.Integer, "+1", 1), InlineData(ENumericTokenType.Integer, "2", 2)]
        [InlineData(ENumericTokenType.Number, "-1.", -1D), InlineData(ENumericTokenType.Number, "0.0", 0D), InlineData(ENumericTokenType.Number, ".1", 0.1D), InlineData(ENumericTokenType.Number, "+.5", 0.5D)]
        public void EncodeTest(ENumericTokenType Type, string Value, double Number)
        {
            string Actual = new NumberToken(Type, Value, Number).Encode();
            Assert.Equal(Value, Actual);
        }
    }
}