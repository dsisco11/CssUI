using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Defines the parts of an element that a property can affect
    /// </summary>
    [Flags]
    public enum EPropertyFlags : uint
    {
        /// <summary>Property primarily affects the visual appearence of the element</summary>
        Visual = (1 << 0),
        /// <summary>Property primarily changes the size of an elements blocks</summary>
        Block = (1 << 1),
        /// <summary>Property might change the flow(layout) positions of child elements</summary>
        Flow = (1 << 2),
        /// <summary>Property might change the elements font</summary>
        Font = (1 << 3),
    }
}
