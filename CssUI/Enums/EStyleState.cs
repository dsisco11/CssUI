using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Defines all of the different styling-states a control can have
    /// </summary>
    public enum EStyleState
    {
        /// <summary>
        /// Style used by default
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Style used when the mouse is overtop the control
        /// </summary>
        Hover,
        /// <summary>
        /// Style used when the mouse button is held down on the control
        /// </summary>
        Active,
        /// <summary>
        /// Style used when a control has focus eg; is selected.
        /// </summary>
        Focused
    }
}
