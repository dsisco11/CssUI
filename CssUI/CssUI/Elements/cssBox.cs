using CssUI.DOM;
using CssUI.Rendering;

namespace CssUI
{
    /// <summary>
    /// Represents a box-control that is used for rendering a solid color.
    /// </summary>
    public sealed class cssBox : cssElement
    {
        public static new readonly string CssTagName = "Rect";

        #region Constuctors
        public cssBox(Document document, IParentElement Parent, string className = null, string ID = null) : base(document, Parent, className, ID)
        {
            Color = new Color(1f, 0f, 1f, 1f);
        }
        #endregion

        #region Drawing
        protected override void Draw()
        {
            if (Color != null)
            {
                Root.Engine.Set_Color(Color);
                Root.Engine.Fill_Rect(Box.Content.Edge);
            }
        }
        #endregion
    }
}
