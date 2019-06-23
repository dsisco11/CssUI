using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public class uiHScrollBar : uiScrollBar
    {
        public override string Default_CSS_TypeName { get { return "HorzScrollBar"; } }

        #region Constructors
        public uiHScrollBar(string Name = "X_Scrollbar") : base(Name, ESliderDirection.Horizontal)
        {
        }
        #endregion
    }
}
