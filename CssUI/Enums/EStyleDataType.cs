using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Style value types
    /// </summary>
    public enum EStyleDataType : byte
    {
        /// <summary>
        /// Placeholder for NULL
        /// </summary>
        UNSET = 0,
        /// <summary>
        /// Value should use whatever 'auto' logic is applicable to determine it's used value
        /// </summary>
        AUTO,
        /// <summary>
        /// Value should resolve to it's default
        /// </summary>
        INITIAL,
        /// <summary>
        /// Value is inherited from ancestor
        /// </summary>
        INHERIT,
        /// <summary>
        /// Used by specific properties, for instance; max-width & max-height use it to specify having no limit
        /// </summary>
        NONE,

        /// <summary>
        /// An integer is one or more decimal digits ‘0’ through ‘9’ and corresponds to a subset of the NUMBER token in the grammar. Integers may be immediately preceded by ‘-’ or ‘+’ to indicate the sign.
        /// </summary>
        INTEGER,
        /// <summary>
        /// A number is either an integer, or zero or more decimal digits followed by a dot (.) followed by one or more decimal digits.
        /// </summary>
        NUMBER,
        /// <summary>
        /// A dimension is a number immediately followed by a unit identifier.
        /// </summary>
        DIMENSION,
        /// <summary>
        /// A percentage value is denoted by <percentage>, consists of a <number> immediately followed by a percent sign ‘%’.
        /// Percentage values are always relative to another value, for example a length.
        /// </summary>
        PERCENT,

        /// <summary>
        /// Value is a 32-bit integer which represents an RGBA color value
        /// </summary>
        COLOR,
        /// <summary>
        /// Value is a string
        /// </summary>
        STRING
    }
}
