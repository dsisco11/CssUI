
namespace CssUI
{
    /// <summary>
    /// Represents a 2D size
    /// </summary>
    public class Size2D
    {
        #region Static Definitions
        public static readonly Size2D Zero = new Size2D(0, 0);
        #endregion

        #region Properties
        public int Width;
        public int Height;
        #endregion

        #region Constructors
        public Size2D()
        {
            Width = Height = 0;
        }
        public Size2D(int n)
        {
            Width = Height = n;
        }
        public Size2D(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
        public Size2D(Size2D size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }
        #endregion

        #region Operators
        public static Size2D operator +(Size2D A, Size2D B)
        {
            return new Size2D(A.Width + B.Width, A.Height + B.Height);
        }

        public static Size2D operator -(Size2D A, Size2D B)
        {
            return new Size2D(A.Width - B.Width, A.Height - B.Height);
        }

        public static bool operator ==(Size2D A, Size2D B)
        {
            if (ReferenceEquals(A, null) || ReferenceEquals(B, null))
                return (ReferenceEquals(A, null) ^ ReferenceEquals(B, null));

            return ((A.Width == B.Width) && (A.Height == B.Height));
        }

        public static bool operator !=(Size2D A, Size2D B)
        {
            if (ReferenceEquals(A, null) || ReferenceEquals(B, null))
                return !(ReferenceEquals(A, null) ^ ReferenceEquals(B, null));

            return ((A.Width != B.Width) || (A.Height != B.Height));
        }

        public override bool Equals(object o)
        {
            return (o is Size2D vec && this == vec);
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


        #region Bounds Limiting

        /// <summary>
        /// Returns the smallest dimensions of this size and the one given
        /// </summary>
        /// <param name="mn"></param>
        /// <returns></returns>
        public Size2D Min(Size2D mn)
        {
            if (ReferenceEquals(mn, null)) return this;
            return new Size2D()
            {
                Width = MathExt.Min(this.Width, mn.Width),
                Height = MathExt.Min(this.Height, mn.Height),
            };
        }

        /// <summary>
        /// Returns the largest dimensions of this size and the one given
        /// </summary>
        /// <param name="mx"></param>
        /// <returns></returns>
        public Size2D Max(Size2D mx)
        {
            if (ReferenceEquals(mx, null)) return this;
            return new Size2D()
            {
                Width = MathExt.Max(this.Width, mx.Width),
                Height = MathExt.Max(this.Height, mx.Height),
            };
        }

        /// <summary>
        /// Clamps this sizes dimensions to the min and max given
        /// </summary>
        /// <param name="sz"></param>
        /// <param name="mn"></param>
        /// <param name="mx"></param>
        /// <returns></returns>
        public Size2D Clamp(Size2D mn, Size2D mx)
        {
            if (ReferenceEquals(mn, null) && ReferenceEquals(mx, null))
            {// We have been given no minimum or maximum values, so just return the size
                return this;
            }
            else if (ReferenceEquals(mn, null))
            {// We have been given no minimum value, so function like Min() instead
                return new Size2D()
                {
                    Width = MathExt.Min(this.Width, mx.Width),
                    Height = MathExt.Min(this.Height, mx.Height),
                };
            }
            else if (ReferenceEquals(mx, null))
            {// We have been given no maximum value, so function like Max() instead
                return new Size2D()
                {
                    Width = MathExt.Max(this.Width, mn.Width),
                    Height = MathExt.Max(this.Height, mn.Height),
                };
            }
            else
            {
                return new Size2D()
                {
                    Width = MathExt.Clamp(this.Width, mn.Width, mx.Width),
                    Height = MathExt.Clamp(this.Height, mn.Height, mx.Height),
                };
            }
        }
        #endregion


        #region Casts
        public static implicit operator ReadOnlySize2i(Size2D size) => new ReadOnlySize2i(size.Width, size.Height);
        #endregion
    }
}
