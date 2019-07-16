namespace CssUI.DOM.Geometry
{
    public class DOMRectReadOnly
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMRect */
        #region Properties
        public double x { get; protected set; }
        public double y { get; protected set; }
        public double width { get; protected set; }
        public double height { get; protected set; }
        #endregion

        #region Accessors
        public double top
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-top */
            get => MathExt.Min(y, y + height);
        }
        public double right
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-right */
            get => MathExt.Max(x, x + width);
        }
        public double bottom
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-bottom */
            get => MathExt.Max(y, y + height);
        }
        public double left
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-left */
            get => MathExt.Min(x, x + width);
        }
        #endregion

        #region Constructor
        public DOMRectReadOnly(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        #endregion

        public static DOMRectReadOnly fromRect(DOMRectInit other)
        {
            return new DOMRectReadOnly(other.x, other.y, other.width, other.height);
        }

    }
}
