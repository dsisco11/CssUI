using System;

namespace CssUI
{
    [Flags]
    public enum FBlockFlags : uint
    {
        /// <summary>
        /// Block is locked and cannot be updated
        /// </summary>
        Locked = (1 << 1),
        /// <summary>
        /// Block is dirty and needs to be updated/resolved
        /// </summary>
        Dirty = (1 << 2),
    }
    /// <summary>
    /// Holds a set of four integers which represent the Left, Top, Right, and Bottom boundaries of a rectangle
    /// Negative values for Width & Height are illegal
    /// </summary>
    public class eBlock
    {
        #region Values
        public FBlockFlags Flags { get; private set; } = 0;
        /// <summary>
        /// Tracks whether the block needs to be recalculated, this isnt for this block instance in particular though, more so for removing variable clutter from the <see cref="cssElement"/> class.
        /// </summary>
        public bool IsDirty { get { return Flags.HasFlag(FBlockFlags.Dirty); } }
        public bool IsLocked { get { return Flags.HasFlag(FBlockFlags.Locked); } }

        public int Left { get; private set; } = 0;
        public int Top { get; private set; } = 0;
        public int Right { get; private set; } = 0;
        public int Bottom { get; private set; } = 0;

        /// <summary>
        /// Holds the origin location for the bounds
        /// </summary>
        readonly ePos Pos = new ePos();
        /// <summary>
        /// Holds the size of the bounds
        /// </summary>
        readonly eSize Size = new eSize();

        /// <summary>
        /// Returns the block's current position
        /// </summary>
        public ePos Get_Pos() { return Pos; }

        public void Set_Pos(ePos pos)
        {
            X = pos.X;
            Y = pos.Y;
        }

        /// <summary>
        /// Returns the block's current APPROXIMATE center position
        /// </summary>
        public ePos Get_Center_Pos() { return new ePos((int)Center_X, (int)Center_Y); }
        /// <summary>
        /// Returns the block's current size
        /// </summary>
        public eSize Get_Size() { return Size; }

