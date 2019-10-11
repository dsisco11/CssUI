using System;
using System.Collections.Generic;
using System.Linq;

namespace CssUI.CSS.Formatting
{
    public class FragmentationRoot
    {/* Docs: https://www.w3.org/TR/css-break-3/ */
        #region Properties
        public List<CssBoxFragment> Fragments = new List<CssBoxFragment>();
        #endregion

        #region Constructors
        public FragmentationRoot(ICollection<CssBoxFragment> fragments)
        {
            Fragments = fragments.ToList();
        }
        #endregion

    }
}
