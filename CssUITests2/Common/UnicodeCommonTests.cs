using Xunit;
using CssUI;
using System;
using System.Collections.Generic;
using System.Text;
using static CssUI.UnicodeCommon;

namespace CssUI.Tests
{
    public class UnicodeCommonTests
    {
        [Fact()]
        public void Ascii_Value_To_HexTest()
        {
            Assert.Equal(15, Ascii_Hex_To_Value('F'));
            Assert.Equal(10, Ascii_Hex_To_Value('A'));
            Assert.Equal(9, Ascii_Hex_To_Value('9'));
            Assert.Equal(1, Ascii_Hex_To_Value('1'));
            Assert.Equal(0, Ascii_Hex_To_Value('0'));

            Assert.Equal("20", Ascii_Value_To_Hex(CHAR_SPACE, 2));
            Assert.Equal("0000FFFF", Ascii_Value_To_Hex(UInt16.MaxValue, 8));
            Assert.Equal("FFFFFFFF", Ascii_Value_To_Hex(UInt32.MaxValue));
            Assert.Equal("FFFFFFFFFFFFFFFF", Ascii_Value_To_Hex(UInt64.MaxValue));

            Assert.Equal("F", Ascii_Value_To_Hex(15));
            Assert.Equal("FF", Ascii_Value_To_Hex(255));
            Assert.Equal("FFF", Ascii_Value_To_Hex(4095));
            Assert.Equal("FFFF", Ascii_Value_To_Hex(65535));
        }

