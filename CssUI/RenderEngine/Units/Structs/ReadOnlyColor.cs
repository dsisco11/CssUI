using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CssUI.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReadOnlyColor : IColorObject
    {
        const float fbyteMax = byte.MaxValue;
        static Vector4 ScalarFactor = new Vector4(fbyteMax);

        #region Instances
        public readonly static ReadOnlyColor MinValue = new ReadOnlyColor(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);
        public readonly static ReadOnlyColor MaxValue = new ReadOnlyColor(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        #endregion

        #region Properties
        /// <summary>Red channel value in the range [0-255]</summary>
        private readonly byte red;
        /// <summary>Green channel value in the range [0-255]</summary>
        private readonly byte green;
        /// <summary>Blue channel value in the range [0-255]</summary>
        private readonly byte blue;
        /// <summary>Alpha channel value in the range [0-255]</summary>
        private readonly byte alpha;
        #endregion

        #region Accessors
        public byte R => red;
        public byte G => green;
        public byte B => blue;
        public byte A => alpha;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new color instance from the given values.
        /// </summary>
        public ReadOnlyColor(byte r, byte g, byte b, byte a)
        {
            red = r;
            green = g;
            blue = b;
            alpha = a;
        }

        /// <summary>
        /// Creates a new color instance from the given values.
        /// </summary>
        public ReadOnlyColor(int Red, int Green, int Blue, int? Alpha)
        {
            red = MathExt.Clamp((byte)Red, byte.MinValue, byte.MaxValue);
            green = MathExt.Clamp((byte)Green, byte.MinValue, byte.MaxValue);
            blue = MathExt.Clamp((byte)Blue, byte.MinValue, byte.MaxValue);
            alpha = MathExt.Clamp((byte?)Alpha ?? byte.MaxValue, byte.MinValue, byte.MaxValue);
        }

        /// <summary>
        /// Instantiates a new color object with the given RGBA values
        /// </summary>
        /// <param name="data">Values to populate the new color object with</param>
        public ReadOnlyColor(Rgba data)
        {
            red = data.Red;
            green = data.Green;
            blue = data.Blue;
            alpha = data.Alpha;
        }

        /// <summary>
        /// Returns a new color instance from the given RGBA values scaled from [0-1] to [0-255].
        /// </summary>
        public ReadOnlyColor(Vector4 RGBA)
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
        public ReadOnlyColor(float Red, float Green, float Blue, float? Alpha = null)
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
        /// Returns the RGBA values scaled down to a range of [0.0 - 1.0]
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 GetVector()
        {
            return new Vector4(red, green, blue, alpha) / ScalarFactor;
        }

        /// <summary>
        /// Scales up the given RGBA values from a range of [0.0 - 1.0] to [0 - 255] and then assigns those values to the color.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVector(Vector4 RGBA)
        {
            Contract.Assert(false);
            throw new Exception($"Instances of {nameof(ReadOnlyColor)} are immutable and cannot be changed!");
            Contract.EndContractBlock();
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Interprets a set of RGBA values as a 32-bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint AsInteger()
        {
            unsafe
            {
                fixed (byte* ptr = &red)
                {
                    return *((uint*)ptr);
                }
            }
            // return (R << 0) + (G << 8) + (B << 16) + (A << 24);
        }

        /// <summary>
        /// Interprets a 32-bit integer as a set of RGBA values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyColor FromInt(uint packed)
        {
            return new ReadOnlyColor(new Rgba(packed));
            /*return new ReadOnlyColor((0xFF & n) << 0,
                                     (0x00FF & n) << 8,
                                     (0x0000FF & n) << 16,
                                     (0x000000FF & n) << 24);*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyColor FromVector(Vector4 vector) => new ReadOnlyColor(vector);
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
        public override string ToString() => $"{GetType().Name}{GetVector().ToString("0.###", CultureInfo.InvariantCulture)}";
        #endregion

        #region Casts
        public static implicit operator ReadOnlyColor(int color) => (ReadOnlyColor)color;
        #endregion
    }
}
