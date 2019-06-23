using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class uiVScrollBar : uiScrollBar
    {
        public override string Default_CSS_TypeName { get { return "VertScrollBar"; } }

        #region Constructors
        public uiVScrollBar(string Name = "Y_Scrollbar") : base(Name, ESliderDirection.Vertical)
        {
        }
        #endregion
    }
}
