using System;
using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Style value types
    /// </summary>
    [Flags, CssEnum]
    public enum ECssValueType : ulong
    {/* Docs: https://www.w3.org/TR/css-values-3/ */
        /// <summary>
        /// Nothing
        /// </summary>
        NULL = (1 << 1),
        /// <summary>
        /// CSS Keyword Used to override declerations, forcing a value to resolve to its inherited or initial values
        /// </summary>
        [CssKeyword("unset")]
        UNSET = (1 << 2),
        /// <summary>
        /// CSS Keyword
        /// Value should use whatever 'auto' logic is applicable to determine it's used value
        /// </summary>
        [CssKeyword("auto")]
        AUTO = (1 << 3),
        /// <summary>
        /// CSS Keyword
        /// Value should resolve to it's definitions default value
        /// </summary>
        [CssKeyword("initial")]
        INITIAL = (1 << 4),
        /// <summary>
        /// CSS Keyword
        /// Value is inherited from ancestor
        /// </summary>
        [CssKeyword("inherit")]
        INHERIT = (1 << 5),
        /// <summary>
        /// CSS Keyword
        /// Used by specific properties, for instance; max-width & max-height use it to specify having no limit
        /// </summary>
        [CssKeyword("none")]
        NONE = (1 << 6),
        /// <summary>
        /// CSS Keyword
        /// 
        /// </summary>
        [CssKeyword("default")]
        DEFAULT = (1 << 7),

        /// <summary>
        /// An integer is one or more decimal digits ‘0’ through ‘9’ and corresponds to a subset of the NUMBER token in the grammar. Integers may be immediately preceded by ‘-’ or ‘+’ to indicate the sign.
        /// </summary>
        INTEGER = (1 << 8),
        /// <summary>
        /// A number is either an integer, or zero or more decimal digits followed by a dot (.) followed by one or more decimal digits.
        /// </summary>
        NUMBER = (1 << 9),
        /// <summary>
        /// A dimension is a number immediately followed by a unit identifier.
        /// </summary>
        DIMENSION = (1 << 10),
        /// <summary>
        /// A percentage value is denoted by <percentage>, consists of a <number> immediately followed by a percent sign ‘%’.
        /// Percentage values are always relative to another value, for example a length.
        /// </summary>
        PERCENT = (1 << 11),

        /// <summary>
        /// Value is a 32-bit integer which represents an RGBA color value
        /// </summary>
        COLOR = (1 << 12),
        /// <summary>
        /// Value is a string
        /// </summary>
        STRING = (1 << 13),
        /// <summary>
        /// Keywords are pre-defined CSS idents that have special meaning relative to a specific properties
        /// See: https://www.w3.org/TR/css-values-4/#keywords
        /// </summary>
        KEYWORD = (1 << 14),
        /// <summary>
        /// </summary>
        FUNCTION = (1 << 15),
        /// <summary>
        /// </summary>
        IMAGE = (1 << 16),
        /// <summary>
        /// </summary>
        POSITION = (1 << 17),



/* Non-Standard */
        /// <summary>
        /// Ratios are used by the media queries
        /// </summary>
        RATIO = (1 << 32),
    }
}
