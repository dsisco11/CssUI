using System;

namespace CssUI
{
    /// <summary>
    /// Represents a 2D size with integer Width and Height (no position).
    /// </summary>
    public record struct Rect2i(int Width, int Height)
    {
        #region Static Definitions
        public static readonly Rect2i Zero = new(0, 0);
        #endregion

        #region Operators
        // ADDITION
        public static Rect2i operator +(Rect2i A, int Value) => new(A.Width + Value, A.Height + Value);
        public static Rect2i operator +(Rect2i A, Rect2i B) => new(A.Width + B.Width, A.Height + B.Height);

        // SUBTRACTION
        public static Rect2i operator -(Rect2i A, int Value) => new(A.Width - Value, A.Height - Value);
        public static Rect2i operator -(Rect2i A, Rect2i B) => new(A.Width - B.Width, A.Height - B.Height);

        // MULTIPLICATION
        public static Rect2i operator *(Rect2i A, int Value) => new(A.Width * Value, A.Height * Value);
        public static Rect2i operator *(Rect2i A, Rect2i B) => new(A.Width * B.Width, A.Height * B.Height);

        // DIVISION
        public static Rect2i operator /(Rect2i A, int Value) => new(A.Width / Value, A.Height / Value);
        public static Rect2i operator /(Rect2i A, Rect2i B) => new(A.Width / B.Width, A.Height / B.Height);
        #endregion

        #region Bounds Limiting
        /// <summary>
        /// Returns the smallest dimensions of this size and the one given.
        /// </summary>
        public readonly Rect2i Min(Rect2i? mn)
        {
            if (mn is null) return this;
            return new Rect2i(
                Math.Min(Width, mn.Value.Width),
                Math.Min(Height, mn.Value.Height));
        }

        /// <summary>
        /// Returns the largest dimensions of this size and the one given.
        /// </summary>
        public readonly Rect2i Max(Rect2i? mx)
        {
            if (mx is null) return this;
            return new Rect2i(
                Math.Max(Width, mx.Value.Width),
                Math.Max(Height, mx.Value.Height));
        }

        /// <summary>
        /// Clamps this size's dimensions to the min and max given.
        /// </summary>
        public readonly Rect2i Clamp(Rect2i? mn, Rect2i? mx)
        {
            if (mn is null && mx is null)
            {
                return this;
            }
            else if (mn is null)
            {
                return new Rect2i(
                    Math.Min(Width, mx!.Value.Width),
                    Math.Min(Height, mx.Value.Height));
            }
            else if (mx is null)
            {
                return new Rect2i(
                    Math.Max(Width, mn.Value.Width),
                    Math.Max(Height, mn.Value.Height));
            }
            else
            {
                return new Rect2i(
                    Math.Clamp(Width, mn.Value.Width, mx.Value.Width),
                    Math.Clamp(Height, mn.Value.Height, mx.Value.Height));
            }
        }
        #endregion

        #region Formatting
        public override readonly string ToString() => $"Rect2i({Width}, {Height})";
        #endregion
    }
}