        public void Set_Size(eSize size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// Negative values for Width are illegal as per: https://www.w3.org/TR/CSS21/visudet.html#propdef-width
        /// </summary>
        public int Width { get { return Size.Width; } set { Size.Width = Math.Max(0, value); calc_half_width(); calc_center_x(); update_ltrb(); } }
        /// <summary>
        /// Negative values for Height are illegal as per: https://www.w3.org/TR/CSS21/visudet.html#propdef-height
        /// </summary>
        public int Height { get { return Size.Height; } set { Size.Height = Math.Max(0, value); calc_half_height(); calc_center_y(); update_ltrb(); } }

        public int X { get { return Pos.X; } set { Pos.X = value; calc_center_x(); update_ltrb(); } }
        public int Y { get { return Pos.Y; } set { Pos.Y = value; calc_center_y(); update_ltrb(); } }


        float Half_Width;
        float Half_Height;
        float Center_X;
        float Center_Y;
        #endregion

        #region Flags
        public void Set_Flag(FBlockFlags flag)
        {
            Flags = (Flags | flag);
        }

        public void Unset_Flag(FBlockFlags flag)
        {
            Flags = (Flags ^ flag);
        }

        /// <summary>
        /// Flags the block as dirty, indicating that it's values should be resolved.
        /// </summary>
        public void Flag_Dirty()
        {
            Set_Flag(FBlockFlags.Dirty);
        }

        /// <summary>
        /// Flags the block not dirty.
        /// </summary>
        public void Flag_Clean()
        {
            Unset_Flag(FBlockFlags.Dirty);
        }


        /// <summary>
        /// Lock the block indicating it's values should not be changed, also flags the block as dirty.
        /// </summary>
        public void Lock()
        {
            Set_Flag(FBlockFlags.Locked | FBlockFlags.Dirty);
        }

        /// <summary>
        /// Unlock the block indicating it's values can be changed.
        /// </summary>
        public void Unlock()
        {
            Unset_Flag(FBlockFlags.Locked);
        }
        #endregion

        #region Constructors
        public eBlock() { }
        public eBlock(eBlock block)
        {
            Pos.X = block.Pos.X;
            Pos.Y = block.Pos.Y;
            Size.Width = block.Size.Width;
            Size.Height = block.Size.Height;

            calc_half_width();
            calc_half_height();
            calc_center_x();
            calc_center_y();
            update_ltrb();
        }
        public eBlock(ePos pos, eSize size)
        {
            Pos.X = pos.X;
            Pos.Y = pos.Y;
            Size.Width = Math.Max(0, size.Width);
            Size.Height = Math.Max(0, size.Height);

            calc_half_width();
            calc_half_height();
            calc_center_x();
            calc_center_y();
            update_ltrb();
        }

        public static eBlock FromTRBL(int Top, int Right, int Bottom, int Left)
        {
            return new eBlock(new ePos(Left, Top), new eSize(Right - Left, Bottom - Top));
        }
        #endregion

        #region Updating
        void calc_center_x() { Center_X = ((float)Pos.X + Half_Width); }
        void calc_center_y() { Center_Y = ((float)Pos.Y + Half_Height); }
        void calc_half_width() { Half_Width = (0.5f * (float)Size.Width); }
        void calc_half_height() { Half_Height = (0.5f * (float)Size.Height); }
        
        void update_ltrb()
        {
            Left = Pos.X;
            Top = Pos.Y;
            Right = (Pos.X + Size.Width);
            Bottom = (Pos.Y + Size.Height);
        }
        #endregion


        /// <summary>
        /// Adds the given offsets total width & height to the block
        /// </summary>
        /// <param name="off"></param>
        /// <returns></returns>
        public eBlock Add(eBlockOffset off)
        {
            return FromTRBL(Top, Right + off.Horizontal, Bottom + off.Vertical, Left);
        }

        /// <summary>
        /// Decreases the bounds area inwards by the given padding values
        /// <para>Keeps the block centered</para>
        /// </summary>
        public eBlock Sub(eBlockOffset off)
        {
            return FromTRBL(Top+off.Top, Right - off.Right, Bottom - off.Bottom, Left + off.Left);
        }

        #region Operators
        /// <summary>
        /// Decreases the bounds area inwards by the given padding values
        /// <para>Keeps the block centered</para>
        /// </summary>
        public static eBlock operator -(eBlock bounds, eBlockOffset off)
        {
            return FromTRBL(bounds.Top + off.Top, bounds.Right - off.Right, bounds.Bottom - off.Bottom, bounds.Left + off.Left);
        }

        /// <summary>
        /// Increases the bounds area outwards by the given padding values
        /// </summary>
        public static eBlock operator +(eBlock bounds, eBlockOffset off)
        {
            return FromTRBL(bounds.Top, bounds.Right + off.Horizontal, bounds.Bottom + off.Vertical, bounds.Left);
        }
        
        public static bool operator ==(eBlock A, eBlock B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return (object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.Left == B.Left && A.Top == B.Top && A.Right == B.Right && A.Bottom == B.Bottom);
        }

        public static bool operator !=(eBlock A, eBlock B)
        {
            if (object.ReferenceEquals(A, null) || object.ReferenceEquals(B, null)) return !(object.ReferenceEquals(A, null) && object.ReferenceEquals(B, null));
            return (A.Left != B.Left || A.Top != B.Top || A.Right != B.Right || A.Bottom != B.Bottom);
        }

        public override string ToString() { return string.Format("["+ nameof(eBlock) + "]<X:{0}, Y:{1}, W:{2}, H:{3}>", X.ToString(), Y.ToString(), Width.ToString(), Height.ToString()); }

        public override bool Equals(object o)
        {

            if (o is eBlock)
            {
                return this == (eBlock)o;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Width ^ Height;
        }
        #endregion

        #region Intersection
        /// <summary>
        /// Returns True if the given point lies within this block
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Intersects(ePos point)
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
        public bool Intersects(eBlock block)
        {
            return (Math.Abs(Center_X - block.Center_X) <= (Half_Width + block.Half_Width)) &&
                   (Math.Abs(Center_Y - block.Center_Y) <= (Half_Height + block.Half_Height));
        }

        #endregion
    }
}
