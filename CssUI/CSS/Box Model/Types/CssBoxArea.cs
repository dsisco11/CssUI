using CssUI.CSS.BoxTree;
using CssUI.DOM.Geometry;

namespace CssUI.CSS
{
    /// <summary>
    /// Represents an sub-area within a CSS box
    /// </summary>
    public class CssBoxArea
    {
        #region Properties
        private readonly ICssBoxFragment Fragment;

        /// <summary>
        /// Holds the origin location for the bounds
        /// </summary>
        private readonly Point2i Pos = new Point2i();
        /// <summary>
        /// Holds the size of the bounds
        /// </summary>
        private readonly Rect2i Dimensions = new Rect2i();

        /// <summary>
        /// The edges of this area
        /// </summary>
        internal CssRect Edge = new CssRect() { Left = 0, Top = 0, Right = 0, Bottom = 0 };
        /// <summary>
        /// The edge sizes of this area
        /// </summary>
        private CssRect Size = new CssRect() { Left = 0, Top = 0, Right = 0, Bottom = 0 };

        /// <summary>
        /// Top edge position of this area
        /// </summary>
        public int Top { get => Edge.Top; private set => Edge.Top = value; }
        /// <summary>
        /// Right edge position of this area
        /// </summary>
        public int Right { get => Edge.Right; private set => Edge.Right = value; }
        /// <summary>
        /// Bottom edge position of this area
        /// </summary>
        public int Bottom { get => Edge.Bottom; private set => Edge.Bottom = value; }
        /// <summary>
        /// Left edge position of this area
        /// </summary>
        public int Left { get => Edge.Left; private set => Edge.Left = value; }


        /// <summary>
        /// Top edge size of this area
        /// </summary>
        public int Size_Top { get => Size.Top; set => Size.Top = value; }
        /// <summary>
        /// Right edge size of this area
        /// </summary>
        public int Size_Right { get => Size.Right; set => Size.Right = value; }
        /// <summary>
        /// Bottom edge size of this area
        /// </summary>
        public int Size_Bottom { get => Size.Bottom; set => Size.Bottom = value; }
        /// <summary>
        /// Left edge size of this area
        /// </summary>
        public int Size_Left { get => Size.Left; set => Size.Left = value; }


        /// <summary>
        /// The combined Left/Right sizes for this edge
        /// </summary>
        public int Size_Horizontal => Size.Left + Size.Right;
        /// <summary>
        /// The combined Top/Bottom sizes for this edge
        /// </summary>
        public int Size_Vertical => Size.Top + Size.Bottom;


        public int X => Pos.X;
        public int Y => Pos.Y;

        public int Width => Dimensions.Width;
        public int Height => Dimensions.Height;

        private int Half_Width => Width / 2;
        private int Half_Height => Height / 2;

        private int CenterX => Left + Half_Width;
        private int CenterY => Bottom + Half_Height;

        /// <summary>
        /// Returns the box's current APPROXIMATE center position
        /// </summary>
        public ReadOnlyPoint2i Get_Center_Pos() => new ReadOnlyPoint2i(CenterX, CenterY);

        /// <summary>
        /// Returns the box's Position
        /// </summary>
        /// <returns></returns>
        /// We return a new object because destructive calculations will likely be done on this
        public ReadOnlyPoint2i Get_Pos() => Pos;

        /// <summary>
        /// Returns the box's Dimensions
        /// </summary>
        /// <returns></returns>
        /// We return a new object because destructive calculations will likely be done on this
        public ReadOnlyRect2i Get_Dimensions() => Dimensions;
        #endregion

        #region Accessors
        /// <summary>
        /// Returns a copy of this areas edges
        /// </summary>
        /// <returns></returns>
        public CssRect Get_Rect()
        {
            return new CssRect() { Top = Edge.Top, Right = Edge.Right, Bottom = Edge.Bottom, Left = Edge.Left };
        }

        /* XXX: Have to restructure the CSS box system because this current system does NOT actually describe the CSS break system for fragmentation */
        private StyleProperties Style => null;

        /// <summary>
        /// The logical-width for this area
        /// </summary>
        /// Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-width
        public int LogicalWidth => Style?.WritingMode != EWritingMode.Horizontal_TB ? Dimensions.Height : Dimensions.Width;

        /// <summary>
        /// The logical-height for this area
        /// </summary>
        public int LogicalHeight => Style?.WritingMode != EWritingMode.Horizontal_TB ? Dimensions.Width : Dimensions.Height;
        #endregion

