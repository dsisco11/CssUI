using System;
using CssUI.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// Style value types
    /// </summary>
    [Flags]
    public enum ECssValueTypes : int
    {/* Docs: https://www.w3.org/TR/css-values-3/ */

        /// <summary>
        /// Nothing
        /// </summary>
        NULL = (1 << 0),
        /// <summary>
        /// CSS Keyword Used to override declerations, forcing a value to resolve to its inherited or initial values
        /// </summary>
        UNSET = (1 << 1),
        /// <summary>
        /// CSS Keyword
        /// Value should use whatever 'auto' logic is applicable to determine it's used value
        /// </summary>
        AUTO = (1 << 2),
        /// <summary>
        /// CSS Keyword
        /// Value should resolve to it's definitions default value
        /// </summary>
        INITIAL = (1 << 3),
        /// <summary>
        /// CSS Keyword
        /// Value is inherited from ancestor
        /// </summary>
        INHERIT = (1 << 4),
        /// <summary>
        /// CSS Keyword
        /// Used by specific properties, for instance; max-width & max-height use it to specify having no limit
        /// </summary>
        NONE = (1 << 5),
        /// <summary>
        /// CSS Keyword
        /// </summary>
        DEFAULT = (1 << 6),


        /// <summary>
        /// An integer is one or more decimal digits ‘0’ through ‘9’ and corresponds to a subset of the NUMBER token in the grammar. Integers may be immediately preceded by ‘-’ or ‘+’ to indicate the sign.
        /// </summary>
        INTEGER = (1 << 16),
        /// <summary>
        /// A number is either an integer, or zero or more decimal digits followed by a dot (.) followed by one or more decimal digits.
        /// </summary>
        NUMBER = (1 << 17),
        /// <summary>
        /// A dimension is a number immediately followed by a unit identifier.
        /// </summary>
        DIMENSION = (1 << 18),
        /// <summary>
        /// A percentage value is denoted by <percentage>, consists of a <number> immediately followed by a percent sign ‘%’.
        /// Percentage values are always relative to another value, for example a length.
        /// </summary>
        PERCENT = (1 << 19),

        /// <summary>
        /// Value is a 32-bit integer which represents an RGBA color value
        /// </summary>
        COLOR = (1 << 20),
        /// <summary>
        /// Value is a string
        /// </summary>
        STRING = (1 << 21),
        /// <summary>
        /// Keywords are pre-defined CSS idents that have special meaning relative to a specific properties
        /// </summary>
        /* Docs: https://www.w3.org/TR/css-values-4/#keywords */
        KEYWORD = (1 << 22),

        /// <summary>
        /// The value is an instance of <see cref="CssFunction"/>.
        /// </summary>
        FUNCTION = (1 << 23),
        /// <summary>
        /// </summary>
        IMAGE = (1 << 24),
        /// <summary>
        /// Specifies the position of an object area inside a positioning area.
        /// The value is an instance of <see cref="Point2f"/>.
        /// This value initially consists of multiple sub-values eg: 'left' and '50%' and it's type before the computed stage is <see cref="COLLECTION"/>.
        /// Docs: https://www.w3.org/TR/css-values-4/#position
        /// </summary>
        POSITION = (1 << 25),
        /// <summary>
        /// Collection values are actually an array of multiple sub values
        /// </summary>
        COLLECTION = (1 << 26),



        /// <summary>
        /// Ratios are used by the media queries
        /// </summary>
        RATIO = (1 << 30),
        /// <summary>
        /// Resolutions are used by the media queries
        /// </summary>
        RESOLUTION = (1 << 31),
    }
}
