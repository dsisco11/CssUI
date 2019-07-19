
namespace CssUI
{
    public class Vec3
    {
        #region Properties
        public double X, Y, Z;
        #endregion

        #region Constructors

        public Vec3()
        {
            X = Y = Z = 0;
        }

        public Vec3(float n)
        {
            X = Y = Z = n;
        }

        public Vec3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vec3(double n)
        {
            X = Y = Z = n;
        }

        public Vec3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vec3(int n)
        {
            X = Y = Z = n;
        }

        public Vec3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vec3(long n)
        {
            X = Y = Z = n;
        }

        public Vec3(long x, long y, long z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        #endregion

        #region Operators
        public static Vec3 operator +(Vec3 A, Vec3 B)
        {
            return new Vec3(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
        }

        public static Vec3 operator -(Vec3 A, Vec3 B)
        {
            return new Vec3(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
        }

        public static Vec3 operator *(Vec3 A, Vec3 B)
        {
            return new Vec3(A.X * B.X, A.Y * B.Y, A.Z * B.Z);
        }

        public static Vec3 operator /(Vec3 A, Vec3 B)
        {
            return new Vec3(A.X / B.X, A.Y / B.Y, A.Z / B.Z);
        }

        public static bool operator ==(Vec3 A, Vec3 B)
        {
            return (MathExt.floatEq(A.X, B.X) && MathExt.floatEq(A.Y, B.Y) && MathExt.floatEq(A.Z, B.Z));
        }

        public static bool operator !=(Vec3 A, Vec3 B)
        {
            return !(MathExt.floatEq(A.X, B.X) && MathExt.floatEq(A.Y, B.Y) && MathExt.floatEq(A.Z, B.Z));
        }

        public override bool Equals(object o)
        {

            if (o is Vec3)
            {
                return this == (Vec3)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31 + X.GetHashCode());
            hash = (hash * 31 + Y.GetHashCode());
            hash = (hash * 31 + Z.GetHashCode());
            return hash;
        }
        #endregion
    }
}
