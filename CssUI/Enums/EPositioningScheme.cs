using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Defines all of the different positioning 'scheme' types for elements
    /// </summary>
    public enum EPositioningScheme
    {
        /// <summary>Object is positioned according to normal flow logic</summary>
        Normal,
        /// <summary>Object is laid out like in normal flow but then moved as far left or right as possible</summary>
        Float,
        /// <summary>Object is not positioned according to normal flow, it defines it's own position relative to a block other than its logical containing block</summary>
        Absolute
    }
}
