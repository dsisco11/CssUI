using System;
using System.Collections.Generic;

namespace CssUI.CSS.Formatting
{
    public class FragmentationRoot
    {/* Docs: https://www.w3.org/TR/css-break-3/ */
        #region Properties
        public List<CssBoxFragment> Fragments = new List<CssBoxFragment>();
        #endregion

        #region Constructors
        public FragmentationRoot(List<CssBoxFragment> fragments)
        {
            Fragments = fragments;
        }
        #endregion

    }
}
