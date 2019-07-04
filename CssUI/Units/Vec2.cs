
namespace CssUI
{
    public class Vec2
    {
        #region Properties
        public double X, Y;
        #endregion

        #region Constructors

        public Vec2()
        {
            X = Y = 0;
        }

        public Vec2(double n)
        {
            X = Y = n;
        }

        public Vec2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2(int n)
        {
            X = Y = n;
        }

        public Vec2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2(long n)
        {
            X = Y = n;
        }

        public Vec2(long x, long y)
        {
            this.X = x;
            this.Y = y;
        }
        #endregion

        #region Operators
        public static Vec2 operator +(Vec2 A, Vec2 B)
        {
            return new Vec2(A.X + B.X, A.Y + B.Y);
        }

        public static Vec2 operator -(Vec2 A, Vec2 B)
        {
            return new Vec2(A.X - B.X, A.Y - B.Y);
        }

        public static bool operator ==(Vec2 A, Vec2 B)
        {
            return (MathExt.floatEq(A.X, B.X) && MathExt.floatEq(A.Y, B.Y));
        }

        public static bool operator !=(Vec2 A, Vec2 B)
        {
            return !(MathExt.floatEq(A.X, B.X) && MathExt.floatEq(A.Y, B.Y));
        }

        public override bool Equals(object o)
        {
            return (o is Vec2 vec && this == vec);
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y;
        }

        public override string ToString()
        {
            return $"{nameof(Vec2)}<{X}, {Y}>";
        }
        #endregion


    }
}
