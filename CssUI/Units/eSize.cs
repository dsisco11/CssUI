
namespace CssUI
{
    /// <summary>
    /// Holds Sizing dimensions for a <see cref="uiElement"/>
    /// </summary>
    public class eSize
    {
        public static eSize Zero = new eSize() { Width = 0, Height = 0 };
        public int Width = 0;
        public int Height = 0;

        #region Constructors
        public eSize() { }
        public eSize(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
        public eSize(System.Drawing.Size sz)
        {
            Width = sz.Width;
            Height = sz.Height;
        }
        public eSize(eSize sz)
        {
            Width = sz.Width;
            Height = sz.Height;
        }
        #endregion

        #region Operators
        public static bool operator ==(eSize x, eSize y)
        {
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return (object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null));
            return (x.Width == y.Width && x.Height == y.Height);
        }
        public static bool operator !=(eSize x, eSize y) { return !(x == y); }

        public override bool Equals(object o)
        {

            if (o is eSize)
            {
                return this == (eSize)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Width ^ Height;
        }

        public static eSize operator +(eSize x, eSize y) { return new eSize(x.Width + y.Width, x.Height + y.Height); }
        public static eSize operator -(eSize x, eSize y) { return new eSize(x.Width - y.Width, x.Height - y.Height); }

        public override string ToString()
        {
            return string.Concat("[", nameof(eSize), "]<", Width, ", ", Height, ">");
        }
        #endregion
    }

    public static class eSize_Ext
    {
        public static eSize Min(this eSize sz, eSize mn)
        {
            if (mn == null) return sz;
            return new eSize()
            {
                Width = MathExt.Min(sz.Width, mn.Width),
                Height = MathExt.Min(sz.Height, mn.Height),
            };
        }
        public static eSize Max(this eSize sz, eSize mx)
        {
            if (mx == null) return sz;
            return new eSize()
            {
                Width = MathExt.Max(sz.Width, mx.Width),
                Height = MathExt.Max(sz.Height, mx.Height),
            };
        }
        public static eSize Clamp(this eSize sz, eSize mn, eSize mx)
        {
            if (mn == null && mx == null)
            {// We have been given no minimum or maximum values, so just return the size
                return sz;
            }
            else if (mn == null)
            {// We have been given no minimum value, so function like Min() instead
                return new eSize()
                {
                    Width = MathExt.Min(sz.Width, mx.Width),
                    Height = MathExt.Min(sz.Height, mx.Height),
                };
            }
            else if (mx == null)
            {// We have been given no maximum value, so function like Max() instead
                return new eSize()
                {
                    Width = MathExt.Max(sz.Width, mn.Width),
                    Height = MathExt.Max(sz.Height, mn.Height),
                };
            }
            else
            {
                return new eSize()
                {
                    Width = MathExt.Clamp(sz.Width, mn.Width, mx.Width),
                    Height = MathExt.Clamp(sz.Height, mn.Height, mx.Height),
                };
            }
        }
    }
}
