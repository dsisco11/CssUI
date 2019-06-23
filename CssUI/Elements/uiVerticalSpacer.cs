using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    /// <summary>
    /// Represents a UI element which acts like a div or block spacer in HTML which occupys an entire row by itsself and eats up vertical space
    /// </summary>
    public class uiVerticalSpacer : uiElement
    {
        public override string Default_CSS_TypeName { get { return "VerticalSpacer"; } }

        #region Constructors
        public uiVerticalSpacer(string ID = null) : base(ID)
        {
            Style.User.Display.Set(EDisplayMode.BLOCK);
            Style.User.BoxSizing.Set(EBoxSizingMode.CONTENT);
        }
        #endregion
    }
}
