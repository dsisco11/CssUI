using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Stores all information about an elements block
    /// Includes: Margin_Block, Border_Block, Padding_Block, Content_Block
    /// </summary>
    public class ElementBlock
    {
        #region Blocks
        /// <summary>
        /// Offset applied to the containing-block when calculating <see cref="Block_Content"/> to account for the presence of a scrollbar (if any)
        /// </summary>
        public readonly eBlockOffset Scrollbar_Offset = new eBlockOffset();
        /// <summary>
        /// The block for an element extending up to the edge of it's borders
        /// <para>‘border-left-width’ + ‘padding-left’ + ‘width’ + ‘padding-right’ + scrollbar width (if any) + ‘border-right-width’ = width of border block</para>
        /// </summary>
        public eBlock Block_Border = new eBlock();
        /// <summary>
        /// The block for an element extending up to the edge of it's padding.
        /// An elements background is contained within this block.
        /// <para>‘padding-left’ + ‘width’ + ‘padding-right’ = width of padding block</para>
        /// </summary>
        public eBlock Block_Padding = new eBlock();
        /// <summary>
        /// The block for an element extending up to the edge of it's content-area
        /// </summary>
        public eBlock Block_Content = new eBlock();
        /// <summary>
        /// The block for an element that dictates the area it's content may be drawn in, or NULL if no clipping should be used
        /// </summary>
        public eBlock ClipArea = null;
        /// <summary>
        /// The block which represents the hitbox for mouse related input events
        /// </summary>
        public eBlock ClickArea { get { return Block_Padding; } }
        public eBlock Containing_Block
        {
            get
            {
                switch (Element.Style.Positioning)
                {
                    case EPositioning.Fixed:
                        return Element?.Root?.Viewport?.Block;
                    default:
                        return Element?.Parent?.Block_Content;
                }
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// The UI element this block belongs to
        /// </summary>
        public readonly cssElement Element = null;
        /// <summary>
        /// Specifies what kind of block this is.
        /// </summary>
        public readonly EBlockType Type = EBlockType.Element;

        public readonly int Width = 0;
        public readonly int Height = 0;

        public readonly int Top = 0;
        public readonly int Right = 0;
        public readonly int Bottom = 0;
        public readonly int Left = 0;

        public readonly int Margin_Top = 0;
        public readonly int Margin_Right = 0;
        public readonly int Margin_Bottom = 0;
        public readonly int Margin_Left = 0;

        public readonly int Padding_Top = 0;
        public readonly int Padding_Right = 0;
        public readonly int Padding_Bottom = 0;
        public readonly int Padding_Left = 0;

        public readonly int Border_Top = 0;
        public readonly int Border_Right = 0;
        public readonly int Border_Bottom = 0;
        public readonly int Border_Left = 0;
        #endregion

        #region Constructors
        public ElementBlock(EBlockType Type, int Width, int Height)
        {
            this.Type = Type;
            this.Width = Width;
            this.Height = Height;
        }
        public ElementBlock(EBlockType Type, cssElement Owner, ElementPropertySystem Style)
        {
            this.Type = Type;
            this.Element = Owner;

            this.Width = Style.Width;
            this.Height = Style.Height;

            this.Top = Style.Top;
            this.Right = Style.Right;
            this.Bottom = Style.Bottom;
            this.Left = Style.Left;

            this.Margin_Top = Style.Margin_Top;
            this.Margin_Right = Style.Margin_Right;
            this.Margin_Bottom = Style.Margin_Bottom;
            this.Margin_Left = Style.Margin_Left;

            this.Padding_Top = Style.Padding_Top;
            this.Padding_Right = Style.Padding_Right;
            this.Padding_Bottom = Style.Padding_Bottom;
            this.Padding_Left = Style.Padding_Left;

            this.Border_Top = Style.Border_Top_Width;
            this.Border_Right = Style.Border_Right_Width;
            this.Border_Bottom = Style.Border_Bottom_Width;
            this.Border_Left = Style.Border_Left_Width;
        }
        #endregion


    }
}
