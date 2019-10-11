using Xunit;
using CssUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.Tests
{
    public class ParsingCommonTests
    {

        [Theory()]
        [InlineData(0, "0"), InlineData(1, "01"), InlineData(2, "2"), InlineData(4, "4"), InlineData(8, "8"), InlineData(16, "16"), InlineData(32, "32"), InlineData(64, "64"), InlineData(128, "128"), InlineData(256, "256"), InlineData(512, "512"), InlineData(1024, "1024"), InlineData(2048, "2048"), InlineData(4096, "4096")]
        public void Digits_To_Base10Test(long expected, string input)
        {
            Assert.Equal(expected, ParsingCommon.Digits_To_Base10(input));
        }

        [Theory()]
        [InlineData(0, "0"), InlineData(1, "1"), InlineData(2, "02"), InlineData(4, "4"), InlineData(8, "8"), InlineData(16, "16"), InlineData(32, "32"), InlineData(64, "64"), InlineData(128, "128"), InlineData(256, "256"), InlineData(512, "512"), InlineData(1024, "1024"), InlineData(2048, "2048"), InlineData(4096, "4096")]
        public void Digits_To_Base10_UnsignedTest(ulong expected, string input)
        {
            Assert.Equal(expected, ParsingCommon.Digits_To_Base10_Unsigned(input));
        }

        [Theory()]
        [InlineData(0, "#0"), InlineData(1, "#1"), InlineData(2, "#2"), InlineData(4, "#4"), InlineData(8, "#8"), InlineData(15, "#F"), InlineData(255, "#FF"), InlineData(4095, "#FFF"), InlineData(65535, "#FFFF"), InlineData(UInt32.MaxValue, "#FFFFFFFF"), InlineData(250, "#FA"), InlineData(4000, "#FA0"), InlineData(44779, "#AEEB"), InlineData(48879, "#BEEF")]
        public void Parse_HexTest(ulong expected, string input)
        {
            Assert.True(ParsingCommon.Parse_Hex(input.AsMemory(), out ulong actual));
            Assert.Equal(expected, actual);
        }

        [Theory()]
        [InlineData(-1024, "-1024"), InlineData(-1, "-1"), InlineData(0, "0"), InlineData(1, "1"), InlineData(2, "2"), InlineData(1024, "1024")]
        public void Parse_IntegerTest(long expected, string input)
        {
            Assert.True(ParsingCommon.Parse_Integer(input.AsMemory(), out long actual));
            Assert.Equal(expected, actual);
        }

        [Theory()]
        [InlineData(-25000.0, "-2.5E4"), InlineData(-2500.0, "-2.5E3"), InlineData(-250.0, "-2.5E2"), InlineData(-1024.5, "-1024.5"), InlineData(-1.5, "-1.5"), InlineData(0.5, "0.5"), InlineData(1.5, "1.5"), InlineData(2.5, "2.5"), InlineData(250.0, "2.5E2"), InlineData(2500.0, "2.5E3"), InlineData(25000.0, "2.5E4")]
        public void Parse_FloatingPointTest(float expected, string input)
        {
            Assert.True(ParsingCommon.Parse_FloatingPoint(input.AsMemory(), out float actual));
            Assert.Equal(expected, actual);
        }
    }
}