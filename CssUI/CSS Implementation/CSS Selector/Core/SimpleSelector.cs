using CssUI.DOM;
using CssUI.DOM.Nodes;
using System.Runtime.CompilerServices;

namespace CssUI.CSS.Selectors
{
    /// <summary>
    /// A simple selector is either a type selector, universal selector, attribute selector, class selector, ID selector, or pseudo-class.
    /// </summary>
    public abstract class SimpleSelector
    {
        public ESimpleSelectorType Type { get; protected set; }

        public SimpleSelector(ESimpleSelectorType Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// Returns whether the selector matches a specified element or index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        abstract public bool Matches(Element E, params Node[] scopeElements);
    }
}
