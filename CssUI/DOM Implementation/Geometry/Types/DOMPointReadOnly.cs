namespace CssUI.DOM.Geometry
{
    public class DOMPointReadOnly
    {/* Docs" https://www.w3.org/TR/geometry-1/#DOMPoint */
        #region Properties
        public double x { get; protected set; }
        public double y { get; protected set; }
        public double z { get; protected set; }
        public double w { get; protected set; }
        #endregion

        #region Constructors
        public DOMPointReadOnly(double x = 0, double y = 0, double z = 0, double w = 1)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public DOMPointReadOnly(DOMPointInit init)
        {
            this.x = init.x;
            this.y = init.y;
            this.z = init.z;
            this.w = init.w;
        }

        public DOMPointReadOnly(DOMPointReadOnly p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
            this.w = p.w;
        }
        #endregion


        public static DOMPointReadOnly fromPoint(DOMPointInit other)
        {
            return new DOMPointReadOnly(other.x, other.y, other.z, other.w);
        }

        public DOMPoint matrixTransform(DOMMatrixInit other)
        {/* Docs: https://www.w3.org/TR/geometry-1/#transforming-a-point-with-a-matrix */
            DOMMatrix matrix = DOMMatrix.fromMatrix(other);
            return matrix.TransformPoint(new DOMPointInit(this.x, this.y, this.z, this.w));
        }



    }
}
