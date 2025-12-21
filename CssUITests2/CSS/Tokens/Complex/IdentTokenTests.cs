using CssUI.CSS.Parser;
using Xunit;

namespace CssUITests.CSS.Parser.Tests;

public class IdentTokenTests
{
    [Theory()]
    [InlineData("text"), InlineData("label"), InlineData("input")]
    public void EqualsTest(string Value)
    {
        Assert.Equal(new IdentToken(Value), new IdentToken(Value));
    }

    [Theory()]
    [InlineData("text"), InlineData("label"), InlineData("input")]
    public void EncodeTest(string Value)
    {
        string Actual = new IdentToken(Value).Encode();
        Assert.Equal(Value, Actual);
    }
}
