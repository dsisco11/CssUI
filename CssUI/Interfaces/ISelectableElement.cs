using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    interface ISelectableElement
    {
        /// <summary>
        /// Whether this element is currently selected
        /// </summary>
        bool Selected { get; }
        /// <summary>
        /// Toggle the selection state
        /// </summary>
        void Select();
    }
}
