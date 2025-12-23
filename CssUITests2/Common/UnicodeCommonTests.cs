using System;
using Xunit;
using static CssUI.UnicodeCommon;

namespace CssUITests;

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

    [Fact()]
    public void Has_ASCII_Upper_AlphaTest_False()
    {
        Assert.False(Has_ASCII_Upper_Alpha("abcdef"));
        Assert.False(Has_ASCII_Upper_Alpha("123456"));
        Assert.False(Has_ASCII_Upper_Alpha(""));
    }

    [Fact()]
    public void Has_ASCII_Lower_AlphaTest_False()
    {
        Assert.False(Has_ASCII_Lower_Alpha("ABCDEF"));
        Assert.False(Has_ASCII_Lower_Alpha("123456"));
        Assert.False(Has_ASCII_Lower_Alpha(""));
    }

    #region Is_NonPrintable Tests

    [Theory()]
    [InlineData(CHAR_TAB)]
    [InlineData(CHAR_C0_DELETE)]
    [InlineData('\u0000')]
    [InlineData('\u0001')]
    [InlineData('\u0008')]
    [InlineData('\u000E')]
    [InlineData('\u001F')]
    public void Is_NonPrintableTest_True(char code)
    {
        Assert.True(Is_NonPrintable(code));
    }

    [Theory()]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_DIGIT_0)]
    [InlineData(CHAR_LINE_FEED)]
    public void Is_NonPrintableTest_False(char code)
    {
        Assert.False(Is_NonPrintable(code));
    }

    #endregion

    #region Is_Surrogate_Code_Point Tests

    [Theory()]
    [InlineData('\uD800')]
    [InlineData('\uDBFF')]
    [InlineData('\uDC00')]
    [InlineData('\uDFFF')]
    public void Is_Surrogate_Code_PointTest_True(char code)
    {
        Assert.True(Is_Surrogate_Code_Point(code));
    }

    [Theory()]
    [InlineData('\uD7FF')]
    [InlineData('\uE000')]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_SPACE)]
    public void Is_Surrogate_Code_PointTest_False(char code)
    {
        Assert.False(Is_Surrogate_Code_Point(code));
    }

    #endregion

    #region Is_NonCharacter_Code_Point Tests

    [Theory()]
    [InlineData('\uFDD0')]
    [InlineData('\uFDEF')]
    [InlineData('\uFFFE')]
    [InlineData('\uFFFF')]
    public void Is_NonCharacter_Code_PointTest_True(char code)
    {
        Assert.True(Is_NonCharacter_Code_Point(code));
    }

    [Theory()]
    [InlineData('\uFDCF')]
    [InlineData('\uFDF0')]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_SPACE)]
    public void Is_NonCharacter_Code_PointTest_False(char code)
    {
        Assert.False(Is_NonCharacter_Code_Point(code));
    }

    #endregion

    #region Is_Ascii_Code_Point Tests

    [Theory()]
    [InlineData('\u0000')]
    [InlineData('\u007F')]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_DIGIT_0)]
    public void Is_Ascii_Code_PointTest_True(char code)
    {
        Assert.True(Is_Ascii_Code_Point(code));
    }

    [Theory()]
    [InlineData('\u0080')]
    [InlineData('\u00FF')]
    [InlineData('\uFFFF')]
    public void Is_Ascii_Code_PointTest_False(char code)
    {
        Assert.False(Is_Ascii_Code_Point(code));
    }

    #endregion

    #region Is_Ascii_Plus_Or_Minus Tests

    [Theory()]
    [InlineData(CHAR_PLUS_SIGN)]
    [InlineData(CHAR_HYPHEN_MINUS)]
    public void Is_Ascii_Plus_Or_MinusTest_True(char code)
    {
        Assert.True(Is_Ascii_Plus_Or_Minus(code));
    }

    [Theory()]
    [InlineData(CHAR_ASTERISK)]
    [InlineData(CHAR_SOLIDUS)]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_DIGIT_0)]
    public void Is_Ascii_Plus_Or_MinusTest_False(char code)
    {
        Assert.False(Is_Ascii_Plus_Or_Minus(code));
    }

    #endregion

    #region Is_Ascii_Tab_Or_Newline Tests

    [Theory()]
    [InlineData(CHAR_TAB)]
    [InlineData(CHAR_LINE_FEED)]
    [InlineData(CHAR_CARRIAGE_RETURN)]
    public void Is_Ascii_Tab_Or_NewlineTest_True(char code)
    {
        Assert.True(Is_Ascii_Tab_Or_Newline(code));
    }

    [Theory()]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_FORM_FEED)]
    [InlineData(CHAR_A_LOWER)]
    public void Is_Ascii_Tab_Or_NewlineTest_False(char code)
    {
        Assert.False(Is_Ascii_Tab_Or_Newline(code));
    }

    #endregion

    #region Is_Ascii_Control Tests

    [Theory()]
    [InlineData(CHAR_C0_DELETE)]
    [InlineData(CHAR_C0_APPLICATION_PROGRAM_COMMAND)]
    [InlineData('\u0080')]
    [InlineData('\u0090')]
    public void Is_Ascii_ControlTest_True(char code)
    {
        Assert.True(Is_Ascii_Control(code));
    }

    [Theory()]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_DIGIT_0)]
    [InlineData('\u007E')]
    public void Is_Ascii_ControlTest_False(char code)
    {
        Assert.False(Is_Ascii_Control(code));
    }

    #endregion

    #region Is_Ascii_Control_Or_Space Tests

    [Theory()]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_C0_DELETE)]
    [InlineData(CHAR_C0_APPLICATION_PROGRAM_COMMAND)]
    public void Is_Ascii_Control_Or_SpaceTest_True(char code)
    {
        Assert.True(Is_Ascii_Control_Or_Space(code));
    }

    [Theory()]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_DIGIT_0)]
    [InlineData(CHAR_TAB)]
    public void Is_Ascii_Control_Or_SpaceTest_False(char code)
    {
        Assert.False(Is_Ascii_Control_Or_Space(code));
    }

    #endregion

    #region Is_Ascii_Alphanumeric Tests

    [Theory()]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_Z_LOWER)]
    [InlineData(CHAR_A_UPPER)]
    [InlineData(CHAR_Z_UPPER)]
    [InlineData(CHAR_DIGIT_0)]
    [InlineData(CHAR_DIGIT_9)]
    public void Is_Ascii_AlphanumericTest_True(char code)
    {
        Assert.True(Is_Ascii_Alphanumeric(code));
    }

    [Theory()]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_HYPHEN_MINUS)]
    [InlineData(CHAR_UNDERSCORE)]
    [InlineData(CHAR_AT_SIGN)]
    public void Is_Ascii_AlphanumericTest_False(char code)
    {
        Assert.False(Is_Ascii_Alphanumeric(code));
    }

    #endregion

    #region Is_Selectable_Char Tests

    [Theory()]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_SPACE)]
    [InlineData(CHAR_DIGIT_0)]
    [InlineData('\u007E')]
    public void Is_Selectable_CharTest_True(char code)
    {
        Assert.True(Is_Selectable_Char(code));
    }

    [Theory()]
    [InlineData(CHAR_C0_DELETE)]
    [InlineData(CHAR_C0_APPLICATION_PROGRAM_COMMAND)]
    [InlineData('\u0080')]
    public void Is_Selectable_CharTest_False(char code)
    {
        Assert.False(Is_Selectable_Char(code));
    }

    #endregion

    #region To_ASCII_Lower_Alpha Additional Tests

    [Theory()]
    [InlineData(CHAR_DIGIT_0, CHAR_DIGIT_0)]
    [InlineData(CHAR_SPACE, CHAR_SPACE)]
    [InlineData(CHAR_A_LOWER, CHAR_A_LOWER)]
    public void To_ASCII_Lower_AlphaTest_NoChange(char input, char expected)
    {
        Assert.Equal(expected, To_ASCII_Lower_Alpha(input));
    }

    #endregion

    #region To_ASCII_Upper_Alpha Additional Tests

    [Theory()]
    [InlineData(CHAR_DIGIT_0, CHAR_DIGIT_0)]
    [InlineData(CHAR_SPACE, CHAR_SPACE)]
    [InlineData(CHAR_A_UPPER, CHAR_A_UPPER)]
    public void To_ASCII_Upper_AlphaTest_NoChange(char input, char expected)
    {
        Assert.Equal(expected, To_ASCII_Upper_Alpha(input));
    }

    #endregion

    #region Ascii_Digit_To_Value Exception Tests

    [Theory()]
    [InlineData(CHAR_A_LOWER)]
    [InlineData(CHAR_SPACE)]
    public void Ascii_Digit_To_ValueTest_ThrowsOnInvalidInput(char code)
    {
        Assert.Throws<IndexOutOfRangeException>(() => Ascii_Digit_To_Value(code));
    }

    #endregion

    #region Ascii_Hex_To_Value Exception Tests

    [Theory()]
    [InlineData(CHAR_Z_UPPER)]  // 'Z' = 0x5A = 90, within table bounds but not hex
    [InlineData(CHAR_SPACE)]    // ' ' = 0x20 = 32, within table bounds but not hex
    public void Ascii_Hex_To_ValueTest_ReturnsInvalidForNonHex(char code)
    {
        // Non-hex characters within table bounds return 0xFF (255) from the lookup table
        Assert.Equal(0xFF, Ascii_Hex_To_Value(code));
    }

    [Theory()]
    [InlineData(CHAR_G_LOWER)]  // 'g' = 0x67 = 103, beyond table length (103 entries: 0-102)
    [InlineData('\u0100')]
    public void Ascii_Hex_To_ValueTest_ThrowsOnOutOfRange(char code)
    {
        Assert.Throws<IndexOutOfRangeException>(() => Ascii_Hex_To_Value(code));
    }

    #endregion

    #region Percent Encoding Set Tests

    [Theory()]
    [InlineData(CHAR_C0_DELETE, true)]
    [InlineData(CHAR_C0_APPLICATION_PROGRAM_COMMAND, true)]
    [InlineData('\u00FF', true)]
    [InlineData(CHAR_A_LOWER, false)]
    [InlineData(CHAR_TILDE, false)]
    public void Percent_Encode_Set_C0_ControlTest(char code, bool expected)
    {
        Assert.Equal(expected, Percent_Encode_Set_C0_Control(code));
    }

    [Theory()]
    [InlineData(CHAR_SPACE, true)]
    [InlineData(CHAR_QUOTATION_MARK, true)]
    [InlineData(CHAR_LEFT_CHEVRON, true)]
    [InlineData(CHAR_RIGHT_CHEVRON, true)]
    [InlineData(CHAR_BACKTICK, true)]
    [InlineData(CHAR_A_LOWER, false)]
    public void Percent_Encode_Set_FragmentTest(char code, bool expected)
    {
        Assert.Equal(expected, Percent_Encode_Set_Fragment(code));
    }

    [Theory()]
    [InlineData(CHAR_HASH, true)]
    [InlineData(CHAR_QUESTION_MARK, true)]
    [InlineData(CHAR_LEFT_CURLY_BRACKET, true)]
    [InlineData(CHAR_RIGHT_CURLY_BRACKET, true)]
    [InlineData(CHAR_A_LOWER, false)]
    public void Percent_Encode_Set_PathTest(char code, bool expected)
    {
        Assert.Equal(expected, Percent_Encode_Set_Path(code));
    }

    [Theory()]
    [InlineData(CHAR_SOLIDUS, true)]
    [InlineData(CHAR_COLON, true)]
    [InlineData(CHAR_SEMICOLON, true)]
    [InlineData(CHAR_EQUALS, true)]
    [InlineData(CHAR_AT_SIGN, true)]
    [InlineData(CHAR_LEFT_SQUARE_BRACKET, true)]
    [InlineData(CHAR_REVERSE_SOLIDUS, true)]
    [InlineData(CHAR_RIGHT_SQUARE_BRACKET, true)]
    [InlineData(CHAR_CARET, true)]
    [InlineData(CHAR_PIPE, true)]
    [InlineData(CHAR_A_LOWER, false)]
    public void Percent_Encode_Set_UserinfoTest(char code, bool expected)
    {
        Assert.Equal(expected, Percent_Encode_Set_Userinfo(code));
    }

    #endregion

    #region Percent_Encode Tests

    [Fact()]
    public void Percent_EncodeTest()
    {
        var result = Percent_Encode(0x20); // space
        Assert.Equal(new char[] { '%', '2', '0' }, result);
    }

    [Fact()]
    public void Percent_EncodeTest_Zero()
    {
        var result = Percent_Encode(0x00);
        Assert.Equal(new char[] { '%', '0', '0' }, result);
    }

    [Fact()]
    public void Percent_EncodeTest_MaxByte()
    {
        var result = Percent_Encode(0xFF);
        Assert.Equal(new char[] { '%', 'F', 'F' }, result);
    }

    #endregion

    #region UTF8_Percent_Encode Tests

    [Fact()]
    public void UTF8_Percent_EncodeTest_NoEncoding()
    {
        // When predicate returns false, character is returned as-is
        var result = UTF8_Percent_Encode('a', c => false);
        Assert.Equal("a", result);
    }

    [Fact()]
    public void UTF8_Percent_EncodeTest_WithEncoding()
    {
        // When predicate returns true, character is percent-encoded
        var result = UTF8_Percent_Encode(' ', c => true);
        Assert.Equal("%20", result);
    }

    [Fact()]
    public void UTF8_Percent_EncodeTest_MultiByteChar()
    {
        // Multi-byte UTF-8 character (é = 0xC3 0xA9)
        var result = UTF8_Percent_Encode('é', c => true);
        Assert.Equal("%C3%A9", result);
    }

    #endregion

    #region Convert_To_Scalar_Values Tests

    [Fact()]
    public void Convert_To_Scalar_ValuesTest_AsciiString()
    {
        var input = "Hello".AsMemory();
        var result = Convert_To_Scalar_Values(input);
        Assert.Equal("Hello", result);
    }

    [Fact()]
    public void Convert_To_Scalar_ValuesTest_EmptyString()
    {
        var input = "".AsMemory();
        var result = Convert_To_Scalar_Values(input);
        Assert.Equal("", result);
    }

    [Fact()]
    public void Convert_To_Scalar_ValuesTest_LoneSurrogateAtEnd()
    {
        // Lone high surrogate at end should be replaced with CHAR_REPLACEMENT
        var input = new char[] { 'a', '\uD800' };
        var result = Convert_To_Scalar_Values(input.AsMemory());
        Assert.Equal("a\uFFFD", result);
    }

    [Fact()]
    public void Convert_To_Scalar_ValuesTest_LoneLowSurrogate()
    {
        // Lone low surrogate should be replaced with CHAR_REPLACEMENT
        var input = new char[] { '\uDC00', 'a' };
        var result = Convert_To_Scalar_Values(input.AsMemory());
        Assert.Equal("\uFFFDa", result);
    }

    #endregion

    #region Ascii_Value_To_Hex Additional Tests

    [Fact()]
    public void Ascii_Value_To_HexTest_WithPadding()
    {
        Assert.Equal("0001", Ascii_Value_To_Hex(1, 4));
        Assert.Equal("00FF", Ascii_Value_To_Hex(255, 4));
    }

    [Fact()]
    public void Ascii_Value_To_HexTest_CharOverload()
    {
        Assert.Equal("41", Ascii_Value_To_Hex('A', 2));
        Assert.Equal("61", Ascii_Value_To_Hex('a', 2));
    }

    [Fact()]
    public void Ascii_Value_To_HexTest_Int32Overload()
    {
        Assert.Equal("7FFFFFFF", Ascii_Value_To_Hex(Int32.MaxValue));
    }

    [Fact()]
    public void Ascii_Value_To_HexTest_Int64Overload()
    {
        Assert.Equal("7FFFFFFFFFFFFFFF", Ascii_Value_To_Hex(Int64.MaxValue));
    }

    [Fact()]
    public void Ascii_Value_To_HexTest_Zero()
    {
        Assert.Equal("0", Ascii_Value_To_Hex(0));
        Assert.Equal("00", Ascii_Value_To_Hex(0, 2));
    }

    #endregion

    #region Constants Validation Tests

    [Fact()]
    public void Constants_AsciiDigits_ContainsAllDigits()
    {
        var digits = ASCII_DIGITS;
        Assert.Equal(10, digits.Length);
        Assert.Equal('0', digits[0]);
        Assert.Equal('9', digits[9]);
    }

    [Fact()]
    public void Constants_SpecialChars_HaveCorrectValues()
    {
        Assert.Equal('\0', EOF);
        Assert.Equal('\0', CHAR_NULL);
        Assert.Equal('\uFFFD', CHAR_REPLACEMENT);
        Assert.Equal(0x10FFFF, CHAR_UNICODE_MAX);
    }

    [Fact()]
    public void Constants_Whitespace_HaveCorrectValues()
    {
        Assert.Equal('\t', CHAR_TAB);
        Assert.Equal('\n', CHAR_LINE_FEED);
        Assert.Equal('\f', CHAR_FORM_FEED);
        Assert.Equal('\r', CHAR_CARRIAGE_RETURN);
        Assert.Equal(' ', CHAR_SPACE);
    }

    [Fact()]
    public void Constants_Brackets_HaveCorrectValues()
    {
        Assert.Equal('{', CHAR_LEFT_CURLY_BRACKET);
        Assert.Equal('}', CHAR_RIGHT_CURLY_BRACKET);
        Assert.Equal('[', CHAR_LEFT_SQUARE_BRACKET);
        Assert.Equal(']', CHAR_RIGHT_SQUARE_BRACKET);
        Assert.Equal('(', CHAR_LEFT_PARENTHESES);
        Assert.Equal(')', CHAR_RIGHT_PARENTHESES);
        Assert.Equal('<', CHAR_LEFT_CHEVRON);
        Assert.Equal('>', CHAR_RIGHT_CHEVRON);
    }

    [Fact()]
    public void Constants_CommonSymbols_HaveCorrectValues()
    {
        Assert.Equal('"', CHAR_QUOTATION_MARK);
        Assert.Equal('\'', CHAR_APOSTRAPHE);
        Assert.Equal('+', CHAR_PLUS_SIGN);
        Assert.Equal('%', CHAR_PERCENT);
        Assert.Equal('-', CHAR_HYPHEN_MINUS);
        Assert.Equal('_', CHAR_UNDERSCORE);
        Assert.Equal('.', CHAR_FULL_STOP);
        Assert.Equal('/', CHAR_SOLIDUS);
        Assert.Equal('\\', CHAR_REVERSE_SOLIDUS);
        Assert.Equal('#', CHAR_HASH);
        Assert.Equal(',', CHAR_COMMA);
        Assert.Equal(':', CHAR_COLON);
        Assert.Equal(';', CHAR_SEMICOLON);
    }

    #endregion
}
