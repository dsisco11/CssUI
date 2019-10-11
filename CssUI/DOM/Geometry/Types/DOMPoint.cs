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
            return (this.x ==  X)
                && (this.y ==  Y);
        }

        public bool Equals(double X, double Y, double Z)
        {
            return (this.x ==  X)
                && (this.y ==  Y)
                && (this.z ==  Z);
        }

        public bool Equals(double X, double Y, double Z, double W)
        {
            return (this.x ==  X)
                && (this.y ==  Y)
                && (this.z ==  Z)
                && (this.w ==  W);
        }
        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is DOMPoint pt)
            {
                return (this.x ==  pt.x) 
                    && (this.y ==  pt.y) 
                    && (this.z ==  pt.z) 
                    && (this.w ==  pt.w);
            }

            if (obj is DOMPointReadOnly ro)
            {
                return (this.x ==  ro.x) 
                    && (this.y ==  ro.y) 
                    && (this.z ==  ro.z) 
                    && (this.w ==  ro.w);
            }

            if (obj is DOMPointInit op)
            {
                return (this.x ==  op.x)
                    && (this.y ==  op.y)
                    && (this.z ==  op.z)
                    && (this.w ==  op.w);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString() => $"{nameof(DOMPoint)}<{x}, {y}, {z}, {w}>";
        #endregion
    }
}
