using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.Layout
{
    /// <summary>
    /// Represents a line-box for layouts
    /// </summary>
    public class LineBox
    {
        /// <summary>
        /// The elements which occupy this line
        /// </summary>
        public List<LineBox_Element> Items;
        /// <summary>
        /// Size of the area which this line's elements occupy
        /// </summary>
        public Size2D InnerSize;

        public LineBox(List<LineBox_Element> items, Size2D inner_size)
        {
            Items = items;
            InnerSize = inner_size;
        }

        /// <summary>
        /// Translates the currently set implicit position for all of this lines elements by a given amount.
        /// </summary>
        /// <param name="X">Distance to translate on the X-axis</param>
        /// <param name="Y">Distance to translate on the Y-axis</param>
        public void Translate(int X, int Y)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var E = Items[i];
                E.Pos.X += X;
                E.Pos.Y += Y;
                // if (E.Pos.X.Implicit.Has_Flags(StyleValueFlags.Absolute)) E.Pos.Set_Implicit_X(E.Pos.X.Implicit.AsInt() + X);
                // if (E.Pos.Y.Implicit.Has_Flags(StyleValueFlags.Absolute)) E.Pos.Set_Implicit_Y(E.Pos.Y.Implicit.AsInt() + Y);
            }
        }

        /// <summary>
        /// Translates the currently set implicit X-Axis position for all of this lines elements by a given amount.
        /// </summary>
        /// <param name="Dist">Distance to translate by</param>
        public void Translate_X(int Dist)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var E = Items[i];
                E.Pos.X += Dist;
                // if (E.Pos.X.Implicit.Has_Flags(StyleValueFlags.Absolute)) E.Pos.Set_Implicit_X(E.Pos.X.Implicit.AsInt() + Dist);
            }
        }

        /// <summary>
        /// Translates the currently set implicit Y-Axis position for all of this lines elements by a given amount.
        /// </summary>
        /// <param name="Dist">Distance to translate by</param>
        public void Translate_Y(int Dist)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var E = Items[i];
                E.Pos.Y += Dist;
                // if (E.Pos.Y.Implicit.Has_Flags(StyleValueFlags.Absolute)) E.Pos.Set_Implicit_Y(E.Pos.Y.Implicit.AsInt() + Dist);
            }
        }
    }
}
