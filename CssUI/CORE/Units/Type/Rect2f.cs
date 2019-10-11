using System;
using System.Runtime.InteropServices;

namespace CssUI
{
    /// <summary>
    /// Represents a 2D rectangle with a width and height but no position
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Rect2f
    {
        #region Static Definitions
        public static readonly Rect2f Zero = new Rect2f(0, 0);
        #endregion

        #region Properties
        public double Width;
        public double Height;
        #endregion

        #region Constructors
        public Rect2f()
        {
            Width = Height = 0;
        }

        public Rect2f(int n)
        {
            Width = Height = n;
        }

        public Rect2f(double n)
        {
            Width = Height = n;
        }

        public Rect2f(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Rect2f(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public Rect2f(in ReadOnlyRect2f Other)
        {
            Width = Other.Width;
            Height = Other.Height;
        }
        #endregion

        #region Operators
        #region Math
        // ADDITION
        public static Rect2f operator +(Rect2f A, int Value)
        {
            return new Rect2f(A.Width + Value,
                              A.Height + Value);
        }
        public static Rect2f operator +(Rect2f A, double Value)
        {
            return new Rect2f(A.Width + Value,
                              A.Height + Value);
        }
        public static Rect2f operator +(Rect2f A, in ReadOnlyRect2f B)
        {
            return new Rect2f(A.Width + B.Width,
                              A.Height + B.Height);
        }

        // SUBTRACTION
        public static Rect2f operator -(Rect2f A, int Value)
        {
            return new Rect2f(A.Width - Value,
                              A.Height - Value);
        }
        public static Rect2f operator -(Rect2f A, double Value)
        {
            return new Rect2f(A.Width - Value,
                              A.Height - Value);
        }
        public static Rect2f operator -(Rect2f A, in ReadOnlyRect2f B)
        {
            return new Rect2f(A.Width - B.Width,
                              A.Height - B.Height);
        }

        // MULTIPLICATION
        public static Rect2f operator *(Rect2f A, int Value)
        {
            return new Rect2f(A.Width * Value,
                              A.Height * Value);
        }
        public static Rect2f operator *(Rect2f A, double Value)
        {
            return new Rect2f(A.Width * Value,
                              A.Height * Value);
        }
        public static Rect2f operator *(Rect2f A, in ReadOnlyRect2f B)
        {
            return new Rect2f(A.Width * B.Width,
                              A.Height * B.Height);
        }

        // DIVISION
        public static Rect2f operator /(Rect2f A, int Value)
        {
            return new Rect2f(A.Width / Value,
                              A.Height / Value);
        }
        public static Rect2f operator /(Rect2f A, double Value)
        {
            return new Rect2f(A.Width / Value,
                              A.Height / Value);
        }
        public static Rect2f operator /(Rect2f A, in ReadOnlyRect2f B)
        {
            return new Rect2f(A.Width / B.Width,
                              A.Height / B.Height);
        }
        #endregion

        #region Equality
        public static bool operator ==(Rect2f A, Rect2f B)
        {
            if ((A is null) || (B is null))
                return (A is null) ^ (B is null);

            if (ReferenceEquals(A, B)) return true;

            return (A.Width ==  B.Width) && (A.Height ==  B.Height);
        }

        public static bool operator !=(Rect2f A, Rect2f B)
        {
            if ((A is null) || (B is null))
                return !((A is null) ^ (B is null));

            if (ReferenceEquals(A, B)) return false;

            return !(A.Width ==  B.Width) || !(A.Height ==  B.Height);
        }

        public override bool Equals(object o)
        {
            return (o is Rect2f val && this == val);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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
        public Rect2f Min(Rect2f mn)
        {
            if (mn is null) return this;
            return new Rect2f()
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
        public Rect2f Max(Rect2f mx)
        {
            if (mx is null) return this;
            return new Rect2f()
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
        public Rect2f Clamp(Rect2f mn, Rect2f mx)
        {
            if (mn is null && mx is null)
            {// We have been given no minimum or maximum values, so just return the size
                return this;
            }
            else if (mn is null)
            {// We have been given no minimum value, so function like Min() instead
                return new Rect2f()
                {
                    Width = MathExt.Min(Width, mx.Width),
                    Height = MathExt.Min(Height, mx.Height),
                };
            }
            else if (mx is null)
            {// We have been given no maximum value, so function like Max() instead
                return new Rect2f()
                {
                    Width = MathExt.Max(Width, mn.Width),
                    Height = MathExt.Max(Height, mn.Height),
                };
            }
            else
            {
                return new Rect2f()
                {
                    Width = MathExt.Clamp(Width, mn.Width, mx.Width),
                    Height = MathExt.Clamp(Height, mn.Height, mx.Height),
                };
            }
        }
        #endregion


        #region Casts
        public static implicit operator ReadOnlyRect2f(Rect2f size)
        {
            return (ReadOnlyRect2f)size;
        }
        #endregion
    }
}
