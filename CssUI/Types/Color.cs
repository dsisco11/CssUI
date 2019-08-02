namespace CssUI
{
    /// <summary>
    /// Encapsulates an RGBA color.
    /// </summary>
    public class Color
    {
        #region Static members
        public static readonly Color White = new Color(1.0, 1.0, 1.0, 1.0);
        public static readonly Color Red = new Color(1.0, 0.0, 0.0, 1.0);
        public static readonly Color Green = new Color(0.0, 1.0, 0.0, 1.0);
        public static readonly Color Blue = new Color(0.0, 0.0, 1.0, 1.0);
        public static readonly Color Transparent = new Color(1.0, 1.0, 1.0, 0.0);
        #endregion

        #region Properties
        /// <summary>Red channel value in the range [0.0 - 1.0]</summary>
        public double R = 1.0;
        /// <summary>Green channel value in the range [0.0 - 1.0]</summary>
        public double G = 1.0;
        /// <summary>Blue channel value in the range [0.0 - 1.0]</summary>
        public double B = 1.0;
        /// <summary>Alpha channel value in the range [0.0 - 1.0]</summary>
        public double A = 0.0;
        #endregion

        #region Accessors

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
        public Color()
        {
        }
        /// <summary>
        /// Creates a new color object by specifying values for the Red, Green, and Blue channels.
        /// </summary>
        /// <param name="R">Value in the range [0.0 - 1.0]</param>
        /// <param name="G">Value in the range [0.0 - 1.0]</param>
        /// <param name="B">Value in the range [0.0 - 1.0]</param>
        public Color(double R, double G, double B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            A = 1.0;
        }
        /// <summary>
        /// Creates a new color object by specifying values for the Red, Green, Blue, and Alpha channels.
        /// </summary>
        /// <param name="R">Value in the range [0.0 - 1.0]</param>
        /// <param name="G">Value in the range [0.0 - 1.0]</param>
        /// <param name="B">Value in the range [0.0 - 1.0]</param>
        /// <param name="A">Value in the range [0.0 - 1.0]</param>
        public Color(double R, double G, double B, double A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public Color(ReadOnlyColor color)
        {
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
            this.A = color.A;
        }
        #endregion

        #region Scaling

        public Color Scale(double r, double g, double b, double a)
        {
            R *= r;
            G *= g;
            B *= b;
            A *= a;
            return this;
        }

        public Color Scale(double rgb, double a)
        {
            R *= rgb;
            G *= rgb;
            B *= rgb;
            A *= a;
            return this;
        }

        public Color ScaleAlpha(double a)
        {
            A *= a;
            return this;
        }

        public Color ScaleRGB(double x)
        {
            R *= x;
            G *= x;
            B *= x;
            return this;
        }

        public Color ScaleRGBA(double x)
        {
            R *= x;
            G *= x;
            B *= x;
            A *= x;
            return this;
        }
        #endregion

        #region Mixing

        private static double Interp(double x, double y, double i)
        {
            return x * (1.0 - i) + y * i;
        }

        public Color Mix(Color c, double blend)
        {
            double R = Interp(this.R, c.R, blend);
            double B = Interp(this.B, c.B, blend);
            double G = Interp(this.G, c.G, blend);
            double A = Interp(this.A, c.A, blend);
            return new Color(R, G, B, A);
        }

        public Color MixAlpha(double Alpha)
        {
            return new Color(R, G, B, A * Alpha);
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

        #region Serialization
        public string Serialize()
        {
            if (iA < 255)
            {
                /* Return RGBA Hex code */
                return string.Concat("#", iR.ToString("X2"), iG.ToString("X2"), iB.ToString("X2"), iA.ToString("X2"));
            }

            /* Return RGB Hex code */
            return string.Concat("#", iR.ToString("X2"), iG.ToString("X2"), iB.ToString("X2"));
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return string.Concat(nameof(Color), "<", R.ToString("0.##"), ", ", G.ToString("0.##"), ", ", B.ToString("0.##"), ", ", A.ToString("0.##"), ">");
        }
        #endregion

        #region Operators

        public static Color operator *(Color a, Color b)
        {
            double R = a.R * b.R;
            double B = a.B * b.B;
            double G = a.G * b.G;
            double A = a.A * b.A;
            return new Color(R, G, B, A);
        }

        public static Color operator /(Color a, Color b)
        {
            double R = a.R / b.R;
            double B = a.B / b.B;
            double G = a.G / b.G;
            double A = a.A / b.A;
            return new Color(R, G, B, A);
        }

        public static Color operator +(Color a, Color b)
        {
            double R = a.R + b.R;
            double B = a.B + b.B;
            double G = a.G + b.G;
            double A = a.A + b.A;
            return new Color(R, G, B, A);
        }

        public static Color operator -(Color a, Color b)
        {
            double R = a.R - b.R;
            double B = a.B - b.B;
            double G = a.G - b.G;
            double A = a.A - b.A;
            return new Color(R, G, B, A);
        }
        #endregion

        #region Casts
        public static implicit operator ReadOnlyColor(Color color) => new ReadOnlyColor(color.R, color.G, color.B, color.A);
        #endregion
    }
}
