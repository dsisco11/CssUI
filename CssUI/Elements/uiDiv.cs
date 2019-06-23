using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Acts like a DIV element in HTML
    /// </summary>
    public class uiDiv : ContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Div"; } }

        #region Constructors
        public uiDiv(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.BLOCK);
            Layout = ELayoutMode.Default;
        }
        #endregion
    }
}
