using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyPoint3f
    {
        #region Properties
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        #endregion

        #region Constructor
        public ReadOnlyPoint3f(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public ReadOnlyPoint3f(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public ReadOnlyPoint3f(in ReadOnlyPoint3f Other)
        {
            X = Other.X;
            Y = Other.Y;
            Z = Other.Z;
        }
        #endregion

        #region Equality
        public static bool operator ==(in ReadOnlyPoint3f A, in ReadOnlyPoint3f B)
        {
            if (ReferenceEquals(A, B)) return true;

            return ((A.X ==  B.X) && (A.Y ==  B.Y) && (A.Z ==  B.Z));
        }

        public static bool operator !=(in ReadOnlyPoint3f A, in ReadOnlyPoint3f B)
        {
            if (ReferenceEquals(A, B)) return false;

            return !(A.X ==  B.X) || !(A.Y ==  B.Y) || !(A.Z ==  B.Z);
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyPoint3f val && val == this);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(X, Y, Z);
        }
        #endregion

    }
}
