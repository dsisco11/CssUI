using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ReadOnlyRect4i
    {
        #region Properties
        public readonly int Top, Right, Bottom, Left;
        #endregion

        #region Constructor
        public ReadOnlyRect4i(int value)
        {
            Top = Right = Bottom = Left = value;
        }

        public ReadOnlyRect4i(double value)
        {
            Top = Right = Bottom = Left = (int)value;
        }

        public ReadOnlyRect4i(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public ReadOnlyRect4i(double top, double right, double bottom, double left)
        {
            Top = (int)top;
            Right = (int)right;
            Bottom = (int)bottom;
            Left = (int)left;
        }

        public ReadOnlyRect4i(in ReadOnlyRect4i Other)
        {
            Top = Other.Top;
            Right = Other.Right;
            Bottom = Other.Bottom;
            Left = Other.Left;
        }
        #endregion

        #region Equality
        public static bool operator ==(in ReadOnlyRect4i A, in ReadOnlyRect4i B)
        {
            if (ReferenceEquals(A, B)) return true;

            return (A.Top == B.Top) && (A.Right == B.Right) && (A.Bottom == B.Bottom) && (A.Left == B.Left);
        }

        public static bool operator !=(in ReadOnlyRect4i A, in ReadOnlyRect4i B)
        {
            if (ReferenceEquals(A, B)) return false;

            return (A.Top != B.Top) && (A.Right != B.Right) && (A.Bottom != B.Bottom) && (A.Left != B.Left);
        }

        public override bool Equals(object o)
        {
            return (o is ReadOnlyRect4i val && this == val);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31 + Top.GetHashCode());
            hash = (hash * 31 + Right.GetHashCode());
            hash = (hash * 31 + Bottom.GetHashCode());
            hash = (hash * 31 + Left.GetHashCode());
            return hash;
        }

        public override string ToString()
        {
            return $"{GetType().Name}<{Top}, {Right}, {Bottom}, {Left}>";
        }
        #endregion

    }
}
