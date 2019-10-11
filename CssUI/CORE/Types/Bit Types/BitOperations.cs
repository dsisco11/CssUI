using System;
using System.Runtime.CompilerServices;

namespace CssUI
{
    public static class BitOperations
    {
        #region Population Count
        /// <summary>
        /// The population count of a binary integer value is the number of one bits in the value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopulationCount(UInt32 input)
        {/* Docs: http://aggregate.org/MAGIC/#Population%20Count%20(Ones%20Count) */
            if (input == 0) return 0;
            /* 32-bit recursive reduction using SWAR...
	           but first step is mapping 2-bit values
	           into sum of 2 1-bit values in sneaky way
	        */
            input -= ((input >> 1) & 0x55555555);
            input = (((input >> 2) & 0x33333333) + (input & 0x33333333));
            input = (((input >> 4) + input) & 0x0f0f0f0f);
            input += (input >> 8);
            input += (input >> 16);
            return (Int32)(input & 0x0000003f);
        }

        /// <summary>
        /// The population count of a binary integer value is the number of one bits in the value.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopulationCount(Int32 input) => PopulationCount((UInt32)input);
        #endregion

        #region Leading Zeros
        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="input"/>
        /// value.
        /// Returns 32 if the specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// </summary>
        /// <returns>Number of leading zero bits in the integer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountLeadingZeros(UInt32 input)
        {// XXX: Once net standard 2.2 is out; add a hardware intrinsic path to this
            if (input == 0) return 32;

            UInt32 n = 1;

            if ((input >> 16) == 0) { n += 16; input <<= 16; }
            if ((input >> 24) == 0) { n += 8; input <<= 8; }
            if ((input >> 28) == 0) { n += 4; input <<= 4; }
            if ((input >> 30) == 0) { n += 2; input <<= 2; }
            n -= (input >> 31);

            return (Int32)n;
        }

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="input"/>
        /// value.
        /// Returns 32 if the specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// </summary>
        /// <returns>Number of leading zero bits in the integer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountLeadingZeros(Int32 input) => CountLeadingZeros((UInt32)input);

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="input"/>
        /// value.
        /// Returns 64 if the specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// </summary>
        /// <returns>Number of leading zero bits in the integer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountLeadingZeros(UInt64 input)
        {// XXX: Once net standard 2.2 is out; add a hardware intrinsic path to this
            if (input == 0) return 64;

            UInt64 n = 1;

            if ((input >> 32) == 0) { n += 32; input <<= 32; }
            if ((input >> 48) == 0) { n += 16; input <<= 16; }
            if ((input >> 56) == 0) { n += 8; input <<= 8; }
            if ((input >> 60) == 0) { n += 4; input <<= 4; }
            if ((input >> 62) == 0) { n += 2; input <<= 2; }
            n -= (input >> 63);

            return (Int32)n;
        }

        /// <summary>
        /// Returns the number of zero bits preceding the highest-order
        /// ("leftmost") one-bit in the two's complement binary representation
        /// of the specified <paramref name="input"/>
        /// value.
        /// Returns 64 if the specified value has no one-bits in its two's complement representation,
        /// in other words if it is equal to zero.
        /// </summary>
        /// <returns>Number of leading zero bits in the integer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountLeadingZeros(Int64 input) => CountLeadingZeros((UInt64)input);
        #endregion

        #region Trailing Zeros
        private static readonly Int32[] MultiplyDeBruijnBitPosition32 =
        {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };
        /// <summary>
        /// Returns the number of trailing zero bits in a binary integer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountTrailingZeros(UInt32 input)
        {// XXX: Once net standard 2.2 is out; add a hardware intrinsic path to this
            return MultiplyDeBruijnBitPosition32[((UInt32)(((Int32)input & -(Int32)input) * 0x077CB531U)) >> 27];
        }

        /// <summary>
        /// Returns the number of trailing zero bits in a binary integer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountTrailingZeros(Int32 input) => CountTrailingZeros((UInt32)input);


        private static readonly Int32[] MultiplyDeBruijnBitPosition64 =
        {
            0, 1, 17, 2, 18, 50, 3, 57,
            47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6,
            61, 44, 12, 35, 60, 11, 10, 9,
        };
        /// <summary>
        /// Returns the number of trailing zero bits in a binary integer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountTrailingZeros(UInt64 input)
        {// XXX: Once net standard 2.2 is out; add a hardware intrinsic path to this
            return MultiplyDeBruijnBitPosition64[((UInt64)((Int64)input & -(Int64)input) * 0x37E84A99DAE458F) >> 58];
        }

        /// <summary>
        /// Returns the number of trailing zero bits in a binary integer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 CountTrailingZeros(Int64 input) => CountTrailingZeros((UInt64)input);
        #endregion
    }
}
