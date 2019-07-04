
namespace CssUI
{
    public class Vec2i
    {
        #region Static Declerations
        public static readonly Vec2i Zero = new Vec2i(0, 0);
        #endregion

        #region Properties
        public int X;
        public int Y;
        #endregion

        #region Constructors
        public Vec2i()
        {
            X = Y = 0;
        }
        public Vec2i(int n)
        {
            X = Y = n;
        }
        public Vec2i(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vec2i(Vec2i vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
        }
        #endregion


        #region Operators
        public static Vec2i operator +(Vec2i A, Vec2i B)
        {
            return new Vec2i(A.X + B.X, A.Y + B.Y);
        }

        public static Vec2i operator -(Vec2i A, Vec2i B)
        {
            return new Vec2i(A.X - B.X, A.Y - B.Y);
        }

        public static bool operator ==(Vec2i A, Vec2i B)
        {
            if (ReferenceEquals(A, null) || ReferenceEquals(B, null))
                return (ReferenceEquals(A, null) ^ ReferenceEquals(B, null));

            return ((A.X == B.X) && (A.Y == B.Y));
        }

        public static bool operator !=(Vec2i A, Vec2i B)
        {
            if (ReferenceEquals(A, null) || ReferenceEquals(B, null))
                return !(ReferenceEquals(A, null) ^ ReferenceEquals(B, null));

            return ((A.X != B.X) || (A.Y != B.Y));
        }

        public override bool Equals(object o)
        {
            return (o is Vec2i vec && this == vec);
        }

        public override int GetHashCode()
        {
            return (int)X ^ (int)Y;
        }

        public override string ToString()
        {
            return $"{nameof(Vec2i)}<{X}, {Y}>";
        }
        #endregion

    }
}
