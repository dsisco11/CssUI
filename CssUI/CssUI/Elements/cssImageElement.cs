using CssUI.CSS;
using CssUI.DOM;

namespace CssUI
{
    /// <summary>
    /// Represents an image rendered by the OpenGL UI system
    /// </summary>
    public class cssImageElement : cssReplacedElement, IImageElement
    {
        public static readonly new string CssTagName = "img";
        public override bool IsEmpty { get { return (Texture == null); } }

        #region Accessors
        /// <summary>
        /// Sets the current image texture
        /// </summary>
        public void Set_Image(cssTexture Tex)
        {
            Texture = Tex;
            if (Texture == null)
            {
                Box.Set_Intrinsic_Size(null, null);
                Box.Set_Content_Size(null, null);
            }
            else
            {
                Box.Set_Intrinsic_Size(Texture.Size.Width, Texture.Size.Height);
                Box.Set_Content_Size(Texture.Size.Width, Texture.Size.Height);
            }
        }

        /// <summary>
        /// Size of the current image
        /// </summary>
        public Size2D Image_Size { get { return new Size2D(Texture.Size); } }
        #endregion

        #region Constructors
        public cssImageElement(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, EReplacedElementType.IMAGE, className, ID)
        {
            Style.ImplicitRules.ObjectFit.Set(EObjectFit.Contain);
        }
        #endregion
        
    }
}
