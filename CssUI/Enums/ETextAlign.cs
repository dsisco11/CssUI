using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// A block of text is a stack of line boxes. This property specifies how the inline-level boxes within each line box align with respect to the start and end sides of the line box. Alignment is not with respect to the viewport or containing block.
    /// </summary>
    public enum ETextAlign : int
    {
        /// <summary>Inline-level content is aligned to the start edge of the line box.</summary>
        Start,
        /// <summary>Inline-level content is aligned to the end edge of the line box.</summary>
        End,
        /// <summary>Inline-level content is aligned to the line left edge of the line box. (In vertical writing modes, this will be either the physical top or bottom, depending on ‘text-orientation’.) [CSS3-WRITING-MODES]</summary>
        Left,
        /// <summary>Inline-level content is aligned to the line right edge of the line box. (In vertical writing modes, this will be either the physical top or bottom, depending on ‘text-orientation’.) [CSS3-WRITING-MODES]</summary>
        Right,
        /// <summary>Inline-level content is centered within the line box.</summary>
        Center,
        /// <summary>Text is justified according to the method specified by the ‘text-justify’ property, in order to exactly fill the line box.</summary>
        Justify,
        /// <summary>This value behaves the same as ‘inherit’ (computes to its parent's computed value) except that an inherited ‘start’ or ‘end’ keyword is interpreted against its parent's ‘direction’ value and results in a computed value of either ‘left’ or ‘right’.</summary>
        MatchParent,
        /// <summary>Specifies ‘start’ alignment of the first line and any line immediately after a forced line break; and ‘end’ alignment of any remaining lines.</summary>
        StartEnd
    }
}
