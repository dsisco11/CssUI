using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyRect2i
    {
        #region Properties
        public readonly int Width;
        public readonly int Height;
        #endregion

        #region Constructor
        public ReadOnlyRect2i(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public ReadOnlyRect2i(double width, double height)
        {
            Width = (int)width;
            Height = (int)height;
        }

        public ReadOnlyRect2i(in ReadOnlyRect2i R)
        {
            Width = R.Width;
            Height = R.Height;
        }
        #endregion

        #region Equality
        public static bool operator ==(in ReadOnlyRect2i A, in ReadOnlyRect2i B)
        {
            if (ReferenceEquals(A, B)) return true;

            return (A.Width == B.Width) && (A.Height == B.Height);
        }

        public static bool operator !=(in ReadOnlyRect2i A, in ReadOnlyRect2i B)
        {
            if (ReferenceEquals(A, B)) return false;

            return (A.Width != B.Width) || (A.Height != B.Height);
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyRect2i val && this == val);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31 + Width.GetHashCode());
            hash = (hash * 31 + Height.GetHashCode());
            return hash;
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{Width}, {Height}>";
        }
        #endregion

    }
}
