using System.Collections.Generic;

namespace CssUI
{
    /// <summary>
    /// Encapsulates everything needed for an element to hold other elements
    /// </summary>
    public interface IParentElement
    {
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

        #region Children
        void Add(cssElement element);
        IEnumerable<cssElement> Items { get; }
        #endregion
    }
}
