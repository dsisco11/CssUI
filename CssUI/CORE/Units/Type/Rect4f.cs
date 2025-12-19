namespace CssUI;

/// <summary>
/// Represents box edges with Top, Right, Bottom, and Left values (e.g., margins, padding, borders).
/// </summary>
public record struct Rect4f(double Top, double Right, double Bottom, double Left)
{
    #region Static Definitions
    public static readonly Rect4f Zero = new(0, 0, 0, 0);
    #endregion

    #region Accessors
    public readonly double Width => Right - Left;
    public readonly double Height => Bottom - Top;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a Rect4f with all edges set to the same value.
    /// </summary>
    public Rect4f(double value) : this(value, value, value, value) { }
    #endregion

    #region Operators
    // ADDITION
    public static Rect4f operator +(Rect4f A, int Value) => new(A.Top + Value, A.Right + Value, A.Bottom + Value, A.Left + Value);
    public static Rect4f operator +(Rect4f A, double Value) => new(A.Top + Value, A.Right + Value, A.Bottom + Value, A.Left + Value);
    public static Rect4f operator +(Rect4f A, Rect4f B) => new(A.Top + B.Top, A.Right + B.Right, A.Bottom + B.Bottom, A.Left + B.Left);

    // SUBTRACTION
    public static Rect4f operator -(Rect4f A, int Value) => new(A.Top - Value, A.Right - Value, A.Bottom - Value, A.Left - Value);
    public static Rect4f operator -(Rect4f A, double Value) => new(A.Top - Value, A.Right - Value, A.Bottom - Value, A.Left - Value);
    public static Rect4f operator -(Rect4f A, Rect4f B) => new(A.Top - B.Top, A.Right - B.Right, A.Bottom - B.Bottom, A.Left - B.Left);

    // MULTIPLICATION
    public static Rect4f operator *(Rect4f A, int Value) => new(A.Top * Value, A.Right * Value, A.Bottom * Value, A.Left * Value);
    public static Rect4f operator *(Rect4f A, double Value) => new(A.Top * Value, A.Right * Value, A.Bottom * Value, A.Left * Value);
    public static Rect4f operator *(Rect4f A, Rect4f B) => new(A.Top * B.Top, A.Right * B.Right, A.Bottom * B.Bottom, A.Left * B.Left);

    // DIVISION
    public static Rect4f operator /(Rect4f A, int Value) => new(A.Top / Value, A.Right / Value, A.Bottom / Value, A.Left / Value);
    public static Rect4f operator /(Rect4f A, double Value) => new(A.Top / Value, A.Right / Value, A.Bottom / Value, A.Left / Value);
    public static Rect4f operator /(Rect4f A, Rect4f B) => new(A.Top / B.Top, A.Right / B.Right, A.Bottom / B.Bottom, A.Left / B.Left);
    #endregion

    #region Formatting
    public override readonly string ToString() => $"Rect4f({Top}, {Right}, {Bottom}, {Left})";
    #endregion
}

