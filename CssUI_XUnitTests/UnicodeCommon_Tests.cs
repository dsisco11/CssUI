using CssUI;
using Xunit;

namespace CssUI_XUnitTests
{
    public class UnicodeCommon_Tests
    {
        [Fact(DisplayName = "UnicodeCommon: Hexadecimal processing")]
        public void Test_Hexadecimal_Processing()
        {
            string hexStr = string.Empty;
            /* Make sure we are converting values to hexadecimal properly */

            Assert.Equal("F", UnicodeCommon.Ascii_Value_To_Hex(16 - 1).ToString());
            Assert.Equal("FF", UnicodeCommon.Ascii_Value_To_Hex(256 - 1).ToString());
            Assert.Equal("FFF", UnicodeCommon.Ascii_Value_To_Hex(4096 - 1).ToString());
            Assert.Equal("FFFF", UnicodeCommon.Ascii_Value_To_Hex(65536 - 1).ToString());

            Assert.Equal(15L, UnicodeCommon.Ascii_Hex_To_Value('F'));
            Assert.Equal(10L, UnicodeCommon.Ascii_Hex_To_Value('A'));
            Assert.Equal(9L, UnicodeCommon.Ascii_Hex_To_Value('9'));
            Assert.Equal(1L, UnicodeCommon.Ascii_Hex_To_Value('1'));
            Assert.Equal(0L, UnicodeCommon.Ascii_Hex_To_Value('0'));
        }
    }
}
