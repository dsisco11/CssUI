namespace CssUI
{
    /// <summary>
    /// Represents a 2D point with double-precision X and Y coordinates.
    /// </summary>
    public record struct Point2f(double X, double Y)
    {
        #region Static Declarations
        public static readonly Point2f Zero = new(0, 0);
        #endregion

        #region Operators
        // ADDITION
        public static Point2f operator +(Point2f A, int Value) => new(A.X + Value, A.Y + Value);
        public static Point2f operator +(Point2f A, double Value) => new(A.X + Value, A.Y + Value);
        public static Point2f operator +(Point2f A, Point2f B) => new(A.X + B.X, A.Y + B.Y);

        // SUBTRACTION
        public static Point2f operator -(Point2f A, int Value) => new(A.X - Value, A.Y - Value);
        public static Point2f operator -(Point2f A, double Value) => new(A.X - Value, A.Y - Value);
        public static Point2f operator -(Point2f A, Point2f B) => new(A.X - B.X, A.Y - B.Y);

        // MULTIPLICATION
        public static Point2f operator *(Point2f A, int Value) => new(A.X * Value, A.Y * Value);
        public static Point2f operator *(Point2f A, double Value) => new(A.X * Value, A.Y * Value);
        public static Point2f operator *(Point2f A, Point2f B) => new(A.X * B.X, A.Y * B.Y);

        // DIVISION
        public static Point2f operator /(Point2f A, int Value) => new(A.X / Value, A.Y / Value);
        public static Point2f operator /(Point2f A, double Value) => new(A.X / Value, A.Y / Value);
        public static Point2f operator /(Point2f A, Point2f B) => new(A.X / B.X, A.Y / B.Y);
        #endregion

        #region Formatting
        public override readonly string ToString() => $"Point2f({X}, {Y})";
        #endregion
    }
}

