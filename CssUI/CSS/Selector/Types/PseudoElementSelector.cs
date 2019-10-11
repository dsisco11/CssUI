using CssUI.DOM;
using CssUI.DOM.Nodes;
using System;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
    public class PseudoElementSelector : SimpleSelector
    {
        protected readonly string Name;

        public PseudoElementSelector(string Name) : base(ESimpleSelectorType.PseudoElementSelector)
        {
            this.Name = Name;
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
                    throw new NotImplementedException($"Selector pseudo-element ({Name}) logic not implemented!");
            }
        }
    }
}
