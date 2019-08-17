using System.Runtime.InteropServices;

namespace CssUI
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ReadOnlyColor
    {
        #region Instances
        public static ReadOnlyColor NaN = new ReadOnlyColor(double.NaN, double.NaN, double.NaN, double.NaN);
        #endregion
        
        #region Properties
        public readonly double R;
        public readonly double G;
        public readonly double B;
        public readonly double A;
        #endregion

        #region Constructors
        public ReadOnlyColor(byte r, byte g, byte b, byte a)
        {
            R = r / 255.0d;
            G = g / 255.0d;
            B = b / 255.0d;
            A = a / 255.0d;
        }
        public ReadOnlyColor(double r, double g, double b, double a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        #endregion

        public override string ToString()
        {
            return $"{GetType().Name}<{R.ToString("0.###")}, {G.ToString("0.###")}, {B.ToString("0.###")}, {A.ToString("0.###")}>";
        }
    }
}
