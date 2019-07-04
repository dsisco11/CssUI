using System;

namespace CssUI
{
    /// <summary>
    /// Represents an sub-area within a <see cref="CssBox"/>
    /// </summary>
    public class cssBoxArea
    {
        #region Properties
        private readonly CssBox Box;

        /// <summary>
        /// Holds the origin location for the bounds
        /// </summary>
        private readonly Vec2i Pos = new Vec2i();
        /// <summary>
        /// Holds the size of the bounds
        /// </summary>
        private readonly Size2D Dimensions = new Size2D();

        /// <summary>
        /// The edges of this area
        /// </summary>
        internal cssRect Edge = new cssRect() { Left = 0, Top = 0, Right = 0, Bottom = 0 };
        /// <summary>
        /// The edge sizes of this area
        /// </summary>
        private cssRect Size = new cssRect() { Left = 0, Top = 0, Right = 0, Bottom = 0 };

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
        public int Size_Horizontal { get => (Size.Left + Size.Right); }
        /// <summary>
        /// The combined Top/Bottom sizes for this edge
        /// </summary>
        public int Size_Vertical { get => (Size.Top + Size.Bottom); }


        public int X { get => Pos.X; }
        public int Y { get => Pos.Y; }

        public int Width { get => Dimensions.Width; }
        public int Height { get => Dimensions.Height; }

        private int Half_Width { get => (Width / 2); }
        private int Half_Height { get => (Height / 2); }

        private int CenterX { get => (Left + Half_Width); }
        private int CenterY { get => (Bottom + Half_Height); }

        /// <summary>
        /// Returns the box's current APPROXIMATE center position
        /// </summary>
        public Vec2i Get_Center_Pos() { return new Vec2i(CenterX, CenterY); }

        /// <summary>
        /// Returns the box's Position
        /// </summary>
        /// <returns></returns>
        /// We return a new object because destructive calculations will likely be done on this
        public Vec2i Get_Pos() => new Vec2i(Pos);

        /// <summary>
        /// Returns the box's Dimensions
        /// </summary>
        /// <returns></returns>
        /// We return a new object because destructive calculations will likely be done on this
        public Size2D Get_Dimensions() => new Size2D(Dimensions);
        #endregion

        #region Accessors
        /// <summary>
        /// Returns a copy of this areas edges
        /// </summary>
        /// <returns></returns>
        public cssRect Get_Rect()
        {
            return new cssRect() { Top = Edge.Top, Right = Edge.Right, Bottom = Edge.Bottom, Left = Edge.Left };
        }
        /// <summary>
        /// The logical-width for this area
        /// </summary>
        /// Docs: https://www.w3.org/TR/css-writing-modes-4/#logical-width
        public int LogicalWidth
        {
            get => (Box.Style.WritingMode == Enums.EWritingMode.Horizontal_TB) ? Dimensions.Width : Dimensions.Height;
        }
        /// <summary>
        /// The logical-height for this area
        /// </summary>
        public int LogicalHeight
        {
            get => (Box.Style.WritingMode == Enums.EWritingMode.Horizontal_TB) ? Dimensions.Height : Dimensions.Width;
        }
        #endregion

        #region Constructor
        public cssBoxArea()
        {
        }

        public cssBoxArea(CssBox Box)
        {
            this.Box = Box;
        }

