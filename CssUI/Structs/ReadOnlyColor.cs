namespace CssUI
{
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
    }
}
