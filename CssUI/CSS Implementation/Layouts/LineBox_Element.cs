
namespace CssUI.Layout
{
    /// <summary>
    /// Holds a <see cref="cssElement"/> and it's proposed layout position
    /// </summary>
    public class LineBox_Element
    {
        public Vec2i Pos;
        public cssElement Element;

        public LineBox_Element(cssElement Element, Vec2i Pos)
        {
            this.Pos = Pos;
            this.Element = Element;
        }
    }
}
