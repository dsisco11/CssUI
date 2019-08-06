namespace CssUI.DOM.Geometry
{
    public class DOMRect : DOMRectReadOnly
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMRect */

        #region Properties
        public new double x { get; set; }
        public new double y { get; set; }
        public new double width { get; set; }
        public new double height { get; set; }
        #endregion

        #region Accessors
        /*public double top
        {*//* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-top *//*
            get => MathExt.Min(y, y + height);
        }
        public double right
        {*//* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-right *//*
            get => MathExt.Max(x, x + width);
        }
        public double bottom
        {*//* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-bottom *//*
            get => MathExt.Max(y, y + height);
        }
        public double left
        {*//* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-left *//*
            get => MathExt.Min(x, x + width);
        }*/
        #endregion

        #region Constructor
        public DOMRect(double x = 0, double y = 0, double width = 0, double height = 0) : base(x, y, width, height)
        {
            /*this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;*/
        }
        #endregion

        public new static DOMRect fromRect(DOMRectInit other)
        {
            return new DOMRect(other.x, other.y, other.width, other.height);
        }
    }
}
