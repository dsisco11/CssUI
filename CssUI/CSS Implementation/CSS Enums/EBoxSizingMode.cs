﻿using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Describes all of the box sizing modes <see cref="cssElement"/>'s can use
    /// </summary>
    [CssEnum]
    public enum EBoxSizingMode : int
    {
        /// <summary>
        /// Width/Height describe the size of the content area, not including the padding, border, or margins
        /// </summary>
        [CssKeyword("content-box")]
        ContentBox,
        /// <summary>
        /// Width/Height describe the border area, including the content size, padding, and border size but not the margins
        /// </summary>
        [CssKeyword("border-box")]
        BorderBox,
    }
}