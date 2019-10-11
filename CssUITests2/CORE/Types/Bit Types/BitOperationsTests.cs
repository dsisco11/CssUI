using Xunit;
using CssUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace CssUI.Tests
{
    public class BitOperationsTests
    {
        [Fact()]
        public void PopulationCountTest()
        {
            Assert.Equal(0, BitOperations.PopulationCount(0u));
            Assert.Equal(1, BitOperations.PopulationCount(1u));
            Assert.Equal(1, BitOperations.PopulationCount(2u));
            Assert.Equal(2, BitOperations.PopulationCount(3u));//, $"Bin: {Convert.ToString((1u << 3), 2).PadLeft(8,'0')}");
            Assert.Equal(1, BitOperations.PopulationCount(4u));
            Assert.Equal(3, BitOperations.PopulationCount(7u));
            Assert.Equal(1, BitOperations.PopulationCount(8u));
            Assert.Equal(4, BitOperations.PopulationCount(15u));
            Assert.Equal(1, BitOperations.PopulationCount(16u));
            Assert.Equal(5, BitOperations.PopulationCount(31u));
            Assert.Equal(1, BitOperations.PopulationCount(32u));
            Assert.Equal(6, BitOperations.PopulationCount(63u));
            Assert.Equal(1, BitOperations.PopulationCount(64u));
            Assert.Equal(7, BitOperations.PopulationCount(127u));
            Assert.Equal(1, BitOperations.PopulationCount(128u));
            Assert.Equal(8, BitOperations.PopulationCount(255u));
            Assert.Equal(1, BitOperations.PopulationCount(256u));
            Assert.Equal(9, BitOperations.PopulationCount(511u));
            Assert.Equal(1, BitOperations.PopulationCount(512u));
            Assert.Equal(10, BitOperations.PopulationCount(1023u));
            Assert.Equal(1, BitOperations.PopulationCount(1024u));
            Assert.Equal(11, BitOperations.PopulationCount(2047u));
            Assert.Equal(1, BitOperations.PopulationCount(2048u));
            Assert.Equal(12, BitOperations.PopulationCount(4095u));
            Assert.Equal(1, BitOperations.PopulationCount(4096u));
            Assert.Equal(13, BitOperations.PopulationCount(8191u));
            Assert.Equal(1, BitOperations.PopulationCount(8192u));
            Assert.Equal(14, BitOperations.PopulationCount(16383u));
            Assert.Equal(1, BitOperations.PopulationCount(16384u));
            Assert.Equal(15, BitOperations.PopulationCount(32767u));
            Assert.Equal(1, BitOperations.PopulationCount(32768u));
            Assert.Equal(16, BitOperations.PopulationCount(65535u));
            Assert.Equal(1, BitOperations.PopulationCount(65536u));

            Assert.Equal(31, BitOperations.PopulationCount(0xFFFFFFFE));
            Assert.Equal(32, BitOperations.PopulationCount(0xFFFFFFFF));
        }

        [Fact(DisplayName = "CountLeadingZeros 32bit")]
        public void CountLeadingZerosTest()
        {
            Assert.Equal(32, BitOperations.CountLeadingZeros(0u));
            Assert.Equal(31, BitOperations.CountLeadingZeros(1u << 0));
            Assert.Equal(29, BitOperations.CountLeadingZeros(1u << 2));
            Assert.Equal(27, BitOperations.CountLeadingZeros(1u << 4));
            Assert.Equal(23, BitOperations.CountLeadingZeros(1u << 8));
            Assert.Equal(15, BitOperations.CountLeadingZeros(1u << 16));
            Assert.Equal(31, BitOperations.CountLeadingZeros(1u << 32));
        }

        [Fact(DisplayName = "CountLeadingZeros 64bit")]
        public void CountLeadingZerosTest1()
        {
            Assert.Equal(64, BitOperations.CountLeadingZeros(0ul));
            Assert.Equal(63, BitOperations.CountLeadingZeros(1ul << 0));
            Assert.Equal(61, BitOperations.CountLeadingZeros(1ul << 2));
            Assert.Equal(59, BitOperations.CountLeadingZeros(1ul << 4));
            Assert.Equal(55, BitOperations.CountLeadingZeros(1ul << 8));
            Assert.Equal(47, BitOperations.CountLeadingZeros(1ul << 16));
            Assert.Equal(31, BitOperations.CountLeadingZeros(1ul << 32));
            Assert.Equal(0, BitOperations.CountLeadingZeros(1ul << 63));
            Assert.Equal(63, BitOperations.CountLeadingZeros(1ul << 64));
        }

        [Fact(DisplayName = "CountTrailingZeros 32bit")]
        public void CountTrailingZerosTest()
        {
            Assert.Equal(0, BitOperations.CountTrailingZeros(0u));
            Assert.Equal(0, BitOperations.CountTrailingZeros(1u << 32));
            Assert.Equal(31, BitOperations.CountTrailingZeros(1u << 31));
            Assert.Equal(16, BitOperations.CountTrailingZeros(1u << 16));
            Assert.Equal(8, BitOperations.CountTrailingZeros(1u << 8));
            Assert.Equal(4, BitOperations.CountTrailingZeros(1u << 4));
            Assert.Equal(2, BitOperations.CountTrailingZeros(1u << 2));
            Assert.Equal(0, BitOperations.CountTrailingZeros(1u << 0));
        }

        [Fact(DisplayName = "CountTrailingZeros 64bit")]
        public void CountTrailingZerosTest1()
        {
            Assert.Equal(0, BitOperations.CountTrailingZeros(0ul));
            Assert.Equal(0, BitOperations.CountTrailingZeros(1ul << 64));
            Assert.Equal(63, BitOperations.CountTrailingZeros(1ul << 63));
            Assert.Equal(32, BitOperations.CountTrailingZeros(1ul << 32));
            Assert.Equal(16, BitOperations.CountTrailingZeros(1ul << 16));
            Assert.Equal(8, BitOperations.CountTrailingZeros(1ul << 8));
            Assert.Equal(4, BitOperations.CountTrailingZeros(1ul << 4));
            Assert.Equal(2, BitOperations.CountTrailingZeros(1ul << 2));
            Assert.Equal(0, BitOperations.CountTrailingZeros(1ul << 0));
        }
    }
}