using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CssUI.Rendering
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Rgba
    {
        #region Properties
        private readonly byte red;
        private readonly byte green;
        private readonly byte blue;
        private readonly byte alpha;
        #endregion

        #region Accessors
        public byte Red => red;
        public byte Green => green;
        public byte Blue => blue;
        public byte Alpha => alpha;
        #endregion

        #region Constructors
        public Rgba(uint packed)
        {
            var rgb = Rgba.Unpack(packed);
            red = rgb[0];
            green = rgb[1];
            blue = rgb[2];
            alpha = rgb[3];
        }

        public Rgba(byte red, byte green, byte blue, byte alpha)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.alpha = alpha;
        }
        #endregion

        /// <summary>
        /// Views the 8-bit RGBA values at this objects address as a 32-bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly uint Pack()
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
        /// Packs a set of 8-bit RGBA values into a 32-bit integer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] Unpack(uint packed)
        {
            Contract.Ensures(Contract.Result<byte[]>() != null);
            Contract.Ensures(Contract.Result<byte[]>().Length == 4);

            byte[] bytes = new byte[4];
            fixed (byte* ptr = bytes)
                *((uint*)ptr) = (uint)packed;
            
            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

    }
}
