using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CssUI.CSS.Parser;

/// <summary>
/// A union struct for storing numeric token data without boxing.
/// Uses explicit layout to overlay int and double at the same memory location.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly struct NumericTokenData
{
    /// <summary>
    /// Storage for integer values.
    /// </summary>
    [FieldOffset(0)]
    public readonly int IntegerValue;

    /// <summary>
    /// Storage for double/number values.
    /// </summary>
    [FieldOffset(0)]
    public readonly double NumberValue;

    #region Constructors
    private NumericTokenData(int value)
    {
        // Initialize double first to zero out all bytes
        NumberValue = 0;
        IntegerValue = value;
    }

    private NumericTokenData(double value)
    {
        // Initialize int first to zero out all bytes
        IntegerValue = 0;
        NumberValue = value;
    }
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates a <see cref="NumericTokenData"/> containing an integer value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericTokenData FromInteger(int value) => new NumericTokenData(value);

    /// <summary>
    /// Creates a <see cref="NumericTokenData"/> containing a double value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumericTokenData FromNumber(double value) => new NumericTokenData(value);
    #endregion
}
