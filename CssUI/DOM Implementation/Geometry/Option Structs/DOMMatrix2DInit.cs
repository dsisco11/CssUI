namespace CssUI.DOM.Geometry
{
    public class DOMMatrix2DInit
    {
        #region Properties
        public double a => m11;
        public double b => m12;
        public double c => m21;
        public double d => m22;
        public double e => m41;
        public double f => m42;
        public readonly double m11;
        public readonly double m12;
        public readonly double m21;
        public readonly double m22;
        public readonly double m41;
        public readonly double m42;
        #endregion

        #region Constructors
        public DOMMatrix2DInit(double m11, double m12, double m21, double m22, double m41, double m42)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            this.m41 = m41;
            this.m42 = m42;
        }
        #endregion
    }
}
