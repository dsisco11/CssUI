using System;
using System.Runtime.InteropServices;

namespace CssUI
{
    /// <summary>
    /// Represents a 2D rectangle with a width and height but no position
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Rect2i
    {
        #region Static Definitions
        public static readonly Rect2i Zero = new Rect2i(0, 0);
        #endregion

        #region Properties
        public int Width;
        public int Height;
        #endregion

        #region Constructors
        public Rect2i()
        {
            Width = Height = 0;
        }
        public Rect2i(int n)
        {
            Width = Height = n;
        }
        public Rect2i(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public Rect2i(double width, double height)
        {
            Width = (int)width;
            Height = (int)height;
        }
        public Rect2i(in ReadOnlyRect2i Other)
        {
            Width = Other.Width;
            Height = Other.Height;
        }
        #endregion

        #region Operators
        #region Math
        // ADDITION
        public static Rect2i operator +(Rect2i A, int Value)
        {
            return new Rect2i(A.Width + Value,
                              A.Height + Value);
        }
        public static Rect2i operator +(Rect2i A, double Value)
        {
            return new Rect2i(A.Width + (int)Value,
                              A.Height + (int)Value);
        }
        public static Rect2i operator +(Rect2i A, in ReadOnlyRect2i B)
        {
            return new Rect2i(A.Width + B.Width,
                              A.Height + B.Height);
        }

        // SUBTRACTION
        public static Rect2i operator -(Rect2i A, int Value)
        {
            return new Rect2i(A.Width - Value,
                              A.Height - Value);
        }
        public static Rect2i operator -(Rect2i A, double Value)
        {
            return new Rect2i(A.Width - (int)Value,
                              A.Height - (int)Value);
        }
        public static Rect2i operator -(Rect2i A, in ReadOnlyRect2i B)
        {
            return new Rect2i(A.Width - B.Width,
                              A.Height - B.Height);
        }

        // MULTIPLICATION
        public static Rect2i operator *(Rect2i A, int Value)
        {
            return new Rect2i(A.Width * Value,
                              A.Height * Value);
        }
        public static Rect2i operator *(Rect2i A, double Value)
        {
            return new Rect2i(A.Width * (int)Value,
                              A.Height * (int)Value);
        }
        public static Rect2i operator *(Rect2i A, in ReadOnlyRect2i B)
        {
            return new Rect2i(A.Width * B.Width,
                              A.Height * B.Height);
        }

        // DIVISION
        public static Rect2i operator /(Rect2i A, int Value)
        {
            return new Rect2i(A.Width / Value,
                              A.Height / Value);
        }
        public static Rect2i operator /(Rect2i A, double Value)
        {
            return new Rect2i(A.Width / (int)Value,
                              A.Height / (int)Value);
        }
        public static Rect2i operator /(Rect2i A, in ReadOnlyRect2i B)
        {
            return new Rect2i(A.Width / B.Width,
                              A.Height / B.Height);
        }
        #endregion

        #region Equality
        public static bool operator ==(Rect2i A, Rect2i B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return (A.Width == B.Width) && (A.Height == B.Height);
        }

        public static bool operator !=(Rect2i A, Rect2i B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return (A.Width != B.Width) || (A.Height != B.Height);
        }

        public override bool Equals(object o)
        {
            return (o is Rect2i val && this == val);
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
        #endregion


        #region Bounds Limiting

        /// <summary>
        /// Returns the smallest dimensions of this size and the one given
        /// </summary>
        /// <param name="mn"></param>
        /// <returns></returns>
        public Rect2i Min(Rect2i mn)
        {
            if (mn is null) return this;
            return new Rect2i()
            {
                Width = MathExt.Min(Width, mn.Width),
                Height = MathExt.Min(Height, mn.Height),
            };
        }

        /// <summary>
        /// Returns the largest dimensions of this size and the one given
        /// </summary>
        /// <param name="mx"></param>
        /// <returns></returns>
        public Rect2i Max(Rect2i mx)
        {
            if (mx is null) return this;
            return new Rect2i()
            {
                Width = MathExt.Max(Width, mx.Width),
                Height = MathExt.Max(Height, mx.Height),
            };
        }

        /// <summary>
        /// Clamps this sizes dimensions to the min and max given
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="mx"></param>
        /// <returns></returns>
        public Rect2i Clamp(Rect2i mn, Rect2i mx)
        {
            if (mn is null && mx is null)
            {// We have been given no minimum or maximum values, so just return the size
                return this;
            }
            else if (mn is null)
            {// We have been given no minimum value, so function like Min() instead
                return new Rect2i()
                {
                    Width = MathExt.Min(Width, mx.Width),
                    Height = MathExt.Min(Height, mx.Height),
                };
            }
            else if (mx is null)
            {// We have been given no maximum value, so function like Max() instead
                return new Rect2i()
                {
                    Width = MathExt.Max(Width, mn.Width),
                    Height = MathExt.Max(Height, mn.Height),
                };
            }
            else
            {
                return new Rect2i()
                {
                    Width = MathExt.Clamp(Width, mn.Width, mx.Width),
                    Height = MathExt.Clamp(Height, mn.Height, mx.Height),
                };
            }
        }
        #endregion


        #region Casts
        public static implicit operator ReadOnlyRect2i(Rect2i size)
        {
            return (ReadOnlyRect2i)size;
        }
        #endregion
    }
}
