namespace CssUI.DOM.Geometry
{
    /// <summary>
    /// Represents a quadrilateral.
    /// </summary>
    public class DOMQuad
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMQuad */
        #region Properties
        public DOMPoint p1;
        public DOMPoint p2;
        public DOMPoint p3;
        public DOMPoint p4;
        #endregion

        #region Constructors
        public DOMQuad(DOMPointInit p1, DOMPointInit p2, DOMPointInit p3, DOMPointInit p4)
        {
            this.p1 = new DOMPoint(p1.x, p1.y, p1.z, p1.w);
            this.p2 = new DOMPoint(p2.x, p2.y, p2.z, p2.w);
            this.p3 = new DOMPoint(p3.x, p3.y, p3.z, p3.w);
            this.p4 = new DOMPoint(p4.x, p4.y, p4.z, p4.w);
        }
        
        public DOMQuad(DOMPoint p1, DOMPoint p2, DOMPoint p3, DOMPoint p4)
        {
            this.p1 = new DOMPoint(p1);
            this.p2 = new DOMPoint(p2);
            this.p3 = new DOMPoint(p3);
            this.p4 = new DOMPoint(p4);
        }
        
        public DOMQuad(DOMPointReadOnly p1, DOMPointReadOnly p2, DOMPointReadOnly p3, DOMPointReadOnly p4)
        {
            this.p1 = new DOMPoint(p1);
            this.p2 = new DOMPoint(p2);
            this.p3 = new DOMPoint(p3);
            this.p4 = new DOMPoint(p4);
        }
        #endregion

        public static DOMQuad fromRect(DOMRectInit R)
        {/* Docs: https://www.w3.org/TR/geometry-1/#create-a-domquad-from-the-domrectinit-dictionary */
            var p1 = new DOMPoint(R.X, R.Y);
            var p2 = new DOMPoint(R.X+R.Width, R.Y);
            var p3 = new DOMPoint(R.X+R.Width, R.Y+R.Height);
            var p4 = new DOMPoint(R.X, R.Y+R.Height);

            return new DOMQuad(p1, p2, p3, p4);
        }

        public static DOMQuad fromRect(DOMQuadInit init)
        {/* Docs: https://www.w3.org/TR/geometry-1/#create-a-domquad-from-the-domquadinit-dictionary */
            return new DOMQuad(init.p1, init.p2, init.p3, init.p4);
        }

        public DOMRect getBounds()
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domquad-getbounds */
            var bounds = new DOMRect();
            var left = MathExt.Min(p1.x, MathExt.Min(p2.x, MathExt.Min(p3.x, p4.x)));
            var top = MathExt.Min(p1.y, MathExt.Min(p2.y, MathExt.Min(p3.y, p4.y)));
            var right = MathExt.Max(p1.x, MathExt.Max(p2.x, MathExt.Max(p3.x, p4.x)));
            var bottom = MathExt.Max(p1.y, MathExt.Max(p2.y, MathExt.Max(p3.y, p4.y)));

            bounds.X = left;
            bounds.Y = top;
            bounds.Width = right - left;
            bounds.Height = bottom - top;

            return bounds;
        }
    }
}
