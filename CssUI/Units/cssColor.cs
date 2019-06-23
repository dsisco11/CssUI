using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Encapsulates an RGBA color.
    /// </summary>
    public class cssColor
    {
        #region Static members
        public static readonly cssColor White = new cssColor(1.0, 1.0, 1.0, 1.0);
        public static readonly cssColor Red = new cssColor(1.0, 0.0, 0.0, 1.0);
        public static readonly cssColor Green = new cssColor(0.0, 1.0, 0.0, 1.0);
        public static readonly cssColor Blue = new cssColor(0.0, 0.0, 1.0, 1.0);
        public static readonly cssColor Transparent = new cssColor(1.0, 1.0, 1.0, 0.0);
        #endregion

        #region Accessors
        /// <summary>Red channel value in the range [0.0 - 1.0]</summary>
        public double R = 1.0;
        /// <summary>Green channel value in the range [0.0 - 1.0]</summary>
        public double G = 1.0;
        /// <summary>Blue channel value in the range [0.0 - 1.0]</summary>
        public double B = 1.0;
        /// <summary>Alpha channel value in the range [0.0 - 1.0]</summary>
        public double A = 0.0;

        /// <summary>Red channel value in the range [0-255]</summary>
        public int iR { get { return (int)(R * 255.0); } }
        /// <summary>Green channel value in the range [0-255]</summary>
        public int iG { get { return (int)(G * 255.0); } }
        /// <summary>Blue channel value in the range [0-255]</summary>
        public int iB { get { return (int)(B * 255.0); } }
        /// <summary>Alpha channel value in the range [0-255]</summary>
        public int iA { get { return (int)(A * 255.0); } }
        #endregion

        #region Constructors
        public cssColor()
        {
        }
        /// <summary>
        /// Creates a new color object by specifying values for the Red, Green, and Blue channels.
        /// </summary>
        /// <param name="R">Value in the range [0.0 - 1.0]</param>
        /// <param name="G">Value in the range [0.0 - 1.0]</param>
        /// <param name="B">Value in the range [0.0 - 1.0]</param>
        public cssColor(double R, double G, double B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = 1.0;
        }
        /// <summary>
        /// Creates a new color object by specifying values for the Red, Green, Blue, and Alpha channels.
        /// </summary>
        /// <param name="R">Value in the range [0.0 - 1.0]</param>
        /// <param name="G">Value in the range [0.0 - 1.0]</param>
        /// <param name="B">Value in the range [0.0 - 1.0]</param>
        /// <param name="A">Value in the range [0.0 - 1.0]</param>
        public cssColor(double R, double G, double B, double A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }
        #endregion

        #region Helpers

        public cssColor Scale(double r, double g, double b, double a)
        {
            this.R *= r;
            this.G *= g;
            this.B *= b;
            this.A *= a;
            return this;
        }

        public cssColor Scale(double rgb, double a)
        {
            this.R *= rgb;
            this.G *= rgb;
            this.B *= rgb;
            this.A *= a;
            return this;
        }

        public cssColor ScaleAlpha(double a)
        {
            this.A *= a;
            return this;
        }

        public cssColor ScaleRGB(double x)
        {
            this.R *= x;
            this.G *= x;
            this.B *= x;
            return this;
        }

        public cssColor ScaleRGBA(double x)
        {
            this.R *= x;
            this.G *= x;
            this.B *= x;
            this.A *= x;
            return this;
        }
        #endregion

        #region Mixing

        private static double Interp(double x, double y, double i)
        {
            return (x*(1.0-i)) + (y*i);
        }

        public cssColor Mix(cssColor c, double blend)
        {
            double R = Interp(this.R, c.R, blend);
            double B = Interp(this.B, c.B, blend);
            double G = Interp(this.G, c.G, blend);
            double A = Interp(this.A, c.A, blend);
            return new cssColor(R, G, B, A);
        }

        public cssColor MixAlpha(double Alpha)
        {
            return new cssColor(R, G, B, A * Alpha);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Converts the color to a 32-bit integer representing the RGBA values
        /// </summary>
        public int ToInt()
        {
            return (iR << 0) + (iG << 8) + (iB << 16) + (iA << 24);
        }


        public string ToCssString()
        {
            if (iA != 255)
            {// Use RGBA format (preserves alpha)
                return string.Concat("rgba(", R, ", ", G, ", ", B, ", ", A, ")");
            }
            else
            {// Use Hexadecimal format (easier to read)
                return ToHexRGB();
            }
        }

        /// <summary>
        /// Converts the color to a hexadecimal RGB color string
        /// </summary>
        public string ToHexRGB()
        {
            return string.Concat("#", iR.ToString("X2"), iG.ToString("X2"), iB.ToString("X2"));
        }

        /// <summary>
        /// Converts the color to a hexadecimal RGBA color string
        /// </summary>
        public string ToHexRGBA()
        {
            return string.Concat("#", iR.ToString("X2"), iG.ToString("X2"), iB.ToString("X2"), iA.ToString("X2"));
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return string.Concat(nameof(cssColor), "<", R.ToString("0.##"), ", ", G.ToString("0.##"), ", ", B.ToString("0.##"), ", ", A.ToString("0.##"), ">");
        }
        #endregion

        #region Operators

        public static cssColor operator *(cssColor a, cssColor b)
        {
            double R = a.R * b.R;
            double B = a.B * b.B;
            double G = a.G * b.G;
            double A = a.A * b.A;
            return new cssColor(R, G, B, A);
        }

        public static cssColor operator /(cssColor a, cssColor b)
        {
            double R = a.R / b.R;
            double B = a.B / b.B;
            double G = a.G / b.G;
            double A = a.A / b.A;
            return new cssColor(R, G, B, A);
        }

        public static cssColor operator +(cssColor a, cssColor b)
        {
            double R = a.R + b.R;
            double B = a.B + b.B;
            double G = a.G + b.G;
            double A = a.A + b.A;
            return new cssColor(R, G, B, A);
        }

        public static cssColor operator -(cssColor a, cssColor b)
        {
            double R = a.R - b.R;
            double B = a.B - b.B;
            double G = a.G - b.G;
            double A = a.A - b.A;
            return new cssColor(R, G, B, A);
        }
        #endregion
    }
}
