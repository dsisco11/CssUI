using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CssUI.Rendering
{
    /// <summary>
    /// Encapsulates an RGBA color.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Color : ColorObject<Color>
    {
        const float fbyteMax = byte.MaxValue;
        static Vector4 ScalarFactor = new Vector4(fbyteMax);

        #region Static members
        public static readonly Color White = new Color(255, 255, 255);
        public static readonly Color Red = new Color(255, 0, 0);
        public static readonly Color Green = new Color(0, 255, 0);
        public static readonly Color Blue = new Color(0, 0, 255);
        public static readonly Color Transparent = new Color(0, 0, 0, 0);// CSS defines transparent as all 0s, it's best we stick to that and if an exception is required make a seperate instance somewhere else.

        public readonly static Color MinValue = new Color(byte.MinValue, byte.MinValue, byte.MinValue);
        public readonly static Color MaxValue = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        #endregion
        
        #region Properties
        /// <summary>Red channel value in the range [0-255]</summary>
        private byte red;
        /// <summary>Green channel value in the range [0-255]</summary>
        private byte green;
        /// <summary>Blue channel value in the range [0-255]</summary>
        private byte blue;
        /// <summary>Alpha channel value in the range [0-255]</summary>
        private byte alpha;
        #endregion

        #region Accessors
        public byte R { get => red; set => red = value; }
        public byte G { get => green; set => green = value; }
        public byte B { get => blue; set => blue = value; }
        public byte A { get => alpha; set => alpha = value; }
        #endregion

        #region Constructors
        public Color()
        {
            red = green = blue = alpha = byte.MinValue;
        }

        /// <summary>
        /// Creates a new color instance from the given values.
        /// </summary>
        public Color(byte Red, byte Green, byte Blue)
        {
            red = Red;
            green = Green;
            blue = Blue;
            alpha = byte.MaxValue;
        }

        /// <summary>
        /// Creates a new color instance from the given values.
        /// </summary>
        public Color(byte Red, byte Green, byte Blue, byte Alpha)
        {
            red = Red;
            green = Green;
            blue = Blue;
            alpha = Alpha;
        }

        public Color(IColorObject color)
        {
            if (color is null) throw new ArgumentNullException(nameof(color));
            Contract.EndContractBlock();

            uint packed = color.AsInteger();
            byte[] rgb = Rgba.Unpack(packed);
            red = rgb[0];
            green = rgb[1];
            blue = rgb[2];
            alpha = rgb[3];
        }

        /// <summary>
        /// Instantiates a new color object with the given packed RGBA values
        /// </summary>
        /// <param name="packed">4 bytes packed into a single 32-bit integer representing RGBA values</param>
        public Color(uint packed)
        {
            var rgb = Rgba.Unpack(packed);
            red = rgb[0];
            green = rgb[1];
            blue = rgb[2];
            alpha = rgb[3];
        }

        /// <summary>
        /// Instantiates a new color object with the given RGBA values
        /// </summary>
        /// <param name="data">Values to populate the new color object with</param>
        public Color(Rgba data)
        {
            red = data.Red;
            green = data.Green;
            blue = data.Blue;
            alpha = data.Alpha;
        }

        /// <summary>
        /// Returns a new color instance from the given RGBA values scaled from [0-1] to [0-255].
        /// </summary>
        public Color(Vector4 RGBA)
        {
            var scaled = RGBA * fbyteMax;
            red = MathExt.Clamp((byte)scaled.X, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)scaled.Y, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)scaled.Z, byte.MinValue, byte.MaxValue);
            alpha = MathExt.Clamp((byte)scaled.W, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Returns a new color instance from the given RGBA values scaled from [0-1] to [0-255].
        /// </summary>
        public Color(float Red, float Green, float Blue, float? Alpha = null)
        {
            var scaled = new Vector3(Red, Green, Blue) * fbyteMax;
            red = MathExt.Clamp((byte)scaled.X, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)scaled.Y, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)scaled.Z, byte.MinValue, byte.MaxValue);
            alpha = MathExt.Clamp((byte?)(Alpha * fbyteMax) ?? byte.MaxValue, byte.MinValue, byte.MaxValue);
        }
        #endregion

        #region Scalars
        /// <summary>
        /// Returns the RGBA values in vector form
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector4 GetVector()
        {
            return new Vector4(R, G, B, A) / ScalarFactor;
        }

        /// <summary>
        /// Converts the given vector into proper RGBA form and overwrites this instances values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetVector(Vector4 RGBA)
        {
            var scaled = RGBA * fbyteMax;
            red = MathExt.Clamp((byte)scaled.X, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)scaled.Y, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)scaled.Z, byte.MinValue, byte.MaxValue);
            alpha = MathExt.Clamp((byte)scaled.W, byte.MinValue, byte.MaxValue);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Interprets a set of RGBA values as a 32-bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override uint AsInteger()
        {
            unsafe
            {
                fixed (void* ptr = &red)
                {
                    return *((uint*)ptr);
                }
            }
        }

        /// <summary>
        /// Interprets a 32-bit integer as a set of RGBA values
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Color From(uint packed)
        {
            return new Color(packed);
            /*
            return new Color((byte)((0xFF & packed) << 0),
                            (byte)((0x00FF & packed) << 8),
                            (byte)((0x0000FF & packed) << 16),
                            (byte)((0x000000FF & packed) << 24));
            */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Color From(Vector4 vector) => new Color(vector);
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

        /// <summary>
        /// Converts the color to a hexadecimal RGBA color string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToHexRGBA()
        {
            return string.Concat("#",
                                 red.ToString("X2", CultureInfo.InvariantCulture),
                                 green.ToString("X2", CultureInfo.InvariantCulture),
                                 blue.ToString("X2", CultureInfo.InvariantCulture),
                                 alpha.ToString("X2", CultureInfo.InvariantCulture));
        }

        public string Serialize()
        {
            if (alpha < byte.MaxValue)
            {
                /* Return RGBA Hex code */
                return ToHexRGBA();
            }

            /* Return RGB Hex code */
            return ToHexRGB();
        }
        #endregion

        #region ToString
        //public override string ToString() => $"{GetType().Name}{GetVector().ToString("0.###", CultureInfo.InvariantCulture)}";
        //public override string ToString() => $"#{AsInteger().ToString("X8", CultureInfo.InvariantCulture)}";
        public override string ToString() => ToHexRGBA();
        #endregion

        #region Casts
        public static implicit operator ReadOnlyColor(Color color) => new ReadOnlyColor(color.red, color.green, color.blue, color.alpha);
        public static implicit operator SimpleColor(Color color) => new SimpleColor(color.red, color.green, color.blue);
        public static implicit operator Color(uint color) => color;
        #endregion
    }
}
