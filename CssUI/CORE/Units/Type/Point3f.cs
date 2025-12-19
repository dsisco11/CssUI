namespace CssUI;

/// <summary>
/// Represents a 3D point with double-precision X, Y, and Z coordinates.
/// </summary>
public record struct Point3f(double X, double Y, double Z)
{
    #region Static Declarations
    public static readonly Point3f Zero = new(0, 0, 0);
    #endregion

    #region Operators
    // ADDITION
    public static Point3f operator +(Point3f A, int Value) => new(A.X + Value, A.Y + Value, A.Z + Value);
    public static Point3f operator +(Point3f A, double Value) => new(A.X + Value, A.Y + Value, A.Z + Value);
    public static Point3f operator +(Point3f A, Point3f B) => new(A.X + B.X, A.Y + B.Y, A.Z + B.Z);

    // SUBTRACTION
    public static Point3f operator -(Point3f A, int Value) => new(A.X - Value, A.Y - Value, A.Z - Value);
    public static Point3f operator -(Point3f A, double Value) => new(A.X - Value, A.Y - Value, A.Z - Value);
    public static Point3f operator -(Point3f A, Point3f B) => new(A.X - B.X, A.Y - B.Y, A.Z - B.Z);

    // MULTIPLICATION
    public static Point3f operator *(Point3f A, int Value) => new(A.X * Value, A.Y * Value, A.Z * Value);
    public static Point3f operator *(Point3f A, double Value) => new(A.X * Value, A.Y * Value, A.Z * Value);
    public static Point3f operator *(Point3f A, Point3f B) => new(A.X * B.X, A.Y * B.Y, A.Z * B.Z);

    // DIVISION
    public static Point3f operator /(Point3f A, int Value) => new(A.X / Value, A.Y / Value, A.Z / Value);
    public static Point3f operator /(Point3f A, double Value) => new(A.X / Value, A.Y / Value, A.Z / Value);
    public static Point3f operator /(Point3f A, Point3f B) => new(A.X / B.X, A.Y / B.Y, A.Z / B.Z);
    #endregion

    #region Formatting
    public override readonly string ToString() => $"Point3f({X}, {Y}, {Z})";
    #endregion
}

