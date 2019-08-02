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
