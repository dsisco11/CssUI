using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Implements W3C Box-Model layout logic.
    /// </summary>
    public sealed class Layout_BoxModel : LayoutDirectorBase
    {
        #region Constructors
        public Layout_BoxModel() { }
        #endregion
        
        public override eBlock Handle(IParentElement Owner, cssElement[] controls)
        {
            eSize MaxArea = Owner.Get_Layout_Area();
            Reset();

            for(int i=0; i<controls.Length; i++)
            {
                var E = controls[i];
                if (!E.Affects_Layout) continue;

                // Get the elements bounds
                
                var cBlock = E.Peek_Block();
                var cSize = cBlock.Get_Size();// E.Get_Layout_Size();
                int cRight = (layoutPos.X + cSize.Width);

                // If the last element we positioned was a block-level element then start a new row...
                if (Previous != null && Previous.Style.Display == EDisplayMode.BLOCK) Start_New_Line();
                // If this is a block element and we already have other elements on this row then start a new row...
                else if (E.Style.Display == EDisplayMode.BLOCK && Line.Count > 0) Start_New_Line();
                // Check if putting the element on the current row would place it out of bounds.
                else if (cRight > MaxArea.Width && Line.Count > 0)// Check if this control will extend beyond the edges of it's container
                {
                    // This isnt the first element of this row (this check ensures each row has atleast 1 element on it, even if they go out of bounds)
                    Start_New_Line();
                }

                // ====[ BLOCK entitys get a row to themselves ]====

                // Add the current element to our line
                Add_To_Line(E, cBlock, cSize);
            }

            if (Line.Count > 0) Start_New_Line();// Make sure our line in progress is added

            Perform_Alignment((Owner as cssElement).Style.TextAlign);
            Finalize_Positions();

            return layoutBlock;
        }
        
    }
}
