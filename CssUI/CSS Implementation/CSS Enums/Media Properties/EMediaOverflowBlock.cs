using CssUI.Internal;

namespace CssUI.CSS.Media
{
    [MetaEnum]
    public enum EMediaOverflowBlock
    {

        /// <summary>
        /// There is no affordance for overflow in the block axis; any overflowing content is simply not displayed. 
        /// Examples: billboards
        /// </summary>
        [MetaKeyword("none")]
        None,

        /// <summary>
        /// Overflowing content in the block axis is exposed by allowing users to scroll to it. 
        /// Examples: computer screens
        /// </summary>
        [MetaKeyword("scroll")]
        Scroll,

        /// <summary>
        /// Overflowing content in the block axis is exposed by allowing users to scroll to it, 
        /// but page breaks can be manually triggered (such as via break-inside, etc) to cause the following content to display on the following page. 
        /// Examples: slideshows
        /// </summary>
        [MetaKeyword("optional-paged")]
        Optional_Paged,

        /// <summary>
        /// Content is broken up into discrete pages; content that overflows one page in the block axis is displayed on the following page. 
        /// Examples: printers, ebook readers
        /// </summary>
        [MetaKeyword("paged")]
        Paged,
    }
}
