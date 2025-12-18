namespace CssUI
{
    /// <summary>
    /// Represents a 2D point with integer X and Y coordinates.
    /// </summary>
    public record struct Point2i(int X, int Y)
    {
        #region Static Declarations
        public static readonly Point2i Zero = new(0, 0);
        #endregion

        #region Operators
        // ADDITION
        public static Point2i operator +(Point2i A, int Value) => new(A.X + Value, A.Y + Value);
        public static Point2i operator +(Point2i A, Point2i B) => new(A.X + B.X, A.Y + B.Y);

        // SUBTRACTION
        public static Point2i operator -(Point2i A, int Value) => new(A.X - Value, A.Y - Value);
        public static Point2i operator -(Point2i A, Point2i B) => new(A.X - B.X, A.Y - B.Y);

        // MULTIPLICATION
        public static Point2i operator *(Point2i A, int Value) => new(A.X * Value, A.Y * Value);
        public static Point2i operator *(Point2i A, Point2i B) => new(A.X * B.X, A.Y * B.Y);

        // DIVISION
        public static Point2i operator /(Point2i A, int Value) => new(A.X / Value, A.Y / Value);
        public static Point2i operator /(Point2i A, Point2i B) => new(A.X / B.X, A.Y / B.Y);
        #endregion

        #region Formatting
        public override readonly string ToString() => $"Point2i({X}, {Y})";
        #endregion
    }
}

