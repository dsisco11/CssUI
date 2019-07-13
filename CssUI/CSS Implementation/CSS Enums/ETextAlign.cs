using CssUI.CSS.Internal;

namespace CssUI.CSS
{
    /// <summary>
    /// A block of text is a stack of line boxes. This property specifies how the inline-level boxes within each line box align with respect to the start and end sides of the line box. Alignment is not with respect to the viewport or containing block.
    /// </summary>
    [CssEnum]
    public enum ETextAlign : int
    {
        /// <summary>Inline-level content is aligned to the start edge of the line box.</summary>
        [CssKeyword("start")]
        Start,
        /// <summary>Inline-level content is aligned to the end edge of the line box.</summary>
        [CssKeyword("end")]
        End,
        /// <summary>Inline-level content is aligned to the line left edge of the line box. (In vertical writing modes, this will be either the physical top or bottom, depending on ‘text-orientation’.) [CSS3-WRITING-MODES]</summary>
        [CssKeyword("left")]
        Left,
        /// <summary>Inline-level content is aligned to the line right edge of the line box. (In vertical writing modes, this will be either the physical top or bottom, depending on ‘text-orientation’.) [CSS3-WRITING-MODES]</summary>
        [CssKeyword("right")]
        Right,
        /// <summary>Inline-level content is centered within the line box.</summary>
        [CssKeyword("center")]
        Center,
        /// <summary>Text is justified according to the method specified by the ‘text-justify’ property, in order to exactly fill the line box.</summary>
        [CssKeyword("justify")]
        Justify,
        /// <summary>This value behaves the same as ‘inherit’ (computes to its parent's computed value) except that an inherited ‘start’ or ‘end’ keyword is interpreted against its parent's ‘direction’ value and results in a computed value of either ‘left’ or ‘right’.</summary>
        [CssKeyword("match-parent")]
        MatchParent,
        /// <summary>Specifies ‘start’ alignment of the first line and any line immediately after a forced line break; and ‘end’ alignment of any remaining lines.</summary>
        [CssKeyword("start-end")]
        StartEnd
    }
}
