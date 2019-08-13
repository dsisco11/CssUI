using CssUI.Internal;

namespace CssUI.CSS.Media
{
    [MetaEnum]
    public enum EMediaScan
    {
        /// <summary>
        /// CRT and some types of plasma TV screens used “interlaced” rendering, 
        /// where video frames alternated between specifying only the “even” lines on the screen and only the “odd” lines, 
        /// exploiting various automatic mental image-correction abilities to produce smooth motion. 
        /// This allowed them to simulate a higher FPS broadcast at half the bandwidth cost.
        /// </summary>
        [MetaKeyword("interlace")]
        Interlace,

        /// <summary>
        /// A screen using “progressive” rendering displays each screen fully, and needs no special treatment.
        /// Most modern screens, and all computer screens, use progressive rendering.
        /// </summary>
        [MetaKeyword("progressive")]
        Progressive,
    }
}
