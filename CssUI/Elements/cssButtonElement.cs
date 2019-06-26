using System.IO;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Determines the position of an image within a UI element
    /// </summary>
    public enum EImagePlacement
    {
        /// <summary>Image is positioned preceeding text content</summary>
        BeforeText,
        /// <summary>Image is positioned following text content</summary>
        AfterText,
    }

    /// <summary>
    /// Represents a UI element which functions as a clickable button and can have an image, text, or both
    /// </summary>
    public class cssButtonElement : cssScrollableElement, IGenericGraphic, ITextElement
    {
        public override string Default_CSS_TypeName { get { return "Button"; } }
        public EReplacedElementType ImageType { get { return (gfx!=null ? gfx.Kind : EReplacedElementType.NONE); } }

        #region Components
        cssTextElement txt;
        cssReplacedElement gfx;
        #endregion

        #region Text Accessors
        public string Text { get { return txt?.Text; } set { ensure_txt(); txt.Text = value; } }
        //public Font TextFont { get { return txt?.TextFont; } set { ensure_txt(); txt.TextFont = value; } }

        /// <summary>
        /// Ensures that we have an image component created
        /// </summary>
        void ensure_txt()
        {
            if (txt == null)
            {
                txt = new cssTextElement(this);
                update_component_order();
            }
        }
        #endregion

        #region Image Accessors
        /*
        public uiImage Image
        {
            get
            {
                if (gfx.Kind != EReplacedElementType.IMAGE) return null;
                return ((uiImage)gfx)?;
            }
            set
            {
                ensure_img();
                ((uiImage)gfx).Image = value;
            }
        }
        */
        public EImagePlacement ImagePlacement { get { return image_placement; } set { image_placement = value; update_component_order(); } }
        EImagePlacement image_placement = EImagePlacement.BeforeText;

        /// <summary>
        /// Ensures that we have an image component created
        /// </summary>
        void ensure_img()
        {
            if (gfx != null && gfx.Kind != EReplacedElementType.IMAGE) { Remove(gfx); gfx = null; }
            if (gfx == null)
            {
                gfx = new cssImageElement(this);
                gfx.Style.ImplicitRules.Set_SizeMax_Implicit(CssValue.Pct_OneHundred, CssValue.Pct_OneHundred);
                update_component_order();
            }
        }
        #endregion

        #region Svg Accessors

        /// <summary>
        /// Sets the SVG by parsing Svg markup from a string
        /// </summary>
        public void Set_Svg(string markup)
        {
            ensure_svg();
            ((cssSvgElement)gfx).Set_SVG(markup);
        }

        /// <summary>
        /// Sets the SVG by parsing Svg markup from a string
        /// </summary>
        public void Set_Svg(byte[] bytes)
        {
            ensure_svg();
            ((cssSvgElement)gfx).Set_SVG(bytes);
        }

        /// <summary>
        /// Sets the SVG by parsing Svg markup from a string
        /// </summary>
        public void Set_Svg(Stream stream)
        {
            ensure_svg();
            ((cssSvgElement)gfx).Set_SVG(stream);
        }

        public void Clear_Svg()
        {
            ensure_svg();
            ((cssSvgElement)gfx).Clear_SVG();
        }

        /// <summary>
        /// Ensures that we have an SVG component created
        /// </summary>
        void ensure_svg()
        {
            if (gfx!=null && gfx.Kind != EReplacedElementType.SVG) { Remove(gfx); gfx = null; }
            if (gfx == null)
            {
                gfx = new cssSvgElement(this);
                gfx.Style.ImplicitRules.Set_SizeMax_Implicit(CssValue.Pct_OneHundred, CssValue.Pct_OneHundred);
                update_component_order();
            }
        }
        #endregion


        /// <summary>
        /// Updates the order in which our image and text components are arranged
        /// </summary>
        protected void update_component_order()
        {
            //txt?.Clear_Pos();
            //gfx?.Clear_Pos();

            if (gfx != null && txt != null)
            {
                switch (image_placement)
                {
                    case EImagePlacement.BeforeText:
                        {
                            //gfx.alignLeftSide();
                            //txt.moveRightOf(gfx);
                        }
                        break;
                    case EImagePlacement.AfterText:
                        {
                            //txt.alignLeftSide();
                            //gfx.moveRightOf(txt);
                        }
                        break;
                }
            }
        }
        
        #region Constructors
        public cssButtonElement(IParentElement Parent, string ID = null) : base(Parent, ID)
        {
            Flags_Remove(EElementFlags.DoubleClickable);// Button elements cannot process double click events, we just want normal click events instead.
        }
        #endregion

        /*
        #region Click logic

        /// <summary>
        /// Called whenever this button is clicked, this is used to assign an actual purpose to the button
        /// <para>This exists so that complex UI elements which implement a button as a sub-component do not have to hook into the buttons <see cref="uiElement.onClick"/> event and thus prevent external code from being able to cancel events.</para>
        /// </summary>
        public Action Function = delegate { };
        
        public override void Handle_Click(uiElement Sender)
        {
            base.Handle_Click(Sender);
            Function();
        }
        #endregion
        */
    }
}
