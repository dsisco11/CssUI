using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public static class UI_CONSTANTS
    {
        /// <summary>
        /// Time delay(in ms) after the mouse stops moving overtop a ui element, before it is considered to be 'hovering' over that element.
        /// </summary>
        public static int HOVER_TIME = 300;
        /// <summary>
        /// Minimum distance the mouse must click and drag before an element considers the input a dragging operation
        /// </summary>
        public static int DRAG_START_THRESHOLD = 5;
        /// <summary>
        /// Minimum distance to the trackbar which our mouse must remain within or else the drag preview reverts back to it's initial value.
        /// </summary>
        public static int TRACKBAR_DRAG_THRESHOLD = 150;
    }
}