        #region Constructors
        public CssBoxArea()
        {
        }

        public CssBoxArea(ICssBoxFragment Box)
        {
            this.Fragment = Box;
        }

        /// <summary>
        /// Makes a copy of the given <see cref="CssBoxArea"/>
        /// </summary>
        /// <param name="Area"></param>
        public CssBoxArea(CssBoxArea Area)
        {
            Fragment = Area.Fragment;
            Edge = new CssRect() { Top = Area.Edge.Top, Right = Area.Edge.Right, Bottom = Area.Edge.Bottom, Left = Area.Edge.Left };
            Size = new CssRect() { Top = Area.Size.Top, Right = Area.Size.Right, Bottom = Area.Size.Bottom, Left = Area.Size.Left };
            update_pos_and_dimensions();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Box"></param>
        /// <param name="Position"></param>
        /// <param name="Size"></param>
        public CssBoxArea(CssBox Box, Point2i Position, Rect2i Size)
        {
            Update_Bounds(Position.X, Position.Y, Size.Width, Size.Height);
        }

        /// <summary>
        /// </summary>
        public CssBoxArea(long Top, long Right, long Bottom, long Left)
        {
            Edge = new CssRect() { Top = (int)Top, Right = (int)Right, Bottom = (int)Bottom, Left = (int)Left };
            Size = new CssRect() { Top = 0, Right = 0, Bottom = 0, Left = 0 };
            update_pos_and_dimensions();
        }

        /// <summary>
        /// </summary>
        public CssBoxArea(DOMRect Rect)
        {
            Edge = new CssRect() { Top = (int)Rect.Top, Right = (int)Rect.Right, Bottom = (int)Rect.Bottom, Left = (int)Rect.Left };
            Size = new CssRect() { Top = 0, Right = 0, Bottom = 0, Left = 0 };
            update_pos_and_dimensions();
        }
        #endregion

        #region Updating

        private void update_trbl()
        {
            Edge.Top = Y - Size.Top;
            Edge.Right = X + Width - Size.Right;
            Edge.Bottom = Y + Height + Size.Bottom;
            Edge.Left = X + Size.Left;
        }

        private void update_pos_and_dimensions()
        {
            Pos.X = Left;
            Pos.Y = Bottom;

            Dimensions.Width = Right - Left;
            Dimensions.Height = Bottom - Top;
        }

        public void Set_Dimensions(int Width, int Height)
        {
            Dimensions.Width = Width;
            Dimensions.Height = Height;
        }

        public void Set_TRBL(int Left, int Bottom)
        {
            Edge.Top = Bottom;
            Edge.Right = Left;
            Edge.Bottom = Bottom;
            Edge.Left = Left;
        }

        public void Set_TRBL(int Top, int Right, int Bottom, int Left)
        {
            Edge.Top = Top;
            Edge.Right = Right;
            Edge.Bottom = Bottom;
            Edge.Left = Left;
        }

        /// <summary>
        /// Update this edge area to be the size given and positioned at the given coordinates
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void Update_Bounds(int X, int Y, int Width, int Height)
        {
            Pos.X = X;
            Pos.Y = Y;
            Dimensions.Width = Width;
            Dimensions.Height = Height;

        }

        /// <summary>
        /// Updates this edge area to encapsulate the given edge area
        /// </summary>
        /// <param name="edge"></param>
        public void Encapsulate(CssBoxArea Area)
        {
            Edge.Top = Area.Top - Size.Top;
            Edge.Right = Area.Right - Size.Right;
            Edge.Bottom = Area.Bottom + Size.Bottom;
            Edge.Left = Area.Left + Size.Left;
            update_pos_and_dimensions();
        }

        /// <summary>
        /// Updates this edge area to fit within the given edge area
        /// </summary>
        /// <param name="edge"></param>
        public void Fit(CssBoxArea Area)
        {
            Edge.Top = Area.Top - Area.Size.Top;
            Edge.Right = Area.Right - Area.Size.Right;
            Edge.Bottom = Area.Bottom + Area.Size.Bottom;
            Edge.Left = Area.Left + Area.Size.Left;
            update_pos_and_dimensions();
        }

        /// <summary>
        /// Updates this edge area to fit within the given edge area and sets it's dimensions within that area
        /// </summary>
        /// <param name="edge"></param>
        public void Fit(CssBoxArea Area, Point2i Offset, Rect2i Size)
        {
            Edge.Top = Area.Top - Area.Size.Top + Offset.Y;
            Edge.Right = Area.Right - Area.Size.Right + Offset.X + Size.Width;
            Edge.Bottom = Area.Bottom + Area.Size.Bottom + Offset.Y + Size.Height;
            Edge.Left = Area.Left + Area.Size.Left + Offset.X;
            update_pos_and_dimensions();
        }
        #endregion

        #region Setters
        /// <summary>
        /// Zeroes out all position and size values
        /// </summary>
        public void Clear()
        {
            Size_Top = Size_Right = Size_Bottom = Size_Left = 0;
            Top = Right = Bottom = Left = 0;
        }

        public void Set_Pos_X(int X)
        {
            Pos.X = X;
            update_trbl();
        }

        public void Set_Pos_Y(int Y)
        {
            Pos.Y = Y;
            update_trbl();
        }

        public void Set_Width(int Width)
        {
            Dimensions.Width = Width;
            update_trbl();
        }

        public void Set_Height(int Height)
        {
            Dimensions.Height = Height;
            update_trbl();
        }
        #endregion

        #region Cloning
        public static CssBoxArea ShallowCopy(CssBoxArea Area)
        {
            if (ReferenceEquals(Area, null))
                return null;

            return new CssBoxArea(Area.Fragment)
            {
                Edge = new CssRect() { Top = Area.Edge.Top, Right = Area.Edge.Right, Bottom = Area.Edge.Bottom, Left = Area.Edge.Left },
                Size = new CssRect() { Top = Area.Size.Top, Right = Area.Size.Right, Bottom = Area.Size.Bottom, Left = Area.Size.Left }
            };
        }

        public DOMRect getBoundingClientRect()
        {
            return new DOMRect(X, Y, Width, Height);
        }
        #endregion


        #region Intersection
        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public bool Intersects(Point2i point)
        {
            return Left <= point.X && Right >= point.X && Top <= point.Y && Bottom >= point.Y;
        }

        /// <summary>
        /// Returns True if the given point lies within this area
        /// </summary>
        public bool Intersects(int X, int Y)
        {
            return Left <= X && Right >= X && Top <= Y && Bottom >= Y;
        }

        /// <summary>
        /// Returns True if the given <see cref="CssBoxArea"/> intersects this area
        /// </summary>
        public bool Intersects(CssBoxArea box)
        {
            bool intersectsX = (Left <=  box.Right) && (Right >=  box.Left);
            bool intersectsY = (Bottom <=  box.Top) && (Top >=  box.Bottom);
            return intersectsX && intersectsY;
            /*return Math.Abs(CenterX - box.CenterX) <= Half_Width + box.Half_Width &&
                   Math.Abs(CenterY - box.CenterY) <= Half_Height + box.Half_Height;*/
        }

        /// <summary>
        /// Returns True if the given <see cref="DOMRect"/> intersects this area
        /// </summary>
        public bool Intersects(DOMRectReadOnly rect)
        {
            bool intersectsX = (Left <=  rect.Right) && (Right >=  rect.Left);
            bool intersectsY = (Bottom <=  rect.Top) && (Top >=  rect.Bottom);
            return intersectsX && intersectsY;
        }

        #endregion

        #region Operators
        public static bool operator ==(CssBoxArea A, CssBoxArea B)
        {
            // If either object is null return whether they are BOTH null
            if (ReferenceEquals(A, null) || ReferenceEquals(B, null))
                return ReferenceEquals(A, null) && ReferenceEquals(B, null);

            return A.GetHashCode() == B.GetHashCode();
        }
        public static bool operator !=(CssBoxArea A, CssBoxArea B)
        {
            // If either object is null return whether they are BOTH null
            if (ReferenceEquals(A, null) || ReferenceEquals(B, null))
                return !(ReferenceEquals(A, null) && ReferenceEquals(B, null));

            return A.GetHashCode() != B.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is CssBoxArea area && GetHashCode() == area.GetHashCode();
        }
        #endregion

        #region Hashing

        public override int GetHashCode()
        {
            int hash = 17;
            hash = (hash * 31) + Top;
            hash = (hash * 31) + Right;
            hash = (hash * 31) + Bottom;
            hash = (hash * 31) + Left;

            hash = (hash * 31) + Size_Top;
            hash = (hash * 31) + Size_Right;
            hash = (hash * 31) + Size_Bottom;
            hash = (hash * 31) + Size_Left;

            return hash;
        }
        #endregion
    }
}
