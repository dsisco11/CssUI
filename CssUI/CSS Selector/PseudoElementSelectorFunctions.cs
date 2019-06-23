using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CssUI.CSS
{
    public class CssPseudoElementSelectorFunction : CssPseudoElementSelector
    {
        protected readonly List<CssToken> Args;

        public CssPseudoElementSelectorFunction(string Name, List<CssToken> Args = null) : base(Name)
        {
            this.Args = Args;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        override public bool Matches(cssElement E)
        {
            switch (Name)
            {
                default:
                    throw new CssSelectorError("Selector pseudo-element function (", Name, ") logic not implemented!");
            }
        }
    }

    static class PseudoElementFunctions
    {
    }
}
