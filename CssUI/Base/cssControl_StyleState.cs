using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CssUI
{
    /*
     * Ok so the eStyle class is supposed to basically implement the same kind of element-state based value transitioning system that the web CSS3 standard uses.
     * For a list of the intended element states see: https://www.w3.org/TR/CSS21/selector.html%23id-selectors#dynamic-pseudo-classes
     * 
     * This means that it needs to:
     * 1)  Track the different values a property has at the different states
     * 2)  Hold the CURRENT value instance for each property
     * 3)  Apply transition logic when needed
    */
    /*
     * So to form a class that can meet these criteria and hold all of the styling properties for an element it must:
     * - Holds values for each property at it's different states
     * - Allow us to detect when a property is being changed
     * - Allow us to make a copy of the property's current value (for transitioning purposes)
     * - Allow us to override what absolute value is seen for the property as it transitions
     * - Determine what part of the element that property affects so it can be flagged as dirty (maybe a flag system) eg: Block, Text Color, etc.
     */

    enum StylePropIndex : uint
    {
        Null = 0,
        Pos,
        Size,
        Padding,
        Margin,

    }

    /// <summary>
    /// Holds the style data for a <see cref="cssElement"/>
    /// </summary>
    public class cssStyle
    {
    }

    /// <summary>
    /// Holds all the style related data for an individual state
    /// </summary>
    public class cssStyle_State
    {
        #region Color
        /// <summary>
        /// Forecolor of the control
        /// </summary>
        public cssColor Color = null;
        /// <summary>
        /// Background color of the control, if <c>Null</c> then no background will be drawn
        /// </summary>
        public cssColor ColorBackground = null;
        #endregion

        #region Border Style
        /// <summary>
        /// Styling information for the controls borders
        /// </summary>
        uiBorderStyle Border = new uiBorderStyle();
        #endregion

        public cssStyle_State() { }

        public static cssStyle_State Default
        {
            get
            {
                return new cssStyle_State()
                {
                    Color = cssColor.White,
                };
            }
        }
    }
}
