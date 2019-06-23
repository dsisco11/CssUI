using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Defines all of the stlying distance unit types
    /// </summary>
    public enum EStyleUnit : byte
    {
        /// <summary>
        /// Specified no unit length
        /// </summary>
        None = 0,
        /// <summary>
        /// Relative to pixel size
        /// </summary>
        PX,
        /// <summary>
        /// Relative to font size
        /// </summary>
        EM,
        /// <summary>
        /// Relative to x-height of the elements font
        /// </summary>
        EX,
        /// <summary>
        /// Relative to the width of the "0" glyph in the elements font
        /// </summary>
        CH,
        /// <summary>
        /// Relative to font size of the root element
        /// </summary>
        REM,
        /// <summary>
        /// Relative to viewports width
        /// </summary>
        VW,
        /// <summary>
        /// Relative to viewports height
        /// </summary>
        VH,
        /// <summary>
        /// Relative to the minimum of the viewports height and width
        /// </summary>
        VMIN,
        /// <summary>
        /// Relative to the maximum of the viewports height and width
        /// </summary>
        VMAX,

        /// <summary>
        /// Degrees. There are 360 degrees in a full circle.
        /// </summary>
        DEG,
        /// <summary>
        /// Gradians, also known as 'gons' or 'grades'. There are 400 gradians in a full circle
        /// </summary>
        GRAD,
        /// <summary>
        /// Radians. There are 2PI radians in a full circle
        /// </summary>
        RAD,
        /// <summary>
        /// Turns. There is 1 turn in a full circle
        /// </summary>
        TURN
    }
}
