using System;
using System.Runtime.CompilerServices;

namespace CssUI;


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

        for (UInt64 i = 0; i < POWER_LUT_SIZE; i++)
        {
            POWER_LUT[i] = new UInt64[POWER_LUT_SIZE];
            for (UInt64 j = 0; j < POWER_LUT_SIZE; j++)
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

    #region RangeClamp
    /// <summary>
    /// Clamps a value to a range, automatically determining min/max from a and b.
    /// Unlike Math.Clamp, this handles the case where a > b.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte RangeClamp(Byte value, Byte a, Byte b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SByte RangeClamp(SByte value, SByte a, SByte b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int16 RangeClamp(Int16 value, Int16 a, Int16 b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt16 RangeClamp(UInt16 value, UInt16 a, UInt16 b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 RangeClamp(Int32 value, Int32 a, Int32 b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt32 RangeClamp(UInt32 value, UInt32 a, UInt32 b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 RangeClamp(Int64 value, Int64 a, Int64 b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 RangeClamp(UInt64 value, UInt64 a, UInt64 b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Single RangeClamp(Single value, Single a, Single b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Double RangeClamp(Double value, Double a, Double b) => Math.Max(Math.Min(a, b), Math.Min(Math.Max(a, b), value));
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

    #region Integer Exponents
    /// <summary>
    /// A cached power function which fetches low values from a cache before defaulting to the frameworks implementation
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Pow(Int32 x, UInt32 y)
    {
        if (x >= 0 && x <= POWER_LUT_SIZE && y >= 0 && y <= POWER_LUT_SIZE) return (Int32)POWER_LUT[x][y];
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

