using System;

namespace CssUI.DOM
{
    /// <summary>
    /// Used by custom element internals to specify the validity state of their form value
    /// </summary>
    [Flags]
    public enum EValidityState : int
    {/* Docs: https://html.spec.whatwg.org/multipage/custom-elements.html#elementinternals */

        valueMissing = (1 << 1),
        typeMismatch = (1 << 2),
        patternMismatch = (1 << 3),
        tooLong = (1 << 4),
        tooShort = (1 << 5),
        rangeUnderflow = (1 << 6),
        rangeOverflow = (1 << 7),
        stepMismatch = (1 << 8),
        badInput = (1 << 9),
        customError = (1 << 10),
    }
}
