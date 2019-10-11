namespace CssUI.DOM.Geometry
{
    [System.Diagnostics.DebuggerDisplay("[{Top}, {Right}, {Bottom}, {Left}]")]
    public class DOMRectReadOnly
    {/* Docs: https://www.w3.org/TR/geometry-1/#DOMRect */

        #region Properties
        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }
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
        public DOMRectReadOnly(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        #endregion

        public static DOMRectReadOnly fromRect(DOMRectInit other)
        {
            return new DOMRectReadOnly(other.X, other.Y, other.Width, other.Height);
        }

    }
}
