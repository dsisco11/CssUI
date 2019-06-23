using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.Layout
{
    /// <summary>
    /// Holds a <see cref="uiElement"/> and it's proposed layout position
    /// </summary>
    public class LineBox_Element
    {
        public ePos Pos;
        public uiElement Element;

        public LineBox_Element(uiElement Element, ePos Pos)
        {
            this.Pos = Pos;
            this.Element = Element;
        }
    }
}
