using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Point2i
    {
        #region Static Declerations
        public static readonly Point2i Zero = new Point2i(0, 0);
        #endregion

        #region Properties
        public int X;
        public int Y;
        #endregion

        #region Constructors
        public Point2i()
        {
            X = Y = 0;
        }
        public Point2i(int n)
        {
            X = Y = n;
        }
        public Point2i(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Point2i(long n)
        {
            X = Y = (int)n;
        }
        public Point2i(long X, long Y)
        {
            this.X = (int)X;
            this.Y = (int)Y;
        }
        public Point2i(ReadOnlyPoint2i vec)
        {
            X = vec.X;
            Y = vec.Y;
        }
        #endregion


        #region Operators
        #region Math
        // ADDITION
        public static Point2i operator +(Point2i A, int Value)
        {
            return new Point2i(A.X + Value,
                               A.Y + Value);
        }
        public static Point2i operator +(Point2i A, double Value)
        {
            return new Point2i(A.X + (int)Value,
                               A.Y + (int)Value);
        }
        public static Point2i operator +(Point2i A, in ReadOnlyPoint2i B)
        {
            return new Point2i(A.X + B.X,
                               A.Y + B.Y);
        }

        // SUBTRACTION
        public static Point2i operator -(Point2i A, int Value)
        {
            return new Point2i(A.X - Value,
                               A.Y - Value);
        }
        public static Point2i operator -(Point2i A, double Value)
        {
            return new Point2i(A.X - (int)Value,
                               A.Y - (int)Value);
        }
        public static Point2i operator -(Point2i A, in ReadOnlyPoint2i B)
        {
            return new Point2i(A.X - B.X,
                               A.Y - B.Y);
        }

        // MULTIPLICATION
        public static Point2i operator *(Point2i A, int Value)
        {
            return new Point2i(A.X * Value,
                               A.Y * Value);
        }
        public static Point2i operator *(Point2i A, double Value)
        {
            return new Point2i(A.X * (int)Value,
                               A.Y * (int)Value);
        }
        public static Point2i operator *(Point2i A, in ReadOnlyPoint2i B)
        {
            return new Point2i(A.X * B.X,
                               A.Y * B.Y);
        }

        // DIVISION
        public static Point2i operator /(Point2i A, int Value)
        {
            return new Point2i(A.X / Value,
                               A.Y / Value);
        }
        public static Point2i operator /(Point2i A, double Value)
        {
            return new Point2i(A.X / (int)Value,
                               A.Y / (int)Value);
        }
        public static Point2i operator /(Point2i A, in ReadOnlyPoint2i B)
        {
            return new Point2i(A.X / B.X,
                               A.Y / B.Y);
        }
        #endregion

        #region Equality
        public static bool operator ==(Point2i A, Point2i B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return ((A.X == B.X) && (A.Y == B.Y));
        }

        public static bool operator !=(Point2i A, Point2i B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return ((A.X != B.X) || (A.Y != B.Y));
        }

        public override bool Equals(object o)
        {
            return (o is Point2i vec && this == vec);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31 + X.GetHashCode());
            hash = (hash * 31 + Y.GetHashCode());
            return hash;
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{X}, {Y}>";
        }
        #endregion
        #endregion

        public static implicit operator ReadOnlyPoint2i(Point2i point)
        {
            return (ReadOnlyPoint2i)point;
        }
    }
}
