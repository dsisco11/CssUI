using Xunit;
using CssUI.CSS.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.CSS.Parser.Tests
{
    public class HashTokenTests
    {
        [Theory()]
        [InlineData(EHashTokenType.ID, "#id"), InlineData(EHashTokenType.Unrestricted, "#hash")]
        public void EqualsTest(EHashTokenType Type, string Value)
        {
            Assert.Equal(new HashToken(Type, Value), new HashToken(Type, Value));
        }

        [Theory()]
        [InlineData(EHashTokenType.ID, "#id"), InlineData(EHashTokenType.Unrestricted, "#hash")]
        public void EncodeTest(EHashTokenType Type, string Value)
        {
            string Actual = new HashToken(Type, Value).Encode();
            Assert.Equal(Value, Actual);
        }
    }
}