        [Theory()]
        [InlineData(true, CHAR_SPACE), InlineData(true, CHAR_TAB), InlineData(true, CHAR_FORM_FEED), InlineData(true, CHAR_LINE_FEED)]
        [InlineData(false, CHAR_NULL), InlineData(false, CHAR_DIGIT_0), InlineData(false, CHAR_AT_SIGN), InlineData(false, CHAR_A_LOWER), InlineData(false, CHAR_A_UPPER)]
        public void Is_Ascii_WhitespaceTest(bool expected, char code)
        {
            Assert.Equal(expected, Is_Ascii_Whitespace(code));
        }
        
        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        public void Is_Ascii_DigitTest_T(char code)
        {
            Assert.True(Is_Ascii_Digit(code));
        }
        [Theory()]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER), InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER), InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_Ascii_DigitTest_F(char code)
        {
            Assert.False(Is_Ascii_Digit(code));
        }

        [Theory()]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER), InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER), InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_Ascii_AlphaTest_T(char code)
        {
            Assert.True(Is_Ascii_Alpha(code));
        }
        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        public void Is_Ascii_AlphaTest_F(char code)
        {
            Assert.False(Is_Ascii_Alpha(code));
        }

        [Theory()]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER), InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        public void Is_ASCII_Lower_AlphaTest_T(char code)
        {
            Assert.True(Is_ASCII_Lower_Alpha(code));
        }
        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER), InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_ASCII_Lower_AlphaTest_F(char code)
        {
            Assert.False(Is_ASCII_Lower_Alpha(code));
        }

        [Theory()]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER), InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_ASCII_Upper_AlphaTest_T(char code)
        {
            Assert.True(Is_ASCII_Upper_Alpha(code));
        }
        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER), InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        public void Is_ASCII_Upper_AlphaTest_F(char code)
        {
            Assert.False(Is_ASCII_Upper_Alpha(code));
        }

        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER)]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER)]
        public void Is_Ascii_Hex_DigitTest_T(char code)
        {
            Assert.True(Is_Ascii_Hex_Digit(code));
        }
        [Theory()]
        [InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        [InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_Ascii_Hex_DigitTest_F(char code)
        {
            Assert.False(Is_Ascii_Hex_Digit(code));
        }

        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER)]
        public void Is_Ascii_Hex_Digit_LowerTest_T(char code)
        {
            Assert.True(Is_Ascii_Hex_Digit_Lower(code));
        }
        [Theory()]
        [InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER), InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_Ascii_Hex_Digit_LowerTest_F(char code)
        {
            Assert.False(Is_Ascii_Hex_Digit_Lower(code));
        }

        [Theory()]
        [InlineData(CHAR_DIGIT_0), InlineData(CHAR_DIGIT_1), InlineData(CHAR_DIGIT_2), InlineData(CHAR_DIGIT_3), InlineData(CHAR_DIGIT_4), InlineData(CHAR_DIGIT_5), InlineData(CHAR_DIGIT_6), InlineData(CHAR_DIGIT_7), InlineData(CHAR_DIGIT_8), InlineData(CHAR_DIGIT_9)]
        [InlineData(CHAR_A_UPPER), InlineData(CHAR_B_UPPER), InlineData(CHAR_C_UPPER), InlineData(CHAR_D_UPPER), InlineData(CHAR_E_UPPER), InlineData(CHAR_F_UPPER)]
        public void Is_Ascii_Hex_Digit_UpperTest_T(char code)
        {
            Assert.True(Is_Ascii_Hex_Digit_Upper(code));
        }
        [Theory()]
        [InlineData(CHAR_A_LOWER), InlineData(CHAR_B_LOWER), InlineData(CHAR_C_LOWER), InlineData(CHAR_D_LOWER), InlineData(CHAR_E_LOWER), InlineData(CHAR_F_LOWER), InlineData(CHAR_G_LOWER), InlineData(CHAR_Z_LOWER)]
        [InlineData(CHAR_G_UPPER), InlineData(CHAR_Z_UPPER)]
        public void Is_Ascii_Hex_Digit_UpperTest_F(char code)
        {
            Assert.False(Is_Ascii_Hex_Digit_Upper(code));
        }


        [Fact()]
        public void To_ASCII_Lower_AlphaTest()
        {
            Assert.Equal(CHAR_A_LOWER, To_ASCII_Lower_Alpha(CHAR_A_UPPER));
            Assert.Equal(CHAR_B_LOWER, To_ASCII_Lower_Alpha(CHAR_B_UPPER));
            Assert.Equal(CHAR_C_LOWER, To_ASCII_Lower_Alpha(CHAR_C_UPPER));
            Assert.Equal(CHAR_D_LOWER, To_ASCII_Lower_Alpha(CHAR_D_UPPER));
            Assert.Equal(CHAR_E_LOWER, To_ASCII_Lower_Alpha(CHAR_E_UPPER));
            Assert.Equal(CHAR_F_LOWER, To_ASCII_Lower_Alpha(CHAR_F_UPPER));
            Assert.Equal(CHAR_Z_LOWER, To_ASCII_Lower_Alpha(CHAR_Z_UPPER));
        }

        [Fact()]
        public void To_ASCII_Upper_AlphaTest()
        {
            Assert.Equal(CHAR_A_UPPER, To_ASCII_Upper_Alpha(CHAR_A_LOWER));
            Assert.Equal(CHAR_B_UPPER, To_ASCII_Upper_Alpha(CHAR_B_LOWER));
            Assert.Equal(CHAR_C_UPPER, To_ASCII_Upper_Alpha(CHAR_C_LOWER));
            Assert.Equal(CHAR_D_UPPER, To_ASCII_Upper_Alpha(CHAR_D_LOWER));
            Assert.Equal(CHAR_E_UPPER, To_ASCII_Upper_Alpha(CHAR_E_LOWER));
            Assert.Equal(CHAR_F_UPPER, To_ASCII_Upper_Alpha(CHAR_F_LOWER));
            Assert.Equal(CHAR_Z_UPPER, To_ASCII_Upper_Alpha(CHAR_Z_LOWER));
        }

        [Theory()]
        [InlineData(0, CHAR_DIGIT_0), InlineData(1, CHAR_DIGIT_1), InlineData(2, CHAR_DIGIT_2), InlineData(3, CHAR_DIGIT_3), InlineData(4, CHAR_DIGIT_4), InlineData(5, CHAR_DIGIT_5), InlineData(6, CHAR_DIGIT_6), InlineData(7, CHAR_DIGIT_7), InlineData(8, CHAR_DIGIT_8), InlineData(9, CHAR_DIGIT_9)]
        public void Ascii_Digit_To_ValueTest(int value, char code)
        {
            Assert.Equal(value, Ascii_Digit_To_Value(code));
        }

        [Theory()]
        [InlineData(0, CHAR_DIGIT_0), InlineData(1, CHAR_DIGIT_1), InlineData(2, CHAR_DIGIT_2), InlineData(3, CHAR_DIGIT_3), InlineData(4, CHAR_DIGIT_4), InlineData(5, CHAR_DIGIT_5), InlineData(6, CHAR_DIGIT_6), InlineData(7, CHAR_DIGIT_7), InlineData(8, CHAR_DIGIT_8), InlineData(9, CHAR_DIGIT_9)]
        [InlineData(10, CHAR_A_LOWER), InlineData(11, CHAR_B_LOWER), InlineData(12, CHAR_C_LOWER), InlineData(13, CHAR_D_LOWER), InlineData(14, CHAR_E_LOWER), InlineData(15, CHAR_F_LOWER)]
        [InlineData(10, CHAR_A_UPPER), InlineData(11, CHAR_B_UPPER), InlineData(12, CHAR_C_UPPER), InlineData(13, CHAR_D_UPPER), InlineData(14, CHAR_E_UPPER), InlineData(15, CHAR_F_UPPER)]
        public void Ascii_Hex_To_ValueTest(int value, char code)
        {
            Assert.Equal(value, Ascii_Hex_To_Value(code));
        }

        [Fact()]
        public void Has_ASCII_Lower_AlphaTest()
        {
            Assert.True(Has_ASCII_Lower_Alpha("aAbBcC"));
        }

        [Fact()]
        public void Has_ASCII_Upper_AlphaTest()
        {
            Assert.True(Has_ASCII_Upper_Alpha("aAbBcC"));
        }

        /*
        [Fact()]
        public void Convert_To_Scalar_ValuesTest()
        {
        }

        [Fact()]
        public void Percent_EncodeTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void Percent_DecodeTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void UTF8_Percent_EncodeTest()
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void String_Percent_DecodeTest()
        {
            throw new NotImplementedException();
        }
        */
    }
}