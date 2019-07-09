using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
    public class CssPseudoElementSelectorFunction : PseudoElementSelector
    {
        protected readonly List<CssToken> Args;

        public CssPseudoElementSelectorFunction(string Name, List<CssToken> Args = null) : base(Name)
        {
            this.Args = Args;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        override public bool Matches(Element E, params Node[] scopeElements)
        {
            switch (Name)
            {
                default:
                    throw new NotImplementedException($"Selector pseudo-element function ({Name}) logic not implemented!");
            }
        }
    }

    static class PseudoElementFunctions
    {
    }
}
