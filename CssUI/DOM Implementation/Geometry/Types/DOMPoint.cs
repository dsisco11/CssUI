namespace CssUI.DOM.Geometry
{
    public class DOMPoint : IDOMPointReadOnly
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMPoint */
        #region Properties
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public double w { get; set; }
        #endregion

        #region Constructors
        public DOMPoint(double x = 0, double y = 0, double z = 0, double w = 1)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public DOMPoint(DOMPointInit init)
        {
            this.x = init.x;
            this.y = init.y;
            this.z = init.z;
            this.w = init.w;
        }

        public DOMPoint(IDOMPointReadOnly p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
            this.w = p.w;
        }
        #endregion

        #region Specification Defined
        public static DOMPoint fromPoint(DOMPointInit other)
        {
            return new DOMPoint(other.x, other.y, other.z, other.w);
        }

        public DOMPoint matrixTransform(DOMMatrixInit other)
        {/* Docs: https://www.w3.org/TR/geometry-1/#transforming-a-point-with-a-matrix */
            DOMMatrix matrix = DOMMatrix.fromMatrix(other);
            return matrix.TransformPoint(new DOMPointInit(this.x, this.y, this.z, this.w));
        }
        #endregion

        #region Object Equality
        public bool Equals(double X, double Y)
        {
            return MathExt.floatEq(this.x, X)
                && MathExt.floatEq(this.y, Y);
        }

        public bool Equals(double X, double Y, double Z)
        {
            return MathExt.floatEq(this.x, X)
                && MathExt.floatEq(this.y, Y)
                && MathExt.floatEq(this.z, Z);
        }

        public bool Equals(double X, double Y, double Z, double W)
        {
            return MathExt.floatEq(this.x, X)
                && MathExt.floatEq(this.y, Y)
                && MathExt.floatEq(this.z, Z)
                && MathExt.floatEq(this.w, W);
        }
        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is DOMPoint pt)
            {
                return MathExt.floatEq(this.x, pt.x) 
                    && MathExt.floatEq(this.y, pt.y) 
                    && MathExt.floatEq(this.z, pt.z) 
                    && MathExt.floatEq(this.w, pt.w);
            }

            if (obj is DOMPointReadOnly ro)
            {
                return MathExt.floatEq(this.x, ro.x) 
                    && MathExt.floatEq(this.y, ro.y) 
                    && MathExt.floatEq(this.z, ro.z) 
                    && MathExt.floatEq(this.w, ro.w);
            }

            if (obj is DOMPointInit op)
            {
                return MathExt.floatEq(this.x, op.x)
                    && MathExt.floatEq(this.y, op.y)
                    && MathExt.floatEq(this.z, op.z)
                    && MathExt.floatEq(this.w, op.w);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31) + x.GetHashCode();
            hash = (hash * 31) + y.GetHashCode();
            hash = (hash * 31) + z.GetHashCode();
            hash = (hash * 31) + w.GetHashCode();

            return hash;
        }

        public override string ToString() => $"{nameof(DOMPoint)}<{x}, {y}, {z}, {w}>";
        #endregion
    }
}
