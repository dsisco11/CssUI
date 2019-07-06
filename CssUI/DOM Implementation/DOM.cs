using System;
using System.Collections.Generic;

namespace CssUI.DOM
{
    // XXX: finish this
    /// <summary>
    /// A Document Object Model implementation
    /// </summary>
    public class DOM
    {
        #region Propreties
        /// <summary>
        /// The root element, aka the body.
        /// </summary>
        public readonly cssRootElement Body;
        #endregion

        #region Constructors
        public DOM(cssRootElement Root)
        {
            Body = Root;
        }
        #endregion
    }
}
