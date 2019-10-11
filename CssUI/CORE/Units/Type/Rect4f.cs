using System.Runtime.InteropServices;

namespace CssUI
{
    /// <summary>
    /// Represents a 2D rectangle with a coordinate for its Top, Right, Bottom, and Left sides
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Rect4f
    {
        #region Static Definitions
        public static readonly Rect4f Zero = new Rect4f(0, 0, 0, 0);
        #endregion

        #region Properties
        public double Top, Right, Bottom, Left;
        #endregion

        public double Width => Right - Left;
        public double Height => Bottom - Top;


        #region Constructors
        public Rect4f()
        {
            Top = Right = Bottom = Left = 0;
        }

        public Rect4f(int value)
        {
            Top = Right = Bottom = Left = value;
        }

        public Rect4f(double value)
        {
            Top = Right = Bottom = Left = value;
        }

        public Rect4f(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public Rect4f(double top, double right, double bottom, double left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public Rect4f(in ReadOnlyRect4f Other)
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
        public static Rect4f operator +(Rect4f A, int Value)
        {
            return new Rect4f(A.Top + Value,
                              A.Right + Value,
                              A.Bottom + Value,
                              A.Left + Value);
        }
        public static Rect4f operator +(Rect4f A, double Value)
        {
            return new Rect4f(A.Top + Value,
                              A.Right + Value,
                              A.Bottom + Value,
                              A.Left + Value);
        }
        public static Rect4f operator +(Rect4f A, in ReadOnlyRect4f B)
        {
            return new Rect4f(A.Top + B.Top,
                              A.Right + B.Right,
                              A.Bottom + B.Bottom,
                              A.Left + B.Left);
        }

        // SUBTRACTION

        public static Rect4f operator -(Rect4f A, int Value)
        {
            return new Rect4f(A.Top - Value,
                              A.Right - Value,
                              A.Bottom - Value,
                              A.Left - Value);
        }
        public static Rect4f operator -(Rect4f A, double Value)
        {
            return new Rect4f(A.Top - Value,
                              A.Right - Value,
                              A.Bottom - Value,
                              A.Left - Value);
        }
        public static Rect4f operator -(Rect4f A, in ReadOnlyRect4f B)
        {
            return new Rect4f(A.Top - B.Top,
                              A.Right - B.Right,
                              A.Bottom - B.Bottom,
                              A.Left - B.Left);
        }

        // MULTIPLICATION

        public static Rect4f operator *(Rect4f A, int Value)
        {
            return new Rect4f(A.Top * Value,
                              A.Right * Value,
                              A.Bottom * Value,
                              A.Left * Value);
        }
        public static Rect4f operator *(Rect4f A, double Value)
        {
            return new Rect4f(A.Top * Value,
                              A.Right * Value,
                              A.Bottom * Value,
                              A.Left * Value);
        }
        public static Rect4f operator *(Rect4f A, in ReadOnlyRect4f B)
        {
            return new Rect4f(A.Top * B.Top,
                              A.Right * B.Right,
                              A.Bottom * B.Bottom,
                              A.Left * B.Left);
        }

        // DIVISION

        public static Rect4f operator /(Rect4f A, int Value)
        {
            return new Rect4f(A.Top / Value,
                              A.Right / Value,
                              A.Bottom / Value,
                              A.Left / Value);
        }
        public static Rect4f operator /(Rect4f A, double Value)
        {
            return new Rect4f(A.Top / Value,
                              A.Right / Value,
                              A.Bottom / Value,
                              A.Left / Value);
        }
        public static Rect4f operator /(Rect4f A, in ReadOnlyRect4f B)
        {
            return new Rect4f(A.Top / B.Top,
                              A.Right / B.Right,
                              A.Bottom / B.Bottom,
                              A.Left / B.Left);
        }
        #endregion

        #region Equality
        public static bool operator ==(Rect4f A, Rect4f B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return (A.Top ==  B.Top) && (A.Right ==  B.Right) && (A.Bottom ==  B.Bottom) && (A.Left ==  B.Left);
        }

        public static bool operator !=(Rect4f A, Rect4f B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return !(A.Top ==  B.Top) || !(A.Right ==  B.Right) || !(A.Bottom ==  B.Bottom) || !(A.Left ==  B.Left);
        }

        public override bool Equals(object o)
        {
            return (o is Rect4f val && this == val);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{Top}, {Right}, {Bottom}, {Left}>";
        }
        #endregion
        #endregion



        #region Casts
        public static implicit operator ReadOnlyRect4f(Rect4f size)
        {
            return (ReadOnlyRect4f)size;
        }
        #endregion
    }
}
