using CssUI.CSS;
using CssUI.DOM;
using CssUI.Enums;

namespace CssUI
{
    /// <summary>
    /// A replaced element is one whose content has intrinsic dimensions and thus needs to be drawn and sized differently.
    /// <para>Intrinsic dimensions here means that the Width/Height is defined by the content itself and not what the element imposes, Eg: images</para>
    /// </summary>
    public abstract class cssReplacedElement : cssElement
    {
        #region Properties
        public readonly EReplacedElementType Kind = EReplacedElementType.NONE;
        #endregion

        #region Accessors
        protected bool Dirty_Texture = false;
        public cssTexture Texture { get; protected set; } = null;
        #endregion
        
        #region Constructors
        public cssReplacedElement(Document document, IParentElement Parent, EReplacedElementType Kind, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            this.Kind = Kind;
            this.Box.Flags |= EBoxFlags.REPLACED_ELEMENT;
        }
        #endregion

        #region Drawing
        protected override void Draw()
        {
            Draw_Background();

            Root.Engine.Set_Color(Color);
            //Root.Engine.Set_Texture((Texture == null ? glTexture.Default : Texture));
            if (Texture != null)
            {
                Texture.Update();
                Root.Engine.Set_Texture(Texture);
            }
            Root.Engine.Fill_Rect(Box.Replaced.Get_Rect());
            Root.Engine.Set_Texture(null);
        }

        internal override void Draw_Debug_Bounds()
        {
            base.Draw_Debug_Bounds();
            Root.Engine.Set_Color(1f, 0f, 1f, 1f);// Purple
            Root.Engine.Draw_Rect(1, Box.Replaced.Get_Rect());
        }
        #endregion

        #region Texture Updating
        protected virtual void Update_Texture()
        {
            Dirty_Texture = false;
        }
        #endregion

    }
}
