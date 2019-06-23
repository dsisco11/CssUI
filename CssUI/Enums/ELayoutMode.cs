using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI
{
    public enum ELayoutMode
    {
        /// <summary>No layout</summary>
        None,
        /// <summary>Box-model</summary>
        Default,
        /// <summary>Every item treated as if it were block-level</summary>
        Stack,
    }
}
