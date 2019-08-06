
namespace CssUI.DOM.Geometry
{
    public static class Geometry
    {

        public static bool Point_Within_Rect(DOMPointReadOnly point, DOMRectReadOnly rect)
        {
            return (point.x >= rect.left) && (point.x <= rect.right) && (point.y >= rect.top) && (point.y <= rect.bottom);
        }
    }
}
