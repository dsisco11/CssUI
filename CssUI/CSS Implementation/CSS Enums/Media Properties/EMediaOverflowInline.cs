﻿using CssUI.CSS.Internal;

namespace CssUI.CSS.Media
{
    [CssEnum]
    public enum EMediaOverflowInline
    {

        /// <summary>
        /// There is no affordance for overflow in the block axis; any overflowing content is simply not displayed. 
        /// Examples: billboards
        /// </summary>
        [CssKeyword("none")]
        None,

        /// <summary>
        /// Overflowing content in the block axis is exposed by allowing users to scroll to it. 
        /// Examples: computer screens
        /// </summary>
        [CssKeyword("scroll")]
        Scroll,
    }
}
