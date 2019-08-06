namespace CssUI.DOM.Geometry
{
    public class DOMPoint : DOMPointReadOnly
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMPoint */
        #region Properties
        public new double x { get; set; }
        public new double y { get; set; }
        public new double z { get; set; }
        public new double w { get; set; }
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
        /*public static DOMPoint fromPoint(DOMPointInit other)
        {
            return new DOMPoint(other.x, other.y, other.z, other.w);
        }

        public DOMPoint matrixTransform(DOMMatrixInit other)
        {*//* Docs: https://www.w3.org/TR/geometry-1/#transforming-a-point-with-a-matrix *//*
            DOMMatrix matrix = DOMMatrix.fromMatrix(other);
            return matrix.TransformPoint(new DOMPointInit(this.x, this.y, this.z, this.w));
        }*/
        #endregion

        #region Object Equality
        public bool Equals(double X, double Y)
        {
            return MathExt.Feq(this.x, X)
                && MathExt.Feq(this.y, Y);
        }

        public bool Equals(double X, double Y, double Z)
        {
            return MathExt.Feq(this.x, X)
                && MathExt.Feq(this.y, Y)
                && MathExt.Feq(this.z, Z);
        }

        public bool Equals(double X, double Y, double Z, double W)
        {
            return MathExt.Feq(this.x, X)
                && MathExt.Feq(this.y, Y)
                && MathExt.Feq(this.z, Z)
                && MathExt.Feq(this.w, W);
        }
        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is DOMPoint pt)
            {
                return MathExt.Feq(this.x, pt.x) 
                    && MathExt.Feq(this.y, pt.y) 
                    && MathExt.Feq(this.z, pt.z) 
                    && MathExt.Feq(this.w, pt.w);
            }

            if (obj is DOMPointReadOnly ro)
            {
                return MathExt.Feq(this.x, ro.x) 
                    && MathExt.Feq(this.y, ro.y) 
                    && MathExt.Feq(this.z, ro.z) 
                    && MathExt.Feq(this.w, ro.w);
            }

            if (obj is DOMPointInit op)
            {
                return MathExt.Feq(this.x, op.x)
                    && MathExt.Feq(this.y, op.y)
                    && MathExt.Feq(this.z, op.z)
                    && MathExt.Feq(this.w, op.w);
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
