using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Represents a box-control that is used for rendering a solid color.
    /// </summary>
    public sealed class uiBox : uiElement
    {
        public override string Default_CSS_TypeName { get { return "Box"; } }

        #region Constuctors
        public uiBox(string ID = null) : base(ID)
        {
            Color = new uiColor(1f, 0f, 1f, 1f);
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
