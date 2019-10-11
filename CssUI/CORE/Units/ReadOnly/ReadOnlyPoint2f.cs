using System;
using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyPoint2f
    {
        #region Properties
        public readonly double X;
        public readonly double Y;
        #endregion

        #region Constructor
        public ReadOnlyPoint2f(int x, int y)
        {
            X = x;
            Y = y;
        }

        public ReadOnlyPoint2f(double x, double y)
        {
            X = x;
            Y = y;
        }

        public ReadOnlyPoint2f(in ReadOnlyPoint2f Other)
        {
            X = Other.X;
            Y = Other.Y;
        }
        #endregion

        #region Equality
        public static bool operator ==(in ReadOnlyPoint2f A, in ReadOnlyPoint2f B)
        {
            if (ReferenceEquals(A, B)) return true;

            return (A.X == B.X) && (A.Y == B.Y);
        }

        public static bool operator !=(in ReadOnlyPoint2f A, in ReadOnlyPoint2f B)
        {
            if (ReferenceEquals(A, B)) return false;

            return !(A.X == B.X) || !(A.Y == B.Y);
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyPoint2f vec && this == vec);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, GetHashCode(), Y.GetHashCode());
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{X}, {Y}>";
        }
        #endregion
    }
}
