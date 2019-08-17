using CssUI.Rendering;

namespace CssUI.HTML
{
    /// <summary>
    /// Represents an image texture loaded into memory to be displayed
    /// </summary>
    public class EmbeddedImage : EmbeddedContent
    {
        #region Properties
        public GpuTexture Image { get; private set; } = null;
        #endregion

        #region Constructors
        public EmbeddedImage()
        {
        }
        #endregion

        #region Accessors
        internal override int? Intrinsic_Width => Image?.Size.Width;
        internal override int? Intrinsic_Height => Image?.Size.Height;
        #endregion


    }
}
