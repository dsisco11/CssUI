using CssUI.CSS.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Implements border styling for all four sides of a control.
    /// </summary>
    public class uiBorderStyle
    {
        public static uiBorderStyle Default { get { return new uiBorderStyle() { Left = uiBorder_Propertys.Default, Top = uiBorder_Propertys.Default, Right = uiBorder_Propertys.Default, Bottom = uiBorder_Propertys.Default }; } }
        #region Values
        public uiBorder_Propertys Left = new uiBorder_Propertys();
        public uiBorder_Propertys Top = new uiBorder_Propertys();
        public uiBorder_Propertys Right = new uiBorder_Propertys();
        public uiBorder_Propertys Bottom = new uiBorder_Propertys();
        public bool Dirty { get { return (Left.Dirty || Top.Dirty || Right.Dirty || Bottom.Dirty); } set { Left.Dirty = Top.Dirty = Right.Dirty = Bottom.Dirty = value; } }
        //public glTexture texture { get; private set; } = null;
        #endregion

        #region Constructors
        public uiBorderStyle() { }
        public uiBorderStyle(int left, int top, int right, int bottom)
        {
            Left.Size = left;
            Top.Size = top;
            Right.Size = right;
            Bottom.Size = bottom;
        }
        #endregion

        public eBlockOffset Get_Offsets() { return new eBlockOffset(Left.Size, Top.Size, Right.Size, Bottom.Size); }
    }

    /// <summary>
    /// Styling for a single border side
    /// </summary>
    public class uiBorder_Propertys
    {
        public static uiBorder_Propertys Default { get { return new uiBorder_Propertys(cssColor.White, 2, EBorderStyle.None); } }

        #region Properties
        public bool Dirty { get; set; } = true;

        private cssColor color = null;
        private int width = 2;
        private EBorderStyle? style = null;
        #endregion

        #region Accessors
        /// <summary>
        /// Color of the border
        /// </summary>
        public cssColor Color { get { return color; } set { Dirty = true; color = value; } }
        /// <summary>
        /// Returns the 'Used' width for the border.
        /// <para>if the Style is set to 'none' then the returned value will always be 0</para>
        /// </summary>
        public int Size { get { return (Style == EBorderStyle.None ? 0 : width); } set { Dirty = true; width = value; } }
        /// <summary>
        /// 'Style' of the border, dictates how the border is drawn.
        /// </summary>
        public EBorderStyle? Style { get { return style; } set { Dirty = true; style = value; } }
        /// <summary>
        /// Returns whether the border should be drawn.
        /// </summary>
        public bool IsVisible { get { return (!object.ReferenceEquals(null, color) && Size > 0); } }
        #endregion

        #region Constructors
        public uiBorder_Propertys() { }

        public uiBorder_Propertys(cssColor Color, int Width, EBorderStyle? Style)
        {
            this.Color = Color;
            this.Size = Width;
            this.Style = Style;
        }
        #endregion

    }
}
