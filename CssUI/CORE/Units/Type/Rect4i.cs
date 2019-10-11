using System.Runtime.InteropServices;

namespace CssUI
{
    /// <summary>
    /// Represents a 2D rectangle with a width and height but no position
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Rect4i
    {
        #region Static Definitions
        public static readonly Rect4i Zero = new Rect4i(0, 0, 0, 0);
        #endregion

        #region Properties
        public int Top, Right, Bottom, Left;
        #endregion

        #region Constructors
        public Rect4i()
        {
            Top = Right = Bottom = Left = 0;
        }

        public Rect4i(int value)
        {
            Top = Right = Bottom = Left = value;
        }

        public Rect4i(double value)
        {
            Top = Right = Bottom = Left = (int)value;
        }

        public Rect4i(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public Rect4i(double top, double right, double bottom, double left)
        {
            Top = (int)top;
            Right = (int)right;
            Bottom = (int)bottom;
            Left = (int)left;
        }

        public Rect4i(in ReadOnlyRect4i Other)
        {
            Top = Other.Top;
            Right = Other.Right;
            Bottom = Other.Bottom;
            Left = Other.Left;
        }
        #endregion

        #region Operators
        #region Math
        // ADDITION
        public static Rect4i operator +(Rect4i A, int Value)
        {
            return new Rect4i(A.Top + Value,
                              A.Right + Value,
                              A.Bottom + Value,
                              A.Left + Value);
        }
        public static Rect4i operator +(Rect4i A, double Value)
        {
            return new Rect4i(A.Top + Value,
                              A.Right + Value,
                              A.Bottom + Value,
                              A.Left + Value);
        }
        public static Rect4i operator +(Rect4i A, in ReadOnlyRect4i B)
        {
            return new Rect4i(A.Top + B.Top,
                              A.Right + B.Right,
                              A.Bottom + B.Bottom,
                              A.Left + B.Left);
        }

        // SUBTRACTION

        public static Rect4i operator -(Rect4i A, int Value)
        {
            return new Rect4i(A.Top - Value,
                              A.Right - Value,
                              A.Bottom - Value,
                              A.Left - Value);
        }
        public static Rect4i operator -(Rect4i A, double Value)
        {
            return new Rect4i(A.Top - Value,
                              A.Right - Value,
                              A.Bottom - Value,
                              A.Left - Value);
        }
        public static Rect4i operator -(Rect4i A, in ReadOnlyRect4i B)
        {
            return new Rect4i(A.Top - B.Top,
                              A.Right - B.Right,
                              A.Bottom - B.Bottom,
                              A.Left - B.Left);
        }

        // MULTIPLICATION

        public static Rect4i operator *(Rect4i A, int Value)
        {
            return new Rect4i(A.Top * Value,
                              A.Right * Value,
                              A.Bottom * Value,
                              A.Left * Value);
        }
        public static Rect4i operator *(Rect4i A, double Value)
        {
            return new Rect4i(A.Top * Value,
                              A.Right * Value,
                              A.Bottom * Value,
                              A.Left * Value);
        }
        public static Rect4i operator *(Rect4i A, in ReadOnlyRect4i B)
        {
            return new Rect4i(A.Top * B.Top,
                              A.Right * B.Right,
                              A.Bottom * B.Bottom,
                              A.Left * B.Left);
        }

        // DIVISION

        public static Rect4i operator /(Rect4i A, int Value)
        {
            return new Rect4i(A.Top / Value,
                              A.Right / Value,
                              A.Bottom / Value,
                              A.Left / Value);
        }
        public static Rect4i operator /(Rect4i A, double Value)
        {
            return new Rect4i(A.Top / Value,
                              A.Right / Value,
                              A.Bottom / Value,
                              A.Left / Value);
        }
        public static Rect4i operator /(Rect4i A, in ReadOnlyRect4i B)
        {
            return new Rect4i(A.Top / B.Top,
                              A.Right / B.Right,
                              A.Bottom / B.Bottom,
                              A.Left / B.Left);
        }
        #endregion

        #region Equality
        public static bool operator ==(Rect4i A, Rect4i B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return (A.Top == B.Top) && (A.Right == B.Right) && (A.Bottom == B.Bottom) && (A.Left == B.Left);
        }

        public static bool operator !=(Rect4i A, Rect4i B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return (A.Top != B.Top) && (A.Right != B.Right) && (A.Bottom != B.Bottom) && (A.Left != B.Left);
        }

        public override bool Equals(object o)
        {
            return (o is Rect4i val && this == val);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31 + Top.GetHashCode());
            hash = (hash * 31 + Right.GetHashCode());
            hash = (hash * 31 + Bottom.GetHashCode());
            hash = (hash * 31 + Left.GetHashCode());
            return hash;
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{Top}, {Right}, {Bottom}, {Left}>";
        }
        #endregion
        #endregion



        #region Casts
        public static implicit operator ReadOnlyRect4i(Rect4i size)
        {
            return (ReadOnlyRect4i)size;
        }
        #endregion
    }
}
