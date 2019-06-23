

namespace CssUI
{
    public enum EDisplayMode : int
    {
        /// <summary>This value causes an element to generate no boxes (i.e., the element has no effect on layout). Descendant elements do not generate any boxes either; this behavior cannot be overridden by setting the ‘display’ property on the descendants.</summary>
        NONE = 0,
        /// <summary>Element takes up an entire line by itself</summary>
        BLOCK,
        /// <summary>Element takes up only as much space as it needs, inline elements cannot specify an explicit width or height, their dimensions are completely determined by their content.</summary>
        INLINE,
        /// <summary></summary>
        INLINE_BLOCK,

    }
}