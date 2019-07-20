using System;
using System.Linq;
using CssUI.CSS;

namespace CssUI
{
    /// <summary>
    /// Holds a function name and a set of <see cref="CssValue"/> arguments
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
        public readonly CssValue[] Args = null;
        #endregion

        public StyleFunction(AtomicString Name, params CssValue[] Args)
        {
            this.Name = Name;
            this.Args = Args.ToArray();
        }
    }
}
