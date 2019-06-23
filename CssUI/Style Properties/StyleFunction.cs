using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Holds a function name and a set of <see cref="CSSValue"/> arguments
    /// </summary>
    public class StyleFunction
    {
        #region Properties
        /// <summary>
        /// Function name
        /// </summary>
        public readonly AtomicString Name = null;
        /// <summary>
        /// Arguments
        /// </summary>
        public readonly CSSValue[] Args = null;
        #endregion

        public StyleFunction(AtomicString Name, params CSSValue[] Args)
        {
            this.Name = Name;
            this.Args = Args.ToArray();
        }
    }
}
