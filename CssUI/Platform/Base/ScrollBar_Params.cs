using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.Platform
{
    /// <summary>
    /// Holds settings for scrollbars.
    /// </summary>
    public struct ScrollBar_Params
    {
        /// <summary>
        /// The width or height of the scrollbar (depending on if it is a horizontal or vertical bar)
        /// </summary>
        public int Size;
        /// <summary>
        /// The width or height of the scrollbars thumb (depending on if it is a horizontal or vertical bar)
        /// </summary>
        public int ThumbSize;
        /// <summary>
        /// The width or height which the arrow graphic on the buttons should be
        /// </summary>
        public int BtnArrowSize;
    }
}
