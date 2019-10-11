using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyPoint2i
    {
        #region Properties
        public readonly int X;
        public readonly int Y;
        #endregion

        #region Constructor
        public ReadOnlyPoint2i(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public ReadOnlyPoint2i(double x, double y)
        {
            X = (int)x;
            Y = (int)y;
        }

        public ReadOnlyPoint2i(in ReadOnlyPoint2i Other)
        {
            X = Other.X;
            Y = Other.Y;
        }
        #endregion


        #region Equality
        public static bool operator ==(in ReadOnlyPoint2i A, in ReadOnlyPoint2i B)
        {
            if (ReferenceEquals(A, B)) return true;

            return ((A.X == B.X) && (A.Y == B.Y));
        }

        public static bool operator !=(in ReadOnlyPoint2i A, in ReadOnlyPoint2i B)
        {
            if (ReferenceEquals(A, B)) return false;

            return ((A.X != B.X) || (A.Y != B.Y));
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyPoint2i vec && this == vec);
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

    }
}
