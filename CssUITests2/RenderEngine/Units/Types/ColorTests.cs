using Xunit;
using CssUI.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace CssUI.Rendering.Tests
{
    public class ColorTests
    {
        const uint MASK_WHITE = 0xFFFFFFFFu;
        const uint MASK_RED = 0xFF0000FFu;
        const uint MASK_GREEN = 0xFF00FF00u;
        const uint MASK_BLUE = 0xFFFF0000u;
        const uint MASK_BLACK = 0x00000000u;

        [Fact(DisplayName = "Color: Constructor(uint)")]
        public void ColorTest()
        {
            Assert.Equal(new Color(MASK_WHITE), Color.White);
            Assert.Equal(new Color(MASK_RED), Color.Red);
            Assert.Equal(new Color(MASK_GREEN), Color.Green);
            Assert.Equal(new Color(MASK_BLUE), Color.Blue);
            Assert.Equal(new Color(MASK_BLACK), Color.Transparent);
        }

        [Fact(DisplayName = "Color: Constructor(Rgba)")]
        public void ColorTest1()
        {
            Assert.Equal(new Color(new Rgba(MASK_WHITE)), Color.White);
            Assert.Equal(new Color(new Rgba(MASK_RED)), Color.Red);
            Assert.Equal(new Color(new Rgba(MASK_GREEN)), Color.Green);
            Assert.Equal(new Color(new Rgba(MASK_BLUE)), Color.Blue);
            Assert.Equal(new Color(new Rgba(MASK_BLACK)), Color.Transparent);
        }

        [Fact(DisplayName = "Color: Constructor(IColorObject)")]
        public void ColorTest2()
        {
            Assert.Equal(new Color(new Color(new Rgba(MASK_WHITE))), Color.White);
            Assert.Equal(new Color(new Color(new Rgba(MASK_RED))), Color.Red);
            Assert.Equal(new Color(new Color(new Rgba(MASK_GREEN))), Color.Green);
            Assert.Equal(new Color(new Color(new Rgba(MASK_BLUE))), Color.Blue);
            Assert.Equal(new Color(new Color(new Rgba(MASK_BLACK))), Color.Transparent);
        }

        [Fact(DisplayName = "Color: Constructor(Vector4)")]
        public void ColorTest3()
        {
            Assert.Equal(new Color(new Vector4(1f, 1f, 1f, 1f)), Color.White);
            Assert.Equal(new Color(new Vector4(1f, 0f, 0f, 1f)), Color.Red);
            Assert.Equal(new Color(new Vector4(0f, 1f, 0f, 1f)), Color.Green);
            Assert.Equal(new Color(new Vector4(0f, 0f, 1f, 1f)), Color.Blue);
            Assert.Equal(new Color(new Vector4(0f, 0f, 0f, 0f)), Color.Transparent);
        }

        [Fact(DisplayName = "Color: Constructor(R, G, B, A)")]
        public void ColorTest4()
        {
            Assert.Equal(new Color(1f, 1f, 1f, 1f), Color.White);
            Assert.Equal(new Color(1f, 0f, 0f, 1f), Color.Red);
            Assert.Equal(new Color(0f, 1f, 0f, 1f), Color.Green);
            Assert.Equal(new Color(0f, 0f, 1f, 1f), Color.Blue);
            Assert.Equal(new Color(0f, 0f, 0f, 0f), Color.Transparent);
        }

        [Fact(DisplayName = "Color: GetVector")]
        public void GetVectorTest()
        {
            Assert.Equal(new Color(new Vector4(1f, 1f, 1f, 1f)).GetVector(), Color.White.GetVector());
            Assert.Equal(new Color(new Vector4(1f, 0f, 0f, 1f)).GetVector(), Color.Red.GetVector());
            Assert.Equal(new Color(new Vector4(0f, 1f, 0f, 1f)).GetVector(), Color.Green.GetVector());
            Assert.Equal(new Color(new Vector4(0f, 0f, 1f, 1f)).GetVector(), Color.Blue.GetVector());
            Assert.Equal(new Color(new Vector4(0f, 0f, 0f, 0f)).GetVector(), Color.Transparent.GetVector());
        }

        [Fact(DisplayName = "Color: SetVector")]
        public void SetVectorTest()
        {
            var C = new Color();
            C.SetVector(new Vector4(1f, 1f, 1f, 1f));
            Assert.Equal(C, Color.White);
            C.SetVector(new Vector4(1f, 0f, 0f, 1f));
            Assert.Equal(C, Color.Red);
            C.SetVector(new Vector4(0f, 1f, 0f, 1f));
            Assert.Equal(C, Color.Green);
            C.SetVector(new Vector4(0f, 0f, 1f, 1f));
            Assert.Equal(C, Color.Blue);
            C.SetVector(new Vector4(0f, 0f, 0f, 0f));
            Assert.Equal(C, Color.Transparent);
        }

        [Fact(DisplayName = "Color: AsInteger")]
        public void AsIntegerTest()
        {
            Assert.Equal(MASK_WHITE, Color.White.AsInteger());
            Assert.Equal(MASK_RED, Color.Red.AsInteger());
            Assert.Equal(MASK_GREEN, Color.Green.AsInteger());
            Assert.Equal(MASK_BLUE, Color.Blue.AsInteger());
            Assert.Equal(MASK_BLACK, Color.Transparent.AsInteger());
        }

        [Fact(DisplayName = "Color: From(uint)")]
        public void From_UINT_Test()
        {
            Color C = new Color();
            Assert.Equal(C.From(MASK_WHITE), Color.White);
            Assert.Equal(C.From(MASK_RED), Color.Red);
            Assert.Equal(C.From(MASK_GREEN), Color.Green);
            Assert.Equal(C.From(MASK_BLUE), Color.Blue);
            Assert.Equal(C.From(MASK_BLACK), Color.Transparent);
        }

        [Fact(DisplayName = "Color: From(Vector4)")]
        public void From_Vector_Test()
        {
            Color C = new Color();
            Assert.Equal(C.From(new Vector4(1f, 1f, 1f, 1f)), Color.White);
            Assert.Equal(C.From(new Vector4(1f, 0f, 0f, 1f)), Color.Red);
            Assert.Equal(C.From(new Vector4(0f, 1f, 0f, 1f)), Color.Green);
            Assert.Equal(C.From(new Vector4(0f, 0f, 1f, 1f)), Color.Blue);
            Assert.Equal(C.From(new Vector4(0f, 0f, 0f, 0f)), Color.Transparent);
        }

        [Fact(DisplayName = "Color: ToHexRGB")]
        public void ToHexRGBTest()
        {
            Assert.Equal("#FFFFFF", Color.White.ToHexRGB());
            Assert.Equal("#FF0000", Color.Red.ToHexRGB());
            Assert.Equal("#00FF00", Color.Green.ToHexRGB());
            Assert.Equal("#0000FF", Color.Blue.ToHexRGB());
            Assert.Equal("#000000", Color.Transparent.ToHexRGB());
        }

        [Fact(DisplayName = "Color: ToHexRGBA")]
        public void ToHexRGBATest()
        {
            Assert.Equal("#FFFFFFFF", Color.White.ToHexRGBA());
            Assert.Equal("#FF0000FF", Color.Red.ToHexRGBA());
            Assert.Equal("#00FF00FF", Color.Green.ToHexRGBA());
            Assert.Equal("#0000FFFF", Color.Blue.ToHexRGBA());
            Assert.Equal("#00000000", Color.Transparent.ToHexRGBA());
        }

        [Fact(DisplayName = "Color: Serialize")]
        public void SerializeTest()
        {
            Assert.Equal("#FFFFFF", Color.White.Serialize());
            Assert.Equal("#FF0000", Color.Red.Serialize());
            Assert.Equal("#00FF00", Color.Green.Serialize());
            Assert.Equal("#0000FF", Color.Blue.Serialize());
            Assert.Equal("#00000000", Color.Transparent.Serialize());
        }

        [Fact(DisplayName = "Color: ToString")]
        public void ToStringTest()
        {
            Assert.Equal($"#FFFFFFFF", Color.White.ToString());
            Assert.Equal($"#FF0000FF", Color.Red.ToString());
            Assert.Equal($"#00FF00FF", Color.Green.ToString());
            Assert.Equal($"#0000FFFF", Color.Blue.ToString());
            Assert.Equal($"#00000000", Color.Transparent.ToString());
        }
    }
}