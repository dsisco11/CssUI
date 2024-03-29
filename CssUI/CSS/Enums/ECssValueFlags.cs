﻿using System;

namespace CssUI.CSS.Internal
{
    /// <summary>
    /// Flags which describe the attributes for a <see cref="CSSValue"/>
    /// </summary>
    [Flags]
    public enum ECssValueFlags : int
    {
        /// <summary>
        /// </summary>
        None = 0x0,

        /// <summary>
        /// The final value will depend on values from other propertys
        /// </summary>
        Depends = (1 << 1),

        /// <summary>
        /// Value can be already be resolved to a concrete number, it is "absolutely" defined and does not depend on the state of any other values.
        /// </summary>
        Absolute = (1 << 2),

    }
}
