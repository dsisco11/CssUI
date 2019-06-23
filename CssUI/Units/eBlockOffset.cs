using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Holds four values for Top, Right, Bottom, and Left which represent offsets for each side of an <see cref="eBlock"/>
    /// </summary>
    public class eBlockOffset
    {
        public static readonly eBlockOffset Zero = new eBlockOffset(0);
        public static readonly eBlockOffset One = new eBlockOffset(1);

        #region Values
        private int _left;
        private int _right;
        private int _top;
        private int _bottom;
        #endregion

        #region Accessors
        public int Left
        {
            get { return _left; }
            set
            {
                bool changed = (value != _left);
                _left = value;
                if (changed) onChanged?.Invoke();
            }
        }
        public int Right
        {
            get { return _right; }
            set
            {
                bool changed = (value != _right);
                _right = value;
                if (changed) onChanged?.Invoke();
            }
        }
        public int Top
        {
            get { return _top; }
            set
            {
                bool changed = (value != _top);
                _top = value;
                if (changed) onChanged?.Invoke();
            }
        }
        public int Bottom
        {
            get { return _bottom; }
            set
            {
                bool changed = (value != _bottom);
                _bottom = value;
                if (changed) onChanged?.Invoke();
            }
        }

        public int All {
            get { return (Left + Top + Right + Bottom); }
            set { Left = Top = Right = Bottom = value; }
        }
        public int Horizontal {
            get { return (Left + Right); }
            set { Left = Right = (value / 2); }
        }
        public int Vertical {
            get { return (Top + Bottom); }
            set { Top = Bottom = (value / 2); }
        }
        #endregion

        public event Action onChanged;

        #region Constructors
        public eBlockOffset() { }
        public eBlockOffset(int all)
        {
            Left = Top = Right = Bottom = all;
        }

        public eBlockOffset(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public eBlockOffset(eBlockOffset p)
        {
            Left = p.Left;
            Top = p.Top;
            Right = p.Right;
            Bottom = p.Bottom;
        }
        #endregion

        #region Operators
        public static bool operator ==(eBlockOffset A, eBlockOffset B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.Left == B.Left && A.Top == B.Top && A.Right == B.Right && A.Bottom == B.Bottom);
        }

        public static bool operator !=(eBlockOffset A, eBlockOffset B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return !(object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.Left != B.Left || A.Top != B.Top || A.Right != B.Right || A.Bottom != B.Bottom);
        }

        public override string ToString() { return string.Concat("[", nameof(eBlockOffset), "]<", Left, ", ", Top, ", ", Right, ", ", Bottom, ">"); }

        public override bool Equals(object o)
        {

            if (o is eBlockOffset)
            {
                return this == (eBlockOffset)o;
            }

            return false;
        }
        
        public override int GetHashCode()
        {
            return Left ^ Right ^ Top ^ Bottom;
        }
        #endregion
    }
}
