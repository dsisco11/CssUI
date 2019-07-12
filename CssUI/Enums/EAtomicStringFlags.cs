using System;

namespace CssUI.Enums
{
    [Flags]
    public enum EAtomicStringFlags : int
    {
        None = 0x0,
        /// <summary>
        /// When comparing these strings compare them case-insensitively
        /// </summary>
        CaseInsensitive = (1 << 1),
        /// <summary>
        /// This atomic string contains uppercase characters
        /// </summary>
        HasUppercase = (1 << 2),
    }
}
