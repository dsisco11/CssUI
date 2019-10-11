using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyRect4f
    {
        #region Properties
        public readonly double Top, Right, Bottom, Left;
        #endregion

        #region Constructor
        public ReadOnlyRect4f(int value)
        {
            Top = Right = Bottom = Left = value;
        }

        public ReadOnlyRect4f(double value)
        {
            Top = Right = Bottom = Left = value;
        }

        public ReadOnlyRect4f(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public ReadOnlyRect4f(double top, double right, double bottom, double left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public ReadOnlyRect4f(in ReadOnlyRect4f Other)
        {
            Top = Other.Top;
            Right = Other.Right;
            Bottom = Other.Bottom;
            Left = Other.Left;
        }
        #endregion

        #region Equality
        public static bool operator ==(in ReadOnlyRect4f A, in ReadOnlyRect4f B)
        {
            if (ReferenceEquals(A, B)) return true;

            return (A.Top ==  B.Top) && (A.Right ==  B.Right) && (A.Bottom ==  B.Bottom) && (A.Left ==  B.Left);
        }

        public static bool operator !=(in ReadOnlyRect4f A, in ReadOnlyRect4f B)
        {
            if (ReferenceEquals(A, B)) return false;

            return !(A.Top ==  B.Top) || !(A.Right ==  B.Right) || !(A.Bottom ==  B.Bottom) || !(A.Left ==  B.Left);
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyRect4f val && this == val);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Top, Right, Bottom, Left);
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{Top}, {Right}, {Bottom}, {Left}>";
        }
        #endregion

    }
}
