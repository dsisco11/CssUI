using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Just a generic block-level panel element which can have other elements added to it.
    /// </summary>
    public class uiPanel : ContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Panel"; } }

        #region Constructors
        public uiPanel(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
