using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace CssUI
{
    class MathExt
    {
        #region Min
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int A, int B) => (A < B ? A : B);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(long A, long B) => (A < B ? A : B);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float A, float B) => (A < B ? A : B);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(double A, double B) => (A < B ? A : B);
        #endregion

        #region Max
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int A, int B) => (A > B ? A : B);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Max(long A, long B) => (A > B ? A : B);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float A, float B) => (A > B ? A : B);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Max(double A, double B) => (A > B ? A : B);
        #endregion

        #region Clamp
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clamp(long value, long min, long max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max) => Max(min, Min(max, value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max) => Max(min, Min(max, value));
        #endregion

        #region RangeClamp
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeClamp(int value, int a, int b) { int min = (a < b ? a : b); int max = (a < b ? b : a); return Max(min, Min(max, value)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RangeClamp(long value, long a, long b) { long min = (a < b ? a : b); long max = (a < b ? b : a); return Max(min, Min(max, value)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RangeClamp(float value, float a, float b) { float min = (a < b ? a : b); float max = (a < b ? b : a); return Max(min, Min(max, value)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RangeClamp(double value, double a, double b) { double min = (a < b ? a : b); double max = (a < b ? b : a); return Max(min, Min(max, value)); }
        #endregion

        #region Distance
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector2 A, Vector2 B)
        {
            double x = (A.X - B.X);
            double y = (A.Y - B.Y);
            return Math.Sqrt((double)((x * x) + (y * y)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector3 A, Vector3 B)
        {
            double x = (A.X - B.X);
            double y = (A.Y - B.Y);
            double z = (A.Z - B.Z);
            return Math.Sqrt((double)((x * x) + (y * y) + (z * z)));
        }
        #endregion
        
        #region Radians / Degrees
        internal const double Radians = (Math.PI / 180.0);
        internal const double Ratio_DegToRad = (Math.PI / 180.0);
        internal const double Ratio_GradToRad = (Math.PI / 200.0);
        internal const double Ratio_TurnToRad = (Math.PI / 0.5);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Ratio_DegToRad;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Ratio_DegToRad;
        }
        #endregion

        /// <summary>
        /// Determines the equality of two floating point values based on the delta between them.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="epsilon"></param>
        /// <returns>True If the delta between the values is less than <paramref name="epsilon"/> </returns>
        public static bool floatEq(double a, double b, double epsilon = 0.0000001f)
        {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < double.Epsilon)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * double.Epsilon);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
    }
}
