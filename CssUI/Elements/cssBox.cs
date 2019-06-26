
namespace CssUI
{
    /// <summary>
    /// Represents a box-control that is used for rendering a solid color.
    /// </summary>
    public sealed class cssBox : cssElement
    {
        public override string Default_CSS_TypeName { get { return "Box"; } }

        #region Constuctors
        public cssBox(IParentElement Parent, string ID = null) : base(Parent, ID)
        {
            Color = new cssColor(1f, 0f, 1f, 1f);
        }
        #endregion

        #region Drawing
        protected override void Draw()
        {
            if (Color != null)
            {
                Root.Engine.Set_Color(Color);
                Root.Engine.Fill_Rect(Block_Content);
            }
        }
        #endregion
    }
}
