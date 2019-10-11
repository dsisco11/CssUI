
namespace CssUI
{
    public class Point3f
    {
        #region Properties
        public double X, Y, Z;
        #endregion

        #region Constructors

        public Point3f()
        {
            X = Y = Z = 0;
        }

        public Point3f(in float n)
        {
            X = Y = Z = n;
        }

        public Point3f(in float x, in float y, in float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3f(double n)
        {
            X = Y = Z = n;
        }

        public Point3f(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3f(int n)
        {
            X = Y = Z = n;
        }

        public Point3f(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3f(long n)
        {
            X = Y = Z = n;
        }

        public Point3f(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3f(in ReadOnlyPoint3f Other)
        {
            X = Other.X;
            Y = Other.Y;
            Z = Other.Z;
        }
        #endregion

        #region Operators
        #region Math
        // ADDITION
        public static Point3f operator +(Point3f A, int Value)
        {
            return new Point3f(A.X + Value,
                               A.Y + Value,
                               A.Z + Value);
        }
        public static Point3f operator +(Point3f A, double Value)
        {
            return new Point3f(A.X + Value,
                               A.Y + Value,
                               A.Z + Value);
        }
        public static Point3f operator +(Point3f A, in ReadOnlyPoint3f B)
        {
            return new Point3f(A.X + B.X,
                               A.Y + B.Y,
                               A.Z + B.Z);
        }

        // SUBTRACTION
        public static Point3f operator -(Point3f A, int Value)
        {
            return new Point3f(A.X - Value,
                               A.Y - Value,
                               A.Z - Value);
        }
        public static Point3f operator -(Point3f A, double Value)
        {
            return new Point3f(A.X - Value,
                               A.Y - Value,
                               A.Z - Value);
        }
        public static Point3f operator -(Point3f A, in ReadOnlyPoint3f B)
        {
            return new Point3f(A.X - B.X,
                               A.Y - B.Y,
                               A.Z - B.Z);
        }

        // MULTIPLICATION
        public static Point3f operator *(Point3f A, int Value)
        {
            return new Point3f(A.X * Value,
                               A.Y * Value,
                               A.Z * Value);
        }
        public static Point3f operator *(Point3f A, double Value)
        {
            return new Point3f(A.X * Value,
                               A.Y * Value,
                               A.Z * Value);
        }
        public static Point3f operator *(Point3f A, in ReadOnlyPoint3f B)
        {
            return new Point3f(A.X * B.X,
                               A.Y * B.Y,
                               A.Z * B.Z);
        }

        // DIVISION
        public static Point3f operator /(Point3f A, int Value)
        {
            return new Point3f(A.X / Value,
                               A.Y / Value,
                               A.Z / Value);
        }
        public static Point3f operator /(Point3f A, double Value)
        {
            return new Point3f(A.X / Value,
                               A.Y / Value,
                               A.Z / Value);
        }
        public static Point3f operator /(Point3f A, in ReadOnlyPoint3f B)
        {
            return new Point3f(A.X / B.X,
                               A.Y / B.Y,
                               A.Z / B.Z);
        }
        #endregion

        #region Equality
        public static bool operator ==(Point3f A, Point3f B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return ((A.X ==  B.X) && (A.Y ==  B.Y) && (A.Z ==  B.Z));
        }

        public static bool operator !=(Point3f A, Point3f B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return !(A.X ==  B.X) || !(A.Y ==  B.Y) || !(A.Z ==  B.Z);
        }

        public override bool Equals(object o)
        {
            return (o is Point3f val && val == this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
        #endregion

        public static implicit operator ReadOnlyPoint3f(Point3f point)
        {
            return (ReadOnlyPoint3f)point;
        }
    }
}
