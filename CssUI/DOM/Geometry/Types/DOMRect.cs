
namespace CssUI.DOM.Geometry
{
    [System.Diagnostics.DebuggerDisplay("[{Top}, {Right}, {Bottom}, {Left}]")]
    public class DOMRect : DOMRectReadOnly
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMRect */

        #region Properties
        public new double X { get; set; }
        public new double Y { get; set; }
        public new double Width { get; set; }
        public new double Height { get; set; }
        #endregion

        #region Accessors
        public double Top
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-top */
            get => MathExt.Min(Y, Y + Height);
        }
        public double Right
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-right */
            get => MathExt.Max(X, X + Width);
        }
        public double Bottom
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-bottom */
            get => MathExt.Max(Y, Y + Height);
        }
        public double Left
        {/* Docs: https://www.w3.org/TR/geometry-1/#dom-domrectreadonly-domrect-left */
            get => MathExt.Min(X, X + Width);
        }
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
            return new DOMRect(other.X, other.Y, other.Width, other.Height);
        }
    }
}
