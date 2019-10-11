using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyRect2f
    {
        #region Properties
        public readonly double Width;
        public readonly double Height;
        #endregion

        #region Constructor
        public ReadOnlyRect2f(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public ReadOnlyRect2f(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public ReadOnlyRect2f(ReadOnlyRect2f R)
        {
            Width = R.Width;
            Height = R.Height;
        }
        #endregion

        #region Equality
        public static bool operator ==(in ReadOnlyRect2f A, in ReadOnlyRect2f B)
        {
            if (ReferenceEquals(A, B)) return true;

            return (A.Width ==  B.Width) && (A.Height ==  B.Height);
        }

        public static bool operator !=(in ReadOnlyRect2f A, in ReadOnlyRect2f B)
        {
            if (ReferenceEquals(A, B)) return false;

            return !(A.Width ==  B.Width) || !(A.Height ==  B.Height);
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyRect2f val && this == val);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Width, Height);
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{Width}, {Height}>";
        }
        #endregion

    }
}
