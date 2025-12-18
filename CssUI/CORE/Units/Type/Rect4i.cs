namespace CssUI
{
    /// <summary>
    /// Represents box edges with integer Top, Right, Bottom, and Left values.
    /// </summary>
    public record struct Rect4i(int Top, int Right, int Bottom, int Left)
    {
        #region Static Definitions
        public static readonly Rect4i Zero = new(0, 0, 0, 0);
        #endregion

        #region Accessors
        public readonly int Width => Right - Left;
        public readonly int Height => Bottom - Top;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a Rect4i with all edges set to the same value.
        /// </summary>
        public Rect4i(int value) : this(value, value, value, value) { }
        #endregion

        #region Operators
        // ADDITION
        public static Rect4i operator +(Rect4i A, int Value) => new(A.Top + Value, A.Right + Value, A.Bottom + Value, A.Left + Value);
        public static Rect4i operator +(Rect4i A, Rect4i B) => new(A.Top + B.Top, A.Right + B.Right, A.Bottom + B.Bottom, A.Left + B.Left);

        // SUBTRACTION
        public static Rect4i operator -(Rect4i A, int Value) => new(A.Top - Value, A.Right - Value, A.Bottom - Value, A.Left - Value);
        public static Rect4i operator -(Rect4i A, Rect4i B) => new(A.Top - B.Top, A.Right - B.Right, A.Bottom - B.Bottom, A.Left - B.Left);

        // MULTIPLICATION
        public static Rect4i operator *(Rect4i A, int Value) => new(A.Top * Value, A.Right * Value, A.Bottom * Value, A.Left * Value);
        public static Rect4i operator *(Rect4i A, Rect4i B) => new(A.Top * B.Top, A.Right * B.Right, A.Bottom * B.Bottom, A.Left * B.Left);

        // DIVISION
        public static Rect4i operator /(Rect4i A, int Value) => new(A.Top / Value, A.Right / Value, A.Bottom / Value, A.Left / Value);
        public static Rect4i operator /(Rect4i A, Rect4i B) => new(A.Top / B.Top, A.Right / B.Right, A.Bottom / B.Bottom, A.Left / B.Left);
        #endregion

        #region Formatting
        public override readonly string ToString() => $"Rect4i({Top}, {Right}, {Bottom}, {Left})";
        #endregion
    }
}

