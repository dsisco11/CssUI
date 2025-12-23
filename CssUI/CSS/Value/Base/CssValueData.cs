using System.Runtime.InteropServices;

namespace CssUI.CSS;

/// <summary>
/// A union struct for storing CSS value data without boxing for primitive types.
/// Uses explicit layout to overlay different value types at the same memory location.
/// </summary>
/// <remarks>
/// Layout:
/// - Offset 0-19: Overlapped primitive storage (double, int, CssColor, CssColorHdr)
/// - Offset 24: Object reference for strings and complex types
///
/// The CssColorHdr is the largest primitive at 20 bytes (4×float + 1 byte enum).
/// We use offset 24 for the object reference to ensure proper alignment on 64-bit systems.
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = 32)]
internal struct CssValueData
{
    #region Overlapped Fields (Offset 0)
    /// <summary>
    /// Storage for double/number values.
    /// </summary>
    [FieldOffset(0)]
    public double NumberValue;

    /// <summary>
    /// Storage for integer values.
    /// </summary>
    [FieldOffset(0)]
    public long IntegerValue;

    /// <summary>
    /// Storage for 8-bit RGBA color values.
    /// </summary>
    [FieldOffset(0)]
    public CssColor ColorValue;

    /// <summary>
    /// Storage for HDR color values (wide gamut).
    /// </summary>
    [FieldOffset(0)]
    public CssColorHdr ColorHdrValue;
    #endregion

    #region Reference Field (Offset 24)
    /// <summary>
    /// Storage for reference types (strings, arrays, complex objects).
    /// </summary>
    [FieldOffset(24)]
    public object? ObjectValue;
    #endregion

    #region Factory Methods
    /// <summary>
    /// Creates a <see cref="CssValueData"/> containing a double value.
    /// </summary>
    public static CssValueData FromNumber(double value)
    {
        var data = new CssValueData();
        data.NumberValue = value;
        return data;
    }

    /// <summary>
    /// Creates a <see cref="CssValueData"/> containing an integer value.
    /// </summary>
    public static CssValueData FromInteger(long value)
    {
        var data = new CssValueData();
        data.IntegerValue = value;
        return data;
    }

    /// <summary>
    /// Creates a <see cref="CssValueData"/> containing a color value.
    /// </summary>
    public static CssValueData FromColor(CssColor value)
    {
        var data = new CssValueData();
        data.ColorValue = value;
        return data;
    }

    /// <summary>
    /// Creates a <see cref="CssValueData"/> containing an HDR color value.
    /// </summary>
    public static CssValueData FromColorHdr(CssColorHdr value)
    {
        var data = new CssValueData();
        data.ColorHdrValue = value;
        return data;
    }

    /// <summary>
    /// Creates a <see cref="CssValueData"/> containing an object reference.
    /// </summary>
    public static CssValueData FromObject(object? value)
    {
        var data = new CssValueData();
        data.ObjectValue = value;
        return data;
    }
    #endregion
}
