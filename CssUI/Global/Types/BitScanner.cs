using System;

namespace CssUI
{
    /* Source: https://stackoverflow.com/a/31374629 */
    public static class BitScanner
    {
        private const UInt64 Magic = 0x37E84A99DAE458F;

        private static readonly Int32[] MagicTable =
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
        /// Returns the Most significant bit of the given number
        /// </summary>
        public static int BitScanForward(UInt64 b)
        {
            return MagicTable[((UInt64)((Int64)b & -(Int64)b) * Magic) >> 58];
        }

        /// <summary>
        /// Returns the Least significant bit of the given number
        /// </summary>
        public static int BitScanReverse(UInt64 b)
        {
            b |= b >> 1;
            b |= b >> 2;
            b |= b >> 4;
            b |= b >> 8;
            b |= b >> 16;
            b |= b >> 32;
            b = b & ~(b >> 1);
            return MagicTable[b * Magic >> 58];
        }
    }
}
