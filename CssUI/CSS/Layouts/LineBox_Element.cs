
namespace CssUI.Layout
{
    /// <summary>
    /// Holds a <see cref="cssElement"/> and it's proposed layout position
    /// </summary>
    public class LineBox_Element
    {
        public Point2i Pos;
        public cssElement Element;

        public LineBox_Element(cssElement Element, Point2i Pos)
        {
            this.Pos = Pos;
            this.Element = Element;
        }
    }
}
