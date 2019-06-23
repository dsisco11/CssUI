using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public enum ESelectorCombinator
    {// SEE:  
        None = 0,
        /// <summary>
        /// '>>' or ' '
        /// </summary>
        Descendant,
        /// <summary>
        /// '>'
        /// </summary>
        Child,
        /// <summary>
        /// '+'
        /// </summary>
        Sibling_Adjacent,
        /// <summary>
        /// '~'
        /// </summary>
        Sibling_General,
    }
}
