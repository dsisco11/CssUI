using CssUI.Rendering;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SimpleColor : ColorObject<SimpleColor>
    {
        const float fbyteMax = byte.MaxValue;

        #region Instances
        public readonly static SimpleColor MinValue = new SimpleColor(byte.MinValue, byte.MinValue, byte.MinValue);
        public readonly static SimpleColor MaxValue = new SimpleColor(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        #endregion

        #region Properties
        private byte red;
        private byte green;
        private byte blue;

        public byte R { get => red; set => red = value; }
        public byte G { get => green; set => green = value; }
        public byte B { get => blue; set => blue = value; }
        #endregion

        #region Constructors
        public SimpleColor()
        {
            red = green = blue = byte.MinValue;
        }

        /// <summary>
        /// Creates a new color instance from the given values.
        /// </summary>
        public SimpleColor(byte Red, byte Green, byte Blue)
        {
            red = Red;
            green = Green;
            blue = Blue;
        }

        /// <summary>
        /// Creates a new color instance from the given values.
        /// </summary>
        public SimpleColor(int Red, int Green, int Blue)
        {
            red = MathExt.Clamp((byte)Red, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)Green, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)Blue, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Instantiates a new color object with the given values
        /// </summary>
        /// <param name="data">Values to populate the new color object with</param>
        public SimpleColor(Rgba data)
        {
            red = data.Red;
            green = data.Green;
            blue = data.Blue;
        }

        /// <summary>
        /// Returns a new color instance from the given RGBA values scaled from [0-1] to [0-255].
        /// </summary>
        public SimpleColor(Vector4 RGBA)
        {
            var scaled = RGBA * fbyteMax;
            red = MathExt.Clamp((byte)scaled.X, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)scaled.Y, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)scaled.Z, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Returns a new color instance from the given RGBA values scaled from [0-1] to [0-255].
        /// </summary>
        public SimpleColor(float Red, float Green, float Blue)
        {
            var scaled = new Vector3(Red, Green, Blue) * fbyteMax;
            red = MathExt.Clamp((byte)scaled.X, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)scaled.Y, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)scaled.Z, byte.MinValue, byte.MaxValue);
        }
        #endregion

        #region Scalars

        /// <summary>
        /// Returns the RGBA values scaled down to a range of [0.0 - 1.0]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector4 GetVector()
        {
            return new Vector4(red, green, blue, byte.MaxValue) / fbyteMax;
        }

        /// <summary>
        /// Scales up the given RGBA values from a range of [0.0 - 1.0] to [0 - 255] and then assigns those values to the color.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetVector(Vector4 RGBA)
        {
            var scaled = RGBA * fbyteMax;
            red = MathExt.Clamp((byte)scaled.X, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)scaled.Y, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)scaled.Z, byte.MinValue, byte.MaxValue);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Interprets a set of RGB values as a 32-bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override uint AsInteger()
        {
            unsafe
            {
                fixed (byte* ptr = &red)
                {
                    const uint mask = 0xFFFFFF00;
                    return (mask & *((uint*)ptr));
                }
            }
            //return (R << 0) + (G << 8) + (B << 16);
        }

        /// <summary>
        /// Interprets a 32-bit integer as a set of RGB values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override SimpleColor From(uint packed)
        {
            return new SimpleColor(new Rgba(packed));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override SimpleColor From(Vector4 vector) => new SimpleColor(vector);
        #endregion

        #region Serialization
        /// <summary>
        /// Converts the color to a hexadecimal RGB color string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToHexRGB()
        {
            return string.Concat("#",
                                 red.ToString("X2", CultureInfo.InvariantCulture),
                                 green.ToString("X2", CultureInfo.InvariantCulture),
                                 blue.ToString("X2", CultureInfo.InvariantCulture));
        }
        #endregion

        #region Serialization
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Serialize()
        {
            /* Return RGB Hex code */
            return ToHexRGB();
        }
        #endregion

        #region ToString
        public override string ToString() => $"{GetType().Name}{GetVector().ToString("0.###", CultureInfo.InvariantCulture)}";
        #endregion
    }
}
