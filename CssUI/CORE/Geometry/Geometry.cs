using CssUI.DOM.Geometry;
using System.Diagnostics.Contracts;

namespace CssUI
{
    /// <summary>
    /// Provides helpful functions for things like shape intersection
    /// </summary>
    public static class Geometry
    {
        #region DOMRect Intersection
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in DOMRectReadOnly Rect, int X, int Y)
        {
            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            Contract.EndContractBlock();

            return (Rect.Left <=  X) && (Rect.Right >=  X) && (Rect.Top <=  Y) && (Rect.Bottom >=  Y);
        }
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in DOMRectReadOnly Rect, double X, double Y)
        {
            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            Contract.EndContractBlock();

            return (Rect.Left <=  X) && (Rect.Right >=  X) && (Rect.Top <=  Y) && (Rect.Bottom >=  Y);
        }
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in DOMRectReadOnly Rect, in Point2i Point)
        {
            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            if (Point is null)
            {
                throw new System.ArgumentNullException(nameof(Point));
            }

            Contract.EndContractBlock();

            return Rect.Left <= Point.X && Rect.Right >= Point.X && Rect.Top <= Point.Y && Rect.Bottom >= Point.Y;
        }
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in DOMRectReadOnly Rect, in DOMPointReadOnly Point)
        {
            if (Point is null)
            {
                throw new System.ArgumentNullException(nameof(Point));
            }

            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            Contract.EndContractBlock();

            bool intersectsX = (Point.x >=  Rect.Left) && (Point.x <=  Rect.Right);
            bool intersectsY = (Point.y >=  Rect.Top) && (Point.y <=  Rect.Bottom);
            return intersectsX && intersectsY;
            //return (point.x >= rect.left) && (point.x <= rect.right) && (point.y >= rect.top) && (point.y <= rect.bottom);
        }
        /// <summary>
        /// Returns True if the given <see cref="DOMRect"/> intersects this area
        /// </summary>
        public static bool Intersects(in DOMRectReadOnly Left, in DOMRectReadOnly Right)
        {
            if (Left is null)
            {
                throw new System.ArgumentNullException(nameof(Left));
            }

            if (Right is null)
            {
                throw new System.ArgumentNullException(nameof(Right));
            }

            Contract.EndContractBlock();

            bool intersectsX = (Left.Left <=  Right.Right) && (Left.Right >=  Right.Left);
            bool intersectsY = (Left.Bottom <=  Right.Top) && (Left.Top >=  Right.Bottom);
            return intersectsX && intersectsY;
        }
        #endregion


        #region DOMRect Intersection
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in Rect4f Rect, int X, int Y)
        {
            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            Contract.EndContractBlock();

            return (Rect.Left <=  X) && (Rect.Right >=  X) && (Rect.Top <=  Y) && (Rect.Bottom >=  Y);
        }
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in Rect4f Rect, double X, double Y)
        {
            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            Contract.EndContractBlock();

            return (Rect.Left <=  X) && (Rect.Right >=  X) && (Rect.Top <=  Y) && (Rect.Bottom >=  Y);
        }
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in Rect4f Rect, in Point2i Point)
        {
            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            if (Point is null)
            {
                throw new System.ArgumentNullException(nameof(Point));
            }

            Contract.EndContractBlock();

            return Rect.Left <= Point.X && Rect.Right >= Point.X && Rect.Top <= Point.Y && Rect.Bottom >= Point.Y;
        }
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public static bool Intersects(in Rect4f Rect, in Point2f Point)
        {
            if (Point is null)
            {
                throw new System.ArgumentNullException(nameof(Point));
            }

            if (Rect is null)
            {
                throw new System.ArgumentNullException(nameof(Rect));
            }

            Contract.EndContractBlock();

            bool intersectsX = (Point.X >=  Rect.Left) && (Point.X <=  Rect.Right);
            bool intersectsY = (Point.Y >=  Rect.Top) && (Point.Y <=  Rect.Bottom);
            return intersectsX && intersectsY;
        }
        /// <summary>
        /// Returns True if the given <see cref="DOMRect"/> intersects this area
        /// </summary>
        public static bool Intersects(in Rect4f Left, in Rect4f Right)
        {
            if (Left is null)
            {
                throw new System.ArgumentNullException(nameof(Left));
            }

            if (Right is null)
            {
                throw new System.ArgumentNullException(nameof(Right));
            }

            Contract.EndContractBlock();

            bool intersectsX = (Left.Left <=  Right.Right) && (Left.Right >=  Right.Left);
            bool intersectsY = (Left.Bottom <=  Right.Top) && (Left.Top >=  Right.Bottom);
            return intersectsX && intersectsY;
        }
        #endregion
    }
}
