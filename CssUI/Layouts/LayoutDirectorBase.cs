using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssUI.Enums;
using CssUI.Layout;

namespace CssUI
{
    public abstract class LayoutDirectorBase : ILayoutDirector
    {
        #region Properties
        protected cssBoxArea layoutBlock;
        protected Vec2i layoutPos;
        /// <summary>
        /// The biggest control height we encountered for a particular row, so we know how far to move down
        /// </summary>
        protected int lineHeight = 0;
        /// <summary>
        /// List of element son our current layout-line
        /// </summary>
        protected List<LineBox_Element> Line;
        /// <summary>
        /// Historical list of all controls which have been positioned so far
        /// </summary>
        protected List<LineBox> Lines;
        protected cssElement Previous = null;
        #endregion

        public abstract cssBoxArea Handle(IParentElement Owner, cssElement[] controls);

        protected virtual void Reset()
        {
            layoutBlock = new cssBoxArea();
            layoutPos = new Vec2i();
            lineHeight = 0;
            Previous = null;
            Line = new List<LineBox_Element>(1);
            Lines = new List<LineBox>(1);
        }

        protected virtual void Start_New_Line()
        {
            Lines.Add(new LineBox(Line, new Size2D(layoutPos.X, lineHeight)));
            Line = new List<LineBox_Element>(1);
            // Update the width of our layout area
            int Width = Math.Max(layoutBlock.Width, layoutPos.X);
            // Update the height of our layout area
            int Height = Math.Max(layoutBlock.Height, layoutPos.Y + lineHeight);
            layoutBlock.Set_Dimensions(Width, Height);
            // Progress our current layout position to the next line
            layoutPos.X = 0;
            layoutPos.Y += lineHeight;

            // Reset the tracked height of this line
            lineHeight = 0;
            Previous = null;
        }

        protected virtual void Add_To_Line(cssElement E, cssBoxArea cArea, Size2D cSize)
        {
            Line.Add(new LineBox_Element(E, new Vec2i(layoutPos)));
            Previous = E;
            // Now we use the elements CURRENT block size here because if we just used whatever guaranteed value the element can give us then elements with a percentage padding, margin, or size would always overlap their adjacent elements!
            lineHeight = Math.Max(lineHeight, cSize.Height);
            // E.Pos.Set_Implicit(layoutPos.X, layoutPos.Y);
            // Progress our current layout position
            layoutPos.X += cSize.Width;
        }

        protected virtual void Perform_Alignment(ETextAlign TextAlign)
        {
            if (TextAlign != ETextAlign.Start && TextAlign != ETextAlign.Left)
            {
                // Now perform alignment of inline-level elements according to the text-align property of the owning element
                foreach (LineBox line in Lines)
                {
                    if (layoutBlock.Width < line.InnerSize.Width) throw new Exception("LayoutBlock width appears to be smaller than this Line-Box's!");
                    if (layoutBlock.Height < line.InnerSize.Height) throw new Exception("LayoutBlock height appears to be smaller than this Line-Box's!");

                    switch (TextAlign)
                    {
                        case ETextAlign.End:
                        case ETextAlign.Right:// Functionally the same as 'End' (in most cases)
                            {
                                line.Translate_X(layoutBlock.Width - line.InnerSize.Width);
                            }
                            break;
                        case ETextAlign.Center:
                            {
                                line.Translate_X((layoutBlock.Width - line.InnerSize.Width) / 2);
                            }
                            break;
                        default:
                            throw new NotImplementedException(string.Format("Unhandled Text-Align value ({0})!", Enum.GetName(typeof(ETextAlign), TextAlign)));
                    }
                }
            }
        }

        protected void Finalize_Positions()
        {
            // Now perform alignment of inline-level elements according to the text-align property of the owning element
            foreach (LineBox line in Lines)
            {
                foreach (var item in line.Items)
                {
                    item.Element.Box.Set_Layout_Pos(item.Pos.X, item.Pos.Y);
                }
            }
        }

    }
}
