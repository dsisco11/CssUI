
namespace CssUI.DOM.Geometry
{
    public static class Geometry
    {

        public static bool Point_Within_Rect(DOMPointReadOnly point, DOMRectReadOnly rect)
        {
            bool intersectsX = MathExt.Fgteq(point.x, rect.left) && MathExt.Flteq(point.x, rect.right);
            bool intersectsY = MathExt.Fgteq(point.y, rect.top) && MathExt.Flteq(point.y, rect.bottom);
            return intersectsX && intersectsY;
            //return (point.x >= rect.left) && (point.x <= rect.right) && (point.y >= rect.top) && (point.y <= rect.bottom);
        }

        public static bool Intersects(DOMRectReadOnly A, DOMRectReadOnly B)
        {
            bool intersectsX = MathExt.Flteq(A.left, B.right) && MathExt.Fgteq(A.right, B.left);
            bool intersectsY = MathExt.Flteq(A.bottom, B.top) && MathExt.Fgteq(A.top, B.bottom);
            return intersectsX && intersectsY;
        }
    }
}
