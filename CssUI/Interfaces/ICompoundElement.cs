using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Encapsulates everything needed for an element to hold other elements
    /// </summary>
    public interface ICompoundElement
    {
        #region Events
        #endregion
        
        #region Content
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the maximum size which content for this control can take up
        /// </summary>
        eSize Get_Layout_Area();
        #endregion

        #region Layout
        ELayoutMode Layout { get; }
        #endregion

        #region Controls
        IEnumerable<cssElement> Items { get; }
        #endregion
    }
}
