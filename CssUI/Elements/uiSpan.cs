using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Acts like a SPAN element in HTML
    /// </summary>
    public class uiSpan : ContainerElement
    {
        public override string Default_CSS_TypeName { get { return "Span"; } }

        #region Constructors
        public uiSpan(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.INLINE);
            Layout = ELayoutMode.None;
        }
        #endregion
    }
}