        /// <summary>
        /// Makes a copy of the given <see cref="cssBoxArea"/>
        /// </summary>
        /// <param name="Area"></param>
        public cssBoxArea(cssBoxArea Area)
        {
            this.Box = Area.Box;
            this.Edge = new cssRect() { Top=Area.Edge.Top, Right=Area.Edge.Right, Bottom = Area.Edge.Bottom, Left=Area.Edge.Left };
            this.Size = new cssRect() { Top=Area.Size.Top, Right=Area.Size.Right, Bottom = Area.Size.Bottom, Left=Area.Size.Left };
            this.update_pos_and_dimensions();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Box"></param>
        /// <param name="Position"></param>
        /// <param name="Size"></param>
        public cssBoxArea(CssBox Box, Vec2i Position, Size2D Size)
        {
            this.Box = Box;
            Update_Bounds(Position.X, Position.Y, Size.Width, Size.Height);
        }

        /// <summary>
        /// Makes a copy of the given <see cref="cssBoxArea"/>
        /// </summary>
        /// <param name="Area"></param>
        public cssBoxArea(int Top, int Right, int Bottom, int Left)
        {
            this.Edge = new cssRect() { Top = Top, Right = Right, Bottom = Bottom, Left = Left };
            this.Size = new cssRect() { Top = 0, Right = 0, Bottom = 0, Left = 0 };
            this.update_pos_and_dimensions();
        }
        #endregion

        #region Updating

        private void update_trbl()
        {
            Edge.Top = (Y - Size.Top);
            Edge.Right = (X + Width - Size.Right);
            Edge.Bottom = (Y + Height + Size.Bottom);
            Edge.Left = (X + Size.Left);
        }

        private void update_pos_and_dimensions()
        {
            Pos.X = Left;
            Pos.Y = Bottom;

            Dimensions.Width = (Right - Left);
            Dimensions.Height = (Bottom - Top);
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
        public void Encapsulate(cssBoxArea Area)
        {
            Edge.Top = (Area.Top - Size.Top);
            Edge.Right = (Area.Right - Size.Right);
            Edge.Bottom = (Area.Bottom + Size.Bottom);
            Edge.Left = (Area.Left + Size.Left);
            update_pos_and_dimensions();
        }

        /// <summary>
        /// Updates this edge area to fit within the given edge area
        /// </summary>
        /// <param name="edge"></param>
        public void Fit(cssBoxArea Area)
        {
            Edge.Top = (Area.Top - Area.Size.Top);
            Edge.Right = (Area.Right - Area.Size.Right);
            Edge.Bottom = (Area.Bottom + Area.Size.Bottom);
            Edge.Left = (Area.Left + Area.Size.Left);
            update_pos_and_dimensions();
        }

        /// <summary>
        /// Updates this edge area to fit within the given edge area and sets it's dimensions within that area
        /// </summary>
        /// <param name="edge"></param>
        public void Fit(cssBoxArea Area, Vec2i Offset, Size2D Size)
        {
            Edge.Top = (Area.Top - Area.Size.Top) + (Offset.Y);
            Edge.Right = (Area.Right - Area.Size.Right) + (Offset.X + Size.Width);
            Edge.Bottom = (Area.Bottom + Area.Size.Bottom) + (Offset.Y + Size.Height);
            Edge.Left = (Area.Left + Area.Size.Left) + (Offset.X);
            update_pos_and_dimensions();
        }
        #endregion

        #region Setters
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

        public static cssBoxArea ShallowCopy(cssBoxArea Area)
        {
            if (ReferenceEquals(Area, null))
                return null;

            return new cssBoxArea(Area.Box)
            {
                Edge = new cssRect() { Top = Area.Edge.Top, Right = Area.Edge.Right, Bottom = Area.Edge.Bottom, Left = Area.Edge.Left },
                Size = new cssRect() { Top = Area.Size.Top, Right = Area.Size.Right, Bottom = Area.Size.Bottom, Left = Area.Size.Left }
            };
        }

        #region Intersection
        /// <summary>
        /// Returns True if the given point lies within this block
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(Vec2i point)
        {
            return (Left <= point.X && Right >= point.X && Top <= point.Y && Bottom >= point.Y);
        }

        /// <summary>
        /// Returns True if the given point lies within this block
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(int X, int Y)
        {
            return (Left <= X && Right >= X && Top <= Y && Bottom >= Y);
        }

        /// <summary>
        /// Returns True if the given block intersects this block
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(cssBoxArea box)
        {
            return (Math.Abs(CenterX - box.CenterX) <= (Half_Width + box.Half_Width)) &&
                   (Math.Abs(CenterY - box.CenterY) <= (Half_Height + box.Half_Height));
        }

        #endregion

        #region Operators
        public static bool operator ==(cssBoxArea A, cssBoxArea B)
        {
            // If either object is null return whether they are BOTH null
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));

            return A.GetHashCode() == B.GetHashCode();
        }
        public static bool operator !=(cssBoxArea A, cssBoxArea B)
        {
            // If either object is null return whether they are BOTH null
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null))
                return !(object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));

            return A.GetHashCode() != B.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is cssBoxArea area && (GetHashCode() == area.GetHashCode());
        }
        #endregion

        #region Hashing

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + this.Top;
            hash = hash * 31 + this.Right;
            hash = hash * 31 + this.Bottom;
            hash = hash * 31 + this.Left;

            hash = hash * 31 + this.Size_Top;
            hash = hash * 31 + this.Size_Right;
            hash = hash * 31 + this.Size_Bottom;
            hash = hash * 31 + this.Size_Left;

            return hash;
        }
        #endregion
    }
}
