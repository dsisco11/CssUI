using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CssUI
{

#pragma warning disable IDE0049
    public static class MathExt
    {
        #region Static
        const Int32 POWER_LUT_SIZE = 15;
        private static readonly UInt64[][] POWER_LUT;
        private static readonly Double[][] FRACTION_POWER_LUT;
        static MathExt()
        {
            POWER_LUT = new UInt64[POWER_LUT_SIZE][];

            for (UInt64 i=0; i < POWER_LUT_SIZE; i++)
            {
                POWER_LUT[i] = new UInt64[POWER_LUT_SIZE];
                for (UInt64 j =0; j < POWER_LUT_SIZE; j++)
                {
                    POWER_LUT[i][j] = (UInt64)Math.Pow(i, j);
                }
            }


            FRACTION_POWER_LUT = new Double[POWER_LUT_SIZE][];
            for (UInt64 i = 0; i < POWER_LUT_SIZE; i++)
            {
                FRACTION_POWER_LUT[i] = new Double[POWER_LUT_SIZE];
                for (UInt64 j = 0; j < POWER_LUT_SIZE; j++)
                {
                    FRACTION_POWER_LUT[i][j] = Math.Pow(i, -(Double)j);
                }
            }
        }
        #endregion

        #region Min
        /* We use these instead of the normal Math class because these are aggressively inlined and avoid a jmp call and an eq comparison (usually negligable but its an effortless improvement nonetheless) */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte Min(Byte x, Byte y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte Min(SByte x, SByte y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 Min(Int16 x, Int16 y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 Min(UInt16 x, UInt16 y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Min(Int32 x, Int32 y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Min(UInt32 x, UInt32 y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 Min(Int64 x, Int64 y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 Min(UInt64 x, UInt64 y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Single Min(Single x, Single y) => (x < y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double Min(Double x, Double y) => (x < y ? x : y);
        #endregion

        #region Max
        /* We use these instead of the normal Math class because these are aggressively inlined and avoid a jmp call and an eq comparison (usually negligable but its an effortless improvement nonetheless) */
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte Max(Byte x, Byte y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte Max(SByte x, SByte y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 Max(Int16 x, Int16 y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 Max(UInt16 x, UInt16 y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Max(Int32 x, Int32 y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Max(UInt32 x, UInt32 y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 Max(Int64 x, Int64 y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 Max(UInt64 x, UInt64 y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Single Max(Single x, Single y) => (x > y ? x : y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double Max(Double x, Double y) => (x > y ? x : y);
        #endregion

        #region Clamp
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte Clamp(Byte value, Byte min, Byte max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte Clamp(SByte value, SByte min, SByte max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 Clamp(Int16 value, Int16 min, Int16 max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 Clamp(UInt16 value, UInt16 min, UInt16 max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Clamp(Int32 value, Int32 min, Int32 max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Clamp(UInt32 value, UInt32 min, UInt32 max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 Clamp(Int64 value, Int64 min, Int64 max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 Clamp(UInt64 value, UInt64 min, UInt64 max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Single Clamp(Single value, Single min, Single max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double Clamp(Double value, Double min, Double max) => Max(min, Min(max, value));
        #endregion

        #region RangeClamp
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte RangeClamp(Byte value, Byte a, Byte b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte RangeClamp(SByte value, SByte a, SByte b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 RangeClamp(Int16 value, Int16 a, Int16 b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 RangeClamp(UInt16 value, UInt16 a, UInt16 b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 RangeClamp(Int32 value, Int32 a, Int32 b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RangeClamp(UInt32 value, UInt32 a, UInt32 b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 RangeClamp(Int64 value, Int64 a, Int64 b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 RangeClamp(UInt64 value, UInt64 a, UInt64 b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Single RangeClamp(Single value, Single a, Single b) => Max(Min(a, b), Min(Max(a, b), value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double RangeClamp(Double value, Double a, Double b) => Max(Min(a, b), Min(Max(a, b), value));
        #endregion
        
        #region Radians / Degrees
        internal const Double Radians = (Math.PI / 180.0);
        internal const Double Ratio_DegToRad = (Math.PI / 180.0);
        internal const Double Ratio_GradToRad = (Math.PI / 200.0);
        internal const Double Ratio_TurnToRad = (Math.PI / 0.5);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Single DegreesToRadians(Single degrees)
        {
            return degrees * (Single)Ratio_DegToRad;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double DegreesToRadians(Double degrees)
        {
            return degrees * Ratio_DegToRad;
        }
        #endregion

#if false
        #region Floating-Point equality with epsilon tolerance
        /// <summary>
        /// Returns <c>True</c> if <paramref name="x"/> is equal to <paramref name="y"/> within a range of error <paramref name="epsilon"/>
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool Feq(Double x, Double y)
        {
            Double absA = Math.Abs(x);
            Double absB = Math.Abs(y);
            Double diff = absA - absB;

            if (x == y)
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < Double.Epsilon)
            {
                return true;
            }
            else if (x == 0 || y == 0)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < Double.Epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < Double.Epsilon;
            }
        }
        /// <summary>
        /// Returns <c>True</c> if <paramref name="x"/> is equal to <paramref name="y"/> within a range of error <paramref name="epsilon"/>
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool Feq(Double x, Double y, Double epsilon)
        {
            Double absA = Math.Abs(x);
            Double absB = Math.Abs(y);
            Double diff = absA - absB;

            if (x == y)
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < Double.Epsilon)
            {
                return true;
            }
            else if (x == 0 || y == 0)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < Double.Epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }


        /// <summary>
        /// Returns <c>True</c> if <paramref name="x"/> is less than or equal to <paramref name="y"/> within a range of error <paramref name="epsilon"/>
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool Flteq(Double x, Double y)
        {
            Double absA = Math.Abs(x);
            Double absB = Math.Abs(y);
            Double diff = absA - absB;

            if (x <= y)
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < Double.Epsilon)
            {
                return true;
            }
            else if (x == 0 || y == 0)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < Double.Epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < Double.Epsilon;
            }
        }
        /// <summary>
        /// Returns <c>True</c> if <paramref name="x"/> is less than or equal to <paramref name="y"/> within a range of error <paramref name="epsilon"/>
        /// </summary>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool Flteq(Double x, Double y, Double epsilon)
        {
            Double absA = Math.Abs(x);
            Double absB = Math.Abs(y);
            Double diff = absA - absB;

            if (x <= y)
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < Double.Epsilon)
            {
                return true;
            }
            else if (x == 0 || y == 0)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < Double.Epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }


        /// <summary>
        /// Returns <c>True</c> if <paramref name="x"/> is greater than or equal to <paramref name="y"/> within a range of error <paramref name="epsilon"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool Fgteq(Double x, Double y)
        {
            Double absA = Math.Abs(x);
            Double absB = Math.Abs(y);
            Double diff = absA - absB;

            if (x >= y)
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < Double.Epsilon)
            {
                return true;
            }
            else if (x == 0 || y == 0)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < Double.Epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < Double.Epsilon;
            }
        }
        /// <summary>
        /// Returns <c>True</c> if <paramref name="x"/> is greater than or equal to <paramref name="y"/> within a range of error <paramref name="epsilon"/>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool Fgteq(Double x, Double y, Double epsilon)
        {
            Double absA = Math.Abs(x);
            Double absB = Math.Abs(y);
            Double diff = absA - absB;

            if (x >= y)
            { // shortcut, handles infinities
                return true;
            }
            else if (diff < Double.Epsilon)
            {
                return true;
            }
            else if (x == 0 || y == 0)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < Double.Epsilon;
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
        #endregion
#endif
        #region Integer Exponents
        /// <summary>
        /// A cached power function which fetches low values from a cache before defaulting to the frameworks implementation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Pow(Int32 x, UInt32 y)
        {
            if (x >=0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE) return (Int32)POWER_LUT[x][y];
            /*Int32 n = 1;

            while (true)
            {
                if ((y & 1) != 0) n = x * n;
                y = y >> 1;
                if (y == 0) return n;
                x *= x;
            }*/
            return (Int32)Math.Pow(x, y);
        }

        /// <summary>
        /// A cached power function which fetches low values from a cache before defaulting to the frameworks implementation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Pow(UInt32 x, UInt32 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE) return (UInt32)POWER_LUT[x][y];
            /*UInt32 n = 1;

            while (true)
            {
                if ((y & 1) != 0) n = x * n;
                y = y >> 1;
                if (y == 0) return n;
                x *= x;
            }*/
            return (UInt32)Math.Pow(x, y);
        }

        /// <summary>
        /// A cached power function which fetches low values from a cache before defaulting to the frameworks implementation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 Pow(Int64 x, UInt64 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE) return (Int64)POWER_LUT[x][y];
            /*Int64 n = 1;

            while (true)
            {
                if ((y & 1) != 0) n = x * n;
                y = y >> 1;
                if (y == 0) return n;
                x *= x;
            }*/
            return (Int64)Math.Pow(x, y);
        }

        /// <summary>
        /// A cached power function which fetches low values from a cache before defaulting to the frameworks implementation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 Pow(UInt64 x, UInt64 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE) return POWER_LUT[x][y];
            /*UInt64 n = 1;

            while (true)
            {
                if ((y & 1) != 0) n = x * n;
                y = y >> 1;
                if (y == 0) return n;
                x *= x;
            }*/

            return (UInt64)Math.Pow(x, y);
        }
        #endregion

        #region Fractional Exponents
        /// <summary>
        /// A cached wrapper around Math.Pow which specifically calculates the Negative power
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double NPow(Int32 x, UInt32 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE)
                return FRACTION_POWER_LUT[x][y];
            return Math.Pow(x, -(Double)y);
        }

        /// <summary>
        /// A cached wrapper around Math.Pow which specifically calculates the Negative power
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double NPow(UInt32 x, UInt32 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE)
                return FRACTION_POWER_LUT[x][y];
            return Math.Pow(x, -(Double)y);
        }

        /// <summary>
        /// A cached wrapper around Math.Pow which specifically calculates the Negative power
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double NPow(Int64 x, UInt64 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE)
                return FRACTION_POWER_LUT[x][y];
            return Math.Pow(x, -(Double)y);
        }

        /// <summary>
        /// A cached wrapper around Math.Pow which specifically calculates the Negative power
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Double NPow(UInt64 x, UInt64 y)
        {
            if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE)
                return FRACTION_POWER_LUT[x][y];
            return Math.Pow(x, -(Double)y);
        }
        #endregion
    }
}
