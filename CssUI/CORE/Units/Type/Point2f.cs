using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Point2f
    {
        #region Static Declerations
        public static readonly Point2f Zero = new Point2f(0, 0);
        #endregion

        #region Properties
        public double X, Y;
        #endregion

        #region Constructors

        public Point2f()
        {
            X = Y = 0;
        }

        public Point2f(double n)
        {
            X = Y = n;
        }

        public Point2f(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point2f(int n)
        {
            X = Y = n;
        }

        public Point2f(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point2f(long n)
        {
            X = Y = n;
        }

        public Point2f(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Point2f(in ReadOnlyPoint2f Other)
        {
            X = Other.X;
            Y = Other.Y;
        }
        #endregion

        #region Operators
        #region Math
        // ADDITION
        public static Point2f operator +(Point2f A, int Value)
        {
            return new Point2f(A.X + Value,
                               A.Y + Value);
        }
        public static Point2f operator +(Point2f A, double Value)
        {
            return new Point2f(A.X + Value,
                               A.Y + Value);
        }
        public static Point2f operator +(Point2f A, in ReadOnlyPoint2f B)
        {
            return new Point2f(A.X + B.X,
                               A.Y + B.Y);
        }

        // SUBTRACTION
        public static Point2f operator -(Point2f A, int Value)
        {
            return new Point2f(A.X - Value,
                               A.Y - Value);
        }
        public static Point2f operator -(Point2f A, double Value)
        {
            return new Point2f(A.X - Value,
                               A.Y - Value);
        }
        public static Point2f operator -(Point2f A, in ReadOnlyPoint2f B)
        {
            return new Point2f(A.X - B.X,
                               A.Y - B.Y);
        }

        // MULTIPLICATION
        public static Point2f operator *(Point2f A, int Value)
        {
            return new Point2f(A.X * Value,
                               A.Y * Value);
        }
        public static Point2f operator *(Point2f A, double Value)
        {
            return new Point2f(A.X * Value,
                               A.Y * Value);
        }
        public static Point2f operator *(Point2f A, in ReadOnlyPoint2f B)
        {
            return new Point2f(A.X * B.X,
                               A.Y * B.Y);
        }

        // DIVISION
        public static Point2f operator /(Point2f A, int Value)
        {
            return new Point2f(A.X / Value,
                               A.Y / Value);
        }
        public static Point2f operator /(Point2f A, double Value)
        {
            return new Point2f(A.X / Value,
                               A.Y / Value);
        }
        public static Point2f operator /(Point2f A, in ReadOnlyPoint2f B)
        {
            return new Point2f(A.X / B.X,
                               A.Y / B.Y);
        }
        #endregion

        #region Equality
        public static bool operator ==(Point2f A, Point2f B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return (A.X ==  B.X) && (A.Y ==  B.Y);
        }

        public static bool operator !=(Point2f A, Point2f B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return !(A.X ==  B.X) || !(A.Y ==  B.Y);
        }

        public override bool Equals(object o)
        {
            return (o is Point2f vec && this == vec);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{X}, {Y}>";
        }
        #endregion
        #endregion

        public static implicit operator ReadOnlyPoint2f(Point2f point)
        {
            return (ReadOnlyPoint2f)point;
        }
    }
}
