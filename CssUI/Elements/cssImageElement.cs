using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// Represents an image rendered by the OpenGL UI system
    /// </summary>
    public class cssImageElement : cssReplacedElement, IImageElement
    {
        public override string TypeName { get { return "Image"; } }
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
                Set_Intrinsic_Size(0, 0);
                Style.UserRules.Content_Width.Set(0);
                Style.UserRules.Content_Height.Set(0);
                Style.Set_Content_Width(0);
                Style.Set_Content_Height(0);
            }
            else
            {
                Set_Intrinsic_Size(Texture.Size.Width, Texture.Size.Height);
                Style.Set_Content_Width(Texture.Size.Width);
                Style.Set_Content_Height(Texture.Size.Height);
            }
        }

        /// <summary>
        /// Size of the current image
        /// </summary>
        public eSize Image_Size { get { return new eSize(Texture.Size); } }
        #endregion

        #region Constructors
        public cssImageElement(IParentElement Parent, string className = null, string ID = null) : base(Parent, EReplacedElementType.IMAGE, className, ID)
        {
            Style.ImplicitRules.ObjectFit.Value = EObjectFit.Contain;
        }
        #endregion
        
    }
}
