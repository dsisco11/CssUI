using System;
using Xunit;

namespace CssUI.Tests;

public class MathExtTests
{
    [Fact()]
    public void MinTest()
    {
        Assert.Equal(0, Math.Min(0, 0));
        Assert.Equal(0, Math.Min(0, 1));
        Assert.Equal(1, Math.Min(1, 2));
        Assert.Equal(2, Math.Min(2, 4));
        Assert.Equal(4, Math.Min(4, 8));
        Assert.Equal(8, Math.Min(8, 16));
        Assert.Equal(16, Math.Min(16, 32));
        Assert.Equal(32, Math.Min(32, 64));
        Assert.Equal(Int32.MinValue, Math.Min(Int32.MinValue, Int32.MaxValue));
    }

    [Fact()]
    public void MaxTest()
    {
        Assert.Equal(0, Math.Max(0, 0));
        Assert.Equal(1, Math.Max(0, 1));
        Assert.Equal(2, Math.Max(1, 2));
        Assert.Equal(4, Math.Max(2, 4));
        Assert.Equal(8, Math.Max(4, 8));
        Assert.Equal(16, Math.Max(8, 16));
        Assert.Equal(32, Math.Max(16, 32));
        Assert.Equal(64, Math.Max(32, 64));
        Assert.Equal(Int32.MaxValue, Math.Max(Int32.MinValue, Int32.MaxValue));
    }

    [Fact()]
    public void ClampTest()
    {
        Assert.Equal(0, Math.Clamp(-1, 0, 1));
        Assert.Equal(8, Math.Clamp(0x00, 8, 16));
        Assert.Equal(16, Math.Clamp(0xFF, 8, 16));
        Assert.Equal(Int32.MinValue, Math.Clamp(-0xFFFFFFFF, Int32.MinValue, Int32.MaxValue));
        Assert.Equal(Int32.MaxValue, Math.Clamp(0xFFFFFFFF, Int32.MinValue, Int32.MaxValue));
    }

    [Fact()]
    public void RangeClampTest()
    {
        Assert.Equal(0, MathExt.RangeClamp(-1, 0, 1));
        Assert.Equal(0, MathExt.RangeClamp(-1, 1, 0));

        Assert.Equal(8, MathExt.RangeClamp(0x00, 16, 8));
        Assert.Equal(8, MathExt.RangeClamp(0x00, 8, 16));

        Assert.Equal(16, MathExt.RangeClamp(0xFF, 16, 8));
        Assert.Equal(16, MathExt.RangeClamp(0xFF, 8, 16));

        Assert.Equal(Int32.MinValue, MathExt.RangeClamp(-0xFFFFFFFF, Int32.MaxValue, Int32.MinValue));
        Assert.Equal(Int32.MinValue, MathExt.RangeClamp(-0xFFFFFFFF, Int32.MinValue, Int32.MaxValue));

        Assert.Equal(Int32.MaxValue, MathExt.RangeClamp(0xFFFFFFFF, Int32.MaxValue, Int32.MinValue));
        Assert.Equal(Int32.MaxValue, MathExt.RangeClamp(0xFFFFFFFF, Int32.MinValue, Int32.MaxValue));
    }

    [Fact()]
    public void DegreesToRadiansTest()
    {
        Assert.Equal(0, MathExt.DegreesToRadians(0));
        Assert.Equal((float)Math.PI * 0.5f, MathExt.DegreesToRadians(90));
        Assert.Equal((float)Math.PI * 1.0f, MathExt.DegreesToRadians(180));
        Assert.Equal((float)Math.PI * 1.5f, MathExt.DegreesToRadians(270));
        Assert.Equal((float)Math.PI * 2.0f, MathExt.DegreesToRadians(360));
    }

    [Fact()]
    public void PowTest()
    {
        Assert.Equal(2, MathExt.Pow(2, 1));
        Assert.Equal(4, MathExt.Pow(2, 2));
        Assert.Equal(8, MathExt.Pow(2, 3));
        Assert.Equal(16, MathExt.Pow(2, 4));
        Assert.Equal(32, MathExt.Pow(2, 5));
        Assert.Equal(64, MathExt.Pow(2, 6));
        Assert.Equal(128, MathExt.Pow(2, 7));
        Assert.Equal(256, MathExt.Pow(2, 8));
    }

    [Fact()]
    public void NPowTest()
    {
        Assert.Equal(0.5, MathExt.NPow(2, 1));
        Assert.Equal(0.25, MathExt.NPow(2, 2));
        Assert.Equal(0.125, MathExt.NPow(2, 3));
        Assert.Equal(0.0625, MathExt.NPow(2, 4));
        Assert.Equal(0.03125, MathExt.NPow(2, 5));
        Assert.Equal(0.015625, MathExt.NPow(2, 6));
        Assert.Equal(0.0078125, MathExt.NPow(2, 7));
        Assert.Equal(0.00390625, MathExt.NPow(2, 8));
    }

}
