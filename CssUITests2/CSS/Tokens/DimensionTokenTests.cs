using Xunit;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class DimensionTokenTests
    {
        [Theory()]
        [InlineData(ENumericTokenType.Integer, "-1", -1, "px"), InlineData(ENumericTokenType.Integer, "0", 0, "px"), InlineData(ENumericTokenType.Integer, "+1", 1, "px"), InlineData(ENumericTokenType.Integer, "2", 2, "px")]
        [InlineData(ENumericTokenType.Number, "-1.", -1D, "px"), InlineData(ENumericTokenType.Number, "0.0", 0D, "px"), InlineData(ENumericTokenType.Number, ".1", 0.1D, "px"), InlineData(ENumericTokenType.Number, "+.5", 0.5D, "px")]
        public void EqualsTest(ENumericTokenType Type, string Value, double Number, string Unit)
        {
            Assert.Equal(new DimensionToken(Type, Value, Number, Unit), new DimensionToken(Type, Value, Number, Unit));
        }

        [Theory()]
        [InlineData(ENumericTokenType.Integer, "-1", -1, "px", "-1px"), InlineData(ENumericTokenType.Integer, "0", 0, "px", "0px"), InlineData(ENumericTokenType.Integer, "+1", 1, "px", "+1px"), InlineData(ENumericTokenType.Integer, "2", 2, "px", "2px")]
        [InlineData(ENumericTokenType.Number, "-1.", -1D, "px", "-1.px"), InlineData(ENumericTokenType.Number, "0.0", 0D, "px", "0.0px"), InlineData(ENumericTokenType.Number, ".1", 0.1D, "px", ".1px"), InlineData(ENumericTokenType.Number, "+.5", 0.5D, "px", "+.5px")]
        public void EncodeTest(ENumericTokenType Type, string Value, double Number, string Unit, string Expected)
        {
            string Actual = new DimensionToken(Type, Value, Number, Unit).Encode();
            Assert.Equal(Expected, Actual);
        }

    }
}