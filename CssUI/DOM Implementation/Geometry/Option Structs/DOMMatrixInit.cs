namespace CssUI.DOM.Geometry
{
    public class DOMMatrixInit : DOMMatrix2DInit
    {
        #region Properties
        public readonly double m13 = 0;
        public readonly double m14 = 0;
        public readonly double m23 = 0;
        public readonly double m24 = 0;
        public readonly double m31 = 0;
        public readonly double m32 = 0;
        public readonly double m33 = 1;
        public readonly double m34 = 0;
        public readonly double m43 = 0;
        public readonly double m44 = 1;
        public readonly bool is2D;
        #endregion

        #region Constructors
        public DOMMatrixInit(double m11, double m12, double m21, double m22, double m41, double m42, double m13 = 0, double m14 = 0, double m23 = 0, double m24 = 0, double m31 = 0, double m32 = 0, double m33 = 1, double m34 = 0, double m43 = 0, double m44 = 1) : base(m11, m12, m21, m22, m41, m42)
        {
            this.m13 = m13;
            this.m14 = m14;
            this.m23 = m23;
            this.m24 = m24;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
            this.m34 = m34;
            this.m43 = m43;
            this.m44 = m44;
        }
        #endregion


    }
}
